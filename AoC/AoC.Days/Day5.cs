using AoC.Common;

namespace AoC.Days
{

    public class Day5 : Day
    {
        protected override void DoPart1(List<string> lines, out long val)
        {
            var sum = 0;
            var ranges = lines.Slice(0, lines.IndexOf(""));
            var ingredients = lines.Slice(lines.IndexOf("") + 1, lines.Count - lines.IndexOf("") - 1);

            List<Tuple<long, long>> vals = new();
            foreach (var item in ranges)
            {
                var parts = item.Split('-');

                vals.Add(new Tuple<long, long>(long.Parse(parts[0]), long.Parse(parts[1])));
            }

            foreach (var ingredient in ingredients.Select(long.Parse))
            {
                foreach (var item in vals)
                {
                    if(ingredient >= item.Item1 && ingredient <= item.Item2)
                    {
                        sum += 1;
                        break;
                    }
                }
            }

            val = sum;
        }

        protected override void DoPart2(List<string> lines, out long val)
        {
            var ranges = lines.Slice(0, lines.IndexOf(""));

            var tuples = new List<Tuple<long, long>>();
            foreach (var item in ranges)
            {
                var parts = item.Split('-');
                tuples.Add(new Tuple<long, long>(long.Parse(parts[0]), long.Parse(parts[1])));
            }

            tuples.Sort((a, b) => a.Item1.CompareTo(b.Item1));

            var merged = new List<Tuple<long, long>>();
            foreach (var t in tuples)
            {
                if (merged.Count == 0)
                {
                    merged.Add(t);
                    continue;
                }

                var last = merged[merged.Count - 1];
                if (t.Item1 <= last.Item2) 
                {
                    var newRange = new Tuple<long, long>(last.Item1, Math.Max(last.Item2, t.Item2));
                    merged[merged.Count - 1] = newRange;
                }
                else
                {
                    merged.Add(t);
                }
            }

            long uniqueCount = 0;
            foreach (var r in merged)
            {
                uniqueCount += (r.Item2 - r.Item1 + 1);
            }

            val = uniqueCount;
        }
    }
}
