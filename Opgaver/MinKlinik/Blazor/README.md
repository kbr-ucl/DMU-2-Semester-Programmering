# Opgaver — Blazor SSR

Fire progressive opgaver der bygger op til en komplet Blazor-frontend på MinKlinik-solutionen. Alle opgaver bruger `InteractiveServer` rendermode.

| # | Opgave | Varighed | Fokus |
|---|--------|----------|-------|
| 1 | [Komponenter og parametre](Opgave-1-Komponenter-og-parametre.md) | 45–60 min | `[Parameter]`, `ChildContent`, scoped CSS |
| 2 | [Formular og validering](Opgave-2-Formular-og-validering.md) | 60–75 min | `EditForm`, `@bind-Value`, DataAnnotations |
| 3 | [EventCallback og lifecycle](Opgave-3-EventCallback-og-lifecycle.md) | 60–75 min | `EventCallback<T>`, `OnInitializedAsync`, `IDisposable` |
| 4 | [MinKlinik Blazor-frontend](Opgave-4-MinKlinik-Blazor-frontend.md) | ≈ 4 timer | Clean Architecture-integration, fuld CRUD |

## Progression

Opgave 1–3 er selvstændige træningsøvelser i eget projekt — én Blazor-konceptgruppe ad gangen. Opgave 4 samler alle koncepter til et produktionslignende UI-lag oven på den eksisterende `MinKlinik.slnx`.

## Aflevering

Hver opgave har en **Acceptkriterier**-liste — brug den som selv-tjek før aflevering. Refleksionsspørgsmålene er udgangspunkt for mundtlig gennemgang.

## Format

Opgaverne leveres som skeleton-kode med `// TODO:`-markeringer. De studerende udfylder de markerede huller og verificerer mod acceptkriterierne.

## Rendermode — fælles konvention

**`@rendermode InteractiveServer` sættes på page-niveau — aldrig på `<Routes />`.**

| Hvor | Hvordan |
|------|---------|
| `App.razor` | `<Routes />` uden `@rendermode` — lader alle sider være Static SSR som default |
| `Pages/MinSide.razor` | Første linje efter `@page`: `@rendermode InteractiveServer` |
| Child-komponenter i `Shared/` | **Ingen** `@rendermode` — de arver fra siden der bruger dem |
| `Program.cs` | Stadig kald `AddInteractiveServerComponents()` og `AddInteractiveServerRenderMode()` — infrastrukturen skal være registreret selv om den bruges pr. side |

Fordelen: Kun sider der faktisk har behov for interaktivitet åbner en SignalR-forbindelse. Oversigts- og detalje-sider der kun viser data kan forblive Static SSR.

## Semantisk HTML5 — fælles krav

Alle opgaver bruger semantisk HTML5. Det betyder:

- Sidens hovedindhold pakkes i `<main>`; introduktion i `<header>`; "øvrige ting" som supplement er `<aside>`
- Afsnit grupperes i `<section aria-labelledby="...">` eller `<article>` når det står for sig selv
- Navigation bruger `<nav aria-label="...">`; brødkrumme også
- Formularer bruger `<fieldset>` + `<legend>`, og hvert `<label for="id">` peger på et input med samme `id`
- Tabeller har `<caption>` (må være `.sr-only`) og `<th scope="col|row">`
- Tidspunkter står i `<time datetime="...">`
- Bekræftelses-/modaldialoger bruger HTML5 `<dialog>` — ikke `<div role="dialog">`
- Fejlbeskeder: `role="alert"`; statusbeskeder: `role="status"`; asynkrone områder: `aria-live` og `aria-busy`

### Standard `.sr-only`-klasse

Flere opgaver bruger `.sr-only` til tekst der kun skal læses af skærmlæsere. Læg denne i `wwwroot/app.css`:

```css
.sr-only {
    position: absolute;
    width: 1px;
    height: 1px;
    padding: 0;
    margin: -1px;
    overflow: hidden;
    clip: rect(0, 0, 0, 0);
    white-space: nowrap;
    border: 0;
}
```
