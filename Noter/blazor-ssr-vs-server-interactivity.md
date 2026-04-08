# Blazor SSR vs. Blazor Server Interactivity i .NET 10

## En introduktion for begyndere

> Denne artikel henvender sig til studerende, der er nye i Blazor og .NET 10. Vi gennemgår de to mest brugte server-side renderingsformer i Blazor: **Static SSR** og **Interactive Server**. Du lærer, hvad forskellen er, hvornår du bruger hvad, og hvordan du kommer i gang.
>
> .NET 10 er en **Long-Term Support (LTS)** release, som Microsoft understøtter i tre år. Det er den anbefalede version til nye projekter i 2026.

---

## Hvad er Blazor?

Blazor er Microsofts komponent-baserede web-framework, der lader dig bygge web-applikationer med C# og Razor-syntaks i stedet for JavaScript. Siden .NET 8 har Blazor haft en model, hvor du kan **vælge renderingsform pr. komponent** frem for at låse hele applikationen til én bestemt tilgang. .NET 10 bygger videre på denne model med forbedret tilstandshåndtering, bedre reconnection-oplevelse og nye værktøjer til fejlhåndtering.

Det betyder, at du i samme applikation kan have sider, der blot viser statisk indhold, side om side med komponenter, der reagerer på brugerinteraktion i realtid.

---

## De to tilgange: Et overblik

| | **Static SSR** | **Interactive Server** |
|---|---|---|
| **Rendering** | HTML genereres på serveren og sendes som færdig HTML til browseren | Komponenten renderes på serveren, men en SignalR-forbindelse holder den "levende" |
| **Interaktivitet** | Ingen — knapper, `@onclick` osv. virker **ikke** | Fuld — alle event-handlers (`@onclick`, `@onchange` osv.) virker |
| **Forbindelse** | Klassisk HTTP request/response (som MVC eller Razor Pages) | Vedvarende WebSocket via SignalR |
| **Performance** | Meget hurtig — ingen overhead fra SignalR eller WebAssembly | Lidt langsommere opstart pga. SignalR-forbindelse |
| **SEO** | Fremragende — ren HTML | God (med prerendering), men kræver ekstra opsætning |
| **Ressourceforbrug** | Lavt — ingen servertilstand pr. bruger | Højere — serveren holder tilstand for hver aktiv bruger |
| **Typisk brug** | Informationssider, blogs, produktkataloger, dashboards med skrivebeskyttet data | Formularer med validering, realtidsopdateringer, interaktive tabeller, chat |

---

## Static SSR — det simple udgangspunkt

Static SSR er **standardtilstanden** i Blazor Web App-skabelonen. Når du opretter et nyt Blazor Web App-projekt i .NET 10, bruger alle komponenter automatisk Static SSR, medmindre du eksplicit angiver noget andet.

### Sådan fungerer det

1. Browseren sender en HTTP-request til serveren.
2. Serveren kører din komponents C#-kode (f.eks. henter data fra en database).
3. Serveren genererer HTML ud fra din Razor-komponent.
4. HTML'en sendes til browseren, som viser den.

Det er den klassiske request/response-model — præcis som webben altid har fungeret. Der er ingen vedvarende forbindelse mellem browser og server bagefter.

### Et simpelt eksempel

```razor
@page "/produkter"

<h1>Vores produkter</h1>

@if (produkter is null)
{
    <p>Indlæser...</p>
}
else
{
    @foreach (var produkt in produkter)
    {
        <div class="produkt-card">
            <h3>@produkt.Navn</h3>
            <p>Pris: @produkt.Pris kr.</p>
        </div>
    }
}

@code {
    private List<Produkt>? produkter;

    protected override async Task OnInitializedAsync()
    {
        produkter = await ProduktService.HentAlleAsync();
    }
}
```

Denne komponent henter data og viser den. Der er ingen brugerinteraktion — og det behøver der heller ikke at være. Siden fungerer fint som Static SSR.

### Hvad virker IKKE i Static SSR?

Alt, der kræver, at serveren reagerer på brugerhandlinger i browseren:

```razor
<!-- DETTE VIRKER IKKE i Static SSR! -->
<button @onclick="StigTæller">Klik mig</button>
<p>Antal klik: @tæller</p>

@code {
    private int tæller = 0;

    private void StigTæller()
    {
        tæller++;
    }
}
```

Knappen vises i browseren, men der sker ingenting, når du klikker. `@onclick` kræver en aktiv forbindelse til serveren, som Static SSR ikke har.

### Hvad virker SÅ i Static SSR?

Selvom du ikke kan bruge `@onclick`, kan du stadig håndtere **formularer** via HTTP POST — præcis som i klassisk webudvikling:

