# Hvad er pre-condtions og post-conditions

I programmering og softwareudvikling handler **pre-conditions** (førbetingelser) og **post-conditions** (efterbetingelser) om at lave klare aftaler om, hvordan en funktion eller et stykke kode skal virke.

Du kan tænke på det som en **kontrakt** mellem den, der bruger koden, og selve koden.

**pre-conditions** (førbetingelser) og **post-conditions** (efterbetingelser) er tæt koblet til Definition of Done (DoD) og Acceptkriterier, der hører til i User Story beskrivelsen af opgaven (se hvorledes det kobler senere i dokumentet).

Her er en simpel forklaring opdelt i hverdagssprog og programmeringssprog.

------

### 1. Hverdagsanalogien: En Hæveautomat

Forestil dig, at du går hen til en hæveautomat for at hæve 500 kr. For at denne handling (funktionen) skal lykkes, er der nogle regler før og efter.

#### Pre-condition (Førbetingelsen)

Dette er kravet til **dig** (brugeren) *før* automaten kan gøre sit arbejde.

- **Krav:** Du skal have et kort, du skal kunne din PIN-kode, og der skal være mindst 500 kr. på din konto.
- *Hvis du ikke opfylder dette:* Så virker automaten ikke, og den afviser dig. Det er din fejl.

#### Post-condition (Efterbetingelsen)

Dette er løftet fra **automaten** til dig *efter* handlingen er færdig (hvis du opfyldte kravene).

- **Resultat:** Du står med 500 kr. i hånden, og din saldo på kontoen er præcis 500 kr. lavere end før.
- *Hvis automaten ikke opfylder dette:* Så er maskinen i stykker (en "bug"). Det er automatens fejl.

------

### 2. Teknisk Forklaring

Når vi skriver en metode i C#, definerer vi disse betingelser for at undgå fejl.

#### Pre-condition (Kravet der tjekkes i starten af metoden)

Det er en betingelse, der **skal være sand**, lige før metodens logik udføres.

- Det er "kalders" ansvar (den der kalder metoden) at pre-conditions overholdes (kontrakten imellem den der kalder og den der udfører).

- Hvis pre-condition fejler, kan metoden ikke garantere, at den virker, og der kastes en **exception**

  - > **"En exception er en hændelse, der afbryder programmets normale flow."**

    Denne hændelse kan udløses af to ting:

    1. **Interne fejl:** Brud på pre-conditions (programmeringsfejl / ugyldigt input).
    2. **Eksterne fejl:** Problemer i miljøet (netværk, disk, database).

  -  I hverdagssprog: *"Du har brudt min pre-condition (kontrakten), så nu kaster jeg en exception, og jeg nægter at køre videre, før du fikser dit input."*.

#### Post-condition (Garantien efter slut)

Det er en betingelse, der **vil være sand**, lige efter metoden er færdig.

- Det er metodens ansvar at opfylde post-condition hvis pre-condition er opfyldt.
- Det fortæller os, hvad vi kan regne med, at metoden har gjort ved systemet eller data (klassens egenskaber - dvs. dens properties og fields).

------

### 3. Et kode-eksempel (Division)

Lad os sige, vi laver en simpel funktion, der skal dividere to tal: $a / b$.

**Funktion:** `divider(a, b)`

- **Pre-condition:** `b` må ikke være 0.
  - *Hvorfor?* Man kan ikke dividere med nul. Hvis du sender 0 ind som `b`, bryder du kontrakten, og programmet går ned (crasher).
- **Post-condition:** Resultatet er lig med `a` divideret med `b`.

**Hvis vi skrev det som en kontrakt:**

> "Hvis du lover ikke at give mig et 0 (Pre-condition), så lover jeg at give dig det korrekte resultat tilbage (Post-condition)."

### Hvorfor bruger vi det?

