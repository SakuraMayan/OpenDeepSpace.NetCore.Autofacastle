using Autofac.Core.Resolving.Pipeline;
using Castle.DynamicProxy;
using OpenDeepSpace.NetCore.Autofacastle.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.Middlewares
{
    /// <summary>
    /// 自动注入实例中间件
    /// </summary>
    internal class AutomaticInjectionMiddleware : IResolveMiddleware
    {
        public PipelinePhase Phase => PipelinePhase.RegistrationPipelineStart;

        /// <summary>
        /// 执行自动注入
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        public void Execute(ResolveRequestContext context, Action<ResolveRequestContext> next)
        {
            next(context);

            //获取当前的实例
            object? instance = context.Instance;
            if (instance != null)
            {
                var instanceType = instance.GetType();
                if (ProxyUtil.IsProxy(instance))//如果是代理类 获取实际类以及其类型
                {
                    instance = ProxyUtil.GetUnproxiedInstance(instance);
                    instanceType = ProxyUtil.GetUnproxiedType(instance);
                }

                //获取属性 不包含基类的
                var propertyInfos = instanceType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                foreach (var propertyInfo in propertyInfos)
                {
                    ResolveInjection(instance, instanceType, propertyInfo, context);
                    ValueInjection(instance, propertyInfo, context);
                }

                //获取字段 不包含基类的
                var fieldInfos = instanceType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                foreach (var fieldInfo in fieldInfos)
                {
                    ResolveInjection(instance, instanceType, fieldInfo, context);
                    ValueInjection(instance, fieldInfo, context);
                }
            }

        }

        /// <summary>
        /// 解析注入
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="instanceType"></param>
        /// <param name="memberInfo"></param>
        /// <param name="context"></param>
        private static void ResolveInjection(object instance, Type instanceType, MemberInfo memberInfo, ResolveRequestContext context)
        {
            //判断是否需要自动注入
            bool result = instanceType.IsNeedAutomaticInjection(memberInfo);
            if (!result)
                return;
            //解析获取自动注入的值
            object? serviceInstance = context.ResolveServiceInstance(memberInfo);

            if (serviceInstance != null)
            {

                (memberInfo as PropertyInfo)?.SetValue(instance, serviceInstance);
                (memberInfo as FieldInfo)?.SetValue(instance, serviceInstance);
            }
        }

        /// <summary>
        /// 值注入
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="memberInfo"></param>
        /// <param name="context"></param>
        private static void ValueInjection(object instance, MemberInfo memberInfo, ResolveRequestContext context)
        {
            //判断是否需要值注入
            bool result = memberInfo.IsNeedValueInjection();
            if (!result)
                return;
            //获取值
            object value = context.ResolveValue(memberInfo);

            if (value != null)
            {
                (memberInfo as PropertyInfo)?.SetValue(instance, value);
                (memberInfo as FieldInfo)?.SetValue(instance, value);
            }

        }
    }
}
