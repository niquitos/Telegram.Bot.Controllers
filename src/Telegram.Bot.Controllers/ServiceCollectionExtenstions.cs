using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot.Polling;

namespace Telegram.Bot.Controllers;

public static class ServiceCollectionExtenstions
{
    public static IServiceCollection AddTelegramBotControllers(this IServiceCollection services, string token)
    {
        services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(token));
        services.AddSingleton<IUpdateHandler, MessageUpdateHandler>();
        services.AddSingleton<IHostedService, BotBackgroundService>();
        
        var assembly = Assembly.GetCallingAssembly(); 
        var controllerTypes = assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(TelegramBotControllerBase)) && !t.IsAbstract);

        foreach (var type in controllerTypes)
        {
            services.AddTransient(type);
            services.AddTransient(typeof(TelegramBotControllerBase), serviceProvider =>
            {
                return (TelegramBotControllerBase)serviceProvider.GetRequiredService(type);
            });
        }

        return services;
    }
}
