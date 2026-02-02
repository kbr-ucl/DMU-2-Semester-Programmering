# xUnit.net v3: Fra Nybegynder til Avanceret

Unit testing er rygraden i professionel softwareudvikling. Det giver dig tryghed til at ændre i din kode uden frygt for at ødelægge eksisterende funktionalitet.

I denne guide tager vi dig fra din allerførste test til avancerede teknikker med **xUnit.net v3**. Vi fokuserer på struktur, læsbarhed og robuste tests.

## 1. Fundamentet: Navngivning og Struktur

Før vi skriver kode, skal vi have styr på sproget. En unit test er dokumentation. Hvis en test fejler om 6 måneder, skal du (eller din kollega) kunne læse navnet og vide præcis, hvad der gik galt.

Vi bruger **Given-When-Then** konventionen oversat til metodenavne:

```
MetodeNavn_Tilstand_ForventetResultat
```

- **MetodeNavn:** Hvad tester vi?
- **Tilstand:** Hvad er inputtet eller scenariet?
- **ForventetResultat:** Hvad er outputtet eller effekten?

**Eksempel:** `BeregnFragt_OrdreUnderGrænse_TillæggerGebyr`

### Koden vi skal teste (System Under Test)

Vi bruger en simpel klasse, `FragtBeregner`, som gennemgående eksempel.

Reglerne er:

1. Ordrer under 500 kr. koster 50 kr. i fragt.
2. Ordrer på 500 kr. og derover er fragtfri.

C#

```c#
public class FragtBeregner
{
    public decimal BeregnFragt(decimal ordreBeløb)
    {
        if (ordreBeløb < 500)
            return 50;
            
        return 0;
    }
}
```

------

## 2. Din første test: `[Fact]`

Den simpleste test i xUnit er en `[Fact]`. Den bruges, når testen altid er sand (invariant), og ikke kræver forskellige input-parametre.

Vi bygger testen op efter **AAA-modellen** (Arrange, Act, Assert).

C#

```c#
using Xunit;

public class FragtBeregnerTests
{
    [Fact]
    public void BeregnFragt_BeløbErLavt_ReturnererStandardFragt()
    {
        // 1. Arrange (Givet)
        var beregner = new FragtBeregner();
        decimal ordreBeløb = 100;
        decimal forventetFragt = 50;

        // 2. Act (Når)
        decimal resultat = beregner.BeregnFragt(ordreBeløb);

        // 3. Assert (Så)
        Assert.Equal(forventetFragt, resultat);
    }
}
```

*Her bekræfter vi "Happy Path" – at koden virker under normale omstændigheder.*

------

## 3. Datadrevet test: `[Theory]` og Grænseværdier

Fejl i software opstår sjældent "midt i" tallene. De opstår i grænserne (Edge Cases). I vores tilfælde er grænsen 500 kr.

En robust teststrategi kræver **Boundary Value Analysis**. Vi skal teste:

1. Lige under grænsen (499)
2. På grænsen (500)
3. Lige over grænsen (501)

At skrive tre separate `[Fact]`-tests er spild af tid. I stedet bruger vi `[Theory]`, som lader os genbruge testmetoden med forskellige data via `[InlineData]`.

C#

```c#
[Theory]
[InlineData(499, 50)] // Lige under: Skal koste penge
[InlineData(500, 0)]  // På grænsen: Skal være gratis (kritisk punkt!)
[InlineData(501, 0)]  // Over grænsen: Skal være gratis
public void BeregnFragt_BeløbErOmkringGrænse_ReturnererKorrektFragt(
    decimal ordreBeløb, 
    decimal forventetFragt)
{
    // Arrange
    var beregner = new FragtBeregner();

    // Act
    decimal resultat = beregner.BeregnFragt(ordreBeløb);

    // Assert
    Assert.Equal(forventetFragt, resultat);
}
```

*Ved at bruge Theory sikrer vi, at logikken holder vand præcis der, hvor den "knækker".*

------

## 4. Test af fejlscenarier (Exceptions)

En god testsuite tjekker også, at systemet afviser ugyldige data. Lad os sige, at vi opdaterer `FragtBeregner` til at kaste en fejl, hvis beløbet er negativt.

C#

```c#
// Ny logik i FragtBeregner
if (ordreBeløb < 0) throw new ArgumentException("Negativt beløb");
```

