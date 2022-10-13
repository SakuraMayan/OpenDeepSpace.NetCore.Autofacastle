using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using OpenDeepSpace.Demo.Api.Filters;
using OpenDeepSpace.Demo.Services.BasicAttributeBatchInjection;
using OpenDeepSpace.Demo.Services.BasicInterfaceBatchInjection;
using OpenDeepSpace.NetCore.Autofacastle;
using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection;
using OpenDeepSpace.NetCore.Autofacastle.Extensions;
using OpenDeepSpace.NetCore.Autofacastle.Reflection;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddControllersAsServices();//AddControllersAsServices()将Controller作为服务 使用该句话才能在Controller中使用特性的方式完成依赖注入服务
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/*builder.Host.UseAutofacastle(automaticInjectionSelectors:new List<OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.AutomaticInjectionSelector>() { 

    new OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.AutomaticInjectionSelector(t=>t.BaseType==typeof(ControllerBase))
        
});//使用Autofacastle*/

/*builder.Host.UseAutofacastle(classInterceptSelectors: new List<OpenDeepSpace.NetCore.Autofacastle.AspectAttention.ClassInterceptSelector>()
{
    new OpenDeepSpace.NetCore.Autofacastle.AspectAttention.ClassInterceptSelector(t=>t.GetInterfaces().Any(t=>t==typeof(ITransientServiceA)))
});*/

/*builder.Host.UseAutofacastle(automaticInjectionSelectors: new List<OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.AutomaticInjectionSelector>() {

    new OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.AutomaticInjectionSelector(t=>t.BaseType==typeof(ITransientServiceB))

});*/

//builder.Host.UseAutofacastle();

//
builder.Host.ConfigureContainer<ContainerBuilder>(container =>
{

    //自己传入程序集
    var assemblies = AssemblyFinder.GetAllAssemblies().Where(assembly => !assembly.FullName.StartsWith("Microsoft") && !assembly.FullName.StartsWith("System"));
    container.UseAutofacastle(assemblies: assemblies.ToList(), IsConfigureIntercept: true);
    //外部手动注入服务实例的添加拦截
    container.RegisterType(typeof(ExternalService)).AddIntercept(typeof(ExternalService));

}).UseServiceProviderFactory(new AutofacastleServiceProviderFactory());

builder.Services.AddMvcCore(op => {

    //op.Filters.Add<IocManagerFilter>();
});

//未使用特性或接口注入的类，自己手动注入的类需要使用拦截可以采用如下方式

var app = builder.Build();

//(app.Services as AutofacServiceProvider).LifetimeScope
//app.Services.GetAutofacRoot()
//使用IocManager
IocManager.InitContainer(app.Services.GetAutofacRoot());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseAuthorization();

app.MapControllers();

app.Run();
