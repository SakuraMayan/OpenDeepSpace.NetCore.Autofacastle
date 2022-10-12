using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.Demo.Services.BasicInterfaceBatchInjection
{

    /// <summary>
    /// 动态泛型的瞬时ServiceA
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDynamicGenericTransientServiceA<T>
    {
        public T TValue { get; set; }
        public void Business();
    }
}
