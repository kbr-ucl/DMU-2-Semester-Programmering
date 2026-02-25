# SOLID Refactoring af TekstStatistikKalkulator

Opgaven består i at lave en `TekstStatistikKalkulator` der kan analyserer en tekstfil og give svar på filens indhold af:

- Antal linjer
- Antal ord
- Antal anslag

Et kodeskelet kunne se således ud:

```c#
public class TekstStatistikKalkulator
{
    public TekstStatistik BeregnTekstStatistik(string fileNavn)
    {
        // Indlæs fil

        // Beregn statistik

        // returner statistik
    }
}
```

## Non SOLID version

Vores aldrende senior udvikler der ikke kender til SOLID principperne har løst opgaven således:

```c#
public class TekstStatistikKalkulatorNonSolid
{
    public TekstStatistik BeregnTekstStatistik(string fileNavn)
    {
        // Indlæs fil
        string[] lines = File.ReadAllLines(fileNavn);

        // Beregn statistik
        int antalLinjer = lines.Length;
        int antalAnslag = 0;
        int antalOrd = 0;

        foreach (var line in lines)
        {
            antalAnslag += line.Length;
            antalOrd += line.Split(' ').Length;
        }

        // returner statistik
        return new TekstStatistik(antalAnslag, antalOrd, antalLinjer);
    }
}

public record TekstStatistik(int AntalAnslag, int AntalOrd, int AntalLinjer);
```

## Din opgave

Din CTO har bedt dig om at omskrive denne kode til kode der er vedligeholdelsesvenlig kode der kan regressionstestes. Men inden du får "go" på opgaven vil CTO'en have et oplæg fra din side om hvordan du vil gøre (hvilke principper og mønstre vil du bruge) samt en trin for trin plan for hvordan du vil gøre det.

---

## Dit oplæg

### Mål

At omskrive den eksisterende kode, så den bliver nem at vedligeholde, udvide og teste — med et solidt fundament af unit tests der over tid fungerer som regressionstests.

### Min tilgang: SOLID-principperne som fundament

Jeg vil bruge SOLID-principperne som styrende designprincipper for omskrivningen, fordi de direkte muliggør både vedligeholdelsesvenlighed og testbarhed.

#### Single Responsibility Principle

Jeg vil bryde store klasser og metoder op, så hver klasse kun har ét ansvar. Det giver os to fordele: det bliver nemmere at lokalisere og rette fejl, og vi kan skrive fokuserede unit tests der kun tester én ting ad gangen.

#### Open/Closed Principle

Koden struktureres, så ny funktionalitet kan tilføjes via udvidelse frem for at ændre eksisterende kode. Det betyder, at vores eksisterende tests forbliver gyldige, når vi tilføjer nye features — vi skriver blot nye tests til den nye funktionalitet.

#### Liskov Substitution Principle

Jeg sikrer, at nedarvningshierarkier er korrekte, så subtyper altid kan erstatte deres basistyper uden overraskelser. Det gør vores tests pålidelige og genbrugelige på tværs af typehierarkiet.

#### Interface Segregation Principle

Jeg opdeler store interfaces i mindre, fokuserede interfaces. Det er afgørende for testbarheden, fordi små interfaces er langt nemmere at mocke og stubbe i vores unit tests.

#### Dependency Inversion Principle

Dette er nøglen til hele teststratgien. Ved at lade klasser afhænge af abstraktioner (interfaces) frem for konkrete implementeringer, kan vi injicere mocks og stubs i vores tests. Uden dette princip er det i praksis umuligt at teste klasser isoleret.

### Teststrategi

Når koden er omskrevet efter SOLID-principperne, er den modulær og løst koblet, hvilket gør det muligt at skrive unit tests for hver komponent i isolation. Disse unit tests akkumuleres over tid og danner automatisk vores regressionstest-suite — et sikkerhedsnet der fanger utilsigtede bivirkninger, hver gang vi ændrer eller udvider systemet.

### Forventet resultat

Efter omskrivningen vil vi have en kodebase, hvor vi trygt kan refaktorere, rette fejl og tilføje nye features, fordi vores testsuiten hurtigt afslører, hvis noget går i stykker. Det reducerer risikoen ved ændringer markant og gør vedligeholdelsen billigere og hurtigere over tid.

---

## Relevante SOLID-principper for denne kode

Når vi kigger på `TekstStatistikKalkulatorNonSolid`, er der **tre** SOLID-principper der er relevante:

### 1. Single Responsibility Principle

Klassen har i dag **to ansvarsområder**: den læser filer fra filsystemet *og* beregner statistik. Hvis vi vil ændre, hvor vi henter teksten fra (f.eks. en database, et API eller brugerinput), skal vi ændre den samme klasse, som beregner statistik. De to ting bør adskilles.

### 2. Dependency Inversion Principle

Metoden `BeregnTekstStatistik` afhænger direkte af den konkrete implementering `File.ReadAllLines` — altså en tæt kobling til filsystemet. Det gør klassen **umulig at unit teste** uden at have en faktisk fil på disken. Vi bør i stedet afhænge af en abstraktion (et interface), så vi kan injicere en mock i vores tests.

### 3. Open/Closed Principle

Fordi filindlæsningen er hardkodet, kan vi ikke udvide klassen til at håndtere andre tekstkilder uden at **modificere** den eksisterende kode. Hvis vi abstraherer datakilden bag et interface, kan vi tilføje nye kilder (API, database, stream) ved blot at lave nye implementeringer — uden at røre kalkulatoren.

