using Application.Interfaces;
using CaseService.API.CaseService.Application.Interfaces;
using CaseService.API.CaseService.Infrastructure.Messaging;
using CaseService.API.CaseService.Infrastructure.Repositories;
using Infrastructure.Messaging;
using Infrastructure.Options;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure (this IServiceCollection services, IConfiguration config)
        {
            var embedBase = config["EmbeddingService:BaseUrl"]
                            ?? "http://embedding:8080";

            services
                .AddHttpClient<IEmbeddingClient, EmbeddingClient>(client =>
                {
                    client.BaseAddress = new Uri(embedBase);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                });

            services.AddScoped<ICaseEventPublisher,RabbitMqCaseEventPublisher>();
            services.AddScoped<ICaseRepository, MongoCaseRepository>();
            services.AddScoped<IMailService, MailClient>();

            //services.AddScoped<IMailService, GmailService>();


            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.SetKebabCaseEndpointNameFormatter();
                busConfigurator.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(new Uri(config["MessageBroker:Host"]!), h =>
                    {
                        h.Username(config["MessageBroker:Username"]);
                        h.Password(config["MessageBroker:Password"]);
                    });

                    configurator.ConfigureEndpoints(context);

                });
            });

            var host = config["MessageBroker:Host"];
            Console.WriteLine($"RabbitMQ Host from config: {host}");


            services.Configure<GmailOptions>(
                config.GetSection(GmailOptions.GmailOptionskey));

            services.AddSingleton<IMongoClient>(sp =>
                new MongoClient(config.GetConnectionString("Mongo")));
                  services.AddScoped(sp =>
                sp.GetRequiredService<IMongoClient>()
                  .GetDatabase(config["Mongo:DatabaseName"]));



            return services;
        }
    }
}
