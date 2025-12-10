using AoC.Common;

namespace AoC.Days.Tests
{
    public class Day1Tests
    {
        private readonly List<string> inputs;

        public Day1Tests()
        {
            inputs = [
                "L68",
                "L30",
                "R48",
                "L5 ",
                "R60",
                "L55",
                "L1 ",
                "L99",
                "R14",
                "L82"
            ];
        }

        [Fact]
        public void Part1()
        {
            // Arrange
            string expected = "3";
            Day01 day = new();
            var cleanList = Sanitizer.CleanList(inputs);

            // Act
            var actual = day.Part1(cleanList);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Part2()
        {
            // Arrange
            string expected = "6";
            Day01 day = new();
            var cleanList = Sanitizer.CleanList(inputs);

            // Act
            var actual = day.Part2(cleanList);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}