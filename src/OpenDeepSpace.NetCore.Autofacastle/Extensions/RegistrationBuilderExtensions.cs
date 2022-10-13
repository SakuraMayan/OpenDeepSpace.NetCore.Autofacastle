using Autofac.Builder;
using Autofac.Extras.DynamicProxy;
using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Interceptor;
using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class RegistrationBuilderExtensions
    {
        /// <summary>
        /// 添加拦截器
        /// </summary>
        /// <param name="registrationBuilder"></param>
        /// <param name="implementationType"></param>
        /// <param name="Intercepted">是否可被重复添加拦截</param>
        /// <returns></returns>
        public static IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle> AddIntercept(this IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle> registrationBuilder, Type implementationType,bool Intercepted = true)
        {
            var interceptedTypeInfo = AutofacastleCollection.InterceptedTypeInfos.FirstOrDefault(t => t.InterceptedType == implementationType && t.Intercepted==false);
            if (interceptedTypeInfo != null)
            {
                if (interceptedTypeInfo.IsInterceptPointAbstractIntercept)
                {
                    switch (interceptedTypeInfo.InterceptType)
                    {
                        case AspectAttention.Enums.InterceptType.ClassIntercept://类拦截
                            var enableClassInterceptors = registrationBuilder.TryEnableClassInterceptors();
                            if (enableClassInterceptors.Item1)//能被代理
                                enableClassInterceptors.Item2.InterceptedBy(typeof(InterceptPointAbstractIntercept));
                            break;
                        case AspectAttention.Enums.InterceptType.InterfaceIntercept://接口拦截
                            var enableInterfaceInterceptors = registrationBuilder.TryEnableInterfaceInterceptors();//尝试代理
                            if (enableInterfaceInterceptors.Item1)//能被代理
                                enableInterfaceInterceptors.Item2.InterceptedBy(typeof(InterceptPointAbstractIntercept));
                            break;
                        default:
                            break;
                    }
                    interceptedTypeInfo.Intercepted = !Intercepted;
                    return registrationBuilder;
                }

                //AbstractIntercept拦截
                if (interceptedTypeInfo.IsAbstractIntercept)
                {
                    switch (interceptedTypeInfo.InterceptType)
                    {
                        case AspectAttention.Enums.InterceptType.ClassIntercept://类拦截
                            var enableClassInterceptors = registrationBuilder.TryEnableClassInterceptors();
                            if (enableClassInterceptors.Item1)//能被代理
                                enableClassInterceptors.Item2.InterceptedBy(typeof(AbstractIntercept));
                            break;
                        case AspectAttention.Enums.InterceptType.InterfaceIntercept://接口拦截
                            var enableInterfaceInterceptors = registrationBuilder.TryEnableInterfaceInterceptors();//尝试代理
                            if (enableInterfaceInterceptors.Item1)//能被代理
                                enableInterfaceInterceptors.Item2.InterceptedBy(typeof(AbstractIntercept));
                            break;
                        default:
                            break;
                    }
                    interceptedTypeInfo.Intercepted = !Intercepted;
                }
            }
            return registrationBuilder;
        }

        /// <summary>
        /// 添加拦截器
        /// </summary>
        /// <param name="registrationBuilder"></param>
        /// <param name="implementationType"></param>
        /// <param name="Intercepted">是否可被重复添加拦截</param>
        /// <returns></returns>
        public static IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> AddIntercept(this IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> registrationBuilder, Type implementationType,bool Intercepted=true)
        {
            var interceptedTypeInfo = AutofacastleCollection.InterceptedTypeInfos.FirstOrDefault(t => t.InterceptedType == implementationType && t.Intercepted==false);
            if (interceptedTypeInfo != null)
            {
                if (interceptedTypeInfo.IsInterceptPointAbstractIntercept)
                {
                    switch (interceptedTypeInfo.InterceptType)
                    {
                        case AspectAttention.Enums.InterceptType.ClassIntercept://类拦截
                            var enableClassInterceptors = registrationBuilder.TryEnableClassInterceptors();
                            if (enableClassInterceptors.Item1)//能被代理
                                enableClassInterceptors.Item2.InterceptedBy(typeof(InterceptPointAbstractIntercept));
                            break;
                        case AspectAttention.Enums.InterceptType.InterfaceIntercept://接口拦截
                            var enableInterfaceInterceptors = registrationBuilder.TryEnableInterfaceInterceptors();//尝试代理
                            if (enableInterfaceInterceptors.Item1)//能被代理
                                enableInterfaceInterceptors.Item2.InterceptedBy(typeof(InterceptPointAbstractIntercept));
                            break;
                        default:
                            break;
                    }
                    interceptedTypeInfo.Intercepted = !Intercepted;
                    return registrationBuilder;
                }

                //AbstractIntercept拦截
                if (interceptedTypeInfo.IsAbstractIntercept)
                {
                    switch (interceptedTypeInfo.InterceptType)
                    {
                        case AspectAttention.Enums.InterceptType.ClassIntercept://类拦截
                            var enableClassInterceptors = registrationBuilder.TryEnableClassInterceptors();
                            if (enableClassInterceptors.Item1)//能被代理
                                enableClassInterceptors.Item2.InterceptedBy(typeof(AbstractIntercept));
                            break;
                        case AspectAttention.Enums.InterceptType.InterfaceIntercept://接口拦截
                            var enableInterfaceInterceptors = registrationBuilder.TryEnableInterfaceInterceptors();//尝试代理
                            if (enableInterfaceInterceptors.Item1)//能被代理
                                enableInterfaceInterceptors.Item2.InterceptedBy(typeof(AbstractIntercept));
                            break;
                        default:
                            break;
                    }
                }
                interceptedTypeInfo.Intercepted =  !Intercepted;
            }
            return registrationBuilder;

        }
    }
}
