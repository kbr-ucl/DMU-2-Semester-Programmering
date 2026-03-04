# Aggregate Root, Entity & Value Object

*Forklaringsmateriale — Domain-Driven Design for begyndere*

---

## Introduktion

Denne forklarer om Domain-Driven Design (DDD). Du vil lære de tre centrale byggeklodser: Aggregate Root, Entity og Value Object.

> **Læringsmål**
>
> - Forstå forskellen mellem Entity og Value Object
> - Kunne identificere Aggregate Roots i et domæne
> - Vide hvornår man bruger Entity vs. Value Object
> - Kunne tegne en simpel aggregate-struktur

---

## 1. Hvad er Domain-Driven Design?

Domain-Driven Design (DDD) er en tilgang til softwareudvikling, hvor man modellerer software tæt efter det forretningsdomæne, den skal understøtte. I stedet for at starte med databasetabeller eller API-endpoints, starter man med at forstå forretningens sprog og begreber.

DDD blev introduceret af Eric Evans i bogen "Domain-Driven Design: Tackling Complexity in the Heart of Software" fra 2003. De tre begreber, vi fokuserer på i dag, er centrale "taktiske" mønstre i DDD.

Kernen i DDD er, at **koden skal tale samme sprog som forretningen**. Hvis en domæneekspert taler om "ordrer", "kunder" og "leveringsadresser", så skal koden indeholde klasser med netop disse navne.

---

## 2. Entity (Entitet)

En Entity er et objekt, der har en unik identitet. Det vigtigste kendetegn er, at to entities kan have de samme attributter, men alligevel være forskellige objekter, fordi de har forskellige identiteter.

### Kendetegn ved en Entity

- Har en unik identitet (f.eks. et ID eller nummer)
- Identiteten er stabil over tid – den ændrer sig ikke
- Andre attributter kan ændre sig (mutable)
- To entities med samme data men forskelligt ID er IKKE ens
- Har typisk en livscyklus (oprettes, ændres, slettes)

### Eksempel: Ordre og Ordrelinje

Tænk på to ordrer i en webshop. De indeholder begge det samme produkt, samme antal og samme leveringsadresse. Alligevel er de to forskellige ordrer, fordi de har forskellige ordre-ID'er – måske oprettet af to forskellige kunder.

```csharp
public class Order {
    public Guid Id { get; private set; }                // Unik identitet
    public Guid CustomerId { get; private set; }        // Kan ikke ændres udefra
    public DateTime CreatedAt { get; private set; }     // Kan ikke ændres udefra
    public Address ShippingAddress { get; private set; } // Kan udskiftes via metode

    // Equality baseret på ID, ikke attributter
    public override bool Equals(object obj) {
        return obj is Order o && o.Id == this.Id;
    }
}
```

Her ser vi, at to ordrer med præcis samme indhold stadig er **forskellige entities**, fordi de har hvert deres `Id`. Bemærk brugen af `private set` – det sikrer, at attributter kun kan ændres indefra klassen selv, typisk via metoder der håndhæver forretningsregler.

> **Tommelfingerregel**
>
> Spørg dig selv: "Hvis to objekter har præcis de samme data, er de så det SAMME objekt?"
>
> Hvis svaret er **NEJ** → det er en Entity.

---

## 3. Value Object (Værdiobjekt)

Et Value Object er det modsatte af en Entity: Det har ingen unik identitet. To Value Objects med de samme værdier er identiske og kan frit udskiftes.

### Kendetegn ved et Value Object

- Ingen unik identitet
- Defineres udelukkende af sine værdier (attributter)
- Immutable (kan ikke ændres efter oprettelse)
- To Value Objects med samme værdier ER ens
- Kan frit erstattes med en kopi

### Eksempel: Adresse

Tænk på en adresse. Hvis to kunder begge bor på "Nørrebrogade 42, 2200 København N", så er de to adresser identiske. Vi behøver ikke at skelne mellem dem med et ID.

```csharp
public record Address {
    public string Street { get; init; }
    public string City { get; init; }
    public string ZipCode { get; init; }

    // C# record giver automatisk værdi-baseret equality
    // new Address("Nørrebrogade 42", "København N", "2200")
    //   == new Address("Nørrebrogade 42", "København N", "2200")
    // → true!
}
```

### Flere eksempler på Value Objects

| Value Object | Attributter         | Hvorfor Value Object?         |
| ------------ | ------------------- | ----------------------------- |
| Money        | Amount, Currency    | 100 DKK = 100 DKK             |
| DateRange    | Start, End          | Samme periode = samme værdi   |
| Email        | Address (string)    | Samme emailadresse = identisk |
| Color        | R, G, B             | Rød er rød uanset kontekst    |
| Coordinate   | Latitude, Longitude | Samme punkt på kort = ens     |



