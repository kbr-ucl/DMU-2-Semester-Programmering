# Kan du hjælpe mig med at forstå soild principperne

Selvfølgelig — og fedt at du vil forstå SOLID! Det er fem grundlæggende principper inden for objektorienteret design, der hjælper dig med at skrive kode, som er lettere at vedligeholde, teste og udvide. Jeg forklarer dem på en jordnær måde, uden jargon.

------

# 🧱 SOLID-principperne — forklaret enkelt

## 🟦 **S — Single Responsibility Principle (SRP)**

**En klasse skal kun have én grund til at ændre sig.**  
 Med andre ord: Én klasse = Én opgave.

**Hvorfor?**  
 Hvis en klasse gør for mange ting, bliver den svær at teste og ændre uden at ødelægge noget andet.

**Eksempel:**  
 En `Invoice`-klasse bør *ikke* både beregne moms, gemme data i databasen og sende e-mails. Del det op i flere klasser.

------

## 🟩 **O — Open/Closed Principle (OCP)**

**Software skal være åben for udvidelse, men lukket for ændring.**

**Hvad betyder det?**  
 Du skal kunne tilføje ny funktionalitet *uden* at ændre eksisterende kode.

**Eksempel:**  
 Hvis du har en betalingsløsning, skal du kunne tilføje MobilePay uden at ændre koden for Visa og Mastercard — du tilføjer bare en ny klasse.

------

## 🟨 **L — Liskov Substitution Principle (LSP)**

**En subclass skal kunne bruges i stedet for sin superclass uden problemer.**

**Kort sagt:**  
 Hvis `Bird` har en metode `fly()`, så skal en `Penguin` *ikke* arve fra `Bird`, fordi pingviner ikke kan flyve.

**Hvorfor?**  
 Arv skal give mening — ellers opstår mærkelige bugs.

------

## 🟧 **I — Interface Segregation Principle (ISP)**

**Klienter skal ikke tvinges til at implementere metoder, de ikke bruger.**

**Eksempel:**  
 Et stort interface som `IAnimal` med metoderne `Fly()`, `Swim()`, `Run()` er dårligt.
 En fisk skal ikke tvinges til at implementere `Run()`.

**Løsning:**  
 Lav mindre, mere specifikke interfaces:

- `IFlyable`
- `ISwimmable`
- `IRunnable`

------

## 🟥 **D — Dependency Inversion Principle (DIP)**

**Afhæng af abstraktioner, ikke konkrete implementeringer.**

**Hvad betyder det?**  
 Klasser skal ikke kende detaljer om hinanden — de skal kommunikere gennem interfaces.

**Eksempel:**  
 I stedet for at `Car` opretter en `GasEngine` direkte, skal den modtage et interface `IEngine`.
 Så kan du senere skifte til `ElectricEngine` uden at ændre `Car`.

------

# 🎯 Kort opsummeret i én tabel

| Bogstav | Navn                  | Essens                                          |
| ------- | --------------------- | ----------------------------------------------- |
| S       | Single Responsibility | Én klasse = én opgave                           |
| O       | Open/Closed           | Udvid, men ændr ikke                            |
| L       | Liskov Substitution   | Subklasser skal opføre sig som deres baseklasse |
| I       | Interface Segregation | Små, specifikke interfaces                      |
| D       | Dependency Inversion  | Afhæng af abstraktioner, ikke konkrete klasser  |

------

Hvis du vil, kan jeg også:

