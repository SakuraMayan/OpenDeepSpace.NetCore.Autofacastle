using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Attributes
{
    /// <summary>
    /// 类拦截使用在类上
    /// 主要辅助判断类拦截 
    /// 比如一个类实现了其他接口 比如IDispose接口
    /// 实际还是以类的方式拦截的情况下 即该类中的方法并不是实现自的接口
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ClassInterceptAttribute : Attribute
    {
    }
}
