using Labels = System.Collections.Generic.List<(char Key, int Count)>;

namespace AdventOfCode2023.Day7
{
    public class Day7(ITestOutputHelper output)
    {
        private readonly List<string> _input = File.ReadAllLines("Day7/input.txt").ToList();
        private readonly List<string> _testInput = File.ReadAllLines("Day7/test_input.txt").ToList();

        [Fact]
        public void PartOne_Example()
        {
            var parser = new CamelCardsParser(_testInput);
            var answer = parser.Calculate();
            Assert.Equal(6440, answer);
            output.WriteLine($"Answer: {answer}");
        }

        [Fact]
        public void PartOne()
        {
            var parser = new CamelCardsParser(_input);
            var answer = parser.Calculate();
            output.WriteLine($"Answer: {answer}");
        }

        [Fact]
        public void PartTwo_Example()
        {
            var parser = new CamelCardsParser(_testInput);
            var answer = parser.Calculate2();
            Assert.Equal(6440, answer);
            output.WriteLine($"Answer: {answer}");
        }

        [Fact]
        public void PartTwo()
        {
            var parser = new CamelCardsParser(_input);
            var answer = parser.Calculate2();
            output.WriteLine($"Answer: {answer}");
        }
    }

    public sealed class CamelCardsParser
    {
        private List<Hand> Hands { get; }

        private static readonly List<IHandType> handTypes =
        [
            new HighCard(),
            new FiveOfKind(),
            new FourOfKind(),
            new FullHouse(),
            new ThreeOfKind(),
            new TwoPair(),
            new OnePair()
        ];

        public CamelCardsParser(IEnumerable<string> rawLines)
        {
            Hands = rawLines.Select(x => x.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .Select(x => x[0].Contains('J') ?
                    new JokerHand(x[0], int.Parse(x[1])):
                    new Hand(x[0], int.Parse(x[1])))
                .ToList();
        }

        public long Calculate()
        {
            return Hands
                .Select(x => x.SetHandType(handTypes))
                .Order(new HandComparer(new string("AKQJT98765432".Reverse().ToArray())))
                .Select((x, i) => x.Bet * (i + 1))
                .Sum();
        }

        public int Calculate2()
        {
            return Hands
                .Select(x => x.SetHandType(handTypes))
                .Order(new HandComparer(new string("AKQT98765432J".Reverse().ToArray())))
                .Select((x, i) => x.Bet * (i + 1))
                .Sum();
        }
    }

    public class HandComparer(string labelOrder) : IComparer<Hand>
    {
        private readonly Dictionary<char, int> _labelToValue = labelOrder.Select((x, i) => (x, i)).ToDictionary(x => x.x, x => x.i);

        public int Compare(Hand? a, Hand? b)
        {
            if (a is null && b is null)
            {
                return 0;
            }

            if (a is null)
            {
                return -1;
            }

            if (b is null)
            {
                return 1;
            }

            if (a.HandType != b.HandType)
            {
                return a.HandType.CompareTo(b.HandType);
            }

            for (var i = 0; i < a.Cards.Length; i++)
            {
                if (a.Cards[i] != b.Cards[i])
                    return _labelToValue[a.Cards[i]].CompareTo(_labelToValue[b.Cards[i]]);
            }

            return 0;
        }
    }

    public sealed class JokerHand(string cards, int bet) : Hand(cards, bet)
    {
        public override Hand SetHandType(IEnumerable<IHandType> handTypes)
        {
            HandType = handTypes.First(y => y.IsMatch(this)).Value;
            return this;
        }
    }
    public class Hand
    {
        public int Bet { get; }
        public string Cards { get; }
        public Labels Labels { get; }
        public int HandType { get; protected set; }

        public Hand(string cards, int bet)
        {
            Bet = bet;
            Cards = cards;
            Labels = cards.GroupBy(x => x).Select(x => (x.Key, x.Count())).ToList();
        }

        public virtual Hand SetHandType(IEnumerable<IHandType> handTypes)
        {
            HandType = handTypes.First(y => y.IsMatch(this)).Value;
            return this;
        }
    }

    public class FiveOfKind : IHandType
    {
        public int Value => 7;

        public bool IsMatch(Hand h)
        {
            return h.Labels.Count == 1;
        }

        public bool IsMatch(JokerHand h)
        {
            return h.Labels.Count == 2;
        }
    }

    public class FourOfKind : IHandType
    {
        public int Value => 6;

        public bool IsMatch(Hand hand)
        {
            return hand.Labels.Count == 2 && hand.Labels.Any(x => x.Count is 4 or 1);
        }

        public bool IsMatch(JokerHand hand)
        {
            return hand.Labels.Count == 3 && hand.Labels.Any(x => x.Count >= 2);
        }
    }

    public class FullHouse : IHandType
    {
        public int Value => 5;

        public bool IsMatch(Hand hand)
        {
            return hand.Labels.Count == 2 && hand.Labels.Any(x => x.Count is 2 or 3);
        }

        public bool IsMatch(JokerHand hand)
        {
            return hand.Labels.Count == 3;
        }
    }

    public class ThreeOfKind : IHandType
    {
        public int Value => 4;
        public bool IsMatch(Hand hand)
        {
            return hand.Labels.Count == 3 && hand.Labels.Any(x => x.Count == 3);
        }

        public bool IsMatch(JokerHand hand)
        {
            return hand.Labels.Count == 4 && hand.Labels.Any(x => x.Count == 2);
        }
    }

    public class TwoPair : IHandType
    {
        public int Value => 3;

        public bool IsMatch(Hand hand)
        {
            return hand.Labels.Count == 3;
        }
        public bool IsMatch(JokerHand hand)
        {
            return hand.Labels.Count == 4;
        }
    }

    public class OnePair : IHandType
    {
        public int Value => 2;

        public bool IsMatch(Hand hand)
        {
            return hand.Labels.Count == 4;
        }

        public bool IsMatch(JokerHand hand)
        {
            return true;
        }
    }

    public class HighCard : IHandType
    {
        public int Value => 1;

        public bool IsMatch(Hand hand)
        {
            return hand.Labels.Count == 5;
        }

        public bool IsMatch(JokerHand hand)
        {
            return false;
        }
    }

    public interface IHandType
    {
        int Value { get; }
        bool IsMatch(Hand hand);
        bool IsMatch(JokerHand hand);
    }
}
