namespace CardPrinter;

class DeckInfo
{
    public IEnumerable<(string imagePath, int count)> CardCounts => _cardCounts;

    private List<(string imagePath, int count)> _cardCounts = new List<(string, int)>();

    public void AddCardCount(string imagePath, int count) =>
        _cardCounts.Add((imagePath, count));
}