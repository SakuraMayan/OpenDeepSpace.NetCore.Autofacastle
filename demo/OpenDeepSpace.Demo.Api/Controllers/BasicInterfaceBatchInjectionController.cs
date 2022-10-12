using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenDeepSpace.Demo.Services.BasicInterfaceBatchInjection;
using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.Attributes;

namespace OpenDeepSpace.Demo.Api.Controllers
{
    /// <summary>
    /// 基于接口的批量注入测试控制器
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BasicInterfaceBatchInjectionController : ControllerBase
    {

        [AutomaticInjection]
        public ITransientServiceA TransientServiceA { get; set; }

        [AutomaticInjection]
        private readonly IScopedServiceA ScopedServiceA;

        [AutomaticInjection]
        private readonly Lazy<IScopedServiceA> ScopedServiceAA;//懒加载

        /// <summary>
        /// 
        /// </summary>
        /// <param name="singletonServiceA"></param>
        public BasicInterfaceBatchInjectionController(ISingletonServiceA singletonServiceA)
        {
            SingletonServiceA = singletonServiceA;
        }

        private ISingletonServiceA SingletonServiceA { get; set; }

        [AutomaticInjection]
        private IDynamicGenericTransientServiceA<int> dynamicGenericTransientServiceA {get;set;}

        

        [HttpGet]
        public void Test()
        {
            TransientServiceA.Business();
            ScopedServiceA.Business();
            ScopedServiceAA.Value.Business();
            SingletonServiceA.Business();

            dynamicGenericTransientServiceA.TValue = 20;
            dynamicGenericTransientServiceA.Business();
        }

    }
}
