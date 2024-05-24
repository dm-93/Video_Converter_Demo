using Bitmovin.Api.Sdk.Models;
using System.Threading.Tasks;

namespace Cambium_Video.Services.Interfaces
{
    public interface IConfigurationService
    {
        Task<CodecConfiguration> GetConfigurationAsync();
    }
}
