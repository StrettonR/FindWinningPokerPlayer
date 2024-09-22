
using System;
using static Program;
using static System.Runtime.InteropServices.JavaScript.JSType;

internal class Program {
    private static void Main(string[] args) {
        var numberOfHandsToGenerate = 1000;
        var generateHands = new GenerateHands();
        if (args != null && args.Length != 0) {
            if (int.TryParse(args[0], out int result)) {
                numberOfHandsToGenerate = result;
            }
        }
        if (numberOfHandsToGenerate > 0) {
            GenerateHands.GenerateRandomHandsAndExportToTxt(numberOfHandsToGenerate, generateHands);
        }
        Console.WriteLine("Finished. " + generateHands.NumberOfHandsGenerated + " hands generated.");
    }

    public class GenerateHands {
        public string FilePath = "C:\\\path\\to\\your\\file\\hands.txt";
        public int NumberOfHandsGenerated { get; set; }
        
        public GenerateHands() {
            this.NumberOfHandsGenerated = 0;
        }

        public GenerateHands(string filePath, int numberOfHandsGenerated) {
            FilePath = filePath;
            this.NumberOfHandsGenerated = numberOfHandsGenerated;
        }

        public static void GenerateRandomHandsAndExportToTxt(int numberOfHands, GenerateHands generateHands) {
            ClearFile();
            for (var i = 0; i < numberOfHands; i++) {
                var hand = new Hand();
                GenerateRandomHand(hand);
                //Console.Write((i + 1) + " | " + hand.CurrentHand);
                ExportHandToTxt(hand.CurrentHand);
                generateHands.NumberOfHandsGenerated++;
            }
            RemoveLastLineFromFile();
        }

        private static void ClearFile() {
            var _ = new GenerateHands();
            var filePath = _.FilePath;
            try {
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write)) {

                }
            } catch (Exception ex) {
                Console.WriteLine($"Error clearing file: {ex.Message}");
            }
        }

        static void RemoveLastLineFromFile() {
            var _ = new GenerateHands();
            var filePath = _.FilePath;
            int charactersToRemove = 2;
            if (File.Exists(filePath) && charactersToRemove > 0) {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite)) {
                    if (fs.Length >= charactersToRemove) {
                        fs.Seek(-charactersToRemove, SeekOrigin.End);
                        fs.SetLength(fs.Position);
                    } else {
                        fs.SetLength(0);
                    }
                }
            }
        }

        private static void GenerateRandomHand(Hand hand) {
            var handAsString = "";
            for (int i = 0; i < 10; i++) {
                var card = SelectRandomCardAndRemoveFromDeck(hand);
                handAsString += card;
                if (i != 9) {
                    handAsString += " ";
                }
            }
            hand.CurrentHand = handAsString + Environment.NewLine;
        }

        static void ExportHandToTxt(string card) {
            var _ = new GenerateHands();
            var filePath = _.FilePath;
            //todo fix. An error can occur here when hands.txt is open elsewhere.
            //possible fix by deleting the contents of the file hands.txt
            //potentially it is running too fast
            Thread.Sleep(1);
            try {
                File.AppendAllText(filePath, card);
            } catch (Exception ex) {
                Console.WriteLine($"Error: {ex.Message} . Possible fix by deleting the contents of the file hands.txt, and running again.");
            }
        }

        private static string SelectRandomCardAndRemoveFromDeck(Hand hand) {
            var random = new Random();
            var randomIndex = random.Next(hand.Deck.Count);
            var card = hand.Deck[randomIndex];
            hand.Deck.RemoveAt(randomIndex);
            return card;
        }
    }

    public class Hand {
        public List<string> Deck = new List<string>() {
            // Clubs
            "AC", "2C", "3C", "4C", "5C", "6C", "7C", "8C", "9C", "TC", "JC", "QC", "KC",
            // Diamonds
            "AD", "2D", "3D", "4D", "5D", "6D", "7D", "8D", "9D", "TD", "JD", "QD", "KD",
            // Hearts
            "AH", "2H", "3H", "4H", "5H", "6H", "7H", "8H", "9H", "TH", "JH", "QH", "KH",
            // Spades
            "AS", "2S", "3S", "4S", "5S", "6S", "7S", "8S", "9S", "TS", "JS", "QS", "KS"
        };

        public string CurrentHand { get; set; }
    }
}