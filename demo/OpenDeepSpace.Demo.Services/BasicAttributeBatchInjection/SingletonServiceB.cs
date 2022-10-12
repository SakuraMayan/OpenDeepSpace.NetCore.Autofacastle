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
    /// 单例服务ServiceA 值存在一个
    /// </summary>
    [Singleton(AsServices =new Type[] {typeof(ISingletonServiceB) },AutoActivate =true)]
    public class SingletonServiceB : ISingletonServiceB,IIdProvider
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public SingletonServiceB()
        {
            Console.WriteLine($"{nameof(SingletonServiceB)} 使用了AutoActivate 项目启动初始化");
        }

        public void Business()
        {
            Console.WriteLine($"{nameof(SingletonServiceB)}.{nameof(Business)} Id:{Id}");
        }
    }
}
