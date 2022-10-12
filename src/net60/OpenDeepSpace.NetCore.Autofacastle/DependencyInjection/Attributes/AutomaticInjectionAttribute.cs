using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.Attributes
{
    /// <summary>
    /// 自动注入特性
    /// 该特性使用在类或属性或字段上 
    /// 使用了该特性的类 属性 字段都会自动注入 使用在类上表示该类下面的所有属性以及字段都会自动注入
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
    public class AutomaticInjectionAttribute : Attribute
    {
        /// <summary>
        /// 实现类 指定注入的实现类 用于单接口多实现的情况
        /// </summary>
        public Type? ImplementationType { get; set; }

        //为了兼容
        /// <summary>
        /// 指定Keyed
        /// </summary>
        public object? Keyed { get; set; }

        /// <summary>
        /// 指定Named
        /// </summary>
        public string? Named { get; set; } 

    }
}
