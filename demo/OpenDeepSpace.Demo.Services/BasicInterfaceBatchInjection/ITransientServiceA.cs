using OpenDeepSpace.Demo.Service.Aop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.Demo.Services.BasicInterfaceBatchInjection
{
    /// <summary>
    /// 瞬时的ServiceA接口
    /// </summary>
    [Log2Before(GroupName = "第二组日志")]
    public interface ITransientServiceA
    {
        
        [LogAfterReturn]
        [LogThrow]
        public void Business();

    }
}
