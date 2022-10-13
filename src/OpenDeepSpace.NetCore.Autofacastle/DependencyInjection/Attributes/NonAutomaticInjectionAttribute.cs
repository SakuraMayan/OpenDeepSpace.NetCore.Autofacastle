using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.Attributes
{
    /// <summary>
    /// 不自动注入特性 
    /// 使用在某个类或属性或字段不需要注入的时候 
    /// 一般是结合类上使用了<see cref="AutomaticInjectionAttribute"/>特性
    /// 或结合自动注入筛选器来使用 
    /// 使用了该特性表示类 属性 字段不会自动注入 使用在类上表示该类下面的所有属性以及字段都不会自动注入
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
    public class NonAutomaticInjectionAttribute : Attribute
    {
    }
}