For at teste dette bruger vi `Assert.Throws`. Her tester vi ikke returværdien, men at en specifik handling udløser en fejl.

C#

```c#
[Fact]
public void BeregnFragt_NegativtBeløb_KasterArgumentException()
{
    // Arrange
    var beregner = new FragtBeregner();
    decimal ugyldigtBeløb = -10;

    // Act & Assert
    // Vi "pakker" handlingen ind, så xUnit kan overvåge den
    Assert.Throws<ArgumentException>(() => beregner.BeregnFragt(ugyldigtBeløb));
}
```

*Bemærk navngivningen: Vi tydeliggør i navnet, at resultatet er en "Exception".*

------

## 5. Renere kode: `[MemberData]` og Constructor Setup

Når dine tests vokser, kan koden blive rodet. Her er to teknikker til at rydde op.

### Fælles opsætning via Constructor

Hvis du skriver `var beregner = new FragtBeregner();` i hver eneste test, bryder du DRY-princippet (Don't Repeat Yourself). I xUnit fungerer klassens Constructor som setup-metode. Den køres før *hver* test.

### Komplekse data med MemberData

Hvis du har mange test-cases, eller hvis dine data er objekter snarere end simple tal, fylder `[InlineData]` for meget. Brug i stedet `[MemberData]`.

Her er et eksempel, der kombinerer begge dele:

C#

```c#
public class FragtBeregnerTests
{
    private readonly FragtBeregner _beregner;

    // 1. Constructor Setup: Kører før hver test
    public FragtBeregnerTests()
    {
        _beregner = new FragtBeregner();
    }

    // 2. Datakilde til MemberData
    public static TheoryData<decimal, decimal> FragtScenarier
    {
        get
        {
            var data = new TheoryData<decimal, decimal>();
            data.Add(100, 50);
            data.Add(500, 0);
            data.Add(1000, 0);
            return data;
        }
    }

    [Theory]
    [MemberData(nameof(FragtScenarier))]
    public void BeregnFragt_ForskelligeScenarier_ReturnererForventet(
        decimal beløb, 
        decimal forventet)
    {
        // Arrange er klaret i constructoren!

        // Act
        var resultat = _beregner.BeregnFragt(beløb);

        // Assert
        Assert.Equal(forventet, resultat);
    }
}
```

------

## 6. Asynkrone Tests (`async` / `await`)

Moderne .NET kode taler ofte med databaser eller API'er asynkront. xUnit understøtter dette direkte. Testmetoden skal blot ændres fra `void` til `async Task`.

C#

```c#
[Fact]
public async Task BeregnFragtAsync_GyldigtBeløb_ReturnererBeregnetVærdi()
{
    // Arrange
    var beregner = new FragtBeregner();

    // Act - bemærk await keywordet
    decimal resultat = await beregner.BeregnFragtAsync(100);

    // Assert
    Assert.Equal(50, resultat);
}
```

------

## Opsamling

Du har nu en komplet værktøjskasse til unit testing i xUnit v3:

1. **Navngivning:** `Given_When_Then` sikrer læsbarhed.
2. **`[Fact]`:** Til simple, uforanderlige tests.
3. **`[Theory]`:** Til systematisk test af grænseværdier.
4. **`Assert.Throws`:** Til at sikre korrekt fejlhåndtering.
5. **Refaktorering:** Brug `Constructor` og `[MemberData]` til at holde test-koden vedligeholdelsesvenlig.

**Næste skridt:** En naturlig forlængelse af dette er at lære om **Mocking**. Det bliver nødvendigt, når din `FragtBeregner` ikke bare skal returnere et tal, men måske skal slå en vare op i en database. Vil du se et eksempel på det?



------

## 7. Mocking: Når koden har afhængigheder

Indtil nu har vores `FragtBeregner` været isoleret. Den har kun brugt ren matematik. Men i virkeligheden er vores kode sjældent alene. Den skal slå op i en database, kalde et API, eller læse en fil.

Hvis vi tester kode, der kalder en *rigtig* database, er det ikke længere en **Unit Test**, men en **Integrations Test**. Det er langsomt, og data kan ændre sig.

Vi har brug for at "fake" databasen. Det kaldes **Mocking**.

### Scenariet: VIP Kunder

Vi udvider vores system. Hvis en kunde er VIP, er fragten altid gratis. Vi har ikke en database, men vi har et interface, der repræsenterer den:

C#

```
public interface IKundeService
{
    bool ErKundeVip(int kundeId);
}
```

Vi opdaterer vores `FragtBeregner` til at bruge dette interface (Dependency Injection):

C#

```
public class FragtBeregner
{
    private readonly IKundeService _kundeService;

    // Vi tager interfacet ind gennem constructoren
    public FragtBeregner(IKundeService kundeService)
    {
        _kundeService = kundeService;
    }

    public decimal BeregnFragt(int kundeId, decimal ordreBeløb)
    {
        // Hvis kunden er VIP, returner 0 (gratis)
        if (_kundeService.ErKundeVip(kundeId))
        {
            return 0;
        }

        // Ellers brug standard logik
        if (ordreBeløb < 500)
            return 50;
            
        return 0;
    }
}
```

### Installation af Moq

For at mocke interfacet bruger vi biblioteket **Moq** (udtales "Mock").

Du installerer det via NuGet:

```
dotnet add package Moq
```

### Sådan skriver du testen med Moq

Vi vil teste, at en VIP kunde får gratis fragt, *selvom* beløbet er under 500 kr.

Vi skal bruge Moq til at fortælle systemet: *"Når nogen spørger, om kunde nr. 1 er VIP, så svar JA"*.

C#

```
using Xunit;
using Moq; // Husk denne

public class FragtBeregnerTests
{
    [Fact]
    public void BeregnFragt_KundeErVip_ReturnererNul()
    {
        // 1. Arrange
        // Vi opretter en Mock af interfacet (en "falsk" service)
        var mockService = new Mock<IKundeService>();

        // SETUP: Vi træner mock'en.
        // "Når ErKundeVip kaldes med ID 1, så returnér true"
        mockService.Setup(service => service.ErKundeVip(1))
                   .Returns(true);

        // Vi giver mock'ens OBJEKT videre til vores beregner
        var beregner = new FragtBeregner(mockService.Object);

        // 2. Act
        // Vi tester med et lavt beløb (som normalt ville koste 50 kr)
        decimal resultat = beregner.BeregnFragt(1, 100);

        // 3. Assert
        Assert.Equal(0, resultat);
    }
}
```

### Hvad skete der lige her?

1. **`new Mock<IKundeService>()`**: Vi laver en tom skal, der ligner `IKundeService`.
2. **`.Setup(...)`**: Vi programmerer skallen. Uden dette ville metoden bare returnere `false` (default værdi for bool).
3. **`.Object`**: Det er den instans, der rent faktisk ligner interfacet, som vi kan give til vores klasse.

------

## 8. Verificering: Blev metoden kaldt?

Nogle gange er resultatet ikke en returværdi, men en handling (f.eks. "Gem i loggen" eller "Send en email"). Her kan vi bruge Moq til at tjekke, om koden faktisk gjorde det, den skulle.

Lad os sige, at vi vil sikre os, at `FragtBeregner` rent faktisk tjekker, om kunden er VIP.

C#

```
[Fact]
public void BeregnFragt_KalderKundeService_EnGang()
{
    // Arrange
    var mockService = new Mock<IKundeService>();
    var beregner = new FragtBeregner(mockService.Object);

    // Act
    beregner.BeregnFragt(99, 500);

    // Assert (Verify)
    // Vi tjekker: Blev metoden 'ErKundeVip' kaldt med ID 99 præcis én gang?
    mockService.Verify(service => service.ErKundeVip(99), Times.Once);
}
```

Dette er ekstremt nyttigt for at sikre, at din applikation ikke "glemmer" at kalde vigtige funktioner, eller at den ikke kalder dem for mange gange (hvilket kan være dårligt for performance).

------

## Afsluttende opsummering

Du har nu været igennem en komplet rejse i xUnit.net v3 og Moq:

1. **Struktur:** Vi bruger `Given_When_Then` for at gøre tests læsbare.
2. **Basis:** `[Fact]` til simple tests.
3. **Variation:** `[Theory]` til at teste grænseværdier og undgå fejl.
4. **Robusthed:** `Assert.Throws` til at fange fejlhåndtering.
5. **Isolering:** **Moq** lader os teste vores logik isoleret fra databaser og eksterne systemer ved at simulere (mocke) afhængigheder.

Hvis du mestrer disse 5 punkter, er du langt foran de fleste i din tilgang til softwarekvalitet. God fornøjelse med testningen!