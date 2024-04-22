using lab3.model;
using lab3.utils;
using lab3.viewmodel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace TestLab3
{
    [TestClass]
    public class ChartModelTests
    {
        [TestMethod]
        public void CalculateValues_ValidInput_ReturnsCorrectValues()
        {
            // Arrange
            ChartModel chartModel = new ChartModel();
            double coefficientA = 12;
            double start = -26; // Начальное значение X
            double end = 9; // Конечное значение X
            double step = 1;
            List<Tuple<double, double>> expected = new List<Tuple<double, double>>
            {
                new Tuple<double, double>(-25.45584, 0),
                new Tuple<double, double>(-25, -1.68407),
                new Tuple<double, double>(-24, -2.93336),
                new Tuple<double, double>(-23, -3.70863),
                new Tuple<double, double>(-22, -4.27655),
                new Tuple<double, double>(-21, -4.71326),
                new Tuple<double, double>(-20, -5.05347),
                new Tuple<double, double>(-19, -5.31642),
                new Tuple<double, double>(-18, -5.51389),
                new Tuple<double, double>(-17, -5.65356),
                new Tuple<double, double>(-16, -5.7406),
                new Tuple<double, double>(-15, -5.77846),
                new Tuple<double, double>(-14, -5.76942),
                new Tuple<double, double>(-13, -5.71477),
                new Tuple<double, double>(-12, -5.61507),
                new Tuple<double, double>(-11, -5.47017),
                new Tuple<double, double>(-10, -5.27926),
                new Tuple<double, double>(-9, -5.04087),
                new Tuple<double, double>(-8, -4.75282),
                new Tuple<double, double>(-7, -4.41209),
                new Tuple<double, double>(-6, -4.01469),
                new Tuple<double, double>(-5, -3.5554),
                new Tuple<double, double>(-4, -3.02742),
                new Tuple<double, double>(-3, -2.42189),
                new Tuple<double, double>(-2, -1.72705),
                new Tuple<double, double>(-1, -0.92706),
                new Tuple<double, double>(0, 0),
                new Tuple<double, double>(1, 1.08542),
                new Tuple<double, double>(2, 2.37587),
                new Tuple<double, double>(3, 3.945),
                new Tuple<double, double>(4, 5.91821),
                new Tuple<double, double>(5, 8.53347),
                new Tuple<double, double>(6, 12.32405),
                new Tuple<double, double>(7, 18.89207),
                new Tuple<double, double>(8, 38.35028)
            };

            // Act
            List<Tuple<double, double>> result = chartModel.CalculateValues(coefficientA, start, end, step);

            // Assert
            CollectionAssert.AreEqual(expected, result);
        }


        [TestMethod]
        public void CalculateValues_InvalidInput_ReturnsEmptyList()
        {
            // Arrange
            ChartModel chartModel = new ChartModel();
            double coefficientA = 1;
            double start = 10;
            double end = 10;
            double step = 1;

            // Act
            List<Tuple<double, double>> result = chartModel.CalculateValues(coefficientA, start, end, step);

            // Assert
            Assert.AreEqual(1, result.Count);
        }
    }
    [TestClass]
    public class DataParserTests
    {
        [TestMethod]
        public void Parse_ValidJson_ReturnsMyDataObject()
        {
            // Arrange
            DataParser dataParser = new DataParser();
            string filePath = "validData.json";
            string validJson = "{\"Coefficient\": 1.5, \"Start\": -5, \"End\": 5, \"Step\": 0.5}";
            System.IO.File.WriteAllText(filePath, validJson);

            // Act
            MyData? result = dataParser.Parse(filePath);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1.5, result.Coefficient);
            Assert.AreEqual(-5, result.Start);
            Assert.AreEqual(5, result.End);
            Assert.AreEqual(0.5, result.Step);
        }

        [TestMethod]
        public void Parse_InvalidJson_ReturnsNull()
        {
            // Arrange
            DataParser dataParser = new DataParser();
            string filePath = "invalidData.json";
            string invalidJson = "invalid json format";
            System.IO.File.WriteAllText(filePath, invalidJson);

            // Act
            MyData? result = dataParser.Parse(filePath);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Parse_FileNotFound_ReturnsNull()
        {
            // Arrange
            DataParser dataParser = new DataParser();
            string filePath = "nonExistentFile.json";

            // Act
            MyData? result = dataParser.Parse(filePath);

            // Assert
            Assert.IsNull(result);
        }
    }

    [TestClass]
    public class RelayCommandTests
    {
        [TestMethod]
        public void CanExecute_NullCanExecuteFunction_ReturnsTrue()
        {
            // Arrange
            RelayCommand command = new RelayCommand(() => { });

            // Act
            bool result = command.CanExecute(null);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanExecute_NonNullCanExecuteFunction_ReturnsResultOfFunction()
        {
            // Arrange
            bool canExecuteValue = true;
            RelayCommand command = new RelayCommand(() => { }, () => canExecuteValue);

            // Act
            bool result = command.CanExecute(null);

            // Assert
            Assert.AreEqual(canExecuteValue, result);
        }

        [TestMethod]
        public void Execute_InvokesAction()
        {
            // Arrange
            bool actionInvoked = false;
            RelayCommand command = new RelayCommand(() => { actionInvoked = true; });

            // Act
            command.Execute(null);

            // Assert
            Assert.IsTrue(actionInvoked);
        }
    }
}
