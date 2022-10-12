using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention.Interceptor
{
    /// <summary>
    /// 拦截调用链构建
    /// </summary>
    internal class InterceptInvokeChainBuilder
    {

        public List<InterceptSurface> InterceptSurfaces { get; set; }

        public readonly Lazy<InterceptContextDelegate> InterceptFunc;

        /// <summary>
        /// 
        /// </summary>
        public InterceptInvokeChainBuilder()
        {
            InterceptFunc = new Lazy<InterceptContextDelegate>(this.BuilderMethodChain);
        }

        private InterceptContextDelegate BuilderMethodChain()
        {
            InterceptNodeBuilder builder = new InterceptNodeBuilder();

            var aroundIndex = 0;
            foreach (var chain in InterceptSurfaces)
            {
                var isLast = aroundIndex == InterceptSurfaces.Count - 1;

                if (chain.MethodAfterReturnIntercept != null)
                {
                    var after = new MethodAfterReturnInterceptHandle(chain.MethodAfterReturnIntercept);
                    //After 后加进去先执行
                    builder.Use(next => async ctx => await after.OnInvocation(ctx, next));
                }

                if (chain.MethodAroundIntercept != null)
                {
                    var around = new MethodAroundInterceptHandle(chain.MethodAroundIntercept, chain.MethodAfterIntercept, chain.MethodAfterThrowingIntercept);
                    //Around 先加进去先执行 后续执行权在Around的实际运行方法
                    builder.Use(next => async ctx => { await around.OnInvocation(ctx, next); });
                }


                if (chain.MethodAroundIntercept == null && chain.MethodAfterThrowingIntercept != null)
                {
                    var aspectThrowingInterceptor = new MethodAfterThrowingInterceptHandle(chain.MethodAfterThrowingIntercept);
                    builder.Use(next => async ctx => { await aspectThrowingInterceptor.OnInvocation(ctx, next); });
                }

                if (chain.MethodAroundIntercept == null && chain.MethodAfterIntercept != null)
                {
                    var after = new MethodAfterInterceptHandle(chain.MethodAfterIntercept);
                    builder.Use(next => async ctx => await after.OnInvocation(ctx, next));
                }

                if (chain.MethodBeforeIntercept != null)
                {
                    //Before先加进去先执行
                    var before = new MethodBeforeInterceptHandle(chain.MethodBeforeIntercept);
                    builder.Use(next => async ctx => await before.OnInvocation(ctx, next));
                }

                aroundIndex++;
                if (!isLast) continue;

                //真正的方法
                builder.Use(next => async ctx =>
                {
                    try
                    {
                        await ctx.Proceed();
                    }
                    catch (Exception ex)
                    {
                        ctx.Exception = ex; // 只有这里会设置值 到错误增强器里面去处理并 在最后一个错误处理器里面throw
                        throw;
                    }

                    await next(ctx);
                });
            }

            var aspectfunc = builder.Build();
            return aspectfunc;
        }
    }
}