1. **Færre fejl:** Hvis man tjekker sine pre-conditions, fanger man fejl tidligt (f.eks. "Hov, du forsøger at dividere med nul!").
2. **Bedre dokumentation:** Andre programmører ved præcis, hvad de må og ikke må sende ind i din funktion.
3. **Ansvar:** Det gør det nemmere at finde ud af, hvem der har lavet fejlen.
   - Fejlede **pre-condition**? Så er det den, der *kaldte* funktionen, der lavede en fejl.
   - Fejlede **post-condition**? Så er det selve funktionen, der er kodet forkert.

------

### Opsamling

| **Begreb**         | **Spørgsmål**                       | **Hvem har ansvaret?** |
| ------------------ | ----------------------------------- | ---------------------- |
| **Pre-condition**  | Hvad skal være sandt **før** start? | Brugeren (Kaldet)      |
| **Post-condition** | Hvad er sandt **efter** slut?       | Funktionen (Koden)     |



--------

### Kodeeksempel: Bankkonto

Her er en simpel C#-klasse. Læg mærke til kommentarerne i koden, der viser hvor pre- og post-conditions er.

```c#
using System;
using System.Diagnostics; // Bruges til Debug.Assert

public class BankKonto
{
    public decimal Saldo { get; private set; }

    public BankKonto(decimal startSaldo)
    {
        Saldo = startSaldo;
    }

    public void HævPenge(decimal beløb)
    {
        // --- 1. PRE-CONDITIONS (Førbetingelser) ---
        // Vi tjekker kravene FØR vi gør noget.
        // Hvis disse ikke er opfyldt, kaster vi en fejl (Exception).

        if (beløb <= 0)
        {
            throw new ArgumentException("Beløbet skal være større end 0.");
        }

        if (beløb > Saldo)
        {
            throw new InvalidOperationException("Ikke nok penge på kontoen.");
        }

        // (Gemmer den gamle saldo kun for at kunne tjekke post-condition senere)
        decimal gammelSaldo = Saldo;

        // --- 2. SELVE HANDLINGEN ---
        Saldo = Saldo - beløb;

        // --- 3. POST-CONDITIONS (Efterbetingelser) ---
        // Vi tjekker, om resultatet er som forventet.
        // I C# bruger man ofte 'Debug.Assert' til dette under udvikling.
        
        // Krav: Den nye saldo skal være lig med den gamle saldo minus beløbet.
        Debug.Assert(Saldo == gammelSaldo - beløb, "Matematikken fejlede! Saldoen stemmer ikke.");
        
        Console.WriteLine($"Hævet: {beløb} kr. Ny saldo: {Saldo} kr.");
    }
}
```

------

### Gennemgang af koden

Her er hvad der sker i de to faser:

#### 1. Pre-conditions (Vagthunden)

I C# bruger vi oftest `if`-sætninger i starten af metoden.

- `if (beløb <= 0)`: Sikrer, at vi ikke prøver at hæve negative penge eller 0 kr.
- `if (beløb > Saldo)`: Sikrer, at der er dækning på kontoen.

Hvis en af disse er sande, stopper vi *øjeblikkeligt* ved at kaste en `Exception`. Vi udfører **ikke** selve handlingen (at trække penge). Det beskytter dataene.

#### 2. Post-conditions (Kvalitetskontrollen)

I koden ovenfor brugte jeg `Debug.Assert`.

- Det er en måde at sige til computeren: *"Tjek lige om det her er sandt. Hvis det ikke er, så stop programmet, for så har jeg programmeret en fejl."*
- I linjen `Debug.Assert(Saldo == gammelSaldo - beløb)` sikrer vi os, at regnestykket rent faktisk blev udført korrekt.

### Hvordan bruges det i virkeligheden?

I professionel C#-udvikling ser man ofte en lille forskel på, hvordan de to håndteres:

1. **Pre-conditions** skrives *næsten altid* direkte i koden (som vist ovenfor med `if` og `throw`), fordi vi skal stoppe ugyldige input fra at ødelægge systemet.
2. **Post-conditions** skrives ofte i **Unit Tests** (automatiske tests) i stedet for inde i selve metoden. Det holder selve koden renere, men sikrer stadig, at funktionen gør det, den lovede.

