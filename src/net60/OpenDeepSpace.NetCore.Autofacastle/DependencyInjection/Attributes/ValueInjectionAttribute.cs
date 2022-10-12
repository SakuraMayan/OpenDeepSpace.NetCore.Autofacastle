using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.Attributes
{
    /// <summary>
    /// 值特性注入
    /// 使用该特性将从相应路径位置下的JSON/XML/INI配置文件中注入值
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ValueInjectionAttribute : Attribute
    {
        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 指定文件所在基路径
        /// 默认为空 直接从入口程序下查找
        /// </summary>
        public string? BasePath { get; set; }

        /// <summary>
        /// 文件名称 默认从appsettings.json下查找
        /// </summary>
        public string FileName { get; set; } = "appsettings.json";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        public ValueInjectionAttribute(string Key)
        {
            this.Key = Key;
        }

    }
}
