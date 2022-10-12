using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint
{
    /// <summary>
    /// 拦截点接口
    /// </summary>
    public interface IInterceptPoint
    {
        
        Task Around(InterceptContext interceptContext, InterceptContextDelegate next);

        //Task AroundBefore(InterceptContext interceptContext);

        //Task AroundAfter(InterceptContext interceptContext);


        void Before();


        
        void After();


        
        void AfterReturn(object value);


        
        void AfterThrowing(Exception exception);
        
    }
}
