using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention
{
    /// <summary>
    /// 拦截上下文代理
    /// </summary>
    /// <param name="interceptContext"></param>
    /// <returns></returns>
    public delegate Task InterceptContextDelegate(InterceptContext interceptContext);
}