- give konkrete kodeeksempler (C#, Java, Python — hvad du vil)
- vise gode og dårlige eksempler
- lave en lille øvelse, så du kan teste din forståelse

Hvad vil du gerne dykke videre ned i?

________

# giv konkrete eksempler i c#.  Og herefter en lille øvelse

Selvfølgelig — lad os gøre SOLID helt konkret med C#‑eksempler og bagefter en lille øvelse, hvor du selv kan prøve kræfter med principperne.

------

# 🟦 S — Single Responsibility Principle (SRP)

### ❌ Dårligt eksempel (én klasse gør for meget)

```csharp
public class Invoice
{
    public decimal CalculateTotal() { /* ... */ }

    public void SaveToDatabase() { /* ... */ }

    public void SendEmail() { /* ... */ }
}
```

### ✅ Godt eksempel (opdelt i ansvar)

```csharp
public class Invoice
{
    public decimal CalculateTotal() { /* ... */ }
}

public class InvoiceRepository
{
    public void Save(Invoice invoice) { /* ... */ }
}

public class EmailService
{
    public void SendInvoice(Invoice invoice) { /* ... */ }
}
```

------

# 🟩 O — Open/Closed Principle (OCP)

### ❌ Dårligt eksempel (ændrer eksisterende kode)

```csharp
public class PaymentService
{
    public void Pay(string method)
    {
        if (method == "Visa") { /* ... */ }
        else if (method == "Mastercard") { /* ... */ }
        else if (method == "MobilePay") { /* ... */ } // nyt → ændrer koden
    }
}
```

### ✅ Godt eksempel (åben for udvidelse)

```csharp
public interface IPaymentMethod
{
    void Pay();
}

public class VisaPayment : IPaymentMethod
{
    public void Pay() { /* ... */ }
}

public class MobilePayPayment : IPaymentMethod
{
    public void Pay() { /* ... */ }
}

public class PaymentService
{
    public void Pay(IPaymentMethod method)
    {
        method.Pay();
    }
}
```

------

# 🟨 L — Liskov Substitution Principle (LSP)

### ❌ Dårligt eksempel (subclass bryder forventninger)

```csharp
public class Bird
{
    public virtual void Fly() { }
}

public class Penguin : Bird
{
    public override void Fly()
    {
        throw new Exception("Penguins can't fly!");
    }
}
```

### ✅ Godt eksempel (korrekt hierarki)

```csharp
public abstract class Bird { }

public interface IFlyingBird
{
    void Fly();
}

public class Sparrow : Bird, IFlyingBird
{
    public void Fly() { /* ... */ }
}

public class Penguin : Bird
{
    // Ingen Fly-metode → ingen brud på forventninger
}
```

------

# 🟧 I — Interface Segregation Principle (ISP)

### ❌ Dårligt eksempel (for stort interface)

```csharp
public interface IAnimal
{
    void Fly();
    void Run();
    void Swim();
}
```

### ✅ Godt eksempel (små, fokuserede interfaces)

```csharp
public interface IFlyable { void Fly(); }
public interface IRunable { void Run(); }
public interface ISwimmable { void Swim(); }

public class Dog : IRunable { public void Run() { } }
public class Duck : IFlyable, ISwimmable { public void Fly() { } public void Swim() { } }
```

------

# 🟥 D — Dependency Inversion Principle (DIP)

### ❌ Dårligt eksempel (afhængig af konkret klasse)

```csharp
public class Car
{
    private GasEngine _engine = new GasEngine();
}
```

### ✅ Godt eksempel (afhængig af abstraktion)

```csharp
public interface IEngine
{
    void Start();
}

public class GasEngine : IEngine
{
    public void Start() { /* ... */ }
}

public class ElectricEngine : IEngine
{
    public void Start() { /* ... */ }
}

public class Car
{
    private readonly IEngine _engine;

    public Car(IEngine engine)
    {
        _engine = engine;
    }
}
```

------

# 🧪 Lille øvelse til dig

Her er en klasse, der *bryder flere SOLID‑principper*.
 Din opgave er at:

1. Identificere hvilke principper der brydes
2. Refaktorere koden, så den følger SOLID

### 🔧 Opgavekode

```csharp
public class ReportGenerator
{
    public string GenerateReport()
    {
        return "Report content";
    }

    public void SaveToFile(string content)
    {
        File.WriteAllText("report.txt", content);
    }

    public void SendEmail(string content)
    {
        // send email...
    }
}
```

------



# Løsningsforslag

Her får du et klart og gennemarbejdet løsningsforslag til øvelsen — og jeg forklarer også, hvilke SOLID‑principper der bliver brudt, og hvordan man retter dem.

------

# 🧩 **1. Hvilke SOLID‑principper bliver brudt?**

### 🔵 **SRP — Single Responsibility Principle**

`ReportGenerator` gør tre ting:

- genererer rapport
- gemmer rapport til fil
- sender e-mail

→ Den har *flere grunde til at ændre sig*, hvilket bryder SRP.

------

### 🟣 **OCP — Open/Closed Principle**

Hvis du vil gemme rapporten i en database i stedet for en fil, eller sende den via SMS i stedet for e-mail, skal du ændre klassen.

→ Den er *ikke* åben for udvidelse, men *lukket* for ændring.

------

### 🔴 **DIP — Dependency Inversion Principle**

Klassen afhænger direkte af:

- `File.WriteAllText`
- en konkret e-mail‑implementering

→ Den burde afhænge af abstraktioner (interfaces), ikke konkrete detaljer.

------

# 🛠️ **2. Løsningsforslag (refaktoreret kode)**

Her er en SOLID‑venlig version, opdelt i ansvar og afhængig af interfaces.

------

## ✨ **Trin 1: Del ansvaret op (SRP)**

```csharp
public class ReportGenerator
{
    public string GenerateReport()
    {
        return "Report content";
    }
}
```

------

## ✨ **Trin 2: Lav abstractions for lagring og afsendelse (DIP + OCP)**

```csharp
public interface IReportSaver
{
    void Save(string content);
}

public interface IReportSender
{
    void Send(string content);
}
```

------

## ✨ **Trin 3: Implementér konkrete strategier (åbent for udvidelse)**

### Gem til fil

```csharp
public class FileReportSaver : IReportSaver
{
    public void Save(string content)
    {
        File.WriteAllText("report.txt", content);
    }
}
```

### Send via e-mail

```csharp
public class EmailReportSender : IReportSender
{
    public void Send(string content)
    {
        // send email...
    }
}
```

------

## ✨ **Trin 4: Orkestrér det hele i en højere‑niveau klasse**

```csharp
public class ReportService
{
    private readonly IReportSaver _saver;
    private readonly IReportSender _sender;

    public ReportService(IReportSaver saver, IReportSender sender)
    {
        _saver = saver;
        _sender = sender;
    }

    public void ProcessReport()
    {
        var generator = new ReportGenerator();
        var content = generator.GenerateReport();

        _saver.Save(content);
        _sender.Send(content);
    }
}
```

------

# 🎉 **Resultat**

Nu:

- har hver klasse ét ansvar (SRP)
- kan du tilføje nye måder at gemme eller sende rapporter på uden at ændre eksisterende kode (OCP)
- afhænger systemet af interfaces i stedet for konkrete klasser (DIP)

