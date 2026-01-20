# En større mini‑applikation hvor du selv skal refaktorere

Her får du en **større mini‑applikation**, der *bevidst* bryder flere SOLID‑principper.
 Din opgave bliver at:

1. Identificere hvilke principper der brydes
2. Refaktorere applikationen, så den følger SOLID
3. (Hvis du vil) få mit løsningsforslag bagefter

Opgaven er lavet så realistisk som muligt, som noget man faktisk kunne møde i en virksomhed.

------

# 🏗️ **Mini‑applikation: “Order Processing System” (dårlig version)**

Forestil dig et system, der håndterer ordrer i en webshop.

Her er den *bevidst dårlige* implementation:

```csharp
public class Order
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string CustomerEmail { get; set; }
}

public class OrderProcessor
{
    public void Process(Order order)
    {
        // 1. Validate order
        if (order.Amount <= 0)
            throw new Exception("Invalid amount");

        // 2. Save to database
        using (var connection = new SqlConnection("connectionstring"))
        {
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO Orders VALUES ({order.Id}, {order.Amount}, '{order.CustomerEmail}')";
            cmd.ExecuteNonQuery();
        }

        // 3. Charge payment
        if (order.Amount > 1000)
        {
            // charge with PayPal
        }
        else
        {
            // charge with CreditCard
        }

        // 4. Send confirmation email
        var smtp = new SmtpClient("smtp.server.com");
        smtp.Send("shop@company.com", order.CustomerEmail, "Order Confirmation", "Thanks for your order!");
    }
}
```

------

# 🚨 **Hvad er galt her?**

Denne klasse bryder næsten *alle* SOLID‑principper:

- **SRP**: Den validerer, gemmer i database, håndterer betaling og sender e-mail
- **OCP**: Hvis du vil tilføje MobilePay, skal du ændre koden
- **LSP**: Betalingslogikken vil bryde sammen, hvis du laver nye betalingsmetoder
- **ISP**: Der er ingen interfaces — alt er hårdt koblet
- **DIP**: Klassen afhænger af konkrete klasser som `SqlConnection`, `SmtpClient`, PayPal‑logik osv.

------

# 🎯 **Din opgave**

Refaktorér systemet, så det følger SOLID.

Du må gerne:

- introducere interfaces
- lave nye klasser
- bruge dependency injection
- opdele ansvar
- lave strategier for betaling
- lave repositories
- lave services
- lave validators

**Du bestemmer selv arkitekturen — bare den følger SOLID.**

------

# 📦 **Bonus: Ekstra krav (valgfrit)**

Hvis du vil udfordre dig selv:

- Tilføj logging
- Tilføj flere betalingsmetoder
- Tilføj mulighed for at sende SMS i stedet for e-mail
- Tilføj unit tests (du behøver ikke skrive dem, bare design til det)

------



