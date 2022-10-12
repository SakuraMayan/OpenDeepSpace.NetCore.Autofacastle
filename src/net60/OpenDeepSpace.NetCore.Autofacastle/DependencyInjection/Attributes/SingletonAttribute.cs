using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.Attributes
{
    /// <summary>
    /// 单例特性 
    /// 使用了该特性将会以单例方式注入
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SingletonAttribute : DependencyInjectionAttribute
    {
        
        /// <summary>
        /// 自动加载预加载
        /// </summary>
        public bool AutoActivate { get; set; }

    }
}
