using System;
using AoC.Common;

namespace AoC.Days
{
    
    public class Day3 : Day
    {

        protected override void DoPart1(List<string> lines, out long val)
        {
            var sum = 0L;
            
            foreach(var item in lines)
            {
                sum += GetValue(item);
            }

            val = sum;
        }

        private static int GetValue(string battery)
        {
            int first = 0, second = 0;
            for(int i = 0; i < battery.Length; i++)
            {
                var num = int.Parse(battery[i].ToString());
                if(num > first && i != battery.Length - 1)
                {
                    first = num;
                    second = 0;
                }
                else if(num > second)
                {
                    second = num;
                }
            }
            var val = $"{first}{second}";
            return int.Parse(val);
        }

        protected override void DoPart2(List<string> lines, out long val)
        {
            var sum = 0L;

            foreach (var item in lines)
            {
                var result  = GetValue2(item);
                Console.WriteLine(result);
                sum += result;
            }

            val = sum;
        }
        private static long GetValue2(string current)
        {
            const int required = 12;

            var n = current.Length;
            var resultChars = new char[required];
            int start = 0;

            for (int pos = 0; pos < required; pos++)
            {
                int remainingNeeded = required - pos;

                int endInclusive = n - remainingNeeded;
                char best = '\0';
                int bestIndex = start;
                for (int i = start; i <= endInclusive; i++)
                {
                    var c = current[i];
                    if (c > best)
                    {
                        best = c;
                        bestIndex = i;

                        if (best == '9')
                            break;
                    }
                }

                resultChars[pos] = best;
                start = bestIndex + 1;
            }

            var resultStr = new string(resultChars);
            return long.Parse(resultStr);
        }
    }
}
