using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Enums
{
    /// <summary>
    /// 拦截类型
    /// </summary>
    public enum InterceptType
    {
        /// <summary>
        /// 类拦截 virtual修饰的方法能被拦截
        /// </summary>
        [Description("类拦截")]
        ClassIntercept,

        /// <summary>
        /// 接口拦截 实现了接口的方法能被拦截
        /// </summary>
        [Description("接口拦截")]
        InterfaceIntercept
    }
}
