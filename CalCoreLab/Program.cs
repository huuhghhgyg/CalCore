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

            //Matrix mt1 = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } });
            //Matrix mt2 = new Matrix(new double[,] { { 2, 1 }, { 3, 2 }, { 2, 4 } });

            //Console.WriteLine($"{mt1.ValueString}\n");
            //Console.WriteLine($"{mt2.ValueString}\n");

            //Console.WriteLine($"{(mt1 * mt2).ValueString}\n");
            ////mt1.T();
            //Console.WriteLine($"{mt1.T().ValueString}\n");

            //Console.WriteLine($"{mt1.T().ValueString}\n");
            //Console.WriteLine($"{mt1.GetSubMatrix(1, 1, 2, 2).ValueString}\n");
            //Console.WriteLine($"{mt1.GetCols(2, 2).ValueString}\n"); //截取列
            //Console.WriteLine($"{mt1.GetCol(2).ValueString}\n"); //截取列
            //Console.WriteLine($"{mt1.GetValue(1, 2)}\n");

            ////Console.WriteLine($"{(mt1-mt2).ValueString}\n");
            ////Console.WriteLine($"{mt1.ValueString}\n");
            ////Console.WriteLine($"{(6.2-mt1).ValueString}\n");

            //Console.WriteLine($"{3 == Double.PositiveInfinity}\n");

            //Console.WriteLine($"{mt2.ValueString}\n");
            //Console.WriteLine($"{mt2.GetRow(1).ValueString}\n");
            //Console.WriteLine();
            double inf = double.PositiveInfinity;
            Matrix pathMatrix = new Matrix(new double[,]
            {
                {0,3,2,inf,inf,4},
                {inf,0,4,inf,4,1},
                {inf,inf,0,-1,6,inf},
                {3,-2,inf,0,1,inf},
                {5,inf,inf,inf,0,3},
                {inf,inf,3,3,inf,0}
            });
            Console.WriteLine(Network.ShortestPath2AllPoints(pathMatrix).ToString()+"\n");
            Console.WriteLine(Network.ShortestPath2LastPoint(pathMatrix).ToString()+"\n");
        }
    }
}
