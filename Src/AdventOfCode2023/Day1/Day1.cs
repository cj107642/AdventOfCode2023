using System.Buffers;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace AdventOfCode2023.Day1
{
    public class Day1(ITestOutputHelper _output)
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
            var finder = new NubmerFinder(_testInput);
            var sum = finder.Calculate();

            Assert.Equal(142, sum);

            _output.WriteLine($"Answer: {sum}");
        }

        [Fact]
        public void ParOne()
        {
            var finder = new NubmerFinder(_input);
            var sum = finder.Calculate();

            _output.WriteLine($"Answer: {sum}");
        }

        [Fact]
        public void PartTwo_Example()
        {
            var finder = new NubmerFinder(_testInput2);
            var sum = finder.Calculate2();

            Assert.Equal(281, sum);
            _output.WriteLine($"Answer: {sum}");
        }

        [Fact]
        public void PartTwo()
        {
            var finder = new NubmerFinder(_input);
            var sum = finder.Calculate2();

            _output.WriteLine($"Answer: {sum}");
        }
    }

    public class NubmerFinder
    {
        private readonly List<string> _input;
        private readonly Dictionary<string, char> numberAsWords;
        private readonly SearchValues<char> numbers;
        private readonly Dictionary<string, char> numberAsWordsWithReversWords;

        public NubmerFinder(List<string> input)
        {
            _input = input;
            numbers = SearchValues.Create(new char[] { '1', '2', '2', '3', '4', '5', '6', '7', '8', '9' });
            numberAsWords = new(){
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
        
            numberAsWordsWithReversWords = numberAsWords.ToDictionary(x => new string(x.Key.Reverse().ToArray()), x => x.Value);
        }

        public int Sum { get; set; }
        public int Calculate()
        {
            foreach (var item in _input)
            {
                TryGetIntChar(item, out var first);
                TryGetIntChar(item.Reverse().ToArray(), out var second);
                Sum += int.Parse("" + first + second);
            }

            return Sum;
        }

        public int Calculate2()
        {
            foreach (var item in _input)
            {
                TryGetNumberWordOrInt(item, numberAsWords, out var first);
                TryGetNumberWordOrInt(item.Reverse().ToArray(), numberAsWordsWithReversWords, out var second);
                Sum += int.Parse("" + first + second);
            }

            return Sum;
        }

        private bool TryGetIntChar(ReadOnlySpan<char> rawLine, out char? intChar)
        {
            intChar = null;
            foreach (var c in rawLine)
            {
                if (numbers.Contains(c))
                {
                    intChar = c;
                    return true;
                }

            }

            return false;
        }

        private bool TryGetNumberWordOrInt(ReadOnlySpan<char> rawLine, Dictionary<string, char> words, out char? intChar)
        {
            intChar = null;
            for (var i = 0; i < rawLine.Length; i++)
            {
                if (numbers.Contains(rawLine[i]))
                {
                    intChar = rawLine[i];
                    return true;
                }

                if (TryGetNumberFromWord(rawLine, i, words, out intChar))
                {
                    return true;
                }
            }

            return false;
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
