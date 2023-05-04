using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        }

        #region 属性
        /// <summary>
        /// 目标函数求最大(默认)或最小
        /// </summary>
        private Target type { get; set; }
        /// <summary>
        /// 目标函数系数数组
        /// </summary>
        private double[] objFunc { get; set; }
        /// <summary>
        /// 约束方程列表
        /// </summary>
        private List<LPBuilderItem> constraints { get; set; }
        /// <summary>
        /// 最大迭代次数（private）
        /// </summary>
        private uint? maxIterate = null;
        /// <summary>
        /// 最大迭代次数访问器（public）
        /// </summary>
        public uint? MaxIterate
        {
            get => maxIterate;
            set => maxIterate = value;
        }
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
        /// 将标准形式矩阵导入模型(约束方程符号为<=)
        /// </summary>
        /// <param name="constraints">系数和右端项组成的的二维数组</param>
        public void SetConstraints(double[,] constraints)
        {
            int row = constraints.GetLength(0), col = constraints.GetLength(1);

            for (int i = 0; i < row; i++)
            {
                double[] cons = new double[col - 1]; //定义系数
                for (int j = 0; j < col - 1; j++) cons[j] = constraints[i, j]; //复制系数
                double b = constraints[i, col - 1]; //右端项
                AddConstraint(cons, "<=", b);
            }
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
                case "≤": return 0;
                case "==": return 1;
                case "=": return 1;
                case ">=": return 2;
                case "≥": return 2;
                default: return -1; //未识别
            }
        }

        /// <summary>
        /// 创建标准化约束矩阵并返回
        /// </summary>
        /// <returns>标准化后的系数矩阵</returns>
        private Matrix StandardizeConstraints()
        {
            //搜索非标准型约束的个数
            int geNum = 0, eqNum = 0; //需要添加辅助变量的个数
            foreach (LPBuilderItem item in constraints)
                if (item.sym != Symbol.EQ)
                    if (item.sym == Symbol.GE) geNum++; // >=需要额外添加辅助变量用于求解

            //添加平衡变量和辅助变量
            Matrix cons = new Matrix(constraints.Count, objFunc.Length + constraints.Count + geNum + 1); //矩阵大小
            //max
            int geIndex = 0; //>=约束的编号
            int[] eqConsIndex = new int[objFunc.Length + constraints.Count]; //记录==约束的位置的数组
            for (int i = 0; i < constraints.Count; i++) //每条约束
            {
                for (int j = 0; j < objFunc.Length; j++) //每个变量
                {
                    cons.Value[i, j] = constraints[i].coeff[j]; //复制约束
                }

                if (constraints[i].sym == Symbol.GE)
                {
                    cons.Value[i, objFunc.Length + i] = -1; //添加平衡变量(>=)
                    cons.Value[i, objFunc.Length + constraints.Count + geIndex++] = 1; //添加辅助变量
                }
                else
                {
                    if (constraints[i].sym == Symbol.EQ)
                    {
                        eqConsIndex[objFunc.Length + i] = 1; //==需要预求解
                        eqNum++;
                    }
                    cons.Value[i, objFunc.Length + i] = 1; //添加平衡变量(<=或==)
                }
                cons.Value[i, cons.Col - 1] = constraints[i].b;
            }

            // 检查是否需要进行第一阶段求解
            IterateState state = SolveFirstStage(ref cons, ref geNum, ref eqNum, eqConsIndex);

            return state == IterateState.Success ? cons : null;
        }

        /// <summary>
        /// 根据输入的参数判断是否需要进行第一阶段的求解。如果需要，则会对传入的系数矩阵进行修改。
        /// </summary>
        /// <param name="cons">系数矩阵</param>
        /// <param name="geNum">大于等于约束的条数</param>
        /// <param name="eqNum">等号约束的条数</param>
        /// <param name="eqConsIndex">记录等号约束的位置的数组</param>
        private IterateState SolveFirstStage(ref Matrix cons, ref int geNum, ref int eqNum, int[] eqConsIndex)
        {
            //发送第一阶段的求解
            if (geNum > 0 || eqNum > 0) //出现>=和==的时候需要预求解。由于==号加入的为辅助变量，因此也需要出基。
            {
                Console.WriteLine($"准备发送第一阶段的求解矩阵：\n{cons.ValueString}");
                double[] objFuncS1 = new double[cons.Col - 1];
                //目标函数求min
                for (int i = objFunc.Length + constraints.Count; i < cons.Col - 1; i++) //设置第一阶段求解的目标函数
                    objFuncS1[i] = 1;
                Array.Copy(eqConsIndex, objFuncS1, eqConsIndex.Length); //复制==辅助变量的位置

                int[] baseNums = new int[constraints.Count]; //找到每行的基变量
                Simplex.GetRowBV(cons, baseNums);

                //处理Sig
                //int[] aidBaseNumI = new int[geNum]; //辅助变量的基变量行号
                double[] sig = new double[cons.Col - 1];
                for (int i = 0; i < cons.Col - 1; i++)
                {
                    sig[i] = -objFuncS1[i]; //-C
                    for (int j = 0; j < cons.Row; j++)
                    {
                        sig[i] += objFuncS1[baseNums[j] - 1] * cons.Value[j, i]; //CB.*A-C计算检验数
                    }
                }

                Simplex.SimplexItem simplexItem0 = Simplex.Optimize(objFuncS1, cons, -1, maxIterate, sig);

                //返回结果
                Console.WriteLine($"第一阶段求解值={simplexItem0.RHS},第一阶段{(simplexItem0.RHS == 0 ? "有最优解" : "无最优解")}");
                if (simplexItem0.RHS != 0) //无最优解，返回信息结束求解
                {
                    Console.WriteLine("该问题无最优解");
                    return IterateState.Infeasible;
                }

                //将迭代得到的系数直接嵌入矩阵中
                //其中会包含最后一列，为预留的b列空间
                Matrix coeff0 = simplexItem0.Coeff; //映射方便操作
                cons = new Matrix(coeff0.GetCols(1, objFunc.Length + constraints.Count + 1));
                //填入b值
                for (int i = 1; i <= cons.Row; i++)
                    cons.Set(i, cons.Col, coeff0.Get(i, coeff0.Col));
            }

            return IterateState.Success; //可以继续求解
        }

        /// <summary>
        /// 对LP对象进行求解
        /// </summary>
        /// <param name="maxIterate">最大迭代数</param>
        public string Solve(uint? maxIterate = null)
        {
            //迭代前的合规性检查
            //每个constraints长度与目标函数长度一致
            for(int i = 0; i < constraints.Count; i++)
            //foreach(LPBuilderItem item in constraints)
            {
                if (constraints[i].coeff.Length != objFunc.Length)
                    throw new ArgumentException($"第{i + 1}个约束方程系数个数（{constraints[i].coeff.Length}）与目标函数（{objFunc.Length}）不一致");
            }

            this.maxIterate = maxIterate; //设置迭代次数上限

            //标准化矩阵，并检测是否需要进行第一阶段求解
            Matrix cons = StandardizeConstraints();

            if (cons == null) return "第一阶段处理失败，求解停止"; //信息被截留，求解停止 

            //设置目标函数
            double[] objFuncCoeff = new double[cons.Col - 1];
            Array.Copy(objFunc, objFuncCoeff, objFunc.Length);

            //发送求解
            //debug
            Console.WriteLine("发送求解的目标函数和约束方程：");
            Matrix objfmtx = new Matrix(1, objFuncCoeff.Length);
            for (int i = 0; i < objFuncCoeff.Length; i++) objfmtx.Value[0, i] = objFuncCoeff[i];
            Console.WriteLine($"{objfmtx.ValueString}");
            Console.WriteLine(cons.ValueString);

            int isMax = type == Target.max ? 1 : -1; //确定迭代系数
            Simplex.SimplexItem simplexItem = Simplex.Optimize(objFuncCoeff, cons, isMax, maxIterate);

            string output;
            if (simplexItem == null || simplexItem.resultArr == null)
                output = "求解失败";
            else
                output = $"最优值RHS={simplexItem.RHS}\n解向量：\n{simplexItem.resultArr.ValueString}\nSigma：\n{new Matrix(simplexItem.Sig).T().ValueString}";

            Console.WriteLine(output);
            return output;
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
