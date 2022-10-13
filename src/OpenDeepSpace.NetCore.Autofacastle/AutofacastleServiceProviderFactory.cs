using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using OpenDeepSpace.NetCore.Autofacastle.AspectAttention;
using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection;
using OpenDeepSpace.NetCore.Autofacastle.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle
{
    public class AutofacastleServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
        private readonly Action<ContainerBuilder>? _configurationAction;

        public AutofacastleServiceProviderFactory(Action<ContainerBuilder>? configurationAction = null,List<AutomaticInjectionSelector>? automaticInjectionSelectors=null,List<NonInterceptSelector>? nonInterceptSelectors=null,List<ClassInterceptSelector>? classInterceptSelectors=null)
        {
            _configurationAction = configurationAction ?? (builder => { });
            if (automaticInjectionSelectors != null && automaticInjectionSelectors.Any())
            { 
                AutofacastleCollection.AutomaticInjectionSelectors.AddRange(automaticInjectionSelectors);
            }
            if (nonInterceptSelectors != null && nonInterceptSelectors.Any())
            { 
                AutofacastleCollection.NonInterceptSelectors.AddRange(nonInterceptSelectors);
            }
            if (classInterceptSelectors != null && classInterceptSelectors.Any())
            { 
                AutofacastleCollection.ClassInterceptSelectors.AddRange(classInterceptSelectors);
            }
        }

        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            var builder = new ContainerBuilder();

            //批量注册
            services.BatchInjection();

            //服务替换
            services.ReplaceServices();

            //收集拦截点
            InterceptExtensions.CollectionInterceptPoint();
            //注入拦截点
            services.InjectionInterceptPoints();
            //收集被拦截的类集
            InterceptExtensions.CollectionInterceptedTypeInfo();
            //尝试注入没有被注入的被拦截类
            //services.TryInjectionInterceptedType();//这里有点问题 应该是所有被拦截的类都需要注入的容器中而不是自己指定
 
            builder.Populate(services);

            if (_configurationAction == null)
                throw new InvalidOperationException($"{nameof(_configurationAction)}为空");

            _configurationAction(builder);

            return builder;
        }

        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            if(containerBuilder==null)
                throw new ArgumentNullException(nameof(containerBuilder));
            return new AutofacServiceProvider(containerBuilder.Build());
        }
    }
}
