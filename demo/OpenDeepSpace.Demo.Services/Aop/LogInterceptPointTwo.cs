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
    //[InterceptPoint("intercept(* * * * (..))",Order =-1)]
    public class LogInterceptPointTwo:AbstractInterceptPoint
    {

        public override async Task Around(InterceptContext interceptContext, InterceptContextDelegate next)
        {
            Console.WriteLine("InterceptPoint环绕前LogTwo");
            await next(interceptContext);
            Console.WriteLine("InterceptPoint环绕后LogTwo");
        }

        public override void Before()
        {
            Console.WriteLine("InterceptPoint方法前LogTwo");

        }

        
        public override void After()
        {
            Console.WriteLine("InterceptPoint方法后无论是否正常返回LogTwo");

        }

        
        public override void AfterReturn(object value)
        {
            Console.WriteLine($"InterceptPoint方法执行正常返回{value}LogTwo");
        }

       
        public override void AfterThrowing(Exception exception)
        {
            Console.WriteLine($"InterceptPoint方法异常{exception}执行LogTwo");
        }
    }
}
