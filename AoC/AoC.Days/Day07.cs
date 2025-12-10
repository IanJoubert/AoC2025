using System.Text.RegularExpressions;
using AoC.Common;

namespace AoC.Days
{

    public class Day07 : Day
    {
        private static void PrintMap<T>(T[,] area)
        {
            for (int i = 0; i < area.GetLength(0); i++)
            {
                for (int j = 0; j < area.GetLength(1); j++)
                {
                    Console.Write(area[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static int PlayMap(char[,] area)
        {
            var split = 0;
            for (int i = 0; i < area.GetLength(0); i++)
            {
                for (int j = 0; j < area.GetLength(1); j++)
                {
                    if(area[i, j] == 'S' || area[i,j] == '|')
                    {
                        if(i + 1 >= area.GetLength(0))
                        {
                            break;
                        }
                        if(area[i + 1, j ] == '.')
                        {
                            area[i + 1, j ] = '|';
                        }
                        else if (area[i + 1, j ] == '^')
                        {
                            area[i + 1, j - 1] = '|';
                            area[i + 1, j + 1] = '|';
                            split++;
                        }
                    }
                }
            }
            return split;
        }

        protected override void DoPart1(List<string> lines, out long val)
        {
            var sum = 0L;

            var area = BuildMatrix(lines);
            sum = PlayMap(area);
            PrintMap(area);

            val = sum;
        }


        protected override void DoPart2(List<string> lines, out long val)
        {
            var sum = 0L;

            var area = BuildMatrix(lines);
            sum = PlayMap2(area);

            val = sum;
        }

        private static long PlayMap2(char[,] area)
        {
            var start = FindStart(area);

            var rows = area.GetLength(0);
            var cols = area.GetLength(1);

            var dp = new long[rows, cols];

            for (int c = 0; c < cols; c++)
                dp[rows - 1, c] = 1L;
            PrintMap(dp);
            for (int r = rows - 2; r >= 0; r--)
            {
                for (int c = 0; c < cols; c++)
                {
                    char below = area[r + 1, c];

                    if (below == '.')
                    {
                        dp[r, c] = dp[r + 1, c];
                    }
                    else if (below == '^')
                    {
                        long left = (c - 1 < 0) ? 1L : dp[r + 1, c - 1];
                        long right = (c + 1 >= cols) ? 1L : dp[r + 1, c + 1];
                        dp[r, c] = left + right;
                    }
                }

                PrintMap(dp);
            }

            return dp[start.row, start.col];
        }

        private static (int row, int col) FindStart(char[,] area)
        {
            for (int r = 0; r < area.GetLength(0); r++)
            {
                for (int c = 0; c < area.GetLength(1); c++)
                {
                    if (area[r, c] == 'S')
                        return (r, c);
                }
            }
            return (-1, -1);
        }

    }

}
