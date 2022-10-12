using Autofac;
using Castle.DynamicProxy;
using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Interceptor;
using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.Attributes;
using OpenDeepSpace.NetCore.Autofacastle.Extensions;
using stakx.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint
{
    /// <summary>
    /// 拦截点拦截逻辑
    /// </summary>
    [Transient(AsServices = new[] { typeof(InterceptPointAbstractIntercept) })]
    public class InterceptPointAbstractIntercept : AsyncInterceptor
    {
        private readonly AbstractIntercept _abstractIntercept;
        private readonly IComponentContext _component;
        private readonly InterceptPointCache _interceptPointCache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        public InterceptPointAbstractIntercept(IComponentContext component)
        {
            _abstractIntercept = component.Resolve<AbstractIntercept>();
            _component = component;
            _interceptPointCache = component.Resolve<InterceptPointCache>();
        }



        /// <summary>
        /// </summary>
        /// <param name="invocation"></param>
        protected override void Intercept(IInvocation invocation)
        {


            var interceptPoint = _interceptPointCache.CacheList.FirstOrDefault(t => t.Key.Equals(new ObjectKey(invocation.TargetType, invocation.Method))).Value;

            if (interceptPoint == null)
            {
                if (!invocation.MethodInvocationTarget.DeclaringType.GetTypeInfo().IsGenericType ||
                    !_interceptPointCache.DynamicCacheList.TryGetValue(invocation.MethodInvocationTarget.GetMethodInfoUniqueName(),
                        out var interceptPointDynamic))
                {
                    //该方法不需要拦截
                    _abstractIntercept.InterceptInternal(invocation);
                    // invocation.Proceed();
                    return;
                }

                interceptPoint = interceptPointDynamic;
            }

            var catpture = invocation.CaptureProceedInfo();
            var interceptContext = new InterceptContext(_component, invocation);
            interceptContext.Proceed = () =>
            {
                catpture.Invoke();
                return new ValueTask();
            };

            var runTask = interceptPoint.InterceptFunc.Value;
            var task = runTask(interceptContext);
            // If the intercept task has yet to complete, wait for it.
            if (!task.IsCompleted)
                // Need to use Task.Run() to prevent deadlock in .NET Framework ASP.NET requests.
                // GetAwaiter().GetResult() prevents a thrown exception being wrapped in a AggregateException.
                // See https://stackoverflow.com/a/17284612
                Task.Run(() => task).GetAwaiter().GetResult();

            task.RethrowIfFaulted();
        }


        /// <summary>
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        protected override async ValueTask InterceptAsync(IAsyncInvocation invocation)
        {
            var interceptPoint = _interceptPointCache.CacheList.FirstOrDefault(t => t.Key.Equals(new ObjectKey(invocation.TargetType, invocation.Method))).Value;

            if (interceptPoint == null)
            {
                if (!invocation.TargetMethod.DeclaringType.GetTypeInfo().IsGenericType ||
                    !_interceptPointCache.DynamicCacheList.TryGetValue(invocation.TargetMethod.GetMethodInfoUniqueName(),
                        out var interceptPointDynamic))
                {
                    //该方法不需要拦截
                    await _abstractIntercept.InterceptInternalAsync(invocation);
                    // await invocation.ProceedAsync();
                    return;
                }

                interceptPoint = interceptPointDynamic;
            }


            var interceptContext = new InterceptContext(_component, invocation);
            interceptContext.Proceed = async () => { await invocation.ProceedAsync(); };
            var runTask = interceptPoint.InterceptFunc.Value;
            await runTask(interceptContext);
        }
    }
}
