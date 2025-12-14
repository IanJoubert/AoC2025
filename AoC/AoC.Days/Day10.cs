using AoC.Common;
using Google.OrTools.Sat;
using static AoC.Days.Day10;

namespace AoC.Days
{

    public class Day10 : Day
    {

        protected override void DoPart1(List<string> lines, out long val)
        {
            var results = new List<int>();
            var diagrams = new List<LightDiagram>();
            foreach (var line in lines)
            {
                diagrams.Add(new LightDiagram(line));
            }

            foreach (var diagram in diagrams)
            {
                results.Add(SolveMinSwitches(diagram));
            }

            val = results.Sum();

        }

        protected override void DoPart2(List<string> lines, out long val)
        {
            var diagrams = new List<LightDiagram>();
            foreach (var line in lines)
            {
                diagrams.Add(new LightDiagram(line));
            }

            var results = new List<int>();
            foreach (var diagram in diagrams)
            {
                //results.Add(SolveMinPressesForJoltage(diagram));
                results.Add(OrToolsSolver.SolveMinPressesILP(diagram, timeLimitSeconds: 10.0));
            }

            val = results.Sum();
        }

        public int SolveMinSwitches(LightDiagram diagram)
        {
            int n = diagram.Lights.Length;
            if (diagram.CheckAllOff()) return 0;

            var switches = diagram.Switches
                .Select(s => s.SwitchArray.Select(x => (int)x).ToArray())
                .ToArray();

            if (n <= 64)
            {
                ulong start = 0UL;
                for (int i = 0; i < n; i++)
                {
                    if (diagram.Lights[i] == 1) start |= (1UL << i);
                }

                var switchMasks = new ulong[switches.Length];
                for (int i = 0; i < switches.Length; i++)
                {
                    ulong mask = 0UL;
                    foreach (var idx in switches[i])
                    {
                        if (idx >= 0 && idx < n) mask |= (1UL << idx);
                    }
                    switchMasks[i] = mask;
                }

                var visited = new HashSet<ulong> { start };
                var q = new Queue<(ulong state, int depth)>();
                q.Enqueue((start, 0));

                while (q.Count > 0)
                {
                    var (state, depth) = q.Dequeue();
                    foreach (var mask in switchMasks)
                    {
                        var next = state ^ mask; // toggle bits
                        if (next == 0UL) return depth + 1;
                        if (visited.Add(next))
                        {
                            q.Enqueue((next, depth + 1));
                        }
                    }
                }

                return -1;
            }
            else
            {
                var start = new int[n];
                for (int i = 0; i < n; i++) start[i] = diagram.Lights[i];
                var startKey = new string(start.Select(i => (char)(i + '0')).ToArray());

                var visited = new HashSet<string> { startKey };
                var q = new Queue<(string key, int depth)>();
                q.Enqueue((startKey, 0));

                while (q.Count > 0)
                {
                    var (key, depth) = q.Dequeue();
                    var keyChars = key.ToCharArray();
                    foreach (var sw in switches)
                    {
                        var nextChars = (char[])keyChars.Clone();
                        foreach (var idx in sw)
                        {
                            if (idx >= 0 && idx < n)
                            {
                                nextChars[idx] = nextChars[idx] == '1' ? '0' : '1';
                            }
                        }

                        bool allOff = true;
                        for (int i = 0; i < n; i++)
                        {
                            if (nextChars[i] == '1') { allOff = false; break; }
                        }
                        if (allOff) return depth + 1;

                        var nextKey = new string(nextChars);
                        if (visited.Add(nextKey))
                        {
                            q.Enqueue((nextKey, depth + 1));
                        }
                    }
                }
                return -1;
            }
        }


