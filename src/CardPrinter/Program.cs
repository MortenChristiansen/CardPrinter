namespace CardPrinter;
 
static class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: Supply the filename of a deck definition as argument");
            Console.ReadLine();
            return;
        }

        foreach (var deckFile in args)
        {
            CreatePdf(deckFile);
        }

        Console.WriteLine("Done");
    }

    private static void CreatePdf(string deckFile)
    {
        var deck = Deck.Parse(deckFile);
        if (deck != null)
            Console.WriteLine($"Deck '{deck.Name}' successfully parsed");

        var isValid = deck.Validate();
        Console.WriteLine($"Valid: {isValid}");

        Console.WriteLine("Converting images");
        var deckInfo = ImageConverter.ConvertImages(deck);
        Console.WriteLine("Formatting pdf");
        PdfFormatter.Format(deck, deckInfo);
        Console.WriteLine("Deleting images");
        CleanUp(deckInfo);
    }

    private static void CleanUp(DeckInfo deckInfo) =>
        deckInfo.CardCounts
            .Where(c => File.Exists(c.imagePath))
            .ToList()
            .ForEach(c => File.Delete(c.imagePath));
}
