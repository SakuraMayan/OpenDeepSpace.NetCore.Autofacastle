using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Interceptor.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Interceptor
{
    /// <summary>
    /// 拦截面
    /// </summary>
    public class InterceptSurface
    {

        /// <summary>
        /// 排序 每个拦截面的序号需要与拦截特性相同 
        /// 即每一个拦截面的各个拦截特性的序号应该是相同的
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 分组
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 方法前拦截特性
        /// </summary>
        public MethodBeforeAbstractInterceptAttribute MethodBeforeIntercept { get; set; }

        /// <summary>
        /// 方法执行完成后不论是否抛出异常
        /// </summary>
        public MethodAfterAbstractInterceptAttribute MethodAfterIntercept { get; set; }

        /// <summary>
        /// 环绕执行
        /// </summary>
        public MethodAroundAbstractInterceptAttribute MethodAroundIntercept { get; set; }


        //以下两个特性 分别对应方法正常 出现异常后各自的方法后拦截

        /// <summary>
        /// 方法执行正常后执行 可以在MethodAfterIntercept后执行
        /// </summary>
        public MethodAfterReturnAbstractInterceptAttribute MethodAfterReturnIntercept { get; set; }

        /// <summary>
        /// 方法执行出现异常后 可以在MethodAfterIntercept后执行
        /// </summary>
        public MethodAfterThrowingAbstractInterceptAttribute MethodAfterThrowingIntercept { get; set; }
    }
}
