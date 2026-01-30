# IoC: En kort intro

Inversion of Control (IoC) og Dependency Injection (DI) kan lyde komplekst, men det handler i bund og grund om at gøre din kode **fleksibel** og **nem at teste**.

Her er en kort guide designet til en Console App.

For at køre eksemplerne skal du installere pakken:

```c#
Microsoft.Extensions.DependencyInjection
```

------

## Hvad er IoC og Constructor Injection?

Forestil dig, at du bygger et hus.

- **Uden IoC:** Du bygger selv din hammer, din sav og din boremaskine inde i huset, før du går i gang. Hvis du vil skifte boremaskine, skal du rive huset ned og bygge det forfra.
- **Med IoC (Constructor Injection):** Du beder om en hammer og en sav, når du møder på arbejde. Nogen (IoC-containeren) rækker dig værktøjet. Du er ligeglad med mærket, så længe det virker.

-----

## Hvad er Dependency Injection (DI)



### 1. Hvad er Dependency Injection (DI)?

Dependency Injection er et designmønster, der handler om at fjerne ordet `new` inde fra midten af dine klasser.

Helt simpelt: **Objekter skal ikke skabe deres egne værktøjer. De skal have dem leveret.**

#### Før DI (Hardcoded):

Tænk på en billig radio, hvor batteriet er loddet fast indeni.

- Hvis batteriet dør, skal du smide hele radioen ud.
- Du kan ikke skifte til et bedre batteri.
- *Koden:* Radioen siger `new Batteri()` inde i sig selv. Den er "afhængig" af præcis dén batteritype.

#### Med DI (Injected):

Tænk på en moderne radio med en batterilåge.

- Radioen er ligeglad med mærket på batteriet.
- Så længe batteriet passer i hullet (følger `Interfacet`), virker radioen.
- Du sætter batteriet i udefra (Injecter det).
- *Koden:* Radioen tager imod `IBatteri` i sin constructor.

**DI gør din kode til "Lego-klodser":** De klikker sammen, men de er ikke limet sammen.

------

### 2. ServiceCollection er din IoC Container

I C# verdenen (især .NET) bruger vi et indbygget framework, der hedder **Microsoft.Extensions.DependencyInjection**.

Dette framework fungerer som en **IoC Container** (Inversion of Control Container).

Tænk på IoC Containeren som en **robot-fabrik**:

1. Du fortæller robotten, hvordan den skal bygge tingene (**ServiceCollection**).
2. Du tænder for robotten (**BuildServiceProvider**).
3. Du beder robotten om et færdigt produkt, og den samler selv alle delene (**GetRequiredService**).

#### Hvorfor kalder vi det "Inversion of Control" (Omvendt kontrol)?

- **Normal kontrol:** Din kode styrer alt. *"Jeg skal bruge en Writer, så jeg laver en `new ConsoleWriter()` nu."*
- **Inversion of Control:** Du afgiver kontrollen til Containeren (Frameworket). *"Jeg skal bruge en Writer. Container, vil du være sød at finde en til mig? Jeg er ligeglad med, hvordan du laver den."*

### Det samlede billede

Når du skriver `var serviceCollection = new ServiceCollection();`, starter du i virkeligheden motoren på dette framework.

Her er sammenhængen skåret helt ind til benet:

| **Begreb**               | **Hvad er det?**                                             | **Analogi**                                            |
| ------------------------ | ------------------------------------------------------------ | ------------------------------------------------------ |
| **Dependency**           | Det værktøj din klasse skal bruge (f.eks. `IWriter`).        | Et batteri.                                            |
| **Dependency Injection** | Handlingen at give værktøjet til klassen (via constructor).  | At sætte batteriet i radioen.                          |
| **IoC Container**        | Hele systemet, der styrer dette automatisk.                  | En robot, der sætter batterier i alle radioer for dig. |
| **ServiceCollection**    | **Konfigurations-delen** af containeren. Her *registrerer* du dine dele. | Indkøbslisten / Byggetegningen.                        |
| **ServiceProvider**      | **Runtime-delen** af containeren. Her *henter* du de færdige objekter. | Selve lageret, hvor du henter varerne.                 |

