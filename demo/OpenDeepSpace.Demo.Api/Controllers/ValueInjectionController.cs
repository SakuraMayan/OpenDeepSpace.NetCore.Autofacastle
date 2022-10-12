using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.Attributes;

namespace OpenDeepSpace.Demo.Api.Controllers
{
    /// <summary>
    /// 值注入控制器
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ValueInjectionController : ControllerBase
    {
        //不指定FileName 默认读取appsettings.json下的 基础数据类型 数组 kv 对象的注入
        //基础数据类型
        /*[ValueInjection("intValue")]
        private readonly int intValue;

        [ValueInjection("intValue")]
        private int intV { get; set; }

        [ValueInjection("doubleV")]
        public double doubleValue { get; set; }

        [ValueInjection("floatV")]
        public float floatValue { get; set; }

        [ValueInjection("boolV")]
        public bool boolValue { get; set; }

        //string
        [ValueInjection("strV")]
        public string stringValue { get; set; }

        //数组
        [ValueInjection("strValues")]
        private readonly string[] strValues;

        //kv dic
        [ValueInjection("dicV")]
        private readonly Dictionary<string, string> dictValues;

        //对象obj
        [ValueInjection("valueInjection")]
        private readonly ValueInjection valueInjection;*/

        //指定FileName 根据环境值来获取数据
        [ValueInjection("intValue", FileName = "appsettings.{env}.json")]
        private readonly int intValue;

        [ValueInjection("intValue", FileName = "appsettings.{env}.json")]
        private int intV { get; set; }

        [ValueInjection("doubleV", FileName = "appsettings.{env}.json")]
        public double doubleValue { get; set; }

        [ValueInjection("floatV", FileName = "appsettings.{env}.json")]
        public float floatValue { get; set; }

        [ValueInjection("boolV", FileName = "appsettings.{env}.json")]
        public bool boolValue { get; set; }

        //string
        [ValueInjection("strV", FileName = "appsettings.{env}.json")]
        public string stringValue { get; set; }

        //数组
        [ValueInjection("strValues", FileName = "appsettings.{env}.json")]
        private readonly string[] strValues;

        //kv dic
        [ValueInjection("dicV", FileName = "appsettings.{env}.json")]
        private readonly Dictionary<string, string> dictValues;

        //对象obj
        [ValueInjection("valueInjection", FileName = "appsettings.{env}.json")]
        private readonly ValueInjection valueInjection;

        //指定FileName 不同的文件类型 文件需要与appsettings文件设置为一样的文件属性 否则需要指定BasePath
        [ValueInjection("intValue", FileName = "appsettings.xml")]
        private readonly int intValueXml;

        [ValueInjection("intValue", FileName = "appsettings.ini")]
        private readonly int intValueIni;

        //指定FileName 以及设置基础路径
        //[ValueInjection("intValue", FileName = "appsettings.xml", BasePath = @"D:\Workspace\OpenDeepSpace\demo\OpenDeepSpace.Api")]
        //private readonly int intValueXmlx;

        [HttpGet]
        public void TestValueInjection()
        {
            Console.WriteLine($"json中内容绑定:{nameof(intValue)}:{intValue}");
            Console.WriteLine($"json中内容绑定:{nameof(intV)}:{intV}");
            Console.WriteLine($"json中内容绑定:{nameof(doubleValue)}:{doubleValue}");
            Console.WriteLine($"json中内容绑定:{nameof(stringValue)}:{stringValue}");
            Console.WriteLine($"json中内容绑定:{nameof(strValues)}:{string.Join(",",strValues)}");
            Console.WriteLine($"json中内容绑定:{nameof(dictValues)}:{dictValues.Count()}");
            Console.WriteLine($"json中内容绑定到简单对象:{nameof(valueInjection)}:{valueInjection.intValue}:{valueInjection.stringValue}");
            Console.WriteLine($"xml中内容绑定:{nameof(intValueXml)}:{intValueXml}");
            Console.WriteLine($"ini中内容绑定:{nameof(intValueIni)}:{intValueIni}");
        }

        public class ValueInjection
        {
            public int intValue { get; set; }

            //基础类型的值可以注入
            public string stringValue { get; set; }
            //这里值无法注入
            [ValueInjection("valueInjection:dictVV")]
            public Dictionary<string, string> dictV { get; set; }
        }
    }
}
