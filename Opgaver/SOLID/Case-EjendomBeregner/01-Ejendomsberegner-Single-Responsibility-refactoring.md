# Egendomsberegner - Single Responsibility refactoring



## Opgaven

Opgaven består i at refaktor nedenstående kode



```c#
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
```



## Foreslået proces

Prøv at svar på følgende spørgsmål:

- Kan funktionaliteten opdeles således koden bliver mere funktionel "ren" (en metode ét ansvar)?

- Er der et skjult model objekt i opgaven?

  



Dernæst er anbefalingen at få skrevet nogle klasser samt metode signature, og efterfølgende at fylde funktionalitet i metoderne.



## Hints

Det kan være en fordel at ændre i koden således at der anvendes en modelklasse "Lejemaal" fremfor "rå" tekststrenge fra filen.

```c#
    public class Lejemaal
    {
        public int Lejlighednummer { get; set; }
        public double Kvadratmeter { get; set; }
        public int AntalRum { get; set; }
    }
```

