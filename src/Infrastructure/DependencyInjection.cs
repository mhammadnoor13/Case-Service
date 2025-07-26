using Application.Interfaces;
using Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            //services.AddScoped<IMailService, GmailService>();


            return services;
        }
    }
}
