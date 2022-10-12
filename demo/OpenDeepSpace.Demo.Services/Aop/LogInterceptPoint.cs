using OpenDeepSpace.NetCore.Autofacastle.AspectAttention;
using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint;
using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.Demo.Service.Aop
{
    /// <summary>
    /// 日志拦截点
    /// </summary>
    [InterceptPoint("intercept(* * * * (..))")]
    public class LogInterceptPoint:AbstractInterceptPoint
    {

        public override async Task Around(InterceptContext interceptContext, InterceptContextDelegate next)
        {
            Console.WriteLine("InterceptPoint环绕前");
            await next(interceptContext);
            Console.WriteLine("InterceptPoint环绕后");
        }

        public override void Before()
        {
            Console.WriteLine("InterceptPoint方法前");

        }

        public override void After()
        {
            Console.WriteLine("InterceptPoint方法后无论是否正常返回");

        }

        public override void AfterReturn(object value)
        {
            Console.WriteLine($"InterceptPoint方法执行正常返回{value}");
        }

        public override void AfterThrowing(Exception exception)
        {
            Console.WriteLine("InterceptPoint方法异常执行");
        }
    }
}
