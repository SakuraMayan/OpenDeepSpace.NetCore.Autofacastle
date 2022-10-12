using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.Demo.Services.BasicAttributeBatchInjection
{
    /// <summary>
    /// 一次请求范围的ServiceB接口
    /// </summary>
    public interface IScopedServiceB
    {
        public void Business();
    }
}
