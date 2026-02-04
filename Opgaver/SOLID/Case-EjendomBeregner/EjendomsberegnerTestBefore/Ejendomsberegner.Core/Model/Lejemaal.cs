namespace Ejendomsberegner.Core.Model;

public record Lejemaal
{
    public int Lejlighednummer { get; set; }
    public double Kvadratmeter { get; set; }
    public int AntalRum { get; set; }
}