---

## 4. Aggregate Root

Nu kommer vi til det mest centrale koncept: Aggregate Root. Et Aggregate er en klynge af relaterede Entities og Value Objects, der behandles som én samlet enhed. Aggregate Root er den øverste Entity, der fungerer som "dørvogter" for hele gruppen.

### Hvad er et Aggregate?

Et Aggregate er en konsistensgrænse. Det betyder, at alle ændringer inden for et Aggregate skal være konsistente på én gang. Udefra kan man kun kommunikere med Aggregatet gennem dets Root.

### Regler for Aggregates

1. **Aggregate Root er den eneste indgang** – udefra må man kun referere til rooten
2. **Interne objekter må ikke deles ud** – returner kopier eller Value Objects
3. **Hele Aggregatet gemmes og hentes som én enhed**
4. **Konsistensregler håndhæves af Root'en**

### Eksempel: Ordre-systemet

Et klassisk eksempel er en Order (ordre). Ordren er Aggregate Root, og den indeholder OrderLines (ordrelinjer). Udefra tilgår man altid ordren – aldrig en enkelt ordrelinje direkte.

```csharp
public class Order {                    // ← AGGREGATE ROOT
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    private readonly List<OrderLine> _lines = new();
    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();
    public Address ShippingAddress { get; private set; } // Value Object

    public void AddLine(Guid productId, int quantity, Money unitPrice) {
        // Forretningslogik håndhæves HER i rooten
        if (quantity <= 0)
            throw new ArgumentException("Antal skal være positivt");

        _lines.Add(new OrderLine(productId, quantity, unitPrice));
    }

    public void ChangeLineQuantity(Guid orderLineId, int newQuantity) {
        if (newQuantity <= 0)
            throw new ArgumentException("Antal skal være positivt");

        var line = _lines.FirstOrDefault(l => l.Id == orderLineId)
            ?? throw new InvalidOperationException("Ordrelinje ikke fundet");

        line.UpdateQuantity(newQuantity);  // Kun Order kan kalde denne
    }

    public void ChangeShippingAddress(Address newAddress) {
        // Value Object er immutable – vi erstatter med et nyt
        ShippingAddress = newAddress;
    }

    public decimal GetTotal() {
        return _lines.Sum(l => l.Quantity * l.UnitPrice.Amount);
    }
}

public class OrderLine {                // ← INTERN ENTITY
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }        // private set!
    public Money UnitPrice { get; private set; }     // Value Object

    internal OrderLine(Guid productId, int quantity, Money unitPrice) {
        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    internal void UpdateQuantity(int newQuantity) {
        // Kun kaldet af Order – aldrig udefra
        Quantity = newQuantity;
    }
}
```

Bemærk flere vigtige detaljer her:

- **`private set`** på alle properties – ingen kan ændre data udefra
- **`internal`** på OrderLine's constructor og `UpdateQuantity` – kun Order (i samme assembly) kan oprette og ændre ordrelinjer
- **`IReadOnlyList`** eksponerer ordrelinjerne, men forhindrer at man tilføjer/fjerner udefra
- Al validering sker i **Order** (Aggregate Root), ikke i OrderLine

> **Vigtig pointe**
>
> Læg mærke til, at Order indeholder en Customer-reference som et ID (eller et lille objekt), IKKE hele Customer-objektet. Customer er sit eget Aggregate Root med sin egen livscyklus.

### Hvorfor CustomerId og ikke et Customer-objekt?

En central regel i DDD er, at **aggregates kun refererer til hinanden via ID** – aldrig ved at holde hele objektet. Der er tre grunde til det:

**Konsistensgrænser.** Hvert aggregate garanterer konsistens inden for sin egen grænse. Hvis Order holdt et fuldt Customer-objekt, ville en ændring af kundens adresse pludselig påvirke ordren. Men det giver ikke mening – ordren blev afgivet med den adresse, kunden havde *dengang*. De to ting lever uafhængigt.

**Persistering.** Når du gemmer en Order, gemmer du hele aggregatet som én enhed. Hvis Customer hang med som et nested objekt, skulle du enten gemme kunden igen (dobbelt ejerskab) eller lave kompleks lazy-loading. Med et ID henter du bare kunden separat, når du har brug for det.

**Skalerbarhed.** Forestil dig, at Customer også har en liste af ordrer, som har ordrelinjer, som refererer til produkter... Pludselig loader du halve databasen i ét træk. ID-referencer bryder den kæde.

I praksis ser det sådan ud, når du har brug for kundedata:

```csharp
// I Order – kun et ID
public Guid CustomerId { get; private set; }

// Når du har brug for kunden, henter du den separat
var order = orderRepository.GetById(orderId);
var customer = customerRepository.GetById(order.CustomerId);
```

