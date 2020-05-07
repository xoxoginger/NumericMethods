using System;
using System.IO; //работа с файлами
using System.Collections;
using System.Collections.Generic;
using System.Linq; //методы Take, Select

namespace Yakobi
{
    class Program
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
        static void Main(string[] args)
        {
            double e = 0.01;
            int size = 0;
            char c_ = ' ';
            using (StreamReader sr = File.OpenText("jacobi_.txt"))
            {
                while (sr.Peek() != ' ')
                {
                    c_ = (char)sr.Read();
                    size = c_ - '0';

                }
            }
            size = 10;
            string[] lines = File.ReadAllLines("jacobi.txt").Take(size).ToArray();
            double[,] a = new double[size, size];
            double max = 0;
            int n = 0, m = 0; //временные индексы

            // разобрать в массив
            for (int i = 0; i < size; i++)
            {
                double[] row = lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(Double.Parse).ToArray();
                for (int j = 0; j < size; j++)
                {
                    a[i, j] = row[j];
                }
            }

            double t_k = 1;
            int it = 0;
            List<double[,]> U = new List<double[,]>();
            while (t_k > e)
            {
                it++;
                t_k = 0;
                //max
                max = 0;
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (i == j)
                            continue;
                        else
                        {
                            if ((i < j) && (Math.Abs(a[i, j]) >= max))
                            {
                                max = Math.Abs(a[i, j]);
                                n = i;
                                m = j;
                            }
                        }
                    }
                }

                //fi
                double fi = 0;
                if (a[n, n] == a[m, m])
                    fi = Math.PI / 4;
                else
                    fi = 0.5 * Math.Atan(2 * a[n, m] / (a[n, n] - a[m, m]));

                //U
                double[,] U_k = new double[size, size];
                
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        U_k[i, j] = 0;
                        if (i == j)
                            U_k[i, j] = 1;
                        if ((i == n && j == n) || (i == m && j == m))
                            U_k[i, j] = Math.Cos(fi);
                        if (i == n && j == m)
                            U_k[i, j] = -Math.Sin(fi);
                        if (i == m && j == n)
                            U_k[i, j] = Math.Sin(fi);
                    }
                }
                U.Add(U_k);
                //A(k+1)
                double[,] a_k = new double[size, size];
                a_k = mult(mult(transp(U_k), a), U_k);

                //критерий окончания
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (i < j)
                        {
                            t_k += a_k[i, j] * a_k[i, j];
                        }
                    }
                }
                t_k = Math.Sqrt(t_k);

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        a[i, j] = a_k[i, j];
                        a_k[i, j] = 0;
                    }
                }

            }

            //nums
            using (StreamWriter outputFile = new StreamWriter("jacobi_ownnums.txt"))
            {
                outputFile.WriteLine("Own numbers:");
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if(i==j)
                            outputFile.WriteLine(a[i,j]);
                    }
                } 
                outputFile.WriteLine(it.ToString() + " iterations");
            }

            //vecs
            double[,] res = new double[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i == j)
                        res[i, j] = 1;
                    else
                        res[i, j] = 0;
                }
            }
            foreach (double[,] x in U)
            {
                res = mult(res, x);
            }

            using (StreamWriter outputFile = new StreamWriter("jacobi_ownvecs.txt"))
            {
                outputFile.WriteLine("Own vectors:");
                for (int i = 0; i < size; i++)
                {
                    outputFile.WriteLine("x" + (i+1));
                    for (int j = 0; j < size; j++)
                    {
                        outputFile.WriteLine(res[j, i]);
                    }
                }
                outputFile.WriteLine(it.ToString() + " iterations");
            }
            

        }
    }

}
