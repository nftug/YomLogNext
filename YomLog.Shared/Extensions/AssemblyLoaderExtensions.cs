// Reference: https://qiita.com/jun1s/items/4cf712d92151658f6250

using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using YomLog.Shared.Attributes;

namespace YomLog.Shared.Extensions;

public static class ServiceCollectionAssemblyExtensions
{
    public static IServiceCollection AddFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetExportedTypes().Where(t => !t.IsInterface && !t.IsAbstract);

        foreach (var type in types)
        {
            // 属性からLifetimeを決定
            ServiceLifetime? _lifetime =
                type.IsDefined(typeof(InjectAsTransientAttribute))
                ? ServiceLifetime.Transient
                : type.IsDefined(typeof(InjectAsScopedAttribute))
                ? ServiceLifetime.Scoped
                : type.IsDefined(typeof(InjectAsSingletonAttribute))
                ? ServiceLifetime.Singleton
                : null;
            if (_lifetime is not ServiceLifetime lifetime) continue;

            // 対象のクラスが実装している全てのインターフェース及び基底クラスに対してこの型をサービス登録する
            services.AddAsImplementedTypes(type, lifetime);

            // 自分自身の型に対してもサービス登録する
            services.Add(new ServiceDescriptor(type, type, lifetime));
            DebugOutput(type, type, lifetime);
        }

        return services;
    }

    public static IServiceCollection AddFromCurrentAssembly(this IServiceCollection services)
        => AddFromAssembly(services, Assembly.GetCallingAssembly());

    private static void AddAsImplementedTypes(this IServiceCollection services, Type type, ServiceLifetime lifetime)
    {
        var interfaces = type.GetInterfaces()
            .Where(x => x != typeof(IDisposable) && x != typeof(INotifyPropertyChanged));

        foreach (Type t in interfaces)
        {
            services.Add(new ServiceDescriptor(t, type, lifetime));
            DebugOutput(type, t, lifetime);
        }

        if (type.BaseType is Type baseType && baseType != typeof(object))
        {
            services.Add(new ServiceDescriptor(type.BaseType, type, lifetime));
            DebugOutput(type, type.BaseType, lifetime);
        }
    }

    private static void DebugOutput(Type iType, Type type, ServiceLifetime lifetime)
        => Debug.WriteLine($"Add{lifetime}<{iType.Name}, {type.Name}>");
}
