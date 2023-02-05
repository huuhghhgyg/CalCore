using System;
using System.Collections.Generic;
using System.Text;

namespace CalCore
{
    internal static class Format
    {
        /// <summary>
        /// 将数组转换为矩阵样式的string类型变量，以便输出
        /// </summary>
        /// <typeparam name="T">实数类型</typeparam>
        /// <param name="array">数组</param>
        /// <returns>Matrix类默认输出格式的值</returns>
        internal static string ArrayString<T>(T[] array)
        {
            StringBuilder sb = new StringBuilder("[");

            foreach (T t in array)
            {
                sb.Append($"{t}\t");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(']');

            return sb.ToString();
        }
    }
}
