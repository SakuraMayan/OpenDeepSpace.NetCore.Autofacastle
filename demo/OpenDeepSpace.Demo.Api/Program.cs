using OpenDeepSpace.NetCore.Autofacastle.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddControllersAsServices();//AddControllersAsServices()将Controller作为服务 使用该句话才能在Controller中使用特性的方式完成依赖注入服务
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseAutofacastle();//使用Autofacastle

//未使用特性或接口注入的类，自己手动注入的类需要使用拦截可以采用如下方式

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
