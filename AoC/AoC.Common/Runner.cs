using System.Text.RegularExpressions;

namespace AoC.Common
{
    public class Runner(string filename = "./input.txt")
    {
        private readonly string _fileName = filename;

        public void Run(Day day)
        {
            Console.WriteLine("Hello, AoC!");
            var lines = File.ReadLines(_fileName);

            var cleanList = Sanitizer.CleanList(lines);

            var part1Result = day.Part1(cleanList);
            Console.WriteLine($"Part1: {part1Result}");

            var part2Result = day.Part2(cleanList);
            Console.WriteLine($"Part2: {part2Result}");

            Console.WriteLine("Bye, AoC!");
        }
    }
}
