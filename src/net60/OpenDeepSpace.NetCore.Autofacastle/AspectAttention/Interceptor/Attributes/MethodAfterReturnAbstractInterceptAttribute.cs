using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Interceptor.Attributes
{
    /// <summary>
    /// 方法正常执行无异常后 执行
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface)]
    public abstract class MethodAfterReturnAbstractInterceptAttribute : MethodInterceptBaseAttribute
    {
        /// <summary>
        /// 方法正常执行完成后执行
        /// </summary>
        /// <param name="interceptContext"></param>
        /// <param name="result"></param>
        public abstract Task AfterReturn(InterceptContext interceptContext, object result);
    }
}
