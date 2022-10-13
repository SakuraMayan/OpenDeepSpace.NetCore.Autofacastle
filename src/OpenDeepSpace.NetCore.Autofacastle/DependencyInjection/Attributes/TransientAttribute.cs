using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.Attributes
{
    /// <summary>
    /// 瞬时注入 每一次都产生新的实例
    /// 使用了该特性将以瞬时周期注入
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TransientAttribute : DependencyInjectionAttribute
    {
        

    }
}
