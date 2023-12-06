using System.Text;
using Race = (long Time, long Record);
namespace AdventOfCode2023.Day6
{
    public class Day6(ITestOutputHelper output)
    {
        private readonly List<string> _input = File.ReadAllLines("Day6/input.txt").ToList();
        private readonly List<string> _testInput = File.ReadAllLines("Day6/test_input.txt").ToList();

        [Fact]
        public void PartOne_Example()
        {
            var parser = new RaceParser(_testInput);
            var answer = parser.Calculate();

            Assert.Equal(288, answer);
            output.WriteLine($"Answer: {answer}");
        }

        [Fact]
        public void PartOne()
        {
            var parser = new RaceParser(_input);
            var answer = parser.Calculate();

            output.WriteLine($"Answer: {answer}");
        }

        [Fact]
        public void PartTwo_Example()
        {
            var parser = new RaceParser(_testInput);
            var answer = parser.Calculate2();

            Assert.Equal(71503, answer);
            output.WriteLine($"Answer: {answer}");
        }

        [Fact]
        public void PartTwo()
        {
            var parser = new RaceParser(_input);
            var answer = parser.Calculate2();

            output.WriteLine($"Answer: {answer}");
        }
    }

    public class RaceParser
    {
        private List<Race> _races = [];
        public RaceParser(List<string> testInput)
        {
            var times = testInput[0].Split(':')[1].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var records = testInput[1].Split(':')[1].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < times.Length; i++)
            {
                _races.Add((int.Parse(times[i]), int.Parse(records[i])));
            }
        }

        public int Calculate()
        {
            var sum = 1;
            foreach (var race in _races)
            {
                sum *= CalculateBeat(race);
            }

            return sum;
        }

        public int Calculate2()
        {
            var sum = 1;
            var time = new StringBuilder();
            var record = new StringBuilder();
            foreach (var race in _races)
            {
                time.Append(race.Time);
                record.Append(race.Record);
            }

            return CalculateBeat(new Race(long.Parse(time.ToString()), long.Parse(record.ToString())));
        }

        private int CalculateBeat(Race race)
        {
            var sum = 0;
            for (var i = 0; i < race.Time; i++)
            {
                if ((race.Time - i) * i > race.Record)
                {
                    sum++;
                }
            }


            return sum;
        }
    }
}
