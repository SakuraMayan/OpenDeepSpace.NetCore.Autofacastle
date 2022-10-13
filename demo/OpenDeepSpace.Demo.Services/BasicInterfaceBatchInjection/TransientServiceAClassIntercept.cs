using OpenDeepSpace.Demo.Service.Aop;
using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Attributes;
using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.Demo.Services.BasicInterfaceBatchInjection
{
    /// <summary>
    /// 实现抽象类里面的方法以类拦截方式拦截的也可被拦截
    /// </summary>
    [ClassIntercept]
    public class TransientServiceAClassIntercept : AbstractTransientServiceAClassIntercept, IDisposable,ITransient
    {

        /*//拦截标在虚方法上
        [LogBefore]
        [LogAfter]
        [LogAfterReturn]
        [LogThrow]
        public virtual void Business()
        { 
        
        }*/

        /*[LogBefore]
        [LogAfter]
        [LogAfterReturn]
        [LogThrow]*/
        public override void BusinessException()
        {
            throw new Exception("类拦截虚方法业务异常了");
        }

        public void Dispose()
        {
            
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class AbstractTransientServiceAClassIntercept
    {
        //拦截标在抽象类的抽象方法或虚方法上 实现该抽象类的类 也会被这些拦截特性所拦截到
        [LogBefore]
        [LogAfter]
        [LogAfterReturn]
        [LogThrow]
        public abstract void BusinessException();
    }
}
