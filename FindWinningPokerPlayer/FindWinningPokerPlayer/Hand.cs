namespace FindWinningPokerPlayer {
    public class Hand {
        public Player Player { get; set; }
        public List<Card> Cards { get; set; }

        /// <summary>
        /// Rank = ...
        /// 1 -> High Card: Highest value card.
        /// 2 -> One Pair: Two cards of the same value.
        /// 3 -> Two Pairs: Two different pairs.
        /// 4 -> Three of a Kind: Three cards of the same value.
        /// 5 -> Straight: All cards are consecutive values.
        /// 6 -> Flush: All cards of the same suit.
        /// 7 -> Full House: Three of a kind and a pair.
        /// 8 -> Four of a Kind: Four cards of the same value.
        /// 9 -> Straight Flush: All cards are consecutive values of same suit.
        /// 10 -> Royal Flush: Ten, summaryJack, Queen, King, Ace, in same suit.
        /// <summary>
        public int Rank { get; set; }
        public string RankDescription { get; set; }

        public Hand() {
            Player = new Player() {
                Id = 0
            };
            Cards = new List<Card>();
            Rank = 0;
            RankDescription = "";
        }

        public Hand(int player) {
            Player = new Player() {
                Id = player
            };
            Cards = new List<Card>();
            Rank = 0;
            RankDescription = "";
        }

        public void AddCard(string card) {
            if (String.IsNullOrEmpty(card)) {
                return;
            }
            //if Card.Length == 3, then card is a picture card. example: Jack Of Clubs => "JC" => "11C"
            var suitIndex = 1;
            var isPictureCardOrTen = card.Length > 2;
            var cardIndexAsString = card[0].ToString();
            if (isPictureCardOrTen) {
                suitIndex = 2;
                cardIndexAsString = card.Substring(0, 2);
            }
            var number = 0; 
            var suit = card[suitIndex];
            if (int.TryParse(cardIndexAsString, out int result)) {
                number = result;
            } else {
                switch (card[0].ToString().ToLower()) {
                case "t":
                    number = 10;
                    break;
                case "j":
                    number = 11;
                    break;
                case "q":
                    number = 12;
                    break;
                case "k":
                    number = 13;
                    break;
                case "a":
                    number = 14;
                    break;
                }
            }
            var cd = new Card() {
                Number = number,
                Suit = suit.ToString().ToUpper()
            };
            this.Cards.Add(cd);
            this.OrderCards();
        }

        public void OrderCards() {
            if (this.Cards != null && this.Cards.Count != 0) {
                this.Cards = this.Cards.OrderBy(card => card.Number).ToList();
            }
        }

        public void AsignRankToHand() {
            if (this.Player.Id == 0 || this.Cards == null || this.Cards.Count != 5) {
                return;
            }
            if (this.IsRoyalFlush(this.Cards)) {
                this.Rank = 10;
            } else if (this.IsStraightFlush(this.Cards)) {
                this.Rank = 9;
            } else if (this.IsFourOfAKind(this.Cards)) {
                this.Rank = 8;
            } else if (this.IsFullHouse(this.Cards)) {
                this.Rank = 7;
            } else if (this.IsFlush(this.Cards)) {
                this.Rank = 6;
            } else if (IsStraight(this.Cards)) {
                this.Rank = 5;
            } else if (this.IsThreeOfAKind(this.Cards)) {
                this.Rank = 4;
            } else if (this.IsTwoPairs(this.Cards)) {
                this.Rank = 3;
            } else if (IsOnePair(this.Cards)) {
                this.Rank = 2;
            } else {
                this.Rank = 1;
            }
            this.SetRankDescription(this.Rank);
        }

        private void SetRankDescription(int rank) {
            //Rank = ...
            //1 -> High Card: Highest value card.
            //2 -> One Pair: Two cards of the same value.
            //3 -> Two Pairs: Two different pairs.
            //4 -> Three of a Kind: Three cards of the same value.
            //5 -> Straight: All cards are consecutive values.
            //6 -> Flush: All cards of the same suit.
            //7 -> Full House: Three of a kind and a pair.
            //8 -> Four of a Kind: Four cards of the same value.
            //9 -> Straight Flush: All cards are consecutive values of same suit.
            //10 -> Royal Flush: Ten, summaryJack, Queen, King, Ace, in same suit.
            switch (rank) {
            case 1:
                this.RankDescription = "High Card";
                break;
            case 2:
                this.RankDescription = "One Pair";
                break;
            case 3:
                this.RankDescription = "Two Pairs";
                break;
            case 4:
                this.RankDescription = "Three of a Kind";
                break;
            case 5:
                this.RankDescription = "Straight";
                break;
            case 6:
                this.RankDescription = "Flush";
                break;
            case 7:
                this.RankDescription = "Full House";
                break;
            case 8:
                this.RankDescription = "Four of a Kind";
                break;
            case 9:
                this.RankDescription = "Straight Flush";
                break;
            case 10:
                this.RankDescription = "Royal Flush";
                break;
            }
        }

        internal static List<int> FindPairs(List<Card> cards, int search) {
            var pairs = cards
                .GroupBy(card => card.Number)
                .Where(group => group.Count() >= search)
                .Select(group => group.Key)
                .ToList();

            return pairs;
        }

        internal static bool IsOnePair(List<Card> cards) {
            return FindPairs(cards, 2).Count == 1;
        }

        internal bool IsTwoPairs(List<Card> cards) {
            return FindPairs(cards, 2).Count == 2 && !this.IsThreeOfAKind(this.Cards);
        }

        internal bool IsThreeOfAKind(List<Card> cards) {
            return FindPairs(cards, 3).Count == 1 && !this.IsFourOfAKind(this.Cards);
        }

        internal static bool IsStraight(List<Card> cards) {
            var number = cards[0].Number;
            foreach (var card in cards) {
                if (card.Number != number) {
                    return false;
                }
                number++;
            }
            return true;
        }

        internal bool IsFlush(List<Card> cards) {
            var suit = cards[0].Suit;
            foreach (var card in cards) {
                if (card.Suit != suit) {
                    return false;
                }
            }
            return true;
        }

        internal bool IsFullHouse(List<Card> cards) {
            if (FindPairs(cards, 3).Count() != 1) {
                return false;
            }
            if (FindPairs(cards, 2).Count() != 2) {
                return false;
            }
            return true;
        }

        internal bool IsFourOfAKind(List<Card> cards) {
            return FindPairs(cards, 4).Count() == 1;
        }

        internal bool IsStraightFlush(List<Card> cards) {
            return this.IsFlush(cards) && IsStraight(cards);
        }

        internal bool IsRoyalFlush(List<Card> cards) {
            if (cards.Count == 0) {
                return false;
            }
            if (cards[0].Number != 10) {
                return false;
            }
            if (cards[1].Number != 11) {
                return false;
            }
            if (cards[2].Number != 12) {
                return false;
            }
            if (cards[3].Number != 13) {
                return false;
            }
            if (cards[4].Number != 14) {
                return false;
            }
            return this.IsFlush(cards);
        }
    }
}
