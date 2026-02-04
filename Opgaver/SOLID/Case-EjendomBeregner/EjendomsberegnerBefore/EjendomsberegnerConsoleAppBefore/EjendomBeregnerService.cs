namespace EjendomsberegnerConsoleAppBefore;

public class EjendomBeregnerService
{
    public string DataFileName { get; set; } = "LejemaalData.csv";

    /// <summary>
    ///     Beregner ejendommens kvadratmeter ud fra ejendommens lejelmål.
    ///     Lejemål er i en simikolon separeret tekstfil. Formatet af filen er:
    ///     lejlighednummer; kvadratmeter; antal rum
    ///     lejlighednummer: int
    ///     kvadratmeter: double
    ///     antal rum: double
    /// 
    ///     Første linje i filen er en header og skal ignoreres.
    /// 
    ///     Filen med lejemål er med i projektet og hedder "LejemaalData.csv"
    /// </summary>
    /// <returns></returns>
    public double BeregnKvadratmeter()
    {
        string[] lejemaalene = File.ReadAllLines(DataFileName).Skip(1).ToArray();
        double kvadratmeter = 0.0;


        foreach (string lejemaal in lejemaalene)
        {
            string[] lejemaalParts = lejemaal.Split(';');
            double lejemaalKvadratmeter;
            double.TryParse(RemoveQuotes(lejemaalParts[1]), out lejemaalKvadratmeter);
            kvadratmeter += lejemaalKvadratmeter;
        }

        return kvadratmeter;
    }

    private string RemoveQuotes(string lejemaalPart)
    {
        return lejemaalPart.Replace('"', ' ').Trim();
    }
}