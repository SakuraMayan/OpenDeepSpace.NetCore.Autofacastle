using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Attributes
{
    /// <summary>
    /// 不需要被拦截的类上加上该特性
    /// 主要配置InterceptPoint拦截点设置时 某些类确实不需要代理出现异常的控制
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class NonInterceptAttribute : Attribute
    {
    }
}