        public int SolveMinPressesForJoltage(LightDiagram diagram)
        {
            var target = diagram.Joltage;
            int m = target.Length;

            bool allZero = true;
            for (int i = 0; i < m; i++) if (target[i] != 0) { allZero = false; break; }
            if (allZero) return 0;

            var buttons = diagram.Switches
                .Select(s => s.SwitchArray.Select(x => (int)x).Where(idx => idx >= 0 && idx < m).Distinct().ToArray())
                .Where(arr => arr.Length > 0)
                .ToArray();

            for (int i = 0; i < m; i++)
            {
                if (target[i] > 0)
                {
                    bool touched = false;
                    foreach (var btn in buttons)
                    {
                        if (btn.Contains(i)) { touched = true; break; }
                    }
                    if (!touched) return -1;
                }
            }

            long totalStates = 1;
            for (int i = 0; i < m; i++)
            {
                totalStates *= (long)target[i] + 1L;
                if (totalStates > int.MaxValue) break;
            }

            if (totalStates <= int.MaxValue)
            {
                var baseSizes = new int[m];
                long product = 1;
                for (int i = 0; i < m; i++)
                {
                    baseSizes[i] = target[i] + 1;
                    product *= baseSizes[i];
                }

                var multipliers = new int[m];
                multipliers[0] = 1;
                for (int i = 1; i < m; i++)
                {
                    multipliers[i] = multipliers[i - 1] * baseSizes[i - 1];
                }

                var visited = new bool[product];
                var q = new Queue<(int index, int depth, int[] state)>();

                var startState = new int[m];
                int startIndex = 0;
                visited[startIndex] = true;
                q.Enqueue((startIndex, 0, startState));

                while (q.Count > 0)
                {
                    var (index, depth, state) = q.Dequeue();

                    foreach (var btn in buttons)
                    {
                        var next = (int[])state.Clone();
                        bool valid = true;
                        for (int k = 0; k < btn.Length; k++)
                        {
                            int idx = btn[k];
                            next[idx]++;
                            if (next[idx] > target[idx])
                            {
                                valid = false;
                                break;
                            }
                        }
                        if (!valid) continue;

                        bool reached = true;
                        for (int i = 0; i < m; i++)
                        {
                            if (next[i] != target[i]) { reached = false; break; }
                        }
                        if (reached) return depth + 1;

                        int nextIndex = 0;
                        for (int i = 0; i < m; i++)
                        {
                            nextIndex += next[i] * multipliers[i];
                        }

                        if (!visited[nextIndex])
                        {
                            visited[nextIndex] = true;
                            q.Enqueue((nextIndex, depth + 1, next));
                        }
                    }
                }

                return -1;
            }
            else
            {
                return SolveMinPressesForJoltage_AStar(target, buttons);
            }
        }

        private int SolveMinPressesForJoltage_AStar(int[] target, int[][] buttons)
        {
            int m = target.Length;
            int maxCoverage = 0;
            foreach (var b in buttons) if (b.Length > maxCoverage) maxCoverage = b.Length;
            if (maxCoverage == 0) return -1;

            static int Heuristic(int[] state, int[] target, int maxCoverage)
            {
                long sumRem = 0;
                int maxRem = 0;
                for (int i = 0; i < state.Length; i++)
                {
                    int rem = target[i] - state[i];
                    if (rem > 0)
                    {
                        sumRem += rem;
                        if (rem > maxRem) maxRem = rem;
                    }
                }
                if (sumRem <= 0) return 0;
                int lowerByCoverage = (int)((sumRem + maxCoverage - 1) / maxCoverage);
                return Math.Max(lowerByCoverage, maxRem);
            }

            var pq = new PriorityQueue<(int[] state, int g), int>();
            var start = new int[m];
            int h0 = Heuristic(start, target, maxCoverage);
            pq.Enqueue((start, 0), h0);

            var bestG = new Dictionary<string, int>(StringComparer.Ordinal);

            string KeyFromState(int[] s)
            {
                var chars = new char[s.Length];
                for (int i = 0; i < s.Length; i++) chars[i] = (char)s[i];
                return new string(chars);
            }

            bestG[KeyFromState(start)] = 0;

            const int MAX_EXPANSIONS = 5_000_000;
            int expansions = 0;

            while (pq.Count > 0)
            {
                var (state, g) = pq.Dequeue();

                if (++expansions > MAX_EXPANSIONS) return -1;

                bool isGoal = true;
                for (int i = 0; i < m; i++)
                {
                    if (state[i] != target[i]) { isGoal = false; break; }
                }
                if (isGoal) return g;

                foreach (var btn in buttons)
                {
                    var next = (int[])state.Clone();
                    bool valid = true;
                    for (int j = 0; j < btn.Length; j++)
                    {
                        int idx = btn[j];
                        next[idx]++;
                        if (next[idx] > target[idx]) { valid = false; break; }
                    }
                    if (!valid) continue;

                    var key = KeyFromState(next);
                    int g2 = g + 1;
                    if (bestG.TryGetValue(key, out var known) && known <= g2) continue;
                    bestG[key] = g2;

                    int h = Heuristic(next, target, maxCoverage);
                    int f = g2 + h;
                    pq.Enqueue((next, g2), f);
                }
            }

            return -1;
        }

