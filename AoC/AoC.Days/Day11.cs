using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Common;

namespace AoC.Days
{
    public class Day11 : Day
    {
        protected override void DoPart1(List<string> lines, out long val)
        {
            var graph = BuildGraph(lines);
            val = CountSimplePaths(graph, "you", "out", StringComparer.OrdinalIgnoreCase);
        }

        protected override void DoPart2(List<string> lines, out long val)
        {
            var graph = BuildGraph(lines);
            const string reqA = "dac";
            const string reqB = "fft";

            Console.WriteLine($"[Day11] input lines = {lines.Count}");

            if (!graph.ContainsKey("svr") || !graph.ContainsKey("out"))
            {
                val = 0;
                return;
            }

            // Strongly connected components
            var scc = StronglyConnectedComponents(graph);
            bool isDag = scc.Components.All(c => c.Count == 1) && !HasSelfLoop(graph);

            Console.WriteLine($"[Day11] SCC count = {scc.Components.Count}; isDag = {isDag}");

            if (isDag)
            {
                // Topological order
                var topo = TopologicalSort(graph);
                // DP: for each node keep 4-state mask count: bit0 = visited dac, bit1 = visited fft
                var dp = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
                var dpMask = new Dictionary<string, long[]>(StringComparer.OrdinalIgnoreCase);
                foreach (var node in topo)
                {
                    dpMask[node] = new long[4];
                }

                int startMask = 0;
                if (string.Equals("svr", reqA, StringComparison.OrdinalIgnoreCase)) startMask |= 1;
                if (string.Equals("svr", reqB, StringComparison.OrdinalIgnoreCase)) startMask |= 2;
                dpMask["svr"][startMask] = 1;

                foreach (var node in topo)
                {
                    var cur = dpMask[node];
                    if (!graph.TryGetValue(node, out var neigh)) continue;
                    foreach (var nb in neigh)
                    {
                        int add = 0;
                        if (string.Equals(nb, reqA, StringComparison.OrdinalIgnoreCase)) add |= 1;
                        if (string.Equals(nb, reqB, StringComparison.OrdinalIgnoreCase)) add |= 2;
                        for (int m = 0; m < 4; m++)
                            dpMask[nb][m | add] = checked(dpMask[nb][m | add] + cur[m]);
                    }
                }

                val = dpMask.ContainsKey("out") ? dpMask["out"][3] : 0;
                Console.WriteLine($"[Day11] exact count (DAG DP) = {val}");
                return;
            }

            // Fallback: bounded DFS with pruning (may still be expensive)
            Console.WriteLine("[Day11] Graph has cycles; falling back to bounded DFS with pruning.");
            var reverse = BuildReverseGraph(graph);
            var canReachOut = GetReverseReachable(reverse, "out");
            var canReachDac = GetReverseReachable(reverse, reqA);
            var canReachFft = GetReverseReachable(reverse, reqB);

            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var path = new List<string>();
            var seenReq = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            long totalMatching = 0;
            var samples = new List<List<string>>();
            const int maxSamples = 50;
            const long maxVisitedEndings = 10_000_000; // increased safety cap
            long exploredEndings = 0;
            bool capped = false;
            bool stop = false;

            void Dfs(string node)
            {
                if (stop || visited.Contains(node))
                    return;
                if (!canReachOut.Contains(node))
                    return;
                if (!seenReq.Contains(reqA) && !canReachDac.Contains(node))
                    return;
                if (!seenReq.Contains(reqB) && !canReachFft.Contains(node))
                    return;

                visited.Add(node);
                path.Add(node);

                bool added = false;
                if ((string.Equals(node, reqA, StringComparison.OrdinalIgnoreCase) || string.Equals(node, reqB, StringComparison.OrdinalIgnoreCase)) && !seenReq.Contains(node))
                {
                    seenReq.Add(node);
                    added = true;
                }

                if (string.Equals(node, "out", StringComparison.OrdinalIgnoreCase))
                {
                    exploredEndings++;
                    if (seenReq.Contains(reqA) && seenReq.Contains(reqB))
                    {
                        totalMatching++;
                        if (samples.Count < maxSamples)
                            samples.Add(new List<string>(path));
                    }

                    if (exploredEndings >= maxVisitedEndings)
                    {
                        capped = true;
                        stop = true;
                    }
                }
                else if (graph.TryGetValue(node, out var neigh))
                {
                    foreach (var nb in neigh)
                    {
                        if (stop) break;
                        if (visited.Contains(nb)) continue;
                        if (!canReachOut.Contains(nb)) continue;
                        if (!seenReq.Contains(reqA) && !canReachDac.Contains(nb)) continue;
                        if (!seenReq.Contains(reqB) && !canReachFft.Contains(nb)) continue;
                        Dfs(nb);
                    }
                }

                if (added) seenReq.Remove(node);
                path.RemoveAt(path.Count - 1);
                visited.Remove(node);
            }

            Dfs("svr");

            if (capped)
            {
                Console.WriteLine($"[Day11] Search capped after exploring {exploredEndings} endings. Matches found so far = {totalMatching}. Increase cap to continue.");
            }
            else
            {
                Console.WriteLine($"[Day11] matching paths found (sampled up to {maxSamples}) = {totalMatching}");
            }
            for (int i = 0; i < samples.Count; i++)
                Console.WriteLine($"[Day11] sample[{i}] = {string.Join("->", samples[i])}");

            val = totalMatching;
        }

