# Kapitel 7 — Entity, Value Object og Aggregate Root

## Kapitel-ILOs

Efter at have læst dette kapitel kan du:

- *(Relational)*: **Forklare** forskellen mellem Entity, Value Object og Aggregate Root og **identificere** hvilken kategori en given klasse tilhører.
- *(Relational)*: **Anvende** Value Object-mønstret med C# `record`-typen til værdier i et domæne.
- *(Relational)*: **Designe** en aggregate-grænse der beskytter invariants og forhindrer ekstern adgang til indre entities.
- *(Extended Abstract)*: **Vurdere** om en eksisterende model har korrekte aggregate-grænser — eller om den lider af lækager der kompromitterer invariants.

> **Forudsætninger:** Du har læst kap. 1 (`private set`, indkapsling), kap. 4 (invariants, kontrakter) og kap. 6 (DDD som perspektiv).

---

## 7.1 Motivation

I MinKlinik har du indtil nu set `Konsultation` som "bare en klasse". Lad os se på den fra et nyt vinkel: *hvordan ved du hvilken konsultation du har?* Hvis to konsultationer har samme `Tidspunkt`, samme `PatientId` og samme `BehandlerId`, er de så samme konsultation?

Svaret er nej. De er to forskellige bookings. De har *identitet* — typisk repræsenteret med en `Guid Id`. To konsultationer med samme indhold men forskellige id'er er forskellige objekter i systemet.

Sammenlign med `Tidsinterval`:

```csharp
var ti1 = new Tidsinterval(new DateTime(2026, 5, 6, 10, 0, 0), new DateTime(2026, 5, 6, 10, 15, 0));
var ti2 = new Tidsinterval(new DateTime(2026, 5, 6, 10, 0, 0), new DateTime(2026, 5, 6, 10, 15, 0));
```

Er `ti1` og `ti2` "samme"? Ja — de repræsenterer det samme tidsinterval. De har ingen identitet udover deres indhold. To intervaller med samme `Fra` og `Til` er ækvivalente.

Det her er forskellen mellem en **Entity** og en **Value Object** — DDD's to mest grundlæggende byggeklodser. Når en Entity bliver det "rod" der ejer en gruppe relaterede objekter med samme livscyklus, har du en **Aggregate Root**.

> **Stop og tænk:** Tag begrebet "100 kr" som eksempel. Er en 100-kroneseddel det samme som en anden 100-kroneseddel? På banken er svaret: ja, de er ækvivalente — det er en Value Object. På et kriminalteknisk bord (fingeraftryk, serie-numre): nej, de er forskellige — det er Entities. Identitet afhænger af konteksten.

---

## 7.2 Begrebsapparat

| Begreb | Forklaring |
|--------|-----------|
| **Entity** | Et objekt med identitet. To Entities med samme indhold men forskellige id'er er forskellige. |
| **Value Object** | Et objekt uden identitet — defineret af sit indhold. To Value Objects med samme værdier er ækvivalente. |
| **Aggregate** | En klynge relaterede objekter behandlet som én enhed for konsistens. |
| **Aggregate Root** | Den ene Entity i en aggregate der repræsenterer hele klyngen udadtil. Ekstern kode må kun referere root, ikke indre objekter. |
| **Identitet** *(identity)* | Den egenskab der gør et objekt unikt — typisk en `Guid Id`. |
| **Value-equality** | Equality baseret på indhold, ikke reference. C# `record` giver det automatisk. |
| **Reference-equality** | Equality baseret på objekt-reference. C# `class`'s default. |

---

## 7.3 Kerneindhold

### 7.3.1 Entity — identitet er det vigtigste

En Entity er karakteriseret af én ting: identitet. Selv hvis alle dens andre felter ændrer sig, er den stadig "den samme" så længe identiteten består:

```csharp
public class Patient
{
    public Guid Id { get; private set; }
    public string Navn { get; private set; }
    public string Cpr { get; private set; }
    public string Email { get; private set; }

    public void OpdatérEmail(string nyEmail) { /* ... */ }
}
```

En patient med id `abc-123` er den samme patient hvis hun ændrer email, navn (efter ægteskab) eller adresse. Identiteten er forankret i `Id` — ikke i indholdet.

Equality for Entities er id-baseret:

```csharp
public abstract class Entity
{
    public Guid Id { get; protected set; }

    public override bool Equals(object? obj) =>
        obj is Entity other && Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();
}
```

Konvention i MinKlinik: alle Entities arver fra en `Entity`-base-klasse der håndterer `Id` og equality. Aggregate Roots arver derefter fra `AggregateRoot : Entity`.

### 7.3.2 Value Object — indhold er det vigtigste

En Value Object har ingen identitet. To value objects med samme indhold er ens:

