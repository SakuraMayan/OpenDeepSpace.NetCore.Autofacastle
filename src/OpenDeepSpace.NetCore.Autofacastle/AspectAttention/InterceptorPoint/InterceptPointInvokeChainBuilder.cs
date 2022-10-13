using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint
{
    /// <summary>
    /// 拦截点调用链构建
    /// </summary>
    public class InterceptPointInvokeChainBuilder
    {
        public readonly Lazy<InterceptContextDelegate> InterceptFunc;

        public InterceptPointInvokeChainBuilder()
        {
            InterceptFunc = new Lazy<InterceptContextDelegate>(() => BuilderMethodChain());
        }

        /// <summary>
        ///     支持把Aspect传进来
        /// </summary>
        /// <param name="mimin"></param>
        public InterceptPointInvokeChainBuilder(Lazy<InterceptContextDelegate> mimin)
        {
            InterceptFunc = new Lazy<InterceptContextDelegate>(() => BuilderMethodChain(mimin));
        }

        public List<InterceptPointRealMethod> InterceptPointRealMethods { get; set; }

        private InterceptContextDelegate BuilderMethodChain(Lazy<InterceptContextDelegate> mimin = null)
        {
            var builder = new InterceptNodeBuilder();

            var aroundIndex = 0;
            foreach (var chain in InterceptPointRealMethods)
            {
                var isLast = aroundIndex == InterceptPointRealMethods.Count - 1;

                if (chain.AfterReturnInterceptMethod != null)
                {
                    var after = new MethodAfterReturnInterceptHandle(chain.AfterReturnInterceptMethod);
                    //After 后加进去先执行
                    builder.Use(next => async ctx => await after.OnInvocation(ctx, next));
                }

                if (chain.AroundInterceptMethod != null)
                {
                    var around = new MethodAroundInterceptHandle(chain.AroundInterceptMethod,
                        chain.AfterInterceptMethod != null ? new MethodAfterInterceptHandle(chain.AfterInterceptMethod, true) : null,
                        chain.AfterThrowingInterceptMethod != null ? new MethodAfterThrowingInterceptHandle(chain.AfterThrowingInterceptMethod, true) : null);
                    //Around 先加进去先执行 后续执行权交给了Around的实际运行方法
                    builder.Use(next => async ctx => await around.OnInvocation(ctx, next));
                }


                if (chain.AroundInterceptMethod == null && chain.AfterThrowingInterceptMethod != null)
                {
                    var aspectThrowingInterceptor = new MethodAfterThrowingInterceptHandle(chain.AfterThrowingInterceptMethod);
                    builder.Use(next => async ctx => { await aspectThrowingInterceptor.OnInvocation(ctx, next); });
                }

                if (chain.AroundInterceptMethod == null && chain.AfterInterceptMethod != null)
                {
                    var after = new MethodAfterInterceptHandle(chain.AfterInterceptMethod);
                    builder.Use(next => async ctx => await after.OnInvocation(ctx, next));
                }


                if (chain.BeforeInterceptMethod != null)
                {
                    //Before先加进去先执行
                    var before = new MethodBeforeInterceptHandle(chain.BeforeInterceptMethod);
                    builder.Use(next => async ctx => await before.OnInvocation(ctx, next));
                }

                aroundIndex++;

                if (!isLast) continue;

                //真正的方法
                builder.Use(next => async ctx =>
                {
                    try
                    {
                        if (mimin != null)
                            await mimin.Value.Invoke(ctx);
                        else
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
