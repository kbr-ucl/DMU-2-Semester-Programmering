# Opgave 3 — EventCallback og lifecycle

**Tema:** Komponent-kommunikation, `EventCallback<T>` og `OnInitializedAsync`
**Varighed:** 60–75 min
**Render mode:** `InteractiveServer`
**Forudsætning:** Opgave 1 og 2

---

## Læringsmål

Efter denne opgave kan du:

- Sende data fra child til parent med `EventCallback<T>`
- Anvende komponentens livscyklus (`OnInitializedAsync`, `OnParametersSet`)
- Bruge `StateHasChanged` bevidst — og forstå hvornår det *ikke* er nødvendigt
- Implementere `IDisposable` for at rydde op efter timere/subscriptions
- Teste at interaktivitet faktisk virker i `InteractiveServer`

---

## Scenarie

Du bygger et lille "Huskeliste"-værktøj. Brugeren kan tilføje opgaver via en child-komponent, og parent-komponenten holder listen. En anden child-komponent viser en timer der tæller sekunder siden siden blev loadet — for at demonstrere lifecycle og Dispose.

---

## Trin 1 — Opret model

Opret `Models/Opgave.cs`:

```csharp
namespace BlazorOpgave3.Models;

public record Opgave(Guid Id, string Tekst, DateTime Oprettet, bool Faerdig);
```

---

## Trin 2 — Child 1: `OpgaveInput.razor`

> Ingen `@rendermode` på child-komponenter — de arver fra den side der bruger dem (`Huskeliste.razor` sætter den, se Trin 5).

```razor
<div class="opgave-input">
    <input @bind="tekst" @bind:event="oninput"
           @onkeydown="HaandterTast"
           placeholder="Skriv en opgave..."
           maxlength="120" />

    <button @onclick="Tilfoej" disabled="@(string.IsNullOrWhiteSpace(tekst))">
        Tilføj
    </button>
</div>

@code {
    private string tekst = string.Empty;

    // TODO: Tilføj en [Parameter] EventCallback<string> ved navn OnTilfoej

    private async Task Tilfoej()
    {
        if (string.IsNullOrWhiteSpace(tekst)) return;

        // TODO: Kald OnTilfoej.InvokeAsync med teksten
        // TODO: Nulstil `tekst` til string.Empty
    }

    private async Task HaandterTast(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await Tilfoej();
        }
    }
}
```

**Hint:** `EventCallback<T>` kaldes via `InvokeAsync(værdi)`. Bemærk: du skal **ikke** selv kalde `StateHasChanged` — Blazor gør det automatisk efter en callback.

---

## Trin 3 — Child 2: `OpgaveRaekke.razor`

```razor
@using BlazorOpgave3.Models

<li class="opgave-raekke @(Opgave.Faerdig ? "done" : "")">
    <input type="checkbox" checked="@Opgave.Faerdig"
           @onchange="ToggleFaerdig" />
    <span>@Opgave.Tekst</span>
    <time>@Opgave.Oprettet.ToString("HH:mm:ss")</time>
    <button class="slet" @onclick="Slet">✕</button>
</li>

@code {
    [Parameter, EditorRequired]
    public Opgave Opgave { get; set; } = default!;

    // TODO: Tilføj EventCallback<Guid> OnSlet og EventCallback<Guid> OnToggle

    private Task Slet() => /* TODO: Invoke OnSlet med Opgave.Id */ Task.CompletedTask;

    private Task ToggleFaerdig() => /* TODO: Invoke OnToggle med Opgave.Id */ Task.CompletedTask;
}
```

---

## Trin 4 — Child 3: `SekundTimer.razor` (lifecycle-demo)

Demonstrerer `OnInitializedAsync`, `StateHasChanged` og `IDisposable`:

```razor
@implements IDisposable

<p class="timer" aria-live="off">Sekunder siden load: <time>@sekunder</time></p>

@code {
    private int sekunder;
    private Timer? timer;

    protected override void OnInitialized()
    {
        // TODO: Opret en System.Threading.Timer der kalder en callback hvert sekund.
        //       Callbacken skal:
        //         1. Gøre sekunder++
        //         2. Kalde InvokeAsync(StateHasChanged) — HVORFOR InvokeAsync? (se refleksion)
    }

    public void Dispose()
    {
        // TODO: timer?.Dispose();
    }
}
```