**Kort sagt:**

Uden en IoC Container (som `ServiceCollection` hjælper med at bygge), skulle du selv skrive `new Greeter(new ConsoleWriter())` alle steder i din kode. Med containeren slipper du for det manuelle "samle-arbejde".


----------

## Analogi: Restaurantens Menukort


### 1. Analogi: Restaurantens Menukort

Forestil dig, du skal åbne en restaurant. Før du kan servere mad, skal du lave en liste over, hvad køkkenet kan lave.

- **ServiceCollection:** Dette er det blanke stykke papir, hvor du skriver menuen ned.
- **AddTransient...:** Det er her, du skriver en linje på papiret: *"Hvis nogen beder om en 'IWriter', så giv dem en 'ConsoleWriter'."*
- **BuildServiceProvider():** Nu afleverer du menukortet til køkkenchefen. Nu er restauranten åben.

**Vigtigt:** Når du putter ting i `ServiceCollection`, bliver der **ikke** oprettet nogen objekter endnu. Du laver kun listen.

------

### 2. Teknisk set: Det er bare en liste

Mange bliver overraskede over dette, men `ServiceCollection` er faktisk bare en helt almindelig liste.

I C# implementerer den `IList<ServiceDescriptor>`. Det betyder, at den indeholder en række små beskrivelser (descriptors) af dine services.

Du kan faktisk se det i koden:


```
var serviceCollection = new ServiceCollection();

// Listen er tom nu
Console.WriteLine($"Antal opskrifter: {serviceCollection.Count}"); // Skriver 0

// Vi tilføjer en opskrift
serviceCollection.AddTransient<IWriter, ConsoleWriter>();

// Nu ligger der én ting i listen
Console.WriteLine($"Antal opskrifter: {serviceCollection.Count}"); // Skriver 1
```

Hver gang du skriver `serviceCollection.Add...`, lægger du bare et nyt element i listen, der indeholder tre ting:

1. **Hvad** beder folk om? (Interface: `IWriter`)
2. **Hvem** skal gøre det? (Implementation: `ConsoleWriter`)
3. **Hvordan** skal levetiden være? (Lifetime: `Transient`)

------

### 3. Forskellen på Collection og Provider

Det er her, mange begyndere snubler. Det er vigtigt at kende forskel på de to faser:

**Fase 1: Konfiguration (`ServiceCollection`)**

- Her *forbereder* du alt.
- Du må ændre i listen (tilføje/fjerne).
- Du kan ikke hente services endnu.

**Fase 2: Runtime (`ServiceProvider`)**

- Dette sker, når du kalder `.BuildServiceProvider()`.
- Nu er listen "låst".
- Det er en færdig maskine, der spytter objekter ud baseret på opskrifterne fra Fase 1.

### Opsamling

Så når du ser `serviceCollection` i din kode, så tænk på det som **forberedelses-fasen**.

1. Du opretter listen.
2. Du fylder listen med regler (Dependency Injection opsætning).
3. Du "låser" listen og bygger din applikation (Provideren).



------

## Her er tre eksempler, der bygger ovenpå hinanden.

### 1. Det helt simple eksempel

Her har vi en `Greeter` klasse, der er afhængig af en `IWriter`. Vi "injicerer" `IWriter` gennem constructoren.

**Interfacet og implementationen:**

C#

```c#
public interface IWriter
{
    void Write(string message);
}

public class ConsoleWriter : IWriter
{
    public void Write(string message)
    {
        Console.WriteLine($"[Console]: {message}");
    }
}
```

**Klassen der bruger afhængigheden (Consumer):**

