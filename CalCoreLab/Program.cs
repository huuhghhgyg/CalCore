using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace CalCoreLab
{
    class Program
    {
        //"+12+23*34*1-45/-5"
        //"1+((1+2)+(1+2))+(2+3)"
        static string formula = "1+1+1+3-2";
        static void Main(string[] args)
        {
            //string result = MultiplyClassify(formula);
            //Console.WriteLine(result);
            Console.WriteLine(MultiplyToPlus(formula));
            Console.ReadLine();
        }

        static string MultiplyToPlus(string formula)//乘法级别的分类【乘除法→加法】(无判断是否需要处理）
        {
            List<string> formulaBlocks = new List<string>();
            string formulaBlocksCache = "";
            foreach (char letter in formula)
            {
                if (letter.ToString().IndexOfAny("+-".ToArray()) != -1)//是+-
                {
                    if (formulaBlocksCache != "")//cache不为空【】
                    {
                        formulaBlocks.Add(formulaBlocksCache);//添加内容
                    }
                    formulaBlocksCache = letter.ToString();//改成+-
                }
                else if (letter.ToString().IndexOfAny("*/".ToArray()) != -1)
                {//乘除号
                    formulaBlocksCache += letter.ToString();
                    formulaBlocks.Add(formulaBlocksCache);//添加内容
                    formulaBlocksCache = "";
                }
                else
                {
                    if (formulaBlocksCache == "") //为空【】2
                    {
                        formulaBlocksCache = "+";
                    }
                    formulaBlocksCache += letter.ToString();//数字或加减号【+2】3
                }
            }

            formulaBlocks.Add(formulaBlocksCache);//保存最后一FormulaBlock

            DebugList(formulaBlocks);

            for (int i = 0; i < formulaBlocks.Count; i++)//处理乘除号
            {
                string block = formulaBlocks[i];
                if (block.EndsWith("*") || block.EndsWith("/"))//+33* -66
                {
                    int index = formulaBlocks.IndexOf(block);//找到block的指针位置
                    string symbol = block.Substring(block.Length - 1, 1);
                    string result;
                    bool again = false;//连乘或除

                    if (symbol == "*")
                    {
                        string nextItem = formulaBlocks[index + 1];//下一项
                        if (nextItem.EndsWith("*"))
                        {//下一项也是乘法
                            result = (double.Parse(block.Substring(0, block.Length - 1))
                            * double.Parse(nextItem.Substring(0, nextItem.Length - 1))).ToString() + "*";
                            again = true;
                        }
                        else
                        {//普通计算乘法
                            result = (double.Parse(block.Substring(0, block.Length - 1))
                                    * double.Parse(nextItem)).ToString();
                        }
                    }
                    else
                    {
                        string nextItem = formulaBlocks[index + 1];//下一项
                        if (nextItem.EndsWith("/"))
                        {//下一项也是除法
                            result = (double.Parse(block.Substring(0, block.Length - 1))
                                                        / double.Parse(nextItem.Substring(0, nextItem.Length - 1))).ToString() + "*";
                            again = true;
                        }
                        else
                        {//普通计算除法
                            result = (double.Parse(block.Substring(0, block.Length - 1))
                                    / double.Parse(formulaBlocks[index + 1])).ToString();
                        }
                    }

                    if (!result.StartsWith("-"))//修饰开头
                    {
                        result = "+" + result;
                    }

                    formulaBlocks[index + 1] = result;
                    formulaBlocks.Remove(block);
                    if (again == true)
                    {
                        i--;
                    }
                }
            }
            return List2String(formulaBlocks);
        }

        static string RemoveBrackets(string formula)//(无判断是否需要处理)
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
                        if (c.ToString() == ")")
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

        public static string List2String(List<string> list)
        {
            string result = "";
            foreach (string str in list)
            {
                result += str;
            }
            return result;
        }


        private static void DebugList(List<string> list)
        {
            //PASS
            foreach (string str in list)
            {
                Console.WriteLine(str);
            }
            Console.WriteLine("DEBUG ENDED");

        }

        private static string lowCal(string formula)//循环*1
        {
            string cache = "";
            double result = 0;
            string _char;
            int num = 0;

            foreach (char each in formula)//遍历算式中的每个字符
            {
                _char = each.ToString();//格式化
                if (_char == "E")
                {
                    //cache += formula.Substring(0, 1);
                    //formula = formula.Substring(2, formula.Length - 1);
                    num = 1;
                }
                if (_char.IndexOfAny("+-".ToArray()) != -1 && cache != "" && num != 0)
                {
                    result += Convert.ToDouble(cache);
                    cache = _char;
                }
                else
                {
                    cache += _char;
                }
                num--;
            }
            result += Convert.ToDouble(cache);
            return result.ToString();
        }

    }
}
