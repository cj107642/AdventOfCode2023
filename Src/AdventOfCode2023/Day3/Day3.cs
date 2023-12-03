using System.Text.RegularExpressions;
using Point = (int X, int Y);

namespace AdventOfCode2023.Day3
{
    public class Day3(ITestOutputHelper output)
    {
        private readonly List<string> _input = File.ReadAllLines("Day3/input.txt").ToList();
        private readonly List<string> _testInput = File.ReadAllLines("Day3/test_input.txt").ToList();

        [Fact]
        public void PartOne_Example()
        {
            var finder = new NumberFinder(_testInput);
            var answer = finder.Calculate();

            Assert.Equal(4361, answer);
            output.WriteLine($"Answer is: {answer}");
        }

        [Fact]
        public void PartOne()
        {
            var finder = new NumberFinder(_input);
            var answer = finder.Calculate();

            output.WriteLine($"Answer is: {answer}");
        }

        [Fact]
        public void PartTwo_Example()
        {
            var finder = new NumberFinder(_testInput);
            var answer = finder.Calculate2();

            Assert.Equal(467835, answer);
            output.WriteLine($"Answer is: {answer}");

        }

        [Fact]
        public void PartTwo()
        {
            var finder = new NumberFinder(_input);
            var answer = finder.Calculate2();

            output.WriteLine($"Answer is: {answer}");
        }
    }

    public partial class NumberFinder
    {
        private Dictionary<Point, Number> Numbers { get; } = [];
        private Dictionary<Point, char> SpecialChars { get; } = [];
        private int Sum { get; set; }

        private readonly List<Func<Point, Point>> _adjacentPoints =
        [
            p => new Point(p.X + 1, p.Y + 1),
            p => new Point(p.X, p.Y + 1),
            p => new Point(p.X - 1, p.Y + 1),

            p => new Point(p.X + 1, p.Y),
            p => new Point(p.X - 1, p.Y),

            p => new Point(p.X + 1, p.Y - 1),
            p => new Point(p.X, p.Y - 1),
            p => new Point(p.X - 1, p.Y - 1),
        ];

        private Regex _regex = MyRegex();
        private Regex _regex1 = MyRegex1();

        public NumberFinder(List<string> rawLines)
        {
            Build(rawLines);
        }

        public int Calculate()
        {
            foreach (var specialChar in SpecialChars)
            {
                foreach (var adjacentPoint in _adjacentPoints)
                {
                    var point = adjacentPoint(specialChar.Key);
                    if (Numbers.TryGetValue(point, out var number) && !number.Taken)
                    {
                        number.Taken = true;
                        Sum += number.Value;
                    }
                }
            }

            return Sum;
        }

        public int Calculate2()
        {
            foreach (var specialChar in SpecialChars.Where(x => x.Value == '*'))
            {
                List<int> adjacentNumbers = [];
                foreach (var adjacentPoint in _adjacentPoints)
                {
                    var point = adjacentPoint(specialChar.Key);
                    if (Numbers.TryGetValue(point, out var number) && !number.Taken)
                    {
                        number.Taken = true;
                        adjacentNumbers.Add(number.Value);
                    }
                }

                if (adjacentNumbers.Count == 2)
                {
                    Sum += adjacentNumbers.Aggregate((prev, current) => prev * current);
                }
            }

            return Sum;
        }

        public void Build(List<string> rawLines)
        {
            for (var i = 0; i < rawLines.Count; i++)
            {
                var numbers = _regex.Matches(rawLines[i]);
                foreach (Match match in numbers)
                {
                    var number = new Number { Value = int.Parse(match.Value) };
                    for (var k = 0; k < match.Length; k++)
                    {
                        Numbers.Add(new Point(match.Index + k, i), number);
                    }
                }

                var specialChars = _regex1.Matches(rawLines[i]);
                foreach (Match match in specialChars)
                {
                    SpecialChars.Add(new Point(match.Index, i), match.Value[0]);
                }
            }
        }

        [GeneratedRegex(@"\d+")]
        private static partial Regex MyRegex();
        [GeneratedRegex(@"[^\d\.]")]
        private static partial Regex MyRegex1();
    }

    public class Number
    {
        public int Value { get; init; }
        public bool Taken { get; set; }
    }
}
