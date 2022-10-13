using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.Enums
{
    /// <summary>
    /// 解析模式 
    /// 主要用于单接口多实现的IocManager的Resolve<TService>(Type ImplementationType)指定明确能够解析出
    /// </summary>
    public enum ResolveMode
    {
        /// <summary>
        /// 根据Keyed解析
        /// </summary>
        [Description("根据Keyed解析")]
        Keyed,
        /// <summary>
        /// 根据Named解析
        /// </summary>
        [Description("根据Named解析")]
        Named,
        /// <summary>
        /// 根据自身解析
        /// </summary>
        [Description("根据自身解析")]
        Self,
    }
}
