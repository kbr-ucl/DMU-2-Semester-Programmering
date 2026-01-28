# OOP Domain laget

Når vi går fra en **OOA Domæne Model** (vores teoretiske analyse) til **DDD (Domain-Driven Design)** som er en konkret **Domain-Centric** tilgang til implementering i C#, transformeres vores klasser til det, vi kalder **Entities**.

En **Entity** i DDD er kendetegnet ved, at den har en unik **Identitet**, som følger den over tid, uanset om dens egenskaber ændrer sig.

Her er de vigtigste skridt i transformationen:

### 1. Fra Attribut til Identitet

I din OOA-analyse har en bog et `Isbn`. I DDD gør vi dette til den unikke nøgle. Selvom bogen får et nyt cover eller skifter titel (teoretisk set), er det stadig den samme "enhed" (Entity) i systemet på grund af dens ID.

### 2. Indkapsling (Encapsulation)

I DDD må man ikke bare ændre data udefra (ingen "public setters" på alt). Vi gør egenskaberne `private set`, så vi tvinger koden til at bruge vores metoder. Det sikrer, at domænereglerne overholdes. Er der tale om egenskaber der ikke må ændres efter objektet er oprettet anvendes `init` der sikre at kun konstruktøren kan sætte egenskaben.

### 3. Logikken ind i klassen

I stedet for bare at gemme data, skal klassen styre sin egen tilstand. Metoden `LånBog()` skal tjekke, om bogen allerede er udlånt, samt om medlemmet allerede har tre igangværende udlån, før den ændrer status.

------

### Eksempel i C#

Her er hvordan din `Bog` og `Medlem` ser ud som DDD Entities kodet i C#, hvor der er anvendt pre-conditions og post-conditions: 

> [!TIP]
>
> Se note om pre-conditions og post-condition

