using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention
{
    /// <summary>
    /// 拦截处理接口
    /// </summary>
    public interface IInterceptHandle
    {
        Task OnInvocation(InterceptContext interceptContext, InterceptContextDelegate next);
    }
}
