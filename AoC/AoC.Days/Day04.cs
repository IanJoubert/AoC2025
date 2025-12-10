using AoC.Common;

namespace AoC.Days
{

    public class Day04 : Day
    {

        private char[,] BuildFloor(List<string> lines)
        {

            var floor = new char[lines.Count, lines[0].Length];
            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = 0; j < lines[0].Length; j++)
                {
                    floor[i, j] = lines[i][j];
                }
            }
            return floor;
        }

        private void PrintFloor(char[,] floor)
        {
            for (int i = 0; i < floor.GetLength(0); i++)
            {
                for (int j = 0; j < floor.GetLength(1); j++)
                {
                    Console.Write(floor[i, j]);
                }
                Console.WriteLine();
            }
        }

        protected override void DoPart1(List<string> lines, out long val)
        {
            var sum = 0;

            var floor = BuildFloor(lines);

            for (int i = 0; i < floor.GetLength(0); i++)
            {
                for (int j = 0; j < floor.GetLength(1); j++)
                {
                    sum += GetValue(floor, i, j);
                }

                //PrintFloor(floor);
            }

            val = sum;
        }

        private static int GetValue(char[,] floor, int i, int j)
        {
            if (floor[i, j] != '@')
            {
                return 0;
            }
            var counter = 0;
            if (i > 0 && floor[i - 1, j] == '@')
            {
                counter++;
            }
            if (i < floor.GetLength(0) - 1 && floor[i + 1, j] == '@')
            {
                counter++;
            }
            if (j > 0 && floor[i, j - 1] == '@')
            {
                counter++;
            }
            if (j < floor.GetLength(1) - 1 && floor[i, j + 1] == '@')
            {
                counter++;
            }
            if (i > 0 && j > 0 && floor[i - 1, j - 1] == '@')
            {
                counter++;
            }
            if (i > 0 && j < floor.GetLength(1) - 1 && floor[i - 1, j + 1] == '@')
            {
                counter++;
            }
            if (i < floor.GetLength(0) - 1 && j > 0 && floor[i + 1, j - 1] == '@')
            {
                counter++;
            }
            if (i < floor.GetLength(0) - 1 && j < floor.GetLength(1) - 1 && floor[i + 1, j + 1] == '@')
            {
                counter++;
            }

            if (counter < 4)
            {
                return 1;
            }
            return 0;
        }

        protected override void DoPart2(List<string> lines, out long val)
        {
            var sum = 0;
            var oldSum = -1;

            var floor = BuildFloor(lines);

            while (sum > oldSum)
            {
                oldSum = sum;
                for (int i = 0; i < floor.GetLength(0); i++)
                {
                    for (int j = 0; j < floor.GetLength(1); j++)
                    {
                        sum += GetValue2(ref floor, i, j);
                    }

                    //PrintFloor(floor);
                }
            }
            val = sum;
        }
        private static int GetValue2(ref char[,] floor, int i, int j)
        {
            if (floor[i, j] != '@')
            {
                return 0;
            }
            var counter = 0;
            if (i > 0 && floor[i - 1, j] == '@')
            {
                counter++;
            }
            if (i < floor.GetLength(0) - 1 && floor[i + 1, j] == '@')
            {
                counter++;
            }
            if (j > 0 && floor[i, j - 1] == '@')
            {
                counter++;
            }
            if (j < floor.GetLength(1) - 1 && floor[i, j + 1] == '@')
            {
                counter++;
            }
            if (i > 0 && j > 0 && floor[i - 1, j - 1] == '@')
            {
                counter++;
            }
            if (i > 0 && j < floor.GetLength(1) - 1 && floor[i - 1, j + 1] == '@')
            {
                counter++;
            }
            if (i < floor.GetLength(0) - 1 && j > 0 && floor[i + 1, j - 1] == '@')
            {
                counter++;
            }
            if (i < floor.GetLength(0) - 1 && j < floor.GetLength(1) - 1 && floor[i + 1, j + 1] == '@')
            {
                counter++;
            }

            if (counter < 4)
            {
                floor[i, j] = 'X';
                return 1;
            }
            return 0;
        }
    }
}
