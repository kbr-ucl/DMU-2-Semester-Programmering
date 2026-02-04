// See https://aka.ms/new-console-template for more information

using Ejendomsberegner.Core;
using Ejendomsberegner.Core.FileReaderAdapter;
using Microsoft.Extensions.DependencyInjection;

var services = CreateServices();
// Resolve the IVatCalculator service from the service provider
var kvmCalculator = services.GetRequiredService<IEjendomBeregnerService>();


Console.WriteLine("Hello, World!");

var kvadratmeter = kvmCalculator.BeregnKvadratmeter();
Console.WriteLine($"Ejendommens samlede kvadratmeter er: {kvadratmeter}");


ServiceProvider CreateServices()
{
    var serviceProvider = new ServiceCollection()
        .AddScoped<IEjendomBeregnerService, EjendomBeregnerService>()
        .AddScoped<ILejemaalRepository, LejemaalFraFilRepository>()
        .AddScoped<IFile, FileAdapter>(f => new FileAdapter("LejemaalData.csv"))
        .BuildServiceProvider();
    return serviceProvider;
}