> **Tommelfingerregel**
>
> Inden for et aggregate kan du navigere frit mellem objekterne (Order → OrderLine → Money). Men **mellem aggregates** bruger du altid ID-referencer.

---

## 5. Visualisering: Aggregate-strukturen

En god måde at forstå forholdet mellem de tre begreber er at tegne det:

```
┌─────────────────────────────────────────┐
│          ORDER AGGREGATE                │
│                                         │
│  ┌───────────────────────────────────┐  │
│  │  Order (Aggregate Root)           │  │
│  │  - Id: Guid                       │  │
│  │  - ShippingAddress: Address [VO]  │  │
│  │  - CustomerId: Guid [ref]         │  │
│  └───────────────────────────────────┘  │
│         │                               │
│         ▼                               │
│  ┌───────────────────────────────────┐  │
│  │  OrderLine (Entity)               │  │
│  │  - Id: Guid                       │  │
│  │  - ProductId: Guid [ref]          │  │
│  │  - Quantity: int                  │  │
│  │  - UnitPrice: Money [VO]          │  │
│  └───────────────────────────────────┘  │
└─────────────────────────────────────────┘
```

---

## 6. Sammenligning: Entity vs. Value Object

| Egenskab         | Entity                | Value Object                 |
| ---------------- | --------------------- | ---------------------------- |
| **Identitet**    | Unik ID               | Ingen – defineres af værdier |
| **Equality**     | Baseret på ID         | Baseret på alle attributter  |
| **Mutabilitet**  | Mutable (kan ændres)  | Immutable (uforanderlig)     |
| **Livscyklus**   | Har egen livscyklus   | Følger sin ejer              |
| **Persistering** | Egen tabel (typisk)   | Embedded / owned type        |
| **Eksempler**    | Kunde, Ordre, Produkt | Adresse, Penge, Email        |

---

## 7. Praktiske designheuristikker

### Hvornår er noget en Entity?

- Du skal kunne skelne mellem to instanser med samme data
- Objektet har en livscyklus (oprettes, ændres, slettes)
- Andre dele af systemet refererer til objektet
- Objektets tilstand ændrer sig over tid

### Hvornår er noget et Value Object?

- Du kan frit erstatte objektet med et andet med samme værdier
- Objektet beskriver en egenskab eller måling
- Det giver ikke mening at ændre objektet – man laver et nyt i stedet
- Flere ting kan dele det samme Value Object

### Hvornår er noget en Aggregate Root?

- Objektet ejer og beskytter andre relaterede objekter
- Der er forretningsregler, der gælder på tværs af gruppen
- Gruppen skal gemmes og hentes som én enhed
- Udefra giver det kun mening at tale med det øverste objekt



---

## 8. Typiske begynderfejl

### Fejl 1: Alt bliver en Entity

Mange begyndere giver alt et ID. Men det fører til unødvendig kompleksitet. Spørg altid: "Har jeg brug for at skelne mellem to instanser med samme data?" Hvis ikke, er det sandsynligvis et Value Object.

### Fejl 2: For store Aggregates

Et Aggregate bør være så lille som muligt. Hvis dit Aggregate indeholder hundredvis af objekter, er det sandsynligvis for stort. Husk: Aggregatet er en konsistensgrænse, ikke en organisatorisk gruppe.

### Fejl 3: Direkte adgang til interne objekter

Hvis du kan hente en OrderLine direkte fra databasen (uden at gå gennem Order), bryder du Aggregate-mønstret. Al adgang skal gå gennem Root'en.

> **Opgave 3: Find fejlene**
>
> Nedenstående kode har designproblemer. Identificér mindst 3 problemer og forklar hvad du ville ændre.
>
> ```csharp
> public class Order {
>  public Guid Id { get; set; }              // Problem?
>  public List<OrderLine> Lines { get; set; } // Problem?
> }
> 
> public class OrderLine {
>  public Guid Id { get; set; }
>  public int Quantity { get; set; }          // Problem?
>  public Money UnitPrice { get; set; }
> }
> 
> // I et repository et sted:
> public class OrderLineRepository {            // Problem?
>  public OrderLine GetById(Guid id) { ... }
> }
> ```
>
> Hints:
>
> 1. Hvad sker der, hvis nogen ændrer `Quantity` direkte på en OrderLine?
> 2. Bør `Id` have en public setter?
> 3. Bør man kunne hente en OrderLine uden at gå gennem Order?
> 4. Bør `Lines` være en public `List<>` med setter?

---

## 9. Opsummering

**Entity** = objekt med unik identitet. To entities med samme data er stadig forskellige.

**Value Object** = objekt uden identitet. Defineres af sine værdier. Immutable.

**Aggregate Root** = den øverste Entity i en gruppe, der fungerer som dørvogter og håndhæver konsistens.

