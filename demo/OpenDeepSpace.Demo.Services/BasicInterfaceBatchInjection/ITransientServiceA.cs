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
    public interface ITransientServiceA
    {
        
        public void Business();

    }
}
