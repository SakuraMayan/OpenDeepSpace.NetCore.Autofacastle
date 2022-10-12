using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention
{
    /// <summary>
    /// 抛出异常时拦截处理
    /// </summary>
    public class MethodAfterThrowingInterceptHandle : IInterceptHandle
    {
        private readonly Interceptor.Attributes.MethodAfterThrowingAbstractInterceptAttribute methodAfterThrowingIntercept;
        private readonly bool IsFromAround;//环绕拦截是否执行

        public MethodAfterThrowingInterceptHandle(Interceptor.Attributes.MethodAfterThrowingAbstractInterceptAttribute methodAfterThrowingIntercept, bool isFromAround=false)
        {
            this.methodAfterThrowingIntercept = methodAfterThrowingIntercept;
            IsFromAround = isFromAround;
        }

        //拦截点
        private readonly InterceptPointRunTimeMethod<InterceptorPoint.Attributes.MethodAfterThrowingInterceptAttribute> interceptPointRunTimeMethod;

        public MethodAfterThrowingInterceptHandle(InterceptPointRunTimeMethod<InterceptorPoint.Attributes.MethodAfterThrowingInterceptAttribute> interceptPointRunTimeMethod, bool isFromAround = false)
        {
            this.interceptPointRunTimeMethod = interceptPointRunTimeMethod;
            this.IsFromAround = isFromAround;
        }

        public async Task OnInvocation(InterceptContext interceptContext, InterceptContextDelegate next)
        {
            try
            {
                if (!IsFromAround) await next.Invoke(interceptContext);
            }
            finally
            {
                //只有目标方法出现异常才会走 代理的方法出异常不走
                if (interceptContext.Exception != null)
                {
                    Exception ex = interceptContext.Exception;
                    if (interceptContext.Exception is TargetInvocationException targetInvocationException)
                    {
                        ex = targetInvocationException.InnerException;
                    }

                    if (ex == null)
                    {
                        ex = interceptContext.Exception;
                    }

                    var currentExType = ex.GetType();

                    //拦截点
                    if (interceptPointRunTimeMethod != null)
                    {
                        if (interceptPointRunTimeMethod.InterceptMethodAttribute.ExceptionType == null ||
                            interceptPointRunTimeMethod.InterceptMethodAttribute.ExceptionType == currentExType
                            )
                        {
                            object obj = MethodInvokeHelper.InvokeInstanceMethod(
                                interceptPointRunTimeMethod.Instance,
                                interceptPointRunTimeMethod.MethodInfo,
                                interceptPointRunTimeMethod.MethodParameters,
                                interceptContext.ComponentContext,
                                interceptContext,
                                returnValue: ex,
                                returnParam: interceptPointRunTimeMethod.InterceptMethodAttribute.Throwing
                                );

                            if (typeof(Task).IsAssignableFrom(interceptPointRunTimeMethod.MethodReturnType))
                                await ((Task)obj).ConfigureAwait(false);
                        }
                    }
                    else 
                    {
                        if (methodAfterThrowingIntercept.ExceptionType == null || methodAfterThrowingIntercept.ExceptionType == currentExType)
                        {
                            await methodAfterThrowingIntercept.AfterThrowing(interceptContext, interceptContext.Exception);
                        }
                    
                    }

                }
            }
        }
    }
}
