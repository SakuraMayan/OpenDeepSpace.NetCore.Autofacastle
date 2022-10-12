using OpenDeepSpace.Demo.Service.Aop;
using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Attributes;
using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.Demo.Services.BasicInterfaceBatchInjection
{
    [ClassIntercept]
    public class TransientServiceAClassIntercept : IDisposable,ITransient
    {

        [LogBefore]
        [LogAfter]
        [LogAfterReturn]
        [LogThrow]
        public virtual void BusinessException()
        {
            throw new Exception("类拦截虚方法业务异常了");
        }

        public void Dispose()
        {
            
        }
    }
}
