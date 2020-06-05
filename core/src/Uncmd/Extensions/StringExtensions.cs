using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    /// <summary>
    /// <seealso cref="string"/>扩展类
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 字符串是否为null或空字符串("")
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 字符串是否为null、空字符串("")或只包含空白字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 从末尾删除斜杠
        /// </summary>
        /// <param name="value">The value.</param>
        public static string RemoveSlashFromEnd(this string value)
            => value.TrimEnd().EndsWith("/")
            ? value.TrimEnd().TrimEnd('/')
            : value;

        /// <summary>
        /// 添加斜杠到末尾
        /// </summary>
        public static string WithShashEnding(this string value)
            => !value.TrimEnd().EndsWith("/")
            ? value + "/"
            : value;
    }
}
