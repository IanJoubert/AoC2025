using AoC.Common;

namespace AoC.Days
{

    public class Day8 : Day
    {
        private static List<Tuple<long, int, int>> BuildOrderedDistanceTuple(List<List<long>> input)
        {
            var items = new List<Tuple<long, int, int>>();
            for (int i = 0; i < input.Count; i++)
            {
                for (int j = i + 1; j < input.Count; j++)
                {
                    var distance = CalculateDistance(input[i], input[j]);
                    items.Add(Tuple.Create(distance, i, j));
                }
            }
            items.Sort((x, y) => x.Item1.CompareTo(y.Item1));
            return items;
        }

        protected override void DoPart1(List<string> lines, out long val)
        {
            var input = ParseInput(lines, ",");

            var tuples = BuildOrderedDistanceTuple(input);            

            var dsu = new DisjointSet(input.Count);

            var to = Math.Min(1000, tuples.Count);

            for (int k = 0; k < to; k++)
            {
                var p = tuples[k];
                dsu.Union(p.Item2, p.Item3);
            }

            var counts = new Dictionary<int, int>();
            for (int i = 0; i < input.Count; i++)
            {
                var root = dsu.Find(i);
                if (!counts.TryGetValue(root, out var c)) counts[root] = 1;
                else counts[root] = c + 1;
            }

            var top3 = counts.Values.OrderByDescending(x => x).Take(3).ToList();
            while (top3.Count < 3) top3.Add(1);
            val = top3.Aggregate(1L, (acc, s) => acc * s);
        }

        private static long CalculateDistance(List<long> point1, List<long> point2)
        {
            var dx = point2[0] - point1[0];
            var dy = point2[1] - point1[1];
            var dz = point2[2] - point1[2];

            return dx * dx + dy * dy + dz * dz;
        }

        private sealed class DisjointSet
        {
            private readonly int[] _parent;
            private readonly int[] _size;
            private int _circuits;

            public DisjointSet(int n)
            {
                _parent = new int[n];
                _size = new int[n];
                for (int i = 0; i < n; i++)
                {
                    _parent[i] = i;
                    _size[i] = 1;
                }

                _circuits = n;
            }

            public int NumberOfCircuits => _circuits;

            public int Find(int x)
            {
                var root = x;
                while (_parent[root] != root)
                {
                    root = _parent[root];
                }

                while (x != root)
                {
                    var next = _parent[x];
                    _parent[x] = root;
                    x = next;
                }

                return root;
            }

            public bool Union(int a, int b)
            {
                var ra = Find(a);
                var rb = Find(b);
                if (ra == rb)
                {
                    return false;
                }

                if (_size[ra] < _size[rb])
                {
                    _parent[ra] = rb;
                    _size[rb] += _size[ra];
                }
                else
                {
                    _parent[rb] = ra;
                    _size[ra] += _size[rb];
                }

                _circuits--;
                return true;
            }
        }

        protected override void DoPart2(List<string> lines, out long val)
        {
            var input = ParseInput(lines, ",");

            var tuples = BuildOrderedDistanceTuple(input);


            var dsu = new DisjointSet(input.Count);
            int lastA = -1, lastB = -1;

            foreach (var p in tuples)
            {
                var merged = dsu.Union(p.Item2, p.Item3);
                if (merged)
                {
                    if (dsu.NumberOfCircuits == 1)
                    {
                        lastA = p.Item2;
                        lastB = p.Item3;
                        break;
                    }
                }
            }

            val = input[lastA][0] * input[lastB][0];           
        }
    }

}