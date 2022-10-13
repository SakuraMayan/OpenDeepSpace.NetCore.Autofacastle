using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Interceptor.Attributes
{
    /// <summary>
    /// 在方法执行完后抛出异常时拦截执行
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface)]
    public abstract class MethodAfterThrowingAbstractInterceptAttribute : MethodInterceptBaseAttribute
    {
        /// <summary>
        /// 异常的类型 根据下面的方法解析泛型
        /// </summary>
        public virtual Type ExceptionType { get; }

        /// <summary>
        /// 抛出异常时执行
        /// </summary>
        /// <param name="interceptContext"></param>
        /// <param name="exception"></param>
        public abstract Task AfterThrowing(InterceptContext interceptContext, Exception exception);
    }
}
