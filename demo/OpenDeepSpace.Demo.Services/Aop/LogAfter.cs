using OpenDeepSpace.NetCore.Autofacastle.AspectAttention;
using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Interceptor.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.Demo.Service.Aop
{
    /// <summary>
    /// 方法执行后执行
    /// </summary>
    public class LogAfter : MethodAfterAbstractInterceptAttribute
    {
        public override async Task After(InterceptContext interceptContext, object result)
        {
            Console.WriteLine($"方法后无论是否异常都执行{interceptContext.TargetMethod},返回值{result}");
            await Task.CompletedTask;
        }
    }
}
