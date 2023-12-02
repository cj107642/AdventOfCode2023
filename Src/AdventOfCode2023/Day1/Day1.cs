namespace AdventOfCode2023.Day1
{
    public class Day1(ITestOutputHelper output)
    {
        private readonly List<string> _input = File.ReadAllLines("Day1/input.txt").ToList();
        private readonly List<string> _testInput = [
                "1abc2",
                "pqr3stu8vwx",
                "a1b2c3d4e5f",
                "treb7uchet",
            ];
        private readonly List<string> _testInput2 = [
                "two1nine",
                "eightwothree",
                "abcone2threexyz",
                "xtwone3four",
                "4nineeightseven2",
                "zoneight234",
                "7pqrstsixteen"];

        [Fact]
        public void PartOne_Example()
        {
            var finder = new NumberFinder(_testInput);
            var sum = finder.Calculate();

            Assert.Equal(142, sum);

            output.WriteLine($"Answer: {sum}");
        }

        [Fact]
        public void ParOne()
        {
            var finder = new NumberFinder(_input);
            var sum = finder.Calculate();

            output.WriteLine($"Answer: {sum}");
        }

        [Fact]
        public void PartTwo_Example()
        {
            var finder = new NumberFinder(_testInput2);
            var sum = finder.Calculate2();

            Assert.Equal(281, sum);
            output.WriteLine($"Answer: {sum}");
        }

        [Fact]
        public void PartTwo()
        {
            var finder = new NumberFinder(_input);
            var sum = finder.Calculate2();

            output.WriteLine($"Answer: {sum}");
        }
    }

    public class NumberFinder
    {
        private readonly List<string> _input;
        private readonly Dictionary<string, char> _numberAsWords;
        private readonly SearchValues<char> _numbers;
        private readonly Dictionary<string, char> _numberAsWordsWithReversWords;

        public NumberFinder(List<string> input)
        {
            _input = input;
            _numbers = SearchValues.Create(new[] { '1', '2', '2', '3', '4', '5', '6', '7', '8', '9' });
            _numberAsWords = new Dictionary<string, char>
            {
                { "one",'1' },
                { "two",'2' },
                { "three",'3' },
                { "four",'4' },
                { "five",'5' },
                { "six",'6' },
                { "seven",'7' },
                { "eight",'8' },
                { "nine",'9' },
            };

            _numberAsWordsWithReversWords = _numberAsWords.ToDictionary(x => new string(x.Key.Reverse().ToArray()), x => x.Value);
        }

        public int Sum { get; set; }
        public int Calculate()
        {
            foreach (var item in _input)
            {
                var first = GetIntChar(item);
                var second = GetIntChar(item.Reverse().ToArray());
                Sum += int.Parse("" + first + second);
            }

            return Sum;
        }

        public int Calculate2()
        {
            foreach (var item in _input)
            {
                var first = GetNumberWordOrInt(item, _numberAsWords);
                var second = GetNumberWordOrInt(item.Reverse().ToArray(), _numberAsWordsWithReversWords);
                Sum += int.Parse("" + first + second);
            }

            return Sum;
        }

        private char? GetIntChar(ReadOnlySpan<char> rawLine)
        {
            foreach (var c in rawLine)
            {
                if (_numbers.Contains(c))
                {
                    return c;
                }

            }

            return null;
        }

        private char? GetNumberWordOrInt(ReadOnlySpan<char> rawLine, Dictionary<string, char> words)
        {
            for (var i = 0; i < rawLine.Length; i++)
            {
                if (_numbers.Contains(rawLine[i]))
                {
                    return rawLine[i];
                }

                if (TryGetNumberFromWord(rawLine, i, words, out var intChar))
                {
                    return intChar;
                }
            }

            return null;
        }

        private bool TryGetNumberFromWord(ReadOnlySpan<char> rawLine, int i, Dictionary<string, char> words, out char? intChar)
        {
            intChar = null;
            foreach ((ReadOnlySpan<char> word, var numberAsChar) in words)
            {
                if (word.Length + i >= rawLine.Length)
                    continue;

                var found = true;
                for (var j = 0; j < word.Length; j++)
                {
                    if (rawLine[i + j] != word[j])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    intChar = numberAsChar;
                    return true;
                }
            }

            return false;
        }
    }
}
