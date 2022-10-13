using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.Demo.Services.BasicInterfaceBatchInjection
{
    public class ExternalService
    {
        public virtual void Business()
        {
            Console.WriteLine($"{nameof(ExternalService)}.{nameof(Business)}");
        }
    }
}
