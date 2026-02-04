namespace Ejendomsberegner.Core.FileReaderAdapter;

public class FileAdapter : IFile
{
    private readonly string _dataFilePath;

    public FileAdapter(string dataFilePath)
    {
        // Precondition: Filen skal eksistere
        if (!File.Exists(dataFilePath))
            throw new FileNotFoundException("Data filen blev ikke fundet", dataFilePath);

        _dataFilePath = dataFilePath;
    }

    string[] IFile.ReadAllLines()
    {
        return File.ReadAllLines(_dataFilePath);
    }
}