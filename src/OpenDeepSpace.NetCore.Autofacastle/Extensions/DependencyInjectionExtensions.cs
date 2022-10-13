using OpenDeepSpace.NetCore.Autofacastle.AspectAttention;
using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection;
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
    /// 类型拓展
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// 过滤掉依赖注入接口
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static Type[] FilterDependencyInjectionInterfaces(this Type[] types)
        {

            types = types.Where(t => t != typeof(ITransient) && t != typeof(IScoped) && t != typeof(ISingleton) && t!=typeof(INonIntercept) && t!=typeof(IClassIntercept)).ToArray();

            return types;
        }

        /// <summary>
        /// 是否有依赖注入特性和接口
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsHasDependencyInjectionAttributeOrInterface(this Type type)
        {
            return
                type.GetCustomAttributes().Select(t => t.GetType()).
                    Any(t => t == typeof(TransientAttribute) ||
                           t == typeof(ScopedAttribute) ||
                           t == typeof(SingletonAttribute)
                        )
                    ||

                type.GetInterfaces().Any(t => t == typeof(ITransient)
                          || t == typeof(IScoped)
                          || t == typeof(ISingleton)
                    );


        }

        /// <summary>
        /// 尝试添加相同的Keyed 如果Keyed存在 抛出异常 否则添加
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dependencyInjectionAttribute"></param>
        public static void TryAddExistSameKeyed(this Type type,DependencyInjectionAttribute? dependencyInjectionAttribute)
        {
            //如果Keyed不为空
            if (dependencyInjectionAttribute != null && dependencyInjectionAttribute.Keyed != null)
            {
                if (AutofacastleCollection.KeyedImplementationTypes.ContainsKey(dependencyInjectionAttribute.Keyed))
                {
                    //不是同一类型
                    if (AutofacastleCollection.KeyedImplementationTypes[dependencyInjectionAttribute.Keyed] != type)
                        throw new Exception($"已经存在一个相同的Keyed:{dependencyInjectionAttribute.Keyed}在{AutofacastleCollection.KeyedImplementationTypes[dependencyInjectionAttribute.Keyed]}类上");
                    else
                        return;//是同一类型
                }
                AutofacastleCollection.KeyedImplementationTypes[dependencyInjectionAttribute.Keyed] = type;
            }
        }

        /// <summary>
        /// 尝试添加相同的Named 如果Named存在抛出异常 否则添加
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dependencyInjectionAttribute"></param>
        public static void TryAddExistSameNamed(this Type type,DependencyInjectionAttribute? dependencyInjectionAttribute)
        {
            //如果Named不为空
            if (dependencyInjectionAttribute != null && !string.IsNullOrWhiteSpace(dependencyInjectionAttribute.Named))
            {
                if (AutofacastleCollection.NamedImplementationTypes.ContainsKey(dependencyInjectionAttribute.Named))
                {
                    //如果不是同一个类型
                    if (AutofacastleCollection.NamedImplementationTypes[dependencyInjectionAttribute.Named] != type)
                        throw new Exception($"已经存在一个相同的Named:{dependencyInjectionAttribute.Named}在{AutofacastleCollection.NamedImplementationTypes[dependencyInjectionAttribute.Named]}类上");
                    else
                        return;//是同一类型直接返回
                }
                AutofacastleCollection.NamedImplementationTypes[dependencyInjectionAttribute.Named] = type;
            }
        }

        /// <summary>
        /// 尝试添加相同的Keyed或Named
        /// </summary>
        /// <param name="dependencyInjectionAttribute"></param>
        public static void TryAddExistSameKeyedOrNamed(this Type type,DependencyInjectionAttribute? dependencyInjectionAttribute)
        {
            TryAddExistSameKeyed(type, dependencyInjectionAttribute);
            TryAddExistSameNamed(type, dependencyInjectionAttribute);
        }
    }
}
