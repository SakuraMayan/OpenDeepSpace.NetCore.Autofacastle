using Autofac;
using OpenDeepSpace.NetCore.Autofacastle.AspectAttention;
using OpenDeepSpace.NetCore.Autofacastle.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle
{
    /// <summary>
    /// 方法调用帮助类
    /// </summary>
    public class MethodInvokeHelper
    {
        /// <summary>
        /// 调用切面的拦截方法
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="methodInfo"></param>
        /// <param name="parameters"></param>
        /// <param name="context"></param>
        /// <param name="invocation"></param>
        /// <param name="_next"></param>
        /// <param name="returnValue"></param>
        /// <param name="returnParam"></param>
        /// <returns></returns>
        public static object InvokeInstanceMethod(object instance, MethodInfo methodInfo, ParameterInfo[] parameters, IComponentContext context,
            InterceptContext invocation = null, InterceptContextDelegate _next = null, object returnValue = null, string returnParam = null)
        {
            if (parameters == null || parameters.Length == 0)
            {
                return methodInfo.Invoke(instance, null);
            }

            //自动类型注入

            List<object> parameterObj = new();
            foreach (var parameter in parameters)
            {
                if (invocation != null && parameter.ParameterType == typeof(InterceptContext))
                {
                    parameterObj.Add(invocation);
                    continue;
                }

                if (_next != null && parameter.ParameterType == typeof(InterceptContextDelegate))
                {
                    parameterObj.Add(_next);
                    continue;
                }

                if (returnValue != null && !string.IsNullOrWhiteSpace(returnParam) && parameter.Name.Equals(returnParam))
                {
                    //如果指定的类型会出错
                    parameterObj.Add(returnValue);
                    continue;
                }

                var isNeedAutomaticInjection = instance.GetType().IsNeedAutomaticInjection(parameter);
                if (isNeedAutomaticInjection)//解析服务类
                {
                    var resolveObj = context.ResolveServiceInstance(parameter);

                    parameterObj.Add(resolveObj);
                    continue;
                }

                var isNeedValueInjection = parameter.IsNeedValueInjection();
                if (isNeedValueInjection)//值注入
                {

                    var value = context.ResolveValue(parameter);

                    parameterObj.Add(value);
                    continue;
                }

                if (parameter.HasDefaultValue)
                {
                    parameterObj.Add(parameter.RawDefaultValue);
                    continue;
                }

                if (parameter.IsOptional)
                {
                    parameterObj.Add(Type.Missing);
                    continue;
                }

                if (parameter.IsOut)
                {
                    parameterObj.Add(Type.Missing);
                    continue;
                }

                if (parameter.ParameterType.IsValueType || parameter.ParameterType.IsEnum)
                {
                    parameterObj.Add(parameter.RawDefaultValue);
                    continue;
                }


                //如果拿不到就默认
                context.TryResolve(parameter.ParameterType, out var obj);
                parameterObj.Add(obj);
            }

            return methodInfo.Invoke(instance, parameterObj.ToArray());
        }
    }
}
