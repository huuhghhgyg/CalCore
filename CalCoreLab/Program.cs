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
            double inf = double.PositiveInfinity;
            //Matrix pathMatrix = new Matrix(new double[,]
            //{
            //    {0,3,2,inf,inf,4},
            //    {inf,0,4,inf,4,1},
            //    {inf,inf,0,-1,6,inf},
            //    {3,-2,inf,0,1,inf},
            //    {5,inf,inf,inf,0,3},
            //    {inf,inf,3,3,inf,0}
            //});
            //Console.WriteLine(Network.ShortestPath2AllPoints(pathMatrix).ToString() + "\n");
            //Console.WriteLine(Network.ShortestPath2LastPoint(pathMatrix).ToString() + "\n");
            //Console.WriteLine(pathMatrix.Min + "\n");
            //Console.WriteLine(pathMatrix.GetList(-1).ToString() + "\n");
            //Console.WriteLine("新数组：\n" + new Matrix(pathMatrix).ToString() + "\n");

            //Matrix pathMatrix = new Matrix(new double[,]
            //{
            //    {inf,3,4,7,inf,inf,inf},
            //    {inf,inf,3,2,4,inf,inf},
            //    {inf,inf,inf,inf,5,7,inf},
            //    {inf,inf,inf,inf,2,inf,6},
            //    {inf,inf,inf,inf,inf,1,4},
            //    {inf,inf,inf,inf,inf,inf,2},
            //    {inf,inf,inf,inf,inf,inf,inf}
            //});
            Matrix pathMatrix = new Matrix(new double[,]
            {
                {inf,2,3,inf,inf,inf,inf,inf,inf,},
                {inf,inf,3,2,inf,inf,inf,inf,5},
                {inf,inf,inf,inf,2,inf,inf,6,inf},
                {inf,inf,inf,inf,1,3,inf,inf,inf},
                {inf,inf,inf,inf,inf,inf,3,inf,inf},
                {inf,inf,inf,inf,inf,inf,3,inf,2},
                {inf,inf,inf,inf,inf,inf,inf,2,inf},
                {inf,inf,inf,inf,inf,inf,inf,inf,4},
                {inf,inf,inf,inf,inf,inf,inf,inf,inf}
            });

            Matrix result = Network.GetSpanningTree(pathMatrix);
            Console.WriteLine(result.ToString());

            Console.ReadLine();
        }
    }
}
