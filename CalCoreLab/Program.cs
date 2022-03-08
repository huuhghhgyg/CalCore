using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using CalCore;



namespace CalCoreLab
{
    class Program
    {
        //"+12+23*34*1-45/-5"
        //"1+((1+2)+(1+2))+(2+3)"
        static string formula = "+1+2*3+4/5*6-7+8*9+100*0.1*-1*-1*-10*-0.1";
        static void Main(string[] args)
        {
            string result = CalCore.Core.Calculate(formula);
            Console.WriteLine(result);
            Console.WriteLine();
        }
    }
}
