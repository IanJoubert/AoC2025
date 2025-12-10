using AoC.Common;

namespace AoC.Days.Tests
{
    public class Day2Tests
    {
        private readonly List<string> inputs;

        public Day2Tests()
        {
            inputs = ["11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124" ];
        }

        [Fact]
        public void Part1()
        {
            // Arrange
            string expected = "1227775554";
            Day02 day = new();
            
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
            string expected = "4174379265";
            Day02 day = new();

            var cleanList = Sanitizer.CleanList(inputs);

            // Act
            var actual = day.Part2(cleanList);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}