```razor
@page "/kontakt"

<EditForm Model="besked" FormName="kontakt-form" method="post"
          OnValidSubmit="HandleSubmit" Enhance>
    <DataAnnotationsValidator />

    <label>Dit navn:</label>
    <InputText @bind-Value="besked.Navn" />

    <label>Din besked:</label>
    <InputTextArea @bind-Value="besked.Tekst" />

    <button type="submit">Send</button>
</EditForm>

@code {
    [SupplyParameterFromForm]
    private KontaktBesked besked { get; set; } = new();

    private async Task HandleSubmit()
    {
        // Gem beskeden i databasen
        await BeskedService.GemAsync(besked);
    }
}
```

Læg mærke til tre ting:

- **`FormName`** — hvert formular i Static SSR skal have et unikt navn.
- **`[SupplyParameterFromForm]`** — denne attribut binder de postede formdata til din model.
- **`Enhance`** — giver en SPA-lignende oplevelse ved at bruge `fetch` i stedet for en fuld sidegenindlæsning.

> **Bemærk:** Klient-side validering (f.eks. realtids-feedback mens brugeren skriver) er ikke tilgængelig i Static SSR. Validering sker først, når formularen postes til serveren. Har du brug for øjeblikkelig validering, skal du bruge Interactive Server.

---

## Interactive Server — når du har brug for interaktivitet

Interactive Server (tidligere kendt som "Blazor Server") opretter en vedvarende SignalR-forbindelse (WebSocket) mellem browseren og serveren. Det betyder, at serveren kan reagere på brugerhandlinger i realtid.

### Sådan aktiverer du det

Du tilføjer `@rendermode InteractiveServer` til din komponent:

```razor
@page "/tæller"
@rendermode InteractiveServer

<h1>Interaktiv tæller</h1>

<button @onclick="StigTæller">Klik mig</button>
<p>Antal klik: @tæller</p>

@code {
    private int tæller = 0;

    private void StigTæller()
    {
        tæller++;
    }
}
```

Nu virker `@onclick`, fordi SignalR-forbindelsen sender klik-eventet til serveren, som opdaterer tilstanden og sender den ændrede HTML (en "diff") tilbage til browseren.

### Hvad sker der under motorhjelmen?

1. Browseren loader siden (evt. med prerendering som Static SSR).
2. `blazor.web.js` opretter en SignalR WebSocket-forbindelse.
3. Når brugeren klikker en knap, sendes eventet over WebSocket til serveren.
4. Serveren kører event-handleren, genberegner komponentens tilstand og sender en DOM-diff tilbage.
5. Browseren patcher DOM'en med de nye ændringer.

Det hele sker så hurtigt, at det føles som en lokal applikation — forudsat at netværksforbindelsen er god.

> **Nyt i .NET 10:** `blazor.web.js` leveres nu som et statisk asset med automatisk komprimering og fingerprinting, hvilket giver en reduktion i filstørrelse på ca. 76 % (fra ~183 KB til ~43 KB). Det betyder hurtigere indlæsning og færre problemer med forældede cache-filer.

---

## Opsætning i `Program.cs`

For at kunne bruge Interactive Server-komponenter skal din applikation konfigureres korrekt:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Tilføj Razor-komponent-services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(); // Aktiverer Interactive Server

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

// Map komponenterne og tilføj Interactive Server-rendermode
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode(); // Aktiverer Interactive Server-endpoints

app.Run();
```

Uden disse to linjer (`.AddInteractiveServerComponents()` og `.AddInteractiveServerRenderMode()`) vil `@rendermode InteractiveServer` ikke virke.

---

## Hvornår bruger du hvad?

### Brug Static SSR når:

- Siden primært **viser data** (produktlister, artikler, dashboards).
- Du vil have **hurtig indlæsning** og god SEO.
- Du vil holde **serverressourcer lave** (ingen SignalR-forbindelse pr. bruger).
- Formularer kan håndteres med klassisk HTTP POST (via `EditForm` med `FormName`).

### Brug Interactive Server når:

- Du har brug for **realtidsinteraktion** (knapper, drag-and-drop, live-søgning).
- Komponenten skal **reagere på events** uden sidegenindlæsning.
- Du har brug for **tovejs-databinding** (`@bind`) med øjeblikkelig feedback.
- Du bygger **komplekse formularer** med dynamisk validering eller betinget visning.

### Kombiner dem — "Islands of Interactivity"

Det bedste ved Blazors renderingsmodel er, at du kan **mikse render modes** i samme applikation. En produktside kan bruge Static SSR for selve produktbeskrivelsen, mens en "Tilføj til kurv"-komponent bruger Interactive Server:

```razor
@page "/produkt/{Id:int}"

<!-- Denne del er Static SSR (standard) -->
<h1>@produkt.Navn</h1>
<p>@produkt.Beskrivelse</p>
<p>Pris: @produkt.Pris kr.</p>

