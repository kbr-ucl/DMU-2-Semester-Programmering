# Egendomsberegner - Interface og IoC



## Opgaven

Opgaven består i at refaktor "Egendomsberegner - Single Responsibility refactoring"

Minimums krav: 

- Koden skal opdeles i flere klasser med tilhørende interfaces.
- Der skal anvendes constructor injection hvor klasser anvendes af andre klasser.
- Der skal anvendes Microsoft IoC til at danne instanser af konkrete klasser.


## Foreslået proces

Prøv at svar på følgende spørgsmål:

- Kan funktionaliteten opdeles således koden bliver mere funktionel "ren" klassemæssigt (Single responsibility)?


Dernæst er anbefalingen at få skrevet nogle interfaces, klasser samt metode signature, og efterfølgende at fylde funktionalitet i metoderne.

Herefter skrives constuctore og der tilføjes constructor injection

Sluttelig sættes IoC op i program.cs





## Hints

Nedenstående er vist lidt kode der demonstrerer hvorledes der kan arbejdes med IoC i program.cs

```c#
    using Microsoft.Extensions.DependencyInjection;

// This code is a simple console application that uses dependency injection to resolve a service.
// For more information on dependency injection, see https://aka.ms/dotnet-dependency-injection


var services = CreateServices();

Console.WriteLine("Hello, World!");

// Resolve the IVatCalculator service from the service provider
var vatCalculator = services.GetRequiredService<IVatCalculator>();

// Use the IVatCalculator to add VAT to an amount
var amount = 100.00m;
var amountWithVat = vatCalculator.AddVat(amount);
Console.WriteLine($"Amount with VAT: {amountWithVat}");


//Creates a service provider with the necessary services registered.
ServiceProvider CreateServices()
{
    var serviceProvider = new ServiceCollection()
        .AddScoped<ICalculator, Calculator>() // Registering the ICalculator service
        .AddScoped<IVatCalculator, Vat25Calculator>() // Registering the IVatCalculator service
        .BuildServiceProvider();
    return serviceProvider;
}
```

