# Hvordan nedbryder man en systembeskrivelse til et objektorienteret system der kan implementeres med c#



At nedbryde en systembeskrivelse til et objektorienteret system i C# er en disciplin, hvor man går fra noget abstrakt og ofte tekstbaseret til en konkret struktur af klasser, interfaces og relationer. Det er lidt som at oversætte en idé til en arkitektur, der faktisk kan bygges. Her er en proces, der både er praktisk og anvendelig.

------

# 🧩 Trin-for-trin: Fra systembeskrivelse til objektorienteret design i C#

## 1. **Forstå systembeskrivelsen**

Start med at identificere:

- Hvad systemet *skal gøre* (funktionelle krav)
- Hvilke regler der gælder (forretningslogik)
- Hvilke aktører der interagerer med systemet (brugere, eksterne systemer)

👉 Her handler det om at få overblik, ikke om at tænke i klasser endnu.

------

## 2. **Identificér domænebegreber (substantiver)**

Læs systembeskrivelsen og marker alle vigtige substantiver:

- Kunde
- Ordre
- Produkt
- Faktura
- Betaling

Disse bliver ofte kandidater til **klasser**.

👉 Hvis du kan tegne dem som bokse på et whiteboard, er du på rette vej.

------

## 3. **Identificér handlinger (verber)**

Handlinger bliver ofte:

- Metoder på klasser
- Services
- Use cases

Eksempler:

- "Kunden afgiver en ordre"
- "Systemet beregner totalpris"
- "Ordren sendes"

👉 Verber hjælper dig med at finde *adfærd* i systemet.

------

## 4. **Gruppér ansvar (Single Responsibility Principle)**

For hver domæneklasse: spørg *hvilket ansvar har denne klasse?*

Eksempel:

- **Ordre**: indeholder ordrelinjer, kan beregne totalpris
- **Kunde**: kontaktinfo, historik
- **Produkt**: pris, navn, lagerstatus

👉 Hvis en klasse får for mange ansvarsområder, del den op.

------

## 5. **Definér relationer mellem klasser**

Typiske relationer:

- **Association**: Kunde → Ordre
- **Aggregation**: Ordre → Ordrelinjer
- **Komposition**: Ordrelinje → Produkt (ofte stærk afhængighed)
- **Arv**: Betalingsmetode → Kreditkortbetaling / MobilePay

👉 Brug UML-diagrammer hvis det hjælper.

------

## 6. **Identificér services og forretningslogik**

Ikke alt hører hjemme i domæneklasserne.

Eksempler på services:

- `OrderService` (opret ordre, valider ordre)
- `PaymentService`
- `EmailService`

👉 Services håndterer processer, domæneklasser håndterer data + adfærd.

------

## 7. **Lav interfaces hvor der er variation**

Hvis noget kan have flere implementeringer:

- Betaling
- Notifikationer
- Lagerstyring

Så lav et interface:

```csharp
public interface IPaymentProvider
{
    void ProcessPayment(Order order);
}
```

------

## 8. **Oversæt til C#-klasser**

Når du har klasser, relationer og ansvar på plads, kan du begynde at kode.

Eksempel:

```csharp
public class Order
{
    public int Id { get; set; }
    public Customer Customer { get; set; }
    public List<OrderLine> Lines { get; set; } = new();

    public decimal GetTotal()
    {
        return Lines.Sum(l => l.Quantity * l.Product.Price);
    }
}
```

------

## 9. **Iterér og raffinér**

Objektorienteret design er *iterativt*. Du bliver klogere, efterhånden som du modellerer.

------

# 🎯 Kort sagt

Du nedbryder en systembeskrivelse ved at:

1. Forstå kravene
2. Identificere domæneobjekter
3. Bestemme deres ansvar
4. Definere relationer
5. Placere adfærd de rigtige steder
6. Oversætte det hele til C#-klasser og interfaces

------

# Kan du ændre kodeeksemplerne så de er mere indkapslede. Konkret bør der ikke være public setters





