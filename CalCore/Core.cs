using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalCore
{
    public class Core
    {
        public string lowCal(string formula)
        {
            string cache="";
            double result = 0;
            string _char;
            int num = 0;

            foreach (char each in formula)
            {
                _char = each.ToString();
                if (_char == "E")
                {
                    //cache += formula.Substring(0, 1);
                    //formula = formula.Substring(2, formula.Length - 1);
                    num = 1;
                }
                if (_char.IndexOfAny("+-".ToArray()) != -1 && cache != "" && num!=0)
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

        /*public string lowCal(string math)
        {
            //try
            //{
            int symMath = 0;
            if (math == "")//输入值为空，返回0
            {
                return "0";
            }
            else
            {
                if (math.Substring(0, 1) != "+" && math.Substring(0, 1) != "-")//开头补符号
                {
                    math = "+" + math;
                }
                string mathCache = math;
                while (mathCache != "")//检测数值个数
                {
                    if (mathCache.Substring(0, 1) == "+" || mathCache.Substring(0, 1) == "-")
                    {
                        symMath++;
                    }
                    mathCache = mathCache.Substring(1, mathCache.Length - 1);
                }
                double[] eachMath;//每个数值相加得结果
                eachMath = new double[symMath];//初始化数组
                mathCache = math;//将"草稿math"值进行抄写
                string numNC = "";//数值暂储
                int numAC = 0;//累计加减次数
                string sym = "";//符号变量初始化
                while (mathCache != "")//填充
                {
                    if (mathCache.Substring(0, 1) == "+" || mathCache.Substring(0, 1) == "-")
                    {
                        if (sym == "")//符号
                        {
                            sym = mathCache.Substring(0, 1);
                        }
                        else
                        {
                            eachMath[numAC] = Convert.ToDouble(sym + numNC);
                            numNC = "";
                            numAC++;
                            sym = mathCache.Substring(0, 1);
                        }
                    }
                    else
                    {
                        if (mathCache.Substring(0, 1) == "E")
                        {
                            numNC += mathCache.Substring(0, 1);
                            mathCache = mathCache + "+0";
                            mathCache = mathCache.Substring(1, mathCache.Length - 1);
                        }
                        numNC += mathCache.Substring(0, 1);
                    }
                    mathCache = mathCache.Substring(1, mathCache.Length - 1);
                }
                eachMath[numAC] = Convert.ToDouble(sym + numNC);

                //叠加所有数值
                double calCache = 0;
                for (int i = 0; i != symMath; i++)//有问题，暂留
                {
                    calCache += eachMath[i];
                }
                return calCache.ToString();
            }
            //}
            /*catch
            {
                return "Error";//返回报告未知错误
            }
        }*/


        public string Multiply(string formula,bool calpow)
        {
            if (calpow == true)
            {
                formula = calPow(formula);
            }
            //try
            {
                ////////////////检测开头有没有符号，没有就加上
                if (formula[0].ToString().IndexOfAny("+-".ToArray()) ==-1)
                {
                    formula = "+" + formula;
                }

                ///////////////记乘除号
                string formulaTry = formula;
                int mulNum = 0;//乘除号个数

                string LastChar="";
                int GroupNumber = 0;
                foreach(char c in formula)//formula里的每个字符
                {
                    if (c.ToString().IndexOfAny("*/+-".ToArray()) != -1)
                    {
                        GroupNumber++;
                        if (c.ToString().IndexOfAny("*/".ToArray()) != -1)
                        {
                            mulNum++;
                        }
                    }
                    else
                    {
                        if(LastChar.ToString().IndexOfAny("*/+-".ToArray()) != -1)
                        {
                            GroupNumber++;
                        }
                    }
                    LastChar = c.ToString();
                }
                int mulN = mulNum;//后面乘除法做准备

                ///////////////分组【先读数据后加计数】
                /*
                +12+23*34-45/-56=+12+23*+34-45/-56
                [+12],[+23],[*](标记),[+34],[-45],[/](标记),[-56]
                */
                string[] grp;//组
                grp = new string[GroupNumber];//上限，99999个数组
                int[] muNum;//乘除号所在组号
                muNum = new int[mulNum];
                formulaTry = formula;//算式的备份
                bool firstChk = false;//第一次分组
                string saveCache = "";//暂存变量
                int gn = 0;//组数
                mulNum = 0;//记乘号位置
                           //while (formulaTry.Substring(formulaTry.IndexOf("*"),1)!="+"&& formulaTry.Substring(formulaTry.IndexOf("*"), 1) != "-")//检测*/后面的符号
                while (formulaTry.Length != 0)//分组
                {
                    if (firstChk == false)//第一次的符号或者数字
                    {
                        firstChk = true;
                        saveCache += formulaTry[0].ToString();
                        formulaTry = formulaTry.Substring(1, formulaTry.Length - 1);//去掉第一个字符
                    }
                    if (formulaTry[0].ToString().IndexOfAny("*/".ToArray())!=-1 )
                    {//检测是否乘除号，是的话先 【检测*/后面是否+-号，否则加上+】
                     //存前面的，再存自己，true=false，计数++,乘除数计数器++，计入数组

                        //是乘号 ((((((需修改))))))
                        if (formulaTry.Substring(1, 1) != "+" && formulaTry.Substring(1, 1) != "-")//后面有没有+-，没有？
                        {//*5
                            formulaTry = formulaTry[0].ToString() + "+" + formulaTry.Substring(1, formulaTry.Length - 1);
                        }
                        grp[gn] = saveCache;
                        saveCache = "";
                        gn++;//等会需要加入组，组数先加1【计数++】
                        muNum[mulNum] = gn;
                        mulNum++;
                        grp[gn] = formulaTry[0].ToString();
                        gn++;
                        firstChk = false;
                    }
                    else
                    {
                        if ((formulaTry[0].ToString().IndexOfAny("+-".ToArray())!=-1))//检测是否加减号,是的话先把前面的cache存入数组再把自己加入cache，计数++
                        {
                            grp[gn] = saveCache;
                            gn++;//等会需要加入组，组数先加1【计数++】
                            saveCache = formulaTry[0].ToString();
                        }
                        else
                        {//数字或小数点
                            saveCache += formulaTry[0].ToString();
                        }
                    }
                    formulaTry = formulaTry.Substring(1, formulaTry.Length - 1);//去掉第一个字符
                }
                grp[gn] = saveCache;
                ////////////////做乘除法
                int doMul = 0;
                while (doMul < mulNum)
                {
                    grp[muNum[doMul] + 1] = calMu(grp[muNum[doMul] - 1], grp[muNum[doMul]], grp[muNum[doMul] + 1]);
                    grp[muNum[doMul] - 1] = "+0"; grp[muNum[doMul]] = "+0";
                    doMul++;
                }

                ///////////////滚雪球
                doMul = 1;
                while (doMul <= gn)
                {
                    grp[doMul] = grp[doMul - 1] + grp[doMul];
                    grp[doMul - 1] = "0";
                    doMul++;
                }
                return lowCal(grp[gn]);
            }
            //catch
            {
                //return "Error";
            }
        }

        public string calPow(string formula)
        {
            string forBK = formula;
            string addUp = "", fir = "", sec = "";
            while (forBK.Length != 0)
            {
                if (forBK[0].ToString() == "^")
                {//检测到次方符号
                    while (forBK[0].ToString() != "+" && forBK[0].ToString() != "-" && forBK[0].ToString() != "*" && forBK[0].ToString() != "/")
                    {
                        forBK = forBK.Substring(1, forBK.Length - 1);
                        if (forBK != "")
                        {
                            if (forBK[0].ToString() != "+" && forBK[0].ToString() != "-" && forBK[0].ToString() != "*" && forBK[0].ToString() != "/" && forBK.Substring(0,1)!="^")
                            {
                                sec += forBK[0].ToString();
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    addUp += (Math.Pow(Convert.ToDouble(fir), Convert.ToDouble(sec))).ToString();
                    forBK = addUp + forBK;
                    addUp = "";
                    fir = ""; sec = "";
                }
                if (forBK.Length != 0)
                {
                    if (forBK[0].ToString().IndexOfAny("+-*/".ToArray())!=-1)
                    {
                        addUp += fir;
                        fir = "";
                        addUp += forBK[0].ToString();
                    }
                    else
                    {
                        fir += forBK[0].ToString();
                    }
                    forBK = forBK.Substring(1, forBK.Length - 1);
                }
            }
            if (fir.Length != 0)
            {
                addUp += fir;
            }
            return addUp;
        }

        private string calMu(string fir, string sym, string sec)
        {
            string res = "";
            if (sym == "*")
            {
                res = (Convert.ToDouble(fir) * Convert.ToDouble(sec)).ToString();
            }
            else
            {
                if (sym == "/")
                {
                    res = (Convert.ToDouble(fir) / Convert.ToDouble(sec)).ToString();
                }
                else
                {
                    res = Math.Pow(Convert.ToDouble(fir), Convert.ToDouble(sec)).ToString();
                }
            }
            return res;
        }


        public string plus(string fir,string sym,string sec)
        {
            switch (sym)
            {
                case "*":
                    return (Convert.ToDouble(fir) * Convert.ToDouble(sec)).ToString();

                case "/":
                    return (Convert.ToDouble(fir) / Convert.ToDouble(sec)).ToString();
            }
            return "0";
        }

        public string adCal(string formula)
        {
            //try
            //{
                string forCache = formula;//备份变量

                /*int Knum = 0;//括号对数
                while (forCache != "")//检测括号对数
                {
                    if (forCache[0].ToString() == "(")
                    {
                        Knum++;
                    }
                    forCache = forCache.Substring(1, forCache.Length - 1);
                }*/

                ////////////////////////////格式化
                int mube=0;//MultiplyBefore?
                string cache1, cache2 = "";
                while (mube!=formula.Length)
                {
                    //try
                    {
                        if (forCache.Substring(mube, 1) == "(" && forCache.Substring(mube-1,1)!="*")
                        {
                            cache1 = formula.Substring(0, mube) + "*";
                            cache2 = formula.Substring(mube, formula.Length - mube);
                            formula = cache1 + cache2;
                            forCache = formula;
                        }
                    }
                    //catch{}
                    mube++;
                }
                ////////////////////////////

                int klm = 0;//左括号位置最大值
                forCache = formula;
                int loc = 0;//位置计数器
                int rloc = 0;//右括号位置

                string left = "", k = "", right = "";
                //string mayEnd = "";
                while (forCache.Length != 0)
                {
                    if (forCache[0].ToString() == "(")
                    {
                        klm = loc + 1;//纪录最大的左括号位置
                        forCache = forCache.Substring(1, forCache.Length - 1);
                        loc++;
                    }
                    else
                    {
                        if (forCache[0].ToString() == ")")
                        {
                            rloc = loc + 1;//纪录右括号位置
                            left = formula.Substring(0, klm - 1);//左部分
                            k = formula.Substring(klm, rloc - klm - 1);//中部
                            right = formula.Substring(loc + 1, formula.Length - rloc);//右部
                            k = Multiply(k,true);
                            forCache = left + k + right;
                            klm = 0; loc = 0; rloc = 0;
                            //mayEnd = forCache;
                            formula = forCache;
                        }
                        else
                        {
                            forCache = forCache.Substring(1, forCache.Length - 1);
                            loc++;
                        }
                    }
                }
                return Multiply(formula,true);
            /*}
            catch
            {
                return "Error";
            }*/
        }
    }
}
