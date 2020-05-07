using System;
using System.IO; //работа с файлами
using System.Collections;
using System.Collections.Generic;
using System.Linq; //методы Take, Select

namespace Newton_SIM
{
    class Program
    {
        public static double fi(double x)
        {
            return Math.Sqrt(Math.Pow(2, x) - 0.5);//Math.Log(x * x + 0.5) / Math.Log(2); 
        }
        public static double dfi(double x)
        {
            return Math.Pow(2, x) * Math.Log(2) / (2 * Math.Sqrt(Math.Pow(2, x) - 0.5));
        }
        public static double fi2(double x)
        {
            return x - 0.1 * (Math.Pow(2, x) - x * x - 0.5);
        }
        public static double dfi2(double x)
        {
            return -0.1*Math.Pow(2, x) * Math.Log(2) + 0.2*x + 1;
        }
        public static void SIM(double x0, double e, double q, bool fl)
        {
            List<double> x_k = new List<double>();
            double x_prev;
            double x = x0;
            int it = 0;
            do
            {
                x_k.Add(x);
                x_prev = x;
                if (fl)
                    x = fi(x);
                else
                    x = fi2(x);
                it++;
            }
            while ((q / (1 - q)) * Math.Abs(x - x_prev) > e);

            using (StreamWriter outputFile = File.AppendText("SIM_res.txt"))
            {
                outputFile.WriteLine("SIM:");
                for(int i = 0; i < it; i++)
                {
                    outputFile.Write("x_" + i + " :");
                    outputFile.WriteLine(x_k[i]);
                }
                outputFile.WriteLine(it.ToString() + " iterations");
            }
        }
        public static void Newton(double x0, double e)
        {
            List<double> x_k = new List<double>();
            double x = x0;
            double x_prev;
            int it = 0;
            do
            {
                x_k.Add(x);
                x_prev = x;
                x -= f(x) / df(x);
                it++;
            }
            while (Math.Abs(x - x_prev) >= e);

            using (StreamWriter outputFile = File.AppendText("Newton_res.txt"))
            {
                outputFile.WriteLine("Newton:");
                for (int i = 0; i < it; i++)
                {
                    outputFile.Write("x_" + i + " :");
                    outputFile.WriteLine(x_k[i]);
                }
                outputFile.WriteLine(it.ToString() + " iterations");
            }
        }
        public static double f(double x)
        {
            return Math.Pow(2, x) - Math.Pow(x, 2) - 0.5;
        }
        private static double df(double x)
        {
            return Math.Pow(2, x) * Math.Log(2) - 2*x;
        }
        private static double ddf(double x)
        {
            return Math.Pow(2, x) * Math.Log(2) * Math.Log(2) - 2;
        }
        static void Main(string[] args)
        {
            double e;
            string[] lines = File.ReadAllLines("Newton_SIM.txt").Take(1).ToArray();
            e = Double.Parse(lines[0]);

            //интервалы иксов
            double x0;
            double[] x1 = { 1.45, 1.65, 4, 4.2}; //x
            //SIM
            double q;
            bool fl = true; //если вдруг не считается нормально q
            for (int i = 0, j = 0; j < 2; i += 2, j++)
            {
                if ((dfi(x1[i]) < 1) && (dfi(x1[i+1]) < 1))
                {
                    q = Math.Max(dfi(x1[i]), dfi(x1[i+1]));
                    x0 = (x1[i] + x1[i+1]) / 2;
                    SIM(x0, e, q, fl);
                }
                else //если условие сходимости не выполняется
                {
                    q = Math.Max(dfi2(x1[i]), dfi2(x1[i + 1]));
                    x0 = (x1[i] + x1[i + 1]) / 2;
                    fl = false;
                    SIM(x0, e, q, fl);
                }

                //Newton
                if (f(x1[i]) * f(x1[i+1]) < 0)
                {
                    if (f(x1[i]) * ddf(x1[i]) > 0)
                    {
                        x0 = x1[i];
                        Newton(x0, e);
                    }
                    else if (f(x1[i+1]) * ddf(x1[i+1]) > 0)
                    {
                        x0 = x1[i+1];
                        Newton(x0, e);
                    }
                }
                
            }
        }
    }
}
