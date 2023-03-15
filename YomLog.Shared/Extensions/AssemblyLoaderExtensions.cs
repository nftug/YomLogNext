// Reference: https://qiita.com/jun1s/items/4cf712d92151658f6250

using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace YomLog.Shared.Extensions;

public static class AssemblyLoaderExtensions
{
    public static IEnumerable<Assembly> GetByName(this IEnumerable<Assembly> asmList, string endsWith)
        => asmList.Where(asm => asm.GetName().Name!.EndsWith(endsWith)).ToList();

    public static IEnumerable<Assembly> CollectReferencedAssemblies(
        this Assembly asm,
        IEnumerable<string> targetAssemblyNames
    )
        => new HashSet<Assembly>(
            asm.GetReferencedAssemblies()
                .Where(a => targetAssemblyNames.Contains(a.Name))
                .SelectMany(
                    a => Assembly.Load(a).CollectReferencedAssemblies(targetAssemblyNames))
                .Append(asm)
        );
}

public static class ServiceCollectionAssemblyExtensions
{
    public static IServiceCollection AddByAttribute(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        var attributes = new Type[]
        {
            typeof(InjectAsTransientAttribute),
            typeof(InjectAsScopedAttribute),
            typeof(InjectAsSingletonAttribute)
        };
        var types = assemblies
            .SelectMany((Assembly asm) => asm.GetExportedTypes())
            .Where(t => !t.IsInterface && !t.IsAbstract);

        var typeGroup =
            attributes.GroupJoin(
                types,
                o => o,
                i => i.CustomAttributes
                    .Select(x => x.AttributeType)
                    .FirstOrDefault(at => at.Name.StartsWith("InjectAs")),
                (o, i) => new
                {
                    Types = i,
                    Lifetime = o.Name switch
                    {
                        nameof(InjectAsTransientAttribute) => ServiceLifetime.Transient,
                        nameof(InjectAsScopedAttribute) => ServiceLifetime.Scoped,
                        _ => ServiceLifetime.Singleton
                    }
                }
            )
            .SelectMany(x => x.Types, (x, t) => new { Type = t, x.Lifetime });

        foreach (var item in typeGroup)
        {
            // 既に登録されている型を除外
            if (services.Any(x => x.ServiceType == item.Type)) continue;

            // 対象のクラスが実装している全てのインタフェースに対してこの型をサービス登録する
            services.AddAsImplementedInterfaces(item.Type, item.Lifetime);

            // 自分自身の型に対してもサービス登録する
            services.Add(new ServiceDescriptor(item.Type, item.Type, item.Lifetime));
            DebugOutput(item.Type, item.Type, item.Lifetime);
        }

        return services;
    }

    public static IServiceCollection AddByAttribute(this IServiceCollection services)
        => AddByAttribute(services, new[] { Assembly.GetCallingAssembly() });

    private static void AddAsImplementedInterfaces(this IServiceCollection services, Type type, ServiceLifetime lifetime)
    {
        var interfaces = type.GetInterfaces()
            .Where(x => x != typeof(IDisposable) && x != typeof(INotifyPropertyChanged));

        foreach (Type t in interfaces)
        {
            services.Add(new ServiceDescriptor(t, type, lifetime));
            DebugOutput(type, t, lifetime);
        }
    }

    private static void DebugOutput(Type iType, Type type, ServiceLifetime lifetime)
        => Debug.WriteLine($"Add{lifetime}<{iType.Name}, {type.Name}>");
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class InjectAsTransientAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class InjectAsScopedAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class InjectAsSingletonAttribute : Attribute
{
}
