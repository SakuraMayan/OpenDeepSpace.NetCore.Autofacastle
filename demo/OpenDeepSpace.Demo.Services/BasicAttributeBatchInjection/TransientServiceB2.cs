using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.Demo.Services.BasicAttributeBatchInjection
{
    /// <summary>
    /// 瞬时ServiceB2 用于举例单接口多实现 以及服务替换举例
    /// </summary>
    [Transient(ReplaceServices = new Type[] { typeof(ITransientServiceB)},Keyed ="TB2")]
    public class TransientServiceB2 : ITransientServiceB
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public void Business()
        {
            Console.WriteLine($"{nameof(TransientServiceB2)}.{nameof(Business)} Id:{Id}");
        }
    }
}
