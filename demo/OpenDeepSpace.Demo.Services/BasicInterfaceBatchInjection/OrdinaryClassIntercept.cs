using OpenDeepSpace.Demo.Service.Aop;
using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.Demo.Services.BasicInterfaceBatchInjection
{
    public class OrdinaryClassIntercept:AbstractOrdinaryClassIntercept,ITransient
    {
        [LogBefore]
        public virtual void Business() 
        { 
        
        }

        [Log2Before]
        public override void BusinessOne()
        {
            
        }
    }

    public abstract class AbstractOrdinaryClassIntercept
    { 
        public abstract void BusinessOne();
    }
}
