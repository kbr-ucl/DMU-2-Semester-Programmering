# Opgave 1 — Komponenter og parametre

**Tema:** Din første Razor-komponent med `[Parameter]`
**Varighed:** 45–60 min
**Render mode:** `InteractiveServer`
**Forudsætning:** .NET 10 SDK, Visual Studio 2026 (eller Rider/VS Code)

---

## Læringsmål

Efter denne opgave kan du:

- Oprette et Blazor Server-projekt med `InteractiveServer` rendermode
- Bygge en genanvendelig Razor-komponent med `[Parameter]`
- Anvende `ChildContent` (RenderFragment) til at wrappe indhold
- Style en komponent med isoleret CSS (`.razor.css`)

---

## Scenarie

Du skal bygge en "Studiekort"-side til datamatikeruddannelsen. Siden viser et kort for hver studerende med navn, studienummer og et antal fag. Du skal lave det som **tre** genanvendelige komponenter.

---

## Trin 1 — Opret projektet

```bash
dotnet new blazor -n BlazorOpgave1 --interactivity Server --empty
cd BlazorOpgave1
```

Flaget `--interactivity Server` konfigurerer projektet til `InteractiveServer` rendermode.
Flaget `--empty` fjerner det meste af scaffolded boilerplate så du starter med et rent projekt.

Åbn `Components/App.razor`. **Vi sætter IKKE rendermode på `<Routes />`** — den skal efterlades uden rendermode:

```razor
<Routes />
```

På den måde forbliver alle sider som default **Static SSR**, og kun de sider der har brug for interaktivitet får `@rendermode InteractiveServer` — se Trin 4.

Kør projektet med `dotnet run` og verificér at det starter.

---

## Trin 2 — Byg `StudieKort.razor` (skeleton)

Opret filen `Components/StudieKort.razor`:

> **Note:** Child-komponenter skal **ikke** have deres egen `@rendermode`. De arver rendermode fra den side der bruger dem. I denne opgave er kortet rent display, så siden kan forblive Static SSR.

```razor
<article class="studiekort" aria-labelledby="studiekort-@Studienummer">
    <header>
        <h3 id="studiekort-@Studienummer">@Navn</h3>
        <p class="studienummer">
            <span class="sr-only">Studienummer: </span>
            <span>@Studienummer</span>
        </p>
    </header>

    <section class="indhold" aria-label="Fag">
        @* TODO: Rendr ChildContent her *@
    </section>
</article>

@code {
    // TODO: Tilføj [Parameter] properties for Navn (string) og Studienummer (string)

    // TODO: Tilføj [Parameter] for ChildContent af typen RenderFragment
}
```

**Opgave 2a:** Udfyld `[Parameter]` properties.

**Hint:** `RenderFragment` rendres med `@ChildContent`.

---

## Trin 3 — Byg `FagTag.razor` (skeleton)

Opret `Components/FagTag.razor`:

```razor
<span class="fag-tag @CssKlasse">
    @Fagnavn
    @if (Ects.HasValue)
    {
        <em>(@Ects ECTS)</em>
    }
</span>

@code {
    [Parameter, EditorRequired]
    public string Fagnavn { get; set; } = string.Empty;

    // TODO: Valgfri parameter for ECTS-point (int?)

    // TODO: Computed property CssKlasse der returnerer:
    //   "stor"    hvis Ects >= 15
    //   "mellem"  hvis Ects er mellem 5 og 14
    //   "lille"   ellers
}
```

---

## Trin 4 — Komponér på en side

Opret `Components/Pages/Studiekort.razor`:

```razor
@page "/studiekort"
@rendermode InteractiveServer

<PageTitle>Studiekort</PageTitle>

<main>
    <header>
        <h1>Studiekort — datamatiker, 2. semester</h1>
    </header>

    <section class="kort-grid" aria-label="Studerende">

        <StudieKort Navn="Anna Andersen" Studienummer="dmaa20230001">
            <FagTag Fagnavn="Programmering 2" Ects="10" />
            <FagTag Fagnavn="Databaser" Ects="5" />
            <FagTag Fagnavn="Systemudvikling" Ects="15" />
        </StudieKort>

        @* TODO: Tilføj endnu et StudieKort med anden studerende og andre fag *@

    </section>
</main>
```

---

## Trin 5 — Scoped CSS

Opret `Components/StudieKort.razor.css` (filnavnet skal matche komponenten):

```css
.studiekort {
    border: 1px solid #d0d7de;
    border-radius: 8px;
    padding: 1rem;
    box-shadow: 0 1px 3px rgba(0,0,0,0.08);
}

.studiekort .studienummer {
    color: #6b7280;
    font-family: ui-monospace, monospace;
    margin: 0;
}

.studiekort .indhold {
    display: flex;
    flex-wrap: wrap;
    gap: 0.5rem;
    margin-top: 0.75rem;
}
```

Opret tilsvarende `Components/FagTag.razor.css` der styler `.fag-tag` i tre størrelser (stor/mellem/lille). Brug forskellige baggrundsfarver så størrelsen er visuel.

Opret også styling på `.kort-grid` så kortene ligger i et responsivt CSS Grid (mindst 2 kolonner på skærme > 700px).

---

## Acceptkriterier

- [ ] Projektet starter uden fejl på `/studiekort`
- [ ] `StudieKort` viser navn, studienummer og indhold fra `ChildContent`
- [ ] `FagTag` har tre visuelle størrelser baseret på ECTS
- [ ] Siden viser mindst to studiekort med forskelligt indhold
- [ ] CSS Grid-layoutet er responsivt
- [ ] Ingen kompileringsadvarsler om manglende `[Parameter]` attributter

---

## Refleksionsspørgsmål

1. Hvorfor er `ChildContent` bedre end en `List<string>` når komponenten skal wrappe andre komponenter?
2. Hvad er forskellen på `[Parameter]` og `[Parameter, EditorRequired]`?
3. Hvor i CSS-hierarkiet ender reglerne fra `StudieKort.razor.css` — og hvorfor "lækker" de ikke ud til andre komponenter?
4. Hvorfor pakkes sidens indhold i `<main>` og hvorfor bruges `<article>` til et studiekort? Hvordan hjælper det skærmlæsere?
