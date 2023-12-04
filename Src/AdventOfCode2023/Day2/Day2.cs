using Filter = System.Collections.Generic.Dictionary<string, int>;

namespace AdventOfCode2023.Day2
{
    public class Day2(ITestOutputHelper output)
    {
        private readonly List<string> _input = File.ReadAllLines("Day2/input.txt").ToList();
        private readonly List<string> _testInput =
        [
            "Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green",
            "Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue",
            "Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red",
            "Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red",
            "Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green",
        ];

        [Fact]
        public void PartOne_Example()
        {
            var parser = new GameParser(_testInput);
            parser.Filter(new Filter { { "red", 12 }, { "green", 13 }, { "blue", 14 } });
            var answer = parser.Games.Sum(x => x.Id);
            Assert.Equal(8, answer);
            output.WriteLine($"Answer: {answer}");
        }

        [Fact]
        public void PartOne()
        {
            var parser = new GameParser(_input);
            parser.Filter(new Filter { { "red", 12 }, { "green", 13 }, { "blue", 14 } });
            var answer = parser.Games.Sum(x => x.Id);
            output.WriteLine($"Answer: {answer}");
        }

        [Fact]
        public void PartTwo_Example()
        {
            var parser = new GameParser(_testInput);
            var answer = parser.Power();
            Assert.Equal(2286, answer);
            output.WriteLine($"Answer: {answer}");
        }

        [Fact]
        public void PartTwo()
        {
            var parser = new GameParser(_input);
            var answer = parser.Power();
            output.WriteLine($"Answer: {answer}");
        }

        public class GameParser(List<string> rawLines)
        {
            public List<Game> Games { get; private set; } = [];

            public void Filter(Filter filters)
            {
                Games = [..rawLines.Select(x => new Game(x)).Where(x => x.IsValid(filters))];
            }

            public long Power()
            {
                return rawLines.Select(x => new Game(x).CalculateMinimumCubes()).Sum();
            }
        }

        public class Game
        {
            public Game(string rawLine)
            {
                var parts = rawLine.Split(":");
                Id = int.Parse(parts[0].Split(" ")[1]);
                var cubeSets = parts[1].Split(';');
                CubeSubSets = cubeSets.Select(x => x.Trim()
                    .Split(", ")
                    .Select(y => y.Split(' ')).ToArray()
                ).ToArray();
            }

            private string[][][] CubeSubSets { get; }

            public bool IsValid(Filter filters)
            {
                foreach (var cubeParts in CubeSubSets)
                {
                    foreach (var cubePart in cubeParts)
                    {
                        if (!filters.TryGetValue(cubePart[1], out var filterValue) || int.Parse(cubePart[0]) <= filterValue)
                        {
                            continue;
                        }

                        Valid = false;
                        break;
                    }
                }

                return Valid;
            }

            public long CalculateMinimumCubes()
            {
                foreach (var cubeSubSet in CubeSubSets)
                {
                    foreach (var cubeSetPart in cubeSubSet)
                    {
                        MinimumCubes[cubeSetPart[1]] = Math.Max(MinimumCubes[cubeSetPart[1]], int.Parse(cubeSetPart[0]));
                    }
                }

                return MinimumCubes.Values.Aggregate<int, long>(1, (current, value) => current * value);
            }

            public int Id { get; }
            private bool Valid { get; set; } = true;
            private Dictionary<string, int> MinimumCubes { get; } = new() { { "red", int.MinValue }, { "green", int.MinValue }, { "blue", int.MinValue } };
        }
    }
}
