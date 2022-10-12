using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint.Attributes
{
    /// <summary>
    /// 方法执行后无异常拦截
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MethodAfterReturnInterceptAttribute : Attribute
    {

        /// <summary>
        /// 返回的参数
        /// </summary>
        public string ReturnValue { get; set; }
    }
}
