// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EchoWithCounter
{
    /// <summary>
    /// The Startup class configures services and the request pipeline.
    /// </summary>
    public class Startup
    {
        private bool _isProduction = false;
        public Startup(IHostingEnvironment env)
        {
            _isProduction = env.IsProduction();
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        /// <summary>
        /// Gets the configuration that represents a set of key/value application configuration properties.
        /// </summary>
        /// <value>
        /// The <see cref="IConfiguration"/> that represents a set of key/value application configuration properties.
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> specifies the contract for a collection of service descriptors.</param>
        /// <seealso cref="IStatePropertyAccessor{T}"/>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/dependency-injection"/>
        /// <seealso cref="https://docs.microsoft.com/en-us/azure/bot-service/bot-service-manage-channels?view=azure-bot-service-4.0"/>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBot<EchoWithCounterBot>(options =>
           {
               var secretKey = Configuration.GetSection("botFileSecret")?.Value;

                // Loads .bot configuration file and adds a singleton that your Bot can access through dependency injection.
                var botConfig = BotConfiguration.Load(@".\EchoWithCounter.bot", secretKey);
               services.AddSingleton(sp => botConfig);

                // Retrieve current endpoint.
                var service = botConfig.Services.Where(s => s.Type == "endpoint" && s.Name == "development").FirstOrDefault();
               if (!(service is EndpointService endpointService))
               {
                   throw new InvalidOperationException($"The .bot file does not contain a development endpoint.");
               }

               options.CredentialProvider = new SimpleCredentialProvider(endpointService.AppId, endpointService.AppPassword);

                // Catches any errors that occur during a conversation turn and logs them.
                options.OnTurnError = async (context, exception) =>
               {
                   await context.SendActivityAsync("Sorry, it looks like something went wrong.");
               };


               //IStorage storage = new MemoryStorage();
               ////var state = new ConversationState(storage);
               //options.Middleware.Add(new ConversationState(storage));
           });
            services.AddSingleton(sp => {
                //memory storage
                IStorage dataStore = new MemoryStorage();

                //Azure Storage
                //const string StorageConfigurationId = "";
                //var blobConfig = botConfig.FindServiceByNameOrId(StorageConfigurationId);
                //if (!(blobConfig is BlobStorageService blobStorageConfig))
                //{
                //    throw new InvalidOperationException($"The .bot file does not contain an blob storeage");
                //}
                //const string DefaultBotContainer = "";
                //var storageContainer = string.IsNullOrWhiteSpace(blobStorageConfig.Container) ? DefaultBotContainer : blobStorageConfig.Container;
                //IStorage dataStore1 = new Microsoft.Bot.Builder.Azure.AzureBlobStorage(blobStorageConfig.ConnectionString,
                //    storageContainer);
                var conversationState = new ConversationState(dataStore);
                return conversationState;
            });
            services.AddSingleton<EchoBotAccessors>(sp =>
            {
                var conversationState = sp.GetRequiredService<ConversationState>();
                return new EchoBotAccessors(conversationState)
                {
                    CounterState = conversationState.CreateProperty<CounterState>(EchoBotAccessors.CounterStateName)
                };
            });


        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework();
        }
    }
}
