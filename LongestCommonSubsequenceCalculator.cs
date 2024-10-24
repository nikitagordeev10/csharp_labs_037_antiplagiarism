using System;
using System.Collections.Generic;

namespace Antiplagiarism {
    public static class LongestCommonSubsequenceCalculator {
        // метод для вычисления наибольшей общей подпоследовательности
        public static List<string> Calculate(List<string> firstSequence, List<string> secondSequence) {
            int[,] optimizationTable = CreateOptimizationTable(firstSequence, secondSequence);
            return RestoreAnswer(optimizationTable, firstSequence, secondSequence);
        }

        // метод для создания таблицы оптимизации на основе двух последовательностей
        private static int[,] CreateOptimizationTable(List<string> firstSequence, 
            List<string> secondSequence) {
            // получаем длины последовательностей
            int length1 = firstSequence.Count;
            int length2 = secondSequence.Count;

            // создаем двумерный массив для таблицы оптимизации
            int[,] optTable = new int[length1 + 1, length2 + 1];

            // заполняем таблицу оптимизации значениями
            for (int i = 1; i <= length1; i++) {
                for (int j = 1; j <= length2; j++) {
                    // вычисляем значение для текущей ячейки, используя функцию 
                    optTable[i, j] = CalculateCellOptimizationValue(
                        optTable, firstSequence, secondSequence, i, j);
                }
            }

            // возвращаем готовую таблицу оптимизации
            return optTable;
        }


        // метод для вычисления значения оптимизации для ячейки таблицы
        private static int CalculateCellOptimizationValue(int[,] optTable, List<string> firstSequence, 
            List<string> secondSequence, int i, int j) {
            // проверяем, равны ли элементы на текущих позициях в двух последовательностях
            if (AreElementsEqual(firstSequence, secondSequence, i, j)) {
                // если да, увеличиваем значение на 1
                return optTable[i - 1, j - 1] + 1;
            } else {
                // не равны, берем максимум из значений в ячейках слева и сверху
                return Math.Max(optTable[i, j - 1], optTable[i - 1, j]);
            }
        }


        // метод для проверки равенства элементов на заданных позициях в двух последовательностях
        private static bool AreElementsEqual(List<string> firstSequence, List<string> secondSequence, 
            int index1, int index2) {
            return firstSequence[index1 - 1] == secondSequence[index2 - 1];
        }

        // метод для добавления элемента в начало списка lcs
        private static void AddElementToList(List<string> lcs, List<string> sequence, int index) {
            lcs.Insert(0, sequence[index - 1]);
        }

        // метод для восстановления наибольшей общей подпоследовательности по таблице оптимизации
        private static List<string> RestoreAnswer(int[,] optTable, List<string> firstSequence, 
            List<string> secondSequence) {
            // получаем длины последовательностей
            int length1 = firstSequence.Count;
            int length2 = secondSequence.Count;

            // создаем список для хранения наибольшей общей подпоследовательности (lcs)
            List<string> lcs = new List<string>();

            // пока не достигнут конец хотя бы одной из последовательностей
            while (length1 > 0 && length2 > 0) {
                // если текущие элементы равны, добавляем элемент в lcs и двигаемся по диагонали
                if (AreElementsEqual(firstSequence, secondSequence, length1, length2)) {
                    AddElementToList(lcs, firstSequence, length1);
                    length1--;
                    length2--;
                }
                // иначе двигаемся в направлении, где значение в таблице оптимизации больше
                else if (optTable[length1 - 1, length2] > optTable[length1, length2 - 1]) {
                    length1--;
                }
                // если значения равны, двигаемся вверх
                else {
                    length2--;
                }
            }

            // возвращаем наибольшую общую подпоследовательность
            return lcs;
        }
    }
}
