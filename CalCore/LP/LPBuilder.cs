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
        public void AddConstraint(double[] coeff, string sym, double b)
        {
            // 检测符号
            int symId = SymbolTranslator(sym);
            if (symId == -1) throw new ArgumentException($"输入的符号{sym}有误");

            LPBuilderItem item = new LPBuilderItem(coeff, symId, b);
            constraints.Add(item);
        }

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

        public void Solve()
        {
            //搜索非标准型约束的个数
            int geNum = 0; //需要添加辅助变量的个数
            foreach (LPBuilderItem item in constraints)
                if (item.sym != Symbol.EQ)
                    if (item.sym == Symbol.GE) geNum++; // >=需要额外添加一个变量

            //添加辅助变量
            int length = objFunc.Length; //目标函数长度
            Matrix cons = new Matrix(constraints.Count, length + constraints.Count + geNum + 1);
            //max
            int geIndex = 0; //>=约束的编号
            for (int i = 0; i < constraints.Count; i++) //每条约束
            {
                for (int j = 0; j < length; j++) //每个变量
                {
                    cons.Value[i, j] = constraints[i].coeff[j]; //复制约束
                }
                if (constraints[i].sym == Symbol.GE) cons.Value[i, length + geIndex++] = -1; //添加用于求解>=的辅助变量
                cons.Value[i, length + geNum + i] = 1; //辅助变量
                cons.Value[i, cons.Col - 1] = constraints[i].b;
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

            Simplex.Optimize(objFuncCoeff, cons, 100);
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
