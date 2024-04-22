using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;

namespace lab3.model
{
    public class ChartModel
    {
        // Метод для вычисления значений функции Декартова листа
        public List<Tuple<double, double>> CalculateValues(double coefficientA, double start, double end, double step)
        {
            List<Tuple<double, double>> valuesChart = new List<Tuple<double, double>>();

            // Вычисление l
            double l = 3 * coefficientA / Math.Sqrt(2);
            valuesChart.Add(new Tuple<double, double>(Math.Round(-3 * coefficientA * Math.Sqrt(2) / 2, 5), 0));

            // Вычисление значений функции
            for (int i = 0; i * step + start <= end; i++)
            {
                double x = i * step + start;

                // Проверка на допустимые значения подкоренного выражения
                double expressionNum = l + x;
                double expressionDen = l - 3 * x;

                if (expressionNum <= 0 || expressionDen <= 0)
                {
                    continue;
                }

                // Вычисление значения функции
                if (expressionNum > 0 && expressionDen > 0)
                {
                    double y = x * Math.Pow(expressionNum / expressionDen, 0.5);
                    // Округление до 5 знаков после запятой
                    x = Math.Round(x, 5);
                    y = Math.Round(y, 5);
                    valuesChart.Add(new Tuple<double, double>(x, y));
                }
            }

            return valuesChart;
        }
    }
}
