using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenDeepSpace.Demo.Services.BasicAttributeBatchInjection;
using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.Attributes;

namespace OpenDeepSpace.Demo.Api.Controllers
{
    /// <summary>
    /// 基于特性的批量注入控制器
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AutomaticInjection]
    public class BasicAttributeBatchInjectionController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="singletonServiceB"></param>
        public BasicAttributeBatchInjectionController(ISingletonServiceB singletonServiceB)
        {
            this.singletonServiceB = singletonServiceB;
        }

        [AutomaticInjection(Named ="TB")]
        public ITransientServiceB transientServiceB { get; set; }

        public  IScopedServiceB scopedServiceB { get; set; }

        public IScopedServiceB scopedServiceBB { get; set; }

        public ISingletonServiceB singletonServiceB { get; set; }

        [HttpGet]
        public void Test()
        {
            transientServiceB.Business();
            scopedServiceB.Business();
            scopedServiceBB.Business(); 
            singletonServiceB.Business();
        }

    }
}
