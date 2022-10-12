using OpenDeepSpace.NetCore.Autofacastle.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddControllersAsServices();//AddControllersAsServices()��Controller��Ϊ���� ʹ�øþ仰������Controller��ʹ�����Եķ�ʽ�������ע�����
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseAutofacastle();//ʹ��Autofacastle

//δʹ�����Ի�ӿ�ע����࣬�Լ��ֶ�ע�������Ҫʹ�����ؿ��Բ������·�ʽ

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
