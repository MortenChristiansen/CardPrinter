using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace CardPrinter.Console
{
    class Deck
    {
        public string Name { get; set; }
        public string RootPath { get; set; }
        public Card[] Cards { get; set; }

        public class Card
        {
            public string FileName { get; set; }
            public int Count { get; set; }
        }

        public static Deck Parse(string deckDefinitionPath)
        {
            if (!File.Exists(deckDefinitionPath))
            {
                System.Console.WriteLine("Deck definition does not exist");
                return null;
            }

            var deckDefinitionJson = File.ReadAllText(deckDefinitionPath);

            try
            {
                var deck = JsonConvert.DeserializeObject<Deck>(deckDefinitionJson);
                deck.Normalize();
                return deck;
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Deck definition invalid: " + e.Message);
            }

            return null;
        }

        private void Normalize()
        {
            foreach (var card in Cards ?? new Card[0])
            {
                if (!card.FileName.EndsWith(".psd", StringComparison.InvariantCultureIgnoreCase))
                    card.FileName += ".psd";
            }
        }

        public bool Validate()
        {
            if (!Directory.Exists(RootPath))
            {
                System.Console.WriteLine("Deck root directory does not exist");
                return false;
            }

            if (Cards == null || Cards.Length == 0)
            {
                System.Console.WriteLine("The deck does not contain any cards");
                return false;
            }

            var valid = true;
            foreach (var card in Cards)
            {
                var cardPath = GetPath(card);
                if (!File.Exists(cardPath))
                {
                    System.Console.WriteLine($"The card '{cardPath}' does not exist");
                    valid = false;
                }
            }
            if (Cards.Any(c => c.Count < 1))
            {
                System.Console.WriteLine($"The count for cards cannot be less than 1");
                valid = false;
            }

            return valid;
        }

        public string GetPath(Card card) =>
            Path.Combine(RootPath, card.FileName);
    }
}
