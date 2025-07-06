using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot.Polling;

namespace Telegram.Bot.Controllers;

public static class ServiceCollectionExtenstions
{
    public static IServiceCollection AddTelegramBotControllers(this IServiceCollection services, string token, Action<BotConfiguration>? configAction = null)
    {
        services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(token));
        services.AddSingleton<IUpdateHandler, MessageUpdateHandler>();
        services.AddSingleton<IHostedService, BotBackgroundService>();
        services.AddSingleton<IMessageService, TelegramMessageService>();
        services.AddSingleton<ICommandPathManager, InMemoryCommandPathManager>();

        var assembly = Assembly.GetCallingAssembly();
        var controllerTypes = assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(TelegramBotControllerBase)) && !t.IsAbstract);

        var commandRegistry = new CommandRegistry();
        services.AddSingleton(_ => commandRegistry);

        foreach (var type in controllerTypes)
        {
            services.AddTransient(type);
            services.AddTransient(typeof(TelegramBotControllerBase), serviceProvider =>
            {
                return (TelegramBotControllerBase)serviceProvider.GetRequiredService(type);
            });

            var classAttr = type.GetCustomAttribute<CommandAttribute>();
            var basePath = classAttr?.Path ?? string.Empty;

            var methods = type.GetMethods().Where(x => x.GetCustomAttributes<CommandAttribute>().Any());

            foreach (var method in methods)
            {
                var methodAttrs = method.GetCustomAttributes<CommandAttribute>();

                foreach (var methodAttr in methodAttrs)
                {
                    if (string.IsNullOrEmpty(methodAttr.Path))
                    {
                        if (!string.IsNullOrEmpty(basePath))
                        {
                            commandRegistry.Commands[basePath] = new CommandAttributeDetatails(methodAttr, type, method.Name);
                        }
                        continue;
                    }

                    var fullPath = methodAttr.Override
                        ? methodAttr.Path
                        : string.IsNullOrEmpty(basePath)
                            ? methodAttr.Path
                            : $"{basePath}/{methodAttr.Path}";

                    if (!string.IsNullOrEmpty(fullPath))
                    {
                        commandRegistry.Commands[fullPath] = new CommandAttributeDetatails(methodAttr, type, method.Name);
                    }
                }
            }
        }

        if (configAction is not null)
            services.Configure(configAction);

        return services;
    }
}

