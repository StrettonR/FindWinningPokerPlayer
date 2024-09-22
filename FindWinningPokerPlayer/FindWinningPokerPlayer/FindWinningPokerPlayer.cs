using Microsoft.Extensions.Configuration;

namespace FindWinningPokerPlayer {
    class Program {
        static void Main() {
            Console.WriteLine("Find player with most wins: \n");
            var game = new FindWinningPokerPlayer("FilePath");
            var txt = game.ImportTxt();
            var player1Hands = new Hand(1);
            var player2Hands = new Hand(2);
            while (txt != null) {
                var cardList = game.TxtToHand(txt, 0);
                var player1Hand = AsignRankSetup(cardList, 1);
                var player2Hand = AsignRankSetup(cardList, 2);
                var winner = game.CompareHands(player1Hand, player2Hand);
                FindWinningPokerPlayer.PlayerWinsRunningTotal(player1Hands, player2Hands, winner);
                if (txt.Length >= 31) {
                    txt = txt.Remove(0, 31);
                } else {
                    break;
                }
            }
            var playerWithMostWins = FindWinningPokerPlayer.FindPlayerWithMostWins(player1Hands, player2Hands);
            Console.WriteLine("Player " + player1Hands.Player.Id + " wins " + player1Hands.Player.wins + " times.");
            Console.WriteLine("Player " + player2Hands.Player.Id + " wins " + player2Hands.Player.wins + " times.");
            Console.WriteLine("\nplayer " + playerWithMostWins + " has the most wins");
        }

        private static Hand AsignRankSetup(List<string> cardList, int p) {
            var game = new FindWinningPokerPlayer();
            var hand = new Hand(p);
            game.AsignHandToPlayer(hand, cardList);
            hand.AsignRankToHand();
            return hand;
        }
    }

    public class FindWinningPokerPlayer {
        private string _filePath;
        public string FilePath => _filePath;

        public FindWinningPokerPlayer() : this("FilePath") {

        }

        public FindWinningPokerPlayer(string configKey) {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var filePath = configuration[configKey];
            if (filePath != null) {
                _filePath = filePath;
            } else {
                Console.WriteLine("Edit appsettings.json to direct to hands.txt");
                Console.WriteLine($"Edit appsettings.json to include the key '{configKey}' with the correct file path.");
                Environment.Exit(1);
            }
        }

        public static int FindPlayerWithMostWins(Hand player1Hands, Hand player2Hands) {
            if (player1Hands != null && player2Hands != null) {
                if (player1Hands.Player.wins > player2Hands.Player.wins) {
                    return 1;
                } else if (player1Hands.Player.wins < player2Hands.Player.wins) {
                    return 2;
                } else {
                    return 0;
                }
            }
            return 0;
        }

        public static void PlayerWinsRunningTotal(Hand player1Hands, Hand player2Hands, int winner) {
            if (player1Hands != null && player2Hands != null && winner != 0) {
                if (winner == 1) {
                    player1Hands.Player.wins++;
                } else if (winner == 2) {
                    player2Hands.Player.wins++;
                }
            }
        }

        public string ImportTxt() {
            string filePath = this.FilePath;
            var content = "";
            if (File.Exists(filePath)) {
                content = ReadFile(filePath);
            }
            return content;
        }

        private string ReadFile(string filePath) {
            string content = "";
            try {
                using (StreamReader sr = new StreamReader(filePath)) {
                    content = sr.ReadToEnd();
                }
            } catch (Exception e) {
                Console.WriteLine("Error reading the file: " + e.Message);
            }
            return content;
        }

        public List<string> TxtToHand(string txt, int handNumber) {
            if (!String.IsNullOrEmpty(txt)) {
                var cardsList = new List<string>();
                var line = txt.Split("\r\n")[handNumber];
                var cardsAsStringArray = line.Split(' ').ToArray();
                foreach (var card in cardsAsStringArray) {
                    cardsList.Add(card);
                }
                return cardsList;
            }
            return new List<string>();
        }

