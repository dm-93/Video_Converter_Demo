using Bitmovin.Api.Sdk.Models;
using System.Threading.Tasks;

namespace Cambium_Video.Services.Interfaces
{
    public interface IEncodingBuider
    {
        Task<Encoding> CreateAndConfigureEncoding(Input input,
                                                  string inputPath,
                                                  Output output,
                                                  string outputPath);
    }
}
