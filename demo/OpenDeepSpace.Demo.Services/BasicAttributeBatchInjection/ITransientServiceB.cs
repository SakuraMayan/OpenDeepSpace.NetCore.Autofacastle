using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.Demo.Services.BasicAttributeBatchInjection
{
    /// <summary>
    /// 瞬时的ServiceB接口
    /// </summary>
    public interface ITransientServiceB
    {
        public void Business();
    }
}
