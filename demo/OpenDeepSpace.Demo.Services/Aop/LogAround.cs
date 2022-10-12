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
    /// 环绕方法执行
    /// </summary>
    public class LogAround : MethodAroundAbstractInterceptAttribute
    {

        public override async Task Around(InterceptContext interceptContext, InterceptContextDelegate next)
        {
            Console.WriteLine($"环绕前执行{interceptContext.TargetMethod}");

            await next(interceptContext);

            //出现异常这里将不会执行 会去执行AfterThrow
            Console.WriteLine($"环绕后执行{interceptContext.TargetMethod}");
        }
    }
}