**Giver koden mening, eller vil du se, hvordan man ville skrive en "Unit Test" til at tjekke post-condition i stedet?**



## Tjek af post-conditions i en Unit Test



Det er en rigtig god idé at flytte tjekket af post-conditions ud i en Unit Test. Det holder selve produktionskoden ren, og det sikrer, at du kan teste det igen og igen automatisk (regresionstest - et element fra agil udvikling).

I Unit Testing bruger man en struktur, der kaldes **AAA** (Arrange, Act, Assert). Den passer perfekt til det, vi har talt om:

1. **Arrange:** Opsætning (Svarer til at sikre **Pre-conditions** og start-state).
2. **Act:** Udfør handlingen.
3. **Assert:** Tjek resultatet (Svarer til at verificere **Post-conditions**).

Her er et eksempel skrevet i C# (ved brug af test-frameworket **xUnit**, som er meget populært):

### Kodeeksempel: Unit Test af BankKonto



```c#
using Xunit; // Vi bruger et test-framework

public class BankKontoTests
{
    [Fact] // Dette fortæller systemet, at her er en test
    public void HævPenge_SkalOpdatereSaldo_NårBeløbErGyldigt()
    {
        // --- 1. ARRANGE (Gør klar) ---
        // Her opstiller vi scenariet og sikrer, at vi starter et kendt sted.
        decimal startSaldo = 1000m;
        decimal hæveBeløb = 200m;
        var konto = new BankKonto(startSaldo);

        // --- 2. ACT (Handling) ---
        // Vi kalder funktionen.
        konto.HævPenge(hæveBeløb);

        // --- 3. ASSERT (Tjek Post-condition) ---
        // Det er HER, vi tjekker om post-condition er opfyldt.
        // Vi forventer, at saldoen nu er 800 (1000 - 200).
        
        decimal forventetSaldo = 800m;
        
        // Assert.Equal(forventet, faktisk)
        Assert.Equal(forventetSaldo, konto.Saldo);
    }
}
```

### Hvad sker der her?

1. **Vi opretter en konto med 1000 kr.** (Vi har nu kontrol over start-situationen).
2. **Vi hæver 200 kr.** (Vi udfører koden).
3. **Vi spørger:** *"Er saldoen lig med 800 kr?"*

Hvis saldoen er 800, bliver testen **GRØN** (Bestået).

Hvis saldoen er noget som helst andet (f.eks. hvis programmøren glemte at trække pengene fra), bliver testen **RØD** (Fejlet), og du får en besked om, at post-condition ikke holdt.

------

### Bonus: Test af Pre-condition (Exception)

Man bør også lave en test, der tjekker, at din **pre-condition** (vagthunden) faktisk virker og bider fra sig, hvis nogen prøver at snyde.

Det ser sådan her ud:



```c#
[Fact]
public void HævPenge_SkalKasteFejl_NårBeløbErNegativt()
{
    // 1. Arrange
    var konto = new BankKonto(1000m);

    // 2. & 3. Act & Assert i én
    // Her siger vi: "Jeg forventer, at koden kaster en ArgumentException, 
    // når jeg prøver at hæve -500 kr."
    Assert.Throws<ArgumentException>(() => konto.HævPenge(-500m));
}
```

**Konklusion:**

- Den første test sikrer, at **post-condition** er sand (at matematikken virker).
- Den anden test sikrer, at **pre-condition** håndhæves (at ugyldige input afvises).

Når du har skrevet disse tests og de er "grønne", kan du med god samvittighed sætte flueben ved *"Unit Tests bestået"* i din **Definition of Done**.

Giver AAA-strukturen mening for dig?





-------

# **Acceptkriterier** (Acceptance Criteria) og **Definition of Done** (DoD) 

 Mange blander **Acceptkriterier** (Acceptance Criteria) og **Definition of Done** (DoD) sammen.

For at forstå, hvordan pre- og post-conditions passer ind, skal vi kigge på tre niveauer. Her er sammenhængen:

### 1. Det "manglende led": Acceptkriterier (Acceptance Criteria)

