using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace calCore_Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CalCore.Class1 cl = new CalCore.Class1();
            textBox2.Text = cl.Multiply(textBox1.Text);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            CalCore.Class1 cl = new CalCore.Class1();
            textBox2.Text = cl.Multiply(textBox1.Text);
        }

        /*public string lowCal(string math)
{
   int symMath = 0;
   if (math == "")
   {
       return "0";
   }
   else
   {
       if (math.Substring(0, 1) != "+" || math.Substring(0, 1) != "-")//开头补符号
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
       double[] eachMath;
       eachMath = new double[symMath];
       mathCache = math;
       string numNC = "";
       int numAC = 0;
       string sym = "";
       while (mathCache != "")
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
}*/
    }
}
