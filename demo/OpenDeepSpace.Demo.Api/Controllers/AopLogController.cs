using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenDeepSpace.Demo.Services.BasicAttributeBatchInjection;
using OpenDeepSpace.Demo.Services.BasicInterfaceBatchInjection;
using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.Attributes;

namespace OpenDeepSpace.Demo.Api.Controllers
{
    /// <summary>
    /// Aop日志控制器
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AutomaticInjection]
    public class AopLogController : ControllerBase
    {
        private readonly ITransientServiceA transientServiceA;

        private readonly TransientServiceAClassIntercept transientServiceAClassIntercept;

        private readonly ITransientServiceB transientServiceB;

        [HttpGet]
        public void Test()
        {
            transientServiceA.Business();
            transientServiceAClassIntercept.BusinessException();
        }

        [HttpGet]
        public void TestInterceptPoint()
        { 
            transientServiceB.Business();
        }
    }
}
