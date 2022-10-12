using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.Demo.Services.BasicAttributeBatchInjection
{
    /// <summary>
    /// Id提供者 用于模拟一个实例实现多个接口 然后指定作为某个接口的注入
    /// </summary>
    public interface IIdProvider
    {
        public Guid Id { get; set; }
    }
}
