using System.Drawing;
using AoC.Common;

namespace AoC.Days
{

    public class Day09 : Day
    {

        protected override void DoPart1(List<string> lines, out long val)
        {
            var results = new List<Tuple<Point, Point, double>>();
            var input = ParseInput(lines, ",");

            for (int i = 0; i < lines.Count - 2; i++)
            {
                for (int j = i + 1; j < lines.Count - 1; j++)
                {
                    var val2 = lines[i].Split(",");
                    var val3 = lines[j].Split(",");

                    var point1 = new Point { X = int.Parse(val2[0]), Y = int.Parse(val2[1]) };
                    var point2 = new Point { X = int.Parse(val3[0]), Y = int.Parse(val3[1]) };

                    var area = CalculateRectangleArea(point1, point2);

                    results.Add(Tuple.Create(point1, point2, area));
                }
            }

            var result = results.Select(x => x.Item3).Max();

            val = (long)result;

        }

        protected override void DoPart2(List<string> lines, out long val)
        {
            var input = ParsePoints(lines, ",");

            double bestArea = 0;

            var rowCache = new Dictionary<int, List<(int StartX, int EndX)>>();

            for (int i = 0; i < input.Count - 1; i++)
            {
                var p1 = input[i];
                for (int j = i + 1; j < input.Count; j++)
                {
                    var p2 = input[j];

                    int minX = Math.Min(p1.X, p2.X);
                    int maxX = Math.Max(p1.X, p2.X);
                    int minY = Math.Min(p1.Y, p2.Y);
                    int maxY = Math.Max(p1.Y, p2.Y);

                    var area = CalculateRectangleArea(p1, p2);

                    if (area <= bestArea)
                        continue;

                    if (RectangleFullyContainedInPolygon(minX, maxX, minY, maxY, input, rowCache))
                    {
                        bestArea = area;
                    }
                }
            }

            val = (long)bestArea;
        }

        protected virtual List<Point> ParsePoints(List<string> lines, string delimiter = " ")
        {
            var values = new List<Point>();
            foreach (var line in lines)
            {
                var items = line.Split(delimiter);

                var point = new Point
                {
                    X = int.Parse(items[0]),
                    Y = int.Parse(items[1])
                };
                values.Add(point);
            }

            return values;
        }

        public static double CalculateRectangleArea(Point p1, Point p2)
        {
            double width = Math.Abs(p1.X - p2.X) + 1;
            double height = Math.Abs(p1.Y - p2.Y) + 1;
            if (width == 0 || height == 0)
                return Math.Max(width, height);
            return width * height;
        }

        private static bool RectangleFullyContainedInPolygon(int minX, int maxX, int minY, int maxY, List<Point> polygon, Dictionary<int, List<(int StartX, int EndX)>> rowCache)
        {
            for (int y = minY; y <= maxY; y++)
            {
                if (!rowCache.TryGetValue(y, out var intervals))
                {
                    intervals = BuildIntervalsForRow(y, polygon);
                    rowCache[y] = intervals;
                }

                if (intervals == null || intervals.Count == 0)
                    return false;

                bool covered = false;
                foreach (var it in intervals)
                {
                    if (it.StartX <= minX && it.EndX >= maxX)
                    {
                        covered = true;
                        break;
                    }
                }

                if (!covered)
                    return false;
            }

            return true;
        }

        private static List<(int StartX, int EndX)> BuildIntervalsForRow(int y, List<Point> polygon)
        {
            double sampleY = y + 0.5;
            var intersections = new List<double>();

            var horizontalIntervals = new List<(int StartX, int EndX)>();

            for (int e = 0; e < polygon.Count; e++)
            {
                var a = polygon[e];
                var b = polygon[(e + 1) % polygon.Count];

                if (a.Y == b.Y && a.Y == y)
                {
                    int startX = Math.Min(a.X, b.X);
                    int endX = Math.Max(a.X, b.X);
                    horizontalIntervals.Add((startX, endX));
                    continue;
                }

                if (a.Y == b.Y)
                    continue;

                double minY = Math.Min(a.Y, b.Y);
                double maxY = Math.Max(a.Y, b.Y);

                if (!(minY <= sampleY && sampleY < maxY))
                    continue;

                if (a.X == b.X)
                {
                    intersections.Add(a.X);
                }
                else
                {
                    double x = a.X + (sampleY - a.Y) * (b.X - a.X) / (b.Y - a.Y);
                    intersections.Add(x);
                }
            }

            intersections.Sort();

            var intervals = new List<(int StartX, int EndX)>();
            for (int k = 0; k + 1 < intersections.Count; k += 2)
            {
                double xLeft = intersections[k];
                double xRight = intersections[k + 1];

                int startX = (int)Math.Ceiling(xLeft - 0.5);
                int endX = (int)Math.Floor(xRight - 0.5);

                if (startX <= endX)
                    intervals.Add((startX, endX));
            }

            if (horizontalIntervals.Count > 0)
            {
                intervals.AddRange(horizontalIntervals);
            }

            if (intervals.Count <= 1)
                return intervals.OrderBy(i => i.StartX).ToList();

            intervals = intervals.OrderBy(i => i.StartX).ToList();
            var merged = new List<(int StartX, int EndX)>();
            foreach (var it in intervals)
            {
                if (merged.Count == 0 || it.StartX > merged.Last().EndX + 1)
                    merged.Add(it);
                else
                    merged[merged.Count - 1] = (merged.Last().StartX, Math.Max(merged.Last().EndX, it.EndX));
            }

            return merged;
        }
    }

}