using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalCore.LP
{
    public static class Simplex
    {
        public class SimplexItem
        {
            public double[] ObjFunc { get; set; } //目标函数向量
            public Matrix Sig { get; set; } //目标函数标准型
            public bool SigUpdate { get; set; } = true; //是否跳过更新检验数行
            public Matrix Coeff { get; set; } //约束方程矩阵
            public double RHS { get; set; } //最优值
            public int[] RowObjFuncCoeff { get; set; } //行目标函数系数
            public int[] BaseNum { get; set; } //基变量
            public Matrix resultArr { get; set; } //解向量
        }

        /// <summary>
        /// 根据求解需求（max/min）求Sigma最值
        /// </summary>
        /// <param name="sigma">Sigma矩阵</param>
        /// <param name="coeff">求max=1，求min=-1</param>
        /// <returns></returns>
        private static double SigmaExtreme(Matrix sigma, int coeff)
        {
            return coeff == 1 ? sigma.Min : sigma.Max;
        }

        /// <summary>
        /// 优化启动器，返回迭代结果。目标函数求最大，约束方程全为等式约束且均带有单位矩阵
        /// </summary>
        /// <param name="objFunc">目标函数数组</param>
        /// <param name="coeff">约束系数矩阵</param>
        /// <param name=isMax">默认求最大，值为1；求最小设为-1</param>
        /// <param name="maxIterate">最大迭代次数</param>
        /// <param name="sig">初始检验数行</param>
        /// <returns>迭代结果，SimplexItem对象，可用于下次迭代</returns>
        public static SimplexItem Optimize(double[] objFunc, Matrix coeff, int isMax = 1, uint? maxIterate = null, double[] sig = null)
        {
            // Initialize
            int rows = coeff.Row, cols = coeff.Col; //设置行列数

            //初始化单纯形表对象
            SimplexItem item = new SimplexItem();
            item.ObjFunc = new double[objFunc.Length];
            objFunc.CopyTo(item.ObjFunc, 0);
            item.RowObjFuncCoeff = new int[rows];

            item.Coeff = new Matrix(coeff); //初始化约束方程矩阵
            item.BaseNum = new int[rows]; //初始化基变量对象

            item.Sig = new Matrix(1, objFunc.Length); //初始化
            if (sig != null) //判断是否输入
            {
                for (int i = 0; i < sig.Length; i++) //复制sig值
                    item.Sig.Value[0, i] = sig[i];
            }
            else
            {
                UpdateSigma(item);
            }

            // print
            Console.WriteLine("初始化");
            Console.WriteLine(new Matrix(objFunc).T().ValueString);
            Console.WriteLine(item.Sig.ValueString);
            Console.WriteLine(item.Coeff.ValueString);

            // 迭代循环
            Console.WriteLine("\n开始迭代：");
            int count = 0;
            IterateState state = IterateState.Success; //默认值

            while (SigmaExtreme(item.Sig, isMax) * isMax < 0 &&
                state == 0 &&
                count++ < (maxIterate ?? double.PositiveInfinity)) //只要有值小于0，就继续迭代
            {
                Console.WriteLine($"\n迭代{count}：");
                state = Iterate(item, isMax);
                Console.WriteLine($"RHS={item.RHS}");
            }

            string msg;
            if (SigmaExtreme(item.Sig, isMax) * isMax >= 0) msg = "找到最优值";
            else if (state != 0) msg = "迭代非成功";
            else msg = "超过最大迭代次数";

            Console.WriteLine("\n结束原因：" + msg);
            Console.WriteLine(item.Sig.ValueString);
            Console.WriteLine(item.Coeff.ValueString);

            if (state != IterateState.Success)
                return null; //求解失败
            else
            {
                //Console.WriteLine("最优值：" + item.RHS);

                // 获取解
                int[] baseNum = new int[rows];
                GetRowBV(item.Coeff, baseNum);

                Matrix result = new Matrix(1, coeff.Col - 1); //解的数组(矩阵)
                for (int i = 0; i < rows; i++)
                {
                    result.Set(1, baseNum[i], item.Coeff.Value[i, cols - 1]);
                }
                item.resultArr = result;
                //Console.WriteLine("解向量：\n" + result.ValueString);

                //获取行对应变量
                double[] objFuncCoeff = new double[rows];
                // 行 i+1 的系数为 objFuncCoeff[i]
                for (int i = 0; i < rows; i++) //计算CB
                {
                    objFuncCoeff[i] = item.ObjFunc[baseNum[i] - 1]; //获取对应CB
                    //Console.WriteLine($"行{i + 1}的系数为{objFuncCoeff[i]}");
                }

                //更新RHS
                double rhsSum = 0;
                for (int i = 0; i < rows; i++)
                {
                    rhsSum += objFuncCoeff[i] * item.Coeff.Get(i + 1, cols);
                }
                item.RHS = rhsSum;

                return item; //返回单纯形表
            }
        }

        /// <summary>
        /// 获取每行的基变量，函数对baseNum直接进行操作
        /// </summary>
        /// <param name="cmt">传入的约束矩阵</param>
        /// <param name="baseNum">传入用于存储基变量的数组</param>
        /// 通过扫描行中1的列确定是否基变量，行i的基变量为Xj
        internal static void GetRowBV(Matrix cmt, int[] baseNum)
        {
            int rows = cmt.Row, cols = cmt.Col;

            for (int i = 1; i <= rows; i++) //找每行的基变量
            {
                bool baseFound = false;
                for (int j = cols - 1; j > 0 && !baseFound; j--) //遍历每列（倒序）
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
        /// 根据SimplexItem的信息更新Sigma值
        /// </summary>
        /// <param name="item">要更新Sigma值的SimplexItem对象</param>
        private static void UpdateSigma(SimplexItem item)
        {
            item.BaseNum = new int[item.Coeff.Col - 1];
            GetRowBV(item.Coeff, item.BaseNum);

            //处理Sig
            //int[] aidBaseNumI = new int[geNum]; //辅助变量的基变量行号
            for (int i = 0; i < item.Coeff.Col - 1; i++)
            {
                item.Sig.Value[0, i] = -item.ObjFunc[i];
                for (int j = 0; j < item.Coeff.Row; j++)
                {
                    item.Sig.Value[0, i] += item.ObjFunc[item.BaseNum[j] - 1] * item.Coeff.Value[j, i]; //CB.*A-C计算检验数
                }
            }
        }

        /// <summary>
        /// 对单纯形表对象进行迭代
        /// </summary>
        /// <param name="item">单纯形表对象</param>
        /// <param name="coeff">可选参数，默认求最大。设为-1求最小</param>
        /// <returns>迭代状态对象</returns>
        public static IterateState Iterate(SimplexItem item, int coeff = 1)
        {
            Console.WriteLine($"输入值:\n{item.Sig.ValueString}\n{item.Coeff.ValueString}");

            int rows = item.Coeff.Row;
            int cols = item.Coeff.Col;
            Matrix cmt = item.Coeff;
            Matrix sig = item.Sig;


            // 找到最小/大的Sigma值对应的列
            // 最小/大Sigma值为{minSig},在第{minSigCol}列
            int minSigCol = 1;
            double minSig = sig.Value[0, 0] * coeff;
            for (int i = 1; i < cols; i++)
                if (sig.Get(1, i) * coeff < minSig)
                {
                    minSig = sig.Get(1, i) * coeff;
                    minSigCol = i;
                }
            //Console.WriteLine($"最{(coeff == 1 ? "小" : "大")}Sig值为{minSig * coeff},在第{minSigCol}列,Sig:\n{sig.ValueString}");

            // 计算比值，得到最小比值项，对应变量进基
            // 最小theta值为{theta[minThetaRow - 1]},
            // 对应行为{minThetaRow},
            // 对应出基变量为X{baseNum[minThetaRow - 1]}, 此行系数计算后应为1
            double[] theta = new double[rows];
            int minThetaRow = 1;
            for (int i = 1; i <= rows; i++)
            {
                theta[i - 1] = cmt.Get(i, cols) / cmt.Get(i, minSigCol); //b/a （当被除数为0，计算为正无穷）
                //Console.WriteLine($"theta({i})={cmt.Get(i, cols)}/{cmt.Get(i, minSigCol)}={theta[i - 1]}");
                if (theta[i - 1] < 0) theta[i - 1] = double.PositiveInfinity; //不允许存在负数（设置为正无穷）
                if (theta[i - 1] < theta[minThetaRow - 1]) minThetaRow = i;
            }
            if (theta[minThetaRow - 1] == double.PositiveInfinity)
            {
                Console.WriteLine("目标函数值在此约束下无界");
                return IterateState.Unbounded; //最小theta值为正无穷，找不到出基变量，无界解。
            }
            //Console.WriteLine($"最小theta值为{theta[minThetaRow - 1]},对应行为{minThetaRow},对应出基变量为X{item.BaseNum[minThetaRow - 1]},此行系数计算后应为1");


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

            // 找到每行的基变量
            GetRowBV(cmt, item.BaseNum);

            double[] objFuncCoeff = new double[rows];
            // 行 i+1 的系数为 objFuncCoeff[i]
            for (int i = 0; i < rows; i++) //计算CB
            {
                objFuncCoeff[i] = item.ObjFunc[item.BaseNum[i] - 1]; //获取对应CB
                //Console.WriteLine($"行{i + 1}的系数为{objFuncCoeff[i]}");
            }


            // 更新检验数行
            if (item.SigUpdate)
            {
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
            }
            else item.SigUpdate = true; //重新打开更新检验行的设置


            //更新RHS
            double rhsSum = 0;
            for (int i = 0; i < rows; i++)
            {
                rhsSum += objFuncCoeff[i] * cmt.Get(i + 1, cols);
            }
            item.RHS = rhsSum;

            return IterateState.Success; //迭代成功
        }
    }
}
