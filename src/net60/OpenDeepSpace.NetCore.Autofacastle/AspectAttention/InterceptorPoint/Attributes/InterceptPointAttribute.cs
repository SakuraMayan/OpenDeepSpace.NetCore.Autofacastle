using OpenDeepSpace.NetCore.Autofacastle.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention.InterceptorPoint.Attributes
{
    /// <summary>
    /// 拦截点
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class InterceptPointAttribute : Attribute
    {
        //拦截表达式判断
        private static readonly Regex RegexInterceptExpression = new(@"^intercept[(][\?%*._a-zA-Z0-9]+[\s+][\?%*._a-zA-Z0-9]+[\s+][\?%*._a-zA-Z0-9]+[\s+][\?%*._a-zA-Z0-9]+[\s+]([(][?%*.,_a-zA-Z0-9]+[)]|[(][)])[)]");

        /// <summary>
        /// 分组
        /// </summary>
        public string GroupName { get; set; } = "";

        /// <summary>
        /// 排序
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 拦截表达式
        /// </summary>
        private readonly string _interceptExpression;

        /// <summary>
        /// 用于匹配返回值类型
        /// </summary>
        private readonly string _returnType;

        /// <summary>
        /// 返回值类型
        /// </summary>
        private readonly string returnType;

        /// <summary>
        /// 用于匹配命名空间类型
        /// </summary>
        private readonly string _nameSpace;

        /// <summary>
        /// class的名称
        /// </summary>
        private readonly string _class;

        /// <summary>
        /// 方法名称
        /// </summary>
        private readonly string _method;

        /// <summary>
        /// 方法参数类型
        /// </summary>
        private readonly string methodParamsType;

        /// <summary>
        /// 方法参数类型字典 key为方法参数下标位置 v为对应的参数类型
        /// </summary>
        private readonly Dictionary<int, string> methodParamsTypeDic = new();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="InterceptExpression">
        /// 拦截表达式
        /// intercept(返回值 命名空间 类名 方法 (参数类型))
        /// *表示任意
        /// ?表示一个
        /// ..在参数中表示任意参数 ()表示空参数 非任意参数要注意参数的类型按照顺序
        /// </param>
        public InterceptPointAttribute(string InterceptExpression)
        {
            //判断是否符合表达式
            _interceptExpression = InterceptExpression.TrimStart().TrimEnd();//去掉前后的空格

            //判断拦截表达式是否符合要求
            if (!RegexInterceptExpression.IsMatch(_interceptExpression))
            {
                throw new Exception($"拦截表达式:{_interceptExpression}不正确,形式应该如:intercept(返回值 命名空间 类名 方法 (参数))");
            }

            _interceptExpression = _interceptExpression.Replace("*", "%").Replace("?", "_");

            //解析出 返回值 命名空间 类名 参数
            //去掉拦截主体 intercept()
            var interceptBody = _interceptExpression.RemoveStartAndEnd("intercept(", ")");

            //通过空格分割出
            var bodies = interceptBody.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            _returnType = bodies[0];
            _nameSpace = bodies[1];
            _class = bodies[2];
            _method = bodies[3];
            methodParamsType = bodies[4];

            //判断返回值类型能否转换
            if (_returnType != "%") //不为任意返回值
            {
                returnType = _returnType;
            }


            //方法参数类型按逗号分割
            var splitParams = methodParamsType.RemoveStartAndEnd("(", ")").Split(",", StringSplitOptions.RemoveEmptyEntries);
            //如果方法参数类型分割出来大于1 并且某个包含..即任意个数参数类型 不合法 比如 System.Threading.Tasks.Task,..
            if (splitParams.Length > 1 && splitParams.Any(t => t == ".."))
                throw new Exception("任意个数参数类型不能与具体参数混合使用");

            if (splitParams.Length >= 1 && splitParams.All(t => t != ".."))
            {
                //筛选出任意参数类型 % 
                for (var i = 0; i < splitParams.Length; i++)
                {
                    if (splitParams[i] == "%") //某个位置的参数为任意参数 例如 *,system.int
                    {

                    }
                    else//非任意参数
                    {

                    }
                    methodParamsTypeDic[i] = splitParams[i];

                }
            }
        }

        /// <summary>
        /// 是否当前class满足
        /// </summary>
        /// <returns></returns>
        internal bool IsValidClass(Type classType)
        {
            //本类命名空间为空直接返回
            if (string.IsNullOrWhiteSpace(classType.Namespace))
                return false;

            //匹配命名空间
            if (!classType.Namespace.Like(this._nameSpace))
            {
                return false;
            }

            //配置了class
            if (!classType.Name.Like(this._class))
            {
                return false;
            }


            return true;
        }

        /// <summary>
        /// 是否method可用
        /// </summary>
        /// <returns></returns>
        internal bool IsVaildMethod(MethodInfo methodInfo)
        {

            //非任意方法名
            //方法名匹配
            if (!methodInfo.Name.Like(this._method))
            {
                return false;
            }
            if (methodInfo.ReturnType.FullName == null)
                return false;

            //返回值匹配
            if (returnType != null && !methodInfo.ReturnType.FullName.Equals(returnType, StringComparison.OrdinalIgnoreCase))//存在具体参数类型比较
                return false;

            //方法参数类型匹配
            var methodInfoParams = methodInfo.GetParameters();

            //如果是任意个数的参数类型..不用判断
            if (!(methodParamsType == "(..)"))
            {
                //非任意参数 空参数() 但是当前方法存在参数 返回false
                if (methodParamsType == "()" && methodInfoParams.Any())
                    return false;
                //非任意参数 非空参
                //判断参数个数
                if (methodParamsTypeDic.Keys.Count != methodInfoParams.Length)//个数不等
                    return false;

                //个数相同 判断各个位置参数类型
                for (int i = 0; i < methodParamsTypeDic.Keys.Count; i++)
                {

                    //如果是任意参数直接continue
                    if (methodParamsTypeDic[i] == "%")
                        continue;

                    if (methodInfoParams[i].ParameterType.FullName == null)
                        return false;

                    //否则比较参数类型 不相同返回false
                    if (!methodParamsTypeDic[i].Equals(methodInfoParams[i].ParameterType.FullName, StringComparison.OrdinalIgnoreCase)) //相同位置的参数类型不相同
                        return false;
                }
            }

            return true;
        }
    }
}
