using System.Text.RegularExpressions;

namespace AdventOfCode2023.Day5
{
    public class Day5(ITestOutputHelper output)
    {
        private readonly List<string> _input = File.ReadAllLines("Day5/input.txt").ToList();
        private readonly List<string> _testInput = File.ReadAllLines("Day5/test_input.txt").ToList();

        [Fact]
        public void PartOne_Example()
        {
            var parser = new AlmanacParser(_testInput);
            var answer = parser.Calculate();

            Assert.Equal(35, answer);
            output.WriteLine($"Answer: {answer}");
        }

        [Fact]
        public void PartOne()
        {
            var parser = new AlmanacParser(_input);
            var answer = parser.Calculate();

            output.WriteLine($"Answer: {answer}");
        }

        [Fact]
        public void PartTwo_Example()
        {
            var parser = new AlmanacParser(_testInput);
            var answer = parser.Calculate2();

            Assert.Equal(46, answer);
            output.WriteLine($"Answer: {answer}");
        }

        [Fact]
        public void PartTwo()
        {
            var parser = new AlmanacParser(_input);
            var answer = parser.Calculate2();

            output.WriteLine($"Answer: {answer}");
        }
    }

    internal partial class AlmanacParser
    {
        private readonly IReadOnlyList<string> _rawLines;
        private readonly Regex _mapRegex = MyRegex();
        private List<Seed> Seeds { get; }
        private List<SeedWithRange> SeedsWithRange { get; } = [];
        private List<Map> Maps { get; } = [];
        public AlmanacParser(IReadOnlyList<string> rawLines)
        {
            _rawLines = rawLines;
            var seedInput = rawLines[0].Split(':', StringSplitOptions.RemoveEmptyEntries)[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
            Seeds = seedInput.Select(x => new Seed { Value = x }).ToList();
            for (var i = 0; i < seedInput.Count; i += 2)
            {
                SeedsWithRange.Add(new SeedWithRange(seedInput[i], seedInput[i] + seedInput[i + 1]));
            }

            BuildMaps();
        }
        private void BuildMaps()
        {
            var queue = new Queue<string>(_rawLines);
            while (queue.TryDequeue(out var rawLine))
            {
                var match = _mapRegex.Match(rawLine);
                if (match.Length < 1)
                {
                    continue;
                }

                List<Mapper> mappers = [];
                while (queue.TryDequeue(out var mapper) && !string.IsNullOrWhiteSpace(mapper))
                {
                    var (destinationStart, sourceStart, length) = mapper.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray() switch
                    {
                        [var d, var s, var l] => (d, s, l),
                        _ => (0,0,0)
                    };

                    mappers.Add(new Mapper(sourceStart, destinationStart, length));
                }

                var map = new Map($"{match.Groups[1].Value}", mappers);

                Maps.Add(map);
            }
        }
        public long Calculate()
        {
            foreach (var seed in Seeds)
            {
                foreach (var map in Maps)
                {
                    foreach (var mapper in map.Mappers)
                    {
                        if (mapper.InRange(seed.Value))
                        {
                            seed.Value = mapper.Map(seed.Value);
                            break;
                        }
                    }
                }
            }

            return Seeds.Min(x => x.Value);
        }
        public long Calculate2()
        {
            var seeds = new List<SeedWithRange>(SeedsWithRange);
            foreach (var map in Maps)
            {
                List<SeedWithRange> newSeeds = [];
                seeds.ForEach(x => x.Mapped = false);
                foreach (var mapper in map.Mappers)
                {
                    foreach (var seed in seeds)
                    {
                        if (seed.Mapped)
                        {
                            newSeeds.Add(seed);
                            continue;
                        }

                        newSeeds.AddRange(mapper.MapSeed(seed));
                    }

                    seeds = [..newSeeds];
                    newSeeds.Clear();
                }
            }

            return seeds.Min(x => x.Start);
        }

        [GeneratedRegex("(.*-to-.*) ")]
        private static partial Regex MyRegex();
    }
    internal sealed class Map(string name, IEnumerable<Mapper> mappers)
    {
        public string Name { get; } = name;
        public readonly List<Mapper> Mappers = mappers.OrderBy(x => x.SourceStart).ToList();
    }
    internal sealed class Mapper(long sourceStart, long destinationStart, long length)
    {
        public readonly long SourceStart = sourceStart;
        public readonly long SourceEnd = sourceStart + length - 1;
        public readonly long DestinationStart = destinationStart;
        public readonly long DestinationEnd = destinationStart + length - 1;

        public List<SeedWithRange> MapSeed(SeedWithRange seed)
        {
            List<SeedWithRange> newSeeds = [];
            if (seed.End < SourceStart || seed.Start > SourceEnd)
            {
                newSeeds.Add(seed);
                return newSeeds;
            }

            if (seed.Start < SourceStart && seed.End > SourceStart)
            {
                newSeeds.Add(new SeedWithRange(seed.Start, SourceStart - 1));
                if (seed.End > SourceEnd)
                {
                    newSeeds.Add(new SeedWithRange(SourceEnd + 1, seed.End));
                    newSeeds.Add(new SeedWithRange(DestinationStart, DestinationEnd, true));
                }
                else
                {
                    newSeeds.Add(new SeedWithRange(DestinationStart, DestinationStart + Math.Abs(SourceStart - seed.End), true));
                }
                return newSeeds;
            }

            if (seed.Start >= SourceStart && seed.End <= SourceEnd)
            {
                newSeeds.Add(new SeedWithRange(DestinationStart + Math.Abs(seed.Start - SourceStart), DestinationEnd - Math.Abs(seed.End - SourceEnd), true));
                return newSeeds;
            }

            if (seed.Start >= SourceStart && seed.End > SourceEnd)
            {
                newSeeds.Add(new SeedWithRange(SourceEnd + 1, seed.End));
                newSeeds.Add(new SeedWithRange(DestinationStart + Math.Abs(seed.Start - SourceStart), DestinationEnd, true));
            }

            return newSeeds;
        }

        internal bool InRange(long index)
        {
            return index >= SourceStart && index < SourceEnd;
        }

        internal long Map(long index)
        {
            return DestinationStart + index - SourceStart;
        }
    }
    internal sealed class Seed
    {
        public long Value { get; set; }
    }
    internal sealed class SeedWithRange(long start, long end, bool mapped = false)
    {
        public readonly long Start = start;
        public readonly long End = end - 1;
        public bool Mapped { get; set; } = mapped;
    }
}
