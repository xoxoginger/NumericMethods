using System;
using System.IO; //работа с файлами
using System.Collections;
using System.Collections.Generic;
using System.Linq; //методы Take, Select

namespace lab3_Lagrange_Newtone
{
    class Program
    {
        public static double[] parsing_pi(int size)
        {
            double[] X = new double[size * 2+1];
            // прочитать первые  строк файла
            string[] lines = File.ReadAllLines("LN.txt").Take(size * 2 + 1).ToArray();
            string[] row = lines;

            for (int i = 0; i < size * 2 + 1; i++) //кол-во строк
            {

                double d = 0, d1, d2;
                string digit = "", digit1 = "", digit2 = "";
                for (int j = 0; j < row[i].Length; j++) //проход по строке
                {
                    //если в формате 0.1pi , 2pi
                    if (Char.IsDigit(row[i], j) || row[i].ElementAt(j) == '\n')
                    {
                        continue;
                    }
                    else if (row[i].ElementAt(j) == ',' || row[i].ElementAt(j) == 'p' || row[i].ElementAt(j) == '/')
                    {
                        if (row[i].ElementAt(j) == ',') //десятичные числа
                        {
                            int it = 0;
                            while (row[i].ElementAt(it) != 'p')
                            {
                                digit += Char.ToString(row[i].ElementAt(it));
                                it++;
                            }
                            d = Convert.ToDouble(digit); //коэффициент
                            break;
                        }
                        else if (row[i].ElementAt(j) == '/') //если в формате 3pi/6
                        {
                            for (int it = 0; it < j - 2; it++) //-2, потому что 2 символа - это pi
                                digit1 += Char.ToString(row[i].ElementAt(it));
                            for (int it = j + 1; it < row[i].Length; it++) //оставшаяся часть числа
                                digit2 += Char.ToString(row[i].ElementAt(it));
                            d1 = Convert.ToDouble(digit1);
                            d2 = Convert.ToDouble(digit2);
                            d = d1 / d2;
                            break;
                        }
                        else //целые числа
                        {
                            int end = 0;
                            for (int it = 0; it < row[i].Length; it++) //проверка, в каком формате ввод
                            {
                                if (!Char.IsDigit(row[i], it))
                                    end++;
                            }
                            if (end == 2)
                            {
                                for (int it = 0; it < j; it++)
                                    digit += Char.ToString(row[i].ElementAt(it));
                                d = Convert.ToDouble(digit); //коэффициент
                                break;
                            }
                        }
                    }
                }
                X[i] = d * Math.PI;
            }
            return X;
        }

        public static double[] Y(double[] X)
        {
            double[] r = new double[X.Length];
            for(int i = 0; i< X.Length; i++)
            {
                r[i] = Math.Sin(X[i]);
            }
            return r;
        }

        public static double F(double X)
        {
            return Math.Sin(X);
        }

        public static double[] f_div_dw(double[] X, double[] Y, int size)
        {
            double[] div = new double[size];
            double[] dw = new double[size];
            for(int i = 0; i < size; i++)
            {
                dw[i] = 1;
                for(int j = 0; j< size; j++)
                {
                    if(i != j)
                        dw[i] *= (X[i] - X[j]);
                }
                div[i] = Y[i] / dw[i];
            }
            return div;
        }

        public static void L(double[] X, double[] Y, double X_n, int size)
        {
            double res = 0;
            double[] div = f_div_dw(X,Y,size);
            double[] temp = new double[size];
            for (int i = 0; i < size; i++)
            {
                temp[i] = 1;
                for (int j = 0; j < size; j++)
                {
                    if (i != j)
                        temp[i] *= (X_n - X[j]);
                }
                res += temp[i] * div[i];
            }
            
            using(StreamWriter wr = File.AppendText("LN_res.txt"))
            {
                wr.Write("Lagrange Interpolation Polynomial, " + (size - 1) + " degree:" + "\n\nL(X) = ");
                for (int i = 0; i < size; i++)
                {
                    if(Math.Sign(div[i]) == 1)
                    {
                        if (i != 0)
                            wr.Write("+");
                    }
                    
                    wr.Write(div[i]);
                    for (int j = 0; j < size; j++)
                    {
                        if (i != j)
                            wr.Write("(x - " + X[j] + ")");
                    }
                    wr.WriteLine();
                    
                }
                wr.Write("\n");
                wr.WriteLine("L(X*) = " + res + "\n" + "f(X*) = " + F(X_n) + "\n" + "ΔL(X*) = " + Math.Abs(F(X_n) - res));

            }
        }

