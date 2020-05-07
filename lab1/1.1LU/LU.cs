using System;
using System.IO; //работа с файлами
using System.Collections;
using System.Collections.Generic;
using System.Linq; //методы Take, Select
using System.Numerics;

namespace lab1_LU
{
   public class Program
    {
        public static double[,] mult(double[,] a, double[,] b)
        {
            if (a.GetLength(1) != b.GetLength(0)) throw new Exception("Матрицы нельзя перемножить");
            double[,] r = new double[a.GetLength(0), b.GetLength(1)];
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    for (int k = 0; k < b.GetLength(0); k++)
                    {
                        r[i, j] += a[i, k] * b[k, j];
                    }
                }
            }
            return r;
        }
        public static double[] mult(double[,] a, double[] b)
        {
            if (a.GetLength(1) != b.GetLength(0)) throw new Exception("Матрицы нельзя перемножить");
            double[] r = new double[b.GetLength(0)];
            for (int i = 0; i < a.GetLength(0); i++)
            {

                for (int j = 0; j < b.GetLength(0); j++)
                {
                    r[i] += a[i, j] * b[j];
                }
            }
            return r;
        }

        public double[] LUP(double[,] A, double[] b)
        {
            int n = A.GetLength(0);
            double[,] C = new double[n, n];
            double[,] P = new double[n, n];
            C = A;
            double[,] P_ = { { 0, 0, 0, 1 }, { 1, 0, 0, 0 }, { 0, 0, 1, 0 }, { 0, 1, 0, 0 } };
            //загружаем в матрицу P единичную матрицу

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                        P[i, j] = 1;
                    else
                        P[i, j] = 0;
                }
            }
            for (int i = 0; i < n; i++)
            {
                //поиск опорного элемента
                double pivotValue = 0;
                int pivot = -1;
                for (int row = i; row < n; row++)
                {
                    if (Math.Abs(C[row, i]) > pivotValue)
                    {
                        pivotValue = Math.Abs(C[row, i]);
                        pivot = row;
                    }
                }
                if (pivotValue != 0)
                {
                    //меняем местами i-ю строку и строку с опорным элементом
                    //P.SwapRows(pivot, i);
                    for (int j = 0; j < n; j++)
                    {
                        double temp;
                        temp = P[pivot, j];
                        P[pivot, j] = P[i, j];
                        P[i, j] = temp;
                    }
                    //C.SwapRows(pivot, i);
                    for (int j = 0; j < n; j++)
                    {
                        double temp;
                        temp = C[pivot, j];
                        C[pivot, j] = C[i, j];
                        C[i, j] = temp;
                    }
                    for (int j = i + 1; j < n; j++)
                    {
                        C[j, i] /= C[i, i];
                        for (int k = i + 1; k < n; k++)
                            C[j, k] -= C[j, i] * C[i, k];
                    }
                }
            }
            //теперь матрица C = L + U - E
            double[,] L = new double[n, n];
            double[,] U = new double[n, n];
            //раскладываем в матрицы L, U
            for(int i = 0; i < n; i++)
            {
                for(int j = 0; j < n; j++)
                {
                    if(i == j)
                    {
                        L[i, j] = 1;
                        U[i, j] = C[i, j];
                    }
                    if(i < j)
                    {
                        L[i, j] = 0;
                        U[i, j] = C[i, j];
                    }
                    if(i > j)
                    {
                        U[i, j] = 0;
                        L[i, j] = C[i, j];
                    }
                }
            }
            
            double[] x = new double[n];
            double[] z = new double[n];
            double[] Pb = mult(P, b);
            for (int i = 0; i < n; i++)
            {
                if (i == 0)
                    z[0] = Pb[0];
                else
                {
                    double sum = 0;
                    for (int j = 0; j <= i - 1; j++)
                    {
                        sum += L[i, j] * z[j];
                    }
                    z[i] = Pb[i] - sum;
                }
            }

            for (int i = n - 1; i >= 0; i--)
            {
                if (i == n - 1)
                    x[n-1] = z[n-1] / U[n-1, n-1];
                else
                {
                    double sum = 0;
                    for (int j = i + 1; j < n; j++)
                    {
                        sum += U[i, j] * x[j];
                    }
                    x[i] = 1 / U[i,i] * (z[i] - sum);
                }
            }
            
            double[,] lu = new double[n, n];
            lu = mult(transp(P), mult(L, U));
            //вывод
            using (StreamWriter outputFile = new StreamWriter("LU_res.txt"))
            {
                outputFile.WriteLine("L=");
                for(int i = 0; i < n; i++)
                {
                    for(int j = 0; j < n; j++)
                    {
                        outputFile.Write(L[i,j] + " ");
                    }
                    outputFile.WriteLine();
                }
                outputFile.WriteLine();
                outputFile.WriteLine("U=");
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        outputFile.Write(U[i, j] + " ");
                    }
                    outputFile.WriteLine();
                }
                outputFile.WriteLine();
                outputFile.WriteLine("X=");
                foreach (double i in x)
                    outputFile.WriteLine(i);

                outputFile.WriteLine("Check:");
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        outputFile.Write(lu[i, j] + " ");
                    }
                    outputFile.WriteLine();
                }
            }
            return x;
        }

        public static double[,] transp(double[,] a)
        {
            double[,] r = new double[a.GetLength(0), a.GetLength(1)];
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    r[i, j] = a[j, i];
                }
            }
            return r;
        }
        static void Main()
        {
            int size = 0;
            char c;
            using (StreamReader sr = File.OpenText("LU_.txt"))
            {
                while (sr.Peek() != ' ')
                {
                    c = (char)sr.Read();
                    size = c - '0';
                }
            }
            // прочитать первые 5 строк файла
            string[] lines = File.ReadAllLines("LU.txt").Take(size).ToArray();

            double[,] matrix = new double[size, size];
            double[] right = new double[size];
            // разобрать в массивы
            for (int i = 0; i < size; i++)
            {
                double[] row = lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(Double.Parse).ToArray();
                for (int j = 0, n = 0; n < size+1; n++, j++)
                {
                    
                    if (j == size)
                    {
                        right[i] = row[j];
                    }
                    else
                    {
                        matrix[i, j] = row[j];
                    }
                }
            }
            //LUP(matrix, right);
        }
    }
}
