using Microsoft.AspNetCore.Mvc.Filters;
using OpenDeepSpace.Demo.Services.BasicAttributeBatchInjection;
using OpenDeepSpace.Demo.Services.BasicInterfaceBatchInjection;
using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection;
using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.Enums;

namespace OpenDeepSpace.Demo.Api.Filters
{
    /// <summary>
    /// 
    /// </summary>
    public class IocManagerFilter:IAsyncActionFilter
    {

            private IServiceProvider sp => IocManager.Resolve<IServiceProvider>();

            private IScopedServiceB scopedServiceB => IocManager.Resolve<IScopedServiceB>();

            //多实现的测试IocManager
            private ITransientServiceA transientServiceA => IocManager.TryResolve<ITransientServiceA>(typeof(TransientServiceA));
            private ITransientServiceA transientServiceATwo => IocManager.Resolve<ITransientServiceA>(typeof(TransientServiceA), ResolveMode.Self);
            private ITransientServiceA transientServiceATrip => IocManager.Resolve<ITransientServiceA>(typeof(TransientServiceA).FullName);
            private ITransientServiceA transientServiceAFour => IocManager.Resolve<ITransientServiceA>(Keyed: typeof(TransientServiceA));

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var serviceProvider = context.HttpContext.RequestServices;

                if (sp != null)
                    Console.WriteLine($"{nameof(sp)}不为空，获取成功");
                transientServiceA.Business();
                transientServiceATwo.Business();
                transientServiceATrip.Business();
                transientServiceAFour.Business();


                await next();

                await Task.CompletedTask;
            }

    }
}
