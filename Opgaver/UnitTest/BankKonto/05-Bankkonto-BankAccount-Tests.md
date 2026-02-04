# Bankkonto - BankAccount Tests

Her er en komplet testklasse skrevet i **xUnit**.

Den er struktureret den efter **Arrange-Act-Assert** mønsteret, som er standarden for gode unit tests. Der er også inkluderet både `[Fact]` (til enkelte tests) og `[Theory]` (til at teste flere scenarier med samme logik).

### Testklasse: BankAccountTests.cs

```c#
using Xunit;
using System;
using Domain; // Husk at referere til din BankAccount namespace

namespace Domain.Tests
{
    public class BankAccountTests
    {
        // ---------------------------------------------------------
        // 1. Constructor Tests (Oprettelse af konto)
        // ---------------------------------------------------------

        [Fact]
        public void Constructor_WithValidArguments_ShouldSetProperties()
        {
            // Arrange
            string accountNumber = "123456";
            string owner = "Jens Hansen";
            decimal initialBalance = 1000m;

            // Act
            var account = new BankAccount(accountNumber, owner, initialBalance);

            // Assert
            Assert.Equal(accountNumber, account.AccountNumber);
            Assert.Equal(owner, account.Owner);
            Assert.Equal(initialBalance, account.Balance);
            Assert.False(account.IsFrozen);
        }

        [Fact]
        public void Constructor_WithNegativeBalance_ShouldThrowException()
        {
            // Arrange, Act & Assert
            // Vi forventer en ArgumentOutOfRangeException, hvis vi starter med minus
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                new BankAccount("123", "Jens", -100));
        }

        // ---------------------------------------------------------
        // 2. Deposit Tests (Indsæt penge)
        // ---------------------------------------------------------

        [Fact]
        public void Deposit_WithPositiveAmount_ShouldIncreaseBalance()
        {
            // Arrange
            var account = new BankAccount("123", "Jens", 100);

            // Act
            account.Deposit(50);

            // Assert
            Assert.Equal(150, account.Balance);
        }

        [Theory] // Theory lader os køre samme test med forskellige input
        [InlineData(0)]
        [InlineData(-50)]
        public void Deposit_WithZeroOrNegativeAmount_ShouldThrowException(decimal amount)
        {
            // Arrange
            var account = new BankAccount("123", "Jens", 100);

            // Act & Assert
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => account.Deposit(amount));
            Assert.Contains("Indskud skal være positivt", ex.Message);
        }

        [Fact]
        public void Deposit_WhenAccountIsFrozen_ShouldThrowException()
        {
            // Arrange
            var account = new BankAccount("123", "Jens", 100);
            account.FreezeAccount(); // Vi fryser kontoen

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => account.Deposit(50));
        }

        // ---------------------------------------------------------
        // 3. Withdraw Tests (Hæv penge)
        // ---------------------------------------------------------

        [Fact]
        public void Withdraw_WithValidAmount_ShouldDecreaseBalance()
        {
            // Arrange
            var account = new BankAccount("123", "Jens", 200);

            // Act
            account.Withdraw(50);

            // Assert
            Assert.Equal(150, account.Balance);
        }

        [Fact]
        public void Withdraw_MoreThanBalance_ShouldThrowException()
        {
            // Arrange
            var account = new BankAccount("123", "Jens", 100);

            // Act & Assert (Forsøger at hæve 101 kr, når der kun er 100 kr)
            var ex = Assert.Throws<InvalidOperationException>(() => account.Withdraw(101));
            Assert.Equal("Ikke nok penge på kontoen.", ex.Message);
        }

        [Fact]
        public void Withdraw_WhenAccountIsFrozen_ShouldThrowException()
        {
            // Arrange
            var account = new BankAccount("123", "Jens", 100);
            account.FreezeAccount();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => account.Withdraw(10));
        }
    }
}
```

------

### Vigtige pointer /  xUnit-features:

1. **`[Fact]` vs. `[Theory]`:**
   - Brug `[Fact]` til tests, der altid er sande under de givne omstændigheder (invariant).
   - Brug `[Theory]` sammen med `[InlineData]` (se `Deposit_WithZeroOrNegativeAmount...`) for at undgå at skrive den samme test 5 gange bare for at teste forskellige tal. Det sparer meget kode.
2. **`Assert.Throws<T>`:**
   - Dette er måden, vi tester, at vores forretningsregler ("Du må ikke hæve overtræk") faktisk stopper systemet som forventet.
3. **Arrange-Act-Assert:**
   - Bemærk, hvor letlæselig koden er, når man tydeligt opdeler opsætning, handling og verificering.

