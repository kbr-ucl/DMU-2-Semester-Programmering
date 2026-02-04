namespace EjendomsberegnerConsoleAppBefore;

// 1. Target interface
public interface ICelsiusSensor
{
    double GetTemperatureCelsius();
}

// 2. Adaptee class (incompatible interface)
public class FahrenheitSensor
{
    public double GetTemperatureFahrenheit()
    {
        return 98.6; // Simuleret temperatur
    }
}

// 3. Adapter class
public class FahrenheitToCelsiusAdapter : ICelsiusSensor
{
    private readonly FahrenheitSensor _fahrenheitSensor;

    public FahrenheitToCelsiusAdapter(FahrenheitSensor fahrenheitSensor)
    {
        _fahrenheitSensor = fahrenheitSensor;
    }

    public double GetTemperatureCelsius()
    {
        double fahrenheit = _fahrenheitSensor.GetTemperatureFahrenheit();
        return (fahrenheit - 32) * 5 / 9;
    }
}

// 4. Client code
class Program
{
    static void Main()
    {
        ICelsiusSensor sensor = new FahrenheitToCelsiusAdapter(new FahrenheitSensor());
        Console.WriteLine($"Temperatur i Celsius: {sensor.GetTemperatureCelsius():F2}°C");
    }
}