## Hvad er *ikke* relevant her?

**Liskov Substitution** og **Interface Segregation** er ikke i spil, da der ikke er noget nedarvningshierarki eller store interfaces at forholde sig til.

## Kerneproblemet i én sætning

`File.ReadAllLines` sidder direkte i kalkulationslogikken — det bryder SRP, DIP og OCP på én gang, og det er præcis dét der gør koden svær at vedligeholde og umulig at unit teste isoleret.

---

## Trin-for-trin: Refactoring af koden

### Trin 1 — Definer en abstraktion for tekstkilden

Opret et interface (f.eks. `ITekstKilde`) der beskriver, hvordan tekst hentes — uden at specificere *hvorfra*. Interfacet skal have én metode til at returnere tekstens linjer. Det bryder den direkte afhængighed til filsystemet og anvender Dependency Inversion Principle.

Designmønsteret bag dette trin er **Strategy Pattern**. Ved at definere `ITekstKilde` som et interface, indkapsler vi selve *strategien* for at hente tekst. Kalkulatoren kender kun til interfacet og er ligeglad med, hvilken konkret strategi der bruges — om det er en fil, et API eller en database. Det gør det muligt at udskifte strategien frit, både i produktionskode og i tests, uden at ændre kalkulatorens logik.

> [!NOTE]
>
> ***Hvorfor er det ikke adapter eller wapper?***
>
> De tre mønstre ligner hinanden strukturelt, men har forskellige *formål*:
>
> **Adapter/Wrapper** bruges, når man har en eksisterende klasse med et interface, der ikke passer til det, man har brug for. Adapteren oversætter mellem to inkompatible interfaces. Her har vi ikke et eksisterende interface, vi skal tilpasse os til — vi *designer* et nyt interface fra bunden.
>
> **Strategy** bruges, når man vil gøre en *adfærd* udskiftelig. Det er præcis det, vi gør: vi definerer `ITekstKilde` som en udskiftelig strategi for, hvordan tekst hentes. Kalkulatoren ved ikke *hvorfra* teksten kommer — den kender kun kontrakten. Pointen er, at vi kan vælge mellem flere ligeværdige strategier (fil, API, database) uden at ændre kalkulatoren.
>
> Hvis vi derimod havde et tredjeparts-bibliotek med en metode som `FetchContent()`, og vi oprettede `ITekstKilde` for at oversætte det kald til `HentLinjer()`, *så* ville det være en adapter — fordi formålet ville være at tilpasse et eksisterende interface til vores eget.
>
> Kort sagt: forskellen ligger i *intentionen*, ikke i strukturen. Her designer vi en udskiftelig adfærd, og det er Strategy.



### Trin 2 — Implementer den konkrete tekstkilde

Flyt `File.ReadAllLines`-logikken ud i en selvstændig klasse, der implementerer det nye interface. Denne klasse har nu ét og kun ét ansvar: at hente tekst fra en fil. Det er Single Responsibility Principle i praksis.

### Trin 3 — Omskriv kalkulatoren til at afhænge af abstraktionen

Kalkulatoren skal ikke længere modtage et filnavn. I stedet modtager den interfacet via constructor injection. Kalkulatoren kender dermed ikke til filsystemet — den ved kun, at den får linjer fra *en eller anden* tekstkilde.

### Trin 4 — Udvid med nye tekstkilder

Nu kan vi tilføje nye kilder (API, database, stream osv.) uden at røre kalkulatoren — vi laver blot en ny klasse der implementerer interfacet. Det er Open/Closed Principle i praksis: åben for udvidelse, lukket for modifikation.

---

## Trin-for-trin: Teststrategi

### Trin 1 — Opret en mock af tekstkilde-interfacet

Fordi kalkulatoren nu afhænger af et interface, kan vi lave en simpel test-implementering (en mock) der returnerer præcis de data, vi vil teste med — helt uden filsystem, netværk eller andre eksterne afhængigheder.

### Trin 2 — Skriv unit tests for kalkulatoren

Med mocken kan vi teste kalkulationslogikken isoleret. Hver test bør verificere præcis én ting og følge Arrange-Act-Assert-mønsteret: opsæt testdata, kald metoden, og verificer resultatet. Dæk edge cases som tom tekst, én linje, og flere linjer.

### Trin 3 — Kør tests som regressionstest-suite

Unit tests køres automatisk ved hver kodeændring (f.eks. via en CI/CD-pipeline). Over tid vokser testsuiten og danner et sikkerhedsnet: hvis en fremtidig ændring bryder eksisterende funktionalitet, fanger testene det med det samme.



# Møde med CTO

På dit møde med CTO'en får du godkendt din plan. Bortset fra trin 3 som skal vente indtil du har en godkendt kode. Din CTO beder derfor dig nu om at omskrive koden, og komme tilbage når du er klar til et kodereview.

Du går derfor nu i gang med at omskrive koden iht. firmaets gældende IT Governance Plan samt NuGet Governance Policy.

Disse har bl.a. denne positivliste:

- .NET 10
- Visual Studio 2026
- xUnit V3.
- Moq



Og bl.a. denne negativ liste:

- Anvendelse af AI til kodegenerering 



*God arbejdslyst - vi ses til kodereview*