<!-- Denne komponent er interaktiv -->
<TilføjTilKurv ProduktId="@Id" />
```

Hvor `TilføjTilKurv.razor` er:

```razor
@rendermode InteractiveServer

<button @onclick="Tilføj">Tilføj til kurv</button>
<p>@besked</p>

@code {
    [Parameter] public int ProduktId { get; set; }
    private string besked = "";

    private async Task Tilføj()
    {
        await KurvService.TilføjAsync(ProduktId);
        besked = "Tilføjet!";
    }
}
```

Denne tilgang kaldes **"islands of interactivity"** — øer af interaktivitet i et hav af statisk indhold. Du får det bedste fra begge verdener: hurtig indlæsning og interaktivitet, præcis der hvor det behøves.

---

## Streaming Rendering — en mellemvej

Der findes en mellemvej mellem Static SSR og fuld interaktivitet: **Streaming Rendering**. Det er stadig Static SSR (ingen SignalR), men serveren kan sende HTML i etaper:

```razor
@page "/rapport"
@attribute [StreamRendering]

<h1>Salgsrapport</h1>

@if (data is null)
{
    <p>Henter data...</p>
}
else
{
    <table>
        @foreach (var linje in data)
        {
            <tr>
                <td>@linje.Produkt</td>
                <td>@linje.Beløb kr.</td>
            </tr>
        }
    </table>
}

@code {
    private List<SalgsLinje>? data;

    protected override async Task OnInitializedAsync()
    {
        // Simuler en langsom databaseforespørgsel
        data = await SalgsService.HentRapportAsync();
    }
}
```

Med `[StreamRendering]` viser browseren straks "Henter data...", og når databasen svarer, streames den opdaterede HTML ned og patches ind i siden — alt sammen inden for det samme HTTP-response, uden nogen SignalR-forbindelse.

---

## Nyt i .NET 10: Forbedringer du skal kende

.NET 10 introducerer flere vigtige forbedringer, der påvirker arbejdet med render modes.

### 1. `[PersistentState]` — slut med flimrende data

Et klassisk irritationsmoment med Interactive Server og prerendering har været, at data "flimrer": komponenten renderes først statisk med data, så forsvinder dataen kortvarigt, og så dukker den op igen, fordi komponenten initialiseres en gang til, når SignalR-forbindelsen er klar.

.NET 10 løser dette med den nye `[PersistentState]`-attribut:

```razor
@page "/patienter"
@rendermode InteractiveServer

<h1>Patientliste</h1>

@if (Patienter is null)
{
    <p>Indlæser...</p>
}
else
{
    @foreach (var patient in Patienter)
    {
        <p>@patient.Navn — @patient.Diagnose</p>
    }
}

@code {
    [PersistentState]
    public List<Patient>? Patienter { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (Patienter is null)
        {
            Patienter = await PatientService.HentAlleAsync();
        }
    }
}
```

Sådan fungerer det: Under prerendering serialiserer Blazor den data, der er markeret med `[PersistentState]`, og indlejrer den i den HTML, der sendes til browseren. Når komponenten så skifter til interaktiv tilstand, genbruger Blazor den gemte data i stedet for at hente den igen. Resultatet er en glat overgang uden flimmer.

> **Tip:** Brug `[PersistentState(AllowUpdates = true)]` hvis du navigerer mellem sider internt i appen og vil bevare data på tværs af navigation.

`[PersistentState]` løser også et andet problem: Hvis en bruger mister sin SignalR-forbindelse (f.eks. fordi de går væk fra computeren), vil Blazor gemme tilstanden, når circuitet frigives. Når brugeren vender tilbage og reconnector, gendannes tilstanden automatisk i et nyt circuit. Det giver en langt mere robust brugeroplevelse.

### 2. `NavigationManager.NotFound()` — korrekt 404-håndtering

Tidligere var det besværligt at returnere en korrekt HTTP 404-statuskode, når en ressource ikke fandtes. .NET 10 giver en simpel løsning med `NotFound()` på `NavigationManager`, som virker ens på tværs af alle render modes:

```razor
@page "/patient/{Id:int}"
@rendermode InteractiveServer
@inject NavigationManager Navigation

<h1>@patient?.Navn</h1>