```c#
public class Bog
{
    public string Isbn { get; init; }
    public string Titel { get; init; }
    public string Forfatter { get; init; }
    public bool ErUdlånt { get; private set; }

    // Konstruktør: Sikrer at vi aldrig har en "tom" bog i systemet
    public Bog(string isbn, string forfatter, string titel)
    {
        // PRE-CONDITIONS (Førbetingelser)
        // Vi tjekker kravene FØR vi gør noget.
        // Hvis disse ikke er opfyldt, kaster vi en fejl (Exception).
        if (string.IsNullOrWhiteSpace(isbn)) throw new ArgumentNullException(nameof(isbn));
        if (string.IsNullOrWhiteSpace(forfatter)) throw new ArgumentNullException(nameof(forfatter));
        if (string.IsNullOrWhiteSpace(titel)) throw new ArgumentNullException(nameof(titel));

        // SELVE HANDLINGEN
        Titel = titel;
        Forfatter = forfatter;
        Isbn = isbn;
        ErUdlånt = false;

        // POST-CONDITIONS (Efterbetingelser) ---
        // Vi tjekker, om resultatet er som forventet.
        // I C# bruger man ofte 'Debug.Assert' til dette under udvikling.
        Debug.Assert(Isbn == isbn, "Isbn blev ikke sat");
        Debug.Assert(Forfatter == forfatter, "Forfatter blev ikke sat");
        Debug.Assert(Titel == titel, "Titel blev ikke sat");
        Debug.Assert(!ErUdlånt, "ErUdlånt blev ikke sat til falsk");
    }

    // Adfærd: Metoder der håndterer statusændringer sikkert
    public void Udlån()
    {
        // Gward Clauses (pre-conditions)
        if (ErUdlånt)
            throw new InvalidOperationException("Fejl: Bogen er allerede udlånt.");

        // Handling
        ErUdlånt = true;

        // Post-condition
        Debug.Assert(ErUdlånt, "ErUdlånt blev ikke sat til sandt");
    }

    public void Aflever()
    {
        // Gward Clauses (pre-conditions)
        if (!ErUdlånt)
            throw new InvalidOperationException("Fejl: Bogen er allerede på biblioteket.");

        // Handling
        ErUdlånt = false;

        // post-conditions
        Debug.Assert(!ErUdlånt, "ErUdlånt blev ikke sat til falsk");
    }
}

public class Medlem
{
    public int Medlemsnummer { get; init; }
    public string Navn { get; init; }

    // Vi bruger en liste til INTERNT at holde styr på lånte bøger
    private List<Bog> LånteBøger { get; }

    // Vi skjuler listen, så ingen kan bruge .Add() udefra og omgå vores regler
    public IReadOnlyCollection<Bog> AktueltLånteBøger => LånteBøger.AsReadOnly();

    public Medlem(int medlemsnummer, string navn)
    {
        // PRE-CONDITIONS (Førbetingelser)
        // Vi tjekker kravene FØR vi gør noget.
        // Hvis disse ikke er opfyldt, kaster vi en fejl (Exception).
        if (string.IsNullOrWhiteSpace(navn)) throw new ArgumentNullException(nameof(navn));
        if (string.IsNullOrWhiteSpace(navn)) throw new ArgumentNullException(nameof(navn));

        // SELVE HANDLINGEN
        Medlemsnummer = medlemsnummer;
        Navn = navn;
        LånteBøger = [];

        // POST-CONDITIONS (Efterbetingelser) ---
        // Vi tjekker, om resultatet er som forventet.
        // I C# bruger man ofte 'Debug.Assert' til dette under udvikling.
        Debug.Assert(Medlemsnummer == medlemsnummer, "Medlemsnummer blev ikke sat");
        Debug.Assert(Navn == navn, "Navn blev ikke sat");
        Debug.Assert(LånteBøger.Count == 0, "LånteBøger blev ikke initialiseret korrekt");
    }

    // Adfærd: Her samles logikken
    public void LånBog(Bog bog)
    {
        // Gward Clauses (pre-conditions)
        ArgumentNullException.ThrowIfNull(bog);

        // Regel: Maks 3 bøger
        if (LånteBøger.Count >= 3)
            throw new InvalidOperationException("Du må højst låne 3 bøger.");

        
        // Handlingen udføres (Koordinering mellem objekter)
        bog.Udlån();
        LånteBøger.Add(bog);

        // Post-condition
        Debug.Assert(LånteBøger.Contains(bog), "Bogen blev ikke tilføjet til lånte bøger");
        // Bemærk at vi IKKE tjekker bog.ErUdlånt her, da det er et andet objekt, hvorfor vi stoler på at bog.Udlån() virker korrekt.
    }

    public void AfleverBog(Bog bog)
    {
        // Gward Clauses (pre-conditions)
        ArgumentNullException.ThrowIfNull(bog);

        if (!LånteBøger.Contains(bog))
            throw new InvalidOperationException("Medlemmet har ikke lånt denne bog.");

        // Handlingen udføres (Koordinering mellem objekter)
        bog.Aflever();
        LånteBøger.Remove(bog);

        // Post-condition
        Debug.Assert(!LånteBøger.Contains(bog), "Bogen blev ikke fjernet fra lånte bøger");
        // Igen stoler vi på at bog.Aflever() virker korrekt.
    }
}
```

------

### Sammenkoblingen til DDD-begreber

I dette eksempel ser du tre kerne-elementer af DDD:

1. **Gward Clauses:** I metoderne tjekker vi `if (ErUdlånt)` samt  `if (LånteBøger.Count >= 3)`. Dette sikrer, at objektet aldrig kommer i en ugyldig tilstand. Det er "Domain Centric" logik.
2. **Rich Domain Model:** Vores klasser er ikke bare tomme beholdere (Anemic Domain Model), men har faktisk logik og beskytter deres egne data.
3. **Identitet:** `Isbn` og `MedlemsNummer` bruges til at skelne mellem objekter, selvom to personer hedder "Lars" eller to bøger har samme titel. I den kommende persistering i database erklærer vi i opsætningen i databasen `Isbn` og  `MedlemsNummer` som primær nøgler.

**Hvad betyder det for din kode?**

Det betyder, at hvis en programmør prøver at udlåne en bog, der allerede er væk, så "skælder koden ud" (kaster en Exception). Logikken bor i **hjertet** af systemet, præcis som vi tegnede i domain centric arkitekturen.

