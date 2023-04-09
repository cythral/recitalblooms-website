using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Amazon.S3;
using Amazon.S3.Model;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using RecitalBlooms.Website.Cicd.Utils;

namespace RecitalBlooms.Website.Cicd.DeployDriver
{
    /// <inheritdoc />
    public class Host : IHost
    {
        private readonly StackDeployer deployer;
        private readonly EcsDeployer ecsDeployer;
        private readonly EcrUtils ecrUtils;
        private readonly CommandLineOptions options;
        private readonly IHostApplicationLifetime lifetime;

        /// <summary>
        /// Initializes a new instance of the <see cref="Host" /> class.
        /// </summary>
        /// <param name="deployer">Service for deploying cloudformation stacks.</param>
        /// <param name="ecsDeployer">Service for deploying ECS services.</param>
        /// <param name="ecrUtils">Utilities for interacting with ECR.</param>
        /// <param name="options">Command line options.</param>
        /// <param name="lifetime">Service that controls the application lifetime.</param>
        /// <param name="serviceProvider">Object that provides access to the program's services.</param>
        public Host(
            StackDeployer deployer,
            EcsDeployer ecsDeployer,
            EcrUtils ecrUtils,
            IOptions<CommandLineOptions> options,
            IHostApplicationLifetime lifetime,
            IServiceProvider serviceProvider
        )
        {
            this.deployer = deployer;
            this.ecsDeployer = ecsDeployer;
            this.ecrUtils = ecrUtils;
            this.options = options.Value;
            this.lifetime = lifetime;
            Services = serviceProvider;
        }

        /// <inheritdoc />
        public IServiceProvider Services { get; }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            EnvironmentConfig? config = null;

            await Step($"Pull {options.Environment} config", async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var key = $"{options.ArtifactsLocation!.AbsolutePath.TrimStart('/')}/config.{options.Environment}.json";
                var s3 = new AmazonS3Client();
                var request = new GetObjectRequest
                {
                    BucketName = options.ArtifactsLocation!.Host,
                    Key = key,
                };

                var response = await s3.GetObjectAsync(request, cancellationToken);
                config = await JsonSerializer.DeserializeAsync<EnvironmentConfig>(response.ResponseStream, cancellationToken: cancellationToken);

                Console.WriteLine("Loaded configuration from S3.");
            });

            await Step($"Deploy template to {options.Environment}", async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var context = new DeployContext
                {
                    StackName = "recitalblooms-website",
                    TemplateURL = $"https://{options.ArtifactsLocation!.Host}.s3.amazonaws.com{options.ArtifactsLocation!.AbsolutePath}/template.yml",
                    Parameters = config?.Parameters ?? new(),
                    Capabilities = { "CAPABILITY_IAM", "CAPABILITY_AUTO_EXPAND" },
                    Tags = config?.Tags ?? new(),
                };

                await deployer.Deploy(context, cancellationToken);
            });

            lifetime.StopApplication();
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private static async Task Step(string title, Func<Task> action)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n{title} ==========\n");
            Console.ResetColor();

            await action();
        }
    }
}
