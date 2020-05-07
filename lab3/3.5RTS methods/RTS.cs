using System;
using System.IO; //работа с файлами
using System.Collections;
using System.Collections.Generic;
using System.Linq; //методы Take, Select

namespace lab3_rect_trap_Simp
{
    class Program
    {

        public static double f(double x)
        {
            return x / (2 * x + 5);
        }

        public static double rect_m(double[] Xi, double h)
        {
            double res = 0;
            for (int i = 1; i < Xi.Length; i++)
                res += f((Xi[i] + Xi[i - 1]) / 2);
            res *= h;
            return res;
        }

       
        public static double trap_m(double[] Yi, double h)
        {
            double res = 0;
            for (int i = 1; i < Yi.Length - 1; i++)
                res += Yi[i];
            res *= 2;
            res += Yi[0];
            res += Yi[Yi.Length - 1];
            res *= h / 2;
            return res;
        }

        public static double trap_m2(double[] Yi, double h)
        {
            double res = 0;
            for (int i = 1; i < Yi.Length; i++)
                res += (Yi[i] + Yi[i - 1]);
            res *= h / 2;
            return res;
        }
        public static double Simpson_m(double[] Yi, double h)
        {
            double res = 0, tres = 0;
            for (int i = 1; i < Yi.Length - 1; i += 2)
                res += Yi[i];
            res *= 4;
            for (int i = 2; i < Yi.Length - 1; i += 2)
                tres += Yi[i];
            tres *= 2;
            res += tres;
            res += Yi[0];
            res += Yi[Yi.Length - 1];
            res *= h / 3;
            return res;
        }
        
        public static double RRR_m(double z1, double z2, double h1, double h2, int p)
        {
            double res = 0;
            res = z1 + ((z1 - z2) / (Math.Pow(h2 / h1, p) - 1));
            return res;
        }

        public static double RRR_m2(double z1, double z2, double h1, double h2, int p)
        {
            double res = 0;
            res = ((z1 - z2) / (Math.Pow(h2 / h1, p) - 1));
            return res;
        }

        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("rtS.txt").Take(4).ToArray();

            double x0 = 0; //пределы
            double xk = 0;
            double h1 = 0; //шаги
            double h2 = 0;
            double exactvalue = -0.0591223254840045;
            //input
            for (int i = 0; i < 4; i++)
            {
                double[] row = lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(Double.Parse).ToArray();
                switch (i)
                {
                    case 0:
                        x0 = row[0];
                        break;
                    case 1:
                        xk = row[0];
                        break;
                    case 2:
                        h1 = row[0];
                        break;
                    case 3:
                        h2 = row[0];
                        break;
                }
            }
            //таблицы
            double[] Xi_1 = new double[(int)((xk - x0) / h1 + 1)];
            double[] Xi_2 = new double[(int)((xk - x0) / h2 + 1)];
            double[] Yi_1 = new double[(int)((xk - x0) / h1 + 1)];
            double[] Yi_2 = new double[(int)((xk - x0) / h2 + 1)];
            //для h1
            for (int i = 0; i < Xi_1.Length; i++)
            {
                if (i == 0)
                    Xi_1[i] = x0;
                else
                    Xi_1[i] = Xi_1[i - 1] + h1;
                Yi_1[i] = f(Xi_1[i]);
            }
            //и для h2
            for (int i = 0; i < Xi_2.Length; i++)
            {
                if (i == 0)
                    Xi_2[i] = x0;
                else
                    Xi_2[i] = Xi_2[i - 1] + h2;
                Yi_2[i] = f(Xi_2[i]);
            }

            //вывод
            using (StreamWriter wr = File.AppendText("rtS_res.txt"))
            {
                //p - порядок точности расчетной формулы, для rect, trap = 2, Simpson = 4
                wr.Write("Метод прямоугольников:\nh1: " + rect_m(Xi_1, h1) + "\nh2: " + rect_m(Xi_2, h2));
                wr.Write("\nУточнение: " + RRR_m(rect_m(Xi_1, h1), rect_m(Xi_2, h2), h1, h2, 2));
                wr.Write("\nОценка погрешности: " + RRR_m2(rect_m(Xi_1, h1), rect_m(Xi_2, h2), h1, h2, 2));
                //wr.Write("\nМетод трапеций:\nh1: " + trap_m(Yi_1, h1) + "\nh2: " + trap_m(Yi_2, h2));
                wr.Write("\nМетод трапеций:\nh1: " + trap_m2(Yi_1, h1) + "\nh2: " + trap_m2(Yi_2, h2));
                wr.Write("\nУточнение: " + RRR_m(trap_m(Yi_1, h1), trap_m(Yi_2, h2), h1, h2, 2));
                wr.Write("\nОценка погрешности: " + RRR_m2(trap_m(Yi_1, h1), trap_m(Yi_2, h2), h1, h2, 2));
                wr.Write("\nМетод Симпсона:\nh1: " + Simpson_m(Yi_1, h1) + "\nh2: " + Simpson_m(Yi_2, h2));
                wr.Write("\nУточнение: " + RRR_m(Simpson_m(Yi_1, h1), Simpson_m(Yi_2, h2), h1, h2, 4));
                wr.Write("\nОценка погрешности: " + RRR_m2(Simpson_m(Yi_1, h1), Simpson_m(Yi_2, h2), h1, h2, 4));
                wr.Write("\nТочное значение интеграла: " + exactvalue);
            }
        }
    }
}
