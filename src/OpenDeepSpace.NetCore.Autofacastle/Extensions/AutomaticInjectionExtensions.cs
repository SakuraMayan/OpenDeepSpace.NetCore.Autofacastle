using Autofac;
using Autofac.Core.Resolving.Pipeline;
using Microsoft.Extensions.Options;
using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.Extensions
{
    /// <summary>
    /// 自动注入拓展
    /// </summary>
    public static class AutomaticInjectionExtensions
    {

        /// <summary>
        /// 是否自动注入
        /// </summary>
        /// <param name="instanceType"></param>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static bool IsNeedAutomaticInjection(this Type instanceType, MemberInfo memberInfo)
        {
            return IsNeedAutomaticInjection(instanceType,memberInfo,null);
        }

        /// <summary>
        /// 是否自动注入
        /// </summary>
        /// <param name="instanceType"></param>
        /// <param name="parameterInfo"></param>
        /// <returns></returns>
        public static bool IsNeedAutomaticInjection(this Type instanceType, ParameterInfo parameterInfo)
        {
            return IsNeedAutomaticInjection(instanceType, null, parameterInfo);

        }

        /// <summary>
        /// 解析出服务实例
        /// </summary>
        /// <param name="resolveRequestContext"></param>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static object? ResolveServiceInstance(this ResolveRequestContext resolveRequestContext,MemberInfo memberInfo)
        {
            return ResolveServiceInstance(resolveRequestContext, null, memberInfo, null);
        }

        /// <summary>
        /// 解析出服务实例
        /// </summary>
        /// <param name="componentContext"></param>
        /// <param name="parameterInfo"></param>
        /// <returns></returns>
        public static object? ResolveServiceInstance(this IComponentContext componentContext, ParameterInfo parameterInfo)
        {
            return ResolveServiceInstance(null, componentContext, null, parameterInfo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="parameterInfo"></param>
        /// <returns></returns>
        private static object? ResolveServiceInstance(ResolveRequestContext? resolveRequestContext, IComponentContext? componentContext,MemberInfo? memberInfo, ParameterInfo? parameterInfo)
        {
            object? serviceInstance = null;//服务类实例
            Type? serviceType = null;//服务类类型
            Type? implementationType = null;//实现类型
            object? Keyed = null;//Keyed
            string? Named = null;//Named
            if (memberInfo != null)
            {
                if ((memberInfo as PropertyInfo) != null)
                    serviceType = (memberInfo as PropertyInfo)!.PropertyType;
                if ((memberInfo as FieldInfo) != null)
                    serviceType = (memberInfo as FieldInfo)!.FieldType;
            }
            if (parameterInfo != null)
                serviceType = parameterInfo.ParameterType;

            if (serviceType == null)//serviceType为空 直接返回
                return serviceInstance;

            //获取成员信息上的自动注入特性
            AutomaticInjectionAttribute? automaticInjectionAttrInMemberInfo = null;
            if (memberInfo != null) automaticInjectionAttrInMemberInfo = memberInfo.GetCustomAttribute<AutomaticInjectionAttribute>();
            if (parameterInfo != null) automaticInjectionAttrInMemberInfo = parameterInfo.GetCustomAttribute<AutomaticInjectionAttribute>();
            //是否存在自动注入特性
            if (automaticInjectionAttrInMemberInfo != null)
            { 
                implementationType = automaticInjectionAttrInMemberInfo.ImplementationType;
                Keyed = automaticInjectionAttrInMemberInfo.Keyed;
                Named = automaticInjectionAttrInMemberInfo.Named;
            }

            //尝试去解析服务实例
            if (resolveRequestContext != null) 
            {
                //如果实现类型不为空 
                if (implementationType != null)
                {
                    /*//非泛型 或 是泛型但不是IOptions<> 尝试优先以Keyed去解析
                    if (!implementationType.IsGenericType || implementationType.IsGenericType && typeof(IOptions<>) != implementationType.GetGenericTypeDefinition())
                    {*/

                        resolveRequestContext.TryResolveKeyed(implementationType, serviceType, out serviceInstance);
                        if (serviceInstance == null && implementationType.FullName != null)
                        {
                       
                            //通过Keyed未能解析出实例 尝试通过Named去解析
                            resolveRequestContext.TryResolveNamed(implementationType.FullName, serviceType, out serviceInstance);

                        }

                        //通过keyed和Named都未解析出实例 尝试直接通过实现类型去解析
                        if (serviceInstance == null)
                            resolveRequestContext.TryResolve(implementationType, out serviceInstance);
                    /*}
                    else
                    {
                        resolveRequestContext.TryResolve(implementationType, out serviceInstance);
                    }*/

                }
                else if (Keyed != null)//Keyed不为空
                {
                    if (serviceInstance == null)//根据实现类没有解析出来
                    {
                        resolveRequestContext.TryResolveKeyed(Keyed,serviceType, out serviceInstance);
                    }
                }
                else if (Named != null )//Named不为空
                {
                    if (serviceInstance == null)//根据实现类 和 Keyed都没解析出来
                    {
                        resolveRequestContext.TryResolveNamed(Named,serviceType, out serviceInstance);
                    }
                }
                else
                {//如果都没指定

                    //直接服务类型去解析
                    resolveRequestContext.TryResolve(serviceType, out serviceInstance);

                }

            }
            if (componentContext != null)
            {
                //如果实现类型不为空 
                if (implementationType != null)
                {
                    /*//非泛型 或 是泛型但不是IOptions<> 尝试优先以Keyed去解析
                    if (!implementationType.IsGenericType || implementationType.IsGenericType && typeof(IOptions<>) != implementationType.GetGenericTypeDefinition())
                    {*/

                        componentContext.TryResolveKeyed(implementationType, serviceType, out serviceInstance);
                        if (serviceInstance == null && implementationType.FullName != null)
                        {
                            //通过Keyed未能解析出实例 尝试通过Named去解析
                            componentContext.TryResolveNamed(implementationType.FullName,serviceType, out serviceInstance);

                        }
                        //通过keyed和Named都未解析出实例 尝试直接通过实现类型去解析
                        if (serviceInstance == null)
                            componentContext.TryResolve(implementationType, out serviceInstance);
                    /*}
                    else
                    {
                        componentContext.TryResolve(implementationType, out serviceInstance);
                    }*/
                    

                }
                else if (Keyed != null)//Keyed不为空
                {
                    if (serviceInstance == null)//根据实现类没有解析出来
                    {
                        componentContext.TryResolveKeyed(Keyed, serviceType, out serviceInstance);
                    }
                }
                else if (Named != null)//Named不为空
                {
                    if (serviceInstance == null)//根据实现类 和 Keyed都没解析出来
                    {
                        componentContext.TryResolveNamed(Named, serviceType, out serviceInstance);
                    }
                }
                else
                {
                    //直接服务类型去解析
                    componentContext.TryResolve(serviceType, out serviceInstance);

                }
            }

            //解析出的实例不为空
            if (serviceInstance != null && implementationType!=null)
            {
                //对选项的处理IOptions<xxx>
                //implementationType是IOptions<> 并且 serviceType不是IOptions<>
                if (implementationType.IsGenericType && implementationType.GetGenericTypeDefinition() == typeof(IOptions<>) &&  (!serviceType.IsGenericType || (serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() != typeof(IOptions<>))))
                {
                    //获取真实的值
                    //获取返回值中的属性某个属性信息
                    PropertyInfo? propertyInfo = serviceInstance.GetType().GetProperty("Value");
                    //获取出IOptions<xxxOption>的value值 即xxxOption
                    serviceInstance = propertyInfo?.GetValue(serviceInstance);
                }
            }

            return serviceInstance;
        }

        private static bool IsNeedAutomaticInjection(this Type instanceType, MemberInfo? memberInfo, ParameterInfo? parameterInfo)
        { 
            //类上使用了不自动注入特性 直接返回
            if (instanceType.GetCustomAttribute<NonAutomaticInjectionAttribute>() != null)
                return false;

            //获取不自动注入特性
            NonAutomaticInjectionAttribute? nonAutomaticInjectionAttrInMemberInfo = null;
            if (memberInfo != null) nonAutomaticInjectionAttrInMemberInfo = memberInfo.GetCustomAttribute<NonAutomaticInjectionAttribute>();
            if (parameterInfo != null) nonAutomaticInjectionAttrInMemberInfo = parameterInfo.GetCustomAttribute<NonAutomaticInjectionAttribute>();  
            if (nonAutomaticInjectionAttrInMemberInfo != null)
                return false;
            //获取成员信息上的自动注入特性
            AutomaticInjectionAttribute? automaticInjectionAttrInMemberInfo = null;
            if (memberInfo != null) automaticInjectionAttrInMemberInfo = memberInfo.GetCustomAttribute<AutomaticInjectionAttribute>();
            if(parameterInfo!=null) automaticInjectionAttrInMemberInfo = parameterInfo.GetCustomAttribute<AutomaticInjectionAttribute>();
            //获取类上的自动注入特性
            var automaticInjectionAttrInClass = instanceType.GetCustomAttribute<AutomaticInjectionAttribute>();
            //判断类是否符合自动注入筛选器集的条件
            bool isConformAutomaticInjectionSelector = false;
            if (AutofacastleCollection.AutomaticInjectionSelectors != null && AutofacastleCollection.AutomaticInjectionSelectors.Any())
            {
                //符合筛选条件
                if (AutofacastleCollection.AutomaticInjectionSelectors.Any(t => t.Predicate(instanceType)))
                    isConformAutomaticInjectionSelector = true;
            }

            //如果成员信息或类或符合自动注入筛选级的条件都需要自动注入
            if (automaticInjectionAttrInMemberInfo != null || automaticInjectionAttrInClass != null || isConformAutomaticInjectionSelector == true)
            {
                return true;
            }

            return false;
        }

    }
}
