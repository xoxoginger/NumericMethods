using System;
using System.Numerics;
using System.IO; //работа с файлами
using System.Linq; //методы Take, Select

namespace Newton_SIM_system
{
    class Program
    {
        private static float df1dx1(float x1, float x2, int m)
        {
            if (m == 0) return 0;
            else return 2 * x1 * x2;
        }

        private static float df1dx2(float x1, float x2, int m)
        {
            if (m == 0) return (1-x2) / (float)Math.Sqrt(4 - Math.Pow(x2 - 1, 2));
            else return 4 + x1 * x1; ;
        }

        private static float df2dx1(float x1, float x2, int m)
        {
            if (m == 0) return -16 * x1 / (float)Math.Pow((x1 * x1 + 4), 2);
            else return 2 * x1 - 2;
        }

        private static float df2dx2(float x1, float x2, int m)
        {
            if (m == 0) return 0;
            else return 2 * x2 - 2;
        }

        private static float f1(float x1, float x2, int m)
        {
            if (m == 0) return (float)Math.Sqrt(4 - Math.Pow((x2 - 1), 2)) + 1;
            //(int, int) point = (0, 0); - кортеж - вместо Vector2
            else return (x1 * x1 + 4) * x2 - 8;//(x1 * x1 + 4) * x2 - 8;
        }

        private static float f2(float x1, float x2, int m)
        {
            if (m == 0) return 8 / (x1 * x1 + 4);
            else return (x1 - 1) * (x1 - 1) + (x2 - 1) * (x2 - 1) - 4; //(x1 - 1) * (x1 - 1) + (x2 - 1) * (x2 - 1) - 4;
        }

        /*private static float detA1(float x1, float x2, int m)
        {
            return f1(x1, x2, m) * df2dx2(x1, x2, m) - f2(x1, x2, m) * df1dx2(x1, x2, m);
        }

        private static float detA2(float x1, float x2, int m)
        {
            return df1dx1(x1, x2, m) * f2(x1, x2, m) - df2dx1(x1, x2, m) * f1(x1, x2, m);
        }

        private static float detJ(float x1, float x2, int m)
        {
            return df1dx1(x1, x2, m) * df2dx2(x1, x2, m) - df2dx1(x1, x2, m) * df1dx2(x1, x2, m);
        }*/

        public static double norm(Vector2 X)
        {
            double norm;
            X.X = Math.Abs(X.X);
            X.Y = Math.Abs(X.Y);
            norm = Math.Max(X.X, X.Y);
            return norm;
        }

        public static float norm(float x1, float x2, int m)
        {
            float norm;
            norm = Math.Max(Math.Abs(df1dx1(x1, x2, m)) + Math.Abs(df1dx2(x1, x2, m)), Math.Abs(df2dx1(x1, x2, m)) + Math.Abs(df2dx2(x1, x2, m)));
            return norm;
        }

        public static double[] linear(float x1, float x2)
        {
            double[,] matrix = { {df1dx1(x1, x2, 1), df1dx2(x1, x2, 1) }, { df2dx1(x1, x2, 1), df2dx2(x1, x2, 1) } };
            double[] b = { -f1(x1, x2, 1), -f2(x1, x2, 1) };
            
            double[] X = new lab1_LU.Program().LUP(matrix, b);

            return X;
        }
        public static void Newtone(Vector2 x0, double e, int m)
        {
            Vector2 x = x0;
            Vector2 prev_x;
            int it = 0;

            do
            {
                float x1 = x.X;
                float x2 = x.Y;

                prev_x = x;

                x1 = prev_x.X + (float)linear(x1, x2)[0];
                x2 = prev_x.Y + (float)linear(x1, x2)[1];

               // x1 -= detA1(x1, x2, m) / detJ(x1, x2, m);
               // x2 -= detA2(x1, x2, m) / detJ(x1, x2, m);

                x.X = x1;
                x.Y = x2;

                it++;
            }
            while (norm(Vector2.Subtract(x, prev_x)) > e);

            using (StreamWriter outputFile = new StreamWriter("Newtone_res.txt"))
            {
                outputFile.WriteLine("Newtone:");
                outputFile.WriteLine("x1 = " + x.X);
                outputFile.WriteLine("x2 = " + x.Y);
                outputFile.WriteLine(it.ToString() + " iterations");
            }
        }

        public static void SIM(Vector2 x, double e, int m)
        {
            float q = norm(x.X, x.Y, 0);
            int it = 0;
            Vector2 x_prev;

            //если критерий выполнен
            if (q < 1)
            {
                do
                {
                    x_prev = x;
                    x.X = f1(x_prev.X, x_prev.Y, 0);
                    x.Y = f2(x_prev.X, x_prev.Y, 0);

                    it++;
                } while (q / (1 - q) * norm(Vector2.Subtract(x, x_prev)) > e);
                using (StreamWriter outputFile = new StreamWriter("SIM_res.txt"))
                {
                    outputFile.WriteLine("SIM:");
                    outputFile.WriteLine("x1 = " + x.X);
                    outputFile.WriteLine("x2 = " + x.Y);
                    outputFile.WriteLine(it.ToString() + " iterations");
                }
            }
            else
            {
                using (StreamWriter outputFile = new StreamWriter("SIM_res.txt"))
                {
                    outputFile.WriteLine("q > 1, use another equivalent system");
                }
            }
        }

        static void Main(string[] args)
        {
            double e;
            string[] lines = File.ReadAllLines("Newton_SIM.txt").Take(1).ToArray();
            e = Double.Parse(lines[0]);

            Vector2 x;
            x.X = 3;
            x.Y = 0.5f;

            SIM(x, e, 0);
            Newtone(x, e, 1);
        }
    }
}
