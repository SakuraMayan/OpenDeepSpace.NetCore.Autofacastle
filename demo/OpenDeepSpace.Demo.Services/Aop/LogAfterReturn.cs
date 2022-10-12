using OpenDeepSpace.NetCore.Autofacastle.AspectAttention;
using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Interceptor.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.Demo.Service.Aop
{
    public class LogAfterReturn : MethodAfterReturnAbstractInterceptAttribute
    {
        public override async Task AfterReturn(InterceptContext interceptContext, object result)
        {
            Console.WriteLine($"方法正常执行后:{interceptContext.TargetMethod},{result}");

            await Task.CompletedTask;
        }
    }
}