---

## Trin 5 — Parent: `Components/Pages/Huskeliste.razor`

```razor
@page "/huskeliste"
@rendermode InteractiveServer
@using BlazorOpgave3.Models

<PageTitle>Huskeliste</PageTitle>

<main>
    <header>
        <h1>Huskeliste</h1>
        <SekundTimer />
    </header>

    <section aria-label="Tilføj opgave">
        <OpgaveInput OnTilfoej="TilfoejOpgave" />
    </section>

    <section aria-labelledby="liste-titel">
        <h2 id="liste-titel" class="sr-only">Opgaver</h2>

        @if (opgaver.Count == 0)
        {
            <p><em>Ingen opgaver endnu.</em></p>
        }
        else
        {
            <ul class="liste">
                @foreach (var o in opgaver)
                {
                    <OpgaveRaekke @key="o.Id"
                                  Opgave="o"
                                  OnSlet="SletOpgave"
                                  OnToggle="ToggleOpgave" />
                }
            </ul>

            <footer>
                <p>Færdige: @opgaver.Count(x => x.Faerdig) / @opgaver.Count</p>
            </footer>
        }
    </section>
</main>

@code {
    private List<Opgave> opgaver = new();

    protected override async Task OnInitializedAsync()
    {
        // Simuler at vi henter eksisterende opgaver fra en server/database
        await Task.Delay(200);
        opgaver.Add(new Opgave(Guid.NewGuid(), "Læs kapitel 7", DateTime.Now, false));
        opgaver.Add(new Opgave(Guid.NewGuid(), "Kode opgave 1", DateTime.Now, true));
    }

    private void TilfoejOpgave(string tekst)
    {
        // TODO: Tilføj ny Opgave til listen
    }

    private void SletOpgave(Guid id)
    {
        // TODO: Fjern opgave med matching Id
    }

    private void ToggleOpgave(Guid id)
    {
        // TODO: Erstat opgaven med en ny hvor Faerdig er toggled.
        // Hint: fordi Opgave er en record, kan du bruge `with`-syntaks:
        //       var ny = gammel with { Faerdig = !gammel.Faerdig };
    }
}
```

---

## Trin 6 — Minimal styling (`Huskeliste.razor.css`)

```css
.liste { list-style: none; padding: 0; }
.opgave-raekke {
    display: grid;
    grid-template-columns: auto 1fr auto auto;
    gap: 0.75rem;
    align-items: center;
    padding: 0.5rem;
    border-bottom: 1px solid #eee;
}
.opgave-raekke.done span { text-decoration: line-through; color: #888; }
.timer { color: #6b7280; font-family: ui-monospace, monospace; }
.slet { background: none; border: 0; cursor: pointer; color: #c00; }
```

---

## Acceptkriterier

- [ ] `/huskeliste` viser to indlæste opgaver (fra `OnInitializedAsync`)
- [ ] Sekund-timeren tæller opad hvert sekund
- [ ] Når siden forlades, frigives timeren (sæt et breakpoint i `Dispose`)
- [ ] Tilføj-knap og Enter-tast virker begge
- [ ] Checkboks toggler opgavens status
- [ ] Sletknap fjerner opgaven fra listen
- [ ] `@key="o.Id"` er sat på `OpgaveRaekke` — test hvad der sker hvis du fjerner den og sletter midten af listen

---

## Refleksionsspørgsmål

1. **Hvorfor `InvokeAsync(StateHasChanged)` i timer-callbacken** i stedet for blot `StateHasChanged()`?
   *Hint:* Timer-callbacken kører på en baggrundstråd uden for Blazors synkroniserings­kontekst.
2. Hvad er forskellen på `OnInitializedAsync` og `OnParametersSetAsync`? Hvornår kaldes hvilken?
3. Hvorfor er `@key` vigtigt i en `foreach`? Hvad sker der i rendertræet uden den?
4. Hvorfor er `Opgave` en `record` og ikke en `class` — hvilken fordel giver `with`-syntaksen her?
