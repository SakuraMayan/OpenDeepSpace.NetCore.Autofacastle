using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint
{
    /// <summary>
    /// 抽象拦截点
    /// </summary>
    public abstract class AbstractInterceptPoint : IInterceptPoint
    {
        [MethodAfterIntercept]
        public virtual void After()
        {
            
        }

        [MethodAfterReturnIntercept(ReturnValue = "value")]
        public virtual void AfterReturn(object value)
        {
            
        }

        //[MethodAroundIntercept]
        //public async Task Around(InterceptContext interceptContext, InterceptContextDelegate next)
        //{

        //    await AroundBefore(interceptContext);

        //    await next(interceptContext);

        //    await AroundAfter(interceptContext);

        //}

        //public virtual async Task AroundAfter(InterceptContext interceptContext)
        //{
        //    await Task.CompletedTask;
        //}

        //public virtual async Task AroundBefore(InterceptContext interceptContext)
        //{
        //    await Task.CompletedTask;
        //}

        [MethodBeforeIntercept]
        public virtual void Before()
        {
            
        }

        [MethodAfterThrowingIntercept(Throwing = "exception")]
        public virtual void AfterThrowing(Exception exception)
        {
            
        }

        [MethodAroundIntercept]
        public virtual async Task Around(InterceptContext interceptContext, InterceptContextDelegate next)
        {
            await next(interceptContext);
        }
    }
}
