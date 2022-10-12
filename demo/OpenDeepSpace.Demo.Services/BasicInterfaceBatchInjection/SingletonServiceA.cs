using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.Demo.Services.BasicInterfaceBatchInjection
{
    /// <summary>
    /// 单例服务ServiceA 值存在一个
    /// </summary>
    public class SingletonServiceA : ISingletonServiceA,ISingleton
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public void Business()
        {
            Console.WriteLine($"{nameof(SingletonServiceA)}.{nameof(Business)} Id:{Id}");
        }
    }
}
