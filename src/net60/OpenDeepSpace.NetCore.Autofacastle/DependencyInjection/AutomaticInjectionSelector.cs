using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.DependencyInjection
{
    /// <summary>
    /// 自动注入筛选器 
    /// 可用于指定满足条件的类不再需要使用<see cref="Attributes.AutomaticInjectionAttribute"/>特性直接就可以完成属性以及值的注入
    /// </summary>
    public class AutomaticInjectionSelector : NamedTypeSelector
    {
        public AutomaticInjectionSelector(Func<Type, bool> predicate) : base(predicate)
        {
        }

        public AutomaticInjectionSelector(string name, Func<Type, bool> predicate) : base(name, predicate)
        {
        }
        
    }
}