Selvfølgelig — stærk indkapsling er en af de vigtigste discipliner i objektorienteret design, og C# giver dig flere gode værktøjer til det. Lad mig vise dig, hvordan de tidligere eksempler kan skrues sammen, så objekterne bliver mere robuste, uforanderlige og ansvarlige for deres egen tilstand.

Jeg giver dig både en *klassisk* tilgang og en *mere moderne* C#‑stil.

------

# 🔒 Mere indkapslede C#‑klasser (uden public setters)

## 🎯 Principper vi følger

- Ingen `public set` på properties
- Tilstand ændres kun gennem metoder
- Konstruktorer sikrer gyldig initialisering
- Samlinger eksponeres som *read‑only views*
- Domæneobjekter beskytter deres invariants

------

# 🧱 Eksempel: Order, Customer, Product

## **Product** (immutable)

```csharp
public class Product
{
    public int Id { get; }
    public string Name { get; }
    public decimal Price { get; }

    public Product(int id, string name, decimal price)
    {
        if (price < 0)
            throw new ArgumentException("Price cannot be negative");

        Id = id;
        Name = name;
        Price = price;
    }
}
```

------

## **OrderLine** (immutable except quantity)

```csharp
public class OrderLine
{
    public Product Product { get; }
    public int Quantity { get; private set; }

    public OrderLine(Product product, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive");

        Product = product;
        Quantity = quantity;
    }

    public void ChangeQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be positive");

        Quantity = newQuantity;
    }

    public decimal LineTotal => Quantity * Product.Price;
}
```

------

## **Order** (helt indkapslet, styrer selv sine linjer)

```csharp
public class Order
{
    private readonly List<OrderLine> _lines = new();

    public int Id { get; }
    public Customer Customer { get; }

    public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();

    public Order(int id, Customer customer)
    {
        Id = id;
        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
    }

    public void AddLine(Product product, int quantity)
    {
        _lines.Add(new OrderLine(product, quantity));
    }

    public void RemoveLine(OrderLine line)
    {
        _lines.Remove(line);
    }

    public decimal GetTotal()
    {
        return _lines.Sum(l => l.LineTotal);
    }
}
```

------

## **Customer** (indkapslet, men fleksibel)

```csharp
public class Customer
{
    public int Id { get; }
    public string Name { get; private set; }
    public string Email { get; private set; }

    public Customer(int id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
    }

    public void UpdateContactInfo(string name, string email)
    {
        Name = name;
        Email = email;
    }
}
```

------

# 🧠 Hvorfor denne stil er stærkere

- Du kan **ikke** ændre objektets tilstand udefra ved et uheld
- Du tvinger al mutation gennem **metoder**, hvor du kan validere
- Du får et domæne, der er **robust**, **forudsigeligt** og **testbart**
- Du undgår “anemiske” modeller, hvor alt er public data uden adfærd

------



# Version med records





Selvfølgelig — records kan være en elegant måde at skabe mere *immutable* og kompakte domæneobjekter på. Men der er et par vigtige nuancer:

- **Records er som udgangspunkt immutable**, men du kan kombinere dem med *private setters*, *init‑only properties*, eller *mutable fields* hvis du vil.
- **Domain‑driven design** anbefaler ofte *immutable value objects* og *mutable entities*.
   → Derfor giver det mening at bruge **records til value objects** og **classes til entities**.
   Men hvis du vil køre *ren record‑stil*, kan du sagtens.

Her får du begge tilgange.

------

# 🧱 Version 1: Records som *immutable value objects*

Dette er den mest idiomatiske brug af records.

## **Product** (value object)

```csharp
public record Product(int Id, string Name, decimal Price);
```

## **OrderLine** (value object)

```csharp
public record OrderLine(Product Product, int Quantity)
{
    public decimal LineTotal => Quantity * Product.Price;
}
```

Her er alt uforanderligt. Hvis du vil ændre quantity, laver du en ny instans:

```csharp
var updated = line with { Quantity = 5 };
```

------

