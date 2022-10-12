using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.Demo.Services.BasicInterfaceBatchInjection
{
    /// <summary>
    /// 一次请求范围内产生一个实例
    /// </summary>
    public class ScopedServiceA : IScopedServiceA,IScoped
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public void Business()
        {
            Console.WriteLine($"{nameof(ScopedServiceA)}.{nameof(Business)} Id:{Id}");
        }
    }
}
