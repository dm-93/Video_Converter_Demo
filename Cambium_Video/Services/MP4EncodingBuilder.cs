using Bitmovin.Api.Sdk;
using Bitmovin.Api.Sdk.Models;
using Cambium_Video.Services.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Models = Bitmovin.Api.Sdk.Models;

namespace Cambium_Video.Services
{
    internal sealed class MP4EncodingBuilder : IEncodingBuider
    {
        private readonly BitmovinApi bitmovinApi;
        private readonly IConfigurationService configurationService;
        public MP4EncodingBuilder(BitmovinApi bitmovinApi)
        {
            this.bitmovinApi = bitmovinApi;
            this.configurationService = new VideoConfigurationService(bitmovinApi);
        }

        public async Task<Models.Encoding> CreateAndConfigureEncoding(Input input,
                                                      string inputPath,
                                                      Output output,
                                                      string outputPath)
        {
            var codecConfigurations = new List<CodecConfiguration> { await configurationService.GetConfigurationAsync() };
            var encoding = await CreateEncodingAsync();
            foreach (var codecConfig in codecConfigurations)
            {
                var stream = await CreateStreamAsync(encoding, input, inputPath, codecConfig);

                string muxingOutputPath;
                if (codecConfig is VideoConfiguration)
                {
                    muxingOutputPath = $"{outputPath}/video/{((VideoConfiguration)codecConfig).Height}";
                }
                else
                {
                    muxingOutputPath = $"{outputPath}/audio/{((AudioConfiguration)codecConfig).Bitrate / 1000}";
                }

                await CreateMp4Muxing(encoding, stream, output, muxingOutputPath);
            }
            return encoding;
        }

        private async Task<Models.Encoding> CreateEncodingAsync()
        {
            var encoding = await bitmovinApi.Encoding.Encodings.CreateAsync(new Models.Encoding
            {
                Name = "My encoding from Mov to MP4 Encoding",
                CloudRegion = CloudRegion.AZURE_EUROPE_WEST
            });
            return encoding;
        }

        private async Task<Models.Stream> CreateStreamAsync(Models.Encoding encoding, Input input, string blobName, CodecConfiguration codecConfiguration)
        {
            string inputPath = blobName;

            var streamInput = new StreamInput
            {
                InputId = input.Id,
                InputPath = inputPath,
                SelectionMode = StreamSelectionMode.AUTO
            };

            var createdStream = await bitmovinApi.Encoding.Encodings.Streams.CreateAsync(encoding.Id, new Models.Stream
            {
                InputStreams = new List<StreamInput>
                                {
                                  streamInput
                                },
                CodecConfigId = codecConfiguration.Id
            });
            return createdStream;
        }

        private Task CreateMp4Muxing(Models.Encoding encoding, Models.Stream stream, Output output,
          string outputPath)
        {
            var muxingStream = new MuxingStream()
            {
                StreamId = stream.Id
            };

            var muxing = new Mp4Muxing()
            {
                Outputs = new List<EncodingOutput>() { BuildEncodingOutput(output, outputPath) },
                Streams = new List<MuxingStream>() { muxingStream },
                Filename = "output.mp4",
            };

            return bitmovinApi.Encoding.Encodings.Muxings.Mp4.CreateAsync(encoding.Id, muxing);
        }

        private EncodingOutput BuildEncodingOutput(Output output, string outputPath)
        {
            var aclEntry = new AclEntry()
            {
                Permission = AclPermission.PUBLIC_READ
            };

            var encodingOutput = new EncodingOutput()
            {
                OutputPath = "",
                OutputId = output.Id,
                Acl = new List<AclEntry>() { aclEntry }
            };

            return encodingOutput;
        }

        private string BuildAbsolutePath(string outputPath, string relativePath)
        {
            return Path.Join(outputPath, relativePath);
        }
    }
}
