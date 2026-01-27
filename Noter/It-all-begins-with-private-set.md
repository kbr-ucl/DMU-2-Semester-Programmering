# It all begins with private set



Han du hjælpe mig med at skrive en til to sider til begyndere i C# og OOP
Det skal omhandle indkapsling og vigtigheden af privat set på properties. Jeg tænker titlen skal være:
It all begins with privat set



Selvfølgelig. Her er et udkast til en artikel på cirka 1-2 sider, skrevet i et letforståeligt sprog rettet mod begyndere. Den fokuserer på *hvorfor* vi gør tingene, snarere end bare syntaksen.

Du kan kopiere teksten direkte eller tilpasse den, som du har lyst.

------

# It all begins with private set

### En begynderguide til indkapsling og datakontrol i C#

Når du først stifter bekendtskab med C# og Object-Oriented Programming (OOP), lærer du hurtigt at lave klasser og properties. Det ser ofte sådan her ud:

C#

```
public class BankAccount
{
    public decimal Balance { get; set; }
}
```

Det virker umiddelbart fint. Du kan oprette en konto, og du kan ændre saldoen. Men denne lille linje kode `public decimal Balance { get; set; }` er faktisk en af de mest almindelige kilder til fejl i større systemer.

Hvorfor? Fordi du har givet hele verden lov til at gøre hvad som helst ved din data.

### Problemet med "Public Set"

Forestil dig, at du bygger et system til en bank. Hvis din `Balance` property har en `public set`, kan enhver anden del af din kode gøre dette:

C#

```
var myAccount = new BankAccount();
myAccount.Balance = -1000000; // Ups! En negativ million?
myAccount.Balance = 500;       // Hvor kom de penge fra?
```

Med `public set` er der ingen regler.

1. Der er ingen kontrol af, om tallene giver mening (validering).
2. Vi ved ikke *hvorfor* saldoen ændrede sig (var det et indskud? En rente? En fejl?).

Det er her, **Indkapsling** (Encapsulation) kommer ind i billedet. Indkapsling handler om at beskytte objektets data (state) mod at blive ødelagt udefra.

### Løsningen: Private Set

Den nemmeste og vigtigste måde at starte med indkapsling på, er ved at ændre ét lille ord. Vi gør "set"-delen privat:

C#

```
public class BankAccount
{
    public decimal Balance { get; private set; } // Magien sker her
}
```

**Hvad betyder det?**

- **Public get:** Alle må stadig gerne *se* (læse), hvad saldoen er.
- **Private set:** Kun klassen `BankAccount` selv må *ændre* (skrive) saldoen.

Nu vil koden `myAccount.Balance = 100;` give en fejlmeddelelse. Vi har låst døren udefra. Men hvordan ændrer vi så saldoen, når vi rent faktisk har brug for det?

### Kontrolleret adgang gennem metoder

Når vi bruger `private set`, tvinger vi os selv til at lave **metoder** til at ændre dataen. Det betyder, at vi går fra at sige "Sæt saldoen til X" til at sige "Indsæt beløb X".

Her er, hvordan en bedre klasse ser ud:

C#

```
public class BankAccount
{
    // Property med private set
    public decimal Balance { get; private set; }

    // Constructor: Sætter startværdien
    public BankAccount(decimal initialDeposit)
    {
        if (initialDeposit < 0)
        {
            throw new ArgumentException("Du kan ikke starte med gæld.");
        }
        Balance = initialDeposit;
    }

    // Metode til at indsætte penge
    public void Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Beløbet skal være positivt.");
        }
        Balance += amount;
    }

    // Metode til at hæve penge
    public void Withdraw(decimal amount)
    {
        if (amount > Balance)
        {
            throw new InvalidOperationException("Ikke nok penge på kontoen.");
        }
        Balance -= amount;
    }
}
```

### Hvorfor er dette meget bedre?

Ved at bruge `private set` og metoder opnår vi tre store fordele, som er hjørnestenene i robust softwareudvikling:

**1. Dataintegritet (Valid data)**

Vi kan nu garantere, at `Balance` aldrig bliver ugyldig. I eksemplet ovenfor forhindrer `Withdraw`-metoden, at man kan trække over, og `Deposit`-metoden forhindrer, at man kan indsætte negative beløb. Du kan stole på, at objektet altid er i en "gyldig tilstand".

**2. Læsbarhed og hensigt**

Kode med `public set` fortæller kun *hvad* data er. Kode med metoder fortæller en historie.

- Dårligt: `account.Balance += 100;` (Hvad skete der?)
- Godt: `account.Deposit(100);` (Ah, kunden indsatte penge!)

**3. Fremtidssikring**

Hvis du senere får brug for at tilføje logik – f.eks. at sende en email, hver gang nogen hæver over 5.000 kr. – så skal du kun ændre det ét sted: inde i `Withdraw`-metoden. Havde du brugt `public set`, ville du skulle finde alle steder i hele dit program, hvor saldoen blev ændret, og indsætte din email-kode der.

### Konklusion

Som nybegynder kan det virke nemmest bare at skrive `{ get; set; }` og komme videre. Men det er en vane, der skaber skrøbelig kode.

