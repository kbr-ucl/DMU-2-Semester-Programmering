# Forklaring på *interfaces* 

Hvad er et interface?

Et **interface** er en **kontrakt**: det beskriver **hvilke metoder (og evt. egenskaber)** en klasse **skal** have – **men ikke hvordan** de er implementeret.

- Tænk på det som et **stik**: Et strømstik specificerer form og spænding (signaturen), men ikke hvordan elværket producerer strømmen (implementeringen).
- Når en klasse “**implementerer**” et interface, lover den at stille de beskrevne medlemmer til rådighed.

**Hvorfor bruge interfaces?**

- **Polymorfi**: du kan programmere mod en fælles type (interfacet) i stedet for konkrete klasser.
- **Løs kobling & testbarhed**: du kan udskifte en implementering med en anden (fx en mock i tests).
- **Arkitektur**: skaber klare grænseflader mellem moduler/lag.

------

## Basal syntaks



```c#
public interface IBetaling
{
  void Betal(decimal beløb);
}
```



En klasse der implementerer interfacet:



```c#
public class KortBetaling : IBetaling
{
  public void Betal(decimal beløb)
  {
    Console.WriteLine($"Betaler {beløb:C} med kort.");
  }
}

public class MobilePayBetaling : IBetaling
{
  public void Betal(decimal beløb)
  {
    Console.WriteLine($"Betaler {beløb:C} med MobilePay.");
  }
}
```



Brug via polymorfi:



```c#
public class Kasseapparat
{
  private readonly IBetaling *betaling**;*
  public Kasseapparat(IBetaling betaling) // afhængighed injiceres
  {
    betaling = betaling;
  }
  public void Afregn(decimal beløb)
  {
    _betaling.Betal(beløb);
  }
}

// Eksempel:
var kasse = new Kasseapparat(new KortBetaling());
kasse.Afregn(149.95m); // "Betaler 149,95 kr. med kort."
```



Bemærk: `Kasseapparat` kender **kun** til `IBetaling`—ikke til `KortBetaling` eller `MobilePayBetaling`. Det gør koden udskiftelig og testbar.

------

## Egenskaber og metoder i interfaces

Interfaces kan også indeholde **egenskaber** og **metoder** uden krop:



```c#
public interface ILog
{
  string Navn { get; }      // read-only property
  void Skriv(string besked);
}
```



Implementering:



```c#
public class KonsolLog : ILog
{
  public string Navn => "Konsol";
    
  public void Skriv(string besked)
  {
    Console.WriteLine($"[{Navn}] {besked}");
  }
}
```



------

## Flere interfaces på én klasse

En klasse kan implementere **flere** interfaces:



```c#
public interface IStartbar { void Start(); }
public interface IStopbar { void Stop(); }

public class Motor : IStartbar, IStopbar
{
  public void Start() => Console.WriteLine("Motor startet.");
  public void Stop() => Console.WriteLine("Motor stoppet.");
}
```





------

## Polymorfi i praksis

Du kan have en samling af forskellige implementeringer og behandle dem ens:



```c#
var betalingsmetoder = new List<IBetaling>
{
  new KortBetaling(),
  new MobilePayBetaling()
};

foreach (var b in betalingsmetoder)
{
  b.Betal(50m); // Kører den konkrete implementering for hver type
}
```



------

## Interface vs. abstrakt klasse (hurtigt overblik)

|                 | **Interface**                         | **Abstrakt klasse**                                 |
| --------------- | ------------------------------------- | --------------------------------------------------- |
| Formål          | Kontrakt (hvad skal findes)           | Delvis implementering + fælles base                 |
| Medlemmer       | Signaturer (evt. default metoder)     | Felter, egenskaber, metoder (med krop), konstruktør |
| Arv             | Kan implementeres flere               | Kun enkelt arv fra én baseklasse                    |
| Når bruger man… | Løs kobling, udskiftelighed, “roller” | Deladfærd, fælles tilstand, skabelonsmetoder        |

**Tommelregel**: Start med interfaces for løse kontrakter. Brug abstrakte klasser når du vil **dele implementering/tilstand**.

