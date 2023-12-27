using System.Threading.Tasks;

namespace Tsukuru.Maps.Compiler;

public interface IMapCompiler
{
    Task<bool> ExecuteAsync();
}