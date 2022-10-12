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
    /// 一次请求范围内产生一个实例
    /// </summary>
    [Scoped]
    public class ScopedServiceB : IScopedServiceB
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public void Business()
        {
            Console.WriteLine($"{nameof(ScopedServiceB)}.{nameof(Business)} Id:{Id}");
        }
    }
}
