using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CalCore
{
    public class Matrix
    {
        public Matrix(int n, int m) //创建一个n行m列的矩阵
        {
            Value = new double[n, m];
        }
        public Matrix(double[,] value)
        {
            Value = value;
        }

        public double[,] Value { get; set; } //存储矩阵的值
        public int Row { get => Value.GetLength(0); } //行数
        public int Col { get => Value.GetLength(1); } //列数

        #region 属性
        /// <summary>
        /// 将矩阵以String形式输出
        /// </summary>
        public string ValueString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                int i, j = 0;
                for (i = 0; i < Row; i++)
                {
                    sb.Append('['); //添加开头元素
                    for (j = 0; j < Col; j++)
                        sb.Append($"{Value[i, j]} "); //添加元素（带空格）
                    if (j > 0) sb.Remove(sb.Length - 1, 1); //如果元素数量不为空，删除最后一个空格
                    sb.Append("]\n"); //添加结尾元素
                }
                if (i > 0) //如果元素数量不为空，删除最后一个换行
                    sb.Remove(sb.Length - 1, 1);

                if (sb.Length == 0) //无值
                    return "[]";

                return sb.ToString();
            }
        }

        /// <summary>
        /// 获取矩阵中的最大值
        /// </summary>
        public double Max
        {
            get
            {
                double result = double.NegativeInfinity; //将结果设定为一个极小值
                for (int i = 0; i < Row; i++)
                    for (int j = 0; j < Col; j++)
                        if (Value[i, j] > result) result = Value[i, j];
                return result;
            }
        }

        /// <summary>
        /// 获取矩阵中的最小值
        /// </summary>
        public double Min
        {
            get
            {
                double result = double.PositiveInfinity; //将结果设定为一个极大值
                for (int i = 0; i < Row; i++)
                    for (int j = 0; j < Col; j++)
                        if (Value[i, j] < result) result = Value[i, j];
                return result;
            }
        }

        #endregion

        #region 运算
        public static Matrix operator +(Matrix a) => a; //正号
        public static Matrix operator -(Matrix a) //负号
        {
            for (int i = 0; i < a.Row; i++)
                for (int j = 0; j < a.Col; j++)
                    a.Value[i, j] = -a.Value[i, j];
            return a;
        }
        public static Matrix operator +(Matrix a, Matrix b) //矩阵相加（有ref特性，不能直接在a上面计算并返回值）
        {
            if (a.Row == b.Row && a.Col == b.Col) //判断矩阵大小是否相等
            {
                Matrix result = new Matrix(a.Row, a.Col);
                for (int i = 0; i < a.Row; i++)
                    for (int j = 0; j < a.Col; j++)
                        result.Value[i, j] = a.Value[i, j] + b.Value[i, j];
                return result;
            }
            else throw new Exception($"矩阵大小不同无法进行此类型运算:[{a.Row},{a.Col}]*[{b.Row},{b.Col}]");
        }
        public static Matrix operator -(Matrix a, Matrix b) => a + -b; //矩阵相减，调用加法方法
        public static Matrix operator +(Matrix a, double n) //矩阵加实数（快捷方法）
        {
            for (int i = 0; i < a.Row; i++)
                for (int j = 0; j < a.Col; j++)
                    a.Value[i, j] += n;
            return a;
        }
        public static Matrix operator +(double n, Matrix a) => a + n; //交换律映射（矩阵加实数）
        public static Matrix operator -(Matrix a, double n) => a + -n;
        public static Matrix operator -(double n, Matrix a) => n + -a;
        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.Col == b.Row)
            {
                Matrix result = new Matrix(a.Row, b.Col);
                for (int i = 0; i < a.Row; i++) //a的所有行
                {
                    for (int j = 0; j < b.Col; j++) //b的所有列
                    {
                        double element = 0; //矩阵中的数字
                        for (int k = 0; k < a.Col; k++) //取所有数值
                        {
                            element += a.Value[i, k] * b.Value[k, j];
                        }
                        result.Value[i, j] = element;
                    }
                }
                return result;
            }
            else
            {
                throw new Exception($"矩阵乘法的维度不正确:[{a.Row},{a.Col}]*[{b.Row},{b.Col}]");
            }
        }
        public static Matrix operator *(Matrix a, double n) //矩阵中所有元素乘数
        {
            for (int i = 0; i < a.Row; i++)
                for (int j = 0; j < a.Col; j++)
                    a.Value[i, j] *= n;
            return a;
        }
        public static Matrix operator *(double n, Matrix a) => a * n; //交换律
        #endregion

        #region
        public override string ToString() => ValueString; //习惯使用的函数
        public Matrix T() //矩阵转置（步骤）
        {
            Matrix result = new Matrix(Col, Row);
            for (int i = 0; i < Row; i++)
                for (int j = 0; j < Col; j++)
                    result.Value[j, i] = Value[i, j];
            Value = result.Value; //导入转置矩阵
            return this;
        }

        /// <summary>
        /// 从指定的行列元素位置开始截取子矩阵
        /// </summary>
        /// <param name="r">开始截取行</param>
        /// <param name="c">开始截取列</param>
        /// <param name="m">截取行数</param>
        /// <param name="n">截取列数</param>
        /// <returns>截取的矩阵</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Matrix GetSubMatrix(int r, int c, int m, int n)
        {
            if (r >= 1 && c >= 1 && r <= Row && c <= Col) //判断参照元素位置
            {
                r -= 1; c -= 1; //转化为index
                if (r + m <= Row && c + n <= Col) //判断截取范围的合理性
                {
                    Matrix result = new Matrix(m, n);
                    for (int valueRow = r, i = 0; i < m; valueRow++, i++)
                        for (int valueCol = c, j = 0; j < n; valueCol++, j++)
                            result.Value[i, j] = Value[valueRow, valueCol];
                    return result;
                }
                else throw new ArgumentOutOfRangeException("截取的矩阵的范围超过了矩阵本身");
            }
            else throw new ArgumentOutOfRangeException("参照元素位置不存在于矩阵内");
        }
        //此处其实可以引用GetSubMatrix，但是出于执行代码量考虑就重新写了
        /// <summary>
        /// 从输入的矩阵中截取多行
        /// </summary>
        /// <param name="startRow">起始行</param>
        /// <param name="n">截取行数</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public Matrix GetRows(int startRow, int n)
        {
            if (startRow > 0 && startRow <= Row) //检验起始行
            {
                if (startRow + n - 1 <= Row) //检验截取范围
                {
                    Matrix result = new Matrix(n, Col);
                    for (int r = startRow - 1, i = 0; i < n; r++, i++) //i,j为返回结果的index计数
                        for (int j = 0; j < Col; j++)
                            result.Value[i, j] = Value[r, j];
                    return result;
                }
                else throw new ArgumentOutOfRangeException("截取的矩阵的范围超过了矩阵本身");
            }
            else throw new ArgumentException("起始行的位置不存在");
        }
        public Matrix GetRow(int row) => GetRows(row, 1); //截取某一列

        /// <summary>
        /// 从输入的矩阵中截取多列
        /// </summary>
        /// <param name="startCol">起始列</param>
        /// <param name="n">截取列数</param>
        /// <returns>截取的多列矩阵</returns>
        /// <exception cref="ArgumentOutOfRangeException">截取的矩阵的范围超过了矩阵本身</exception>
        /// <exception cref="ArgumentException">起始行的位置不存在</exception>
        public Matrix GetCols(int startCol, int n)
        {
            if (startCol > 0 && startCol <= Col) //检验起始行
            {
                if (startCol + n - 1 <= Col) //检验截取范围
                {
                    Matrix result = new Matrix(Row, n);
                    for (int c = startCol - 1, j = 0; j < n; c++, j++) //i,j为返回结果的index计数
                        for (int i = 0; i < Row; i++)
                            result.Value[i, j] = Value[i, c];
                    return result;
                }
                else throw new ArgumentOutOfRangeException("截取的矩阵的范围超过了矩阵本身");
            }
            else throw new ArgumentException("起始行的位置不存在");
        }
        public Matrix GetCol(int col) => GetCols(col, 1); //截取某一行

        /// <summary>
        /// 使用数学意义上的行和列获取矩阵中的值
        /// </summary>
        /// <param name="row">行，从1开始</param>
        /// <param name="col">列，从1开始</param>
        /// <returns>矩阵指定位置的数值</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public double GetValue(int row, int col) //使用数学的理解获取值
        {
            if (row < 1 || row > Row || col < 1 || col > Col)
                throw new ArgumentOutOfRangeException("指定位置不存在于此矩阵内");
            else return Value[row - 1, col - 1];
        }

        #endregion
    }
}