```csharp
public sealed record Tidsinterval(DateTime Fra, DateTime Til)
{
    public TimeSpan Varighed => Til - Fra;

    public bool OverlapperMed(Tidsinterval anden) =>
        Fra < anden.Til && Til > anden.Fra;
}
```

C# `record`-typen giver automatisk:

- Value-equality (`ti1 == ti2` hvis felterne er ens)
- `ToString()` med felternes værdier
- Deconstruction
- `with`-expressions for ikke-destruktiv opdatering

Det gør `record` til den naturlige default for Value Objects. Brug `class` kun hvis du har brug for arv eller mutation — og Value Objects bør være immutable.

#### Værdier og adfærd

Value Objects er ikke kun datacontainere. De ejer deres egne regler. I `Tidsinterval` er `OverlapperMed` en metode der udnytter at klassen har både `Fra` og `Til`. Det er rig modellering på Value Object-niveau.

Almindelige Value Objects i forretningskode:

- `Pengebeløb` (beløb + valuta)
- `Adresse` (gade, postnummer, by)
- `EmailAdresse` (med valideret format)
- `Tidsinterval`
- `Cpr`
- `Telefonnummer`

Alle har det fælles træk at de defineres af deres indhold — og at de gerne må have intelligens.

> **Stop og tænk:** En `EmailAdresse`-Value Object kunne validere at strengen indeholder `@` i konstruktøren. Hvad opnår du ved at have en `EmailAdresse` Value Object frem for at typere felter som `string`?

(Svaret: typesystemet håndhæver gyldighed. Du kan ikke ved et uheld bruge en almindelig string hvor en email kræves. Du undgår at gentage email-validering på 17 forskellige steder.)

### 7.3.3 Aggregate Root — én indgang til klyngen

Et aggregate er en klynge entities og value objects der hører sammen og deler livscyklus. En `Konsultation` har et `Tidsinterval`, en status, et `PatientId` og et notat. De hænger sammen som én enhed.

Aggregate Root er den ene Entity der *repræsenterer* aggregatet udadtil:

```csharp
public class Konsultation : AggregateRoot
{
    public Tidsinterval Tidspunkt { get; private set; } = null!;
    public KonsultationStatus Status { get; private set; }
    public string Notat { get; private set; } = string.Empty;

    private readonly List<KonsultationsBesked> _interneBeskeder = new();
    public IReadOnlyList<KonsultationsBesked> InterneBeskeder => _interneBeskeder;

    public void TilføjBesked(string tekst, Guid forfatterId)
    {
        if (Status == KonsultationStatus.Aflyst)
            throw new DomainException("Kan ikke tilføje besked til en aflyst konsultation.");

        _interneBeskeder.Add(new KonsultationsBesked(tekst, forfatterId, DateTime.UtcNow));
    }
}
```

Bemærk to ting:

1. **Indre samling er privat.** `_interneBeskeder` er `private readonly`. Den er kun læselig udefra via `IReadOnlyList<>`. Ekstern kode kan *ikke* skrive direkte: `k.InterneBeskeder.Add(...)` kompilerer ikke.

2. **Ændringer går gennem aggregate root.** For at tilføje en besked skal du kalde `k.TilføjBesked(...)` — som håndhæver invariants. Du kan ikke omgå reglerne ved at manipulere indre objekter.

Det her er aggregatets *indkapsling på højere niveau*. Hvor kap. 1's `private set` indkapsler et felt, indkapsler aggregate root sin indre struktur.

#### Tre regler for aggregate-design

**Reference-regel:** Ekstern kode må kun referere til aggregate roots, ikke til indre entities. Hvis en `KonsultationsBesked` skulle deles på tværs af konsultationer, måtte den selv være et aggregate root med et eget `Id`.

**Transaktionsregel:** En transaktion ændrer præcis ét aggregate. Hvis du ændrer både `Konsultation` og `Patient` i samme operation, er det to aggregates — det skal ske i to separate `SaveChangesAsync`, og du må overveje hvordan du håndterer at det ene kan lykkes mens det andet fejler.

**Lille-aggregate-regel:** Aggregates skal være så små som muligt. Hvis du finder dig selv at samle hele klinikken under én root (`Klinik` med listen af alle patienter, behandlere, konsultationer), har du tabt tråden. Hver klynge med egen invariant er sit eget aggregate.

### 7.3.4 Identifikation — hvordan ved du hvad der er hvad?

Når du modellerer et nyt domæne, hvordan beslutter du om noget er Entity, Value Object eller Aggregate Root?

**Spørgsmål 1: Har det identitet udover indholdet?**

- Ja → Entity
- Nej → Value Object

**Spørgsmål 2: Hvis Entity — bærer det reglerne for en hel klynge?**

- Ja → Aggregate Root
- Nej → Almindelig Entity (indre i et aggregate)

For MinKlinik:

