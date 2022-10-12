using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention
{
    /// <summary>
    /// 方法执行前后处理
    /// </summary>
    public class MethodAroundInterceptHandle : IInterceptHandle
    {
        private readonly Interceptor.Attributes.MethodAroundAbstractInterceptAttribute methodAroundIntercept;
        private readonly MethodAfterInterceptHandle afterInterceptHandle;
        private readonly MethodAfterThrowingInterceptHandle throwInterceptHandle;

        public MethodAroundInterceptHandle(Interceptor.Attributes.MethodAroundAbstractInterceptAttribute methodAroundIntercept, Interceptor.Attributes.MethodAfterAbstractInterceptAttribute methodAfterIntercept, Interceptor.Attributes.MethodAfterThrowingAbstractInterceptAttribute methodAfterThrowingIntercept)
        {
            this.methodAroundIntercept = methodAroundIntercept;
            if (methodAfterIntercept != null)
            { 
                this.afterInterceptHandle=new MethodAfterInterceptHandle(methodAfterIntercept,true);   
            }
            if (methodAfterThrowingIntercept != null)
            { 
                this.throwInterceptHandle = new MethodAfterThrowingInterceptHandle(methodAfterThrowingIntercept,true);
            
            }
        }

        //拦截点
        private readonly InterceptPointRunTimeMethod<InterceptorPoint.Attributes.MethodAroundInterceptAttribute> interceptPointRunTimeMethod;

        public MethodAroundInterceptHandle(InterceptPointRunTimeMethod<InterceptorPoint.Attributes.MethodAroundInterceptAttribute> interceptPointRunTimeMethod,MethodAfterInterceptHandle afterInterceptHandle,MethodAfterThrowingInterceptHandle afterThrowInterceptHandle)
        {
            this.interceptPointRunTimeMethod=interceptPointRunTimeMethod;
            this.afterInterceptHandle = afterInterceptHandle;
            this.throwInterceptHandle=afterThrowInterceptHandle;
        }

        public async Task OnInvocation(InterceptContext interceptContext, InterceptContextDelegate next)
        {
            Exception exception = null;
            try
            {
                if (methodAroundIntercept != null)
                { 
                    await methodAroundIntercept.Around(interceptContext, next);
                    return;
                
                }

                //拦截点
                object obj = MethodInvokeHelper.InvokeInstanceMethod(
                    interceptPointRunTimeMethod.Instance,
                    interceptPointRunTimeMethod.MethodInfo,
                    interceptPointRunTimeMethod.MethodParameters,
                    interceptContext.ComponentContext,
                    interceptContext,
                    next
                    );

                if (typeof(Task).IsAssignableFrom(interceptPointRunTimeMethod.MethodReturnType))
                { 
                    await ((Task)obj).ConfigureAwait(false);
                }
             
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                if (exception == null && afterInterceptHandle != null) await afterInterceptHandle.OnInvocation(interceptContext, next);
            }

            try
            {
                if (exception != null && afterInterceptHandle != null)
                {
                    await afterInterceptHandle.OnInvocation(interceptContext, next);
                }

                if (exception != null && throwInterceptHandle != null)
                {
                    await throwInterceptHandle.OnInvocation(interceptContext, next);
                }
            }
            finally
            {
                if (exception != null) throw exception;
            }
        }
    }
}
