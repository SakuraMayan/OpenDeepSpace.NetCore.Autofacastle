using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention
{
    /// <summary>
    /// 被拦截的类型信息
    /// </summary>
    public class InterceptedTypeInfo
    {
        public InterceptedTypeInfo(Type interceptedType, bool isAbstractIntercept, bool isInterceptPointAbstractIntercept, InterceptType interceptType)
        {
            InterceptedType = interceptedType;
            IsAbstractIntercept = isAbstractIntercept;
            IsInterceptPointAbstractIntercept = isInterceptPointAbstractIntercept;
            InterceptType = interceptType;
        }

        /// <summary>
        /// 被拦截的类的类型
        /// </summary>
        public Type InterceptedType { get; set; }

        /// <summary>
        /// 是否被AbstractIntercept拦截
        /// </summary>
        public bool IsAbstractIntercept { get; set; }

        /// <summary>
        /// 是否被InterceptPointAbstractIntercept拦截
        /// </summary>
        public bool IsInterceptPointAbstractIntercept { get; set; }

        /// <summary>
        /// 拦截类型
        /// </summary>
        public InterceptType InterceptType { get; set; } = InterceptType.ClassIntercept;
    }
}
