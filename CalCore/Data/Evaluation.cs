using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CalCore.Data
{
    public static class Evaluation
    {
        /// <summary>
        /// 输入进行过指标正向化和标准化的矩阵，返回所有样本的得分。
        /// </summary>
        /// <param name="matrix">指标矩阵</param>
        /// <param name="weight">权重数组，要求其长度和指标矩阵的列数相同</param>
        /// <returns>矩阵样本的得分数组</returns>
        /// <exception cref="ArgumentException">输入权重错误</exception>
        public static double[] TOPSIS_Score(Matrix matrix, double[] weight = null)
        {
            bool weightExist = false;
            if (weight != null)
            {
                if (weight.Length != matrix.Col)
                    throw new ArgumentException($"输入的权重长度不正确。长度应为{matrix.Col}而非{weight.Length}");
                else
                    weightExist = true;
            }

            //求所有指标的最值
            double[] maxV = new double[matrix.Col];
            double[] minV = new double[matrix.Col];

            for (int i = 1; i <= matrix.Col; i++)
            {
                Matrix mt = matrix.GetCol(i);
                maxV[i - 1] = mt.Max;
                minV[i - 1] = mt.Min;
            }

            //求每个评价对象与最值的距离
            double[] maxD = new double[matrix.Row];
            double[] minD = new double[matrix.Row];
            //求每个对象的得分
            double[] scores = new double[matrix.Row];

            for (int i = 0; i < matrix.Row; i++) //每个对象
            {
                for (int j = 0; j < matrix.Col; j++) //对象的指标与指标最值的距离
                {
                    double maxVal = Math.Pow(maxV[j] - matrix.Value[i, j], 2);
                    double minVal = Math.Pow(minV[j] - matrix.Value[i, j], 2);

                    //如果有输入权重，乘以权重
                    if (weightExist)
                    {
                        maxVal *= weight[j];
                        minVal *= weight[j];
                    }

                    //累加
                    maxD[i] += maxVal;
                    minD[i] += minVal;
                }

                //计算距离（matrix.Col维）
                maxD[i] = Math.Sqrt(maxD[i]);
                minD[i] = Math.Sqrt(minD[i]);

                //计算得分
                scores[i] = minD[i] / (maxD[i] + minD[i]);
            }

            //归一化
            double scoreSum = 0;
            foreach (double val in scores) scoreSum += val; //求和
            for(int i = 0; i < scores.Length; i++)
            {
                scores[i] /= scoreSum;
            }

            return scores;
        }
    }
}
