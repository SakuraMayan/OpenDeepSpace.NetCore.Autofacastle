using OpenDeepSpace.NetCore.Autofacastle.AspectAttention;
using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Interceptor.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.Demo.Service.Aop
{
    public class Log2Before: MethodBeforeAbstractInterceptAttribute
    {

        public override Task Before(InterceptContext interceptorContext)
        {
            Console.WriteLine($"日志2方法前执行{interceptorContext.TargetMethod}");
            return Task.CompletedTask;
        }
    }
}