| Klasse | Kategori | Begrundelse |
|--------|----------|-------------|
| `Patient` | Aggregate Root | Egen identitet, egen livscyklus, egne invariants |
| `Behandler` | Aggregate Root | Som Patient |
| `Konsultation` | Aggregate Root | Egen livscyklus med statusovergange |
| `Behandlingstype` | Aggregate Root | Egen livscyklus, kan eksistere uafhængigt |
| `Tidsinterval` | Value Object | Defineret af `Fra` og `Til` |
| `KonsultationStatus` | Enum (en form for VO) | Lukket sæt af lovlige værdier |
| `KonsultationsBesked` | Indre Entity | Identitet kun inden for konsultations-aggregatet |

> **Stop og tænk:** Hvis to klinikker fusionerer og deres patient-data merges, hvordan håndterer I patienter med samme CPR? Kan en Patient med Id `A` og en Patient med Id `B` være "samme person"? Svaret er ja på menneskeplan, men nej på system-plan — fordi de har forskellige identiteter. Det er en klassisk udfordring der kræver en eksplicit "merge"-proces.

### 7.3.5 LSP og kontrakter på aggregate-niveau

Kap. 4 introducerede pre/post-conditions, og kap. 2 introducerede LSP. På aggregate-niveau bliver disse begreber særligt vigtige:

- Hver public metode på et aggregate root er en *kontrakt* — pre, post og hvilke invariants der bevares.
- Hvis du arver fra et aggregate (sjældent, men det forekommer), gælder LSP: subklassen må ikke styrke pre eller svække post.

I praksis bruger DDD sjældent arv mellem aggregates. Mere almindeligt er *komposition* — `Konsultation` *har* et `Tidsinterval`, *har* en `KonsultationStatus`. Komposition holder LSP-bekymringerne væk og giver lavere kobling.

---

## 7.4 Anvendelse i MinKlinik

I `MinKlinik`-repositoriet, tag `kap-07`, finder du den formaliserede aggregate-struktur:

```csharp
namespace MinKlinik.Domain;

public abstract class Entity
{
    public Guid Id { get; protected set; }
    public override bool Equals(object? obj) => obj is Entity e && Id == e.Id;
    public override int GetHashCode() => Id.GetHashCode();
}

public abstract class AggregateRoot : Entity { }

public class Konsultation   : AggregateRoot { /* ... */ }
public class Patient        : AggregateRoot { /* ... */ }
public class Behandler      : AggregateRoot { /* ... */ }
public class Behandlingstype: AggregateRoot { /* ... */ }

public sealed record Tidsinterval(DateTime Fra, DateTime Til)
{
    public TimeSpan Varighed => Til - Fra;
    public bool OverlapperMed(Tidsinterval anden) =>
        Fra < anden.Til && Til > anden.Fra;
}

public enum KonsultationStatus { Planlagt, Afsluttet, Aflyst }
```

Bemærk:

1. **`AggregateRoot` er en marker-klasse.** Den arver fra `Entity` uden at tilføje noget. Formålet er at gøre eksplicit hvilke klasser der må holdes i et `IRepository<T>` (kap. 11).
2. **`Tidsinterval` er en `record`.** Value-equality, immutability og adfærd — alt på én gang.
3. **`KonsultationStatus` er en enum.** En lukket mængde af lovlige værdier — det enkleste Value Object-mønster.
4. **Ingen indre entities lækker.** Hvis `Konsultation` indeholdt indre objekter, ville de være eksponeret som `IReadOnlyList<>` og kun ændres via metoder på rod-objektet.

Sammenlign med `2026F-DMVE251-2-sem/DemoKode/DetLilleBibliotek/Domain` der viser samme mønster i et bibliotekssystem: `Bog` og `Medlem` som aggregate roots med deres egne value objects.

---

## 7.5 Sammenhæng

Det her kapitel introducerede DDD's tre centrale byggeklodser. Det bygger på kap. 6's perspektiv-skift og åbner op for det næste:

- **Kap. 8** anvender Aggregate Roots med invariants, factory-metoder og guard clauses fra kap. 1 og 4. Det er kernen i den disciplin der gør domain-first praktisk.
- **Kap. 11** designer Facade- og Repository-interfaces. Et `IRepository<T>` i Facade-laget kan typeres med `where T : AggregateRoot` — det er marker-typen der gør reglen *"kun aggregate roots kan persisteres direkte"* til en compile-time regel.
- **Kap. 12** afspejler aggregate-grænser i EF Core-mapping. Value Objects mappes med `OwnsOne` eller `ComplexProperty`. Indre entities ligger i samme tabel som deres aggregate root eller deler key med den.

Aggregate-mønstret er det der binder DDD's ideer sammen til praktisk arkitektur. Uden det fragmenterer modellen sig hurtigt.

---

## 7.6 Hands-on

