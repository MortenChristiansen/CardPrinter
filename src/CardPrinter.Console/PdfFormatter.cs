using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using static CardPrinter.Console.Deck;

namespace CardPrinter.Console
{
    static class PdfFormatter
    {
        private static readonly float PixelsPerMillimeter = PageSize.A4.Height / 297;

        public static void Format(Deck deck, DeckInfo deckInfo)
        {
            var pdfName = $"output/{deck.Name}.pdf";

            if (File.Exists(pdfName))
                File.Delete(pdfName);

            var margin = (float)deck.Dimensions.PageMargin * PixelsPerMillimeter;
            var pdfDoc = new Document(PageSize.A4, margin, margin, margin, margin);
            var writer = PdfWriter.GetInstance(pdfDoc, new FileStream(pdfName, FileMode.OpenOrCreate));

            pdfDoc.Open();
            var content = writer.DirectContent;

            var pageCardIndex = 0;
            foreach (var card in deckInfo.CardCounts)
            {
                for (int i = 0; i < card.count; i++)
                {
                    AddImage(deck.Dimensions, margin, card.imagePath, pdfDoc, content, ref pageCardIndex);
                }
            }

            pdfDoc.Close();
        }

        private static void AddImage(CardDimensions dimensions, float margin, string imagePath, Document pdfDoc, PdfContentByte content, ref int pageCardIndex)
        {
            var availableWidth = PageSize.A4.Width - (2 * margin);
            var availableHeight = PageSize.A4.Height - (2 * margin);
            var cardWidth = (float)dimensions.CardWidth * PixelsPerMillimeter;
            var cardHeight = (float)dimensions.CardHeight * PixelsPerMillimeter;
            var enableClipping = dimensions.ImageBorderClipping.HasValue;
            var clip = (float)(dimensions.ImageBorderClipping ?? 0.0m) * PixelsPerMillimeter;
            var clippedCardWidth = cardWidth - (2 * clip);
            var clippedCardHeight = cardHeight - (2 * clip);
            var cardsPerRow = (int)(availableWidth / (enableClipping ? clippedCardWidth : cardWidth));
            var cardsPerColumn = (int)(availableHeight / (enableClipping ? clippedCardHeight : cardHeight));

            if (pageCardIndex >= cardsPerRow * cardsPerColumn)
            {
                pageCardIndex = 0;
                pdfDoc.NewPage();
            }

            var x = pageCardIndex % cardsPerRow;
            var y = pageCardIndex / cardsPerRow;

            var positionX = margin + (x * cardWidth);
            var positionY = PageSize.A4.Height - (margin + (y * cardHeight)) - cardHeight;

            var path = Path.GetFullPath(imagePath);
            var image = Image.GetInstance(path);
            image.ScaleAbsolute(cardWidth, cardHeight);

            if (enableClipping)
            {
                var temp = content.CreateTemplate(cardWidth, cardHeight);
                temp.Rectangle(clip, clip, cardWidth - (2 * clip), cardHeight - (2 * clip));
                temp.Clip();
                temp.NewPath();
                temp.AddImage(image, cardWidth, 0, 0, cardHeight, 0, 0);
                var clipped = Image.GetInstance(temp);
                clipped.SetAbsolutePosition(positionX - (x * clip * 2) - clip, positionY + (y * clip * 2) + clip);

                pdfDoc.Add(clipped);
            }
            else
            {
                image.SetAbsolutePosition(positionX, positionY);
                pdfDoc.Add(image);
            }

            pageCardIndex++;
        }
    }
}
