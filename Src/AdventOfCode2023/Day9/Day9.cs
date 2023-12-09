
namespace AdventOfCode2023.Day9
{
    public class Day9(ITestOutputHelper output)
    {
        private readonly List<string> _input = File.ReadAllLines("Day9/input.txt").ToList();
        private readonly List<string> _testInput = File.ReadAllLines("Day9/test_input.txt").ToList();

        [Fact]
        public void PartOne_Example()
        {
            var answer = _testInput.Select(x => x.Split(" ").Select(int.Parse).ToList())
                .Select(TraverseHistory).Sum();

            Assert.Equal(114,answer);
            output.WriteLine($"Answer:{answer}");
        }
        [Fact]
        public void PartOne()
        {
            var answer = _input.Select(x => x.Split(" ").Select(int.Parse).ToList())
                .Select(TraverseHistory)
                .Sum();

            output.WriteLine($"Answer:{answer}");
        }

        [Fact]
        public void PartTwo_Example()
        {
            var answer = _testInput.Select(x => x.Split(" ").Select(int.Parse).ToList()).ToList()
                .Select(TraverseHistory2).ToList().Sum();

            Assert.Equal(2,answer);
            output.WriteLine($"Answer:{answer}");
        }

        [Fact]
        public void PartTwo()
        {
            var answer = _input.Select(x => x.Split(" ").Select(int.Parse).ToList()).ToList()
                .Select(TraverseHistory2).ToList().Sum();

            output.WriteLine($"Answer:{answer}");
        }

        private static long TraverseHistory(List<int> history)
        {
            if (history.All(x => x == 0))
                return 0;
            List<int> newList = [];
            for (var i = 0; i < history.Count-1; i++)
            {
                newList.Add(history[i+1]-history[i]);
            }

            return history.Last() + TraverseHistory(newList);
        }
        private static long TraverseHistory2(List<int> history)
        {
            if (history.All(x => x == 0))
                return 0;
            List<int> newList = [];
            for (var i = 0; i < history.Count-1; i++)
            {
                newList.Add(history[i+1]-history[i]);
            }

            return history[0] - TraverseHistory2(newList);
        }
    }
}
