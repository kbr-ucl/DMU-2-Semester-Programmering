# Opgave 2 — Formular og validering

**Tema:** `EditForm`, to-vejs databinding og DataAnnotations
**Varighed:** 60–75 min
**Render mode:** `InteractiveServer`
**Forudsætning:** Opgave 1 er gennemført

---

## Læringsmål

Efter denne opgave kan du:

- Bygge en `EditForm` med `InputText`, `InputNumber`, `InputDate` og `InputSelect`
- Anvende to-vejs databinding med `@bind-Value`
- Validere input med DataAnnotations og `DataAnnotationsValidator`
- Vise valideringsfejl med `ValidationSummary` og `ValidationMessage`
- Håndtere `OnValidSubmit` vs `OnSubmit` korrekt

---

## Scenarie

Receptionen på en klinik skal kunne oprette patienter. Du bygger en formular der validerer CPR-nummer, navn, email og fødselsdato før patienten gemmes i en intern liste.

> Bemærk: I denne opgave gemmes data kun i hukommelsen (en `List<T>` i siden). I opgave 4 kobles formularen på en rigtig Use Case via Facade.

---

## Trin 1 — Opret model-klassen

Opret `Models/PatientFormModel.cs`:

```csharp
using System.ComponentModel.DataAnnotations;

namespace BlazorOpgave2.Models;

public class PatientFormModel
{
    [Required(ErrorMessage = "Navn er påkrævet")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Navn skal være mellem 2 og 100 tegn")]
    public string Navn { get; set; } = string.Empty;

    // TODO: Tilføj Cpr (string).
    //       Regler: Påkrævet. RegEx der matcher 10 cifre (evt. med bindestreg efter 6).
    //       Hint: [RegularExpression(@"^\d{6}-?\d{4}$")]

    // TODO: Tilføj Email (string).
    //       Regler: Påkrævet + [EmailAddress]

    // TODO: Tilføj Fødselsdato (DateTime).
    //       Regler: Påkrævet. Custom validering: må ikke være i fremtiden.
    //       Hint: Implementér IValidatableObject eller skriv en custom ValidationAttribute.

    // TODO: Tilføj Samtykke (bool).
    //       Regler: Skal være true. Hint: [Range(typeof(bool), "true", "true", ErrorMessage = "...")]
}
```

---

## Trin 2 — Byg formular-komponenten

Opret `Components/Pages/OpretPatient.razor`:

```razor
@page "/opret-patient"
@rendermode InteractiveServer
@using BlazorOpgave2.Models

<PageTitle>Opret patient</PageTitle>

<main>
    <header>
        <h1>Opret patient</h1>
    </header>

    <section aria-labelledby="formtitel">
        <h2 id="formtitel" class="sr-only">Patientformular</h2>

        <EditForm Model="formModel" OnValidSubmit="HaandterValidSubmit" FormName="opretPatient">
            <DataAnnotationsValidator />
            <ValidationSummary class="valid-summary" />

            <fieldset>
                <legend>Stamdata</legend>

                <div class="felt">
                    <label for="navn">Navn</label>
                    <InputText id="navn" @bind-Value="formModel.Navn" autocomplete="name" />
                    <ValidationMessage For="() => formModel.Navn" />
                </div>

                @* TODO: Tilføj felter for Cpr, Email, Fødselsdato (InputDate) og Samtykke (InputCheckbox) *@
            </fieldset>

            <button type="submit" disabled="@erIGang">Opret patient</button>
        </EditForm>
    </section>

    @if (seneste is not null)
    {
        <aside class="kvittering" role="status" aria-live="polite">
            <h2>Patient oprettet</h2>
            <p>@seneste.Navn (@seneste.Cpr) blev gemt.</p>
        </aside>
    }

    <section aria-labelledby="liste-titel">
        <h2 id="liste-titel">Oprettede patienter (@oprettede.Count)</h2>
        <PatientListe Patienter="oprettede" />
    </section>
</main>

@code {
    private PatientFormModel formModel = new();
    private List<PatientFormModel> oprettede = new();
    private PatientFormModel? seneste;
    private bool erIGang;

    private async Task HaandterValidSubmit()
    {
        erIGang = true;
        try
        {
            // Simuler netværkskald
            await Task.Delay(400);

            // TODO: Tilføj formModel til listen `oprettede`
            // TODO: Sæt `seneste = formModel`
            // TODO: Nulstil formModel = new() så formularen tømmes
        }
        finally
        {
            erIGang = false;
        }
    }
}
```

---

## Trin 3 — Byg `PatientListe.razor`

```razor
@using BlazorOpgave2.Models

@if (Patienter.Count == 0)
{
    <p><em>Ingen patienter oprettet endnu.</em></p>
}
else
{
    <table>
        <caption class="sr-only">Oversigt over oprettede patienter</caption>
        <thead>
            <tr>
                <th scope="col">Navn</th>
                <th scope="col">CPR</th>
                <th scope="col">Email</th>
                <th scope="col">Fødselsdato</th>
            </tr>
        </thead>
        <tbody>
            @* TODO: Foreach over Patienter og rendér en <tr> per patient.
                     Vis fødselsdato som "dd-MM-yyyy". *@
        </tbody>
    </table>
}

@code {
    [Parameter, EditorRequired]
    public List<PatientFormModel> Patienter { get; set; } = new();
}
```

---

## Trin 4 — Aktiver interaktivitet pr. side (ikke globalt)

**Vigtigt:** Vi sætter `@rendermode InteractiveServer` **pr. side** — ikke på `<Routes />` eller `<HeadOutlet />`. Det holder alle andre sider som Static SSR og begrænser SignalR-overhead til de sider der faktisk har brug for interaktivitet.

Kontrollér at `Components/App.razor` har `<Routes />` **uden** `@rendermode`:

```razor
<HeadOutlet />
...
<Routes />
```

Og at `Program.cs` har registreret server-komponenterne — selv om de kun bruges pr. side:

```csharp
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ...

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
```

Den nødvendige `@rendermode InteractiveServer` står allerede på `OpretPatient.razor` (Trin 2, linje 2).

---

## Udfordring (valgfri)

Lav en **custom** `ValidationAttribute` der validerer CPR-modulus 11. Kald den `[GyldigtCpr]` og brug den i stedet for `[RegularExpression]`. Hint:

```csharp
public sealed class GyldigtCprAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        // TODO: parse til 10 cifre, berægn modulus 11 mod kontrolcifret.
        //       Returner ValidationResult.Success hvis OK, ellers en fejl.
    }
}
```

---

## Acceptkriterier

- [ ] Siden `/opret-patient` renderer formularen og knappen reagerer på klik (InteractiveServer virker)
- [ ] Tom form giver valideringsfejl på alle felter ved submit
- [ ] Fødselsdato i fremtiden afvises med brugervenlig fejl
- [ ] "Samtykke"-checkbox skal være markeret for at kunne indsende
- [ ] Efter valid submit ryddes formularen og patienten vises i listen
- [ ] Under submit er knappen disabled (brug `erIGang`-flaget)

---

## Refleksionsspørgsmål

1. Hvad er forskellen på `OnSubmit` og `OnValidSubmit`? Hvornår ville du bruge `OnInvalidSubmit`?
2. Hvorfor skal `DataAnnotationsValidator` være inde i `EditForm` — hvad gør den?
3. Hvordan ville du flytte valideringsreglerne ud af formular-modellen og over på en Command/DTO i Facade-laget? (tænk CQS)
