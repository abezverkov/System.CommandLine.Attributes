using System;
using System.Collections.Generic;
using System.CommandLine.Binding;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace System.CommandLine.Attributes
{
    public static class CommandLineBuilderExtensions
    {
        public static IServiceCollection AddConfiguration(this IServiceCollection services, Action<ConfigurationBuilder> configBuilder)
        {
            var cb = new ConfigurationBuilder();
            configBuilder(cb);
            var configRoot = cb.Build();

            return services
                .AddSingleton(configRoot)
                .AddSingleton<IConfiguration>(configRoot);
        }

        public static CommandLineBuilder UseBindingContextInjection(this CommandLineBuilder builder, Action<BindingContext> bindingContext)
        {
            builder.UseMiddleware(ctx => bindingContext(ctx.BindingContext));
            return builder;
        }

        public static CommandLineBuilder UseDependencyInjection(this CommandLineBuilder builder, Action<IServiceCollection> servicesAction)
        {
            var services = new ServiceCollection();
            servicesAction(services);
            builder.UseDependencyInjection(services);
            return builder;
        }

        public static CommandLineBuilder UseDependencyInjection(this CommandLineBuilder builder, IServiceCollection services)
        {
            var provider = services
                .WireConcrete()
                .BuildServiceProvider();
                
            builder.UseMiddleware(ctx =>
            {
                foreach (var sd in services)
                {
                    if (sd.ImplementationFactory != null)
                        ctx.BindingContext.AddService(sd.ServiceType, () => sd.ImplementationFactory(provider));
                    else if (sd.ImplementationInstance != null)
                        ctx.BindingContext.AddService(sd.ServiceType, () => sd.ImplementationInstance);
                    else if (sd.ImplementationType != null)
                        ctx.BindingContext.AddService(sd.ServiceType, () => provider.GetRequiredService(sd.ImplementationType));
                }
            });
            return builder;
        }

        internal static IServiceCollection WireConcrete(this IServiceCollection services)
        {
            foreach(var sd in services.Where(x => x.ImplementationType != null).ToList())
            { 
                if (!services.Any(x => x.ServiceType == sd.ImplementationType))
                {
                    services.AddTransient(sd.ImplementationType);
                }
            }
            return services;
        }

    }
}
