using System;
using System.IO; //работа с файлами
using System.Collections;
using System.Collections.Generic;
using System.Linq; //методы Take, Select
using lab1_mpi_zdl;
using System.Drawing;

namespace lab3_Spline
{
    class Program
    {
        public static void Spline()
        {
            int size = 0;
            char c_ = ' ';
            using (StreamReader sr = File.OpenText("spline_.txt"))
            {
                while (sr.Peek() != ' ')
                {
                    c_ = (char)sr.Read();
                    size = c_ - '0';

                }
            }

            // прочитать первые 5 строк файла
            string[] lines = File.ReadAllLines("spline.txt").Take(size).ToArray();
            // double X;
            double[] X_i = new double[size];
            double[] Y_i = new double[size];
            double X_n = 1.5;
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

            double[] a = new double[3]; //c2
            double[] b = new double[3]; //c3
            double[] c = new double[3]; //c4
            double[] d = new double[3]; //=

            double[] h = new double[size];

            for (int i = 1; i < size; i++)
            {
                h[i] = X_i[i] - X_i[i - 1];
            }

            double[,] matrix = new double[a.Length, a.Length];
            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        {
                            for (int j = 0; j < size - 1; j++)
                            {
                                switch (j)
                                {
                                    case 0:
                                        {
                                            a[i] = 2 * (h[1] + h[2]);
                                            break;
                                        }
                                    case 1:
                                        {
                                            b[i] = h[2];
                                            break;
                                        }

                                    case 2:
                                        {
                                            c[i] = 0;
                                            break;
                                        }
                                    case 3:
                                        {
                                            d[i] = 3 * ((Y_i[2] - Y_i[1]) / h[2] - (Y_i[1] - Y_i[0]) / h[1]);
                                            break;
                                        }
                                }
                            }
                            break;
                        }
                    case 1:
                        {
                            for (int j = 0; j < size - 1; j++)
                            {
                                switch (j)
                                {
                                    case 0:
                                        {
                                            a[i] = h[2];
                                            break;
                                        }
                                    case 1:
                                        {
                                            b[i] = 2 * (h[2] + h[3]);
                                            break;
                                        }

                                    case 2:
                                        {
                                            c[i] = h[3];
                                            break;
                                        }
                                    case 3:
                                        {
                                            d[i] = 3 * ((Y_i[3] - Y_i[2]) / h[3] - (Y_i[2] - Y_i[1]) / h[2]);
                                            break;
                                        }
                                }
                            }
                            break;
                        }
                    case 2:
                        {
                            for (int j = 0; j < size - 1; j++)
                            {
                                switch (j)
                                {
                                    case 0:
                                        {
                                            a[i] = 0;
                                            break;
                                        }
                                    case 1:
                                        {
                                            b[i] = h[3];
                                            break;
                                        }

                                    case 2:
                                        {
                                            c[i] = 2 * (h[3] + h[4]);
                                            break;
                                        }
                                    case 3:
                                        {
                                            d[i] = 3 * ((Y_i[4] - Y_i[3]) / h[4] - (Y_i[3] - Y_i[2]) / h[3]);
                                            break;
                                        }
                                }
                            }
                            break;
                        }


                }
            }

            for (int i = 0; i < a.Length; i++)
            {
                for (int j = 0; j < a.Length; j++)
                {
                    if (i == 0)
                        matrix[i, j] = a[j];
                    if (i == 1)
                        matrix[i, j] = b[j];
                    if (i == 2)
                        matrix[i, j] = c[j];
                }
            }

            double[] X = new lab1_progonka.Program().pr(a,b,c,d,3);


            double[] a_i = new double[5];
            double[] b_i = new double[5];
            double[] c_i = new double[5];
            double[] d_i = new double[5];
            X = new lab1_LU.Program().LUP(matrix, d);


            for (int i = 1; i < a_i.Length; i++)
            {
                if (i == 1)
                    c_i[i] = 0;
                if (i > 1)
                {
                    c_i[i] = X[i - 2];
                }
            }
            for (int i = 1; i < a_i.Length; i++)
            {
                a_i[i] = Y_i[i - 1];
                if (i == a_i.Length - 1)
                {
                    b_i[i] = (Y_i[i] - Y_i[i - 1]) / h[i] - 2 / 3.0 * h[i] * c_i[i];
                    d_i[i] = -c_i[i] / (3 * h[i]);
                }
                else
                {
                    b_i[i] = (Y_i[i] - Y_i[i - 1]) / h[i] - 1 / 3.0 * h[i] * (c_i[i + 1] + 2 * c_i[i]);
                    d_i[i] = (c_i[i + 1] - c_i[i]) / (3 * h[i]);
                }

            }
            double f = 0;
            for (int i = 1; i < X_i.Length; i++)
            {
                if (X_n >= X_i[i - 1] && X_n <= X_i[i])
                {
                    f = a_i[i] + b_i[i] * (X_n - X_i[i - 1]) + c_i[i] * Math.Pow((X_n - X_i[i - 1]), 2) + d_i[i] * Math.Pow((X_n - X_i[i - 1]), 3);
                }
            }

            using (StreamWriter wr = File.AppendText("Spline_res.txt"))
            {
                wr.Write("Cubic spline:");
                for (int i = 1; i < X_i.Length; i++)
                {
                    wr.Write("\n\nS(" + X_i[i] + ") = " + a_i[i]);
                    if (Math.Sign(b_i[i]) == 1 || Math.Sign(b_i[i]) == 0)
                        wr.Write("+");
                    wr.Write(b_i[i] + "(x - " + X_i[i - 1] + ")");
                    if (Math.Sign(c_i[i]) == 1 || Math.Sign(c_i[i]) == 0)
                        wr.Write("+");
                    wr.Write(c_i[i] + "(x - " + X_i[i - 1] + ")^2");
                    if (Math.Sign(d_i[i]) == 1 || Math.Sign(d_i[i]) == 0)
                        wr.Write("+");
                    wr.Write(d_i[i] + "(x - " + X_i[i - 1] + ")^3");
                    wr.WriteLine();
                }
                wr.WriteLine();
                wr.WriteLine("S(X*) = " + f);

            }
        }
        static void Main(string[] args)
        {
            Spline();
        }
    }
}
