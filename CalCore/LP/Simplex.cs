using System;
using System.Collections.Generic;
using System.Text;

namespace CalCore.LP
{
    public static class Simplex
    {
        public class SimplexItem
        {
            public double[] ObjFunc { get; set; } //目标函数向量
            public Matrix Sig { get; set; } //目标函数标准型
            public Matrix Coeff { get; set; } //约束方程矩阵
            public double RHS { get; set; } //最优值
            public int[] RowObjFuncCoeff { get; set; } //行目标函数系数
        }

        /// <summary>
        /// 优化启动器
        /// </summary>
        /// <param name="objFunc">目标函数数组</param>
        /// <param name="coeff">约束系数矩阵</param>
        /// <param name="maxIterate">最大迭代次数</param>
        /// <returns></returns>
        public static Matrix Optimize(double[] objFunc, Matrix coeff, int maxIterate)
        {
            // Initialize
            int rows = coeff.Row, cols = coeff.Col; //设置行列数

            SimplexItem item = new SimplexItem();
            item.ObjFunc = new double[objFunc.Length];
            objFunc.CopyTo(item.ObjFunc, 0);
            item.RowObjFuncCoeff = new int[rows];

            // 初始化Sig值，判断初始是否最优
            item.Sig = new Matrix(1, objFunc.Length);
            for (int i = 0; i < objFunc.Length; i++) item.Sig.Value[0, i] = objFunc[i];
            item.Sig = -item.Sig;

            // 初始化约束方程矩阵
            item.Coeff = new Matrix(coeff);

            // print
            Console.WriteLine("初始化");
            Console.WriteLine(item.Sig.ValueString);
            Console.WriteLine(item.Coeff.ValueString);

            // 迭代循环
            Console.WriteLine("\n开始迭代：");
            int count = 0;
            IterateState state = IterateState.Success; //默认值
            while (item.Sig.Min < 0 && state == 0 && count++ < maxIterate) //只要有值小于0，就继续迭代
            {
                Console.WriteLine($"\n迭代{count}：");
                state = Iterate(item);
            }

            if (state == IterateState.Success)
            {
                Console.WriteLine("最优值：" + item.RHS);

                // 获取解
                int[] baseNum = new int[rows];
                GetRowBV(item.Coeff, baseNum);

                Matrix result = new Matrix(1, coeff.Col - 1); //解的数组(矩阵)
                for (int i = 0; i < rows; i++)
                {
                    result.Set(1, baseNum[i], item.Coeff.Value[i, cols - 1]);
                }
                Console.WriteLine("解向量：\n" + result.ValueString);
                return result;
            }
            else return null; // 求解失败
        }

