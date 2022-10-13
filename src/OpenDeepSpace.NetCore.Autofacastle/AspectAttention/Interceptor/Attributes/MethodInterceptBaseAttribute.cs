using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Interceptor.Attributes
{
    /// <summary>
    /// 方法拦截基类特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MethodInterceptBaseAttribute : Attribute
    {
        /// <summary>
        /// 拦截优先级序号 order越小优先级越高
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 分组组名 默认不写为空 
        /// </summary>
        public string GroupName { get; set; } = "";
    }
}
