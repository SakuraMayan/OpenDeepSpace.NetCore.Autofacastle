using OpenDeepSpace.NetCore.Autofacastle.AspectAttention;
using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Attributes;
using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Interceptor.Attributes;
using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint;
using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint.Attributes;
using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.Attributes;
using OpenDeepSpace.NetCore.Autofacastle.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.Extensions
{
    /// <summary>
    /// 拦截拓展
    /// </summary>
    public static class InterceptExtensions
    {

        /// <summary>
        /// 筛选出拦截特性
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static IEnumerable<Attribute> SelectInterceptAttrs(this IEnumerable<Attribute> attributes)
        {
            return attributes.Where(t => t.GetType().BaseType == typeof(MethodBeforeAbstractInterceptAttribute)
                    || t.GetType().BaseType == typeof(MethodAfterAbstractInterceptAttribute) || t.GetType().BaseType == typeof(MethodAroundAbstractInterceptAttribute)
                    || t.GetType().BaseType == typeof(MethodAfterReturnAbstractInterceptAttribute) || t.GetType().BaseType == typeof(MethodAfterThrowingAbstractInterceptAttribute)
                    );
        }

        /// <summary>
        /// 收集拦截点
        /// </summary>
        public static void CollectionInterceptPoint()
        { 
            //获取所有类型
            var types = TypeFinder.GetExcludeSystemNugetAllTypes().Where(t => t.IsClass && !t.IsAbstract);

            //尝试拦截点添加
            types.Where(t => !t.Assembly.IsDynamic).ToList().ForEach(t =>
            {
                t.TryAddInterceptPoint();
            });
        }

        /// <summary>
        /// 收集拦截点
        /// </summary>
        /// <param name="assemblies">程序集</param>
        public static void CollectionInterceptPoint(List<Assembly> assemblies)
        { 
            if(assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));
            assemblies.Where(t => !t.IsDynamic).SelectMany(t => t.GetTypes()).Where(t => !t.IsAbstract && t.IsClass).ToList().ForEach(t =>
            {
                t.TryAddInterceptPoint();
            });
        }

        /// <summary>
        /// 收集拦截点
        /// </summary>
        /// <param name="types">类型集合</param>
        public static void CollectionInterceptPoint(List<Type> types)
        { 
            if(types==null)
                throw new ArgumentNullException(nameof(types));
            types.Where(t => !t.IsAbstract && t.IsClass).ToList().ForEach(t =>
            {
                t.TryAddInterceptPoint();
            });
        }

        /// <summary>
        /// 收集被拦截的类信息集合
        /// 如果确认要拦截点拦截的话需要在收集拦截类信息之前先调用
        /// <see cref="CollectionInterceptPoint"/>
        /// 或
        /// <see cref="CollectionInterceptPoint(List{Assembly})"/>
        /// 或
        /// <see cref="CollectionInterceptPoint(List{Type})"/>
        /// </summary>
        public static void CollectionInterceptedTypeInfo()
        {
            //获取所有类型
            var types = TypeFinder.GetExcludeSystemNugetAllTypes().Where(t => t.IsClass && !t.IsAbstract);
            CollectionInterceptedTypeInfoInternal(types);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="types"></param>
        private static void CollectionInterceptedTypeInfoInternal(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                InterceptedTypeInfo interceptedTypeInfo = new InterceptedTypeInfo(type, type.IsAbstractIntercept(),
                    type.IsInterceptPointIntercept(), type.IsInterfaceIntercept() ? AspectAttention.Enums.InterceptType.InterfaceIntercept : AspectAttention.Enums.InterceptType.ClassIntercept);

                //如果被拦截需要加入到拦截集里面
                if (interceptedTypeInfo.IsAbstractIntercept || interceptedTypeInfo.IsInterceptPointAbstractIntercept)
                    AutofacastleCollection.InterceptedTypeInfos.Add(interceptedTypeInfo);
            }
        }

        /// <summary>
        /// 收集被拦截的类信息集合
        /// </summary>
        /// <param name="assemblies"></param>
        public static void CollectionInterceptedTypeInfo(List<Assembly> assemblies)
        { 
            if(assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));
            var types = assemblies.SelectMany(t => t.GetTypes()).Where(t => !t.IsAbstract && t.IsClass).ToList();
            CollectionInterceptedTypeInfoInternal(types);
        }

        /// <summary>
        /// 收集被拦截的类信息集合
        /// </summary>
        /// <param name="types"></param>
        public static void CollectionInterceptedTypeInfo(List<Type> types)
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));
            types = types.Where(t => t.IsClass && !t.IsAbstract).ToList();
            CollectionInterceptedTypeInfoInternal(types);
        }

        /// <summary>
        /// 尝试添加拦截点
        /// </summary>
        /// <param name="type"></param>
        public static void TryAddInterceptPoint(this Type type)
        {
            //type是否实现AbstractInterceptPoint
            if (!typeof(AbstractInterceptPoint).IsAssignableFrom(type))
                return;

            //是否存在拦截点特性
            InterceptPointAttribute interceptPointAttribute = type.GetCustomAttribute<InterceptPointAttribute>();
            if (interceptPointAttribute == null)
                return;

            //存在 生成拦截点信息
            InterceptPointInfo interceptPointInfo = new InterceptPointInfo()
            {
                InterceptPointType = type,
                InterceptPoint = interceptPointAttribute,
            };

            //获取所有方法
            //var methodInfos = type.GetAllMethod(true);
            //筛选出属于本类的方法 以及AbstractInterceptPoint中的Around方法
            //methodInfos = methodInfos.Where(t => t.DeclaringType == type || (t.DeclaringType == typeof(AbstractInterceptPoint) && t.IsFinal));

            //筛选出属于本类的方法
            var methodInfos = type.GetAllMethod(false);

            //考虑判断是否存在多个相同特性的方法 存在则提示异常


            var methodBefore = methodInfos.FirstOrDefault(t => t.GetCustomAttribute<MethodBeforeInterceptAttribute>() != null);
            var methodAfter = methodInfos.FirstOrDefault(t => t.GetCustomAttribute<MethodAfterInterceptAttribute>() != null);
            var methodAround = methodInfos.FirstOrDefault(t => t.GetCustomAttribute<MethodAroundInterceptAttribute>() != null);
            var methodAfterReturn = methodInfos.FirstOrDefault(t => t.GetCustomAttribute<MethodAfterReturnInterceptAttribute>() != null);
            var methodAfterThrowing = methodInfos.FirstOrDefault(t => t.GetCustomAttribute<MethodAfterThrowingInterceptAttribute>() != null);

            if (methodBefore != null)
                interceptPointInfo.MethodBeforeIntercept = new Tuple<MethodBeforeInterceptAttribute, MethodInfo>(methodBefore.GetCustomAttribute<MethodBeforeInterceptAttribute>(), methodBefore);
            if (methodAfter != null)
                interceptPointInfo.MethodAfterIntercept = new Tuple<MethodAfterInterceptAttribute, MethodInfo>(methodAfter.GetCustomAttribute<MethodAfterInterceptAttribute>(), methodAfter);
            if (methodAround != null)
                interceptPointInfo.MethodAroundIntercept = new Tuple<MethodAroundInterceptAttribute, MethodInfo>(methodAround.GetCustomAttribute<MethodAroundInterceptAttribute>(), methodAround);
            if (methodAfterReturn != null)
                interceptPointInfo.MethodAfterReturnIntercept = new Tuple<MethodAfterReturnInterceptAttribute, MethodInfo>(methodAfterReturn.GetCustomAttribute<MethodAfterReturnInterceptAttribute>(), methodAfterReturn);
            if (methodAfterThrowing != null)
                interceptPointInfo.MethodAfterThrowingIntercept = new Tuple<MethodAfterThrowingInterceptAttribute, MethodInfo>(methodAfterThrowing.GetCustomAttribute<MethodAfterThrowingInterceptAttribute>(), methodAfterThrowing);

            InterceptPointCollection.interceptPointInfos.Add(interceptPointInfo);
        }

        /// <summary>
        /// 是否被AbstractIntercept拦截
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsAbstractIntercept(this Type type)
        {
            if (type.IsNonIntercept())//是否不拦截
                return false;

            //存在拦截特性
            return type.GetCustomAttributes().SelectInterceptAttrs().Any() || type.GetMethodsAttributes().SelectInterceptAttrs().Any();
        }


        /// <summary>
        /// 是否不拦截
        /// </summary>
        /// <returns></returns>
        private static bool IsNonIntercept(this Type type)
        {
            //满足不拦截筛选器的断言条件的不被拦截
            if (AutofacastleCollection.NonInterceptSelectors != null && AutofacastleCollection.NonInterceptSelectors.Any(t => t.Predicate(type)))
                return true;

            // 标注了NonIntercept特性不能被拦截
            // NonIntercept特性所在程序集即本类库所在程序集都不被拦截
            // 实现了INonIntercept接口的不被拦截
            // 打了拦截点的也不能被拦截
            // 标注了类拦截特性的 所有方法都不是虚方法的不能被拦截[拦截无意义]
            // 实现了IClassIntercept接口的 所有方法都不是虚方法的不能被拦截[拦截无意义]
            // 过滤掉依赖注入接口之后没有实现实际接口的类 所有方法都不是虚方法的不能被拦截[拦截无意义]
            if (type.GetCustomAttribute<NonInterceptAttribute>() != null
                ||
                type.Assembly == typeof(NonInterceptAttribute).Assembly 
                ||
                type.GetInterfaces().Any(t => t == typeof(INonIntercept))
                || 
                type.GetCustomAttribute<InterceptPointAttribute>() != null
                || 
                (type.GetCustomAttribute<ClassInterceptAttribute>()!=null && type.GetMethods().All(t=>!t.IsVirtual))
                ||
                (type.GetInterfaces().Any(t=>t==typeof(IClassIntercept)) && type.GetMethods().All(t => !t.IsVirtual))
                ||
                (!type.GetInterfaces().FilterDependencyInjectionInterfaces().Any() && type.GetMethods().All(t => !t.IsVirtual))
                )
                return true;

            return false;
        }


        /// <summary>
        /// 是否被拦截点所拦截
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsInterceptPointIntercept(this Type type)
        {

            bool IsIntercept = false;//是否类被拦截

            if (type.IsNonIntercept())//如果不拦截
                return false;

            //所有拦截点
            var interceptPointInfos = InterceptPointCollection.interceptPointInfos;

            //抽象接口不能
            if (type.IsAbstract || type.IsInterface) return false;

            //无拦截点
            if (!interceptPointInfos.Any()) return false;

            //该拦截点已经被拦截 防止重复注入多个拦截
            if (InterceptPointCollection.InterceptPointInterceptClassList.Contains(type))
                return true;

            //遍历拦截点
            foreach (var interceptPoint in interceptPointInfos)
            {
                //拦截点自身不拦截
                if (type == interceptPoint.InterceptPointType)
                {
                    continue;
                }

                //检查class拦截 如果被拦截 设置为拦截
                if (interceptPoint.InterceptPoint.IsValidClass(type))
                {
                    IsIntercept = true;
                }
                if (!IsIntercept) continue;//类不某个拦截点所拦截直接continue进行下一个切点判断

                //如果类是被拦截 但是类下面没有任何方法需要拦截即不需要拦截
                //如果方法要被拦截 其类需要被拦截

                //检查方法拦截 包含基类的方法
                foreach (var methodInfo in type.GetAllInstanceMethod(true))
                {
                    //检查方法是否拦截 不需要拦截直接continue
                    if (!interceptPoint.InterceptPoint.IsVaildMethod(methodInfo))
                    {
                        continue;
                    }
                    else//方法被拦截
                    {
                        //加入需要被拦截的类并且不存在的拦截类
                        if (IsIntercept && !InterceptPointCollection.InterceptPointInterceptClassList.Contains(type))
                            InterceptPointCollection.InterceptPointInterceptClassList.Add(type);
                    }

                    //需要拦截

                    //判断是否动态泛型
                    if (type.GetTypeInfo().IsGenericTypeDefinition)
                    {
                        //动态泛型里面的普通方法或泛型方法唯一名称
                        var uniqueKey = methodInfo.GetMethodInfoUniqueName();

                        //是否包含 包含就添加 
                        if (InterceptPointCollection.targetDynamicTMethodInfoList.ContainsKey(uniqueKey))
                            InterceptPointCollection.targetDynamicTMethodInfoList[uniqueKey].Add(interceptPoint);
                        else
                            InterceptPointCollection.targetDynamicTMethodInfoList.TryAdd(uniqueKey, new List<InterceptPointInfo>() { interceptPoint });
                    }
                    else
                    {
                        var objectKey = new ObjectKey(type, methodInfo);
                        if (InterceptPointCollection.targetMethodInterceptPointList.ContainsKey(objectKey))
                            InterceptPointCollection.targetMethodInterceptPointList[objectKey].Add(interceptPoint);
                        else
                            InterceptPointCollection.targetMethodInterceptPointList.TryAdd(objectKey, new List<InterceptPointInfo>() { interceptPoint });
                    }
                }


            }



            return IsIntercept;
        }

        /// <summary>
        /// 是否接口拦截
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsInterfaceIntercept(this Type type)
        {
            //动态泛型xxx<> 只能接口拦截
            if (type.GetTypeInfo().IsGenericTypeDefinition) return true;

            //存在类拦截特性
            if (type.GetCustomAttribute<ClassInterceptAttribute>() != null)
                return false;

            //存在实现了IClassIntercept接口 将类拦截
            if (type.GetInterfaces().Any(t => t is IClassIntercept))
                return false;

            //满足类拦截筛选器的将采用类拦截
            if (AutofacastleCollection.ClassInterceptSelectors!=null && AutofacastleCollection.ClassInterceptSelectors.Any(t => t.Predicate(type)))
                return false;


            //存在AsServices看AsServices的类型 如果AsServices不存在任何接口 即为类拦截
            var AsServices = type.GetCustomAttribute<TransientAttribute>()?.AsServices;
            if (AsServices == null)
                AsServices = type.GetCustomAttribute<ScopedAttribute>()?.AsServices;
            if (AsServices == null)
                AsServices = type.GetCustomAttribute<SingletonAttribute>()?.AsServices;
            if (AsServices != null && AsServices.Any())
            {
                if (AsServices.Any(t => t.IsInterface))
                    return true;
                return false;
            }

            //存在有除了过滤掉的接口外的其他接口
            if (type.GetInterfaces().FilterDependencyInjectionInterfaces().Any())
                return true;

            return false;

        }
    }
}