        // Helpers: SCC via Kosaraju
        private static (List<List<string>> Components, Dictionary<string,int> CompId) StronglyConnectedComponents(Dictionary<string, List<string>> graph)
        {
            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var order = new List<string>();
            void Dfs1(string v)
            {
                if (visited.Contains(v)) return;
                visited.Add(v);
                if (graph.TryGetValue(v, out var neigh))
                    foreach (var nb in neigh) Dfs1(nb);
                order.Add(v);
            }
            foreach (var v in graph.Keys) Dfs1(v);

            var rev = BuildReverseGraph(graph);
            var compId = new Dictionary<string,int>(StringComparer.OrdinalIgnoreCase);
            var components = new List<List<string>>();
            for (int i = order.Count - 1; i >= 0; i--)
            {
                var v = order[i];
                if (compId.ContainsKey(v)) continue;
                var stack = new Stack<string>();
                stack.Push(v);
                compId[v] = components.Count;
                components.Add(new List<string> { v });
                while (stack.Count > 0)
                {
                    var u = stack.Pop();
                    if (rev.TryGetValue(u, out var preds))
                    {
                        foreach (var p in preds)
                        {
                            if (!compId.ContainsKey(p))
                            {
                                compId[p] = components.Count - 1;
                                components[^1].Add(p);
                                stack.Push(p);
                            }
                        }
                    }
                }
            }
            return (components, compId);
        }

        private static bool HasSelfLoop(Dictionary<string, List<string>> graph)
        {
            foreach (var kv in graph)
                if (kv.Value.Contains(kv.Key, StringComparer.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        // Topological sort (Kahn)
        private static List<string> TopologicalSort(Dictionary<string, List<string>> graph)
        {
            var indeg = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var v in graph.Keys) indeg[v] = 0;
            foreach (var kv in graph)
            {
                foreach (var nb in kv.Value)
                    indeg[nb] = indeg.GetValueOrDefault(nb) + 1;
            }
            var q = new Queue<string>(indeg.Where(kv => kv.Value == 0).Select(kv => kv.Key));
            var res = new List<string>();
            while (q.Count > 0)
            {
                var v = q.Dequeue();
                res.Add(v);
                if (!graph.TryGetValue(v, out var neigh)) continue;
                foreach (var nb in neigh)
                {
                    indeg[nb]--;
                    if (indeg[nb] == 0) q.Enqueue(nb);
                }
            }
            // include nodes with cycles at end
            if (res.Count != graph.Count)
                res.AddRange(graph.Keys.Except(res));
            return res;
        }

        // Existing helpers reused (BuildReverseGraph, GetReverseReachable, GetReachable, GetAllSimplePaths, BuildGraph, CountSimplePaths) ...

        private static Dictionary<string, List<string>> BuildReverseGraph(Dictionary<string, List<string>> graph)
        {
            var rev = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (var kv in graph)
            {
                if (!rev.ContainsKey(kv.Key)) rev[kv.Key] = new List<string>();
                foreach (var nb in kv.Value)
                {
                    if (!rev.ContainsKey(nb)) rev[nb] = new List<string>();
                    rev[nb].Add(kv.Key);
                }
            }
            return rev;
        }

        private static HashSet<string> GetReverseReachable(Dictionary<string, List<string>> reverseGraph, string target)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!reverseGraph.ContainsKey(target))
                return set;
            var q = new Queue<string>();
            q.Enqueue(target);
            set.Add(target);
            while (q.Count > 0)
            {
                var n = q.Dequeue();
                if (!reverseGraph.TryGetValue(n, out var preds)) continue;
                foreach (var p in preds)
                {
                    if (set.Add(p)) q.Enqueue(p);
                }
            }
            return set;
        }

        private static HashSet<string> GetReachable(Dictionary<string, List<string>> graph, string start)
        {
            var reachable = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!graph.ContainsKey(start))
                return reachable;
            var q = new Queue<string>();
            q.Enqueue(start);
            while (q.Count > 0)
            {
                var n = q.Dequeue();
                if (!reachable.Add(n)) continue;
                if (graph.TryGetValue(n, out var neigh))
                {
                    foreach (var nb in neigh)
                    {
                        if (!reachable.Contains(nb)) q.Enqueue(nb);
                    }
                }
            }
            return reachable;
        }

        private static IEnumerable<List<string>> GetAllSimplePaths(Dictionary<string, List<string>> graph, string start, string end, IEqualityComparer<string> comparer, int maxPaths = 100_000)
        {
            var results = new List<List<string>>();
            if (!graph.ContainsKey(start))
                return results;
            var visited = new HashSet<string>(comparer);
            var path = new List<string>();
            bool stop = false;
            void Dfs(string node)
            {
                if (stop || visited.Contains(node)) return;
                visited.Add(node);
                path.Add(node);
                if (comparer.Equals(node, end))
                {
                    results.Add(new List<string>(path));
                    if (results.Count >= maxPaths) stop = true;
                }
                else if (graph.TryGetValue(node, out var neighbors))
                {
                    foreach (var nb in neighbors)
                    {
                        if (stop) break;
                        Dfs(nb);
                    }
                }
                path.RemoveAt(path.Count - 1);
                visited.Remove(node);
            }
            Dfs(start);
            return results;
        }

        private static long CountSimplePaths(Dictionary<string, List<string>> graph, string start, string end, IEqualityComparer<string> comparer)
        {
            if (!graph.ContainsKey(start)) return 0;
            var visited = new HashSet<string>(comparer);
            long total = 0;
            void Dfs(string node)
            {
                if (comparer.Equals(node, end)) { total++; return; }
                if (!graph.TryGetValue(node, out var neighbors) || neighbors.Count == 0) return;
                visited.Add(node);
                foreach (var nb in neighbors)
                {
                    if (visited.Contains(nb)) continue;
                    Dfs(nb);
                }
                visited.Remove(node);
            }
            Dfs(start);
            return total;
        }

    }
}