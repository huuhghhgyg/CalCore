using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CalCore.Data
{
    /// <summary>
    /// 指标正向化及标准化
    /// </summary>
    public static class Normalize
    {
        /// <summary>
        /// 指标正向化，极小型指标转化为极大型指标
        /// </summary>
        /// <param name="matrix">指标矩阵</param>
        /// <param name="col">需要从极小型指标转化为极大型指标的列</param>
        public static void NormalizeFromMin(Matrix matrix, int col)
        {
            double max = matrix.GetCol(col).Max;
            for (int i = 1; i <= matrix.Row; i++)
            {
                matrix.Set(i, col, max - matrix.Get(i, col));
            }
        }

        public static void NormalizeFromMin(Matrix matrix, int[] cols)
        {
            for(int i = 0; i<cols.Length;i++)
                NormalizeFromMin(matrix, cols[i]);
        }

        /// <summary>
        /// 指标正向化，中间型转化为极大型指标
        /// </summary>
        /// <param name="matrix">指标矩阵</param>
        /// <param name="val">中间值</param>
        /// <param name="col">需要从极小型指标转化为中间型指标的列</param>
        public static void NormalizeFromVal(Matrix matrix, double val, int col)
        {
            double[] values = new double[matrix.Row]; //保存计算值

            values[0] = Math.Abs(matrix.Get(1, col) - val); //计算用于比较的初值
            double max = values[0];
            for (int i = 1; i < matrix.Row; i++) //计算值
            {
                values[i] = Math.Abs(matrix.Value[i, col - 1] - val);
                if (values[i] > max) max = values[i];
            }

            for (int i = 0; i < matrix.Row; i++)
            {
                matrix.Value[i, col - 1] = 1 - values[i] / max;
            }
        }

        /// <summary>
        /// 指标正向化，区间型转化为极大型指标
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="lb">区间上界</param>
        /// <param name="ub">区间下界</param>
        /// <param name="col">需要从极小型指标转化为中间型指标的列</param>
        public static void NormalizeFromRange(Matrix matrix, double lb, double ub, int col)
        {
            double max = matrix.Get(1, col), min = max;
            for (int i = 0; i < matrix.Row; i++)
            {
                double value = matrix.Value[i, col - 1];
                if (value < min) min = value;
                if (value > max) max = value;
            }

            double maxVal = lb - min > max - ub ? lb - min : max - ub; //(lb-min(matrix), max(matrix)-ub)

            for (int i = 0; i < matrix.Row; i++)
            {
                double value = matrix.Value[i, col - 1];
                if (value > ub) matrix.Value[i, col - 1] = 1 - (value - ub) / maxVal;
                else if (value < lb) matrix.Value[i, col - 1] = 1 - (lb - value) / maxVal;
                else matrix.Value[i, col - 1] = 1;
            }
        }

        /// <summary>
        /// 标准化矩阵，消去每列不同量纲的影响
        /// </summary>
        /// <param name="matrix">含有两个及以上指标(列)的矩阵</param>
        public static void Standardize(Matrix matrix)
        {
            for (int i = 1; i <= matrix.Col; i++)
            {
                double sum = 0;
                for (int j = 1; j <= matrix.Row; j++)
                {
                    sum += Math.Pow(matrix.Get(j, i), 2);
                }
                sum = Math.Sqrt(sum); //开根号

                for (int j = 1; j <= matrix.Row; j++)
                {
                    matrix.Set(j, i, matrix.Get(j, i) / sum);
                }
            }
        }
    }
}