Før vi når til Definition of Done, skal vi forbi **Acceptkriterier**.

Pre- og post-conditions er oftest den *tekniske oversættelse* af en User Storys acceptkriterier.

- **User Story (Menneskesprog):** "Som kunde vil jeg kunne hæve penge, så jeg kan købe varer."
- **Acceptkriterie (Regel):** "Man må ikke kunne hæve flere penge, end der står på kontoen."
- **Pre-condition (Koden):** `if (beløb > Saldo) throw Exception;`

> **Tommelfingerregel:**
>
> - **Acceptkriterier** er specifikke for *den ene* User Story (reglerne for netop denne funktion).
> - **Definition of Done** er generel for *alle* User Stories (kvalitetskrav til arbejdet).

------

### 2. Hvordan det kobler til Definition of Done (DoD)

Definition of Done er en tjekliste, der sikrer kvaliteten. Pre- og post-conditions indgår indirekte i DoD gennem **test**.

En typisk Definition of Done ser sådan ud:

1. Koden er skrevet.
2. **Alle Unit Tests er skrevet og bestået.** (HER bor pre/post-conditions!)
3. Ingen fejl (bugs) fundet.
4. Koden er gennemgået af en anden (Code Review).

**Koblingen er altså:**

For at du kan sætte flueben ved *"Unit Tests er bestået"* i din DoD, skal du have skrevet tests, der beviser, at dine **pre-conditions** (f.eks. afvis ugyldigt input) og **post-conditions** (f.eks. saldo er opdateret) virker som de skal.

------

### 3. Konkret eksempel: Bank-appen

Lad os tage vores C#-eksempel og se det gennem Agile-brillerne.

#### A. User Story

> "Som bankkunde vil jeg hæve penge, så jeg har kontanter."

#### B. Acceptkriterier (Specificeres af Product Owner)

1. Kunden må ikke kunne hæve et negativt beløb.
2. Kunden må ikke kunne hæve over sit max-beløb.
3. Saldoen skal være korrekt opdateret efter hævning.

#### C. Udviklerens opgave (Pre/Post-conditions i koden)

Her tager udvikleren Acceptkriterierne og laver dem til kode:

- *Til kriterie 1 (Kode):* **Pre-condition:** `if (beløb < 0) ...`
- *Til kriterie 3 (Kode):* **Post-condition:** Sikrer at `NySaldo == GammelSaldo - Beløb`.

#### D. Definition of Done (Tjeklisten for at lukke opgaven)

For at sige "Jeg er færdig" (Done), kigger udvikleren på teamets DoD:

- [x] **Er koden skrevet?** (Ja, metoden `HævPenge` findes).
- [x] **Er Acceptkriterier opfyldt?** (Ja, koden håndterer negative beløb via pre-conditions).
- [x] **Er der Unit Tests?** (Ja, jeg har lavet en test, der prøver at sende -100 kr ind for at se, om min pre-condition stopper det. Testen er grøn).
- [ ] **Code Review?** (Mangler).

------

### Opsamling: Hierarkiet

Du kan se det som en pyramide:

| **Niveau**          | **Begreb**                   | **Hvad er det?**                    | **Forhold til Pre/Post-conditions**                          |
| ------------------- | ---------------------------- | ----------------------------------- | ------------------------------------------------------------ |
| **Højest** (Proces) | **Definition of Done (DoD)** | Kvalitetsstempel for *alt* arbejde. | Kræver at der er *tests*, som beviser at dine pre/post-conditions virker. |
| **Mellem** (Krav)   | **Acceptkriterier**          | Regler for den *specifikke* story.  | Definerer *hvilke* pre- og post-conditions du skal kode.     |
| **Lavest** (Teknik) | **Pre/Post-conditions**      | Kodelinjer (`if`, `assert`).        | Den faktiske implementering af kravene.                      |

**Kort sagt:**

Du bruger **pre- og post-conditions** i din kode for at opfylde **Acceptkriterierne**, og du skriver tests for disse betingelser for at kunne opfylde **Definition of Done**.

