# **Øvelser og opgaver til OOP i C#**

## ⭐ **Øvelse 1: Lav din første klasse**

**Formål:** Forstå klasser, objekter og properties.

**Opgave:**

1. Opret en klasse `Book`.
2. Klassen skal have følgende properties:
   - `Title` (string)
   - `Author` (string)
   - `Pages` (int)
3. Lav en constructor, der sætter alle tre værdier.
4. Opret et objekt af klassen i `Main()` og udskriv informationerne.

------

## ⭐ **Øvelse 2: Indkapsling med `private set`**

**Formål:** Lære at beskytte data.

**Opgave:**

1. Udvid `Book`-klassen.
2. Gør `Pages` til:

```csharp
public int Pages { get; private set; }
```

1. Tilføj en metode `AddPages(int amount)` som øger antallet af sider.
2. Prøv at ændre `Pages` direkte fra `Main()` – og se at det ikke er muligt.

------

## ⭐ **Øvelse 3: Metoder og adfærd**

**Formål:** Forstå hvordan objekter kan udføre handlinger.

**Opgave:** Lav en klasse `Car` med:

- `Brand` (string)
- `Speed` (int) med **private set**
- Metode `Accelerate()` der øger farten med 10
- Metode `Brake()` der sænker farten med 10 (men ikke under 0)

Test klassen ved at accelerere og bremse flere gange.

------

## ⭐ **Øvelse 4: Arv**

**Formål:** Lære at genbruge kode via inheritance.

**Opgave:**

1. Lav en baseklasse `Animal` med:
   - `Name` (string, private set)
   - Metode `Eat()`
2. Lav to klasser der arver fra `Animal`:
   - `Dog`
   - `Cat`
3. Giv `Dog` en metode `Bark()` og `Cat` en metode `Meow()`.
4. Opret objekter af begge typer og kald deres metoder.

------

## ⭐ **Øvelse 5: Polymorfi**

**Formål:** Forstå hvordan forskellige objekter kan reagere forskelligt på samme metodekald.

**Opgave:**

1. Udvid `Animal` med en virtuel metode:

```csharp
public virtual void MakeSound()
{
    Console.WriteLine("Some generic animal sound");
}
```

1. Override metoden i `Dog` og `Cat`.
2. Lav en liste af `Animal`-objekter og kald `MakeSound()` på dem i et loop.

------

## ⭐ **Øvelse 6: Mini-projekt – Bankkonto**

**Formål:** Kombinere indkapsling, arv og metoder.

**Opgave:** Lav et lille banksystem:

### Baseklasse: `Account`

- `Owner` (string, private set)
- `Balance` (decimal, **private set**)
- Metode `Deposit(decimal amount)`
- Virtuel metode `Withdraw(decimal amount)`

### Underklasser:

- `SavingsAccount`
- `CheckingAccount`

### Krav:

- `SavingsAccount` må ikke gå i minus.
- `CheckingAccount` må gerne gå i minus op til –1000.
- Test systemet ved at lave flere konti og lave ind- og udbetalinger.

------

## ⭐ **Øvelse 7: Design din egen klasse**

**Formål:** Kreativ opgave der viser forståelse.

**Opgave:** Design en klasse efter eget valg – fx:

- `Player`
- `Movie`
- `BankCustomer`
- `Robot`
- `Product`

Krav:

- Min. 3 properties (mindst én med `private set`)
- Min. 2 metoder
- En constructor
- En kort demonstration i `Main()