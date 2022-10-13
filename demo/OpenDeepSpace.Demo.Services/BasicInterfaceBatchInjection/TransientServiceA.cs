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
    /// <summary>
    /// 瞬时的TransientServiceA 每次产生一个新的实例
    /// </summary>
    public class TransientServiceA : ITransientServiceA,ITransient
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [NonIntercept]
        //[LogBefore]
        //[LogAfter]
        public void Business()
        {
            Console.WriteLine($"{nameof(TransientServiceA)}.{nameof(Business)} Id:{Id}");
        }
    }
}
