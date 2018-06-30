
using System.Threading.Tasks;

namespace BlazorChat.Server.Services
{
    public interface ITokenGenerator
    {
        Task<string> Generate(string identity);
    }
}
