using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.Demo.Services.BasicInterfaceBatchInjection
{
    /// <summary>
    /// 一次请求范围的ServiceA接口
    /// </summary>
    public interface IScopedServiceA
    {
        public void Business();
    }
}
