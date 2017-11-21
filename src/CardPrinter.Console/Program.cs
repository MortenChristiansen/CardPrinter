using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace CardPrinter.Console
{
    static class Program
    {
        static void Main(string[] args)
        {
            var deck = ParseDeck(args);
            if (deck != null)
                System.Console.WriteLine($"Deck '{deck.Name}' successfully parsed");

            Normalize(deck);

            var isValid = ValidateDeck(deck);
            System.Console.WriteLine($"Valid: {isValid}");

            System.Console.ReadLine();
        }

        private static Deck ParseDeck(string[] args)
        {
            if (args.Length != 1)
            {
                System.Console.WriteLine("Usage: Supply the filename of a deck definition as argument");
                return null;
            }

            var deckDefinitionPath = args[0];

            if (!File.Exists(deckDefinitionPath))
            {
                System.Console.WriteLine("Deck definition does not exist");
                return null;
            }

            var deckDefinitionJson = File.ReadAllText(deckDefinitionPath);

            try
            {
                return JsonConvert.DeserializeObject<Deck>(deckDefinitionJson);
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Deck definition invalid: " + e.Message);
            }

            return null;
        }

        public static void Normalize(Deck deck)
        {
            foreach (var card in deck.Cards ?? new Deck.Card[0])
            {
                if (!card.FileName.EndsWith(".psd", StringComparison.InvariantCultureIgnoreCase))
                    card.FileName += ".psd";
            }
        }

        public static bool ValidateDeck(Deck deck)
        {
            if (!Directory.Exists(deck.RootPath))
            {
                System.Console.WriteLine("Deck root directory does not exist");
                return false;
            }

            if (deck.Cards == null || deck.Cards.Length == 0)
            {
                System.Console.WriteLine("The deck does not contain any cards");
                return false;
            }

            var valid = true;
            foreach (var card in deck.Cards)
            {
                var cardPath = Path.Combine(deck.RootPath, card.FileName);
                if (!File.Exists(cardPath))
                {
                    System.Console.WriteLine($"The card '{cardPath}' does not exist");
                    valid = false;
                }
            }
            if (deck.Cards.Any(c => c.Count < 1))
            {
                System.Console.WriteLine($"The count for cards cannot be less than 1");
                valid = false;
            }

            return valid;
        }
    }
}
