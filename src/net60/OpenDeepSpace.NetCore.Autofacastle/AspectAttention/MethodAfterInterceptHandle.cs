using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention
{
    /// <summary>
    /// 方法后拦截处理
    /// </summary>
    public class MethodAfterInterceptHandle : IInterceptHandle
    {
        private readonly Interceptor.Attributes.MethodAfterAbstractInterceptAttribute methodAfterIntercept;
        private readonly bool IsAfterAround;//是否环绕后执行??

        public MethodAfterInterceptHandle(Interceptor.Attributes.MethodAfterAbstractInterceptAttribute methodAfterIntercept, bool isAfterAround=false)
        {
            this.methodAfterIntercept = methodAfterIntercept;
            IsAfterAround = isAfterAround;
        }

        //拦截点
        private readonly InterceptPointRunTimeMethod<InterceptorPoint.Attributes.MethodAfterInterceptAttribute> interceptPointRunTimeMethod;

        public MethodAfterInterceptHandle(InterceptPointRunTimeMethod<InterceptorPoint.Attributes.MethodAfterInterceptAttribute> interceptPointRunTimeMethod, bool isAfterAround = false)
        {
            this.interceptPointRunTimeMethod = interceptPointRunTimeMethod; 
            this.IsAfterAround= isAfterAround;
        }

        public async Task OnInvocation(InterceptContext interceptContext, InterceptContextDelegate next)
        {
            try
            {
                if (!IsAfterAround) await next.Invoke(interceptContext);
            }
            finally
            {
                //不管成功还是失败都会执行的 
                if (methodAfterIntercept != null)
                    await this.methodAfterIntercept.After(interceptContext, interceptContext.Exception ?? interceptContext.ReturnValue);
                else 
                {//拦截点执行
                    object obj = MethodInvokeHelper.InvokeInstanceMethod(
                            interceptPointRunTimeMethod.Instance,
                            interceptPointRunTimeMethod.MethodInfo,
                            interceptPointRunTimeMethod.MethodParameters,
                            interceptContext.ComponentContext,
                            interceptContext,
                            returnValue: interceptContext.Exception??interceptContext.ReturnValue,
                            returnParam: interceptPointRunTimeMethod.InterceptMethodAttribute.ReturnValue
                        );

                    if(typeof(Task).IsAssignableFrom(interceptPointRunTimeMethod.MethodReturnType))
                        await ((Task)obj).ConfigureAwait(false);
                }
            }
        }
    }
}
