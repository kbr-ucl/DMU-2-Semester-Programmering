## Hvad er adapter-mønstret?

**Adapter** er et klassisk designmønster (fra “Gang of Four”), der gør det muligt for to klasser med **uens interfaces** at arbejde sammen. Tænk på det som en “stik-adapter”: du har en klient, der forventer et bestemt stik (interface), men din eksisterende komponent leverer et andet – adapteren oversætter imellem de to.

**Kerneidé:**

- Definér et **mål-interface** (det din klient vil bruge).
- Implementér en **adapter**, der opfylder mål-interfacet ved at **delegere** til en “fremmed” klasse (det eksisterende API).
- Klienten kender **kun** mål-interfacet – ikke den konkrete implementering.

Fordele:

- Løser kompatibilitetsproblemer uden at ændre eksisterende kode.
- Forbedrer udskiftelighed (plug-and-play).
- Gør koden **testbar**, fordi du kan mocke mål-interfacet.

------

## Case: Indkapsling af `File.ReadAllText`

Direkte kald til `System.IO.File.ReadAllText(path)` er enkelt – men besværligt at **mocke** i tests og binder din kode hårdt til filsystemet. Med adapter-mønstret kan vi:

1. Udsætte et **fil-læsningsinterface** (`IFileReader`).
2. Implementere en **adapter** (`FileReaderAdapter`), der internt bruger `File.ReadAllText`.
3. Bruge interfacet i klientkoden, så vi kan **mocke** i tests og senere **udskifte implementationen** (f.eks. læse fra cloud storage, memory, database, osv.).

### 1) Mål-interface



```c#
public interface IFileReader
{
    string ReadAllText(string path);
}
```



### 2) Adapter, der bruger `System.IO.File`



```c#
using System.IO;

public sealed class FileReaderAdapter : IFileReader
{
    public string ReadAllText(string path)
    {
        // Du kan tilføje argumentvalidering
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Sti må ikke være tom.", nameof(path));
        // Her adapterer vi “det fremmede API” (System.IO.File) til vores eget interface
        return File.ReadAllText(path);
    }

}
```



### 3) Klientkode, der kun kender interfacet



```c#
public sealed class ConfigLoader
{
    private readonly IFileReader *fileReader**;*
        public ConfigLoader(IFileReader fileReader)
    {
        fileReader = fileReader ?? throw new ArgumentNullException(nameof(fileReader));
    }

    public string LoadConfig(string configPath)
    {
        // Klienten er nu uafhængig af System.IO.File
        var json = _fileReader.ReadAllText(configPath);
        // … parse json, valider, osv.
        return json;
    }
}
```



### 4) Brug i “rigtig” kode (f.eks. i Program.cs)



```c#
// I et DI-setup (f.eks. .NET Generic Host) ville du registrere det sådan her:
// services.AddSingleton<IFileReader, FileReaderAdapter>();
var fileReader = new FileReaderAdapter();
var loader = new ConfigLoader(fileReader);
var json = loader.LoadConfig("appsettings.json");
Console.WriteLine(json);
```





------

## Testbarhed: Mock i unit tests

Fordi klienten kun ser `IFileReader`, kan vi nemt mocke læsning uden diskadgang.



```c#
using Xunit;
public sealed class FakeFileReader : IFileReader
{
    private readonly string content;
    public FakeFileReader(string content) => content = content;
    public string ReadAllText(string path) => content;
}

public class ConfigLoaderTests
{
    [Fact]
    public void LoadConfig_Returns_Content_From_Reader()
    {
        // Arrange
        var fake = new FakeFileReader("{ \"env\": \"Test\" }");
        var loader = new ConfigLoader(fake);

        // Act
        var json = loader.LoadConfig("ignored.json");
        
        // Assert
        Assert.Contains("\"env\": \"Test\"", json);
    }
}
```



Du kan selvfølgelig også bruge en mocking-ramme (Moq, NSubstitute, FakeItEasy), men ovenstående er begynder-venligt og uden ekstra pakker.

------

## Hvorfor er det her adapter – og ikke bare “et interface”?

Godt spørgsmål. Du kunne kalde det en **facade** eller blot “abstraktion”. Forskellen her er, at vi eksplicit **tilpasser** et eksisterende API (statisk klasse `File`) til et **nyt interface**, som klienten allerede forventer. Det er netop adapterens rolle: *at få noget, der ikke passer, til at passe*, uden at ændre på originalen.

- **Target (mål):** `IFileReader`
- **Adaptee (eksisterende API):** `System.IO.File`
- **Adapter:** `FileReaderAdapter` (oversætter kaldene)

------



## Moq

### En simpel test med Moq + xUnit:

```c#
using Moq;
using Xunit;

public class ConfigLoaderTests
{
    [Fact]
    public void LoadConfig_Returns_Content_From_IFileReader()
    {
        // Arrange
        var mock = new Mock<IFileReader>();
        mock.Setup(r => r.ReadAllText("appsettings.json"))
            .Returns("{\"env\":\"Test\"}");

        var sut = new ConfigLoader(mock.Object);

        // Act
        var result = sut.LoadConfig("appsettings.json");

        // Assert
        Assert.Contains("\"env\":\"Test\"", result);

        // Verificér at metoden blev kaldt præcis én gang med samme sti
        mock.Verify(r => r.ReadAllText("appsettings.json"), Times.Once);
        mock.VerifyNoOtherCalls();
    }
}
```

### Brug **It.Is** til fleksibel matching. Du kan også have *fallback* setups:

```c#
[Fact]
public void LoadConfig_Uses_Correct_Path_Prefix_And_Returns_Fallback_When_Not_Matched()
{
    // Arrange
    var mock = new Mock<IFileReader>();

    // Specifikt match: hvis path slutter på ".json"
    mock.Setup(r => r.ReadAllText(It.Is<string>(p => p.EndsWith(".json"))))
        .Returns("{\"type\":\"json\"}");

    // Fallback: alt andet
    mock.Setup(r => r.ReadAllText(It.IsAny<string>()))
        .Returns("UNKNOWN");

    var sut = new ConfigLoader(mock.Object);

    // Act
    var resJson = sut.LoadConfig("settings.json");
    var resOther = sut.LoadConfig("notes.txt");

    // Assert
    Assert.Contains("\"type\":\"json\"", resJson);
    Assert.Equal("UNKNOWN", resOther);

    mock.Verify(r => r.ReadAllText("settings.json"), Times.Once);
    mock.Verify(r => r.ReadAllText("notes.txt"), Times.Once);
}

```



### Stubbing af exceptions (negativ test)

```c#
[Fact]
public void LoadConfig_Throws_If_FileReader_Fails()
{
    // Arrange
    var mock = new Mock<IFileReader>();
    mock.Setup(r => r.ReadAllText(It.IsAny<string>()))
        .Throws(new FileNotFoundException("Filen findes ikke.", "missing.json"));

    var sut = new ConfigLoader(mock.Object);

    // Act + Assert
    var ex = Assert.Throws<FileNotFoundException>(() => sut.LoadConfig("missing.json"));
    Assert.Equal("missing.json", ex.FileName);
}

```