        public void AsignHandToPlayer(Hand hand, List<string> cardList) {
            if (hand == null || hand.Player.Id < 1 || hand.Player.Id > 2 || cardList == null || cardList.Count == 0 || cardList.Count == 1) {
                return;
            }
            var a = 0;
            var b = 5;
            if (hand.Player.Id == 2) {
                a = 5;
                b = 10;
            }
            for (var i = a; i < b; i++) {
                hand.AddCard(cardList[i]);
            }
        }

        internal int CompareHands(Hand player1Hand, Hand player2Hand) {
            var winner = 0;
            if (player1Hand == null || player2Hand == null || player1Hand.Rank == 0 || player2Hand.Rank == 0) {
                return winner;
            }
            if (player1Hand.Rank > player2Hand.Rank) {
                winner = 1;
            } else if (player1Hand.Rank < player2Hand.Rank) {
                winner = 2;
            } else {
                winner = this.TiedHand(player1Hand, player2Hand);
            }
            return winner;
        }

        internal int TiedHand(Hand player1Hand, Hand player2Hand) {
            switch (player1Hand.Rank) {
            //high card
            case 1:
                return TiedHighCardHand(player1Hand, player2Hand);
            //one pair
            case 2:
                var player1Pair = Hand.FindPairs(player1Hand.Cards, 2)[0];
                var player2Pair = Hand.FindPairs(player2Hand.Cards, 2)[0];
                if (player1Pair > player2Pair) {
                    return 1;
                } else if (player1Pair < player2Pair) {
                    return 2;
                } else {
                    return TiedHighCardHand(player1Hand, player2Hand);
                }
            //two pair
            case 3:
                var player1Pairs = Hand.FindPairs(player1Hand.Cards, 2);
                var player2Pairs = Hand.FindPairs(player2Hand.Cards, 2);
                if (player1Pairs != null && player1Pairs.Count() != 0) {
                    for (var i = player1Pairs.Count() - 1; i >= 0; i--) {
                        if (player1Pairs[i] > player2Pairs[i]) {
                            return 1;
                        } else if (player1Pairs[i] < player2Pairs[i]) {
                            return 2;
                        }
                    }
                    return TiedHighCardHand(player1Hand, player2Hand);
                }
                return 0;
            //Three of a Kind
            case 4:
                return FindHighestThreeOfAKind(player1Hand, player2Hand);
            //Straight
            case 5:
                return TiedHighCardHand(player1Hand, player2Hand);
            //Flush
            case 6:
                return TiedHighCardHand(player1Hand, player2Hand);
            //Full House
            case 7:
                //since this is 5 card poker, we only need to search for highest trips.
                //if this is texas hold'em, this will need refactoring.
                return FindHighestThreeOfAKind(player1Hand, player2Hand);
            //Four of a Kind
            case 8:
                return FindHighestFourOfAKind(player1Hand, player2Hand);
            //Straight Flush
            case 9:
                return TiedHighCardHand(player1Hand, player2Hand);
            //Royal Flush does not need to be searched, since only 1 player can have a RF
            default:
                return 0;
            }
        }

        private static int TiedHighCardHand(Hand player1Hand, Hand player2Hand) {
            var winner = 0;
            for (var i = 4; i > 0; i--) {
                var player1Card = player1Hand.Cards[i].Number;
                var player2Card = player2Hand.Cards[i].Number;
                winner = TiedHighCard(player1Card, player2Card);
                if (winner != 0) {
                    return winner;
                }
            }
            return winner;
        }

        private static int TiedHighCard(int player1Card, int player2Card) {
            if (player1Card > player2Card) {
                return 1;
            } else if (player1Card < player2Card) {
                return 2;
            }
            return 0;
        }

        public static int FindHighestThreeOfAKind(Hand player1Hand, Hand player2Hand) {
            var player1Trips = Hand.FindPairs(player1Hand.Cards, 3)[0];
            var player2Trips = Hand.FindPairs(player2Hand.Cards, 3)[0];
            if (player1Trips > player2Trips) {
                return 1;
            } else {
                return 2;
            }
        }

        public static int FindHighestFourOfAKind(Hand player1Hand, Hand player2Hand) {
            var player1Quads = Hand.FindPairs(player1Hand.Cards, 4)[0];
            var player2Quads = Hand.FindPairs(player2Hand.Cards, 4)[0];
            if (player1Quads > player2Quads) {
                return 1;
            } else {
                return 2;
            }
        }
    }
}