Ved at bruge `{ get; private set; }` tager du kontrollen tilbage. Du beskytter din data, og du tvinger din kode til at være logisk og struktureret. Det er forskellen på bare at opbevare data og at lave rigtig Objekt-Orienteret Programmering.

Så næste gang du skriver en property, så spørg dig selv: *"Skal andre virkelig have lov til at overskrive denne værdi direkte?"* Svaret er næsten altid: **Nej. Brug private set.**

------

### Vil du have mig til at...

Lave nogle små øvelsesopgaver (med løsninger), der træner brugen af `private set` og konstruktører, som du kan vedlægge teksten?



------------



Her er to små øvelser, der bygger videre på artiklen. De er designet til at træne dig i at beskytte data og bruge metoder til logik.

Du kan prøve at løse dem selv først i Visual Studio eller en notesblok, før du kigger på løsningsforslagene.

------

### Øvelse 1: Spilkarakteren (The Player)

I computerspil er det vigtigt, at en spillers liv (HP) ikke pludselig bliver ændret forkert (f.eks. at man får -50 liv, eller healer mere end man har plads til).

**Opgave:**

Lav en klasse kaldet `Player`.

1. Den skal have en property: `Health` (heltal).
   - Alle må læse den (`get`).
   - Kun klassen må ændre den (`private set`).
2. Lav en **Constructor**, der tager et navn og start-liv (f.eks. 100) og sætter værdierne.
3. Lav en metode: `TakeDamage(int damage)`.
   - Den skal trække skaden fra `Health`.
   - **Regel:** `Health` må aldrig komme under 0. Hvis skaden er for stor, skal `Health` bare sættes til 0.
4. Lav en metode: `Heal(int amount)`.
   - Den skal lægge liv til `Health`.
   - **Regel:** Vi må ikke komme over 100 (eller hvad start-livet var). *Tip: Du kan evt. lave en `MaxHealth` property også, eller bare hardcode grænsen til 100 for simpelhedens skyld.*

------

### Løsning til Øvelse 1

<details>

<summary><strong>Klik her for at se løsningen</strong></summary>

C#

```
public class Player
{
    public string Name { get; private set; }
    public int Health { get; private set; }
    public int MaxHealth { get; private set; }

    public Player(string name, int startHealth)
    {
        Name = name;
        // Vi gemmer startværdien som MaxHealth, så vi kan huske loftet
        MaxHealth = startHealth; 
        Health = startHealth;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        
        // Validering: Vi kan ikke have mindre end 0 liv
        if (Health < 0)
        {
            Health = 0;
        }
        
        // Alternativ måde at skrive det på:
        // Health = Math.Max(0, Health - damage);
    }

    public void Heal(int amount)
    {
        Health += amount;

        // Validering: Vi kan ikke heale mere end max
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }
}
```

</details>

------

### Øvelse 2: Lagerstyring (Inventory)

Forestil dig en simpel webshop. Vi skal holde styr på, hvor mange varer der er på lager.

**Opgave:**

Lav en klasse kaldet `Product`.

1. Den skal have properties for `Name` (string) og `StockQuantity` (int).
   - `StockQuantity` skal have **private set**.
2. Lav en **Constructor**, der sætter navnet og starter lageret på 0.
3. Lav en metode: `Restock(int amount)`.
   - Den skal lægge antallet til lageret.
   - **Regel:** Man kan ikke "restocke" med negative tal (man kan ikke lægge -5 varer på hylden).
4. Lav en metode: `Sell(int amount)`.
   - Den skal trække antallet fra lageret.
   - **Regel:** Hvis man prøver at sælge flere varer, end der er på lager, skal den enten gøre ingenting, udskrive en fejl til konsollen, eller smide en "Exception" (vælg selv). Lageret må aldrig blive negativt.

------

### Løsning til Øvelse 2

<details>

<summary><strong>Klik her for at se løsningen</strong></summary>

C#

```
public class Product
{
    public string Name { get; private set; }
    public int StockQuantity { get; private set; }

    public Product(string name)
    {
        Name = name;
        StockQuantity = 0; // Vi starter altid med et tomt lager her
    }

    public void Restock(int amount)
    {
        if (amount <= 0)
        {
            // Vi beskytter mod ugyldige tal
            Console.WriteLine("Fejl: Du skal tilføje mindst 1 vare."); 
            return; // Vi stopper metoden her
        }
        
        StockQuantity += amount;
    }

    public void Sell(int amount)
    {
        if (amount > StockQuantity)
        {
            Console.WriteLine($"Fejl: Der er kun {StockQuantity} på lager. Du prøver at sælge {amount}.");
            return;
        }

        StockQuantity -= amount;
    }
}
```

</details>

------

### Hvorfor virker det?

I begge øvelser ser du mønsteret:

1. Data (`Health`, `StockQuantity`) er låst.
2. Metoderne (`TakeDamage`, `Sell`) fungerer som "dørmænd". De tjekker reglerne, *før* de tillader ændringen.

### Vil du have mig til at...

Forklare, hvordan man bruger disse klasser i et `Main`-program (Console Application), så du kan se koden køre i praksis?