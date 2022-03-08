using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CalCore
{
    public class Core
    {
        public static string Calculate(string formula)//总体计算【总入口】
        {
            //检测有无括号
            if (formula.IndexOf("(") != -1)
            {
                //有括号
                formula = RemoveBrackets(formula);//移除括号
            }
            if (formula.IndexOf("*") != -1)
            {
                //涉及乘法
                formula = MultiplyToPlus(formula);
            }
            return lowCal(formula);//进行最后的加减法运算
        }
        private static string lowCal(string formula)
        {
            /* 对算式进行加法运算
             * 输入的算式如 formula = -12+34+-56-78+90
             * 将算式中多余（重复）的运算符删除，得到
             * formula如 -12+34-56-78+90
             * 
             * 注意：
             * 如果有科学计数法，应该在本步骤前做完
            */

            decimal result = 0; // 初始化结果

            if (formula[0] != '+' && formula[0] != '-') formula.Insert(0, "+");// 补齐开头符号

            // 替换重复的运算符
            formula = formula.Replace("+-", "-");
            formula = formula.Replace("-+", "-");

            string figureCache = "0"; // 初始化存放数字的缓存变量(如存放+23或者-30)
            foreach (char c in formula)
            {
                if (c == '+' || c == '-')
                {
                    // 是加减号
                    result += Convert.ToDecimal(figureCache); //将上一个变量输入结果
                    figureCache = c.ToString(); // 首字符换成符号
                }
                else
                {
                    // 不是加减号
                    figureCache += c; // 继续添加数字
                }
            }
            result += Convert.ToDecimal(figureCache); //最后一个变量输入结果

            return result.ToString();
        }

        private static string MultiplyToPlus(string formula) //乘除法级别处理【乘除法→加法】(无判断是否需要其它处理）
        {
            /* 对算式中的乘除法进行处理（替换）
             * 假设预处理后输入的formula为：
             * formula = 1+2*3+4/5*6-7+8*9
             * 进行分割  1,2*3,4/5*6,7,8*9
             * 查看算式块中有没有乘除运算符，没有的直接设为“”，有的进行计算，存入新List
             * 结果与原formula中的对应算式替换
             * 
             * 注意：
             * 匹配乘法算式的正则表达式：(\+|-|)(\d+\.?\d*|\d*\.?\d+)((\*|/)(\+|-|)(\d+\.?\d*|\d*\.?\d+))+
             * 其中，实数的正则表达式为：(\d+\.?\d*|\d*\.?\d+)
             * ※ 是否需要使用StringBuilder?
             */

            // 算式预处理
            formula = formula.TrimStart('+', '-'); // 将头部的加减运算符去掉，以免干扰算式分块

            // 正则表达式匹配
            string regexFormula = @"(\+|-|)(\d+\.?\d*|\d*\.?\d+)((\*|/)(\+|-|)(\d+\.?\d*|\d*\.?\d+))+";
            var matchedFormulas = Regex.Matches(formula,regexFormula);

            string[] formulaBlocks = new string[matchedFormulas.Count]; // 创建已知大小的算式数组
            string[] processBlocks = new string[matchedFormulas.Count]; // 声明结果数组，大小与算式数组相同

            for (int i = 0; i < matchedFormulas.Count; i++)
            {
                formulaBlocks[i] = matchedFormulas[i].Value;
            }

            formulaBlocks.CopyTo(processBlocks, 0); // 复制算式数组的值到结果数组

            for (int i = 0; i < processBlocks.Length; i++)
            {
                processBlocks[i] = CalMultiply(processBlocks[i]); // 计算算式值
            }

            for (int i = 0; i < formulaBlocks.Length; i++)
            {
                formula = formula.Replace(formulaBlocks[i], processBlocks[i]);
            }

            return formula;
        }

        private static string CalMultiply(string formula) //计算乘除法【只有乘除法的算式】
        {
            /* 用于计算仅含有乘除法的算式，如
             * 3*4/5*6
             */
            string figure = "";
            char sym = ' '; // 临时存放符号
            decimal result=0;

            for (int i = 0; i < formula.Length; i++)
            {
                if (sym == ' ') // 无符号
                {
                    if (formula[i] != '*' && formula[i] != '/')
                        figure+=formula[i]; // 字符不是符号，填入数字
                    else
                    {
                        result = Convert.ToDecimal(figure); // 字符是符号，结果基底填入数字
                        figure = ""; // 清空figure的缓存
                        sym = formula[i]; // 得到第一个符号
                    }
                }
                else
                {
                    // 已经有符号
                    if ((formula[i] == '*') || (formula[i] == '/'))
                    {
                        // 是符号
                        switch(sym) // 结算旧符号
                        {
                            case '*':
                                result *= Convert.ToDecimal(figure); // 执行乘法
                                break;
                            case '/':
                                result /= Convert.ToDecimal(figure); // 执行除法
                                break;
                        }
                        figure = ""; // 清空figure的缓存
                        sym= formula[i]; // 填入新符号
                    }
                    else
                    {
                        // 不是符号
                        figure+=formula[i];
                    }
                }
            }

            // 遍历结束，收尾工作
            switch (sym) // 结算旧符号
            {
                case '*':
                    result *= Convert.ToDecimal(figure); // 执行乘法
                    break;
                case '/':
                    result /= Convert.ToDecimal(figure); // 执行除法
                    break;
            }

            if (result > 0)
                return '+' + result.ToString(); // 结果为正数，末尾加0
            else
                return result.ToString(); // 结果为负数，末尾不加0
        }

        private static string RemoveBrackets(string formula)//移除括号(无判断是否需要处理)
        {
            while (formula.IndexOfAny("(".ToCharArray()) != -1)//如果存在括号
            {
                int leftBraketLocation = formula.LastIndexOf("(");
                int detedctorLocation;
                while (formula.LastIndexOf("(") == leftBraketLocation)
                {
                    detedctorLocation = formula.LastIndexOf(")");
                    int stepRecorder = 0;
                    string detectString = formula.Substring(leftBraketLocation, detedctorLocation - leftBraketLocation);
                    foreach (char c in detectString)
                    {
                        if (c == ')')
                        {
                            break;
                        }
                        else
                        {
                            stepRecorder++;
                        }
                    }
                    detedctorLocation = leftBraketLocation + stepRecorder + 1;
                    string block = formula.Substring(leftBraketLocation, detedctorLocation - leftBraketLocation);
                    formula = formula.Replace(block, lowCal(block.Substring(1, block.Length - 2)));
                }
                //Console.WriteLine(formula);
            }
            return formula;
        }

        private static string List2String(List<string> list)
        {
            string result = "";
            foreach (string str in list)
            {
                result += str;
            }
            return result;
        }

        private void DebugList(List<string> list)//Debug专用
        {
            //PASS
            foreach (string str in list)
            {
                Console.WriteLine(str);
            }
            Console.WriteLine("DEBUG ENDED");

        }

    }
}
