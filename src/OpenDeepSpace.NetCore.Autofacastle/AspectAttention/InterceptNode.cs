using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention
{
    /// <summary>
    /// 拦截节点
    /// </summary>
    public class InterceptNode
    {
        /// <summary>
        /// 下一个
        /// </summary>
        public InterceptContextDelegate Next { get; set; }

        /// <summary>
        /// 执行
        /// </summary>
        public InterceptContextDelegate Process { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Func<InterceptContextDelegate, InterceptContextDelegate> Node { get; set; }
    }
}
