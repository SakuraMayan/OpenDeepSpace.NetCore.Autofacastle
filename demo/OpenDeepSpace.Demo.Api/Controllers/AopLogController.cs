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

        private readonly ExternalService externalService;

        private readonly OrdinaryClassIntercept ordinaryClassIntercept;

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void Test()
        {
            transientServiceA.Business();
            transientServiceAClassIntercept.BusinessException();
        }

        [HttpGet]
        public void TestClassIntercept()
        {
            transientServiceAClassIntercept.BusinessException();
        }

        /// <summary>
        /// 测试不拦截
        /// </summary>
        [HttpGet]
        public void TestNonIntercept()
        {
            transientServiceA.Business();
        }

        /// <summary>
        /// 测试拦截点的拦截
        /// </summary>
        [HttpGet]
        public void TestInterceptPoint()
        { 
            transientServiceB.Business();
        }

        /// <summary>
        /// 测试外部服务的拦截
        /// </summary>
        [HttpGet]
        public void TestExternalServiceLog()
        { 
            externalService.Business();
        }

        /// <summary>
        /// 测试常规的类拦截
        /// </summary>
        [HttpGet]
        public void TestOrdinaryClassIntercept()
        { 
            ordinaryClassIntercept.Business();
            ordinaryClassIntercept.BusinessOne();
        }
    }
}
