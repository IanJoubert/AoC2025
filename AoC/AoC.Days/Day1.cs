using AoC.Common;

namespace AoC.Days
{
    public class Day1 : Day
    {
        private static List<KeyValuePair<char, int>> ParseInput(List<string> lines)
        {
            var list = new List<KeyValuePair<char, int>>();
            foreach (var item in lines)
            {
                list.Add(new KeyValuePair<char, int>(item[0], int.Parse(item[1..])));
            }
            return list;
        }

        protected override void DoPart1(List<string> lines, out long val)
        {
            var count = 0;
            var position = 50;

            var instructions = ParseInput(lines);

            foreach (var instruction in instructions)
            {
                position = GetValue(position, instruction);
                //Console.WriteLine($"{instruction.Key}:{instruction.Value} = {position}");
                if (position == 0)
                {
                    count++;
                }
            }
            val = count;
        }

        private static int GetValue(int current, KeyValuePair<char, int> instruction)
        {
            if (instruction.Key == 'R')
            {
                current = current + instruction.Value;
            }
            else if (instruction.Key == 'L')
            {
                current = current - instruction.Value;
            }
            else
            {
                throw new ArgumentException("Invalid instruction key");
            }

            while (current >= 100)
            {
                current = current - 100;
            }
            while (current < 0)
            {
                current = current + 100;
            }
            return current;
        }

        protected override void DoPart2(List<string> lines, out long val)
        {
            var count = 0;
            var position = 50;
            var instructions = ParseInput(lines);

            foreach (var instruction in instructions)
            {
                position = GetValue2(position, out var countInner, instruction);

                //Console.WriteLine($"{instruction.Key}:{instruction.Value} = {position} : {countInner}");
                count += countInner;

            }
            val = count;
        }

        private static int GetValue2(int current, out int count, KeyValuePair<char, int> instruction)
        {
            var steps = instruction.Value;
            int delta;
            if (instruction.Key == 'R')
            {
                delta = 1;
            }
            else if (instruction.Key == 'L')
            {
                delta = -1;
            }
            else
            {
                throw new ArgumentException("Invalid instruction key");
            }

            int t0 = delta == 1 ? ((100 - (current % 100)) % 100) : (current % 100);

            int first = t0 == 0 ? 100 : t0;

            int occurrences = 0;
            if (first <= steps)
            {
                occurrences = 1 + (steps - first) / 100;
            }

            int newPos = (current + delta * steps) % 100;
            if (newPos < 0) newPos += 100;

            count = occurrences;
            return newPos;
        }
    }
}
