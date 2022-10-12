using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Interceptor.Attributes
{
    /// <summary>
    /// 方法执行完成之后执行 无论是否存在异常都会执行
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public abstract class MethodAfterAbstractInterceptAttribute : MethodInterceptBaseAttribute
    {
        /// <summary>
        /// 方法后之执行
        /// </summary>
        /// <param name="interceptContext">拦截上下文</param>
        /// <param name="result">实际方法执行后的结果</param>
        /// <returns></returns>
        public abstract Task After(InterceptContext interceptContext, object result);
    }
}
