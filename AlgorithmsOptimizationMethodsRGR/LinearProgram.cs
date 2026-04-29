using System;
using System.IO;

namespace AlgorithmsOptimizationMethodsRGR
{
    public class LinearProgram
    {
        public Fraction[,] Tableau { get; private set; }
        public int RowsCount { get; private set; }
        public int ColsCount { get; private set; }

        public LinearProgram(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Файл {filePath} не найден в корне.");
            }

            string[] lines = File.ReadAllLines(filePath);
            RowsCount = lines.Length;

            string[] firstRow = lines[0].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            ColsCount = firstRow.Length;
            Tableau = new Fraction[RowsCount, ColsCount];

            for (int i = 0; i < RowsCount; i++)
            {
                string[] currentRow = lines[i].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                for (int j = 0; j < ColsCount; j++)
                {
                    Tableau[i, j] = Fraction.Parse(currentRow[j]);
                }
            }
        }
    }
}