using System;
using System.IO; //работа с файлами
using System.Collections;
using System.Collections.Generic;
using System.Linq; //методы Take, Select

namespace lab3_Deriv
{
    class Program
    {
        static void Main(string[] args)
        {
            int size = 0;
            char c_ = ' ';
            using (StreamReader sr = File.OpenText("D_.txt"))
            {
                while (sr.Peek() != ' ')
                {
                    c_ = (char)sr.Read();
                    size = c_ - '0';

                }
            }


            string[] lines = File.ReadAllLines("D.txt").Take(size).ToArray();
            double[] X_i = new double[size];
            double[] Y_i = new double[size];
            double X = 1;
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
            
            double ddf = 0, df = 0, df_left = 0, df_right = 0;
            for (int i = 1; i < X_i.Length-1; i++)
            {
                if (X == X_i[i])
                {
                    //производная с первым порядком точности
                    df_left = (Y_i[i] - Y_i[i - 1]) / (X_i[i] - X_i[i - 1]);
                    df_right = (Y_i[i + 1] - Y_i[i]) / (X_i[i + 1] - X_i[i]);
                    //со вторым порядком
                    df = df_left + ((df_right - df_left) / (X_i[i + 1] - X_i[i - 1])) * (2 * X - X_i[i - 1] - X_i[i]);
                    //вторая производная
                    ddf = ((df_right - df_left) / (X_i[i + 1] - X_i[i - 1])) * 2;
                }
            }

            using (StreamWriter wr = File.AppendText("D_res.txt"))
            {
                wr.Write("Первая производная: df(X*) = " + df + ", вторая производная: ddf(Х*)" + ddf);
            }
        }
    }
}