        public static double dif1(double[] X, double[] Y, int size, int n)
        {
            double d = (Y[n] - Y[n+1]) / (X[n] - X[n+1]);
            return d;
        }

        public static double dif2(double[] X, double[] Y, int size, int n) //n - начиная с какого элемента
        {
            double d;            
            d = (dif1(X, Y, size, n) - dif1(X, Y, size, n+1)) / (X[n] - X[n+2]);
            return d;
        }

        public static double difn(double[] X, double[] Y, int size, int ord, int n)
        {
            double d;
            if (ord == 3)
            {
                d = (dif2(X, Y, size, 0) - dif2(X, Y, size, 1)) / (X[n] - X[size - 1]);
            }
            else
            {
                    d = (difn(X, Y, size, ord - 1, 0) - difn(X, Y, size, ord - 1, 0)) / (X[0] - X[size - 1]);
            }
            return d;
        }

        public static void N(double[] X, double[] Y, double X_n, int size)
        {
            double[] k = new double[size - 1]; //массив коэф в полиноме
            for (int i = 0; i < size - 1; i++)
            {
                if (i == 0)
                {
                    k[i] = dif1(X, Y, size, 0);
                }
                else if (i == 1)
                {
                    k[i] = dif2(X, Y, size, 0);
                }
                else
                {
                    k[i] = difn(X, Y, size, i + 1, 0);
                }
            }
            double res = 0;
            double[] temp = new double[size];
            for (int i = 0; i < size-1; i++)
            {
                temp[i] = 1;
                for (int j = 0; j <= i; j++)
                {
                    temp[i] *= (X_n - X[j]);
                }
                res += temp[i] * k[i];
            }

            using (StreamWriter wr = File.AppendText("LN_res.txt"))
            {
                wr.WriteLine();
                wr.Write("Newtone Interpolation Polynomial, " + (size - 1) + " degree:" + "\n\nP(X) = " + F(X[0]));
                for (int i = 0; i < size-1; i++)
                {
                    if (Math.Sign(k[i]) == 1)
                    {
                        wr.Write("+");
                    }

                    wr.Write(k[i]);
                    for (int j = 0; j <= i; j++)
                    {
                        wr.Write("(x - " + X[j] + ")");
                    }
                    wr.WriteLine();

                }
                wr.Write("\n");
                wr.WriteLine("P(X*) = " + res + "\n" + "f(X*) = " + F(X_n) + "\n" + "ΔP(X*) = " + Math.Abs(F(X_n) - res));

            }
        }
        static void Main(string[] args)
        {

            int size = 0;
            char c_ = ' ';
            using (StreamReader sr = File.OpenText("LN_.txt"))
            {
                while (sr.Peek() != ' ')
                {
                    c_ = (char)sr.Read();
                    size = c_ - '0';

                }
            }

            double[] X_ia = new double[size];//= { 0.1*Math.PI, 0.2 * Math.PI, 0.3 * Math.PI, 0.4 * Math.PI };
            double[] X_ib = new double[size];//= { 0.1 * Math.PI, Math.PI/6, 0.3 * Math.PI, 0.4 * Math.PI };
           
            double X = Math.PI / 4;

            double[] Big_X = parsing_pi(size);
            //раскладываем в массивы иксов
            for (int i = 0; i < size * 2+1; i++)
            {
                if (i < size)
                    X_ia[i] = Big_X[i];
                if (i > size)
                    X_ib[i - 5] = Big_X[i];
            }

            double[] Y_ia = Y(X_ia);
            double[] Y_ib = Y(X_ib);


            L(X_ia, Y_ia, X, size);
            //L(X_ib, Y_ib, X, size);

            N(X_ia, Y_ia, X, size);
            //N(X_ib, Y_ib, X, size);

            Console.WriteLine();



        }
    }
}
