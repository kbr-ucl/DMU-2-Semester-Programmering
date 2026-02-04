using Ejendomsberegner.Core.FileReaderAdapter;
using Ejendomsberegner.Core.Model;

namespace Ejendomsberegner.Core;

public class LejemaalFraFilRepository : ILejemaalRepository
{
    private readonly IFile _dataFile;

    public LejemaalFraFilRepository(IFile dataFile)
    {
        _dataFile = dataFile;
    }

    List<Lejemaal> ILejemaalRepository.HentLejemaal()
    {
        var raaData = _dataFile.ReadAllLines().Skip(1).ToArray();
        var lejemaal = Konverter(raaData);
        return lejemaal;
    }

    protected List<Lejemaal> Konverter(string[] lejemaalData)
    {
        var lejemaalListe = new List<Lejemaal>();
        foreach (var lejemaalLinje in lejemaalData) lejemaalListe.Add(DanLejemaalObjekt(lejemaalLinje));

        return lejemaalListe;
    }

    protected Lejemaal DanLejemaalObjekt(string lejemaalData)
    {
        var lejemaalParts = lejemaalData.Split(';');
        if (lejemaalParts.Length < 3)
            throw new Exception(
                $"Filformat forkert - der er indlæst en linje med {lejemaalParts.Length} elementer");

        double lejemaalKvadratmeter;
        int lejemaalNummer;
        int antalRum;
        if (!double.TryParse(RemoveQuotes(lejemaalParts[1]), out lejemaalKvadratmeter))
            throw new Exception(
                $"LejemaalKvadratmeter kan ikke konverteres til en double - input data: {lejemaalParts[1]}");

        if (!int.TryParse(RemoveQuotes(lejemaalParts[0]), out lejemaalNummer))
            throw new Exception(
                $"LejemaalNummer kan ikke konverteres til en int - input data: {lejemaalParts[0]}");

        if (!int.TryParse(RemoveQuotes(lejemaalParts[2]), out antalRum))
            throw new Exception($"AntalRum kan ikke konverteres til en int - input data: {lejemaalParts[2]}");

        return new Lejemaal
        {
            Lejlighednummer = lejemaalNummer,
            Kvadratmeter = lejemaalKvadratmeter,
            AntalRum = antalRum
        };
    }

    protected string RemoveQuotes(string lejemaalPart)
    {
        return lejemaalPart.Replace('"', ' ').Trim();
    }
}