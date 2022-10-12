using Autofac;
using OpenDeepSpace.NetCore.Autofacastle.DependencyInjection.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.DependencyInjection
{
    /// <summary>
    /// Ioc管理者 用于在不能使用注入特性或无法注入的类里面来获取实例
    /// </summary>
    public class IocManager
    {
        private static readonly object obj = new();
        private static ILifetimeScope? Container { get; set; }

        //单例模式
        public static void InitContainer(ILifetimeScope container)
        {
            //防止过程中方法被调用_container发生改变
            if (Container == null)
            {
                lock (obj)
                {
                    if (Container == null)
                    {
                        Container = container;
                    }
                }
            }
        }
        /// <summary>
        /// 解析实例 能够明确知道能从其中解析出来 否则出现异常 请使用<see cref="TryResolve{TService}"/>该方法来解析
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public static TService Resolve<TService>() where TService : notnull
        {
            if (Container == null)
                throw new ArgumentException(nameof(Container));

            return Container.Resolve<TService>();
        }

        /// <summary>
        /// 尝试解析
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public static TService? TryResolve<TService>() where TService : class
        {

            if (Container == null)
                throw new ArgumentException(nameof(Container));
            Container.TryResolve(out TService? instance);

            return instance;
        }


        /// <summary>
        /// 解析出指定实现类的实例 
        /// 能明确知道能从某个<see cref="Enums.ResolveMode"/>模式中解析出指定的类
        /// 否则有一次 请考虑使用<see cref="TryResolve{TService}(Type)"/>
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="resolveMode">解析模式 默认为<see cref="ResolveMode.Keyed"/>从Keyed来解析</param>
        /// <param name="ImplementationType"></param>
        /// <returns></returns>
        public static TService Resolve<TService>(Type ImplementationType,ResolveMode resolveMode=ResolveMode.Keyed) where TService : notnull
        {
            if (Container == null)
                throw new ArgumentException(nameof(Container));
            if (ImplementationType == null)
                throw new ArgumentNullException(nameof(ImplementationType));

            TService? instance =default;

            //从Keyed中解析
            if(resolveMode==ResolveMode.Keyed)
                instance=Container.ResolveKeyed<TService>(ImplementationType);
            //从Named中解析
            if(resolveMode==ResolveMode.Named && ImplementationType.FullName!=null)
                instance = Container.ResolveNamed<TService>(ImplementationType.FullName);

            //获取自身直接通过实现类去解析
            if (resolveMode==ResolveMode.Self)
                instance = (TService)Container.Resolve(ImplementationType);


            return instance!;//前面解析不出问题这里一定不为空
        }

        /// <summary>
        /// 尝试解析
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="ImplementationType"></param>
        /// <returns></returns>
        public static TService? TryResolve<TService>(Type ImplementationType) where TService : class
        {
            if (Container == null)
                throw new ArgumentException(nameof(Container));
            if (ImplementationType == null)
                throw new ArgumentNullException(nameof(ImplementationType));

            //优先从Keyed中尝试解析
            Container.TryResolveKeyed(ImplementationType, out TService? instance);

            //如果keyed中解析不出来 尝试从Named中解析
            if (ImplementationType.FullName != null && instance != null)
                Container.TryResolveNamed(ImplementationType.FullName, out instance);

            //如果Keyed和Named都解析不出来 尝试自身解析
            if (instance == null)
            {
                Container.TryResolve(ImplementationType, out object? instanceValue);

                if (instanceValue != null)
                    instance = (TService?)instanceValue;
            }

            return instance;

        }

        /// <summary>
        /// 从Keyed中解析 对于非使用Implementation作为Keyed的 自己命名的Keyed
        /// 如果不确定能解析处出来 请使用<see cref="TryResolve{TService}(object)"/>
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="Keyed"></param>
        /// <returns></returns>
        public static TService Resolve<TService>(object Keyed) where TService : notnull
        {
            if (Container == null)
                throw new ArgumentException(nameof(Container));
            if (Keyed == null)
                throw new ArgumentNullException(nameof(Keyed));

            
            TService instance = Container.ResolveKeyed<TService>(Keyed);
            
            return instance;
        }

        /// <summary>
        /// 尝试从Keyed中解析 对于非使用Implementation作为Keyed的 自己命名的Keyed
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="Keyed"></param>
        /// <returns></returns>
        public static TService? TryResolve<TService>(object Keyed) where TService : notnull
        {
            if (Container == null)
                throw new ArgumentException(nameof(Container));
            if (Keyed == null)
                throw new ArgumentNullException(nameof(Keyed));

            Container.TryResolveKeyed(Keyed, out object? instance);

            return (TService?)instance;
        }

        /// <summary>
        /// 从Named中解析 对于非使用实现类的FullName作为Named的 自己命名的Named
        /// 如果不确定能解析处出来 请使用<see cref="TryResolve{TService}(string)"/>
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="Named"></param>
        /// <returns></returns>
        public static TService Resolve<TService>(string Named) where TService : notnull
        {
            if (Container == null)
                throw new ArgumentException(nameof(Container));
            if (Named == null)
                throw new ArgumentNullException(nameof(Named));


            TService instance = Container.ResolveNamed<TService>(Named);

            return instance;
        }

        /// <summary>
        /// 尝试从Keyed中解析 对于非使用实现类的FullName作为Named的 自己命名的Keyed
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="Named"></param>
        /// <returns></returns>
        public static TService? TryResolve<TService>(string Named) where TService : notnull
        {
            if (Container == null)
                throw new ArgumentException(nameof(Container));
            if (Named == null)
                throw new ArgumentNullException(nameof(Named));

            Container.TryResolveNamed(Named, out object? instance);

            return (TService?)instance;
        }


    }
}
