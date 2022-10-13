using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention
{
    /// <summary>
    /// 不拦截的筛选器
    /// 可用于满足断言条件不需要使用<see cref="Attributes.NonInterceptAttribute"/>特性即可完成不拦截
    /// </summary>
    public class NonInterceptSelector : NamedTypeSelector
    {
        public NonInterceptSelector(Func<Type, bool> predicate) : base(predicate)
        {
        }

        public NonInterceptSelector(string name, Func<Type, bool> predicate) : base(name, predicate)
        {
        }
    }
}
