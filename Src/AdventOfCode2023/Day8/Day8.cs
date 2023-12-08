using System.Collections.Concurrent;

namespace AdventOfCode2023.Day8
{
    public class Day8(ITestOutputHelper output)
    {
        private readonly List<string> _input = File.ReadAllLines("Day8/input.txt").ToList();
        private readonly List<string> _testInput = File.ReadAllLines("Day8/test_input.txt").ToList();
        private readonly List<string> _testInput2 = File.ReadAllLines("Day8/test_input2.txt").ToList();
        private readonly List<string> _testInput3 = File.ReadAllLines("Day8/test_input3.txt").ToList();

        [Fact]
        public void PartOne_Example_One()
        {
            var parser = new DirectionsParser(_testInput);
            var answer = parser.Calculate();
            Assert.Equal(2, answer);
            output.WriteLine($"Answer: {answer}");
        }
        [Fact]
        public void PartOne_Example_Two()
        {
            var parser = new DirectionsParser(_testInput2);
            var answer = parser.Calculate();
            Assert.Equal(6, answer);
            output.WriteLine($"Answer: {answer}");
        }

        [Fact]
        public void PartOne()
        {
            var parser = new DirectionsParser(_input);
            var answer = parser.Calculate();
            output.WriteLine($"Answer: {answer}");
        }

        [Fact]
        public void PartTwo_Example()
        {
            var parser = new DirectionsParser(_testInput3);
            var answer = parser.Calculate2();
            Assert.Equal(6, answer);
            output.WriteLine($"Answer: {answer}");
        }

        [Fact]
        public void PartTwo()
        {
            var parser = new DirectionsParser(_input);
            var answer = parser.Calculate2();
            output.WriteLine($"Answer: {answer}");
        }
    }

    public sealed class DirectionsParser
    {
        public DirectionsParser(IReadOnlyList<string> rawLines)
        {
            Directions = rawLines[0];
            Nodes = rawLines.Skip(2).ToDictionary(x => x[..3], x => new Dictionary<char,string>
            {
                {'L', x[7..10]},
                {'R', x[12..15]},
            });

            StartNodes = Nodes.Where(x => x.Key.EndsWith('A')).Select(x => new Node(x.Key)).ToList();
        }

        private class Node(string value)
        {
            public string Value = value;
        }

        private List<Node> StartNodes { get; }
        private Dictionary<string, Dictionary<char, string>> Nodes { get; }
        private string Directions { get; }

        public long Calculate()
        {
            var currentStep = "AAA";
            int index = 0, counter = 0;
            while (currentStep != "ZZZ")
            {
                if (index >= Directions.Length)
                {
                    index = 0;
                }

                var direction = Directions[index];
                var node = Nodes[currentStep];
                currentStep = node[direction];
                counter++;
                index++;
            }

            return counter;
        }

        public long Calculate2()
        {
            var bag = new ConcurrentBag<long>();
            Parallel.ForEach(StartNodes, node =>
            {
                var index = 0;
                var counter = 0;
                while (!node.Value.EndsWith('Z'))
                {
                    if (index >= Directions.Length)
                    {
                        index = 0;
                    }

                    node.Value = Nodes[node.Value][Directions[index]];
                    index++;
                    counter++;
                }

                bag.Add(counter);
            });

            return bag.Aggregate(1L, LeastCommonMultiple);
        }

        private static long LeastCommonMultiple(long a, long b)
        {
            var (max, min) = (Math.Max(a, b),Math.Min(a, b));
            for (long i = 1; i <= min; i++)
            {
                if (max * i % min == 0)
                {
                    return i * max;
                }
            }

            return min;
        }
    }
}
