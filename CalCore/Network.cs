﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CalCore
{
    public static class Network
    {
        private static double ShortestPathLayerAdd(Matrix rowVec, Matrix colVec) //最短路径矩阵乘法式求和计算最小值（对应位置点乘，非左右乘）
        {
            //条件检测
            if (rowVec.Row != 1) throw new ArgumentException("输入的第一个参数不是行向量");
            if (colVec.Col != 1) throw new ArgumentException("输入的第二个参数不是列向量");
            if (rowVec.Col != colVec.Row) throw new ArgumentException("输入的行向量和列向量元素数量不同，无法计算");

            //计算流程
            double min = double.PositiveInfinity;
            for (int i = 0; i < rowVec.Col; i++)
            {
                //获取数值
                double a = rowVec.Value[0, i];
                double b = colVec.Value[i, 0];

                if (double.IsPositiveInfinity(a) || double.IsPositiveInfinity(b)) continue; //如果其中一个是无穷，就不用算了
                double calResult = a + b; //点乘计算
                if (min > calResult) min = calResult; //替换已知最小值
            }
            return min;
        }

        /// <summary>
        /// 输入路径矩阵，返回第一个点到所有点的最短路长行向量
        /// </summary>
        /// <param name="pathMatrix">点到点的路径矩阵（方阵）</param>
        /// <returns>第一个点到所有点的最短距离行向量</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Matrix ShortestPath2AllPoints(Matrix pathMatrix)
        {
            //条件检测
            if (pathMatrix.Row != pathMatrix.Col) throw new ArgumentException("输入的距离矩阵不是方阵，无法计算");

            //计算流程
            Matrix result = pathMatrix.GetRow(1); //获取开始点到所有点的距离行向量

            for (int i = 0; i < pathMatrix.Row - 1; i++) //计算次数最多n-1次
            {
                int calNum = 0; //计算值变化计数
                //单次计算
                for (int j = 0; j < pathMatrix.Row; j++)
                {
                    //每列计算
                    double multiplyResult = ShortestPathLayerAdd(result, pathMatrix.GetCol(j + 1)); //求点乘最小值
                    if (multiplyResult != result.Value[0, j])
                    {
                        calNum++; //如果计算结果变化，计数
                        result.Value[0, j] = multiplyResult; //替换结果
                    }
                }
                if (calNum == 0) break; //计算结果没有变化，结束计算
            }
            return result;
        }

        /// <summary>
        /// 输入路径矩阵，返回所有点到最后一个点的最短路长列向量
        /// </summary>
        /// <param name="pathMatrix">点到点的路径矩阵（方阵）</param>
        /// <returns>所有点到最后一个点的最短距离列向量</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Matrix ShortestPath2LastPoint(Matrix pathMatrix)
        {
            //条件检测
            if (pathMatrix.Row != pathMatrix.Col) throw new ArgumentException("输入的距离矩阵不是方阵，无法计算");

            //计算流程
            Matrix result = pathMatrix.GetCol(pathMatrix.Col); //获取所有点到最终点的距离列向量

            for (int i = 0; i < pathMatrix.Row - 1; i++) //计算次数最多n-1次
            {
                int calNum = 0; //计算值变化计数
                //单次计算
                for (int j = 0; j < pathMatrix.Row; j++)
                {
                    //每列计算
                    double multiplyResult = ShortestPathLayerAdd(pathMatrix.GetRow(j + 1), result); //求点乘最小值
                    if (multiplyResult != result.Value[j, 0])
                    {
                        calNum++; //如果计算结果变化，计数
                        result.Value[j, 0] = multiplyResult; //替换结果
                    }
                }
                if (calNum == 0) break; //计算结果没有变化，结束计算
            }
            return result;
        }

        /// <summary>
        /// 获取最小支撑树
        /// </summary>
        /// <param name="pathMatrix">上三角距离方阵(默认非有向图)，不存在的路径使用正无穷表示</param>
        /// <returns>与距离矩阵大小相同的0-1矩阵，表示选中的边</returns>
        public static Matrix GetSpanningTree(Matrix pathMatrix)
        {
            if (pathMatrix.Row != pathMatrix.Col) throw new ArgumentException("输入的矩阵不是方阵，无法计算");

            Matrix result = new Matrix(pathMatrix.Row, pathMatrix.Col); //结果为0-1变量矩阵
            int size = pathMatrix.Row; //获取矩阵的行数/列数

            //清空下三角矩阵
            for (int i = 0; i < size; i++)
                for (int j = 0; j < i; j++)
                    pathMatrix.Value[i, j] = double.PositiveInfinity;

            int[] knownPoints = new int[size]; //创建已知点集
            int[] pointsCheck = new int[size]; //点的状态集合
            knownPoints[0] = 0; //默认第一个点为已知点
            pointsCheck[0] = 1; //第一个点已包含


            for (int i = 0; i < pathMatrix.Row - 1; i++) //最小支撑树共n-1条边
            {
                Matrix newRow = pathMatrix.GetRow(knownPoints[i] + 1);
                Matrix minMatrix;
                int minRow, minCol;
                //创建一个与原数组相同的数组
                Matrix newMatrix = new Matrix(pathMatrix);

                //矩阵过滤
                for (int j = 0; j < newMatrix.Row; j++)
                    for (int k = 0; k < newMatrix.Col; k++)
                    {
                        if (pointsCheck[j] == 1 && pointsCheck[k] == 1 || //已知点通向已知点
                            pointsCheck[j] == 0 && pointsCheck[k] == 0) //未知点通向未知点
                        {
                            //屏蔽值
                            newMatrix.Value[j, k] = double.PositiveInfinity;
                        }
                    }

                //求最小值，即为可行最小值
                minMatrix = newMatrix.GetMinList();
                minRow = (int)minMatrix.Value[minMatrix.Row - 1, 0] - 1;
                minCol = (int)minMatrix.Value[minMatrix.Row - 1, 1] - 1;

                result.Value[minRow, minCol] = 1; //选中边
                pathMatrix.Value[minRow, minCol] = double.PositiveInfinity; //消除
                if (pointsCheck[minRow] == 0)
                {
                    knownPoints[i + 1] = minRow; pointsCheck[minRow] = 1; //已知点集中填入编号index，作为下一轮的对象
                }
                else
                {
                    knownPoints[i + 1] = minCol; pointsCheck[minCol] = 1; //确认本次发现的点
                }
            }

            return result;
        }
    }
}
