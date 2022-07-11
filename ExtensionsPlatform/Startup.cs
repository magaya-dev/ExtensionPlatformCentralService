using ExtensionsPlatform.Application.ExtensionActiveAction;
using ExtensionsPlatform.Application.ExtensionCompany;
using ExtensionsPlatform.Application.MgyGateway;
using ExtensionsPlatform.DataStorageClient;
using ExtensionsPlatform.Repo;
using ExtensionsPlatform.Triggers.Validators;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: FunctionsStartup(typeof(ExtensionsPlatform.Startup))]

namespace ExtensionsPlatform
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder
              .Services
              .AddOptions<ApplicationSettings>()
              .Configure<IConfiguration>((settings, configuration) => { configuration.Bind(settings); });

            //builder.Services.AddHttpClient<IAcelinkApiClient, AcelynkApiClient>((s, c) =>
            //{
            //    var conf = s.GetService<IConfiguration>();
            //    var endpoint = conf.GetValue<string>("AcelynkBaseEndpoint");
            //    c.BaseAddress = new Uri(endpoint);
            //    c.DefaultRequestHeaders.Add("Accept", "application/json");
            //});

            builder.Services.AddSingleton<IRequestModelValidator, RequestModelValidator>();
            builder.Services.AddSingleton<ICosmosDBStorageClient, CosmosDBStorageClient>();

            builder.Services.AddSingleton<IExtensionCompanyCosmoRepo, ExtensionCompanyCosmoRepo>();

            builder.Services.AddSingleton<ICompanyService, CompanyService>();

            builder.Services.AddSingleton<IMgyGateWay, MgyGateWay>();

            builder.Services.AddSingleton<IWebhookMgyExtensionDispatcher, WebhookMgyExtensionDispatcher>();

            builder.Services.AddSingleton<IActiveExtensionCosmoRepo, ActiveExtensionCosmoRepo>();
            builder.Services.AddSingleton<IActivarExtensionService, ActivarExtensionService>();
        }
    }
}
