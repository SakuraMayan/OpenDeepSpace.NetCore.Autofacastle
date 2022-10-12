using OpenDeepSpace.NetCore.Autofacastle.AspectAttention;
using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle
{
    /// <summary>
    /// Autofacastle集
    /// </summary>
    internal class AutofacastleCollection
    {
        /// <summary>
        /// 自动注入筛选器
        /// </summary>
        public static List<AutomaticInjectionSelector> AutomaticInjectionSelectors { get; set; } = new();

        /// <summary>
        /// 预加载单例集
        /// </summary>
        public static List<Type> AutoActives { get; set; } = new List<Type>();

        /// <summary>
        /// 使用Keyed注入
        /// </summary>
        public static Dictionary<object, Type> KeyedImplementationTypes = new();

        /// <summary>
        /// 使用Named注入
        /// </summary>
        public static Dictionary<string, Type> NamedImplementationTypes = new();

        /// <summary>
        /// 被拦截的类信息集合
        /// </summary>
        public static List<InterceptedTypeInfo> InterceptedTypeInfos = new();

        /// <summary>
        /// 不拦截的筛选器
        /// </summary>
        public static List<NonInterceptSelector> NonInterceptSelectors { get; set; } = new();

        /// <summary>
        /// 类拦截筛选器
        /// </summary>
        public static List<ClassInterceptSelector> ClassInterceptSelectors { get; set; } = new();
    }
}
