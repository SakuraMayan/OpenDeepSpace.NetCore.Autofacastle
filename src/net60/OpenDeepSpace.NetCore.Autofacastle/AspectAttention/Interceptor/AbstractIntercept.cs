using Autofac;
using Castle.DynamicProxy;
using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.Attributes;
using OpenDeepSpace.NetCore.Autofacastle.Extensions;
using stakx.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Interceptor
{
    /// <summary>
    /// 抽象拦截
    /// AbstractIntercept InterceptCache InterceptPointCache InterceptPointAbstractIntercept
    /// 为防止全局拦截情况下循环依赖异常 设置为非拦截 由于与拦截特性在同一程序集下已不会拦截
    /// </summary>
    [Transient(AsServices = new[] { typeof(AbstractIntercept) })]
    public class AbstractIntercept : AsyncInterceptor
    {
        private readonly IComponentContext componentContext;
        private readonly InterceptCache interceptMethodCache;


        public AbstractIntercept(IComponentContext componentContext)
        {
            this.componentContext = componentContext;
            this.interceptMethodCache = componentContext.Resolve<InterceptCache>();
        }


        protected override void Intercept(IInvocation invocation)
        {
            InterceptInternal(invocation);

        }

        internal void InterceptInternal(IInvocation invocation)
        {
            if (!interceptMethodCache.CacheList.TryGetValue(invocation.MethodInvocationTarget, out var attribute))
            {
                //动态泛型类
                if (!invocation.MethodInvocationTarget.DeclaringType.GetTypeInfo().IsGenericType ||
                    (!interceptMethodCache.DynamicCacheList.TryGetValue(invocation.MethodInvocationTarget.GetMethodInfoUniqueName(), out var AttributesDynamic)))
                {
                    invocation.Proceed();
                    return;
                }

                attribute = AttributesDynamic;
            }


            var catpture = invocation.CaptureProceedInfo();
            var interceptContext = new InterceptContext(componentContext, invocation);
            interceptContext.Proceed = () =>
            {
                catpture.Invoke();
                return new ValueTask();
            };

            var runTask = attribute.InterceptFunc.Value;//这里开始启动拦截链的委托以及构建执行
            var task = runTask(interceptContext);
            // If the intercept task has yet to complete, wait for it.
            if (!task.IsCompleted)
            {
                // Need to use Task.Run() to prevent deadlock in .NET Framework ASP.NET requests.
                // GetAwaiter().GetResult() prevents a thrown exception being wrapped in a AggregateException.
                // See https://stackoverflow.com/a/17284612
                Task.Run(() => task).GetAwaiter().GetResult();
            }

            task.RethrowIfFaulted();
        }

        protected override async ValueTask InterceptAsync(IAsyncInvocation invocation)
        {
            await InterceptInternalAsync(invocation);
        }

        internal async ValueTask InterceptInternalAsync(IAsyncInvocation invocation)
        {
            if (!interceptMethodCache.CacheList.TryGetValue(invocation.TargetMethod, out var attribute))
            {
                var result = !invocation.TargetMethod.DeclaringType.GetTypeInfo().IsGenericType;

                //动态泛型类
                if (!invocation.TargetMethod.DeclaringType.GetTypeInfo().IsGenericType ||
                    (!interceptMethodCache.DynamicCacheList.TryGetValue(invocation.TargetMethod.GetMethodInfoUniqueName(), out var AttributesDynamic)))
                {
                    await invocation.ProceedAsync();
                    return;
                }

                attribute = AttributesDynamic;
            }


            var interceptContext = new InterceptContext(componentContext, invocation);
            interceptContext.Proceed = async () => { await invocation.ProceedAsync(); };

            var runTask = attribute.InterceptFunc.Value;
            await runTask(interceptContext);
        }
    }
}
