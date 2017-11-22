using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Linq;

namespace CardPrinter.Console
{
    static class PdfFormatter
    {
        private static readonly float PixelsPerMillimeter = PageSize.A4.Height / 297;
        private static readonly float Margin = 0 * PixelsPerMillimeter;

        public static void Format(Deck deck, DeckInfo deckInfo)
        {
            var pdfName = $"output/{deck.Name}.pdf";

            if (File.Exists(pdfName))
                File.Delete(pdfName);

            var pdfDoc = new Document(PageSize.A4, Margin, Margin, Margin, Margin);
            var writer = PdfWriter.GetInstance(pdfDoc, new FileStream(pdfName, FileMode.OpenOrCreate));

            pdfDoc.Open();
            var content = writer.DirectContent;

            var pageCardIndex = 0;
            foreach (var card in deckInfo.CardCounts)
            {
                for (int i = 0; i < card.count; i++)
                {
                    AddImage(deck.CardWidth, deck.CardHeight, card.imagePath, pdfDoc, ref pageCardIndex);
                }
            }

            pdfDoc.Close();
        }

        private static void AddImage(decimal cardWidthInMm, decimal cardHeightInMm, string imagePath, Document pdfDoc, ref int pageCardIndex)
        {
            var availableWidth = PageSize.A4.Width - (2 * Margin);
            var availableHeight = PageSize.A4.Height - (2 * Margin);
            var cardWidth = (float)cardWidthInMm * PixelsPerMillimeter;
            var cardHeight = (float)cardHeightInMm * PixelsPerMillimeter;
            var cardsPerRow = (int)(availableWidth / cardWidth);
            var cardsPerColumn = (int)(availableHeight / cardHeight);

            if (pageCardIndex >= cardsPerRow * cardsPerColumn)
            {
                pageCardIndex = 0;
                pdfDoc.NewPage();
            }

            var x = pageCardIndex % cardsPerRow;
            var y = pageCardIndex / cardsPerRow;

            var positionX = Margin + (x * cardWidth);
            var positionY = PageSize.A4.Height - (Margin + (y * cardHeight)) - cardHeight;

            var path = Path.GetFullPath(imagePath);
            var image = Image.GetInstance(path);
            image.SetAbsolutePosition(positionX, positionY);
            image.ScaleAbsolute(cardWidth, cardHeight);
            pdfDoc.Add(image);
            pageCardIndex++;
        }
    }
}
