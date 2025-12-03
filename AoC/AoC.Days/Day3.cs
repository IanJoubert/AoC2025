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

        //private static long GetValue2(string current)
        //{
        //    var strSubVals = current.Substring(0, current.Length - 11).ToCharArray().Select(a => int.Parse(a.ToString())).ToArray();
        //    var first = strSubVals.Max();

        //    var vals = current.Substring(current.IndexOf(first.ToString()) + 1).ToCharArray().Select(a => int.Parse(a.ToString())).ToArray();

        //    var result = new int[12];
        //    result[0] = first;

        //    for (int i = 0; i < vals.Length; i++)
        //    {
        //        if (vals[i] > result[1] && i <= vals.Length - 11)
        //        {
        //            result[1] = vals[i];
        //            continue;
        //        }
        //        else if (vals[i] > result[2] && i <= vals.Length - 10)
        //        {
        //            result[2] = vals[i];
        //            continue;
        //        }
        //        else if (vals[i] > result[3] && i <= vals.Length - 9)
        //        {
        //            result[3] = vals[i];
        //            continue;
        //        }
        //        else if (vals[i] > result[4] && i <= vals.Length - 8)
        //        {
        //            result[4] = vals[i];
        //            continue;
        //        }
        //        else if (vals[i] > result[5] && i <= vals.Length - 7)
        //        {
        //            result[5] = vals[i];
        //            continue;
        //        }
        //        else if (vals[i] > result[6] && i <= vals.Length - 6)
        //        {
        //            result[6] = vals[i];
        //            continue;
        //        }
        //        else if (vals[i] > result[7] && i <= vals.Length - 5)
        //        {
        //            result[7] = vals[i];
        //            continue;
        //        }
        //        else if (vals[i] > result[8] && i <= vals.Length - 4)
        //        {
        //            result[8] = vals[i];
        //            continue;
        //        }
        //        else if (vals[i] > result[9] && i <= vals.Length - 3)
        //        {
        //            result[9] = vals[i];
        //            continue;
        //        }
        //        else if (vals[i] > result[10] && i <= vals.Length - 2)
        //        {
        //            result[10] = vals[i];
        //            continue;
        //        }
        //        else if (vals[i] > result[11] && i <= vals.Length - 1)
        //        {
        //            result[11] = vals[i];
        //            continue;
        //        }
        //    }

        //    return long.Parse(string.Join("", result));
        //}
    }
}
