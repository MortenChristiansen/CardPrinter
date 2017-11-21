namespace CardPrinter.Console
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                System.Console.WriteLine("Usage: Supply the filename of a deck definition as argument");
                System.Console.ReadLine();
                return;
            }

            var deck = Deck.Parse(args[0]);
            if (deck != null)
                System.Console.WriteLine($"Deck '{deck.Name}' successfully parsed");

            var isValid = deck.Validate();
            System.Console.WriteLine($"Valid: {isValid}");

            System.Console.ReadLine();
        }
    }
}
