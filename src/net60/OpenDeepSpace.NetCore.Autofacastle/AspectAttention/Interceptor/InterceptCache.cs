using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Attributes;
using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Interceptor.Attributes;
using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.Attributes;
using OpenDeepSpace.NetCore.Autofacastle.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Interceptor
{
    /// <summary>
    /// 拦截缓存
    /// </summary>
    [Singleton(AutoActivate = true)]
    public class InterceptCache
    {

        /// <summary>
        /// 缓存
        /// </summary>
        internal ConcurrentDictionary<MethodInfo, InterceptInvokeChainBuilder> CacheList { get; set; }

        /// <summary>
        /// 由于动态泛型的method是跟着泛型T变化的 单独缓存
        /// </summary>
        internal ConcurrentDictionary<string, InterceptInvokeChainBuilder> DynamicCacheList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public InterceptCache()
        {
            CacheList = new ConcurrentDictionary<MethodInfo, InterceptInvokeChainBuilder>();
            DynamicCacheList = new ConcurrentDictionary<string, InterceptInvokeChainBuilder>();


            //查找出需要拦截的类
            var needInterceptClasses = AutofacastleCollection.InterceptedTypeInfos.Where(t => t.IsAbstractIntercept);

            //遍历需要拦截的类
            foreach (var needInterceptClass in needInterceptClasses)
            {
                //获取方法
                var methodInfos = needInterceptClass.InterceptedType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(m => !m.IsSpecialName);

                //类上标记的拦截特性
                var classAttributes = needInterceptClass.InterceptedType.GetCustomAttributes().SelectInterceptAttrs();

                foreach (var method in methodInfos)
                {

                    //如果方法上使用了不拦截特性将不被拦截
                    if (method.GetCustomAttribute<NonInterceptAttribute>() != null)
                        continue;


                    //获取所有attribute
                    var methodAttributes = method.GetCustomAttributes().SelectInterceptAttrs();

                    //如果没有任何需要拦截的标记 以及类上也没有标记跳过
                    if (!methodAttributes.Any() && !classAttributes.Any())
                        continue;

                    //组合所有拦截特性 去掉重复的
                    var allInterceptAttributes = classAttributes.Union(methodAttributes);

                    var interceptInvokeChainBuilder = new InterceptInvokeChainBuilder()
                    {
                        InterceptSurfaces = new List<InterceptSurface>()
                    };

                    //存储拦截面 kv k:GroupName v:一个拦截面
                    Dictionary<string, InterceptSurface> InterceptSurfaceDic = new Dictionary<string, InterceptSurface>();

                    foreach (var interceptAttr in allInterceptAttributes)
                    {

                        var interceptBaseType = interceptAttr.GetType().BaseType;//拦截基类
                        var methodInterceptBase = (interceptAttr as MethodInterceptBaseAttribute);
                        var groupName = methodInterceptBase.GroupName;
                        var order = methodInterceptBase.Order;

                        InterceptSurface InterceptSurface = null;
                        //判断存储拦截面中是否存在
                        if (InterceptSurfaceDic.ContainsKey(groupName))
                        {
                            //取出拦截面
                            InterceptSurface = InterceptSurfaceDic[groupName];

                            //判断当前拦截面是否已经存在相应的类型的拦截特性 如果已经存在相同的呢 就抛出异常
                            if (InterceptSurface.MethodBeforeIntercept != null && interceptBaseType == typeof(MethodBeforeAbstractInterceptAttribute))
                            {

                                if (string.IsNullOrWhiteSpace(groupName))
                                    throw new Exception($"默认组中已经存在一个{InterceptSurface.MethodBeforeIntercept}实现{interceptBaseType}的拦截特性,考虑需要为{interceptAttr.GetType()}使用GroupName进行分组");
                                else
                                    throw new Exception($"{groupName}组中已经存在一个{InterceptSurface.MethodBeforeIntercept}实现{interceptBaseType}的拦截特性,考虑需要为{interceptAttr.GetType()}使用GroupName进行分组");
                            }
                            if (InterceptSurface.MethodAfterIntercept != null && interceptBaseType == typeof(MethodAfterAbstractInterceptAttribute))
                            {
                                if (string.IsNullOrWhiteSpace(groupName))
                                    throw new Exception($"默认组中已经存在一个{InterceptSurface.MethodAfterIntercept}实现{interceptBaseType}的拦截特性,考虑需要为{interceptAttr.GetType()}使用GroupName进行分组");
                                else
                                    throw new Exception($"{groupName}组中已经存在一个{InterceptSurface.MethodAfterIntercept}实现{interceptBaseType}的拦截特性,考虑需要为{interceptAttr.GetType()}使用GroupName进行分组");
                            }
                            if (InterceptSurface.MethodAfterThrowingIntercept != null && interceptBaseType == typeof(MethodAfterThrowingAbstractInterceptAttribute))
                            {
                                if (string.IsNullOrWhiteSpace(groupName))
                                    throw new Exception($"默认组中已经存在一个{InterceptSurface.MethodAfterThrowingIntercept}实现{interceptBaseType}的拦截特性,考虑需要为{interceptAttr.GetType()}使用GroupName进行分组");
                                else
                                    throw new Exception($"{groupName}组中已经存在一个{InterceptSurface.MethodAfterThrowingIntercept}实现{interceptBaseType}的拦截特性,考虑需要为{interceptAttr.GetType()}使用GroupName进行分组");
                            }
                            if (InterceptSurface.MethodAroundIntercept != null && interceptBaseType == typeof(MethodAroundAbstractInterceptAttribute))
                            {
                                if (string.IsNullOrWhiteSpace(groupName))
                                    throw new Exception($"默认组中已经存在一个{InterceptSurface.MethodAroundIntercept}实现{interceptBaseType}的拦截特性,考虑需要为{interceptAttr.GetType()}使用GroupName进行分组");
                                else
                                    throw new Exception($"{groupName}组中已经存在一个{InterceptSurface.MethodAroundIntercept}实现{interceptBaseType}的拦截特性,考虑需要为{interceptAttr.GetType()}使用GroupName进行分组");
                            }
                            if (InterceptSurface.MethodAfterReturnIntercept != null && interceptBaseType == typeof(MethodAfterReturnAbstractInterceptAttribute))
                            {
                                if (string.IsNullOrWhiteSpace(groupName))
                                    throw new Exception($"默认组中已经存在一个{InterceptSurface.MethodAfterReturnIntercept}实现{interceptBaseType}的拦截特性,考虑需要为{interceptAttr.GetType()}使用GroupName进行分组");
                                else
                                    throw new Exception($"{groupName}组中已经存在一个{InterceptSurface.MethodAfterReturnIntercept}实现{interceptBaseType}的拦截特性,考虑需要为{interceptAttr.GetType()}使用GroupName进行分组");
                            }
                            else//如果不存在相应的拦截特性 那么就是属于当前拦截面 就判断Order是否相同 不同抛出错误信息
                            {
                                if (InterceptSurface.Order != order)
                                {
                                    if (string.IsNullOrWhiteSpace(groupName))
                                        throw new Exception($"默认组中,{interceptAttr.GetType()}拦截特性的Order:{order}与同组其他拦截特性的Order不同");
                                    else
                                        throw new Exception($"{groupName}组中,{interceptAttr.GetType()}拦截特性的Order:{order}与同组其他拦截特性的Order不同");
                                }
                            }

                        }
                        else
                        {
                            InterceptSurface = new InterceptSurface();
                            InterceptSurface.GroupName = groupName;
                            InterceptSurface.Order = order;

                            InterceptSurfaceDic[groupName] = InterceptSurface;
                        }

                        //switch case 
                        switch (interceptAttr)
                        {
                            case MethodBeforeAbstractInterceptAttribute methodBeforeIntercept:

                                InterceptSurface.MethodBeforeIntercept = methodBeforeIntercept;

                                break;
                            case MethodAfterAbstractInterceptAttribute methodAfterIntercept:

                                InterceptSurface.MethodAfterIntercept = methodAfterIntercept;
                                break;
                            case MethodAroundAbstractInterceptAttribute methodAroundIntercept:

                                InterceptSurface.MethodAroundIntercept = methodAroundIntercept;

                                break;
                            case MethodAfterReturnAbstractInterceptAttribute methodAfterReturnIntercept:

                                InterceptSurface.MethodAfterReturnIntercept = methodAfterReturnIntercept;

                                break;
                            case MethodAfterThrowingAbstractInterceptAttribute methodAfterThrowingIntercept:

                                InterceptSurface.MethodAfterThrowingIntercept = methodAfterThrowingIntercept;

                                break;
                        }

                    }

                    //遍历拦截面dic
                    foreach (string key in InterceptSurfaceDic.Keys)
                    {
                        interceptInvokeChainBuilder.InterceptSurfaces.Add(InterceptSurfaceDic[key]);
                    }


                    //拦截面排序
                    interceptInvokeChainBuilder.InterceptSurfaces = interceptInvokeChainBuilder.InterceptSurfaces.OrderBy(t => t.Order).ToList();

                    if (needInterceptClass.GetType().GetTypeInfo().IsGenericTypeDefinition)
                    {
                        DynamicCacheList.TryAdd(method.GetMethodInfoUniqueName(), interceptInvokeChainBuilder);
                        continue;
                    }

                    CacheList.TryAdd(method, interceptInvokeChainBuilder);

                }

            }

        }


    }
}
