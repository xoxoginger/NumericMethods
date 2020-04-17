using System;
using System.IO; //работа с файлами
using System.Collections;
using System.Collections.Generic;
using System.Linq; //методы Take, Select

namespace lab1_progonka
{
    public class Program
    {
        public double[] pr(double[] a, double[] b, double[] c, double[] d, int size)
        {
            double[] P = new double[size];
            double[] Q = new double[size];

            double[] X = new double[size];

            for (int i = 0; i < size; i++)
            {
                //проверка достаточного условия
                //if ((a[i] == 0 && i != 0) || (c[i] == 0 && i != size - 1) || (Math.Abs(b[i]) < Math.Abs(a[i]) + Math.Abs(c[i])))
                //{
                  //  Console.WriteLine("Warning: exception - sufficient convergence condition is not satisfied.");
                    //Environment.Exit(-1);
                //}
                if (i == 0)
                {
                    P[i] = -c[i] / b[i];
                    Q[i] = d[i] / b[i];
                }
                else
                {
                    P[i] = -c[i] / (b[i] + a[i] * P[i - 1]);
                    Q[i] = (d[i] - a[i] * Q[i - 1]) / (b[i] + a[i] * P[i - 1]);
                }
            }

            for (int i = size - 1; i >= 0; i--)
            {
                if (i == size - 1)
                {
                    X[i] = Q[i];
                }
                else
                {
                    X[i] = P[i] * X[i + 1] + Q[i];
                }

            }
            return X;
        }
        static void Main()
        {
            int size = 0;
            char c_;
            using (StreamReader sr = File.OpenText("pr_.txt"))
            {
                while (sr.Peek() != ' ')
                {
                    c_ = (char)sr.Read();
                    size = c_ - '0';
                }
            }
            // прочитать первые 5 строк файла
            string[] lines = File.ReadAllLines("pr.txt").Take(size).ToArray();

            double[] a = new double[size];
            double[] b = new double[size];
            double[] c = new double[size];
            double[] d = new double[size];

           
            // разобрать в массивы
            for (int i = 0; i < size; i++)
            {

                double[] row = lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(Double.Parse).ToArray();

                for (int j = 0; j < size - 1; j++)
                {
                    switch (j)
                    {
                        case 0:
                            a[i] = row[j];
                            break;
                        case 1:
                            b[i] = row[j];
                            break;
                        case 2:
                            c[i] = row[j];
                            break;
                        case 3:
                            d[i] = row[j];
                            break;
                    }
                }
            }
            double[] X = { 0 };
            //вывод
            using (StreamWriter outputFile = new StreamWriter("pr__.txt"))
            {
                foreach (double x in X)
                    outputFile.WriteLine(x);
            }

        }
    }
}