        /// <summary>
        /// 获取每行的基变量，函数对baseNum直接进行操作
        /// </summary>
        /// <param name="cmt">传入的约束矩阵</param>
        /// <param name="baseNum">传入用于存储基变量的数组</param>
        /// 通过扫描行中1的列确定是否基变量，行i的基变量为Xj
        private static void GetRowBV(Matrix cmt, int[] baseNum)
        {
            int rows = cmt.Row, cols = cmt.Col;

            for (int i = 1; i <= rows; i++) //找每行的基变量
            {
                bool baseFound = false;
                for (int j = 1; j <= cols && !baseFound; j++) //遍历每列
                {
                    if (cmt.Get(i, j) == 1) //目标值为1
                    {
                        //找其他行是否都为0
                        int k;
                        for (k = 1; k <= rows; k++) //遍历该列的每行
                        {
                            if (cmt.Get(k, j) != 0 && k != i) //不是目标行且值不为0
                            {
                                break;
                            }
                        }
                        if (k == rows + 1) //如果行扫描完毕
                        {
                            baseFound = true;
                            baseNum[i - 1] = j;
                            //Console.WriteLine($"行{i}的基变量为X{j}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 对单纯形表对象进行迭代
        /// </summary>
        /// <param name="item">单纯形表对象</param>
        /// <returns>迭代状态对象</returns>
        public static IterateState Iterate(SimplexItem item)
        {
            Console.WriteLine($"输入值:\n{item.Sig.ValueString}\n{item.Coeff.ValueString}");

            int rows = item.Coeff.Row;
            int cols = item.Coeff.Col;
            Matrix cmt = item.Coeff;
            Matrix cb = item.Sig;


            // 找到每行的基变量
            int[] baseNum = new int[rows];
            GetRowBV(cmt, baseNum);


            // 更新检验数行
            // 行 i+1 的系数为 objFuncCoeff[i]
            double[] objFuncCoeff = new double[rows];
            for (int i = 0; i < rows; i++) //计算CB
            {
                objFuncCoeff[i] = item.ObjFunc[baseNum[i] - 1]; //获取对应CB
                //Console.WriteLine($"行{i + 1}的系数为{objFuncCoeff[i]}");
            }
            for (int i = 0; i < cols - 1; i++) //逐列计算检验数Sig
            {
                double sigI = -item.ObjFunc[i];
                for (int j = 0; j < rows; j++) //遍历行
                {
                    sigI += objFuncCoeff[j] * cmt.Get(j + 1, i + 1);
                }
                //Console.WriteLine($"列{i + 1}检验数为{sigI}");
                item.Sig.Value[0, i] = sigI;
            }


            //更新RHS
            double rhsSum = 0;
            for (int i = 0; i < rows; i++)
            {
                rhsSum += objFuncCoeff[i] * cmt.Get(i + 1, cols);
            }
            item.RHS = rhsSum;
            Console.WriteLine("RHS=" + rhsSum);


            // 找到最小的CB值对应的列
            // 最小CB值为{minSig},在第{minSigCol}列
            int minSigCol = 1;
            double minSig = cb.Get(1, 1);
            for (int i = 1; i < cols; i++)
                if (cb.Get(1, i) < minSig)
                {
                    minSig = cb.Get(1, i);
                    minSigCol = i;
                }
            //Console.WriteLine($"最小CB值为{minSig},在第{minSigCol}列");


            // 计算比值，得到最小比值项，对应变量进基
            // 最小theta值为{theta[minThetaRow - 1]},
            // 对应行为{minThetaRow},
            // 对应出基变量为X{baseNum[minThetaRow - 1]}, 此行系数计算后应为1
            double[] theta = new double[rows];
            int minThetaRow = 1;
            for (int i = 1; i <= rows; i++)
            {
                //Console.WriteLine($"theta({i})={cmt.Get(i, cols)}/{cmt.Get(i, minSigCol)}");
                theta[i - 1] = cmt.Get(i, cols) / cmt.Get(i, minSigCol); //b/a （当被除数为0，计算为正无穷）
                if (theta[i - 1] < 0) theta[i - 1] = double.PositiveInfinity; //不允许存在负数
                if (theta[i - 1] < theta[minThetaRow - 1]) minThetaRow = i;
            }
            if (theta[minThetaRow - 1] == double.PositiveInfinity)
            {
                Console.WriteLine("目标函数值在此约束下无界");
                return IterateState.Unbounded; //最小theta值为正无穷，找不到出基变量，无界解。
            }
            Console.WriteLine($"最小theta值为{theta[minThetaRow - 1]},对应行为{minThetaRow},对应出基变量为X{baseNum[minThetaRow - 1]},此行系数计算后应为1");


            // 对应变量出基
            //Console.WriteLine($"需要操作的变量为({minThetaRow},{minSigCol})={cmt.Get(minThetaRow, minSigCol)}");
            double operateValue = cmt.Get(minThetaRow, minSigCol); //行除以需要操作的变量
            int operateRow = minThetaRow, operateCol = minSigCol;
            // 行除变量
            for (int i = 0; i < cols; i++)
            {
                cmt.Value[operateRow - 1, i] /= operateValue;
            }
            //Console.WriteLine("除变量后：\n" + cmt.ValueString);

            // 其他行的所有变量，用他们本身减去他们本身*操作行
            for (int i = 0; i < rows; i++) //所有其他行
            {
                if (i == operateRow - 1) continue; //如果是操作行则跳过
                //获取对应变量，作为操作系数
                operateValue = cmt.Value[i, operateCol - 1];

                //减
                for (int j = 0; j < cols; j++)
                {
                    cmt.Value[i, j] -= operateValue * cmt.Value[operateRow - 1, j];
                }
            }
            //Console.WriteLine("相减后:\n" + cmt.ValueString);

            return IterateState.Success; //迭代成功
        }
    }
}
