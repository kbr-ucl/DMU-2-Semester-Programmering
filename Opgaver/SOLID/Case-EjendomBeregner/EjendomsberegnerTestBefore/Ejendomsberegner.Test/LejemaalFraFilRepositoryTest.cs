using Ejendomsberegner.Core;
using Ejendomsberegner.Core.FileReaderAdapter;
using Ejendomsberegner.Core.Model;
using Moq;

namespace Ejendomsberegner.Test;

public class LejemaalFraFilRepositoryTest
{
    [Fact]
    public void Given_HentLejemaal_Faar_Korrekte_Datalinjer__Then_Lejemaal_Liste_Dannes_Korrekt()
    {
        // Arrange
        var lejemaalData = new[]
        {
            new string("\"lejlighednummer\"; \"kvadratmeter\"; \"antal rum\""),
            new string("\"101\"; \"755\"; \"3\""),
            new string("\"102\"; \"600\"; \"2\"")
        };

        var expected = new List<Lejemaal>
        {
            new() { AntalRum = 3, Kvadratmeter = 755, Lejlighednummer = 101 },
            new() { AntalRum = 2, Kvadratmeter = 600, Lejlighednummer = 102 }
        };

        // TODO: setup sut (System Under Test) here - remember to mock dependencies
        var sut = new LejemaalFraFilRepository() as ILejemaalRepository;

        // Act
        var actual = sut.HentLejemaal();

        // Assert
        // This is a placeholder test method.
        Assert.Equal(expected, actual);
    }

    [Theory]
    // "lejlighednummer";"kvadratmeter";"antal rum"
    [InlineData("\"10x1\"; \"755\"; \"3\"")]
    [InlineData("\"10x1\"; \"755,0\"; \"3\"")]
    [InlineData("\"10x1\"; \"755,0\"; \"3.2\"")]
    public void Given_DanLejemaalObjekt_Faar_Forkerte_Datatyper__Then_Thow_Exception(string lejemaalLinje)
    {
        // Arrange
        // TODO: setup sut (System Under Test) here - remember to mock dependencies
        var service = new LejemaalFraFilRepositoryStub();

        // Act & Assert
        Assert.Throws<Exception>(() => service.DanLejemaalObjekt(lejemaalLinje));
    }

    [Theory]
    // "lejlighednummer";"kvadratmeter";"antal rum"
    [InlineData("\"101\"; \"755\"")]
    public void Given_DanLejemaalObjekt_Faar_Forkerte_Antal_Elementer_I_En_Linje__Then_Thow_Exception(
        string lejemaalLinje)
    {
        // TODO: write test method
        // Arrange


        // Act & Assert
    }

    /// <summary>
    /// Provides a stub implementation of the LejemaalFraFilRepository class for testing or development purposes.
    /// </summary>
    /// <remarks>This class overrides selected members of LejemaalFraFilRepository to allow for controlled
    /// behavior in test scenarios. It can be used to simulate file-based repository operations without requiring access
    /// to actual data files.</remarks>
    public class LejemaalFraFilRepositoryStub : LejemaalFraFilRepository
    {
        public LejemaalFraFilRepositoryStub(IFile dataFile) : base(dataFile)
        {
        }

        /// <summary>
        /// "Snyder" indkapslingen til at kalde den protectede metode i LejemaalFraFilRepository.
        /// "new" bruges til at skjule basisklassens medlem.
        /// </summary>
        /// <param name="lejemaalPart"></param>
        /// <returns></returns>
        public new string RemoveQuotes(string lejemaalPart)
        {
            return base.RemoveQuotes(lejemaalPart);
        }

        /// <summary>
        /// "Snyder" indkapslingen til at kalde den protectede metode i LejemaalFraFilRepository.
        /// "new" bruges til at skjule basisklassens medlem.
        /// </summary>
        /// <param name="lejemaalData"></param>
        /// <returns></returns>
        public new List<Lejemaal> Konverter(string[] lejemaalData)
        {
            return base.Konverter(lejemaalData);
        }

        /// <summary>
        /// "Snyder" indkapslingen til at kalde den protectede metode i LejemaalFraFilRepository.
        /// "new" bruges til at skjule basisklassens medlem.
        /// </summary>
        /// <param name="lejemaalLinje"></param>
        /// <returns></returns>
        public new Lejemaal DanLejemaalObjekt(string lejemaalLinje)
        {
            return base.DanLejemaalObjekt(lejemaalLinje);
        }
    }
}