using System;
using AoC.Common;

namespace AoC.Days
{
    
    public class Day02 : Day
    {
        private static List<Tuple<long, long>> ParseInput(List<string> lines)
        {
            var list = new List<Tuple<long, long>>();
            foreach (var item in lines)
            {
                var items = item.Split(',');
                foreach(var t in items)
                {
                    var startandend = t.Split("-");
                    list.Add(new Tuple<long, long>(long.Parse(startandend[0]), long.Parse(startandend[1])));
                }
            }
            return list;
        }

        protected override void DoPart1(List<string> lines, out long val)
        {
            var sum = 0L;
            var instructions = ParseInput(lines);

            foreach (var items in instructions)
            {
                for (long i = items.Item1; i <= items.Item2; i++)
                {
                    sum += GetValue(i.ToString());
                }
            }
            
            val = sum;
        }

        private static long GetValue(string current)
        {
            var first = current.Substring(0, current.Length / 2);
            var second = current.Substring(current.Length / 2);

            if (first.Equals(second))
            {
                // Console.WriteLine($"Found match: {current}");
                return long.Parse(current);
            }
            return 0;
        }

        protected override void DoPart2(List<string> lines, out long val)
        {
            var sum = 0L;
            var instructions = ParseInput(lines);

            foreach (var items in instructions)
            {
                for (long i = items.Item1; i <= items.Item2; i++)
                {
                    sum += GetValue2(i.ToString());
                }
            }

            val = sum;
        }

        private static long GetValue2(string current)
        {
            var len = current.Length;
            for (int partLen = 1; partLen <= len / 2; partLen++)
            {
                if (len % partLen != 0)
                    continue;

                var part = current.Substring(0, partLen);
                var repeatCount = len / partLen;

                var sb = new System.Text.StringBuilder(len);
                for (int i = 0; i < repeatCount; i++)
                    sb.Append(part);

                if (sb.ToString().Equals(current, StringComparison.Ordinal))
                {
                    // Console.WriteLine($"Found match: {current} (pattern: {part})");
                    return long.Parse(current);
                }
            }

            return 0;
        }
    }
}
