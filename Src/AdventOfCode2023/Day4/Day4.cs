namespace AdventOfCode2023.Day4
{
    public class Day4(ITestOutputHelper output)
    {
        private readonly List<string> _input = File.ReadAllLines("Day4/input.txt").ToList();
        private readonly List<string> _testInput = File.ReadAllLines("Day4/test_input.txt").ToList();

        [Fact]
        public void PartOne_Example()
        {
            var parser = new CardParser(_testInput);
            var answer = parser.Calculate();

            Assert.Equal(13, answer);
            output.WriteLine($"Answer: {answer}");
        }

        [Fact]
        public void PartOne()
        {
            var parser = new CardParser(_input);
            var answer = parser.Calculate();

            output.WriteLine($"Answer: {answer}");
        }

        [Fact]
        public void PartTwo_Example()
        {
            var parser = new CardParser(_testInput);
            var answer = parser.Calculate2();

            Assert.Equal(30, answer);
            output.WriteLine($"Answer: {answer}");
        }

        [Fact]
        public void PartTwo()
        {
            var parser = new CardParser(_input);
            var answer = parser.Calculate2();

            output.WriteLine($"Answer: {answer}");
        }
    }

    public class CardParser(List<string> rawLines)
    {
        private int Sum { get; set; }
        private readonly Dictionary<int, Card> _cards = new();

        public int Calculate()
        {
            foreach (var rawLine in rawLines)
            {
                Sum += ParseCard(rawLine);
            }

            return Sum;
        }

        public int Calculate2()
        {
            ParseCards();
            foreach (var card in _cards)
            {
                TraversCardWinnings(card.Value.Winnings);
            }

            return _cards.Values.Sum(x => x.Instances);
        }

        private int ParseCard(string rawLine)
        {
            var (winning, mine) = rawLine.Split(':')[1].Split('|') switch
            {
                [var w, var m] => (w.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries), m.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries)),
                _ => (Array.Empty<string>(), Array.Empty<string>())
            };

            var sum = 0;
            foreach (var m in mine)
            {
                if (!winning.Contains(m))
                    continue;

                if (sum == 0)
                {
                    sum = 1;
                    continue;
                }

                sum *= 2;
            }

            return sum;
        }

        private void ParseCards()
        {
            foreach (var rawLine in rawLines)
            {
                var parts = rawLine.Split(':');

                var cardId = int.Parse(parts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]);
                var (winning, mine) = parts[1].Split('|') switch
                {
                    [var w, var m] => (w.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries), m.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries)),
                    _ => (Array.Empty<string>(), Array.Empty<string>())
                };

                var card = new Card(cardId);
                foreach (var m in mine)
                {
                    if (!winning.Contains(m))
                        continue;

                    card.Winnings.Add(card.Id + card.Winnings.Count + 1);
                }

                _cards.Add(card.Id, card);
            }
        }

        private void TraversCardWinnings(HashSet<int> winnings)
        {
            foreach (var win in winnings)
            {
                var card = _cards[win];
                card.Instances++;

                TraversCardWinnings(card.Winnings);
            }
        }
    }

    public class Card(int cardId)
    {
        public int Id { get; } = cardId;
        public int Instances { get; set; } = 1;
        public HashSet<int> Winnings { get; } = [];
    }
}
