using Ejendomsberegner.Core.Model;

namespace Ejendomsberegner.Core;

public interface ILejemaalRepository
{
    List<Lejemaal> HentLejemaal();
}