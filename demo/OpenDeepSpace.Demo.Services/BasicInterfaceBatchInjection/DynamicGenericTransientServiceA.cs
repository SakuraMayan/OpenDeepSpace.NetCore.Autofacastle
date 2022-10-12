using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.Demo.Services.BasicInterfaceBatchInjection
{
    /// <summary>
    /// 动态泛型的瞬时服务A实现
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DynamicGenericTransientServiceA<T> : IDynamicGenericTransientServiceA<T>,ITransient
    {

        public T TValue { get; set; }

        public Guid Id { get; set; } = Guid.NewGuid();

        public void Business()
        {
            Console.WriteLine($"{nameof(DynamicGenericTransientServiceA<T>)}.{nameof(Business)} GenericT:{typeof(T)} Value:{TValue} Id:{Id}");
        }
    }

}
