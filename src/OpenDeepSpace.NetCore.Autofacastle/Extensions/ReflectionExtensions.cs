using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// 获取方法的唯一string
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        public static string GetMethodInfoUniqueName(this MethodInfo methodInfo)
        {
            string signatureString = string.Join(",", methodInfo.GetParameters().Select(p => p.ParameterType.Name).ToArray());
            string returnTypeName = methodInfo.ReturnType.Name;

            if (methodInfo.IsGenericMethod)
            {
                string typeParamsString = string.Join(",", methodInfo.GetGenericArguments().Select(g => g.AssemblyQualifiedName).ToArray());


                // returns a string like this: "Assembly.YourSolution.YourProject.YourClass:YourMethod(Param1TypeName,...,ParamNTypeName):ReturnTypeName
                return $"{methodInfo.DeclaringType.Namespace + methodInfo.DeclaringType.Name}:{methodInfo.Name}<{typeParamsString}>({signatureString}):{returnTypeName}";
            }

            return $"{methodInfo.DeclaringType.Namespace + methodInfo.DeclaringType.Name}:{methodInfo.Name}({signatureString}):{returnTypeName}";
        }

        /// <summary>
        /// Get all the method of a class instance
        /// </summary>
        /// <param name="type">Type object of that class</param>
        /// <param name="getBaseType">is get baseType methods</param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> GetAllInstanceMethod(this Type type, bool getBaseType = true)
        {
            if (type == null)
            {
                return Enumerable.Empty<MethodInfo>();
            }

            BindingFlags flags = BindingFlags.Public |
                                 BindingFlags.NonPublic |
                                 BindingFlags.Instance |
                                 BindingFlags.DeclaredOnly;

            if (!getBaseType) return type.GetMethods(flags).Where(m => !m.IsSpecialName);
            return type.GetMethods(flags).Where(m => !m.IsSpecialName).Union(GetAllMethod(type.BaseType,getBaseType));
        }

        /// <summary>
        /// Get all the method of a class
        /// </summary>
        /// <param name="type">Type object of that class</param>
        /// <param name="getBaseType">is get baseType methods</param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> GetAllMethod(this Type type, bool getBaseType = true)
        {
            if (type == null || type == typeof(object))
            {
                return Enumerable.Empty<MethodInfo>();
            }

            BindingFlags flags = BindingFlags.Public |
                                 BindingFlags.NonPublic |
                                 BindingFlags.Static |
                                 BindingFlags.Instance |
                                 BindingFlags.DeclaredOnly;

            if (!getBaseType) return type.GetMethods(flags).Where(m => !m.IsSpecialName);
            return type.GetMethods(flags).Where(m => !m.IsSpecialName).Union(GetAllMethod(type.BaseType,getBaseType));
        }

        /// <summary>
        /// 获取一个类所有方法上的特性
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Attribute> GetMethodsAttributes(this Type type)
        {

            return type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(m => !m.IsSpecialName).SelectMany(t => t.GetCustomAttributes());
        }


        /// <summary>
        /// 修复类型 获取到的泛型接口FullName为null
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type FixTypeReference(this Type type)
        {
            if (type.FullName != null)
                return type;

            string typeQualifiedName = type.DeclaringType != null
                ? type.DeclaringType.FullName + "+" + type.Name + "," + type.Assembly.FullName
                : type.Namespace + "." + type.Name + "," + type.Assembly.FullName;

            return Type.GetType(typeQualifiedName, true);
        }
    }
}
