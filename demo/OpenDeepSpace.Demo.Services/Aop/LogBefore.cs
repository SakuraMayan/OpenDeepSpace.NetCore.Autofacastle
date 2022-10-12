

using OpenDeepSpace.NetCore.Autofacastle.AspectAttention;
using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Interceptor.Attributes;

namespace OpenDeepSpace.Demo.Service.Aop
{
    /// <summary>
    /// 方法调用前记录日志
    /// </summary>
    public class LogBefore : MethodBeforeAbstractInterceptAttribute
    {
        public override Task Before(InterceptContext interceptorContext)
        {
            Console.WriteLine($"方法前执行{interceptorContext.TargetMethod}");
            return Task.CompletedTask;
        }
    }
}