# 🧱 Version 2: Records som *entities* (med kontrolleret mutation)

Records kan også bruges som entities, men så skal du styre mutation via metoder.

## **Customer**

```csharp
public record Customer
{
    public int Id { get; init; }
    public string Name { get; private set; }
    public string Email { get; private set; }

    public Customer(int id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
    }

    public void UpdateContactInfo(string name, string email)
    {
        Name = name;
        Email = email;
    }
}
```

------

# 🧱 Version 3: Order som record med indkapslet liste

Records understøtter mutable felter, så længe du indkapsler dem korrekt.

```csharp
public record Order
{
    private readonly List<OrderLine> _lines = new();

    public int Id { get; init; }
    public Customer Customer { get; init; }

    public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();

    public Order(int id, Customer customer)
    {
        Id = id;
        Customer = customer;
    }

    public void AddLine(Product product, int quantity)
    {
        _lines.Add(new OrderLine(product, quantity));
    }

    public void RemoveLine(OrderLine line)
    {
        _lines.Remove(line);
    }

    public decimal GetTotal() => _lines.Sum(l => l.LineTotal);
}
```

Her får du:

- Immutable metadata (`Id`, `Customer`)
- Mutable indkapslet tilstand (`_lines`)
- Ingen public setters

------

# 🧠 Hvilken stil bør du vælge?

| Type                               | Brug class | Brug record                              |
| ---------------------------------- | ---------- | ---------------------------------------- |
| **Entity** (har identitet)         | ✔️          | ✔️ (men kræver omtanke)                   |
| **Value object** (ingen identitet) | ✔️          | ✔️✔️ bedst                                 |
| **Immutable data**                 | ✔️          | ✔️✔️ bedst                                 |
| **Mutable domæneobjekter**         | ✔️✔️ bedst   | ✔️ (men kræver private setters / metoder) |

------





___________

# Version hvor entities er med class og values er med record



Selvfølgelig — det er faktisk en rigtig sund arkitektur:
 **Entities = class** (fordi de har identitet og typisk ændrer tilstand)
 **Value Objects = record** (fordi de er uforanderlige og sammenlignes på værdi)

Her får du en fuld, konsistent model baseret på det princip.

------

# 🧱 **Value Objects (records)**

Value objects er immutable og repræsenterer *værdier*, ikke identitet.

## **Product** (value object)

```csharp
public record Product(int Id, string Name, decimal Price);
```

## **OrderLine** (value object)

```csharp
public record OrderLine(Product Product, int Quantity)
{
    public decimal LineTotal => Quantity * Product.Price;
}
```

> Bemærk: Hvis du vil ændre quantity, laver du en ny instans:
>  `line = line with { Quantity = 5 };`

------

# 🧱 **Entities (classes)**

Entities har identitet og kontrolleret mutation.

## **Customer** (entity)

```csharp
public class Customer
{
    public int Id { get; }
    public string Name { get; private set; }
    public string Email { get; private set; }

    public Customer(int id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
    }

    public void UpdateContactInfo(string name, string email)
    {
        Name = name;
        Email = email;
    }
}
```

------

## **Order** (entity med indkapslet liste)**

```csharp
public class Order
{
    private readonly List<OrderLine> _lines = new();

    public int Id { get; }
    public Customer Customer { get; }

    public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();

    public Order(int id, Customer customer)
    {
        Id = id;
        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
    }

    public void AddLine(Product product, int quantity)
    {
        _lines.Add(new OrderLine(product, quantity));
    }

    public void RemoveLine(OrderLine line)
    {
        _lines.Remove(line);
    }

    public decimal GetTotal() => _lines.Sum(l => l.LineTotal);
}
```

------

# 🧠 Hvorfor denne kombination er stærk

| Type                      | Brug               | Fordel                                           |
| ------------------------- | ------------------ | ------------------------------------------------ |
| **class (entity)**        | Customer, Order    | Identitet, kontrolleret mutation                 |
| **record (value object)** | Product, OrderLine | Immutable, sammenlignes på værdi, nemme at teste |

