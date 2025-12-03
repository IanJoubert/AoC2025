namespace AoC.Common
{
    public abstract class Day
    {
        public string Part1(List<string> lines)
        {
            DoPart1(lines, out long val);
            return val.ToString();
        }

        public string Part2(List<string> lines)
        {
            DoPart2(lines, out long val);
            return val.ToString();
        }

        protected virtual List<List<long>> ParseInput(List<string> lines)
        {
            var values = new List<List<long>>();
            foreach (var line in lines)
            {
                var items = new List<long>();

                foreach (var item in line.Split(" "))
                {
                    items.Add(long.Parse(item));
                }
                values.Add(items);
            }

            return values;
        }

        protected abstract void DoPart1(List<string> lines, out long val);
        protected abstract void DoPart2(List<string> lines, out long val);
    }
}
