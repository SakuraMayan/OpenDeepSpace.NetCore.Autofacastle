using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.Attributes
{
    /// <summary>
    /// 一次请求范围内特性 每一次只生成一个实例从某个子容器中加载
    /// 使用了该特性将会以一次请求周期注入
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ScopedAttribute : DependencyInjectionAttribute
    {
        

    }
}
