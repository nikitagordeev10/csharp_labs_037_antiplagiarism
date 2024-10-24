using System;
using System.Collections.Generic;

// каждый документ представляет собой список токенов
using DocumentTokens = System.Collections.Generic.List<string>;

namespace Antiplagiarism {
    public class LevenshteinCalculator {
        // сравнивает документы попарно и возвращает результаты сравнения
        public List<ComparisonResult> CompareDocumentsPairwise(List<DocumentTokens> documents) {
            // список для хранения результатов сравнения документов
            List<ComparisonResult> comparisonResults = new List<ComparisonResult>();

            // циклы для сравнения каждого документа с каждым другим документом
            for (int firstDocIndex = 0; firstDocIndex < documents.Count - 1; firstDocIndex++) {
                DocumentTokens firstDoc = documents[firstDocIndex];

                for (int secondDocIndex = firstDocIndex + 1; secondDocIndex < documents.Count; 
                    secondDocIndex++) {
                    DocumentTokens secondDoc = documents[secondDocIndex];

                    // выполнить сравнение двух документов и добавить результат в список
                    ComparisonResult result = CompareTwoDocuments(firstDoc, secondDoc);
                    comparisonResults.Add(result);
                }
            }

            // вернуть список результатов сравнения
            return comparisonResults;
        }

        // сравнить два документа и вернуть результат
        private ComparisonResult CompareTwoDocuments(DocumentTokens firstDocument, 
            DocumentTokens secondDocument) {
            // проверка входных параметров
            ValidateDocument(firstDocument, nameof(firstDocument));
            ValidateDocument(secondDocument, nameof(secondDocument));

            int rows = firstDocument.Count;
            int columns = secondDocument.Count;

            // инициализация матрицы расстояний
            double[,] distanceMatrix = InitializeDistanceMatrix(rows, columns);

            // заполнение матрицы расстояний с использованием динамического программирования
            FillDistanceMatrix(firstDocument, secondDocument, distanceMatrix);

            // получение окончательного значения расстояния из матрицы
            double finalDistance = distanceMatrix[rows, columns];

            // вернуть результат сравнения
            return new ComparisonResult(firstDocument, secondDocument, finalDistance);
        }

        // инициализация матрицы расстояний начальными значениями
        private double[,] InitializeDistanceMatrix(int rows, int columns) {
            double[,] matrix = new double[rows + 1, columns + 1];

            // инициализация первого столбца значениями от 0 до rows
            for (int i = 0; i <= rows; i++) {
                matrix[i, 0] = i;
            }

            // инициализация первой строки значениями от 0 до columns
            for (int j = 0; j <= columns; j++) {
                matrix[0, j] = j;
            }

            return matrix;
        }

        // заполнение матрицы расстояний между токенами двух документов
        private void FillDistanceMatrix(DocumentTokens firstDocument, DocumentTokens secondDocument, 
            double[,] matrix) {
            // получение количества токенов в каждом документе для оптимизации
            int firstDocCount = firstDocument.Count;
            int secondDocCount = secondDocument.Count;

            // проходим по каждой строке и столбцу матрицы, начиная с 1
            for (int i = 1; i <= firstDocCount; i++) {
                for (int j = 1; j <= secondDocCount; j++) {
                    // заполнение текущей ячейки матрицы
                    FillCell(firstDocument, secondDocument, matrix, i, j);
                }
            }
        }

        // заполнение отдельной ячейки матрицы
        private void FillCell(DocumentTokens firstDocument, DocumentTokens secondDocument, 
            double[,] matrix, int i, int j) {
            string token1 = firstDocument[i - 1];
            string token2 = secondDocument[j - 1];

            double substitutionCost = CalculateSubstitutionCost(token1, token2);

            double deleteCost = matrix[i - 1, j] + 1;
            double insertCost = matrix[i, j - 1] + 1;

            matrix[i, j] = Math.Min(Math.Min(deleteCost, insertCost), 
                GetMatchOrSubstituteCost(matrix, i, j, substitutionCost));
        }

        // вычисление стоимости замены токенов
        private double CalculateSubstitutionCost(string token1, string token2) {
            return (token1 == token2) ? 0 : TokenDistanceCalculator.GetTokenDistance(token1, token2);
        }

        // получение стоимости совпадения или замены из матрицы
        private double GetMatchOrSubstituteCost(double[,] matrix, int i, int j, 
            double substitutionCost) {
            return matrix[i - 1, j - 1] + substitutionCost;
        }

        // проверка, что документ не является null
        private void ValidateDocument(DocumentTokens document, string paramName) {
            if (document == null) {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}