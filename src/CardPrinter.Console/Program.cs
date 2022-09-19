namespace CardPrinter.Console;
 
static class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            System.Console.WriteLine("Usage: Supply the filename of a deck definition as argument");
            System.Console.ReadLine();
            return;
        }

        foreach (var deckFile in args)
        {
            CreatePdf(deckFile);
        }

        System.Console.WriteLine("Done");
    }

    private static void CreatePdf(string deckFile)
    {
        var deck = Deck.Parse(deckFile);
        if (deck != null)
            System.Console.WriteLine($"Deck '{deck.Name}' successfully parsed");

        var isValid = deck.Validate();
        System.Console.WriteLine($"Valid: {isValid}");

        System.Console.WriteLine("Converting images");
        var deckInfo = ImageConverter.ConvertImages(deck);
        System.Console.WriteLine("Formatting pdf");
        PdfFormatter.Format(deck, deckInfo);
        System.Console.WriteLine("Deleting images");
        CleanUp(deckInfo);
    }

    private static void CleanUp(DeckInfo deckInfo) =>
        deckInfo.CardCounts
            .Where(c => File.Exists(c.imagePath))
            .ToList()
            .ForEach(c => File.Delete(c.imagePath));
}
