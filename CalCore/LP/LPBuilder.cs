using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CalCore.LP
{
    public class LPBuilder
    {
        /// <summary>
        /// 使用目标函数系数数组初始化LPBuilder
        /// </summary>
        /// <param name="objFunction">目标函数系数数组</param>
        /// <param name="objType">目标函数求最大或最小，最大用max表示，最小用min表示</param>
        public LPBuilder(string objType, double[] objFunction)
        {
            //识别求最大或最小
            if (objType != "max" && objType != "min") throw new ArgumentException($"输入的目标函数求解种类{objType}有误");
            else type = objType == "max" ? Target.max : Target.min;

            //初始化变量
            objFunc = new double[objFunction.Length];
            constraints = new List<LPBuilderItem>();

            //复制目标函数
            Array.Copy(objFunction, objFunc, objFunction.Length);

            //改变目标函数符号
            if (type == Target.min)
                for (int i = 0; i < objFunc.Length; i++) objFunc[i] *= -1;
        }

        #region 属性
        private Target type { get; set; } //目标函数求最大(默认)或最小
        private double[] objFunc { get; set; } //目标函数
        private List<LPBuilderItem> constraints { get; set; } //约束方程列表
        #endregion

        #region 函数
        /// <summary>
        /// 为模型添加约束
        /// </summary>
        /// <param name="coeff">约束方程系数</param>
        /// <param name="sym">约束方程的符号，"<="、"=="或">="</param>
        /// <param name="b">约束方程右端项</param>
        /// <exception cref="ArgumentException"></exception>
        public void AddConstraint(double[] coeff, string sym, double b)
        {
            // 检测符号
            int symId = SymbolTranslator(sym);
            if (symId == -1) throw new ArgumentException($"输入的符号{sym}有误");

            //复制数组
            double[] coeffNew = new double[coeff.Length];
            Array.Copy(coeff, coeffNew, coeff.Length);

            //检测右端项是否为标准型（大于0），并修正
            if (b < 0)
            {
                for (int i = 0; i < coeff.Length; i++) coeffNew[i] *= -1; //系数全部*-1
                if (symId != 1) symId = symId == 0 ? 2 : 0; //非等号需要变号
                b *= -1; //右端项*-1
            }

            LPBuilderItem item = new LPBuilderItem(coeffNew, symId, b);
            constraints.Add(item);
        }

        /// <summary>
        /// 将string类型的约束方程符号转换为对应的Symbol类型的int类型变量
        /// </summary>
        /// <param name="sym"></param>
        /// <returns>对应Symbol类型的变量。返回-1表示未识别成功</returns>
        private int SymbolTranslator(string sym)
        {
            switch (sym)
            {
                case "<=": return 0;
                case "==": return 1;
                case ">=": return 2;
                default: return -1; //未识别
            }
        }

        /// <summary>
        /// 对LP对象进行求解
        /// </summary>
        public void Solve()
        {
            //搜索非标准型约束的个数
            int geNum = 0; //需要添加辅助变量的个数
            foreach (LPBuilderItem item in constraints)
                if (item.sym != Symbol.EQ)
                    if (item.sym == Symbol.GE) geNum++; // >=需要额外添加辅助变量用于求解

            //添加平衡变量和辅助变量
            int length = objFunc.Length; //目标函数长度
            Matrix cons = new Matrix(constraints.Count, length + constraints.Count + geNum + 1); //矩阵大小
            //max
            int geIndex = 0; //>=约束的编号
            for (int i = 0; i < constraints.Count; i++) //每条约束
            {
                for (int j = 0; j < length; j++) //每个变量
                {
                    cons.Value[i, j] = constraints[i].coeff[j]; //复制约束
                }
                //if (constraints[i].sym == Symbol.GE) cons.Value[i, length + geIndex++] = -1; //添加用于求解>=的辅助变量
                //cons.Value[i, length + geNum + i] = 1; //辅助变量
                if (constraints[i].sym == Symbol.GE)
                {
                    cons.Value[i, length + i] = -1; //添加平衡变量(>=)
                    cons.Value[i, length + constraints.Count + geIndex++] = 1; //添加辅助变量
                    //Console.WriteLine($"ge行:\n{cons.ValueString}");
                }
                else cons.Value[i, length + i] = 1; //添加平衡变量(<=或==)
                cons.Value[i, cons.Col - 1] = constraints[i].b;
            }
            //发送第一阶段的求解
            if (geNum > 0) //只有出现>=的时候需要预求解
            {
                Console.WriteLine($"准备发送第一阶段的求解矩阵：\n{cons.ValueString}");
                double[] objFuncS1 = new double[cons.Col - 1];
                for (int i = length + constraints.Count; i < cons.Col - 1; i++) //设置第一阶段求解的目标函数
                    objFuncS1[i] = -1; //求min

                int[] baseNums = new int[constraints.Count]; //找到每行的基变量
                Simplex.GetRowBV(cons, baseNums);

                //处理Sig
                //int[] aidBaseNumI = new int[geNum]; //辅助变量的基变量行号
                double[] sig = new double[cons.Col - 1];
                for (int k = 0; k < cons.Col - 1; k++) sig[k] += objFuncS1[k];
                for (int i = 0, j = 0; j < geNum; i++) //填充辅助变量列表
                {
                    if (baseNums[i] > length + constraints.Count)
                    {
                        //aidBaseNumI[j++] = i; //填充行号
                        j++;
                        for (int k = 0; k < cons.Col - 1; k++) sig[k] += cons.Value[i, k];
                    }
                }
                Simplex.SimplexItem simplexItem0 = Simplex.Optimize(objFuncS1, cons, -1, 5, sig);

                Console.WriteLine($"第一阶段求解值={simplexItem0.RHS},第一阶段{(simplexItem0.RHS == 0 ? "有最优解" : "无最优解")}");
                if (simplexItem0.RHS != 0)
                    return; //无最优解，直接结束求解
                else
                {
                    //将迭代得到的系数直接嵌入矩阵中
                    //其中会包含最后一列，为预留的b列空间
                    Matrix coeff0 = simplexItem0.Coeff; //映射方便操作
                    cons = new Matrix(coeff0.GetCols(1, length + constraints.Count + 1));
                    //填入b值
                    for (int i = 1; i <= cons.Row; i++)
                        cons.Set(i, cons.Col, coeff0.Get(i, coeff0.Col));
                }
            }

            //设置目标函数
            double[] objFuncCoeff = new double[cons.Col - 1];
            Array.Copy(objFunc, objFuncCoeff, objFunc.Length);

            //发送求解
            //debug
            Console.WriteLine("发送求解：");
            Matrix objfmtx = new Matrix(1, objFuncCoeff.Length);
            for (int i = 0; i < objFuncCoeff.Length; i++) objfmtx.Value[0, i] = objFuncCoeff[i];
            Console.WriteLine(objfmtx.ValueString);
            Console.WriteLine(cons.ValueString);

            Simplex.SimplexItem simplexItem = Simplex.Optimize(objFuncCoeff, cons, 1, 100);
            if (simplexItem != null && simplexItem.resultArr != null)
            {
                Console.WriteLine($"最优值RHS={simplexItem.RHS * (type == Target.min ? -1 : 1)}");
                Console.WriteLine($"解向量：\n{simplexItem.resultArr.ValueString}");
            }
            else Console.WriteLine("求解失败");
        }
        #endregion
    }

    internal class LPBuilderItem
    {
        public LPBuilderItem(double[] coeff, int symId, double b)
        {
            this.coeff = coeff;
            this.sym = (Symbol)symId;
            this.b = b;
        }

        public double[] coeff { get; set; } //系数
        public Symbol sym { get; set; } //符号
        public double b { get; set; } //右端项
    }
}
