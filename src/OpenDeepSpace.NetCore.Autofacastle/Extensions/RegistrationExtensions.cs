using Autofac.Builder;
using Autofac.Core;
using Autofac.Core.Resolving.Pipeline;
using Autofac.Features.Scanning;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Autofac.Extras.DynamicProxy
{
    /// <summary>
    /// Adds registration syntax to the <see cref="ContainerBuilder"/> type.
    /// 重写 Autofac.Extras.DynamicProxy RegistrationExtensions
    /// </summary>
    public static class RegistrationExtensions
    {
        private const string InterceptorsPropertyName = "Autofac.Extras.DynamicProxy.RegistrationExtensions.InterceptorsPropertyName";
        private const string InterceptorsForGenericMethodCache = "Autofac.Extras.DynamicProxy.RegistrationExtensions.InterceptorsForGenericMethodCache";

        private const string AttributeInterceptorsPropertyName = "Autofac.Extras.DynamicProxy.RegistrationExtensions.AttributeInterceptorsPropertyName";

        private static readonly IEnumerable<Service> EmptyServices = Enumerable.Empty<Service>();

        private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();

        private static readonly object InterceptorsForGenericMethodCacheLock = new object();

        private static readonly object lockobj=new object();

        /// <summary>
        /// Enable class interception on the target type. Interceptors will be determined
        /// via Intercept attributes on the class or added with InterceptedBy().
        /// Only virtual methods can be intercepted this way.
        /// </summary>
        /// <typeparam name="TLimit">Registration limit type.</typeparam>
        /// <typeparam name="TConcreteReflectionActivatorData">Activator data type.</typeparam>
        /// <typeparam name="TRegistrationStyle">Registration style.</typeparam>
        /// <param name="registration">Registration to apply interception to.</param>
        /// <returns>Registration builder allowing the registration to be configured.</returns>
        public static (bool, IRegistrationBuilder<TLimit, TConcreteReflectionActivatorData, TRegistrationStyle>) TryEnableClassInterceptors<TLimit,
            TConcreteReflectionActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TConcreteReflectionActivatorData, TRegistrationStyle> registration)
            where TConcreteReflectionActivatorData : ReflectionActivatorData
        {
            return EnableClassInterceptors(registration, ProxyGenerationOptions.Default);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLimit"></typeparam>
        /// <typeparam name="TConcreteReflectionActivatorData"></typeparam>
        /// <typeparam name="TRegistrationStyle"></typeparam>
        /// <param name="registration"></param>
        /// <param name="additionalInterfaces"></param>
        /// <returns></returns>
        public static (bool, IRegistrationBuilder<TLimit, TConcreteReflectionActivatorData, TRegistrationStyle>) EnableClassInterceptors<TLimit,
            TConcreteReflectionActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TConcreteReflectionActivatorData, TRegistrationStyle> registration, params Type[] additionalInterfaces)
            where TConcreteReflectionActivatorData : ReflectionActivatorData
        {
            return EnableClassInterceptors(registration, ProxyGenerationOptions.Default, additionalInterfaces);
        }

        /// <summary>
        /// Enable class interception on the target type. Interceptors will be determined
        /// via Intercept attributes on the class or added with InterceptedBy().
        /// Only virtual methods can be intercepted this way.
        /// </summary>
        /// <typeparam name="TLimit">Registration limit type.</typeparam>
        /// <typeparam name="TRegistrationStyle">Registration style.</typeparam>
        /// <param name="registration">Registration to apply interception to.</param>
        /// <param name="options">Proxy generation options to apply.</param>
        /// <param name="additionalInterfaces">Additional interface types. Calls to their members will be proxied as well.</param>
        /// <returns>Registration builder allowing the registration to be configured.</returns>
        public static IRegistrationBuilder<TLimit, ScanningActivatorData, TRegistrationStyle> EnableClassInterceptors<TLimit, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, ScanningActivatorData, TRegistrationStyle> registration,
            ProxyGenerationOptions options,
            params Type[] additionalInterfaces)
        {
            if (registration == null)
            {
                throw new ArgumentNullException(nameof(registration));
            }

            registration.ActivatorData.ConfigurationActions.Add((t, rb) => rb.EnableClassInterceptors(options, additionalInterfaces));
            return registration;
        }

        /// <summary>
        /// Enable class interception on the target type. Interceptors will be determined
        /// via Intercept attributes on the class or added with InterceptedBy().
        /// Only virtual methods can be intercepted this way.
        /// </summary>
        /// <typeparam name="TLimit">Registration limit type.</typeparam>
        /// <typeparam name="TConcreteReflectionActivatorData">Activator data type.</typeparam>
        /// <typeparam name="TRegistrationStyle">Registration style.</typeparam>
        /// <param name="registration">Registration to apply interception to.</param>
        /// <param name="options">Proxy generation options to apply.</param>
        /// <param name="additionalInterfaces">Additional interface types. Calls to their members will be proxied as well.</param>
        /// <returns>Registration builder allowing the registration to be configured.</returns>
        public static (bool, IRegistrationBuilder<TLimit, TConcreteReflectionActivatorData, TRegistrationStyle>) EnableClassInterceptors<TLimit,
            TConcreteReflectionActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TConcreteReflectionActivatorData, TRegistrationStyle> registration,
            ProxyGenerationOptions options,
            params Type[] additionalInterfaces)
            where TConcreteReflectionActivatorData : ReflectionActivatorData
        {

            bool EnableProxy = false;//能否代理

            try
            {

                if (registration == null)
                {
                    throw new ArgumentNullException(nameof(registration));
                }

                if (registration.ActivatorData.ImplementationType.IsGenericTypeDefinition)
                {
                    registration.ConfigurePipeline(p => p.Use(PipelinePhase.Activation, MiddlewareInsertionMode.StartOfPhase, (ctxt, next) =>
                    {
                        next(ctxt);

                        registInterceptorsForGenericMethodCache(ctxt);

                        var interceptors = GetInterceptorServices(ctxt.Registration, ctxt.Instance.GetType())
                            .Select(ctxt.ResolveService)
                            .Cast<IInterceptor>()
                            .ToArray();

                        //这里需要改一下
                        //https://github.com/JSkimming/Castle.Core.AsyncInterceptor/blob/master/src/Castle.Core.AsyncInterceptor/ProxyGeneratorExtensions.cs
                        ctxt.Instance = options == null
                            ? ProxyGenerator.CreateClassProxyWithTarget(ctxt.Instance.GetType(), ctxt.Instance, interceptors)
                            : ProxyGenerator.CreateClassProxyWithTarget(ctxt.Instance.GetType(), ctxt.Instance, options, interceptors);
                    }));

                    EnableProxy = true;

                    return (EnableProxy, registration);
                }

                registration.ActivatorData.ImplementationType =
                    ProxyGenerator.ProxyBuilder.CreateClassProxyType(
                        registration.ActivatorData.ImplementationType,
                        additionalInterfaces ?? Type.EmptyTypes,
                        options);

                var interceptorServices = new List<Service>();
                AddInterceptorServicesToMetadata(registration, interceptorServices, AttributeInterceptorsPropertyName);

                registration.OnPreparing(e =>
                {
                    var proxyParameters = new List<Parameter>();
                    int index = 0;

                    if (options.HasMixins)
                    {
                        foreach (var mixin in options.MixinData.Mixins)
                        {
                            proxyParameters.Add(new PositionalParameter(index++, mixin));
                        }
                    }

                    proxyParameters.Add(new PositionalParameter(index++, GetInterceptorServices(e.Component, registration.ActivatorData.ImplementationType)
                        .Select(s => e.Context.ResolveService(s))
                        .Cast<IInterceptor>()
                        .ToArray()));

                    if (options.Selector != null)
                    {
                        proxyParameters.Add(new PositionalParameter(index, options.Selector));
                    }

                    e.Parameters = proxyParameters.Concat(e.Parameters).ToArray();
                });

                EnableProxy = true;
            }
            catch (Exception ex)
            {
                EnableProxy = false;
            }
            finally
            {

            }
            return (EnableProxy, registration);

        }



        /// <summary>
        /// Enable interface interception on the target type. Interceptors will be determined
        /// via Intercept attributes on the class or interface, or added with InterceptedBy() calls.
        /// </summary>
        /// <typeparam name="TLimit">Registration limit type.</typeparam>
        /// <typeparam name="TActivatorData">Activator data type.</typeparam>
        /// <typeparam name="TSingleRegistrationStyle">Registration style.</typeparam>
        /// <param name="registration">Registration to apply interception to.</param>
        /// <returns>Registration builder allowing the registration to be configured.</returns>
        public static (bool, IRegistrationBuilder<TLimit, TActivatorData, TSingleRegistrationStyle>) TryEnableInterfaceInterceptors<TLimit, TActivatorData,
            TSingleRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TActivatorData, TSingleRegistrationStyle> registration)
        {
            return EnableInterfaceInterceptors(registration, null);
        }

        /// <summary>
        /// Enable interface interception on the target type. Interceptors will be determined
        /// via Intercept attributes on the class or interface, or added with InterceptedBy() calls.
        /// </summary>
        /// <typeparam name="TLimit">Registration limit type.</typeparam>
        /// <typeparam name="TActivatorData">Activator data type.</typeparam>
        /// <typeparam name="TSingleRegistrationStyle">Registration style.</typeparam>
        /// <param name="registration">Registration to apply interception to.</param>
        /// <param name="options">Proxy generation options to apply.</param>
        /// <returns>Registration builder allowing the registration to be configured.</returns>
        public static (bool, IRegistrationBuilder<TLimit, TActivatorData, TSingleRegistrationStyle>) EnableInterfaceInterceptors<TLimit, TActivatorData,
            TSingleRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TActivatorData, TSingleRegistrationStyle> registration, ProxyGenerationOptions options)
        {

            bool EnableProxy = false;//能否被代理

            try
            {

                if (registration == null)
                {
                    throw new ArgumentNullException(nameof(registration));
                }

                registration.ConfigurePipeline(p => p.Use(PipelinePhase.Activation, MiddlewareInsertionMode.StartOfPhase, (ctxt, next) =>
                {
                    next(ctxt);

                    registInterceptorsForGenericMethodCache(ctxt);

                    var proxiedInterfaces = ctxt.Instance
                        .GetType()
                        .GetInterfaces()
                        .Where(ProxyUtil.IsAccessible)
                        .ToArray();

                    if (!proxiedInterfaces.Any())
                    {
                        return;
                    }

                    var theInterface = proxiedInterfaces.First();
                    var interfaces = proxiedInterfaces.Skip(1).ToArray();

                    var interceptors = GetInterceptorServices(ctxt.Registration, ctxt.Instance.GetType())
                        .Select(ctxt.ResolveService)
                        .Cast<IInterceptor>()
                        .ToArray();

                    //这里需要改一下
                    //https://github.com/JSkimming/Castle.Core.AsyncInterceptor/blob/master/src/Castle.Core.AsyncInterceptor/ProxyGeneratorExtensions.cs
                    ctxt.Instance = options == null
                        ? ProxyGenerator.CreateInterfaceProxyWithTarget(theInterface, interfaces, ctxt.Instance, interceptors)
                        : ProxyGenerator.CreateInterfaceProxyWithTarget(theInterface, interfaces, ctxt.Instance, options, interceptors);
                }));

                EnableProxy = true;
            }
            catch (Exception ex)
            {
                EnableProxy = false;
            }



            return (EnableProxy, registration);
        }

        private static void registInterceptorsForGenericMethodCache(ResolveRequestContext ctxt)
        {

            //防止并发访问 出现System.ArgumentException: An item with the same key has already been added. Key: Autofac.Extras.DynamicProxy.RegistrationExtensions.InterceptorsForGenericMethodCache
            lock (InterceptorsForGenericMethodCacheLock)
            {

                if (!ctxt.Registration.Metadata.TryGetValue(InterceptorsForGenericMethodCache, out var _))
                {
                    ctxt.Registration.Metadata.Add(InterceptorsForGenericMethodCache, true);

                }
            }

        }

        /// <summary>
        /// Allows a list of interceptor services to be assigned to the registration.
        /// </summary>
        /// <typeparam name="TLimit">Registration limit type.</typeparam>
        /// <typeparam name="TActivatorData">Activator data type.</typeparam>
        /// <typeparam name="TStyle">Registration style.</typeparam>
        /// <param name="builder">Registration to apply interception to.</param>
        /// <param name="interceptorServices">The interceptor services.</param>
        /// <returns>Registration builder allowing the registration to be configured.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="builder"/> or <paramref name="interceptorServices"/>.</exception>
        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> InterceptedBy<TLimit, TActivatorData, TStyle>(
            this IRegistrationBuilder<TLimit, TActivatorData, TStyle> builder,
            params Service[] interceptorServices)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (interceptorServices == null || interceptorServices.Any(s => s == null))
            {
                throw new ArgumentNullException(nameof(interceptorServices));
            }

            AddInterceptorServicesToMetadata(builder, interceptorServices, InterceptorsPropertyName);

            return builder;
        }


        /// <summary>
        /// Allows a list of interceptor services to be assigned to the registration.
        /// </summary>
        /// <typeparam name="TLimit">Registration limit type.</typeparam>
        /// <typeparam name="TActivatorData">Activator data type.</typeparam>
        /// <typeparam name="TStyle">Registration style.</typeparam>
        /// <param name="builder">Registration to apply interception to.</param>
        /// <param name="interceptorServiceTypes">The types of the interceptor services.</param>
        /// <returns>Registration builder allowing the registration to be configured.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> or <paramref name="interceptorServiceTypes"/>.</exception>
        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> InterceptedBy<TLimit, TActivatorData, TStyle>(
            this IRegistrationBuilder<TLimit, TActivatorData, TStyle> builder,
            params Type[] interceptorServiceTypes)
        {
            if (interceptorServiceTypes == null || interceptorServiceTypes.Any(t => t == null))
            {
                throw new ArgumentNullException(nameof(interceptorServiceTypes));
            }

            return InterceptedBy(builder, interceptorServiceTypes.Select(t => new TypedService(t)).ToArray());
        }


        private static void AddInterceptorServicesToMetadata<TLimit, TActivatorData, TStyle>(
            IRegistrationBuilder<TLimit, TActivatorData, TStyle> builder,
            IEnumerable<Service> interceptorServices,
            string metadataKey)
        {
            lock (lockobj)
            { 
            
                object existing;
                if (builder.RegistrationData.Metadata.TryGetValue(metadataKey, out existing))
                {
                    builder.RegistrationData.Metadata[metadataKey] =
                        ((IEnumerable<Service>)existing).Concat(interceptorServices).Distinct();
                }
                else
                {
                    builder.RegistrationData.Metadata.Add(metadataKey, interceptorServices);
                }
            }

        }

        /// <summary>
        /// 添加拦截器类型到当前的metadata
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="interceptorType"></param>
        public static IComponentRegistration InterceptedBy(this IComponentRegistration builder, Type interceptorType)
        {
            lock (lockobj)
            { 
            
                IEnumerable<Service> services = new Service[] { new TypedService(interceptorType) };
                if (builder.Metadata.TryGetValue(InterceptorsPropertyName, out var existing))
                {
                    builder.Metadata[InterceptorsPropertyName] = ((IEnumerable<Service>)existing).Concat(services).Distinct();
                    ;
                }
                else
                {
                    builder.Metadata.Add(InterceptorsPropertyName, services);
                }
            }


            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IComponentRegistration WithMetadata(this IComponentRegistration builder, string key, object value)
        {
            lock (lockobj)
            { 
                builder.Metadata.Add(key, value);
                return builder;
            
            }

        }

        private static IEnumerable<Service> GetInterceptorServices(IComponentRegistration registration, Type implType)
        {
            if (registration == null)
            {
                throw new ArgumentNullException(nameof(registration));
            }

            if (implType == null)
            {
                throw new ArgumentNullException(nameof(implType));
            }

            var result = EmptyServices;

            object services;
            if (registration.Metadata.TryGetValue(InterceptorsPropertyName, out services))
            {
                result = result.Concat((IEnumerable<Service>)services);
            }

            return registration.Metadata.TryGetValue(AttributeInterceptorsPropertyName, out services)
                ? result.Concat((IEnumerable<Service>)services)
                : result.Concat(new List<Service>());
        }

    }
}
