# Domæneklassen: BankAccount

Denne klasse indeholder properties, der kun kan ændres via metoder (indkapsling), og metoderne kaster specifikke *exceptions*, hvis reglerne brydes.

```c#
using System;

namespace Domain
{
    public class BankAccount
    {
        // Properties
        public string AccountNumber { get; }
        public string Owner { get; }
        public decimal Balance { get; private set; } // Kan kun ændres internt
        public bool IsFrozen { get; private set; }

        // Constructor
        public BankAccount(string accountNumber, string owner, decimal initialBalance = 0)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new ArgumentException("Kontonummer skal være udfyldt.", nameof(accountNumber));

            if (initialBalance < 0)
                throw new ArgumentOutOfRangeException(nameof(initialBalance), "Startsaldo kan ikke være negativ.");

            AccountNumber = accountNumber;
            Owner = owner;
            Balance = initialBalance;
            IsFrozen = false;
        }

        // Forretningsregel: Man må ikke indsætte 0 eller negative beløb.
        public void Deposit(decimal amount)
        {
            if (IsFrozen)
                throw new InvalidOperationException("Kontoen er frosset. Ingen transaktioner tilladt.");

            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Indskud skal være positivt.");

            Balance += amount;
        }

        // Forretningsregler: 
        // 1. Man må ikke hæve negative beløb.
        // 2. Der skal være dækning på kontoen (ingen overtræk).
        // 3. Man kan ikke hæve fra en frosset konto.
        public void Withdraw(decimal amount)
        {
            if (IsFrozen)
                throw new InvalidOperationException("Kontoen er frosset. Ingen hævninger tilladt.");

            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Hævning skal være positivt.");

            if (amount > Balance)
                throw new InvalidOperationException("Ikke nok penge på kontoen.");

            Balance -= amount;
        }

        public void FreezeAccount()
        {
            IsFrozen = true;
        }
    }
}
```

------

### Hvad du kan teste her

Denne klasse er perfekt til at demonstrere de tre hovedkategorier af tests:

1. **Happy Path (Det går godt):**
   - At `Deposit(100)` øger saldoen korrekt.
   - At `Withdraw(50)` mindsker saldoen korrekt.
2. **Edge Cases / Constraints (Grænseværdier):**
   - At man ikke kan oprette en konto med negativ saldo.
   - At man ikke kan indsætte `-50` kr.
3. **State-based logic (Tilstand):**
   - At man ikke kan hæve penge, hvis `IsFrozen` er `true`.
   - At man ikke kan hæve flere penge, end der står på kontoen (Overtræk).

