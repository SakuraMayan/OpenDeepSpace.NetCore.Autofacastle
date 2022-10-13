using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint
{
    /// <summary>
    /// 拦截点收集
    /// </summary>
    public class InterceptPointCollection
    {

        /// <summary>
        /// 所有拦截点信息集合
        /// </summary>
        public static List<InterceptPointInfo> interceptPointInfos { get; set; } = new List<InterceptPointInfo>();

        /// <summary>
        /// 非泛型 目标类目标方法对应的拦截点集合 kv
        /// 一个方法可能拥有多个拦截点
        /// </summary>
        public static ConcurrentDictionary<ObjectKey, List<InterceptPointInfo>> targetMethodInterceptPointList { get; set; } = new ConcurrentDictionary<ObjectKey, List<InterceptPointInfo>>();

        /// <summary>
        /// 针对动态泛型类的method目标集合
        /// </summary>
        public static ConcurrentDictionary<string, List<InterceptPointInfo>> targetDynamicTMethodInfoList { get; set; } = new ConcurrentDictionary<string, List<InterceptPointInfo>>();

        /// <summary>
        /// 拦截点拦截类的集合
        /// </summary>
        public static List<Type> InterceptPointInterceptClassList { get; set; } = new List<Type>();

    }

    /// <summary>
    /// 对象Key 由目标类和方法信息组成 用于非泛型类
    /// </summary>
    public class ObjectKey
    {
        public ObjectKey(Type type, MethodInfo method)
        {
            this.Type = type;
            this.Method = method;
        }

        public Type Type { get; set; }
        public MethodInfo Method { get; set; }

        public override int GetHashCode()
        {
            return this.Type.GetHashCode() + this.Method.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var item = obj as ObjectKey;
            if (item == null)
            {
                return false;
            }

            return this.Type == item.Type && this.Method.Name == item.Method.Name;
        }
    }
}
