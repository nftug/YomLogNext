// Reference: https://qiita.com/jun1s/items/4cf712d92151658f6250

using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace YomLog.Shared.Extensions;

public static class AssemblyLoaderExtensions
{
    public static IEnumerable<Assembly> GetByName(this IEnumerable<Assembly> asmList, string endsWith)
        => asmList.Where(asm => asm.GetName().Name!.EndsWith(endsWith)).ToList();

    public static IEnumerable<Assembly> CollectReferencedAssemblies
        (this Assembly asm, IEnumerable<string> targetAssemblyNames)
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
    public static IServiceCollection AddAssemblyTypes(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        ServiceLifetime lifetime,
        Func<Type, bool> conditions
    )
    {
        var types = assemblies
            .SelectMany((Assembly asm) => asm.GetExportedTypes())
            .Where(t => !t.IsInterface            // インタフェースではない
                        && !t.IsAbstract          // 抽象型ではない
                        && conditions(t))         // 与えた条件に合致する
            .ToList();

        foreach (Type type in types)
        {
            // 既に登録されている型を除外
            if (services.Any(x => x.ServiceType == type)) continue;

            // 対象のクラスが実装している全てのインタフェースに対してこの型をサービス登録する
            services.AddAsImplementedInterfaces(type, lifetime);

            // 自分自身の型に対してもサービス登録する
            services.Add(new ServiceDescriptor(type, type, lifetime));
            DebugOutput(type, type, lifetime);
        }

        return services;
    }

    public static IServiceCollection AddAssemblyTypes<TType>
        (this IServiceCollection services, IEnumerable<Assembly> assemblies, ServiceLifetime lifetime)
    {
        static bool conditions(Type t) => typeof(TType).IsAssignableFrom(t);
        return services.AddAssemblyTypes(assemblies, lifetime, conditions);
    }

    public static IServiceCollection AddAssemblyTypes(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        ServiceLifetime lifetime,
        string classNameEndsWith
    )
    {
        bool conditions(Type t) => t.Name.EndsWith(classNameEndsWith) && t.Name != classNameEndsWith;
        return services.AddAssemblyTypes(assemblies, lifetime, conditions);
    }

    private static void AddAsImplementedInterfaces
        (this IServiceCollection services, Type type, ServiceLifetime lifetime)
    {
        var interfaces = type.GetInterfaces().Where(x => x != typeof(IDisposable));

        foreach (Type t in type.GetInterfaces())
        {
            services.Add(new ServiceDescriptor(t, type, lifetime));
            DebugOutput(type, t, lifetime);
        }
    }

    private static void DebugOutput(Type iType, Type type, ServiceLifetime lifetime)
        => Debug.WriteLine($"Add{lifetime}<{iType.Name}, {type.Name}>");
}
