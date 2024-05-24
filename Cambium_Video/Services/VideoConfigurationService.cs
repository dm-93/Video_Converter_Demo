using Bitmovin.Api.Sdk.Models;
using Bitmovin.Api.Sdk;
using System.Threading.Tasks;
using Cambium_Video.Services.Interfaces;

namespace Cambium_Video.Services
{
    internal sealed class VideoConfigurationService : IConfigurationService
    {
        private readonly BitmovinApi bitmovinApi;
        public VideoConfigurationService(BitmovinApi bitmovinApi)
        {
            this.bitmovinApi = bitmovinApi;
        }

        public async Task<CodecConfiguration> GetConfigurationAsync()
        {
            H264VideoConfiguration codecConfigVideo = null;//await bitmovinApi.Encoding.Configurations.Video.H264.GetAsync("<H264_CC_ID>");

            if (codecConfigVideo is null)
            {
                var codecConfigVideoName = "My H264 Codec Config";
                var codecConfigVideoBitrate = 5000000;
                codecConfigVideo = await bitmovinApi.Encoding.Configurations.Video.H264.CreateAsync(new H264VideoConfiguration
                {
                    Name = codecConfigVideoName,
                    PresetConfiguration = PresetConfiguration.VOD_STANDARD,
                    Width = 1920,
                    Height = 1080,
                    Rate = 30.0f,
                    Bitrate = codecConfigVideoBitrate,
                    Description = codecConfigVideoName + "_" + codecConfigVideoBitrate
                });
            }
            return codecConfigVideo;
        }
    }
}
