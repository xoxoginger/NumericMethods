using System;
using System.IO; //работа с файлами
using System.Collections;
using System.Collections.Generic;
using System.Linq; //методы Take, Select

namespace lab1_mpi_zdl
{
    public class Program
    {
        public static double norm(double[,] a, int size)
        {
            double[] sum = new double[size];
            double norm = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0, n = 0; n < size; n++, j++)
                {
                    sum[i] += Math.Abs(a[i, j]);
                }
            }
            norm = sum.Max();
            return norm;
        }

        public static double norm(double[] X, int size)
        {
            double norm = 0;
            for (int i = 0; i < size; i++)
            {
                X[i] = Math.Abs(X[i]);
            }
            norm = X.Max();
            return norm;
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
        public static double[] subt(double[] a, double[] b)
        {
            double[] r = new double[b.GetLength(0)];
            for(int i = 0; i < a.GetLength(0); i++)
            {
                r[i] = a[i] - b[i];
            }
            return r;
        }
        public static double[,] subt(double[,] a, double[,] b)
        {
            double[,] r = new double[a.GetLength(0), a.GetLength(1)];
            for (int i = 0; i < a.GetLength(1); i++)
            {
                for (int j = 0; j < a.GetLength(0); j++)
                {
                    r[i,j] = a[i,j] - b[i,j];
                }
            }
            return r;
        }

        public static double[] add(double[] a, double[] b)
        {
            double[] r = new double[a.GetLength(0)];
            for (int i = 0; i < a.GetLength(0); i++)
            {
                    r[i] = a[i] + b[i];
            }
            return r;
        }

        public static double[,] reversed_m(double[,] m)
        {
            int n = m.GetLength(0);

            double[,] rev = new double[n, n];
            for (int i = 0; i < n; i++)
                rev[i, i] = 1;

            double[,] big_m = new double[n, 2 * n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                {
                    big_m[i, j] = m[i, j];
                    big_m[i, j + n] = rev[i, j];
                }

            //Прямой ход (Зануление нижнего левого угла)
            for (int k = 0; k < n; k++) //k-номер строки
            {
                //меняем строки, если на гл.диагонали 0
                if (big_m[k, k] == 0)
                {
                    for(int l = k+1; l < n; l++)
                    {
                        if (big_m[l, l] == 0)
                            continue;
                        else
                        {
                            for (int j = 0; j < n * 2; j++)
                            {
                                double temp;
                                temp = big_m[l, j];
                                big_m[l, j] = big_m[k, j];
                                big_m[k, j] = temp;
                            }
                            for (int j = 0; j < n; j++)
                            {
                                double temp;
                                temp = m[l, j];
                                m[l, j] = m[k, j];
                                m[k, j] = temp;
                            }
                            break;
                        }
                    }
                }
                for (int i = 0; i < 2 * n; i++) //i-номер столбца
                {
                    big_m[k, i] = big_m[k, i] / m[k, k]; //Деление k-строки на первый член !=0 для преобразования его в единицу
                }
                for (int i = k + 1; i < n; i++) //i-номер следующей строки после k
                {
                    double K = big_m[i, k] / big_m[k, k]; //Коэффициент
                    for (int j = 0; j < 2 * n; j++) //j-номер столбца следующей строки после k
                        big_m[i, j] = big_m[i, j] - big_m[k, j] * K; //Зануление элементов матрицы ниже первого члена, преобразованного в единицу
                }
                for (int i = 0; i < n; i++) //Обновление, внесение изменений в начальную матрицу
                    for (int j = 0; j < n; j++)
                        m[i, j] = big_m[i, j];
            }

            //Обратный ход (Зануление верхнего правого угла)
            for (int k = n - 1; k > -1; k--) //k-номер строки
            {
                for (int i = 2 * n - 1; i > -1; i--) //i-номер столбца
                    big_m[k, i] = big_m[k, i] / m[k, k];
                for (int i = k - 1; i > -1; i--) //i-номер следующей строки после k
                {
                    double K = big_m[i, k] / big_m[k, k];
                    for (int j = 2 * n - 1; j > -1; j--) //j-номер столбца следующей строки после k
                        big_m[i, j] = big_m[i, j] - big_m[k, j] * K;
                }
            }

            //Отделяем от общей матрицы
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    rev[i, j] = big_m[i, j + n];

            return rev;
        }
        public double[] mpi(double[,]a, double[]b,int size, double e)
        {
            double e_n = 1;
            double[] X = new double[size];
            double[] X_prev = new double[size];

            for (int i = 0; i < size; i++)
            {
                X_prev[i] = b[i];
            }

            double[] r = new double[X_prev.GetLength(0)];
            int it = 0;
            while (e_n > e)
            {
                r = mult(a, X_prev);
                for (int i = 0; i < size; i++)
                {
                    X[i] = b[i] + r[i];
                }
                if (norm(a, size) >= 1)
                    e_n = norm(subt(X, X_prev), size);
                else
                    e_n = norm(a, size) / (1 - norm(a, size)) * norm(subt(X, X_prev), size);
                for (int i = 0; i < size; i++)
                {
                    X_prev[i] = X[i];
                    X[i] = 0;
                }
                it++;
            }

            return X_prev;
            //вывод
            /*using (StreamWriter outputFile = new StreamWriter("mpi_res.txt"))
            {
                outputFile.WriteLine("MPI:");
                foreach (double x in X_prev)
                    outputFile.WriteLine(x);
                outputFile.WriteLine(it.ToString() + " iterations");
            }*/
        }

        public double[] zdl(double[,] a, double[] b, int size, double e)
        {
            //вспомогательные матрицы
            double[,] C = new double[size, size];
            double[,] D = new double[size, size];
            double[,] E = new double[size, size];
            //заполняем матрицы
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i == j)
                        E[i, j] = 1;
                    else
                        E[i, j] = 0;
                    if (j >= i)
                    {
                        D[i, j] = a[i, j];
                        C[i, j] = 0;
                    }
                    else
                    {
                        C[i, j] = a[i, j];
                        D[i, j] = 0;
                    }
                }
            }

            double e_n = 1;
            double[] X = new double[size];
            double[] X_prev = new double[size];

            for (int i = 0; i < size; i++)
            {
                X_prev[i] = b[i];
            }

            double[] r = new double[X_prev.GetLength(0)];

            //int it = 0;

            while (e_n > e)
            {
                X = add(mult(mult(reversed_m(subt(E, C)), D), X_prev), mult(reversed_m(subt(E, C)), b));

                if (norm(a, size) >= 1)
                    e_n = norm(subt(X, X_prev), size);
                else
                    e_n = norm(D, size) / (1 - norm(a, size)) * norm(subt(X, X_prev), size);

                for (int i = 0; i < size; i++)
                {
                    X_prev[i] = X[i];
                    X[i] = 0;
                }

              //  it++;
            }

            return X_prev;
        }
        static void Main()
        {
            double e = 0.01;
            int size = 0;
            char c_ = ' ';
            using (StreamReader sr = File.OpenText("mpi__zdl.txt"))
            {
                while (sr.Peek() != ' ')
                {
                    c_ = (char)sr.Read();
                    size = c_ - '0';

                }
            }

            // прочитать первые 5 строк файла
            string[] lines = File.ReadAllLines("mpi_zdl.txt").Take(size).ToArray();

            double[,] a = new double[size, size];
            double[] b = new double[size];

            double[] x_n = new double[size];
            // разобрать в массивы
            for (int i = 0; i < size; i++)
            {

                double[] row = lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(Double.Parse).ToArray();
                for (int j = 0, n = 0; n < size + 1; n++, j++)
                {

                    if (j == i)
                    {
                        x_n[i] = row[j];
                        a[i, j] = 0;
                        //slau[i, j] = 0;
                    }

                    else if (j == size)
                    {
                        b[i] = row[j];
                    }

                    else
                    {
                        a[i, j] = row[j];
                    }
                }
            }
            
            
            //приводим к эквивалентному виду
            for (int i = 0; i < size; i++)
            {
                for (int j = 0, n = 0; n < size; n++, j++)
                {
                    a[i, j] = -a[i, j] / x_n[i]; 
                }
                b[i] /= x_n[i];
            }
            

            //mpi(a, b, size, e);
            
            //double[] X_prev = zdl(a, b, size, e);
            /*//вывод
            using (StreamWriter outputFile = new StreamWriter("zdl_res.txt"))
            {
                outputFile.WriteLine("ZDL:");
                foreach (double x in X_prev)
                    outputFile.WriteLine(x);
                //outputFile.WriteLine(it.ToString() + " iterations");
            }*/
        }

       
    }
}
