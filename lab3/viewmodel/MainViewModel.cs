using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using lab3.model;
using lab3.utils;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Series;

namespace lab3.viewmodel
{
    public class MainViewModel : ObservableObject
    {
        private double _coefficientA;
        private double _start;
        private double _end;
        private double _step;
        private PlotModel _plotModel;
        private List<Tuple<double, double>> _points;
        private readonly ChartModel _chartModel = new ChartModel();
        private readonly RelayCommand _calculateCommand;
        public readonly RelayCommand _openFileCommand;
        public readonly RelayCommand _startupDialogCommand;
        public readonly RelayCommand _saveInFileCommand;

        public MainViewModel()
        {
            // Initialize PlotModel
            PlotModel = new PlotModel { Title = "Example 1" };

            // Создаем команду для вычисления
            _calculateCommand = new RelayCommand(Calculate);
            _openFileCommand = new RelayCommand(LoadDataFromFile);
            _startupDialogCommand = new RelayCommand(ShowStartupInfo);
            _saveInFileCommand = new RelayCommand(SaveInFile);
        }

        public double CoefficientA
        {
            get => _coefficientA;
            set => SetProperty(ref _coefficientA, value);
        }

        public double Start
        {
            get => _start;
            set => SetProperty(ref _start, value);
        }

        public double End
        {
            get => _end;
            set => SetProperty(ref _end, value);
        }

        public double Step
        {
            get => _step;
            set => SetProperty(ref _step, value);
        }

        public PlotModel PlotModel
        {
            get => _plotModel;
            private set => SetProperty(ref _plotModel, value);
        }

        public List<Tuple<double, double>> Points
        {
            get => _points;
            private set => SetProperty(ref _points, value);
        }

        public RelayCommand CalculateCommand => _calculateCommand;
        public RelayCommand OpenFileCommand => _openFileCommand;
        public RelayCommand StartupDialogCommand => _startupDialogCommand;
        public RelayCommand SaveInFileCommand => _saveInFileCommand;

        public void Calculate()
        {
            try
            {
                if (Step <= 0 || Start >= End)
                {
                    throw new ArgumentException("Недопустимые параметры для построения функции");
                }

                // Вычисляем точки для графика
                Points = _chartModel.CalculateValues(CoefficientA, Start, End, Step);

                // Проверяем, если у нас только одна точка или пустой список
                if (Points.Count <= 1)
                {
                    throw new InvalidOperationException("Невозможно построить график функции в заданном интервале");
                }

                // Создаем новый PlotModel с обновленными данными
                PlotModel = CreatePlotModel(Points);
                List<Tuple<double, double>> newPoints = new List<Tuple<double, double>>();

                for (int i = 0; i < Points.Count; i++)
                {
                    Tuple<double, double> point = Points[i];
                    newPoints.Add(point);
                    if(i==0 || point.Item1 == 0)
                        newPoints.Add(new Tuple<double, double>(point.Item1, point.Item2));
                    else
                        newPoints.Add(new Tuple<double, double>(point.Item1, -point.Item2));
                }
                Points = newPoints;
            }
            catch (ArgumentException ex)
            {
                // В случае недопустимых параметров выводим сообщение об ошибке
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (InvalidOperationException ex)
            {
                // В случае невозможности построения графика выводим предупреждение
                MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                // Здесь можно предложить пользователю изменить границы построения графика
            }
            catch (Exception ex)
            {
                // В случае других ошибок выводим общее сообщение об ошибке
                MessageBox.Show("Произошла непредвиденная ошибка" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private PlotModel CreatePlotModel(List<Tuple<double, double>> data)
        {
            var plotModel = new PlotModel();

            var lineSeries1 = new LineSeries
            {
                Color = OxyColors.Blue // Устанавливаем цвет линии
            };
            var lineSeries2 = new LineSeries
            {
                Color = OxyColors.Blue // Устанавливаем цвет линии
            };

            foreach (var point in data)
            {
                lineSeries1.Points.Add(new DataPoint(point.Item1, point.Item2));
                lineSeries2.Points.Add(new DataPoint(point.Item1, -point.Item2));
            }

            // Добавляем первое значение в обе линии
            var firstPoint = data.FirstOrDefault();
            if (firstPoint != null)
            {
                lineSeries1.Points.Insert(0, new DataPoint(firstPoint.Item1, firstPoint.Item2));
                lineSeries2.Points.Insert(0, new DataPoint(firstPoint.Item1, firstPoint.Item2));
            }

            // Добавляем последнее значение в обе линии
            var lastPoint = data.LastOrDefault();
            if (lastPoint != null)
            {
                lineSeries1.Points.Add(new DataPoint(lastPoint.Item1, lastPoint.Item2));
                lineSeries2.Points.Add(new DataPoint(lastPoint.Item1, -lastPoint.Item2));
            }

            plotModel.Series.Add(lineSeries1);
            plotModel.Series.Add(lineSeries2);

            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "X", Minimum = Start, Maximum = End });
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = OxyPlot.Axes.AxisPosition.Left, Title = "Y" });

            return plotModel;
        }



