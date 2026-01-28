## Domain-Centric og DDD - Fra UI til Entity metode kald

> [!IMPORTANT]
>
> Hvordan hændelser fra brugeren (aktør) bliver til aktivering af adfærd



Det er et meget vigtigt spørgsmål. For begyndere er det tit det sværeste "missing link": Hvordan kommer vi fra et klik på en skærm til at koden inde i `Medlem`-klassen kører?

I moderne softwarearkitektur (og især i DDD) taler vi aldrig direkte med domæne-objekterne fra brugergrænsefladen. Vi har brug for et **mellemlag** – en "koordinator" eller "orkestrator".

I DDD kaldes dette ofte en **Application Service** (eller Use Case).

Her er forklaringen på flowet fra bruger til kode, efterfulgt af et C# eksempel.

### Flowet: De 5 Trin

Forestil dig, at bibliotekaren sidder ved en skærm og trykker på knappen **"Opret Lån"**.

Hændelsen rejser gennem systemet sådan her:

1. **Aktøren (Brugeren)** indtaster data (f.eks. medlems-ID og bog-ID) og klikker "Lån".
2. **API / UI Laget** modtager klikket og sender en besked til applikationen: "Jeg vil gerne udføre kommandoen `LånBog`".
3. **Application Service (Koordinatoren)** tager imod beskeden. Dens job er at hente de rigtige objekter frem fra databasen.
4. **Domæne Logik (Adfærd)** aktiveres. Koordinatoren beder `Medlem`-objektet om at udføre `LånBog()`. Det er her, reglerne tjekkes.
5. **Persistering (Gem)**. Hvis alt gik godt, gemmer koordinatoren de ændrede objekter tilbage i databasen.

------

### C# Eksempel: "UdlånUseCase"

Her laver vi den klasse, der binder det hele sammen. Vi lader som om, vi har et "Repository" (en klasse der henter data fra databasen), da det er standardmåden at finde entities på.

```csharp
// Dette er "Application Layer" - Koordinatoren
public class UdlånUseCase
{
    private readonly IBogRepository _bogRepository;
    private readonly IMedlemsRepository _medlemsRepository;

    // Vi får adgang til databasen (Repository) gennem konstruktøren
    public UdlånUseCase(IBogRepository bogRepo, IMedlemsRepository medlRepo)
    {
        _bogRepository = bogRepo;
        _medlemsRepository = medlRepo;
    }

    // Dette er metoden, der bliver kaldt, når brugeren trykker på knappen
    public void LånAfBog(int medlemsnummer, string isbn)
    {
        // TRIN 1: Hent data (Rehydrering af objekter)
        // Vi henter domæne-objekterne op fra databasen baseret på ID.
        var medlem = _medlemsRepository.Hent(medlemsnummer);
        var bog = _bogRepository.Hent(isbn);

        // Validering: Findes de overhovedet?
        if (medlem == null) throw new Exception("Medlem ikke fundet");
        if (bog == null) throw new Exception("Bog ikke fundet");

        // TRIN 2: Aktiver Adfærd (Her sker magien)
        // Vi kalder metoden på vores DDD Entity.
        // Bemærk: Servicen kender ikke reglerne (f.eks. "maks 3 bøger"). 
        // Den beder bare medlemmet om at gøre det.
        try 
        {
            medlem.LånBog(bog); 
        }
        catch (InvalidOperationException ex)
        {
            // Hvis reglen blev brudt (f.eks. bogen er allerede udlånt),
            // sender vi fejlen tilbage til brugeren her.
            Console.WriteLine($"Fejl: {ex.Message}");
            return; 
        }

        // TRIN 3: Gem ændringer (Persistering)
        // Nu har objekterne ændret sig (bogen er 'udlånt', medlemmet har et bogen i sin liste).
        // Vi skal gemme den nye tilstand i databasen.
        _medlemsRepository.Opdater(medlem);
        
        Console.WriteLine("Bogen er nu udlånt!");
    }
}
```



### Hvorfor gør vi det sådan?

Hvis de spørger, hvorfor man ikke bare retter direkte i databasen fra knappen, er svaret:

- **Sikkerhed:** Hvis vi glemmer at bruge `medlem.LånBog()`, glemmer vi måske at tjekke, om bogen allerede er udlånt. Ved at tvinge alt gennem `BiblioteksService` -> `Entity`, sikrer vi, at reglerne *altid* overholdes.
- **Testbarhed:** Vi kan nemt teste `Medlem`-klassen uden at skulle have en hel database kørende.

Giver denne opdeling mening i forhold til den opgave, du er ved at bygge?