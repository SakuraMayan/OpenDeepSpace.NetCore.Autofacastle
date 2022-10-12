using Autofac;
using Microsoft.Extensions.DependencyInjection;
using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection;
using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.Attributes;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenDeepSpace.NetCore.Autofacastle.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint;

namespace OpenDeepSpace.NetCore.Autofacastle.Extensions
{
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// 批量注入
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies">程序集</param>
        /// <returns></returns>
        public static IServiceCollection BatchInjection(this ServiceCollection services, List<Assembly> assemblies)
        {
            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            var types = assemblies.SelectMany(t => t.GetTypes());

            BatchInjectionInternal(services, types);

            return services;
        }

        /// <summary>
        /// 批量注入
        /// </summary>
        /// <param name="services"></param>
        /// <param name="types">类型集合</param>
        /// <returns></returns>
        public static IServiceCollection BatchInjection(this IServiceCollection services, List<Type> types)
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));

            BatchInjectionInternal(services, types);

            return services;
        }


        /// <summary>
        /// 批量注入
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection BatchInjection(this IServiceCollection services)
        {
            //获取类型
            var types = TypeFinder.GetExcludeSystemNugetAllTypes();
            BatchInjectionInternal(services, types);

            return services;
        }

        private static void BatchInjectionInternal(IServiceCollection services, IEnumerable<Type> types)
        {
            types=types.Where(t => t.IsClass && !t.IsAbstract);

            foreach (var type in types)
            {
                //瞬时
                if (type.IsAssignableTo<ITransient>() || type.GetCustomAttribute<TransientAttribute>() != null)
                {
                    var transientAttr = type.GetCustomAttribute<TransientAttribute>();
                    //如果指定了Keyed或Named尝试添加
                    type.TryAddExistSameKeyedOrNamed(transientAttr);

                    //注入自身
                    services.AddTransient(type);
                    if (transientAttr != null && transientAttr.AsServices != null && transientAttr.AsServices.Any())
                    {//指定服务注入
                        foreach (var service in transientAttr.AsServices)
                        {
                            //如果直接获取的出来Ixxx<> FullName为空 
                            //针对泛型 FullName为空导致为非泛型 补充完整FullName 才能正确批量注入动态泛型
                            //例如services.AddTransient(typeof(Ixxx<>),typeof(xxx()))
                            var fixService = service.FixTypeReference();
                            services.AddTransient(fixService, type);
                        }
                    }
                    else
                    {//未指定服务注入 查找实现的相关接口注入 
                        foreach (var service in type.GetInterfaces().FilterDependencyInjectionInterfaces())
                        {
                            var fixService = service.FixTypeReference();
                            services.AddTransient(fixService, type);
                        }
                    }

                }

                //一次请求
                if (type.IsAssignableTo<IScoped>() || type.GetCustomAttribute<ScopedAttribute>() != null)
                {
                    var scopedAttr = type.GetCustomAttribute<ScopedAttribute>();
                    //如果指定了Keyed或Named尝试添加
                    type.TryAddExistSameKeyedOrNamed(scopedAttr);
                    //注入自身
                    services.AddScoped(type);

                    if (scopedAttr != null && scopedAttr.AsServices != null && scopedAttr.AsServices.Any())
                    {//指定服务注入
                        foreach (var service in scopedAttr.AsServices)
                        {
                            var fixService = service.FixTypeReference();
                            services.AddScoped(fixService, type);
                        }
                    }
                    else
                    {//未指定服务注入 查找实现的相关接口注入 
                        foreach (var service in type.GetInterfaces().FilterDependencyInjectionInterfaces())
                        {
                            var fixService = service.FixTypeReference();
                            services.AddScoped(fixService, type);
                        }
                    }

                }

                //单例
                if (type.IsAssignableTo<ISingleton>() || type.GetCustomAttribute<SingletonAttribute>() != null)
                {
                    var singletonAttr = type.GetCustomAttribute<SingletonAttribute>();
                    //如果指定了Keyed或Named尝试添加
                    type.TryAddExistSameKeyedOrNamed(singletonAttr);
                    //注入自身
                    services.AddSingleton(type);

                    if (singletonAttr != null && singletonAttr.AsServices != null && singletonAttr.AsServices.Any())
                    {//指定服务注入
                        foreach (var service in singletonAttr.AsServices)
                        {
                            var fixService = service.FixTypeReference();
                            services.AddSingleton(fixService, type);
                        }
                        //是否预加载 且不能是动态泛型
                        if (singletonAttr.AutoActivate && !type.IsGenericTypeDefinition)
                            AutofacastleCollection.AutoActives.Add(type);
                    }
                    else
                    {//未指定服务注入 查找实现的相关接口注入 
                        foreach (var service in type.GetInterfaces().FilterDependencyInjectionInterfaces())
                        {
                            var fixService = service.FixTypeReference();

                            services.AddSingleton(fixService, type);
                        }
                    }

                }
            }
        }

        /// <summary>
        /// 替换服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IServiceCollection ReplaceServices(this IServiceCollection services, List<Assembly> assemblies)
        {
            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            var types = assemblies.SelectMany(t => t.GetTypes());

            ReplaceServicesInternal(services, types);

            return services;
        }

        /// <summary>
        /// 替换服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static IServiceCollection ReplaceServices(this ServiceCollection services, List<Type> types)
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));

            ReplaceServicesInternal(services, types);

            return services;
        }


        /// <summary>
        /// 服务替换
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection ReplaceServices(this IServiceCollection services)
        {
            //获取类型
            var types = TypeFinder.GetExcludeSystemNugetAllTypes();

            ReplaceServicesInternal(services, types);

            return services;
        }


        /// <summary>
        /// 注入拦截点 
        /// 注入拦截前之前先调用收集拦截点
        /// <see cref="InterceptExtensions.CollectionInterceptPoint"/>
        /// 或
        /// <see cref="InterceptExtensions.CollectionInterceptPoint(List{Assembly})"/>
        /// 或
        /// <see cref="InterceptExtensions.CollectionInterceptPoint(List{Type})"/>
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection InjectionInterceptPoints(this IServiceCollection services)
        {
            //把收集到的拦截点注入到容器
            foreach (var interceptPointInfo in InterceptPointCollection.interceptPointInfos)
            {
                services.AddTransient(interceptPointInfo.InterceptPointType);
            }

            return services;
        }

        ///// <summary>
        ///// 尝试注入被拦截的类型
        ///// 使用该方法之前请先调用
        ///// <see cref="InterceptExtensions.CollectionInterceptedTypeInfo"/>
        ///// 或
        ///// <see cref="InterceptExtensions.CollectionInterceptedTypeInfo(List{Assembly})"/>
        ///// 或
        ///// <see cref="InterceptExtensions.CollectionInterceptedTypeInfo(List{Type})"/>
        ///// </summary>
        ///// <param name="services"></param>
        ///// <returns></returns>
        //public static IServiceCollection TryInjectionInterceptedType(this IServiceCollection services)
        //{

        //    var serviceProvider = services.BuildServiceProvider();

        //    foreach (var interceptedTypeInfo in AutofacastleCollection.InterceptedTypeInfos)
        //    { 
        //        var interceptedType = interceptedTypeInfo.InterceptedType;

        //        //如果从容器中获取不到 都以Transient的方式注入
        //        var selfInstance = serviceProvider.GetService(interceptedType);
        //    }

        //    return services;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="types"></param>
        private static void ReplaceServicesInternal(IServiceCollection services,IEnumerable<Type> types) 
        {
            types=types.Where(t => t.IsClass && !t.IsAbstract);

            foreach (var type in types)
            {
                var transientAttr = type.GetCustomAttribute<TransientAttribute>();
                var scopedAttr = type.GetCustomAttribute<ScopedAttribute>();
                var singletonAttr = type.GetCustomAttribute<SingletonAttribute>();

                //瞬时 替换服务
                if (transientAttr != null && transientAttr.ReplaceServices != null && transientAttr.ReplaceServices.Any())
                {
                    foreach (var replaceService in transientAttr.ReplaceServices)
                    {

                        services.ReplaceService(replaceService, type, ServiceLifetime.Transient);

                    }
                }

                //一次请求 替换服务
                if (scopedAttr != null && scopedAttr.ReplaceServices != null && scopedAttr.ReplaceServices.Any())
                {
                    foreach (var replaceService in scopedAttr.ReplaceServices)
                    {
                        services.ReplaceService(replaceService, type, ServiceLifetime.Scoped);

                    }
                }

                //单例 替换服务
                if (singletonAttr != null && singletonAttr.ReplaceServices != null && singletonAttr.ReplaceServices.Any())
                {
                    foreach (var replaceService in singletonAttr.ReplaceServices)
                    {

                        services.ReplaceService(replaceService, type, ServiceLifetime.Singleton);

                    }
                }


            }
        }


        /// <summary>
        /// 替换服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public static IServiceCollection ReplaceService(this IServiceCollection services,Type serviceType,Type implementationType, ServiceLifetime lifetime)
        {
            ServiceDescriptor descriptor = new(serviceType, implementationType, lifetime);
            services.Replace(descriptor);
            return services;
        }
    }
}
