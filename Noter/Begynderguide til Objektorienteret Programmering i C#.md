# **Begynderguide til Objektorienteret Programmering i C#**

Objektorienteret programmering (OOP) er en måde at strukturere kode på, så den bliver mere overskuelig, fleksibel og genanvendelig. I C# er OOP helt centralt – næsten alt i sproget er bygget op omkring objekter og klasser.

Denne tutorial gennemgår de fire grundpiller i OOP:

1. **Indkapsling (Encapsulation)**
2. **Abstraktion (Abstraction)**
3. **Arv (Inheritance)**
4. **Polymorfi (Polymorphism)**

Vi starter med den vigtigste for begyndere: **indkapsling**.

------

# 🔒 **1. Indkapsling – Beskyt dine data**

Indkapsling handler om at **skjule data** og kun give adgang til det, der er nødvendigt. Det gør koden mere robust og forhindrer utilsigtede ændringer.

I C# bruger vi **properties** til at styre adgang til felter.

## ✔️ **Hvorfor bruge properties i stedet for public fields?**

- Du kan kontrollere, hvordan værdier læses og skrives
- Du kan validere input
- Du kan ændre implementeringen senere uden at ændre API’et
- Du kan gøre dele af en property skrivebeskyttet

------

## ⭐ **Vigtigt: Properties med `private set`**

En af de mest nyttige teknikker i indkapsling er at gøre en property **kun læsbar udefra**, men stadig **skrivbar indefra klassen**.

### Eksempel:

```csharp
public class Person
{
    public string Name { get; private set; }
    public int Age { get; private set; }

    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }

    public void HaveBirthday()
    {
        Age++; // Tilladt, fordi set er private
    }
}
```

### Hvad betyder `private set`?

- **Andre klasser kan læse værdien**
- **Kun klassen selv kan ændre værdien**

Det er perfekt til data, der skal være stabile udefra, men stadig kunne ændres af objektet selv.

------

# 🧱 **2. Abstraktion – Fokusér på det vigtige**

Abstraktion handler om at vise det nødvendige og skjule det unødvendige.

Eksempel:

```csharp
public class Car
{
    public int Speed { get; private set; }

    public void Accelerate()
    {
        Speed += 10;
    }
}
```

Brugeren af klassen behøver ikke vide, *hvordan* bilen accelererer – kun at den kan.

------

# 🧬 **3. Arv – Genbrug kode**

Arv gør det muligt at lade én klasse bygge videre på en anden.

```csharp
public class Animal
{
    public string Name { get; private set; }

    public Animal(string name)
    {
        Name = name;
    }

    public void Eat()
    {
        Console.WriteLine($"{Name} eats.");
    }
}

public class Dog : Animal
{
    public Dog(string name) : base(name) { }

    public void Bark()
    {
        Console.WriteLine($"{Name} barks.");
    }
}
```

------

# 🎭 **4. Polymorfi – Samme metode, forskellig opførsel**

Polymorfi gør det muligt at kalde den samme metode på forskellige objekter, men få forskellig adfærd.

```csharp
public class Animal
{
    public virtual void MakeSound()
    {
        Console.WriteLine("Some sound...");
    }
}

public class Dog : Animal
{
    public override void MakeSound()
    {
        Console.WriteLine("Woof!");
    }
}
```

------

# 🧩 **Samlet eksempel – OOP i praksis**

Her er et lille system, der bruger alle fire principper:

```csharp
public abstract class Account
{
    public string Owner { get; private set; }
    public decimal Balance { get; private set; }

    public Account(string owner)
    {
        Owner = owner;
    }

    public void Deposit(decimal amount)
    {
        Balance += amount;
    }

    public abstract void Withdraw(decimal amount);
}

public class SavingsAccount : Account
{
    public SavingsAccount(string owner) : base(owner) { }

    public override void Withdraw(decimal amount)
    {
        if (amount <= Balance)
            Deposit(-amount);
    }
}
```

Bemærk igen brugen af **`private set`** for at beskytte `Balance`.

