using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint.Attributes
{
    /// <summary>
    /// 方法前后执行
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MethodAroundInterceptAttribute : Attribute
    {
    }
}
