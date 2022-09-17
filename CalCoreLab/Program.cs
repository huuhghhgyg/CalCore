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
        static void Main(string[] args)
        {
            //string result = CalCore.Core.Calculate(formula);

            Matrix mt1 = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } });
            Matrix mt2 = new Matrix(new double[,] { { 2, 1 }, { 3, 2 }, { 2, 4 } });

            Console.WriteLine($"{mt1.ValueString}\n");
            Console.WriteLine($"{mt2.ValueString}\n");

            Console.WriteLine($"{(mt1 * mt2).ValueString}\n");
            //mt1.T();
            Console.WriteLine($"{mt1.T().ValueString}\n");

            Console.WriteLine($"{mt1.T().ValueString}\n");
            Console.WriteLine($"{mt1.GetSubMatrix(1, 1, 2, 2).ValueString}\n");
            Console.WriteLine($"{mt1.GetCols(2, 2).ValueString}\n"); //截取列
            Console.WriteLine($"{mt1.GetCol(2).ValueString}\n"); //截取列
            Console.WriteLine($"{mt1.GetValue(1, 2)}\n");

            //Console.WriteLine($"{(mt1-mt2).ValueString}\n");
            //Console.WriteLine($"{mt1.ValueString}\n");
            //Console.WriteLine($"{(6.2-mt1).ValueString}\n");

            Console.WriteLine($"{3 == new Inf()}");
            Console.WriteLine();
        }
    }
}
