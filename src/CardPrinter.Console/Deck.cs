using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
