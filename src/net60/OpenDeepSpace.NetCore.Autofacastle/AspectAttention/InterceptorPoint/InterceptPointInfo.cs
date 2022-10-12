using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint
{
    /// <summary>
    /// 拦截点信息
    /// </summary>
    public class InterceptPointInfo
    {
        /// <summary>
        /// 拦截点类类型
        /// </summary>
        public Type InterceptPointType { get; set; }

        /// <summary>
        /// 拦截点
        /// </summary>
        public InterceptPointAttribute InterceptPoint { get; set; }

        /// <summary>
        /// 方法前拦截的方法
        /// </summary>
        public Tuple<MethodBeforeInterceptAttribute, MethodInfo> MethodBeforeIntercept { get; set; }

        /// <summary>
        /// 方法执行后无论是否正常 都拦截的方法
        /// </summary>
        public Tuple<MethodAfterInterceptAttribute, MethodInfo> MethodAfterIntercept { get; set; }

        /// <summary>
        /// 方法正常执行后拦截的方法
        /// </summary>
        public Tuple<MethodAfterReturnInterceptAttribute, MethodInfo> MethodAfterReturnIntercept { get; set; }

        /// <summary>
        /// 方法抛出异常后拦截的方法
        /// </summary>
        public Tuple<MethodAfterThrowingInterceptAttribute, MethodInfo> MethodAfterThrowingIntercept { get; set; }

        /// <summary>
        /// 环绕拦截的方法
        /// </summary>
        public Tuple<MethodAroundInterceptAttribute, MethodInfo> MethodAroundIntercept { get; set; }
    }
}
