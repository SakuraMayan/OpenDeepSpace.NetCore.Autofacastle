using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Interceptor.Attributes
{
    /// <summary>
    /// 方法执行前后执行 环绕执行
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public abstract class MethodAroundAbstractInterceptAttribute : MethodInterceptBaseAttribute
    {
        ///// <summary>
        ///// 环绕前
        ///// </summary>
        ///// <param name="interceptContext"></param>
        ///// <returns></returns>
        //public abstract Task AroundBefore(InterceptContext interceptContext);

        ///// <summary>
        ///// 环绕后
        ///// </summary>
        ///// <param name="interceptContext"></param>
        ///// <returns></returns>
        //public abstract Task AroundAfter(InterceptContext interceptContext);

        ///// <summary>
        ///// 方法执行前后执行
        ///// </summary>
        ///// <param name="interceptContext">拦截上下文</param>
        ///// <param name="next">下一个拦截器 最后一个是执行被拦截的方法</param>
        ///// <returns></returns>
        //public async Task Around(InterceptContext interceptContext, InterceptContextDelegate next)
        //{
        //    await AroundBefore(interceptContext);
        //    await next(interceptContext);
        //    await AroundAfter(interceptContext);
        //}

        /// <summary>
        /// 方法执行前后执行
        /// </summary>
        /// <param name="interceptContext">拦截上下文</param>
        /// <param name="next">下一个拦截器 最后一个是执行被拦截的方法</param>
        /// <returns></returns>
        public abstract Task Around(InterceptContext interceptContext, InterceptContextDelegate next);
    }
}
