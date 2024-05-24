using Bitmovin.Api.Sdk;
using Bitmovin.Api.Sdk.Models;
using Cambium_Video.Services.Interfaces;
using System.Threading.Tasks;

namespace Cambium_Video.Services
{
    internal class AudioConfigurationService : IConfigurationService
    {
        private readonly BitmovinApi bitmovinApi;
        public AudioConfigurationService(BitmovinApi bitmovinApi)
        {
            this.bitmovinApi = bitmovinApi;
        }

        public async Task<CodecConfiguration> GetConfigurationAsync()
        {
            AacAudioConfiguration codecConfigAudio = null;//await bitmovinApi.Encoding.Configurations.Audio.Aac.GetAsync("<AAC_CC_ID>");
            if (codecConfigAudio is null)
            {
                codecConfigAudio = await bitmovinApi.Encoding.Configurations.Audio.Aac.CreateAsync(new AacAudioConfiguration
                {
                    Name = "My Audio Codec Config",
                    Bitrate = 128000
                });
            }
            return codecConfigAudio;
        }
    }
}
