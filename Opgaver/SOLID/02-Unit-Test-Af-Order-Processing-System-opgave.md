# UnitTest af Order Processing System

Denne opgave er en forlængelse af [01-Refactoring-opgave](./01-Refactoring-opgave.md)

I *Order Processing System* er der nogle forretningsregler:

- *Ordresum* skal være større end 0
- *CustomerEmail* skal indeholde tegn - dvs. ikke null eller kun "WhiteSpaces"
- Ved betalingsbeløb større end 1000 skal der betales vha. PayPal, ellers via kreditkort.

Til opgaven skal der bruges XUnit. Du bør starte med at opdaterer den XUnit template til nyeste version, ved i powershell at udføre kommandoen:

```powershell
dotnet new install xunit.v3.templates
```

Når du opretter et testprojekt i Visual Studio skal du vælge: ***xUnit.net v3 Test Project (xUnit.net Team)***

Teori: [xUnit.net v3: Fra Nybegynder til Avanceret](../../Noter/xUnit-Fra-Nybegynder-til-Avanceret.md)



## Opgave

Der skal skrives unittest med XUnit til at sikre at alle forretningsregler er overholdt. Der bør bruges `Theory` således grænseværdier testes. 



**Bonus opgave:**

Lav testkode der tjekker om om *CustomerEmail* har et lovligt format - hint kik på RegX udtryk.