```c#
public class Greeter
{
    private readonly IWriter _writer;

    // Constructor Injection: Vi beder om en IWriter
    public Greeter(IWriter writer)
    {
        _writer = writer;
    }

    public void SayHello()
    {
        _writer.Write("Hej verden!");
    }
}
```

**Opsætning i Program.cs:**

```c#
using Microsoft.Extensions.DependencyInjection;

var serviceCollection = new ServiceCollection();

// 1. Registrer dine services
serviceCollection.AddTransient<IWriter, ConsoleWriter>();
serviceCollection.AddTransient<Greeter>();

// 2. Byg containeren
var provider = serviceCollection.BuildServiceProvider();

// 3. Hent hovedprogrammet og kør det
var app = provider.GetRequiredService<Greeter>();
app.SayHello(); 
```

------

### 2. Eksempel med parametre (Manuel konfiguration)

Hvad hvis din service kræver en specifik indstilling (f.eks. et navn eller en connection string), som ikke er en anden service?

Her bruger vi en **Lambda factory** til at fortælle containeren, hvordan objektet skal oprettes.

**Klassen med parameter:**

C#

```c#
public class CustomWriter : IWriter
{
    private readonly string _prefix;

    public CustomWriter(string prefix)
    {
        _prefix = prefix;
    }

    public void Write(string message)
    {
        Console.WriteLine($"{_prefix}: {message}");
    }
}
```

**Opsætning i Program.cs:**

C#

```c#
// Her fortæller vi containeren præcis hvordan CustomWriter skal laves
serviceCollection.AddTransient<IWriter>(provider => 
{
    return new CustomWriter(">> INFO <<"); 
});

serviceCollection.AddTransient<Greeter>();

// Når vi nu henter Greeter, får den en CustomWriter med prefixet ">> INFO <<"
var provider = serviceCollection.BuildServiceProvider();
var app = provider.GetRequiredService<Greeter>();
app.SayHello();
```

------

### 3. Flere konkrete af samme type (Keyed Services)

Dette er ofte det sværeste for begyndere. Du har ét interface (`IWriter`), men to forskellige implementationer (f.eks. én til fil og én til konsol), og du skal bruge dem begge to forskellige steder.

Fra .NET 8 bruger vi **Keyed Services**. Vi giver hver implementation et "nøgle-navn".

**To forskellige implementationer:**

C#

```c#
public class RedWriter : IWriter
{
    public void Write(string message) => Console.WriteLine($"RED: {message}");
}

public class BlueWriter : IWriter
{
    public void Write(string message) => Console.WriteLine($"BLUE: {message}");
}
```

**En klasse der skal bruge en SPECIFIK version:**

Her bruger vi `[FromKeyedServices("navn")]` attributten i constructoren.

C#

```c#
public class DualGreeter
{
    private readonly IWriter _redWriter;
    private readonly IWriter _blueWriter;

    public DualGreeter(
        [FromKeyedServices("red")] IWriter red, 
        [FromKeyedServices("blue")] IWriter blue)
    {
        _redWriter = red;
        _blueWriter = blue;
    }

    public void GreetBoth()
    {
        _redWriter.Write("Dette er til den røde log.");
        _blueWriter.Write("Dette er til den blå log.");
    }
}
```

**Opsætning i Program.cs:**



```c#
// Registrer med nøgler (Keys)
serviceCollection.AddKeyedTransient<IWriter, RedWriter>("red");
serviceCollection.AddKeyedTransient<IWriter, BlueWriter>("blue");

serviceCollection.AddTransient<DualGreeter>();

var provider = serviceCollection.BuildServiceProvider();
var app = provider.GetRequiredService<DualGreeter>();

app.GreetBoth();
```

------

### Opsamling

1. **Containeren (ServiceCollection):** Det sted hvor du registrerer "opskrifterne" på dine klasser.
2. **AddTransient:** Betyder "Lav en ny hver gang nogen spørger".
3. **Constructor Injection:** Måden dine klasser modtager deres værktøjer på, uden selv at bruge `new Class()`.

