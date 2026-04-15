# Opgave 4 — Blazor-frontend til MinKlinik

**Tema:** Blazor SSR som UI-lag oven på eksisterende Clean Architecture
**Varighed:** 2 lektioner (≈ 4 timer)
**Render mode:** `InteractiveServer`
**Forudsætning:** Opgave 1–3 + MinKlinik-solution fra Clean Architecture-modulet ([Link](https://github.com/kbr-ucl/2026F-DMVE251-2-sem/tree/main/DemoKode/MinKlinik))

---

## Læringsmål

Efter denne opgave kan du:

- Tilføje et Blazor Server-projekt til en eksisterende Clean Architecture Solution
- Registrere Facade-, UseCase- og Infrastructure-services i `Program.cs`
- Bruge `@inject` til Query-interfaces i Razor-komponenter
- Kalde Command Use Cases fra `EditForm` med korrekt fejlhåndtering af `DomainException`
- Implementere fuld CRUD-flow: **oversigt, opret, afslut (med notat), aflys**
- Navigere mellem sider med `NavigationManager` og parameter-routing

---

## Arkitektur-oversigt

```
MinKlinik.slnx
├── src/
│   ├── MinKlinik.Domain/            (eksisterende)
│   ├── MinKlinik.Facade/            (eksisterende — bruges af Blazor)
│   ├── MinKlinik.UseCases/          (eksisterende)
│   ├── MinKlinik.Infrastructure/    (eksisterende)
│   ├── MinKlinik.Api/               (eksisterende — røres ikke)
│   ├── MinKlinik.Console/           (eksisterende — røres ikke)
│   └── MinKlinik.Blazor/            ← NYT PROJEKT
│       ├── Components/
│       │   ├── Pages/
│       │   │   ├── Konsultationer.razor      (oversigt)
│       │   │   ├── OpretKonsultation.razor
│       │   │   └── KonsultationDetalje.razor (afslut + aflys)
│       │   ├── Shared/
│       │   │   ├── KonsultationTabel.razor
│       │   │   ├── StatusBadge.razor
│       │   │   └── BekraeftDialog.razor
│       │   ├── App.razor
│       │   ├── Routes.razor
│       │   └── _Imports.razor
│       └── Program.cs
```

> **Dependency Rule:** `Blazor` refererer kun til `Facade` og `Infrastructure` (sidstnævnte kun for DI-registrering via extension method). Dette svarer præcis til hvordan `MinKlinik.Api` allerede er opbygget.

---

## Trin 1 — Opret Blazor-projektet

Fra solution-rodmappen:

```bash
dotnet new blazor -n MinKlinik.Blazor -o src/MinKlinik.Blazor --interactivity Server --empty
dotnet sln MinKlinik.slnx add src/MinKlinik.Blazor/MinKlinik.Blazor.csproj

# Projektreferencer (afspejler Api-projektet)
dotnet add src/MinKlinik.Blazor/MinKlinik.Blazor.csproj reference src/MinKlinik.Facade/MinKlinik.Facade.csproj
dotnet add src/MinKlinik.Blazor/MinKlinik.Blazor.csproj reference src/MinKlinik.Infrastructure/MinKlinik.Infrastructure.csproj
```

**Hint:** Åbn `MinKlinik.Api/Program.cs` og kig efter den `AddMinKlinik...`-extension method der bygger DI-grafen. Den genbruges i Blazor-projektet.

---

## Trin 2 — Konfigurer `Program.cs`

```csharp
using MinKlinik.Blazor.Components;
// TODO: using før Facade + Infrastructure extension methods

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// TODO: Genbrug samme DI-extension som Api-projektet bruger,
//       fx builder.Services.AddMinKlinikInfrastructure(builder.Configuration);
//       (navnet findes i MinKlinik.Infrastructure — kig i Api/Program.cs)

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// TODO: Seed database hvis relevant — kopier logikken fra Api/Program.cs

app.Run();
```

**Vigtigt:** Kopier `appsettings.json` (connection string) fra `MinKlinik.Api` til `MinKlinik.Blazor`.

---

## Trin 3 — `_Imports.razor`

```razor
@using System.Net.Http
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using static Microsoft.AspNetCore.Components.Web.RenderMode
@using Microsoft.AspNetCore.Components.Web.Virtualization
@using Microsoft.JSInterop

@using MinKlinik.Blazor
@using MinKlinik.Blazor.Components
@using MinKlinik.Blazor.Components.Shared
@using MinKlinik.Facade.DTOs
@using MinKlinik.Facade.Queries
@using MinKlinik.Facade.UseCases
```

---

## Trin 4 — Oversigtsside: `Konsultationer.razor`

Viser alle konsultationer i en tabel. Filter i toppen (dato-interval + behandler).

```razor
@page "/konsultationer"
@rendermode InteractiveServer
@inject IKonsultationQueries KonsultationQueries
@inject IBehandlerQueries BehandlerQueries
@inject NavigationManager Nav

<PageTitle>Konsultationer</PageTitle>

<main>
    <header>
        <h1>Konsultationer</h1>
    </header>

    <section class="toolbar" aria-label="Filtrér og opret">
        <form role="search" @onsubmit:preventDefault>
            <label for="filter-behandler">Behandler</label>
            <select id="filter-behandler"
                    @bind="valgtBehandler" @bind:after="HentFiltreret">
                <option value="">Alle</option>
                @foreach (var b in behandlere)
                {
                    <option value="@b.Id">@b.Navn</option>
                }
            </select>
        </form>

        <button type="button" class="primary"
                @onclick='() => Nav.NavigateTo("/konsultationer/opret")'>
            + Ny konsultation
        </button>
    </section>

    <section aria-live="polite" aria-busy="@(konsultationer is null)">
        @if (konsultationer is null)
        {
            <p><em>Henter...</em></p>
        }
        else if (konsultationer.Count == 0)
        {
            <p>Ingen konsultationer fundet.</p>
        }
        else
        {
            <KonsultationTabel Konsultationer="konsultationer"
                               OnVaelg="GaaTilDetalje" />
        }
    </section>
</main>

@code {
    private IReadOnlyList<KonsultationDto>? konsultationer;
    private IReadOnlyList<BehandlerDto> behandlere = Array.Empty<BehandlerDto>();
    private string valgtBehandler = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        // TODO: Hent behandlere og alle konsultationer parallelt.
        //       Hint: var t1 = BehandlerQueries.HentAlleAsync();
        //             var t2 = KonsultationQueries.HentAlleAsync();
        //             await Task.WhenAll(t1, t2);
    }

    private async Task HentFiltreret()
    {
        // TODO: Hent alle konsultationer, og filtrér i hukommelsen på BehandlerId
        //       hvis valgtBehandler ikke er tom.
        //       (Alternativt: udvid IKonsultationQueries med en SoegAsync-metode)
    }

    private void GaaTilDetalje(Guid id) => Nav.NavigateTo($"/konsultationer/{id}");
}
```

---

## Trin 5 — Delt komponent: `KonsultationTabel.razor`

```razor
@using MinKlinik.Facade.DTOs

<table class="konsultation-tabel">
    <caption class="sr-only">Konsultationer</caption>
    <thead>
        <tr>
            <th scope="col">Tidspunkt</th>
            <th scope="col">Patient</th>
            <th scope="col">Behandler</th>
            <th scope="col">Type</th>
            <th scope="col">Status</th>
            <th scope="col"><span class="sr-only">Handlinger</span></th>
        </tr>
    </thead>
    <tbody>
        @foreach (var k in Konsultationer)
        {
            <tr @key="k.Id">
                <th scope="row">
                    <time datetime="@k.Fra.ToString("s")">
                        @k.Fra.ToString("dd-MM-yyyy HH:mm")
                    </time>
                    – @k.Til.ToString("HH:mm")
                </th>
                <td>@k.PatientNavn</td>
                <td>@k.BehandlerNavn</td>
                <td>@k.BehandlingstypeNavn</td>
                <td><StatusBadge Status="@k.Status" /></td>
                <td>
                    <button type="button" @onclick="() => OnVaelg.InvokeAsync(k.Id)">
                        Åbn
                    </button>
                </td>
            </tr>
        }
    </tbody>
</table>

@code {
    [Parameter, EditorRequired]
    public IReadOnlyList<KonsultationDto> Konsultationer { get; set; } = Array.Empty<KonsultationDto>();

    [Parameter]
    public EventCallback<Guid> OnVaelg { get; set; }
}
```

---

## Trin 6 — `StatusBadge.razor`

```razor
<span class="badge badge-@CssKlasse">@Status</span>

@code {
    [Parameter, EditorRequired]
    public string Status { get; set; } = string.Empty;

    private string CssKlasse => Status switch
    {
        // TODO: Returner CSS-modifiers "planlagt", "afsluttet", "aflyst"
        //       matchende værdierne i KonsultationStatus-enum
        _ => "ukendt"
    };
}
```

Opret matching `StatusBadge.razor.css` med farver pr. status.

---

## Trin 7 — Opret-side: `OpretKonsultation.razor`

```razor
@page "/konsultationer/opret"
@rendermode InteractiveServer
@inject IOpretKonsultationUseCase OpretUseCase
@inject IPatientQueries PatientQueries
@inject IBehandlerQueries BehandlerQueries
@inject IBehandlingstypeQueries TypeQueries
@inject NavigationManager Nav

<PageTitle>Ny konsultation</PageTitle>

<main>
    <header>
        <h1>Ny konsultation</h1>
    </header>

    <EditForm Model="model" OnValidSubmit="Gem" FormName="opret">
        <DataAnnotationsValidator />
        <ValidationSummary />

        <fieldset>
            <legend>Tidspunkt</legend>

            <div class="felt">
                <label for="fra">Fra (dato og tid)</label>
                <InputDate id="fra" Type="InputDateType.DateTimeLocal" @bind-Value="model.Fra" />
                <ValidationMessage For="() => model.Fra" />
            </div>

            <div class="felt">
                <label for="varighed">Varighed (minutter)</label>
                <InputNumber id="varighed" @bind-Value="model.VarighedMinutter" />
                <ValidationMessage For="() => model.VarighedMinutter" />
            </div>
        </fieldset>

        <fieldset>
            <legend>Parter</legend>

            @* TODO: 3 x InputSelect for Patient, Behandler og Behandlingstype
                     bundet til model.PatientId, model.BehandlerId, model.BehandlingstypeId.
                     Hver <label for="..."> skal pege på <InputSelect id="...">.
                     Første <option> bør være en tom "Vælg ..."-option. *@
        </fieldset>

        @if (!string.IsNullOrEmpty(fejl))
        {
            <p class="fejl" role="alert">@fejl</p>
        }

        <footer class="knapper">
            <button type="button" @onclick='() => Nav.NavigateTo("/konsultationer")'>Annullér</button>
            <button type="submit" class="primary" disabled="@gemmer">
                @(gemmer ? "Gemmer..." : "Opret")
            </button>
        </footer>
    </EditForm>
</main>

@code {
    private FormModel model = new();
    private IReadOnlyList<PatientDto> patienter = Array.Empty<PatientDto>();
    private IReadOnlyList<BehandlerDto> behandlere = Array.Empty<BehandlerDto>();
    private IReadOnlyList<BehandlingstypeDto> typer = Array.Empty<BehandlingstypeDto>();
    private bool gemmer;
    private string? fejl;

    protected override async Task OnInitializedAsync()
    {
        // TODO: Hent patienter, behandlere og typer parallelt
    }

    private async Task Gem()
    {
        gemmer = true;
        fejl = null;
        try
        {
            var request = new OpretKonsultationRequest(
                Fra: model.Fra,
                Til: model.Fra.AddMinutes(model.VarighedMinutter),
                BehandlingstypeId: model.BehandlingstypeId,
                PatientId: model.PatientId,
                BehandlerId: model.BehandlerId);

            // TODO: Kald OpretUseCase.Udør(request)
            // TODO: Ved succes — naviger til /konsultationer
        }
        catch (Exception ex) // TODO: Fang konkret DomainException først, så generel Exception
        {
            fejl = ex.Message;
        }
        finally
        {
            gemmer = false;
        }
    }

    private sealed class FormModel
    {
        [Required]
        public DateTime Fra { get; set; } = DateTime.Now.Date.AddHours(9);

        [Range(15, 180, ErrorMessage = "Varighed skal være mellem 15 og 180 minutter")]
        public int VarighedMinutter { get; set; } = 30;

        [Required(ErrorMessage = "Vælg patient")]
        public Guid PatientId { get; set; }

        [Required(ErrorMessage = "Vælg behandler")]
        public Guid BehandlerId { get; set; }

        [Required(ErrorMessage = "Vælg behandlingstype")]
        public Guid BehandlingstypeId { get; set; }
    }
}
```

**Hint — fejlhåndtering:** Domain-laget kaster `DomainException` ved overlap (se `Konsultation.Opret()`). Fang den eksplicit — lad alle andre exceptions boble op til Blazor's `ErrorBoundary`:

```csharp
catch (MinKlinik.Domain.Exceptions.DomainException dex)
{
    fejl = dex.Message; // brugervenlig
}
```

---

## Trin 8 — Detalje/handlings-side: `KonsultationDetalje.razor`

Viser én konsultation og tilbyder **afslut** (med notat) og **aflys** afhængigt af status.

```razor
@page "/konsultationer/{Id:guid}"
@rendermode InteractiveServer
@inject IKonsultationQueries Queries
@inject IAfslutKonsultationUseCase AfslutUseCase
@inject IAflysKonsultationUseCase AflysUseCase
@inject NavigationManager Nav

<PageTitle>Konsultation</PageTitle>

<main>
    <nav aria-label="Brødkrumme">
        <a href="/konsultationer">← Tilbage til oversigt</a>
    </nav>

    <header>
        <h1>Konsultation</h1>
    </header>

    @if (konsultation is null)
    {
        <p><em>Henter...</em></p>
    }
    else
    {
        <article aria-labelledby="detaljer-titel">
            <h2 id="detaljer-titel" class="sr-only">Detaljer</h2>

            <dl class="detaljer">
                <dt>Tidspunkt</dt>
                <dd>
                    <time datetime="@konsultation.Fra.ToString("s")">
                        @konsultation.Fra.ToString("dd-MM-yyyy HH:mm")
                    </time>
                    –
                    <time datetime="@konsultation.Til.ToString("s")">
                        @konsultation.Til.ToString("HH:mm")
                    </time>
                </dd>
                <dt>Patient</dt>        <dd>@konsultation.PatientNavn</dd>
                <dt>Behandler</dt>      <dd>@konsultation.BehandlerNavn</dd>
                <dt>Behandlingstype</dt><dd>@konsultation.BehandlingstypeNavn</dd>
                <dt>Status</dt>         <dd><StatusBadge Status="@konsultation.Status" /></dd>
                @if (!string.IsNullOrWhiteSpace(konsultation.Notat))
                {
                    <dt>Notat</dt>      <dd>@konsultation.Notat</dd>
                }
            </dl>
        </article>

        @if (konsultation.Status == "Planlagt")
        {
            <section class="handlinger" aria-labelledby="handlinger-titel">
                <h2 id="handlinger-titel">Handlinger</h2>

                <section aria-labelledby="afslut-titel">
                    <h3 id="afslut-titel">Afslut</h3>
                    <EditForm Model="afslutModel" OnValidSubmit="Afslut" FormName="afslut">
                        <DataAnnotationsValidator />
                        <label for="notat">Notat</label>
                        <InputTextArea id="notat" @bind-Value="afslutModel.Notat" rows="4"
                                       placeholder="Skriv notat..." />
                        <ValidationMessage For="() => afslutModel.Notat" />
                        <button type="submit" class="primary" disabled="@arbejder">
                            Afslut konsultation
                        </button>
                    </EditForm>
                </section>

                <section aria-labelledby="aflys-titel">
                    <h3 id="aflys-titel">Aflys</h3>
                    <button type="button" class="danger"
                            @onclick="BekraeftAflys" disabled="@arbejder">Aflys</button>
                </section>
            </section>
        }

        @if (visBekraeft)
        {
            <BekraeftDialog Titel="Aflys konsultation"
                            Besked="Er du sikker? Handlingen kan ikke fortrydes."
                            OnBekraeft="Aflys"
                            OnAnnuller="() => visBekraeft = false" />
        }

        @if (!string.IsNullOrEmpty(fejl))
        {
            <p class="fejl" role="alert">@fejl</p>
        }
    }
</main>

@code {
    [Parameter] public Guid Id { get; set; }

    private KonsultationDto? konsultation;
    private AfslutModel afslutModel = new();
    private bool visBekraeft;
    private bool arbejder;
    private string? fejl;

    protected override async Task OnParametersSetAsync()
    {
        // TODO: Hent konsultationen via Queries.HentAsync(Id)
        //       Hvis null — naviger tilbage til oversigt eller vis "ikke fundet"
    }

    private async Task Afslut()
    {
        arbejder = true; fejl = null;
        try
        {
            // TODO: Kald AfslutUseCase.Udør(new AfslutKonsultationRequest(Id, afslutModel.Notat!))
            // TODO: Genhent konsultation så ny status vises
        }
        catch (MinKlinik.Domain.Exceptions.DomainException dex) { fejl = dex.Message; }
        finally { arbejder = false; }
    }

    private void BekraeftAflys() => visBekraeft = true;

    private async Task Aflys()
    {
        visBekraeft = false;
        arbejder = true; fejl = null;
        try
        {
            // TODO: Kald AflysUseCase.Udør(new AflysKonsultationRequest(Id))
            // TODO: Genhent konsultation
        }
        catch (MinKlinik.Domain.Exceptions.DomainException dex) { fejl = dex.Message; }
        finally { arbejder = false; }
    }

    private sealed class AfslutModel
    {
        [Required(ErrorMessage = "Notat er påkrævet")]
        [StringLength(1000, MinimumLength = 3)]
        public string? Notat { get; set; }
    }
}
```

---

## Trin 9 — `BekraeftDialog.razor`

Vi bruger det native HTML5 `<dialog>`-element (med `aria-labelledby` til titlen) — det håndterer focus-trap og Escape-tast gratis.

```razor
@inject IJSRuntime JS
@implements IAsyncDisposable

<dialog @ref="dialogRef" aria-labelledby="dialog-titel-@instansId" @oncancel="Annuller">
    <article>
        <header>
            <h2 id="dialog-titel-@instansId">@Titel</h2>
        </header>

        <p>@Besked</p>

        <footer class="knapper">
            <button type="button" @onclick="Annuller">Annullér</button>
            <button type="button" class="danger" @onclick="Bekraeft">Bekræft</button>
        </footer>
    </article>
</dialog>

@code {
    private ElementReference dialogRef;
    private readonly string instansId = Guid.NewGuid().ToString("N");
    private bool vist;

    [Parameter, EditorRequired] public string Titel { get; set; } = string.Empty;
    [Parameter, EditorRequired] public string Besked { get; set; } = string.Empty;
    [Parameter] public EventCallback OnBekraeft { get; set; }
    [Parameter] public EventCallback OnAnnuller { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // TODO: Kald JS-interop for at åbne modalen:
            //       await JS.InvokeVoidAsync("HTMLDialogElement.prototype.showModal.call", dialogRef);
            //       (eller skriv en lille wrapper i wwwroot/js/dialog.js)
            vist = true;
        }
    }

    private Task Bekraeft() => OnBekraeft.InvokeAsync();
    private Task Annuller() => OnAnnuller.InvokeAsync();

    public async ValueTask DisposeAsync()
    {
        // TODO: Luk dialogen hvis den stadig er åben
        await Task.CompletedTask;
    }
}
```

**Hint — JS-interop:** `<dialog>` åbnes med `dialog.showModal()` — det giver gratis focus-trap, fokus på første interaktive element, og Escape-tast udløser `oncancel`. Lav en lille helper i `wwwroot/js/dialog.js`:

```javascript
export function visModal(element) { element.showModal(); }
export function skjulModal(element) { if (element.open) element.close(); }
```

Og importér den i `Program.cs` eller direkte via `IJSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/dialog.js")`.

---

## Trin 10 — Kør og verificér

```bash
dotnet build MinKlinik.slnx
dotnet run --project src/MinKlinik.Blazor/MinKlinik.Blazor.csproj
```

Åbn `https://localhost:<port>/konsultationer`.

---

## Acceptkriterier

**Arkitektur:**

- [ ] `MinKlinik.Blazor` refererer **kun** til `Facade` og `Infrastructure` — ikke `Domain` direkte (undtagen via `using` for `DomainException`)
- [ ] Ingen `Microsoft.EntityFrameworkCore` using statements i nogen `.razor`-fil
- [ ] DI-registrering genbruger samme extension method som `MinKlinik.Api`
- [ ] Alle `.razor`-sider har `@rendermode InteractiveServer`

**Funktionalitet:**

- [ ] `/konsultationer` viser alle konsultationer fra databasen
- [ ] Filter på behandler opdaterer tabellen
- [ ] `/konsultationer/opret` opretter en konsultation via `IOpretKonsultationUseCase`
- [ ] Overlap-forsøg vises som brugervenlig fejl (ikke crash / ikke stacktrace)
- [ ] Afslut-formular kræver notat (DataAnnotations)
- [ ] Aflys kræver bekræftelse via modal-dialog
- [ ] Efter afslut/aflys opdateres `StatusBadge` korrekt
- [ ] Handlingsknapper er kun synlige når `Status == "Planlagt"`

**Kvalitet:**

- [ ] Mindst 3 genanvendelige komponenter i `Shared/` (`KonsultationTabel`, `StatusBadge`, `BekraeftDialog`)
- [ ] Semantisk HTML5: `<main>`, `<header>`, `<nav>`, `<section>`, `<article>`, `<footer>`, `<dialog>`, `<time>`, `<fieldset>`/`<legend>`
- [ ] Alle `<label for="...">` peger på et `id` på tilhørende input
- [ ] Tabeller har `<caption>` (kan være `.sr-only`) og `<th scope="col|row">`
- [ ] Fejlbeskeder har `role="alert"`; loading-områder har `aria-busy` / `aria-live="polite"`
- [ ] Scoped CSS (`.razor.css`) — ingen globale style-konflikter
- [ ] Ingen `async void` undtagen i event handlers (og helst ikke dér heller)

---

## Udfordringer (valgfri)

1. **`ErrorBoundary`** omkring `<Routes>` der fanger alle ikke-håndterede exceptions og viser en pæn fejlside
2. **Cascading parameter** med "aktuel bruger" (behandler) så kun egne konsultationer vises
3. **Udvid `IKonsultationQueries`** med en `SoegAsync(SøgKonsultationerRequest)`-metode (kræver ændring i Facade + Infrastructure — god øvelse i CQS)
4. **Optimistisk UI**: opdater `konsultation.Status` lokalt før round-trip er færdig; rul tilbage ved fejl

---

## Refleksionsspørgsmål

1. Hvorfor må `MinKlinik.Blazor` referere `MinKlinik.Infrastructure`, når Blazor kun bruger Facade-interfaces i koden? Hvor lækker afhængigheden potentielt?
2. Hvordan ville du teste `OpretKonsultation.razor`-sidens fejlhåndtering uden at starte en rigtig database? (Tænk: mock af `IOpretKonsultationUseCase`)
3. `KonsultationDto.Status` er en `string` — hvorfor ikke enum? Hvad er fordelen ved en string på grænsen af Facade-laget?
4. Ville det være en god idé at lade `OpretKonsultation.razor` bygge sin egen `OpretKonsultationRequest` direkte fra `InputSelect`-værdier, eller skal man have en separat `FormModel` imellem? Argumentér.
