using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention
{
    /// <summary>
    /// 方法正常执行后处理
    /// </summary>
    public class MethodAfterReturnInterceptHandle : IInterceptHandle
    {
        private readonly Interceptor.Attributes.MethodAfterReturnAbstractInterceptAttribute methodAfterReturnIntercept;

        public MethodAfterReturnInterceptHandle(Interceptor.Attributes.MethodAfterReturnAbstractInterceptAttribute methodAfterReturnIntercept)
        {
            this.methodAfterReturnIntercept = methodAfterReturnIntercept;
        }

        //拦截点
        private readonly InterceptPointRunTimeMethod<InterceptorPoint.Attributes.MethodAfterReturnInterceptAttribute> interceptPointRunTimeMethod;

        public MethodAfterReturnInterceptHandle(InterceptPointRunTimeMethod<InterceptorPoint.Attributes.MethodAfterReturnInterceptAttribute> interceptPointRunTimeMethod)
        {
            this.interceptPointRunTimeMethod = interceptPointRunTimeMethod;
        }

        public async Task OnInvocation(InterceptContext interceptContext, InterceptContextDelegate next)
        {
            await next.Invoke(interceptContext);

            //如果出现异常返回 Throw存在就去执行
            if (interceptContext.Exception != null)
            {
                return;
            }

            if (methodAfterReturnIntercept != null)
                await this.methodAfterReturnIntercept.AfterReturn(interceptContext, interceptContext.ReturnValue);
            else 
            {//拦截点执行

                object obj = MethodInvokeHelper.InvokeInstanceMethod(

                    interceptPointRunTimeMethod.Instance,
                    interceptPointRunTimeMethod.MethodInfo,
                    interceptPointRunTimeMethod.MethodParameters,
                    interceptContext.ComponentContext,
                    interceptContext,
                    returnValue: interceptContext.ReturnValue,
                    returnParam:  interceptPointRunTimeMethod.InterceptMethodAttribute.ReturnValue
                );

                if (typeof(Task).IsAssignableFrom(interceptPointRunTimeMethod.MethodReturnType))
                { 
                    await ((Task)obj).ConfigureAwait(false);    
                }

            }
        }
    }
}
