using Autofac;
using Castle.DynamicProxy;
using stakx.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention
{
    /// <summary>
    /// 拦截上下文
    /// </summary>
    public class InterceptContext
    {
        /// <summary>
        /// 空的构造方法
        /// </summary>
        public InterceptContext()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="context"></param>
        /// <param name="invocation"></param>
        public InterceptContext(IComponentContext context, IInvocation invocation)
        {
            this.ComponentContext = context;
            this.InvocationContext = invocation;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="context"></param>
        /// <param name="invocation"></param>
        public InterceptContext(IComponentContext context, IAsyncInvocation invocation)
        {
            this.ComponentContext = context;
            this.AsyncInvocationContext = invocation;
        }

        /// <summary>
        /// autofac容器
        /// </summary>
        public IComponentContext ComponentContext { get; set; }


        /// <summary>
        /// 执行环节上下文
        /// </summary>

        internal IInvocation InvocationContext { get; set; }

        /// <summary>
        /// 异步执行环节上下文
        /// </summary>
        internal IAsyncInvocation AsyncInvocationContext { get; set; }


        /// <summary>
        /// 被拦截的目标方法的参数
        /// </summary>
        public IReadOnlyList<object> Arguments
        {
            get
            {
                if (InvocationContext != null)
                {
                    return InvocationContext.Arguments;
                }

                return AsyncInvocationContext.Arguments;
            }
        }

        /// <summary>
        /// 被拦截的目标方法
        /// </summary>
        public MethodInfo TargetMethod
        {
            get
            {
                if (InvocationContext != null)
                {
                    return InvocationContext.MethodInvocationTarget;
                }

                return AsyncInvocationContext.TargetMethod;
            }
        }

        /// <summary>
        /// 目标类
        /// </summary>
        public Type TargetType
        {
            get
            {
                if (InvocationContext != null)
                {
                    return InvocationContext.TargetType;
                }

                return AsyncInvocationContext.TargetType;
            }
        }

        /// <summary>
        /// 被拦截的目标方法的proxy方法
        /// </summary>
        public MethodInfo Method
        {
            get
            {
                if (InvocationContext != null)
                {
                    return InvocationContext.Method;
                }

                return AsyncInvocationContext.Method;
            }
        }

        /// <summary>
        /// 设置返回值或者获取返回值
        /// </summary>
        public object ReturnValue
        {
            get
            {
                if (InvocationContext != null)
                {
                    return InvocationContext.ReturnValue;
                }

                return AsyncInvocationContext?.Result;
            }
            set
            {
                if (InvocationContext != null)
                {
                    InvocationContext.ReturnValue = value;
                    return;
                }

                if (AsyncInvocationContext != null)
                {
                    AsyncInvocationContext.Result = value;
                }
            }
        }


        /// <summary>
        /// 实际真正的方法用在拦截器链的执行过程中
        /// </summary>
        internal Func<ValueTask> Proceed { get; set; }


        /// <summary>
        /// 有返回Exception
        /// </summary>
        internal Exception Exception { get; set; }
    }
}
