using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint
{
    /// <summary>
    /// 拦截点运行时方法
    /// </summary>
    public class InterceptPointRunTimeMethod<T>
    {
        /// <summary>
        /// 拦截点下对应的拦截方法特性
        /// </summary>
        public T InterceptMethodAttribute { get; set; }

        /// <summary>
        /// 被拦截的实例
        /// </summary>
        public object Instance { get; set; }

        /// <summary>
        /// 被拦截的方法
        /// </summary>
        public MethodInfo MethodInfo { get; set; }

        /// <summary>
        /// 方法返回类型
        /// </summary>
        public Type MethodReturnType { get; set; }

        /// <summary>
        /// 方法参数
        /// </summary>
        public ParameterInfo[] MethodParameters { get; set; }
    }
}
