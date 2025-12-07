using System.Text.RegularExpressions;
using AoC.Common;

namespace AoC.Days
{

    public class Day6 : Day
    {
        protected override void DoPart1(List<string> lines, out long val)
        {
            var sum = 0L;

            var result = lines.Select(s => s.Split(" ")).ToArray();

            var pivotedGrid = Pivot(result);

            foreach (var item in pivotedGrid)
            {
                if (item.Key == "*")
                {
                    sum += item.Value.Aggregate(1L, (currentProduct, nextNumber) => currentProduct * nextNumber);
                }
                else
                {
                    sum += item.Value.Sum();
                }
            }
            val = sum;
        }

        private static List<KeyValuePair<string, long[]>> Pivot(string[][] result)
        {
            var columns = new List<KeyValuePair<string, long[]>>();
            for (int i = 0; i < result[0].Length; i++)
            {
                var column = GetColumn(result, i);
                var kv = new KeyValuePair<string, long[]>(column.Last(), column.Take(column.Count() - 1).Select(long.Parse).ToArray());
                columns.Add(kv);
            }
            return columns;
        }

        private static string[] GetColumn(string[][] matrix, int columnNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(0))
                    .Select(x => matrix[x][columnNumber])
                    .ToArray();
        }


        protected override void DoPart2(List<string> lines, out long val)
        {
            var sum = 0L;

            int rows = lines.Count;
            int cols = lines.Max(l => l.Length);

            for (int i = 0; i < rows; i++)
                lines[i] = lines[i].PadRight(cols, ' ');

            char[,] grid = new char[rows, cols];
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    grid[r, c] = lines[r][c];

            bool IsSeparator(int c)
            {
                for (int r = 0; r < rows; r++)
                    if (grid[r, c] != ' ')
                        return false;
                return true;
            }

            var blocks = new List<(int start, int end)>();
            int x = cols - 1;
            while (x >= 0)
            {
                if (IsSeparator(x)) { x--; continue; }
                int end = x;
                int start = x;
                while (start > 0 && !IsSeparator(start - 1)) start--;
                blocks.Add((start, end));
                x = start - 1;
            }

            long grandTotal = 0;
            for (int p = 0; p < blocks.Count; p++)
            {
                var (start, end) = blocks[p];

                char op = '+';
                bool foundOp = false;
                for (int c = start; c <= end; c++)
                {
                    char ch = grid[rows - 1, c];
                    if (ch == '+' || ch == '*') { op = ch; foundOp = true; break; }
                }
                if (!foundOp)
                {
                    for (int c = start; c <= end; c++)
                    {
                        char ch = grid[rows - 1, c];
                        if (ch != ' ') { op = ch; foundOp = true; break; }
                    }
                }

                var numbers = new List<long>();
                for (int c = end; c >= start; c--)
                {
                    var chars = new List<char>();
                    for (int r = 0; r < rows - 1; r++)
                        chars.Add(grid[r, c]);
                    string digits = new string(chars.Where(char.IsDigit).ToArray());
                    if (digits.Length == 0) continue;
                    numbers.Add(long.Parse(digits));
                }

                long value;
                if (op == '+') value = numbers.Sum();
                else value = numbers.Aggregate(1L, (a, b) => a * b);

                sum += value;
            }
            val = sum;
        }
    }
}
