using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint.Attributes
{
    /// <summary>
    /// 方法后拦截 无论成功失败都会拦截
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MethodAfterInterceptAttribute : Attribute
    {

        /// <summary>
        /// 如果目标方法成功返回 那么就是返回的值 如果目标方法异常 那么就是异常本身
        /// </summary>
        public string ReturnValue { get; set; }
    }
}
