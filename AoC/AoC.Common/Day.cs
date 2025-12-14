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

        protected virtual List<List<long>> ParseInput(List<string> lines, string delimiter = " ")
        {
            var values = new List<List<long>>();
            foreach (var line in lines)
            {
                var items = new List<long>();

                foreach (var item in line.Split(delimiter))
                {
                    items.Add(long.Parse(item));
                }
                values.Add(items);
            }

            return values;
        }

        protected static char[,] BuildMatrix(List<string> lines)
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

        protected static Dictionary<string, List<string>> BuildGraph(List<string> lines)
        {
            var graph = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            foreach (var raw in lines.Where(l => !string.IsNullOrWhiteSpace(l)))
            {
                var parts = raw.Split(new[] { ':' }, 2);
                var node = parts[0].Trim();
                var neighbors = new List<string>();

                if (parts.Length > 1)
                {
                    neighbors = parts[1]
                        .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .Where(s => s.Length > 0)
                        .ToList();
                }

                graph[node] = neighbors;

                foreach (var n in neighbors)
                {
                    if (!graph.ContainsKey(n))
                        graph[n] = new List<string>();
                }
            }

            return graph;
        }
        protected abstract void DoPart1(List<string> lines, out long val);
        protected abstract void DoPart2(List<string> lines, out long val);
    }
}
