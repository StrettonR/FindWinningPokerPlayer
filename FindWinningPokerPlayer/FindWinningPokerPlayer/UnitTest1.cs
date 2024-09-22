namespace FindWinningPokerPlayer {
    public class Tests {
        [SetUp]
        public void Setup() {
        }
        
        [Test]
        public void FindPlayerWithMostWins() {
            var filepath = "FilePath_5TestHands";
            var game = new FindWinningPokerPlayer(filepath);
            var txt = game.ImportTxt();
            // 5H 5C 6S 7S KD pair          2C 3S 8S 8D TD  pair
            // winner = 2
            // 5D 8C 9S JS AC high card     2C 5C 7D 8S QH  high card
            // winner = 1
            // 2D 9C AS AH AC three of a kind    3D 6D 7D TD QD flush
            // winner = 2
            // 4D 6S 9H QH QC pair          3D 6D 7H QD QS pair
            // winner = 1
            // 2H 2D 4C 4D 4S full house    3C 3D 3S 9S 9D full house
            // winner = 1
            //    Hand Player 1     Player 2            Winner
            //1   5H 5C 6S 7S KD    2C 3S 8S 8D TD      Player 2
            //    Pair of Fives     Pair of Eights
            //2   5D 8C 9S JS AC    2C 5C 7D 8S QH      Player 1
            //    Highest card Ace  Highest card Queen
            //3   2D 9C AS AH AC    3D 6D 7D TD QD      Player 2
            //    Three Aces        Flush with Diamonds
            //4   4D 6S 9H QH QC    3D 6D 7H QD QS      Player 1
            //    Pair of Queens    Pair of Queens
            //    Highest card Nine Highest card Seven
            //5   2H 2D 4C 4D 4S    3C 3D 3S 9S 9D      Player 1
            //    Full House        Full House
            //    With Three Fours  with Three Threes

            var expectedWinner = 1;
            var player1Hands = new Hand(1);
            var player2Hands = new Hand(2);
            for (int i = 0; i < 5; i++) {
                var cardList = game.TxtToHand(txt, i);
                var player1Hand = AsignRankSetup(cardList, 1);
                var player2Hand = AsignRankSetup(cardList, 2);
                var winner = game.CompareHands(player1Hand, player2Hand);
                FindWinningPokerPlayer.PlayerWinsRunningTotal(player1Hands, player2Hands, winner);
            }
            var playerWithMostWins = FindWinningPokerPlayer.FindPlayerWithMostWins(player1Hands, player2Hands);
            Assert.That(expectedWinner, Is.EqualTo(playerWithMostWins));
        }

        [Test]
        public void CompareHands() {
            var filepath = "FilePath_5TestHands";
            var game = new FindWinningPokerPlayer(filepath);
            var txt = game.ImportTxt();
            var cardList = game.TxtToHand(txt, 2);
            var player1Hand = AsignRankSetup(cardList, 1);
            var player2Hand = AsignRankSetup(cardList, 2);
            var winner = game.CompareHands(player1Hand, player2Hand);
            Assert.That(winner, Is.EqualTo(2));
        }
        [Test]
        public void CompareMutipleHands() {
            var filepath = "FilePath_5TestHands";
            var game = new FindWinningPokerPlayer(filepath);
            var txt = game.ImportTxt();
            // 5H 5C 6S 7S KD pair          2C 3S 8S 8D TD  pair
            // winner = 2
            // 5D 8C 9S JS AC high card     2C 5C 7D 8S QH  high card
            // winner = 1
            // 2D 9C AS AH AC three of a kind    3D 6D 7D TD QD flush
            // winner = 2
            // 4D 6S 9H QH QC pair          3D 6D 7H QD QS pair
            // winner = 1
            // 2H 2D 4C 4D 4S full house    3C 3D 3S 9S 9D full house
            // winner = 1
            //    Hand Player 1     Player 2            Winner
            //1   5H 5C 6S 7S KD    2C 3S 8S 8D TD      Player 2
            //    Pair of Fives     Pair of Eights
            //2   5D 8C 9S JS AC    2C 5C 7D 8S QH      Player 1
            //    Highest card Ace  Highest card Queen
            //3   2D 9C AS AH AC    3D 6D 7D TD QD      Player 2
            //    Three Aces        Flush with Diamonds
            //4   4D 6S 9H QH QC    3D 6D 7H QD QS      Player 1
            //    Pair of Queens    Pair of Queens
            //    Highest card Nine Highest card Seven
            //5   2H 2D 4C 4D 4S    3C 3D 3S 9S 9D      Player 1
            //    Full House        Full House
            //    With Three Fours  with Three Threes
            List<int> expectedWinner = new List<int>() {
                2,1,2,1,1
            };
            for (int i = 0; i < 5; i++) {
                var cardList = game.TxtToHand(txt, i);
                var player1Hand = AsignRankSetup(cardList, 1);
                var player2Hand = AsignRankSetup(cardList, 2);
                var winner = game.CompareHands(player1Hand, player2Hand);
                if (winner != expectedWinner[i]) {
                    Assert.Fail();
                }
            }
            Assert.Pass();
        }

        [Test]
        public void CompareMutipleHandsTestTiedHands() {
            var filepath = "FilePathTestTiedHands";
            var game = new FindWinningPokerPlayer(filepath);
            // 2C 3D 4H 5S 7C 3C 4D 5H 6S 8C //highcard
            // 2C 2D 4H 5S 7C 2H 2S 5H 7S 8C //pair
            // 2C 2D 4C 4D 7C 2H 2S 4H 4S 8C //2pair
            // 2C 2D 2H 3S 7C 3C 3D 3H 4S 8C //trips
            // 2C 3D 4H 5S 6C TC JD QH KS AC //straight
            // 2C 3C 4C 5C 7C 9D JD QD KD AD //flush
            // 2C 2D 2H 3S 3C 4C 4D 4H 8S 8C //fullhouse
            // 2C 2D 2H 2S 7C 3C 3D 3H 3S 8C //quads
            // 2C 3C 4C 5C 6C 9D TD JD QD KD //straightflush
            // 2C 3C 4C 5C 6C TD JD QD KD AD //royalFlush
            var txt = game.ImportTxt();
            var expectedWinner = new List<int>() {
                2,2,2,2,2
            };
            for (int i = 0; i < 5; i++) {
                var cardList = game.TxtToHand(txt, i);
                var player1Hand = AsignRankSetup(cardList, 1);
                var player2Hand = AsignRankSetup(cardList, 2);
                var winner = game.CompareHands(player1Hand, player2Hand);
                if (winner != expectedWinner[i]) {
                    Assert.Fail();
                }
            }
            Assert.Pass();
        }

        [Test]
        public void TiedHand() {
            var filepath = "FilePath_5TestHands";
            var game = new FindWinningPokerPlayer(filepath);
            var txt = game.ImportTxt();
            var cardList = game.TxtToHand(txt, 0);
            var player1Hand = new Hand(1);
            var player2Hand = new Hand(2);
            game.AsignHandToPlayer(player1Hand, cardList);
            game.AsignHandToPlayer(player2Hand, cardList);
            game.TiedHand(player1Hand, player2Hand);
        }

        [Test]
        public void ImportTxt() {
            var filepath = "FilePathTestHands";
            var game = new FindWinningPokerPlayer(filepath);
            var txt = game.ImportTxt();
            Assert.That(txt, Is.EqualTo("5H 5C 6S 7S KD 2C 3S 8S 8D TD"));
        }

        [Test]
        public void TxtToHandCheckListCount() {
            var filepath = "FilePath_5TestHands";
            var game = new FindWinningPokerPlayer(filepath);
            var txt = game.ImportTxt();
            var list = game.TxtToHand(txt, 0);
            Assert.That(list, Has.Count.EqualTo(10));
        }

        [Test]
        public void TxtToHandCheckCardValue() {
            var filepath = "FilePath_5TestHands";
            var game = new FindWinningPokerPlayer(filepath);
            var txt = game.ImportTxt();
            var cardList = game.TxtToHand(txt, 0);
            Assert.That(cardList[4], Is.EqualTo("KD"));
        }

        [Test]
        public void AsignPlayerId() {
            var player1Hand = new Hand(1);
            Assert.That(player1Hand.Player.Id, Is.EqualTo(1));
        }

        [Test]
        public void AsignHandToPlayer() {
            var filepath = "FilePath_5TestHands";
            var game = new FindWinningPokerPlayer(filepath);
            var txt = game.ImportTxt();
            var cardList = game.TxtToHand(txt, 0);
            var player1Hand = new Hand(1);
            var player2Hand = new Hand(2);
            game.AsignHandToPlayer(player1Hand, cardList);
            game.AsignHandToPlayer(player2Hand, cardList);
            var expected = "5H 5C 6S 7S 13D 2C 3S 8S 8D 10D ";
            var cardAsString = "";
            foreach (var card in player1Hand.Cards) {
                cardAsString += card.Number.ToString();
                cardAsString += card.Suit.ToString();
                cardAsString += " ";
            }
            foreach (var card in player2Hand.Cards) {
                cardAsString += card.Number.ToString();
                cardAsString += card.Suit.ToString();
                cardAsString += " ";
            }
            Assert.That(cardAsString, Is.EqualTo(expected));
        }

        [Test]
        public void AddCardTestNumber() {
            var hand = new Hand();
            var card = "3H";
            hand.AddCard(card);
            var cardNumber = hand.Cards.First().Number;
            Assert.That(cardNumber, Is.EqualTo(3));
        }

        [Test]
        public void AddCardTestPictureCard() {
            var hand = new Hand();
            var card = "ac";
            hand.AddCard(card);
            var cardNumber = hand.Cards.First().Number;
            Assert.That(cardNumber, Is.EqualTo(14));
        }

        [Test]
        public void AddCardTestSuit() {
            var hand = new Hand();
            string card = "3H";
            hand.AddCard(card);
            string cardSuit = hand.Cards.First().Suit;
            Assert.That(card[1].ToString(), Is.EqualTo(cardSuit));
        }

        private static Hand AsignRankSetup(List<string> cardList, int p) {
            var game = new FindWinningPokerPlayer();
            var hand = new Hand(p);
            game.AsignHandToPlayer(hand, cardList);
            hand.AsignRankToHand();
            return hand;
        }

        [Test]
        public void AsignRankTestMultiplehands() {
            var filepath = "FilePath_5TestHands";
            var game = new FindWinningPokerPlayer(filepath);
            var txt = game.ImportTxt();
            var pass = true;
            // 5H 5C 6S 7S KD pair          2C 3S 8S 8D TD  pair
            // winner = 2
            // 5D 8C 9S JS AC high card     2C 5C 7D 8S QH  high card
            // winner = 1
            // 2D 9C AS AH AC three of a kind    3D 6D 7D TD QD flush
            // winner = 2
            // 4D 6S 9H QH QC pair          3D 6D 7H QD QS pair
            // winner = 1
            // 2H 2D 4C 4D 4S full house    3C 3D 3S 9S 9D full house
            // winner = 1
            //    Hand Player 1     Player 2            Winner
            //1   5H 5C 6S 7S KD    2C 3S 8S 8D TD      Player 2
            //    Pair of Fives     Pair of Eights
            //2   5D 8C 9S JS AC    2C 5C 7D 8S QH      Player 1
            //    Highest card Ace  Highest card Queen
            //3   2D 9C AS AH AC    3D 6D 7D TD QD      Player 2
            //    Three Aces        Flush with Diamonds
            //4   4D 6S 9H QH QC    3D 6D 7H QD QS      Player 1
            //    Pair of Queens    Pair of Queens
            //    Highest card Nine Highest card Seven
            //5   2H 2D 4C 4D 4S    3C 3D 3S 9S 9D      Player 1
            //    Full House        Full House
            //    With Three Fours  with Three Threes

            List<int> expectedRankPlayer1 = new List<int>() {
                2,1,4,2,7
            };
            List<int> expectedRankPlayer2 = new List<int>() {
                2,1,6,2,7
            };
            for (int i = 0; i < 5; i++) {
                var cardList = game.TxtToHand(txt, i);
                var player1Hand = AsignRankSetup(cardList, 1);
                var player2Hand = AsignRankSetup(cardList, 2);
                if (player1Hand.Rank != expectedRankPlayer1[i]) {
                    pass = false;
                }
                if (player2Hand.Rank != expectedRankPlayer2[i]) {
                    pass = false;
                }
            }
            Assert.That(pass, Is.True);
        }

        [Test]
        public void AsignRankToHandExpectHighHand() {
            var expected = 1;
            var cardList = new List<string>() { "2H", "5C", "6S", "7S", "KD", "2C", "3S", "9S", "8D", "TD" };
            var player1Hand = AsignRankSetup(cardList, 1);
            Assert.That(player1Hand.Rank, Is.EqualTo(expected));
        }

        [Test]
        public void AsignRankToHandExpectPair() {
            var expected = 2;
            var cardList = new List<string>() { "5H", "5C", "6S", "7S", "KD", "2C", "3S", "9S", "8D", "TD" };
            var player1Hand = AsignRankSetup(cardList, 1);
            Assert.That(player1Hand.Rank, Is.EqualTo(expected));
        }

        [Test]
        public void AsignRankToHandExpectPairPlayer2() {
            var expected = 2;
            var cardList = new List<string>() { "5H", "5C", "6S", "7S", "KD", "2C", "3S", "9S", "TC", "TD" };
            var player1Hand = AsignRankSetup(cardList, 2);
            Assert.That(player1Hand.Rank, Is.EqualTo(expected));
        }

        [Test]
        public void AsignRankToHandExpectTwoPair() {
            var expected = 3;
            var cardList = new List<string>() { "5H", "5C", "6S", "6S", "KD", "2C", "3S", "9S", "8D", "TD" };
            var player1Hand = AsignRankSetup(cardList, 1);
            Assert.That(player1Hand.Rank, Is.EqualTo(expected));
        }

        [Test]
        public void AsignRankToHandExpectThreeOfAKind() {
            var expected = 4;
            var cardList = new List<string>() { "5H", "5C", "5S", "7S", "KD", "2C", "3S", "9S", "8D", "TD" };
            var player1Hand = AsignRankSetup(cardList, 1);
            Assert.That(player1Hand.Rank, Is.EqualTo(expected));
        }

        [Test]
        public void AsignRankToHandExpectThreeOfAKindFalse() {
            var expected = 4;
            var cardList = new List<string>() { "5H", "5C", "5S", "7S", "7D", "2C", "3S", "9S", "8D", "TD" };
            var player1Hand = AsignRankSetup(cardList, 1);
            Assert.IsFalse(player1Hand.Rank == expected);
        }

        [Test]
        public void AsignRankToHandExpectStraight() {
            var expected = 5;
            var cardList = new List<string>() { "5H", "6C", "7S", "8S", "9D", "2C", "3S", "9S", "8D", "TD" };
            var player1Hand = AsignRankSetup(cardList, 1);
            Assert.That(player1Hand.Rank, Is.EqualTo(expected));
        }

        [Test]
        public void AsignRankToHandExpectFlush() {
            var expected = 6;
            var cardList = new List<string>() { "5H", "5H", "6H", "7H", "KH", "2C", "3S", "9S", "8D", "TD" };
            var player1Hand = AsignRankSetup(cardList, 1);
            Assert.That(player1Hand.Rank, Is.EqualTo(expected));
        }

        [Test]
        public void AsignRankToHandExpectIsFullHouse() {
            var expected = 7;
            var cardList = new List<string>() { "5H", "5C", "5S", "7S", "7D", "2C", "3S", "9S", "8D", "TD" };
            var player1Hand = AsignRankSetup(cardList, 1);
            Assert.That(player1Hand.Rank, Is.EqualTo(expected));
        }

        [Test]
        public void AsignRankToHandExpectFourOfAKind() {
            var expected = 8;
            var cardList = new List<string>() { "5H", "5C", "5S", "5D", "KD", "2C", "3S", "9S", "8D", "TD" };
            var player1Hand = AsignRankSetup(cardList, 1);
            Assert.That(player1Hand.Rank, Is.EqualTo(expected));
        }

        [Test]
        public void AsignRankToHandExpectStraightFlush() {
            var expected = 9;
            var cardList = new List<string>() { "5H", "6H", "7H", "8H", "9H", "2C", "3S", "9S", "8D", "TD" };
            var player1Hand = AsignRankSetup(cardList, 1);
            Assert.That(player1Hand.Rank, Is.EqualTo(expected));
        }

        [Test]
        public void AsignRankToHandExpectRoyalFlush() {
            var expected = 10;
            var cardList = new List<string>() { "TH", "JH", "QH", "KH", "AH", "2C", "3S", "9S", "8D", "TD" };
            var player1Hand = AsignRankSetup(cardList, 1);
            Assert.That(player1Hand.Rank, Is.EqualTo(expected));
        }

        [Test]
        public void IsOnePairFalse() {
            var game = new FindWinningPokerPlayer();
            var cardList = new List<string>() { "2H", "5C", "6S", "7S", "KD", "2C", "3S", "9S", "8D", "TD" };
            var player1Hand = new Hand(1);
            game.AsignHandToPlayer(player1Hand, cardList);
            Assert.That(Hand.IsOnePair(player1Hand.Cards), Is.EqualTo(false));
        }

        [Test]
        public void IsOnePairTrue() {
            //5H, 5C one pair, expect true
            var game = new FindWinningPokerPlayer();
            var cardList = new List<string>() { "5H", "5C", "6S", "7S", "KD", "2C", "3S", "9S", "8D", "TD" };
            var player1Hand = new Hand(1);
            game.AsignHandToPlayer(player1Hand, cardList);
            Assert.That(Hand.IsOnePair(player1Hand.Cards), Is.EqualTo(true));
        }

        [Test]
        public void IsRoyalFlush() {
            //5H, 5C one pair, expect true
            var game = new FindWinningPokerPlayer();
            var cardList = new List<string>() { "10C", "JC", "QC", "KC", "AC", "2C", "3S", "9S", "8D", "TD" };
            var player1Hand = new Hand(1);
            game.AsignHandToPlayer(player1Hand, cardList);
            Assert.That(player1Hand.IsRoyalFlush(player1Hand.Cards), Is.EqualTo(true));
        }

        [Test]
        public void OrderCards() {
            var hand = new Hand();
            hand.AddCard("AC");
            hand.AddCard("3C");
            hand.AddCard("2c");
            hand.AddCard("JC");
            hand.AddCard("5C");
            hand.OrderCards();
            var expected = "2C 3C 5C 11C 14C ";
            var handAsStringResult = "";
            foreach (var card in hand.Cards) {
                handAsStringResult += card.Number.ToString();
                handAsStringResult += card.Suit.ToString();
                handAsStringResult += " ";
            }
            var cardNumber = hand.Cards[0];
            Assert.That(expected, Is.EqualTo(handAsStringResult));
        }
    }
}