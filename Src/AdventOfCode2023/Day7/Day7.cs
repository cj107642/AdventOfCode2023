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
            Assert.Equal(5905, answer);
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

    public class CamelCardsParser
    {
        public List<Hand> Hands { get; set; }

        public static Dictionary<char, int> CardTypeToInt = new()
        {
            { 'A', 5 },
            { 'K', 4 },
            { 'Q', 3 },
            { 'J', 2 },
            { 'T', 1 },
        };

        public CamelCardsParser(List<string> rawLines)
        {
            Hands = rawLines.Select(x =>
            {
                var parts = x.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return Hand.Create(parts[0].ToList(), int.Parse(parts[1]));
            }).ToList();

            HandsWithJoker = rawLines.Select(x =>
            {
                var parts = x.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return Hand.Create2(parts[0].ToList(), int.Parse(parts[1]));
            }).ToList();
        }

        public List<Hand> HandsWithJoker { get; set; }

        public long Calculate()
        {
            Hands.Sort(SortByHand);

            long sum = 0;
            for (var i = 0; i < Hands.Count; i++)
            {
                sum += Hands[i].Bet * (i + 1);
            }

            return sum;
        }

        public static int SortByHand(Hand a, Hand b)
        {
            if (a.HandType != b.HandType)
            {
                return a.HandType - b.HandType;
            }

            for (var i = 0; i < a.UnsortedCards.Length; i++)
            {
                if (a.UnsortedCards[i] == b.UnsortedCards[i])
                {
                    continue;
                }


                if (char.IsDigit(a.UnsortedCards[i]) && !char.IsDigit(b.UnsortedCards[i]))
                {
                    return -1;
                }

                if (!char.IsDigit(a.UnsortedCards[i]) && char.IsDigit(b.UnsortedCards[i]))
                {
                    return 1;
                }

                if (char.IsDigit(a.UnsortedCards[i]) && char.IsDigit(b.UnsortedCards[i]))
                {
                    return a.UnsortedCards[i] - b.UnsortedCards[i];
                }

                if (!char.IsDigit(a.UnsortedCards[i]) && !char.IsDigit(b.UnsortedCards[i]))
                {
                    return CardTypeToInt[a.UnsortedCards[i]] - CardTypeToInt[b.UnsortedCards[i]];
                }
            }

            return 0;
        }

        public static int SortByHand2(Hand a, Hand b)
        {
            if (a.HandType != b.HandType)
            {
                return a.HandType - b.HandType;
            }

            for (var i = 0; i < a.UnsortedCards.Length; i++)
            {
                if (a.UnsortedCards[i] == b.UnsortedCards[i])
                {
                    continue;
                }

                if (a.UnsortedCards[i] == 'J')
                {
                    return -1;
                }

                if (b.UnsortedCards[i] == 'J')
                {
                    return 1;
                }


                if (char.IsDigit(a.UnsortedCards[i]) && !char.IsDigit(b.UnsortedCards[i]))
                {
                    return -1;
                }

                if (!char.IsDigit(a.UnsortedCards[i]) && char.IsDigit(b.UnsortedCards[i]))
                {
                    return 1;
                }

                if (char.IsDigit(a.UnsortedCards[i]) && char.IsDigit(b.UnsortedCards[i]))
                {
                    return a.UnsortedCards[i] - b.UnsortedCards[i];
                }

                if (!char.IsDigit(a.UnsortedCards[i]) && !char.IsDigit(b.UnsortedCards[i]))
                {
                    return CardTypeToInt[a.UnsortedCards[i]] - CardTypeToInt[b.UnsortedCards[i]];
                }
            }

            return 0;
        }

        public long Calculate2()
        {
            HandsWithJoker.Sort(SortByHand2);

            long sum = 0;
            for (var i = 0; i < HandsWithJoker.Count; i++)
            {
                sum += HandsWithJoker[i].Bet * (i + 1);
            }

            return sum;
        }
    }

    public class Hand
    {
        public int Bet { get; }

        private readonly Dictionary<string, Func<string, int>> PossibleHands = new()
        {
            { "HighCard", HighCard },
            { "FiveOfKind", FiveOfKind },
            { "FourOfKind", FourOfKind },
            { "FullHouse", FullHouse },
            { "ThreeOfKind", ThreeOfKind },
            { "TwoPair", TwoPair },
            { "Pair", Pair },
        };

        private readonly Dictionary<string, Func<string, int>> PossibleHandsWithJoker = new()
        {
            { "HighCardWithJoker", HighCardWithJoker },
            { "FiveOfKindWithJoker", FiveOfKindWithJoker },
            { "FourOfKindWithJoker", FourOfKindWithJoker },
            { "FullHouseWithJoker", FullHouseWithJoker },
            { "ThreeOfKindWithJoker", ThreeOfKindWithJoker },
            { "TwoPairWithJoker", TwoPairWithJoker },
            { "PairWithJoker", PairWithJoker },
        };

        public static int FiveOfKind(string c)
        {
            return c.All(x => c.First() == x) ? 7 : 0;
        }

        public static int FourOfKind(string c)
        {
            if (c[..4].All(x => x == c.First()))
            {
                return 6;
            }

            if (c[1..].All(x => x == c.Last()))
            {
                return 6;
            }

            return 0;
        }

        public static int FullHouse(string c)
        {
            if (c[..3].All(x => c.First() == x) && c[3..].All(x => c.Last() == x))
            {
                return 5;
            }

            if (c[..2].All(x => c.First() == x) && c[2..].All(x => c.Last() == x))
            {
                return 5;
            }

            return 0;
        }

        public static int ThreeOfKind(string hand)
        {
            if (hand.Any(x => hand.Count(y => y == x) == 3))
            {
                return 4;
            }

            return 0;
        }

        public static int TwoPair(string hand)
        {
            var pairs = 0;
            char? pairChar = null;
            foreach (var card in hand)
            {
                if (pairChar == card)
                {
                    continue;
                }

                if (hand.Count(x => x == card) == 2)
                {
                    pairChar = card;
                    pairs++;
                    if (pairs == 2)
                    {
                        return 3;
                    }
                }
            }

            return 0;
        }

        public static int Pair(string hand)
        {
            if (hand.Any(x => hand.Count(y => y == x) == 2))
            {
                return 2;
            }

            return 0;
        }

        public static int HighCard(string hand)
        {
            var handWithoutJokers = hand.Replace("j", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            var jokers = hand.Length - handWithoutJokers.Length;

            if (jokers > 0)
            {
                return 0;
            }

            if (hand.Distinct().Count() == hand.Length)
            {
                return 1;
            }

            return 0;
        }

        public static int FiveOfKindWithJoker(string hand)
        {
            var handWithoutJokers = hand.Replace("j", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            var jokers = hand.Length - handWithoutJokers.Length;
            if (jokers == 5)
                return 7;

            return handWithoutJokers.Distinct().Count() == 1 ? 7 : 0;
        }

        public static int FourOfKindWithJoker(string hand)
        {
            var handWithoutJokers = hand.Replace("j", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            var jokers = hand.Length - handWithoutJokers.Length;

            var d = CountOccurenceOfChar(handWithoutJokers);

            return d.Any(x => x.Value + jokers > 3) ? 6 : 0;
        }

        public static int FullHouseWithJoker(string hand)
        {
            var handWithoutJokers = hand.Replace("j", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            var jokers = hand.Length - handWithoutJokers.Length;

            if (handWithoutJokers.Distinct().Count() == 2 && jokers > 0)
            {
                return 5;
            }

            var d = CountOccurenceOfChar(handWithoutJokers);

            return !d.Any(x => x.Value > 3) && handWithoutJokers.Distinct().Count() == 2 ? 5 : 0;
        }

        public static Dictionary<char, int> CountOccurenceOfChar(string text)
        {
            var d = new Dictionary<char, int>();
            foreach (var card in text)
            {
                if (d.TryGetValue(card, out var value))
                {
                    d[card] = ++value;
                }
                else
                {
                    d.Add(card, 1);
                }
            }

            return d;
        }

        public static int ThreeOfKindWithJoker(string hand)
        {
            var handWithoutJokers = hand.Replace("j", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            var jokers = hand.Length - handWithoutJokers.Length;

            var d = CountOccurenceOfChar(handWithoutJokers);
            return d.Any(x => x.Value + jokers > 2) ? 4 : 0;
        }

        public static int TwoPairWithJoker(string hand)
        {
            var handWithoutJokers = hand.Replace("j", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            var jokers = hand.Length - handWithoutJokers.Length;
            if (jokers == 2 && handWithoutJokers.Distinct().Count() > 1)
            {
                return 3;
            }

            var pairs = 0;
            foreach (var kvp in CountOccurenceOfChar(handWithoutJokers))
            {
                if (kvp.Value >= 2)
                {
                    pairs++;
                }else if (kvp.Value + jokers >= 2)
                {
                    pairs++;
                    jokers--;
                }

                if (pairs >= 2)
                {
                    return 3;
                }
            }

            return 0;
        }

        public static int PairWithJoker(string hand)
        {
            var handWithoutJokers = hand.Replace("j", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            var jokers = hand.Length - handWithoutJokers.Length;

            return CountOccurenceOfChar(handWithoutJokers).Any( x => x.Value + jokers >= 2) ? 2 : 0;
        }

        public static int HighCardWithJoker(string hand)
        {
            var handWithoutJokers = hand.Replace("j", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            if (handWithoutJokers.Distinct().Count() == hand.Length)
            {
                return 1;
            }

            return 0;
        }

        public Hand(List<char> cards, int bet)
        {
            Bet = bet;
            UnsortedCards = string.Join(string.Empty, cards);
            cards.Sort(SortByChardThenNumber);
            Cards = string.Join(string.Empty, cards);
        }

        public static Hand Create(List<char> cards, int bet)
        {
            var hand = new Hand(cards, bet);
            hand.GetHand();
            return hand;
        }

        public static Hand Create2(List<char> cards, int bet)
        {
            var hand = new Hand(cards, bet);
            hand.GetHand2();
            return hand;
        }

        public void GetHand()
        {
            foreach (var possibleHand in PossibleHands)
            {
                var handType = possibleHand.Value(Cards);
                if (handType > 0)
                {
                    HandType = handType;
                    HandName = possibleHand.Key;
                    break;
                }
            }
        }

        public void GetHand2()
        {
            foreach (var possibleHand in PossibleHandsWithJoker)
            {
                var handType = possibleHand.Value(Cards);
                if (handType > 0)
                {
                    HandType = handType;
                    HandName = possibleHand.Key;
                    break;
                }
            }
        }

        public string Cards { get; set; }
        public string UnsortedCards { get; set; }
        public string HandName { get; set; }
        public int HandType { get; set; }

        private int SortByChardThenNumber(char a, char b)
        {
            if (a == b)
            {
                return 0;
            }

            if (a == 'A')
            {
                return -1;
            }

            if (!char.IsDigit(a) && char.IsDigit(b))
            {
                return -1;
            }

            if (char.IsDigit(a) && !char.IsDigit(b))
            {
                return 1;
            }

            if (char.IsDigit(a) && char.IsDigit(b))
            {
                return b - a;
            }

            if (!char.IsDigit(a) && !char.IsDigit(b))
            {
                return b - a;
            }

            return 0;
        }
    }
}
