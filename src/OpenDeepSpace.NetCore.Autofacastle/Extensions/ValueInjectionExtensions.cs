using Autofac;
using Autofac.Core.Resolving.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.Extensions
{
    /// <summary>
    /// 值注入拓展
    /// </summary>
    public static class ValueInjectionExtensions
    {
        /// <summary>
        /// 是否需要自动注入值
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static bool IsNeedValueInjection(this MemberInfo memberInfo)
        {
            return IsNeedValueInjection(memberInfo, null);
        }

        /// <summary>
        /// 是否需要值自动注入
        /// </summary>
        /// <param name="parameterInfo"></param>
        /// <returns></returns>
        public static bool IsNeedValueInjection(this ParameterInfo parameterInfo)
        {
            return IsNeedValueInjection(null, parameterInfo);
        }

        /// <summary>
        /// 解析值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static object ResolveValue(this ResolveRequestContext context, MemberInfo memberInfo)
        {
            return ResolveValue(context, null, memberInfo, null);
        }

        /// <summary>
        /// 解析值
        /// </summary>
        /// <param name="componentContext"></param>
        /// <param name="parameterInfo"></param>
        /// <returns></returns>
        public static object ResolveValue(this IComponentContext componentContext, ParameterInfo parameterInfo)
        {
            return ResolveValue(null, componentContext, null, parameterInfo);
        }

        private static object ResolveValue(ResolveRequestContext? context, IComponentContext? componentContext, MemberInfo? memberInfo,ParameterInfo? parameterInfo)
        {
            //解析出当前环境
            IHostEnvironment? hostEnvironment = null;
            if(context!=null)
                hostEnvironment = context.Resolve<IHostEnvironment>();
            if (componentContext != null)
                hostEnvironment = componentContext.Resolve<IHostEnvironment>();
            //获取配置构建者
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

            ValueInjectionAttribute? valueAttr = null;
            if(memberInfo!=null)
                valueAttr = memberInfo.GetCustomAttribute<ValueInjectionAttribute>();
            if (parameterInfo != null)
                valueAttr = parameterInfo.GetCustomAttribute<ValueInjectionAttribute>();
            if (valueAttr == null)
                throw new InvalidOperationException($"{nameof(valueAttr)}为空");
            //设置文件基路径
            if (!string.IsNullOrWhiteSpace(valueAttr.BasePath))
                configurationBuilder.SetBasePath(valueAttr.BasePath);

            //如果Path为空 默认读取appsettings.json
            if (string.IsNullOrWhiteSpace(valueAttr.FileName))
                valueAttr.FileName = "appsettings.json";

            //如果Path不为空 判断是否包含环境
            if (valueAttr.FileName.Contains("{env}"))
            {
                if (hostEnvironment == null)
                    throw new InvalidOperationException($"{nameof(hostEnvironment)}为空");
                valueAttr.FileName = valueAttr.FileName.Replace("{env}", hostEnvironment.EnvironmentName);
            }

            IConfigurationRoot? configurationRoot = System.IO.Path.GetExtension(valueAttr.FileName).ToLower() switch
            {
                //json
                ".json" => configurationBuilder.AddJsonFile(valueAttr.FileName, true, true).Build(),
                //xml
                ".xml" => configurationBuilder.AddXmlFile(valueAttr.FileName, true, true).Build(),
                //ini
                ".ini" => configurationBuilder.AddIniFile(valueAttr.FileName, true, true).Build(),
                _ => throw new NotSupportedException($"不支持{System.IO.Path.GetExtension(valueAttr.FileName)}后缀的配置文件中值的注入"),
            };

            //根据类型从配置根获取值
            Type? memberType = null;
            if (memberInfo != null)
            {
                if ((memberInfo as PropertyInfo) != null)
                   memberType = (memberInfo as PropertyInfo)!.PropertyType;
                if((memberInfo as FieldInfo) != null)
                   memberType = (memberInfo as FieldInfo)!.FieldType;
            }

            if(parameterInfo!=null)
                memberType = parameterInfo.ParameterType;

            if (memberType == null)
                throw new InvalidOperationException($"{nameof(memberType)}为空");

            object value = configurationRoot.GetSection(valueAttr.Key).Get(memberType);

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="parameterInfo"></param>
        /// <returns></returns>
        private static bool IsNeedValueInjection(MemberInfo? memberInfo, ParameterInfo? parameterInfo)
        {
            ValueInjectionAttribute? valueAttr = null;
            if (memberInfo != null)
                valueAttr = memberInfo.GetCustomAttribute<ValueInjectionAttribute>();
            if (parameterInfo != null)
                valueAttr = parameterInfo.GetCustomAttribute<ValueInjectionAttribute>();
            if (valueAttr == null)//值特性为空
                return false;
            return true;
        }
    }
}
