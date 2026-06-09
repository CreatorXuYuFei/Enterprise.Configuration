using Enterprise.Configuration.Abstractions;
using Enterprise.Configuration.Exceptions;
using Enterprise.Configuration.Utils;
using Microsoft.Extensions.DependencyInjection;
using static Enterprise.Configuration.Abstractions.OptionsInterfaces;

namespace Enterprise.Configuration.DI
{
    public static class ConfigDiExtensions
    {
        /// <summary>注入配置根到 DI 容器</summary>
        public static IServiceCollection AddEnterpriseConfig(this IServiceCollection services, IConfigRoot configRoot)
        {
            services.AddSingleton(configRoot);
            return services;
        }

        /// <summary>绑定配置实体，注册全套 Options 服务</summary>
        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, string sectionPath)
            where TOptions : class, new()
        {
            services.AddSingleton<IOptions<TOptions>>(sp =>
            {
                var root = sp.GetRequiredService<IConfigRoot>();
                var model = root.GetSection(sectionPath).BindModel<TOptions>();
                ModelValidator.Validate(model);
                return new OptionsWrapper<TOptions>(model);
            });

            services.AddSingleton<IOptionsMonitor<TOptions>>(sp =>
            {
                var root = sp.GetRequiredService<IConfigRoot>();
                return new OptionsMonitor<TOptions>(root, sectionPath);
            });

            return services;
        }
    }
}
