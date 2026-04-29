using System;
using System.Collections.Generic;

namespace AlgorithmsOptimizationMethodsRGR
{
    public class DualSimplexSolver
    {
        private Fraction[,] tableau;
        private int rows;
        private int cols;
        private int[] basis;

        public DualSimplexSolver(LinearProgram lp)
        {
            rows = lp.RowsCount;
            cols = lp.ColsCount;
            tableau = new Fraction[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    tableau[i, j] = lp.Tableau[i, j];
                }
            }

            basis = new int[rows - 1];

            for (int i = 0; i < rows - 1; i++)
            {
                basis[i] = -1;

                for (int j = 0; j < cols - 1; j++)
                {
                    if (tableau[i, j] == new Fraction(1) && IsUnitColumn(j, i))
                    {
                        basis[i] = j;

                        break;
                    }
                }
            }
        }

        private bool IsUnitColumn(int col, int oneRowIndex)
        {
            for (int i = 0; i < rows; i++)
            {
                if (i != oneRowIndex && tableau[i, col] != new Fraction(0))
                {
                    return false;
                }
            }

            return true;
        }

        public void Solve()
        {
            PrintTableau("Исходная таблица");

            while (true)
            {
                int leavingRow = -1;
                Fraction minB = new Fraction(0);

                for (int i = 0; i < rows - 1; i++)
                {
                    if (tableau[i, cols - 1] < minB)
                    {
                        minB = tableau[i, cols - 1];
                        leavingRow = i;
                    }
                }

                if (leavingRow == -1)
                {
                    Console.WriteLine("\nОптимальный план найден (все b >= 0).");

                    PrintSolution();

                    CheckInfiniteSolutions();

                    return;
                }

                Console.WriteLine($"\nВыходящая переменная (строка {leavingRow + 1}): b = {minB} < 0");

                int enteringCol = -1;
                Fraction minRatio = new Fraction(long.MaxValue);
                bool foundNegativeInRow = false;
                Fraction[] ratios = new Fraction[cols - 1];

                for (int j = 0; j < cols - 1; j++)
                {
                    if (tableau[leavingRow, j] < new Fraction(0))
                    {
                        foundNegativeInRow = true;
                        Fraction ratio = tableau[rows - 1, j] / (new Fraction(0) - tableau[leavingRow, j]);
                        ratios[j] = ratio;

                        if (ratio < minRatio)
                        {
                            minRatio = ratio;
                            enteringCol = j;
                        }
                    }
                }

                if (!foundNegativeInRow)
                {
                    Console.WriteLine("\nРешений нет (в строке с отрицательным b нет отрицательных коэффициентов a_ij).");

                    return;
                }

                Console.WriteLine($"Входящая переменная: x{enteringCol + 1} (min симплекс-отношение = {minRatio})");

                PrintTableauWithRatios("Расчет симплекс-отношений", leavingRow, ratios);

                basis[leavingRow] = enteringCol;

                Pivot(leavingRow, enteringCol);

                PrintTableau($"Итерация завершена: x{enteringCol + 1} вошел в базис");
            }
        }

        private void Pivot(int pivotRow, int pivotCol)
        {
            Fraction pivotElement = tableau[pivotRow, pivotCol];

            for (int j = 0; j < cols; j++)
            {
                tableau[pivotRow, j] = tableau[pivotRow, j] / pivotElement;
            }

            for (int i = 0; i < rows; i++)
            {
                if (i != pivotRow)
                {
                    Fraction multiplier = tableau[i, pivotCol];

                    for (int j = 0; j < cols; j++)
                    {
                        tableau[i, j] = tableau[i, j] - multiplier * tableau[pivotRow, j];
                    }
                }
            }
        }

        private void PrintTableau(string title)
        {
            Console.WriteLine($"\n{title}");

            PrintHeader();

            for (int i = 0; i < rows; i++)
            {
                string rowLabel = (i < rows - 1) ? $"x{basis[i] + 1}" : "Z";

                Console.Write($"{rowLabel,-8}");

                Console.Write($"{tableau[i, cols - 1],-10}");

                for (int j = 0; j < cols - 1; j++)
                {
                    Console.Write($"{tableau[i, j],-10}");
                }

                Console.WriteLine();
            }
        }

        private void PrintTableauWithRatios(string title, int lRow, Fraction[] ratios)
        {
            Console.WriteLine($"\n{title}");

            PrintHeader();

            for (int i = 0; i < rows; i++)
            {
                string rowLabel = (i < rows - 1) ? $"x{basis[i] + 1}" : "Z";

                Console.Write($"{rowLabel,-8}");

                Console.Write($"{tableau[i, cols - 1],-10}");

                for (int j = 0; j < cols - 1; j++)
                {
                    Console.Write($"{tableau[i, j],-10}");
                }

                if (i == lRow)
                {
                    Console.Write(" <-");
                }

                Console.WriteLine();
            }

            Console.Write($"{"СО",-8}{"-",-10}");

            for (int j = 0; j < cols - 1; j++)
            {
                bool isCalculated = (ratios[j].Numerator != 0 || ratios[j].Denominator != 0) && (ratios[j] != new Fraction(long.MaxValue));
                string val = isCalculated ? ratios[j].ToString() : "-";

                Console.Write($"{val,-10}");
            }

            Console.WriteLine();
        }

