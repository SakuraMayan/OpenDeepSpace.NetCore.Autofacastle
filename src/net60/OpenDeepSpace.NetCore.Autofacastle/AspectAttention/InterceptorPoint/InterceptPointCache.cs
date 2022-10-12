using Autofac;
using OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Interceptor;
using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint
{
    /// <summary>
    /// 拦截点缓存
    /// </summary>
    [Singleton(AutoActivate = true)]
    public class InterceptPointCache
    {

        /// <summary>
        ///     缓存
        /// </summary>
        internal ConcurrentDictionary<ObjectKey, InterceptPointInvokeChainBuilder> CacheList { get; set; }

        /// <summary>
        ///     由于动态泛型的method是跟着泛型T变化的  所以需要单独缓存
        /// </summary>
        internal ConcurrentDictionary<string, InterceptPointInvokeChainBuilder> DynamicCacheList { get; set; }


        private readonly InterceptCache _cache;
        private readonly IComponentContext _context;

        /// <summary>
        ///     初始化
        /// </summary>
        public InterceptPointCache(IComponentContext context)
        {
            _context = context;
            _cache = context.Resolve<InterceptCache>();

            CacheList = new ConcurrentDictionary<ObjectKey, InterceptPointInvokeChainBuilder>();

            DynamicCacheList = new ConcurrentDictionary<string, InterceptPointInvokeChainBuilder>();

            //普通方法
            foreach (var methodCache in InterceptPointCollection.targetMethodInterceptPointList)
            {
                AddCacheInter(methodCache.Key, methodCache.Value);
            }

            //动态方法
            foreach (var methodCache in InterceptPointCollection.targetDynamicTMethodInfoList)
            {
                AddDynamicCacheInter(methodCache.Key, methodCache.Value);
            }
        }



        /// <summary>
        /// 针对泛型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void AddDynamicCacheInter(string key, List<InterceptPointInfo> value)
        {
            var interceptPointMethodChain = _cache != null && _cache.DynamicCacheList.TryGetValue(key, out var attribute)
                ? new InterceptPointInvokeChainBuilder
                    (attribute.InterceptFunc)
                {
                    InterceptPointRealMethods = new List<InterceptPointRealMethod>()
                }
                : new InterceptPointInvokeChainBuilder
                {
                    InterceptPointRealMethods = new List<InterceptPointRealMethod>()
                };

            //一个方法会有多个interceptPoint
            foreach (var interceptPointInfo in value)
            {
                var interceptPointRealMethod = new InterceptPointRealMethod
                {
                    Order = interceptPointInfo.InterceptPoint.Order
                };

                interceptPointMethodChain.InterceptPointRealMethods.Add(interceptPointRealMethod);

                //拿到拦截点对应的类的实例
                var instance = _context.Resolve(interceptPointInfo.InterceptPointType);

                if (interceptPointInfo.MethodBeforeIntercept != null)
                    interceptPointRealMethod.BeforeInterceptMethod = new InterceptPointRunTimeMethod<Attributes.MethodBeforeInterceptAttribute>
                    {
                        Instance = instance,
                        MethodInfo = interceptPointInfo.MethodBeforeIntercept.Item2,
                        MethodReturnType = interceptPointInfo.MethodBeforeIntercept.Item2.ReturnType,
                        MethodParameters = interceptPointInfo.MethodBeforeIntercept.Item2.GetParameters(),
                        InterceptMethodAttribute = interceptPointInfo.MethodBeforeIntercept.Item1,
                    };

                if (interceptPointInfo.MethodAfterIntercept != null)
                    interceptPointRealMethod.AfterInterceptMethod = new InterceptPointRunTimeMethod<Attributes.MethodAfterInterceptAttribute>
                    {
                        Instance = instance,
                        MethodInfo = interceptPointInfo.MethodAfterIntercept.Item2,
                        MethodReturnType = interceptPointInfo.MethodAfterIntercept.Item2.ReturnType,
                        MethodParameters = interceptPointInfo.MethodAfterIntercept.Item2.GetParameters(),
                        InterceptMethodAttribute = interceptPointInfo.MethodAfterIntercept.Item1,
                    };

                if (interceptPointInfo.MethodAfterReturnIntercept != null)
                    interceptPointRealMethod.AfterReturnInterceptMethod = new InterceptPointRunTimeMethod<Attributes.MethodAfterReturnInterceptAttribute>
                    {
                        Instance = instance,
                        MethodInfo = interceptPointInfo.MethodAfterReturnIntercept.Item2,
                        MethodReturnType = interceptPointInfo.MethodAfterReturnIntercept.Item2.ReturnType,
                        MethodParameters = interceptPointInfo.MethodAfterReturnIntercept.Item2.GetParameters(),
                        InterceptMethodAttribute = interceptPointInfo.MethodAfterReturnIntercept.Item1,
                    };

                if (interceptPointInfo.MethodAroundIntercept != null)
                    interceptPointRealMethod.AroundInterceptMethod = new InterceptPointRunTimeMethod<Attributes.MethodAroundInterceptAttribute>
                    {
                        Instance = instance,
                        MethodInfo = interceptPointInfo.MethodAroundIntercept.Item2,
                        MethodReturnType = interceptPointInfo.MethodAroundIntercept.Item2.ReturnType,
                        MethodParameters = interceptPointInfo.MethodAroundIntercept.Item2.GetParameters(),
                        InterceptMethodAttribute = interceptPointInfo.MethodAroundIntercept.Item1
                    };

                if (interceptPointInfo.MethodAfterThrowingIntercept != null)
                    interceptPointRealMethod.AfterThrowingInterceptMethod = new InterceptPointRunTimeMethod<Attributes.MethodAfterThrowingInterceptAttribute>
                    {
                        Instance = instance,
                        MethodInfo = interceptPointInfo.MethodAfterThrowingIntercept.Item2,
                        MethodReturnType = interceptPointInfo.MethodAfterThrowingIntercept.Item2.ReturnType,
                        MethodParameters = interceptPointInfo.MethodAfterThrowingIntercept.Item2.GetParameters(),
                        InterceptMethodAttribute = interceptPointInfo.MethodAfterThrowingIntercept.Item1
                    };
            }

            interceptPointMethodChain.InterceptPointRealMethods = interceptPointMethodChain.InterceptPointRealMethods.OrderBy(r => r.Order).ToList();
            DynamicCacheList.TryAdd(key, interceptPointMethodChain);
        }

        /// <summary>
        /// 针对非泛型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void AddCacheInter(ObjectKey key, List<InterceptPointInfo> value)
        {
            var interceptPointMethodChain = _cache != null && _cache.CacheList.TryGetValue(key.Method, out var attribute)
                    ? new InterceptPointInvokeChainBuilder
                        (attribute.InterceptFunc)
                    {
                        InterceptPointRealMethods = new List<InterceptPointRealMethod>()
                    }
                    : new InterceptPointInvokeChainBuilder
                    {
                        InterceptPointRealMethods = new List<InterceptPointRealMethod>()
                    }
                ;

            //一个方法会有多个interceptpoint
            foreach (var interceptPointInfo in value)
            {
                var interceptPointRealMethod = new InterceptPointRealMethod
                {
                    Order = interceptPointInfo.InterceptPoint.Order
                };

                interceptPointMethodChain.InterceptPointRealMethods.Add(interceptPointRealMethod);

                //每个切换先拿到对应的实例 重复拿也没关系 因为是单例的
                var instance = _context.Resolve(interceptPointInfo.InterceptPointType);

                if (interceptPointInfo.MethodBeforeIntercept != null)
                    interceptPointRealMethod.BeforeInterceptMethod = new InterceptPointRunTimeMethod<Attributes.MethodBeforeInterceptAttribute>
                    {
                        Instance = instance,
                        MethodInfo = interceptPointInfo.MethodBeforeIntercept.Item2,
                        MethodReturnType = interceptPointInfo.MethodBeforeIntercept.Item2.ReturnType,
                        MethodParameters = interceptPointInfo.MethodBeforeIntercept.Item2.GetParameters(),
                        InterceptMethodAttribute = interceptPointInfo.MethodBeforeIntercept.Item1,
                    };

                if (interceptPointInfo.MethodAfterIntercept != null)
                    interceptPointRealMethod.AfterInterceptMethod = new InterceptPointRunTimeMethod<Attributes.MethodAfterInterceptAttribute>
                    {
                        Instance = instance,
                        MethodInfo = interceptPointInfo.MethodAfterIntercept.Item2,
                        MethodReturnType = interceptPointInfo.MethodAfterIntercept.Item2.ReturnType,
                        MethodParameters = interceptPointInfo.MethodAfterIntercept.Item2.GetParameters(),
                        InterceptMethodAttribute = interceptPointInfo.MethodAfterIntercept.Item1,
                    };

                if (interceptPointInfo.MethodAfterReturnIntercept != null)
                    interceptPointRealMethod.AfterReturnInterceptMethod = new InterceptPointRunTimeMethod<Attributes.MethodAfterReturnInterceptAttribute>
                    {
                        Instance = instance,
                        MethodInfo = interceptPointInfo.MethodAfterReturnIntercept.Item2,
                        MethodReturnType = interceptPointInfo.MethodAfterReturnIntercept.Item2.ReturnType,
                        MethodParameters = interceptPointInfo.MethodAfterReturnIntercept.Item2.GetParameters(),
                        InterceptMethodAttribute = interceptPointInfo.MethodAfterReturnIntercept.Item1,
                    };

                if (interceptPointInfo.MethodAroundIntercept != null)
                    interceptPointRealMethod.AroundInterceptMethod = new InterceptPointRunTimeMethod<Attributes.MethodAroundInterceptAttribute>
                    {
                        Instance = instance,
                        MethodInfo = interceptPointInfo.MethodAroundIntercept.Item2,
                        MethodReturnType = interceptPointInfo.MethodAroundIntercept.Item2.ReturnType,
                        MethodParameters = interceptPointInfo.MethodAroundIntercept.Item2.GetParameters(),
                        InterceptMethodAttribute = interceptPointInfo.MethodAroundIntercept.Item1
                    };

                if (interceptPointInfo.MethodAfterThrowingIntercept != null)
                    interceptPointRealMethod.AfterThrowingInterceptMethod = new InterceptPointRunTimeMethod<Attributes.MethodAfterThrowingInterceptAttribute>
                    {
                        Instance = instance,
                        MethodInfo = interceptPointInfo.MethodAfterThrowingIntercept.Item2,
                        MethodReturnType = interceptPointInfo.MethodAfterThrowingIntercept.Item2.ReturnType,
                        MethodParameters = interceptPointInfo.MethodAfterThrowingIntercept.Item2.GetParameters(),
                        InterceptMethodAttribute = interceptPointInfo.MethodAfterThrowingIntercept.Item1
                    };
            }

            interceptPointMethodChain.InterceptPointRealMethods = interceptPointMethodChain.InterceptPointRealMethods.OrderBy(r => r.Order).ToList();
            CacheList.TryAdd(key, interceptPointMethodChain);
        }

    }
}