> **Hands-on: anvend dette i et eget projekt**
> Som praktisk øvelse kan du tegne aggregate-grænserne i jeres SportZone-domæne:
>
> 1. *Identificér aggregate roots:* Hvilke klasser har egen identitet og egen livscyklus? Tegn dem som de eneste klasser ekstern kode må referere.
> 2. *Identificér value objects:* Find værdier i jeres model der defineres af indhold (tidspunkter, beløb, identifikatorer). Implementér dem som `record`s.
> 3. *Tjek transaktionsreglen:* Hvis I i en use case ændrer to aggregates, så split det. En transaktion = ét aggregate.
> 4. *Tjek reference-reglen:* Hvis I har offentlige `List<T>` eller direkte references til indre entities, kapsl dem væk bag aggregate root-metoder.
>
> *Underviser-noter:* konkrete acceptkriterier for uge 4 findes i Bilag E.

---

## 7.7 Øvelser

### Øvelse 7.1 *(Multistructural — describe, identify)*

For hver af følgende seks klasser, beslut om de er Entity, Value Object eller Aggregate Root. Begrund i én sætning.

1. `Patient` (med Id, Navn, Cpr)
2. `Cpr` (10-cifret streng)
3. `Konsultation` (med Id, Tidspunkt, Status og indre liste af besked-noter)
4. `KonsultationsBesked` (kun gyldig som del af en Konsultation)
5. `Pengebeløb` (decimal + valuta)
6. `Klinik` (med liste af patienter, behandlere, konsultationer)

### Øvelse 7.2 *(Relational — apply)*

Du har denne klasse:

```csharp
public class Adresse {
    public string Gade { get; set; } = "";
    public string Postnummer { get; set; } = "";
    public string By { get; set; } = "";
}
```

Refaktorér til en korrekt Value Object:

1. Gør den til en `record` der validerer i konstruktøren (postnummer skal være 4 cifre).
2. Tilføj en `ErISammeBy(Adresse anden)`-metode.
3. Skriv tre tests (kap. 5): én for konstruktør-validering, én for `ErISammeBy = true`, én for `ErISammeBy = false`.

### Øvelse 7.3 *(Relational — analyse)*

Læs denne kode:

```csharp
public class Bestilling {
    public Guid Id { get; set; }
    public List<BestillingsLinje> Linjer { get; set; } = new();
    public decimal SamletPris => Linjer.Sum(l => l.Pris * l.Antal);
}
public class BestillingsLinje {
    public Guid VareId { get; set; }
    public int Antal { get; set; }
    public decimal Pris { get; set; }
}

// Brugt et sted i koden:
bestilling.Linjer.Add(new BestillingsLinje { VareId = id, Antal = -3, Pris = -50 });
```

1. Identificér mindst tre brud på aggregate-mønstret.
2. Refaktorér `Bestilling` til en korrekt aggregate root med en `TilføjLinje`-metode der validerer.
3. Bør `BestillingsLinje` være en Entity, en Value Object eller en indre del af aggregatet? Begrund.

### Øvelse 7.4 *(Extended Abstract — evaluate, justify)*

Du designer et e-handelssystem. En kollega foreslår at `Order`, `OrderLine` og `Customer` alle er aggregate roots med separate repositories.

1. Argumentér for og imod den opsplitning ud fra (a) reference-reglen, (b) transaktionsreglen, (c) lille-aggregate-reglen.
2. Hvad er sandsynligvis den korrekte aggregate-struktur for systemet? Begrund.
3. Hvilke konsekvenser ville en forkert grænsedragning have for performance og konsistens?

> Facit findes i bilag B.7

---

## 7.8 Litteratur og videre læsning

### Suppleterende noter
- *DDD — Aggregate Root og Entity og Value Object* — uddybende note med flere eksempler.
- *DDD — Entity vs Aggregate Root* — fokuseret på den specifikke distinktion mellem indre entities og roots.

### Bøger
- Eric Evans, *Domain-Driven Design* (2003), kap. 5-6 ("A Model Expressed in Software", "The Life Cycle of a Domain Object").
- Vaughn Vernon, *Implementing Domain-Driven Design* (2013), kap. 5-10 — meget kodeorienteret.
- Vaughn Vernon, *Effective Aggregate Design* (3-delt artikel-serie, fri online) — den definitive guide til aggregate-grænser.

### Demo-kode
- `2026F-DMVE251-2-sem/DemoKode/MinKlinik/src/MinKlinik.Domain` — Konsultation, Patient, Tidsinterval.
- `2026F-DMVE251-2-sem/DemoKode/DetLilleBibliotek/Domain` — Bog, Medlem, value objects.
- `2026F-DMVE251-2-sem/DemoKode/EfValueObjectsSqlServer2025Demo` — viser hvordan Value Objects mappes med EF Core 10's `ComplexProperty` (uddybes i kap. 12).