        private void PrintHeader()
        {
            Console.Write($"\n{"Б.П.",-8} {"1",-10}");

            for (int j = 0; j < cols - 1; j++)
            {
                Console.Write($"{"x" + (j + 1),-10}");
            }

            Console.WriteLine();

            Console.WriteLine(new string('-', 10 + cols * 10));
        }

        private void PrintSolution()
        {
            Console.WriteLine("\nОкончательный ответ");

            Fraction[] result = new Fraction[cols - 1];

            for (int i = 0; i < rows - 1; i++)
            {
                if (basis[i] != -1)
                {
                    result[basis[i]] = tableau[i, cols - 1];
                }
            }

            for (int i = 0; i < cols - 1; i++)
            {
                Console.WriteLine($"x{i + 1} = {result[i]}");
            }

            Fraction finalZ = -tableau[rows - 1, cols - 1];

            Console.WriteLine($"Z = {finalZ}");
        }

        private void CheckInfiniteSolutions()
        {
            List<int> altBasisCols = new List<int>();
            int n = cols - 1;
            int m = rows - 1;

            for (int j = 0; j < n; j++)
            {
                if (tableau[m, j] == new Fraction(0))
                {
                    bool isBasis = false;
                    for (int i = 0; i < m; i++)
                    {
                        if (basis[i] == j)
                        {
                            isBasis = true;
                            break;
                        }
                    }

                    if (!isBasis)
                    {
                        altBasisCols.Add(j);
                    }
                }
            }

            if (altBasisCols.Count > 0)
            {
                Console.WriteLine("\nНайдены признаки бесконечного множества оптимальных решений.");
                Console.WriteLine("(Индексная оценка небазисной переменной равна 0)");
                PrintGeneralSolution(altBasisCols);
            }
        }

        private void PrintGeneralSolution(List<int> altBasisCols)
        {
            int m = rows - 1;
            int n = cols - 1;

            List<int> nonBasisVars = new List<int>();

            for (int j = 0; j < n; j++)
            {
                bool isBasis = false;

                for (int k = 0; k < m; k++)
                {
                    if (basis[k] == j) 
                    { 
                        isBasis = true; 
                        break; 
                    }
                }

                if (!isBasis)
                {
                    nonBasisVars.Add(j);
                }
            }

            Console.WriteLine("Общий вид решения (Параметрический)");
            Console.WriteLine("\nСвободные переменные (параметры)");

            for (int i = 0; i < nonBasisVars.Count; i++)
            {
                int varIdx = nonBasisVars[i];
                string status = altBasisCols.Contains(varIdx) ? "произвольное t >= 0" : "0 (для оптимальности)";
                Console.WriteLine($"x{varIdx + 1} = t{i + 1}, где t{i + 1} = {status}");
            }

            Console.WriteLine("\nОбщее решение в векторном виде");
            Console.Write("X = (");

            for (int j = 0; j < n; j++)
            {
                int basisRow = -1;

                for (int k = 0; k < m; k++)
                {
                    if (basis[k] == j)
                    {
                        basisRow = k; 
                        break;
                    }
                }

                if (basisRow != -1)
                {
                    Console.Write($"{tableau[basisRow, n]}");

                    for (int k = 0; k < nonBasisVars.Count; k++)
                    {
                        Fraction coeff = -tableau[basisRow, nonBasisVars[k]];

                        if (coeff != new Fraction(0))
                        {
                            string sign = coeff > new Fraction(0) ? "+" : "-";
                            Fraction absCoeff = coeff > new Fraction(0) ? coeff : -coeff;
                            string displayCoeff = absCoeff == new Fraction(1) ? "" : $"{absCoeff} * ";
                            Console.Write($" {sign} {displayCoeff}t{k + 1}");
                        }
                    }
                }
                else
                {
                    int tIdx = nonBasisVars.IndexOf(j);
                    Console.Write($"t{tIdx + 1}");
                }

                if (j < n - 1)
                {
                    Console.Write(", ");
                }
            }
            
            Console.WriteLine(")");
            Console.WriteLine("\nПри ограничениях на параметры");
            
            for (int i = 0; i < m; i++)
            {
                bool hasParams = false;
                string expr = "";

                for (int k = 0; k < nonBasisVars.Count; k++)
                {
                    Fraction a_ij = tableau[i, nonBasisVars[k]];
                
                    if (a_ij != new Fraction(0))
                    {
                        if (hasParams && a_ij > new Fraction(0))
                        {
                            expr += " + ";
                        }

                        expr += $"{a_ij} * t{k + 1}";
                        hasParams = true;
                    }
                }

                if (hasParams)
                {
                    Console.WriteLine($"{expr} =< {tableau[i, n]}");
                }
            }

            string allT = string.Join(", ", Enumerable.Range(1, nonBasisVars.Count).Select(i => $"t{i} >= 0"));
            Console.WriteLine($"{allT}");
        }
    }
}