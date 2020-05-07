using System;
using System.IO; //работа с файлами
using System.Collections;
using System.Collections.Generic;
using System.Linq; //методы Take, Select
//using lab1_mpi_zdl;

namespace lab3_least_squares_method
{
    class Program
    {
        public static double[] polynom(double[] X_i,double[] Y_i, int d, int n) // d - степень многочлена
        {
            int size = d + 1;
            double[] a = new double[size]; //резы, коэффициенты а
            double[,] m = new double[size, size];
            double[] b = new double[size];
            //заполнение коэффициентов в системе уравнений
            for(int i = 0; i < size; i++)
            {
                for(int j = 0; j < size; j++)
                {
                    if (i == 0 && j == 0)
                        m[i, j] = n + 1;
                    if(i == 0 && j == 1)
                    {
                        for (int it = 0; it < n + 1; it++)
                            m[i, j] += X_i[it];
                        m[j, i] = m[i, j];
                    }
                    if (i == 0 && j == 2)
                    {
                        for (int it = 0; it < n + 1; it++)
                            m[i, j] += Math.Pow(X_i[it], 2);
                        m[j, i] = m[i, j];
                        m[i + 1, j - 1] = m[i, j]; 
                    }
                    if (i == 1 && j == 1 && d == 1)
                    {
                        for (int it = 0; it < n + 1; it++)
                            m[i, j] += Math.Pow(X_i[it], 2);
                    }
                    if (i == 1 && j == 2)
                    {
                        for (int it = 0; it < n + 1; it++)
                            m[i, j] += Math.Pow(X_i[it], 3);
                        m[j, i] = m[i, j];
                    }
                    if (i == 2 && j == 2)
                    {
                        for (int it = 0; it < n + 1; it++)
                            m[i, j] += Math.Pow(X_i[it], 4);
                    }
                }
                if(i == 0)
                {
                    for (int it = 0; it < n + 1; it++)
                        b[i] += Y_i[it];
                }
                if(i == 1)
                {
                    for (int it = 0; it < n + 1; it++)
                        b[i] += (Y_i[it] * X_i[it]);
                }
                if (i == 2)
                {
                    for (int it = 0; it < n + 1; it++)
                        b[i] += (Y_i[it] * Math.Pow(X_i[it], 2));
                }
            }
            a = new lab1_LU.Program().LUP(m, b);

            return a;
        }
        
        public static double F_n(double X_i, double[] a, int d) //в подсчете суммы квадратов ошибок
        {
            if (d == 1)
                return a[0] + a[1] * X_i;
            else return a[0] + a[1] * X_i + a[2] * Math.Pow(X_i, 2);
        }

        public static double F(double[] X_i, double[] Y_i, double[] a, int n, int d)
        {
            double F = 0;
            for(int i = 0; i <= n; i++)
            {
                F += Math.Pow((F_n(X_i[i], a, d) - Y_i[i]), 2);
            }
            return F;
        }

        public static void outp(double[] X_i, double[] Y_i, double[] a, int n ,int d)
        {
            using (StreamWriter wr = File.AppendText("LS_res.txt"))
            {
                wr.WriteLine();
                wr.Write("Приближающий многочлен " + d + " степени: F"+ d + "(X) = " + a[0]);
                if (Math.Sign(a[1]) == 1 || Math.Sign(a[1]) == 0)
                    wr.Write("+");
                wr.Write(a[1] + "x");
                
                if ( d == 2 )
                {
                    if (Math.Sign(a[2]) == 1 || Math.Sign(a[2]) == 0)
                        wr.Write("+");
                    wr.Write(a[2] + "x^2");
                }
                wr.Write("\n\nСумма квадратов ошибок Ф = " + F(X_i, Y_i, a, n, d) + "\n");
            }
        }
        static void Main(string[] args)
        {
            int size = 0;
            char c_ = ' ';
            using (StreamReader sr = File.OpenText("LS_.txt"))
            {
                while (sr.Peek() != ' ')
                {
                    c_ = (char)sr.Read();
                    size = c_ - '0';

                }
            }

            
            string[] lines = File.ReadAllLines("LS.txt").Take(size).ToArray();
            double[] X_i = new double[size];
            double[] Y_i = new double[size];
            // разобрать в массивы
            for (int i = 0; i < size; i++)
            {

                double[] row = lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(Double.Parse).ToArray();
                for (int j = 0; j < 2; j++)
                {
                    if (j == 0)
                        X_i[i] = row[j];
                    if (j == 1)
                        Y_i[i] = row[j];
                }
            }

            double[] a;
            //многочлен 1-ой степени
            a = polynom(X_i, Y_i, 1, X_i.Length - 1);
            outp(X_i, Y_i, a, X_i.Length - 1, 1);
            //многочлен 2-ой степени
            a = polynom(X_i, Y_i, 2, X_i.Length - 1);
            outp(X_i, Y_i, a, X_i.Length - 1, 2);

        }
    }
}
