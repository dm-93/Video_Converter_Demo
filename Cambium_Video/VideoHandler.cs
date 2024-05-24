using Bitmovin.Api.Sdk.Common.Logging;
using Bitmovin.Api.Sdk;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Bitmovin.Api.Sdk.Models;
using System.Threading.Tasks;
using System;
using Cambium_Video.Services.Interfaces;
using Cambium_Video.Services;
using Enc = Bitmovin.Api.Sdk.Models;

namespace Cambium_Video
{
    public class VideoHandler
    {
        private readonly BitmovinApi bitmovinApi;
        private IEncodingBuider encodingBuider;

        public VideoHandler()
        {
            
            bitmovinApi = BitmovinApi.Builder
                                .WithApiKey("")
                                .WithLogger(new ConsoleLogger())
                                .Build();
            this.encodingBuider = new MP4EncodingBuilder(bitmovinApi);
        }

        [FunctionName("HandleVideo")]
        public async Task Run([BlobTrigger("video-container/{name}", Connection = "connection-string")] System.IO.Stream myBlob, string name, ILogger log)
        {
            var input = await bitmovinApi.Encoding.Inputs.Azure.GetAsync("a82581df-bd60-4a80-8478-fe291fd87770");

            input ??= await CreateInputAsync();

            var output = await bitmovinApi.Encoding.Outputs.Azure.GetAsync("f957fe84-b224-4878-a486-c415c471be14");

            output ??= await CreateOutputAsync();

            var outputPath = "";

            var encoding = await encodingBuider.CreateAndConfigureEncoding(input, name, output, outputPath);

            try
            {
                await bitmovinApi.Encoding.Encodings.StartAsync(encoding.Id);
                Task<Enc.ServiceTaskStatus> GetEncodingStatus() => bitmovinApi.Encoding.Encodings.StatusAsync(encoding.Id);

                var status = await GetEncodingStatus();
                while (status.Status == Status.RUNNING || status.Status == Status.QUEUED)
                {
                    Console.WriteLine($"Encoding status: {status.Status} (progress: {status.Progress}%)");
                    await Task.Delay(10000);
                    status = await GetEncodingStatus();
                }

                if (status.Status == Status.FINISHED)
                {
                    Console.WriteLine("Encoding finished successfully.");
                }
                else
                {
                    Console.WriteLine($"Encoding failed with status: {status.Status}");
                }
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
            }
        }

        private async Task<AzureOutput> CreateOutputAsync()
        {
            var output = new AzureOutput
            {
                AccountName = "",
                AccountKey = "",
                Container = ""
            };
            return await bitmovinApi.Encoding.Outputs.Azure.CreateAsync(output);
        }

        private async Task<AzureInput> CreateInputAsync()
        {
            var a = new AzureInput
            {
                AccountName = "",
                AccountKey = "",
                Container = ""
            };
            var input = await bitmovinApi.Encoding.Inputs.Azure.CreateAsync(a);
            return input;
        }

        
    }
}
