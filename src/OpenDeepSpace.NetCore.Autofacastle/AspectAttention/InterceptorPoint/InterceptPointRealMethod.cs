using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint
{
    /// <summary>
    /// 拦截点实际对应的方法
    /// </summary>
    public class InterceptPointRealMethod
    {
        /// <summary>
        /// 
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 方法前拦截
        /// </summary>
        public InterceptPointRunTimeMethod<MethodBeforeInterceptAttribute> BeforeInterceptMethod { get; set; }

        /// <summary>
        /// 方法后拦截 无论正常与否
        /// </summary>
        public InterceptPointRunTimeMethod<MethodAfterInterceptAttribute> AfterInterceptMethod { get; set; }


        /// <summary>
        /// 方法正常返回后拦截
        /// </summary>
        public InterceptPointRunTimeMethod<MethodAfterReturnInterceptAttribute> AfterReturnInterceptMethod { get; set; }


        /// <summary>
        /// 方法出异常后拦截
        /// </summary>
        public InterceptPointRunTimeMethod<MethodAfterThrowingInterceptAttribute> AfterThrowingInterceptMethod { get; set; }


        /// <summary>
        /// 方法前后拦截
        /// </summary>
        public InterceptPointRunTimeMethod<MethodAroundInterceptAttribute> AroundInterceptMethod { get; set; }
    }
}