@code {
    [Parameter] public int Id { get; set; }
    private Patient? patient;

    protected override async Task OnInitializedAsync()
    {
        patient = await PatientService.FindAsync(Id);

        if (patient is null)
        {
            Navigation.NotFound();
        }
    }
}
```

Nye Blazor Web App-projekter i .NET 10 inkluderer automatisk en `NotFound.razor`-side, der vises, når `NotFound()` kaldes.

### 3. Forbedret reconnection-oplevelse

Blazor Web App-skabelonen i .NET 10 inkluderer en ny `ReconnectModal`-komponent, der giver udviklere bedre kontrol over, hvad brugeren ser, når SignalR-forbindelsen mistes. Den nye komponent respekterer Content Security Policy (CSP) og kan tilpasses med CSS og JavaScript.

Desuden giver det nye `components-reconnect-state-changed`-event mulighed for at reagere programmatisk på forbindelsesændringer.

### 4. Forbedret JavaScript Interop

.NET 10 udvider JavaScript-interop med nye muligheder, der er særligt nyttige i Interactive Server-komponenter:

```csharp
// Opret en instans af et JS-objekt
var objRef = await JS.InvokeConstructorAsync("MyJsClass", "parameter1");

// Læs en property fra et JS-objekt
var count = await JS.GetValueAsync<int>("myObj.count");
```

Disse API'er gør det enklere at arbejde med JavaScript-biblioteker fra dine Blazor-komponenter.

### 5. Forbedret validering

.NET 10 erstatter den refleksionsbaserede validering med **Source Generators**, som genererer valideringskode på kompileringstidspunktet. Det giver bedre performance og er kompatibelt med Native AOT. Derudover understøttes nu validering af **indlejrede objekter og collections**:

```csharp
[ValidatableType]
public class PatientRegistrering
{
    [Required]
    public string Navn { get; set; } = "";

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    public Adresse Adresse { get; set; } = new();
}

public class Adresse
{
    [Required]
    public string Gade { get; set; } = "";

    [Required]
    public string By { get; set; } = "";
}
```

Med `[ValidatableType]` kan Blazor validere hele objektgrafen — ikke kun top-level-properties.

---

## Typiske faldgruber for begyndere

**1. "Min knap virker ikke!"**
Har du husket `@rendermode InteractiveServer`? Uden den er komponenten Static SSR, og events ignoreres.

**2. "Siden flimrer ved indlæsning"**
Det skyldes sandsynligvis **prerendering** — komponenten renderes to gange (først statisk, så interaktivt). Brug `[PersistentState]` til at bevare data mellem de to renderinger. Hvis du bare vil teste hurtigt, kan du slå prerendering fra: `@rendermode @(new InteractiveServerRenderMode(prerender: false))`, men det anbefales ikke i produktion.

**3. "Jeg kan ikke bruge `HttpContext` i min Interactive Server-komponent"**
`HttpContext` er kun tilgængelig under den initielle HTTP-request (Static SSR og prerendering). Når SignalR-forbindelsen er aktiv, er der ingen HTTP-request, og `HttpContext` er `null`. Brug i stedet `[CascadingParameter]` under prerendering eller flyt logikken til en service.

**4. "Min formular kræver `FormName`, men det kender jeg ikke fra Blazor Server i .NET 7"**
`FormName` og `[SupplyParameterFromForm]` er kun nødvendige for formularer i Static SSR. Interactive Server-formularer fungerer som de altid har gjort med `@bind-Value` og `@onclick`.

**5. "Min 404-side virker ikke korrekt"**
I .NET 10 skal du bruge `NavigationManager.NotFound()` i stedet for at forsøge at manipulere HTTP-statuskoden manuelt. Det virker på tværs af alle render modes.

---

## Opsummering

| Spørgsmål | Static SSR | Interactive Server |
|---|---|---|
| Kræver SignalR? | Nej | Ja |
| `@onclick` virker? | Nej | Ja |
| Formularer? | Ja (via HTTP POST) | Ja (via SignalR) |
| Streaming? | Ja (med `[StreamRendering]`) | Ikke relevant |
| Servertilstand pr. bruger? | Nej | Ja |
| God til SEO? | Ja | Ja (med prerendering) |
| `[PersistentState]`? | Ikke relevant | Ja — løser flimmer og reconnection |
| `NavigationManager.NotFound()`? | Ja | Ja |
| Standard i .NET 10? | Ja | Nej (skal aktiveres) |

**Tommelfingerregel:** Start med Static SSR. Tilføj interaktivitet kun der, hvor du faktisk har brug for det. Brug `[PersistentState]` til at undgå flimmer i dine interaktive komponenter. Denne tilgang giver den bedste performance og det laveste ressourceforbrug.

---

## Videre læsning

- [What's new in ASP.NET Core in .NET 10 (Microsoft Learn)](https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-10.0)
- [ASP.NET Core Blazor render modes (Microsoft Learn)](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/render-modes)
- [Blazor prerendered state persistence (Microsoft Learn)](https://learn.microsoft.com/en-us/aspnet/core/blazor/state-management/prerendered-state-persistence)
- [Blazor Basics: Should You Migrate to .NET 10? (Telerik)](https://www.telerik.com/blogs/blazor-basics-should-you-migrate-net-10)
