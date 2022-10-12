using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention
{
    /// <summary>
    /// 方法前拦截处理
    /// </summary>
    public class MethodBeforeInterceptHandle:IInterceptHandle
    {
        private readonly Interceptor.Attributes.MethodBeforeAbstractInterceptAttribute methodBeforeIntercept;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodBeforeIntercept"></param>
        public MethodBeforeInterceptHandle(Interceptor.Attributes.MethodBeforeAbstractInterceptAttribute methodBeforeIntercept)
        {
            this.methodBeforeIntercept = methodBeforeIntercept;
        }

        //拦截点
        private readonly InterceptPointRunTimeMethod<InterceptorPoint.Attributes.MethodBeforeInterceptAttribute> interceptPointRunTimeMethod;

        public MethodBeforeInterceptHandle(InterceptPointRunTimeMethod<InterceptorPoint.Attributes.MethodBeforeInterceptAttribute> interceptPointRunTimeMethod)
        {
            this.interceptPointRunTimeMethod = interceptPointRunTimeMethod;
        }

        public async Task OnInvocation(InterceptContext interceptContext, InterceptContextDelegate next)
        {
            if (methodBeforeIntercept != null)
                await this.methodBeforeIntercept.Before(interceptContext);
            else 
            {//执行拦截点
                object obj = MethodInvokeHelper.InvokeInstanceMethod(
                    interceptPointRunTimeMethod.Instance,
                    interceptPointRunTimeMethod.MethodInfo,
                    interceptPointRunTimeMethod.MethodParameters,
                    interceptContext.ComponentContext,
                    interceptContext
                    );
                if(typeof(Task).IsAssignableFrom(interceptPointRunTimeMethod.MethodReturnType))
                    await ((Task)obj).ConfigureAwait(false);
            }

            await next.Invoke(interceptContext);
        }
    }
}
