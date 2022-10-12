using Autofac;
using Autofac.Builder;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class AutofacastleRegistrationExtensions
    {
        /// <summary>
        /// Populates the Autofac container builder with the set of registered service descriptors
        /// and makes <see cref="IServiceProvider"/> and <see cref="IServiceScopeFactory"/>
        /// available in the container.
        /// </summary>
        /// <param name="builder">
        /// The <see cref="ContainerBuilder"/> into which the registrations should be made.
        /// </param>
        /// <param name="services">
        /// A container builder that can be used to create an <see cref="IServiceProvider" />.
        /// </param>
        public static void Populate(
            this ContainerBuilder builder,
            IServiceCollection services)
        {
            Populate(builder, services, null);
        }

        /// <summary>
        /// Populates the Autofac container builder with the set of registered service descriptors
        /// and makes <see cref="IServiceProvider"/> and <see cref="IServiceScopeFactory"/>
        /// available in the container. Using this overload is incompatible with the ASP.NET Core
        /// support for <see cref="IServiceProviderFactory{TContainerBuilder}"/>.
        /// </summary>
        /// <param name="builder">
        /// The <see cref="ContainerBuilder"/> into which the registrations should be made.
        /// </param>
        /// <param name="services">
        /// A container builder that can be used to create an <see cref="IServiceProvider" />.
        /// </param>
        /// <param name="lifetimeScopeTagForSingletons">
        /// If provided and not <see langword="null"/> then all registrations with lifetime <see cref="ServiceLifetime.Singleton" /> are registered
        /// using <see cref="IRegistrationBuilder{TLimit,TActivatorData,TRegistrationStyle}.InstancePerMatchingLifetimeScope" />
        /// with provided <paramref name="lifetimeScopeTagForSingletons"/>
        /// instead of using <see cref="IRegistrationBuilder{TLimit,TActivatorData,TRegistrationStyle}.SingleInstance"/>.
        /// </param>
        /// <remarks>
        /// <para>
        /// Specifying a <paramref name="lifetimeScopeTagForSingletons"/> addresses a specific case where you have
        /// an application that uses Autofac but where you need to isolate a set of services in a child scope. For example,
        /// if you have a large application that self-hosts ASP.NET Core items, you may want to isolate the ASP.NET
        /// Core registrations in a child lifetime scope so they don't show up for the rest of the application.
        /// This overload allows that. Note it is the developer's responsibility to execute this and create an
        /// <see cref="AutofacServiceProvider"/> using the child lifetime scope.
        /// </para>
        /// </remarks>
        public static void Populate(
            this ContainerBuilder builder,
            IServiceCollection services,
            object? lifetimeScopeTagForSingletons)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            builder.RegisterType<AutofacServiceProvider>().As<IServiceProvider>().ExternallyOwned();
            var autofacServiceScopeFactory = typeof(AutofacServiceProvider).Assembly.GetType("Autofac.Extensions.DependencyInjection.AutofacServiceScopeFactory");
            if (autofacServiceScopeFactory == null)
            {
                throw new Exception("Unable get type of Autofac.Extensions.DependencyInjection.AutofacServiceScopeFactory!");
            }
            builder.RegisterType(autofacServiceScopeFactory).As<IServiceScopeFactory>();

            Register(builder, services, lifetimeScopeTagForSingletons);
        }

        /// <summary>
        /// Configures the lifecycle on a service registration.
        /// </summary>
        /// <typeparam name="TActivatorData">The activator data type.</typeparam>
        /// <typeparam name="TRegistrationStyle">The object registration style.</typeparam>
        /// <param name="registrationBuilder">The registration being built.</param>
        /// <param name="lifecycleKind">The lifecycle specified on the service registration.</param>
        /// <param name="lifetimeScopeTagForSingleton">
        /// If not <see langword="null"/> then all registrations with lifetime <see cref="ServiceLifetime.Singleton" /> are registered
        /// using <see cref="IRegistrationBuilder{TLimit,TActivatorData,TRegistrationStyle}.InstancePerMatchingLifetimeScope" />
        /// with provided <paramref name="lifetimeScopeTagForSingleton"/>
        /// instead of using <see cref="IRegistrationBuilder{TLimit,TActivatorData,TRegistrationStyle}.SingleInstance"/>.
        /// </param>
        /// <returns>
        /// The <paramref name="registrationBuilder" />, configured with the proper lifetime scope,
        /// and available for additional configuration.
        /// <paramref name="AutoActivate"/>
        /// </returns>
        private static IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> ConfigureLifecycle<TActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> registrationBuilder,
            ServiceLifetime lifecycleKind,
            object? lifetimeScopeTagForSingleton,bool AutoActivate=false)
        {
            switch (lifecycleKind)
            {
                case ServiceLifetime.Singleton:
                    if (lifetimeScopeTagForSingleton == null)
                    {
                        registrationBuilder.SingleInstance();

                        if (AutoActivate)
                            registrationBuilder.AutoActivate();
                    }
                    else
                    {
                        registrationBuilder.InstancePerMatchingLifetimeScope(lifetimeScopeTagForSingleton);
                    }

                    break;
                case ServiceLifetime.Scoped:
                    registrationBuilder.InstancePerLifetimeScope();
                    break;
                case ServiceLifetime.Transient:
                    registrationBuilder.InstancePerDependency();
                    break;
            }

            return registrationBuilder;
        }

        /// <summary>
        /// 遍历IServiceCollection中的service并注册到Autofac的容器中
        /// Populates the Autofac container builder with the set of registered service descriptors.
        /// </summary>
        /// <param name="builder">
        /// The <see cref="ContainerBuilder"/> into which the registrations should be made.
        /// </param>
        /// <param name="services">
        /// A container builder that can be used to create an <see cref="IServiceProvider" />.
        /// </param>
        /// <param name="lifetimeScopeTagForSingletons">
        /// If not <see langword="null"/> then all registrations with lifetime <see cref="ServiceLifetime.Singleton" /> are registered
        /// using <see cref="IRegistrationBuilder{TLimit,TActivatorData,TRegistrationStyle}.InstancePerMatchingLifetimeScope" />
        /// with provided <paramref name="lifetimeScopeTagForSingletons"/>
        /// instead of using <see cref="IRegistrationBuilder{TLimit,TActivatorData,TRegistrationStyle}.SingleInstance"/>.
        /// </param>
        private static void Register(
            ContainerBuilder builder,
            IServiceCollection services,
            object? lifetimeScopeTagForSingletons)
        {
            //key value对调
            var keyedDicKvReverse = AutofacastleCollection.KeyedImplementationTypes.ToDictionary(t => t.Value, t => t.Key);
            var namedDicKvReverse = AutofacastleCollection.NamedImplementationTypes.ToDictionary(t => t.Value, t => t.Key);

            foreach (var descriptor in services)
            {
                if (descriptor.ImplementationType != null)//存在实现类
                {
                    
                    // Test if the an open generic type is being registered
                    var serviceTypeInfo = descriptor.ServiceType.GetTypeInfo();

                    if (descriptor.ImplementationType.IsGenericTypeDefinition)//动态泛型 实现类为动态泛型定义
                    {
                        //是泛型定义 并且 包含泛型参数 实现的为开放泛型才注入:xxx<> 实现的非开放泛型不能注入:xxx
                        //注意这里也需要判断是否为泛型 存在这样一种情况FullName为空且包含泛型参数 但是是非泛型定义 看似为泛型！！！
                        //目前未找到原因为何有这种情况？找到原因为批量注入的时候读取出的泛型接口FullName为空 需要补充完整FullName引用类型即可
                        if (serviceTypeInfo.IsGenericTypeDefinition && serviceTypeInfo.ContainsGenericParameters)
                        {

                            var registrationBuilder = builder
                                .RegisterGeneric(descriptor.ImplementationType)
                                .As(descriptor.ServiceType).AsSelf().Keyed(descriptor.ImplementationType, descriptor.ServiceType);
                            if (descriptor.ImplementationType.FullName != null)
                                registrationBuilder.Named(descriptor.ImplementationType.FullName, descriptor.ServiceType);

                            if (keyedDicKvReverse.ContainsKey(descriptor.ImplementationType))
                            {
                                registrationBuilder.Keyed(keyedDicKvReverse[descriptor.ImplementationType], descriptor.ServiceType);
                            }

                            if (namedDicKvReverse.ContainsKey(descriptor.ImplementationType))
                            {
                                registrationBuilder.Named(namedDicKvReverse[descriptor.ImplementationType], descriptor.ServiceType);
                            }

                            registrationBuilder.ConfigureLifecycle(descriptor.Lifetime, lifetimeScopeTagForSingletons)
                                .AddIntercept(descriptor.ImplementationType);
                        }

                    }
                    else
                    {

                        var registrationBuilder = builder
                            .RegisterType(descriptor.ImplementationType).AsSelf()
                            .As(descriptor.ServiceType).Keyed(descriptor.ImplementationType, descriptor.ServiceType);
                        if (descriptor.ImplementationType.FullName != null)
                            registrationBuilder.Named(descriptor.ImplementationType.FullName, descriptor.ServiceType);

                        if (keyedDicKvReverse.ContainsKey(descriptor.ImplementationType))
                        {
                            registrationBuilder.Keyed(keyedDicKvReverse[descriptor.ImplementationType], descriptor.ServiceType);
                        }

                        if (namedDicKvReverse.ContainsKey(descriptor.ImplementationType))
                        {
                            registrationBuilder.Named(namedDicKvReverse[descriptor.ImplementationType], descriptor.ServiceType);
                        }

                        if (AutofacastleCollection.AutoActives.Any(t => t == descriptor.ImplementationType))
                            registrationBuilder.AutoActivate();//预加载

                        registrationBuilder.ConfigureLifecycle(descriptor.Lifetime, lifetimeScopeTagForSingletons,AutofacastleCollection.AutoActives.Any(t=>t==descriptor.ImplementationType))
                            .AddIntercept(descriptor.ImplementationType);

                    }
                }
                else if (descriptor.ImplementationFactory != null)
                {

                    var registration = RegistrationBuilder.ForDelegate(descriptor.ServiceType, (context, parameters) =>
                    {
                        var serviceProvider = context.Resolve<IServiceProvider>();
                        return descriptor.ImplementationFactory(serviceProvider);
                    })
                        .ConfigureLifecycle(descriptor.Lifetime, lifetimeScopeTagForSingletons)
                        .CreateRegistration();

                    builder.RegisterComponent(registration);
                }
                else
                {
                    
                    if (descriptor.ImplementationInstance == null)
                        return;

                    var registrationBuilder = builder
                        .RegisterInstance(descriptor.ImplementationInstance)
                        .As(descriptor.ServiceType).Keyed(descriptor.ImplementationInstance.GetType(), descriptor.ServiceType);
                    var implementationInstanceFullName = descriptor.ImplementationInstance.GetType().FullName;
                    if (implementationInstanceFullName != null)
                        registrationBuilder.Named(implementationInstanceFullName, descriptor.ServiceType);

                    if (keyedDicKvReverse.ContainsKey(descriptor.ImplementationInstance.GetType()))
                    {
                        registrationBuilder.Keyed(keyedDicKvReverse[descriptor.ImplementationInstance.GetType()], descriptor.ServiceType);
                    }

                    if (namedDicKvReverse.ContainsKey(descriptor.ImplementationInstance.GetType()))
                    {
                        registrationBuilder.Named(namedDicKvReverse[descriptor.ImplementationInstance.GetType()], descriptor.ServiceType);
                    }

                    registrationBuilder.ConfigureLifecycle(descriptor.Lifetime, null);
                }
            }
        }
    }
}
