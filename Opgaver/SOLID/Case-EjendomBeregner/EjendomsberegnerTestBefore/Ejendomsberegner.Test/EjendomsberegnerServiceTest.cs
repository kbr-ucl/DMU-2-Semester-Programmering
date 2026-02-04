using Ejendomsberegner.Core;
using Ejendomsberegner.Core.Model;
using Moq;

namespace Ejendomsberegner.Test;

public class EjendomsberegnerServiceTest
{
    [Fact]
    public void Given_BeregnKvadratmeter_Faar_LejemaalListe__Then_Kvadratmeter_Beregnes_Korrekt()
    {
        // Arrange
        var lejemaal = new List<Lejemaal>();
        lejemaal.Add(new Lejemaal { Lejlighednummer = 1, Kvadratmeter = 50.0, AntalRum = 2 });
        lejemaal.Add(new Lejemaal { Lejlighednummer = 2, Kvadratmeter = 75.0, AntalRum = 3 });

        var expected = 125.0;
        // TODO: setup sut (System Under Test) here - remember to mock dependencies
        var sut = new EjendomBeregnerService() as IEjendomBeregnerService;

        // Act
        //TODO: set actual to the result of the method being tested

        // Assert
        Assert.Equal(expected, actual);
    }
}