        private int SolveMinPressesForJoltage_HashSet(LightDiagram diagram, int[] target, int[][] buttons)
        {
            int m = target.Length;
            string StartKey() => new string(Enumerable.Repeat('0', m).ToArray());

            var visited = new HashSet<string> { StartKey() };
            var q = new Queue<(int[] state, int depth)>();
            q.Enqueue((new int[m], 0));

            while (q.Count > 0)
            {
                var (state, depth) = q.Dequeue();

                foreach (var btn in buttons)
                {
                    var next = (int[])state.Clone();
                    bool valid = true;
                    foreach (var idx in btn)
                    {
                        next[idx]++;
                        if (next[idx] > target[idx]) { valid = false; break; }
                    }
                    if (!valid) continue;

                    bool reached = true;
                    for (int i = 0; i < m; i++)
                    {
                        if (next[i] != target[i]) { reached = false; break; }
                    }
                    if (reached) return depth + 1;

                    var keyChars = next.Select(v => (char)(v + '0')).ToArray();
                    var key = new string(keyChars);
                    if (visited.Add(key))
                    {
                        q.Enqueue((next, depth + 1));
                    }
                }
            }

            return -1;
        }


        public class LightDiagram
        {
            public int[] Lights;
            public int[] Joltage;
            public List<Switches> Switches;

            public LightDiagram(string line)
            {
                var items = line.Split(' ');
                Switches = new List<Switches>();

                Lights = items[0].ToCharArray().Skip(1).Take(items[0].IndexOf(']') - 1).Select(s => s == '#' ? 1 : 0).ToArray();

                var startBrace = line.IndexOf('{');
                var endBrace = line.IndexOf('}');
                if (startBrace >= 0 && endBrace > startBrace)
                {
                    var content = line.Substring(startBrace + 1, endBrace - startBrace - 1).Trim();
                    if (string.IsNullOrEmpty(content))
                    {
                        Joltage = new int[0];
                    }
                    else
                    {
                        Joltage = content.Split(',')
                                         .Select(tok => int.Parse(tok.Trim()))
                                         .ToArray();
                    }
                }
                else
                {
                    Joltage = new int[0];
                }

                for (int i = 1; i < items.Length - 1; i++)
                {
                    Switches.Add(new Switches(items[i]));
                }
            }

            public LightDiagram(LightDiagram diagram)
            {
                Lights = (int[])diagram.Lights.Clone();
                Joltage = (int[])diagram.Joltage.Clone();
                Switches = diagram.Switches;
            }

            public void ApplySwitch(int switchIndex)
            {
                var sw = Switches[switchIndex];
                foreach (var item in sw.SwitchArray)
                {
                    Lights[item] = Lights[item] == 1 ? 0 : 1;
                }
            }

            public bool CheckAllOff()
            {
                return Lights.All(s => s == 0);
            }

            public bool CheckMatchesJoltage()
            {
                for (int i = 0; i < Lights.Length; i++)
                {
                    if (Lights[i] != Joltage[i])
                    {
                        return false;
                    }
                }
                return true;
            }

            public bool CheckMatchesJoltage(int[] lights)
            {
                for (int i = 0; i < lights.Length; i++)
                {
                    if (lights[i] != Joltage[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public class Switches
        {
            public int[] SwitchArray;

            public Switches(string item)
            {
                SwitchArray = item.Replace("(", "").Replace(")", "").Split(',').Select(int.Parse).ToArray();
            }
        }
    }

    public static class OrToolsSolver
    {
        public static int SolveMinPressesILP(LightDiagram diagram, double timeLimitSeconds = 5.0)
        {
            var target = diagram.Joltage;
            int m = target.Length;
            int k = diagram.Switches.Count;

            for (int i = 0; i < m; i++)
            {
                if (target[i] > 0)
                {
                    bool touched = false;
                    for (int j = 0; j < k; j++)
                    {
                        if (diagram.Switches[j].SwitchArray.Contains(i)) { touched = true; break; }
                    }
                    if (!touched) return -1;
                }
            }

            var model = new CpModel();

            int ub = target.Sum();

            var x = new IntVar[k];
            for (int j = 0; j < k; j++)
            {
                x[j] = model.NewIntVar(0, ub, $"x_{j}");
            }

            for (int i = 0; i < m; i++)
            {
                var touching = new List<IntVar>();
                for (int j = 0; j < k; j++)
                {
                    if (diagram.Switches[j].SwitchArray.Contains(i)) touching.Add(x[j]);
                }

                if (touching.Count == 0)
                {
                    if (target[i] == 0) continue;
                    return -1;
                }

                model.Add(LinearExpr.Sum(touching) == target[i]);
            }

            model.Minimize(LinearExpr.Sum(x));

            var solver = new CpSolver
            {
                StringParameters = $"max_time_in_seconds: {timeLimitSeconds}"
            };

            var status = solver.Solve(model);
            if (status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible)
            {
                return (int)solver.ObjectiveValue;
            }
            return -1;
        }
    }

}