        private void LoadDataFromFile()
        {
            // Диалог выбора файла
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt"; // Устанавливаем фильтр для файлов

            if (openFileDialog.ShowDialog() == true)
            {
                // Получение пути к выбранному файлу
                string filePath = openFileDialog.FileName;

                if (Path.GetExtension(filePath) != ".txt")
                {
                    // Если выбран неподходящий файл, выводим предупреждение
                    MessageBox.Show("Допустимы только файлы с расширением .txt", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return; // Завершаем метод
                }

                try
                {
                    DataParser parser = new DataParser();
                    MyData? data = parser.Parse(filePath);

                    if (data != null)
                    {
                        CoefficientA = data.Coefficient;
                        Start = data.Start;
                        End = data.End;
                        Step = data.Step;
                        Calculate();
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка при чтении файла: неверный формат", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при чтении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void SaveInFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt"; // Устанавливаем фильтр для файлов

            if (saveFileDialog.ShowDialog() == true)
            {
                // Получение пути к выбранному файлу
                string filePath = saveFileDialog.FileName;

                try
                {
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        foreach (var point in Points)
                        {
                            writer.WriteLine($"{point.Item1} ; {point.Item2}");
                        }
                        foreach (var point in Points)
                        {
                            writer.WriteLine($"{point.Item1} ; {-point.Item2}");
                        }
                    }

                    MessageBox.Show("Результат сохранен успешно", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        public static void ShowStartupInfo()
        {
            MessageBoxResult result = MessageBox.Show(
                "Добро пожаловать! Это информация при запуске программы." +
                "\nРабота №1. Алгоритмы и структуры данных\nПервая лабораторная работа предназначена для приобретения практического опыта в создании простейшего приложения с использованием языка программирования С#.\n" +
                "\nЗадание 16 варианта: Студент выполняет задание обычной или повышенной сложности." +
                "\nНеобходимо написать приложение с использованием технологии WinForms для " +
                "\nПостроения графика функции и вывода таблицы значений функции. Пользователь задает " +
                "\nПравую и левую границу, шаг, коэффициенты (при их наличии). При невозможности" +
                "\nПостроить график функции в заданном интервале пользователю выдается предупреждение " +
                "\nОб этом с предложением сменить границы построения. Если график функции из-за " +
                "\nКоэффициентов вырождается в точку или не может быть построен пользователь также видит предупреждение." +
                "\n Функция: Декартов Лист" +
                "\n\nПоказывать ли данное сообщение в будущем при запуске программы ?" +
                "\n\nЕсли вы указали нет, то сможете найти инфомацию в разделе Информация о разработчике",
                "Программу разработал Харисов Ильяс Ренатович, 424 группа",
                MessageBoxButton.YesNo);

            // Если нажата кнопка "ОК", ничего не делаем
            if (result == MessageBoxResult.Yes)
            {
                SettingsManager.SaveShowStartupMessageSetting(true);
            }
            // Если нажата кнопка "Не показывать", сохраняем настройку
            else if (result == MessageBoxResult.No)
            {
                // Сохраняем настройку, чтобы больше не показывать всплывающее окно
                SettingsManager.SaveShowStartupMessageSetting(false);
            }
        }
    }
}
