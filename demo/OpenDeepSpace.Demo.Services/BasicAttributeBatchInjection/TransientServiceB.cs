using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection;
using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.Demo.Services.BasicAttributeBatchInjection
{
    /// <summary>
    /// 瞬时的TransientServiceA 每次产生一个新的实例
    /// </summary>
    [Transient(Named ="TB")]
    public class TransientServiceB : ITransientServiceB
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public void Business()
        {
            Console.WriteLine($"{nameof(TransientServiceB)}.{nameof(Business)} Id:{Id}");
        }
    }
}
