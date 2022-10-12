using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 检查字符串匹配 和sql的like一样
        /// </summary>
        /// <param name="str">匹配字符串</param>
        /// <param name="pattern">匹配模式</param>
        /// <returns></returns>
        public static bool Like(this string str, string pattern)
        {
            bool isMatch = true,
                isWildCardOn = false,
                isCharWildCardOn = false,
                isCharSetOn = false,
                isNotCharSetOn = false;
            int lastWildCard = -1;
            int patternIndex = 0;
            List<char> set = new();
            char p = '\0';
            bool endOfPattern;
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                endOfPattern = (patternIndex >= pattern.Length);
                if (!endOfPattern)
                {
                    p = pattern[patternIndex];

                    if (!isWildCardOn && p == '%')
                    {
                        lastWildCard = patternIndex;
                        isWildCardOn = true;
                        while (patternIndex < pattern.Length &&
                               pattern[patternIndex] == '%')
                        {
                            patternIndex++;
                        }

                        if (patternIndex >= pattern.Length) p = '\0';
                        else p = pattern[patternIndex];
                    }
                    else if (p == '_')
                    {
                        isCharWildCardOn = true;
                        patternIndex++;
                    }
                    else if (p == '[')
                    {
                        if (pattern[++patternIndex] == '^')
                        {
                            isNotCharSetOn = true;
                            patternIndex++;
                        }
                        else isCharSetOn = true;

                        set.Clear();
                        if (pattern[patternIndex + 1] == '-' && pattern[patternIndex + 3] == ']')
                        {
                            char start = char.ToUpper(pattern[patternIndex]);
                            patternIndex += 2;
                            char end = char.ToUpper(pattern[patternIndex]);
                            if (start <= end)
                            {
                                for (char ci = start; ci <= end; ci++)
                                {
                                    set.Add(ci);
                                }
                            }

                            patternIndex++;
                        }

                        while (patternIndex < pattern.Length &&
                               pattern[patternIndex] != ']')
                        {
                            set.Add(pattern[patternIndex]);
                            patternIndex++;
                        }

                        patternIndex++;
                    }
                }

                if (isWildCardOn)
                {
                    if (char.ToUpper(c) == char.ToUpper(p))
                    {
                        isWildCardOn = false;
                        patternIndex++;
                    }
                }
                else if (isCharWildCardOn)
                {
                    isCharWildCardOn = false;
                }
                else if (isCharSetOn || isNotCharSetOn)
                {
                    bool charMatch = (set.Contains(char.ToUpper(c)));
                    if ((isNotCharSetOn && charMatch) || (isCharSetOn && !charMatch))
                    {
                        if (lastWildCard >= 0) patternIndex = lastWildCard;
                        else
                        {
                            isMatch = false;
                            break;
                        }
                    }

                    isNotCharSetOn = isCharSetOn = false;
                }
                else
                {
                    if (char.ToUpper(c) == char.ToUpper(p))
                    {
                        patternIndex++;
                    }
                    else
                    {
                        if (lastWildCard >= 0) patternIndex = lastWildCard;
                        else
                        {
                            isMatch = false;
                            break;
                        }
                    }
                }
            }

            endOfPattern = (patternIndex >= pattern.Length);

            if (isMatch && !endOfPattern)
            {
                bool isOnlyWildCards = true;
                for (int i = patternIndex; i < pattern.Length; i++)
                {
                    if (pattern[i] != '%')
                    {
                        isOnlyWildCards = false;
                        break;
                    }
                }

                if (isOnlyWildCards) endOfPattern = true;
            }

            return isMatch && endOfPattern;
        }

        /// <summary>
        /// 从开始处删掉字符串
        /// </summary>
        /// <param name="originString"></param>
        /// <param name="removeString"></param>
        /// <returns></returns>
        public static string RemoveStart(this string originString, string removeString)
        {

            if (string.IsNullOrWhiteSpace(removeString))
                return originString;
            if (!originString.StartsWith(removeString))
                throw new Exception($"字符串不是以{removeString}开头");
            if (originString.Length < removeString.Length)
                throw new Exception($"原始字符串长度小于要删除的字符串长度");

            return originString[removeString.Length..];

        }


        /// <summary>
        /// 从结尾处删掉字符串
        /// </summary>
        /// <param name="originString"></param>
        /// <param name="removeString"></param>
        /// <returns></returns>
        public static string RemoveEnd(this string originString, string removeString)
        {

            if (string.IsNullOrWhiteSpace(removeString))
                return originString;
            if (!originString.EndsWith(removeString))
                throw new Exception($"字符串不是以{removeString}结尾");
            if (originString.Length < removeString.Length)
                throw new Exception($"原始字符串长度小于要删除的字符串长度");



            return originString[..^removeString.Length];

        }


        /// <summary>
        /// 从开始删除某些串 结尾删除某些串
        /// </summary>
        /// <param name="originString"></param>
        /// <param name="removeStartString"></param>
        /// <param name="removeEndString"></param>
        /// <returns></returns>
        public static string RemoveStartAndEnd(this string originString, string removeStartString, string removeEndString)
        {
            return originString.RemoveStart(removeStartString).RemoveEnd(removeEndString);
        }
    }
}
