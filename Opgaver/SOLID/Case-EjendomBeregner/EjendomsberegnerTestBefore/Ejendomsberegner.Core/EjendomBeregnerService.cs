namespace Ejendomsberegner.Core;

/// <summary>
///     Kan funktionaliteten opdeles således koden bliver mere funktionel "ren" (en metode ét ansvar)?
///     Er der et skjult model objekt i opgaven?
///     Hvad skal retur typen for adapteren være?
/// </summary>
public class EjendomBeregnerService : IEjendomBeregnerService
{
    private readonly ILejemaalRepository _repo;

    public EjendomBeregnerService(ILejemaalRepository repo)
    {
        _repo = repo;
    }

    /// <summary>
    ///     Beregner ejendommens kvadratmeter ud fra ejendommens lejelmål.
    ///     Lejemål er i en simikolon separeret tekstfil. Formatet af filen er:
    ///     lejlighednummer; kvadratmeter; antal rum
    ///     lejlighednummer: int
    ///     kvadratmeter: double
    ///     antal rum: double
    ///     Første linje i filen er en header og skal ignoreres.
    ///     Filen med lejemål er med i projektet og hedder "LejemaalData.csv"
    /// </summary>
    /// <returns></returns>
    double IEjendomBeregnerService.BeregnKvadratmeter()
    {
        var lejemaalene = _repo.HentLejemaal();
        var kvadratmeter = 0.0;


        foreach (var lejemaal in lejemaalene) kvadratmeter += lejemaal.Kvadratmeter;

        return kvadratmeter;
    }
}