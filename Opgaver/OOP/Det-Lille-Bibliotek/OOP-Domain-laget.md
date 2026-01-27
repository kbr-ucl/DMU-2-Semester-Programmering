# OOP Domain laget

Når vi går fra en **OOA Domæne Model** (vores teoretiske analyse) til **DDD (Domain-Driven Design)** som er en konkret **Domain-Centric** tilgang til implementering i C#, transformeres vores klasser til det, vi kalder **Entities**.

En **Entity** i DDD er kendetegnet ved, at den har en unik **Identitet**, som følger den over tid, uanset om dens egenskaber ændrer sig.

Her er de vigtigste skridt i transformationen:

### 1. Fra Attribut til Identitet

I din OOA-analyse har en bog et `ISBN`. I DDD gør vi dette til den unikke nøgle. Selvom bogen får et nyt cover eller skifter titel (teoretisk set), er det stadig den samme "enhed" (Entity) i systemet på grund af dens ID.

### 2. Indkapsling (Encapsulation)

I DDD må man ikke bare ændre data udefra (ingen "public setters" på alt). Vi gør egenskaberne `private set`, så vi tvinger koden til at bruge vores metoder. Det sikrer, at domænereglerne overholdes.

### 3. Logikken ind i klassen

I stedet for bare at gemme data, skal klassen styre sin egen tilstand. Metoden `LånBog()` skal tjekke, om bogen allerede er udlånt, samt om medlemmet allerede har tre igangværende udlån, før den ændrer status.

------

### Eksempel i C#

Her er hvordan din `Bog` og `Medlem` ser ud som DDD Entities kodet i C#:

```c#
public class Entity
{
    public Guid Id { get; protected set; }
}
public class Bog : Entity
{
    public string Titel { get; private set; }
    public string Forfatter { get; private set; }
    public string Isbn { get; private set; }
    public bool ErUdlånt { get; private set; }

    // Konstruktør: Sikrer at vi aldrig har en "tom" bog i systemet
    public Bog(string titel, string forfatter, string isbn, bool erUdlånt = false)
    {
        if (string.IsNullOrWhiteSpace(titel)) throw new ArgumentException("Titel mangler");

        Id = Guid.NewGuid();
        Titel = titel;
        Forfatter = forfatter;
        Isbn = isbn;
        ErUdlånt = erUdlånt;
    }

    // Adfærd: Metoder der håndterer statusændringer sikkert
    public void Udlån()
    {
        if (ErUdlånt)
            throw new InvalidOperationException("Fejl: Bogen er allerede udlånt.");

        ErUdlånt = true;
    }

    public void Aflever()
    {
        if (!ErUdlånt)
            throw new InvalidOperationException("Fejl: Bogen er allerede på biblioteket.");

        ErUdlånt = false;
    }
}

public class Medlem : Entity
{
    public string Navn { get; private set; }

    // Vi skjuler listen, så ingen kan bruge .Add() udefra og omgå vores regler
    private List<Bog> LånteBøger { get; set; }
    public IReadOnlyCollection<Bog> AktuelleLånteBøger => _lånteBøger.AsReadOnly();

    public Medlem(string navn)
    {
        if (string.IsNullOrWhiteSpace(navn)) throw new ArgumentException("Navn mangler");

        Id = Guid.NewGuid();
        Navn = navn;
        LånteBøger = [];
    }

    // Adfærd: Her samles logikken
    public void LånBog(Bog bog)
    {
        ArgumentNullException.ThrowIfNull(bog);

        // Regel: Maks 3 bøger
        if (LånteBøger.Count >= 3)
            throw new InvalidOperationException("Du må højst låne 3 bøger.");

        // Handlingen udføres (Koordinering mellem objekter)
        bog.Udlån();
        LånteBøger.Add(bog);
    }

    public void AfleverBog(Bog bog)
    {
        if (!LånteBøger.Contains(bog))
            throw new InvalidOperationException("Medlemmet har ikke lånt denne bog.");

        bog.Aflever();
        LånteBøger.Remove(bog);
    }
}
```

------

### Sammenkoblingen til DDD-begreber

I dette eksempel ser du tre kerne-elementer af DDD:

1. **Gward Clauses:** I metoderne tjekker vi `if (ErUdlånt)` samt  `if (LånteBøger.Count >= 3)`. Dette sikrer, at objektet aldrig kommer i en ugyldig tilstand. Det er "Domain Centric" logik.
2. **Rich Domain Model:** Vores klasser er ikke bare tomme beholdere (Anemic Domain Model), men har faktisk logik og beskytter deres egne data.
3. **Identitet:** `Isbn` og `MedlemsNummer` bruges til at skelne mellem objekter, selvom to personer hedder "Lars" eller to bøger har samme titel. Men pga. kommende persistering i database anvender vi en teknisk nøgle - Id - som er defineret i superklassen `Entity`. Da `MedlemsNummer` er et opfundet nøglefelt, udgår det, idet Id opfylder behovet. For `Isbn` derimod, er det nødvendigt at erklærer `Isbn` som en unik egenskab. Dette håndteres senere i forbindeles med database opsætningen.

**Hvad betyder det for din kode?**

Det betyder, at hvis en programmør prøver at udlåne en bog, der allerede er væk, så "skælder koden ud" (kaster en Exception). Logikken bor i **hjertet** af systemet, præcis som vi tegnede i løg-arkitekturen.

