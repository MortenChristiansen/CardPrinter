using iTextSharp.text;
using iTextSharp.text.pdf;
using static CardPrinter.Console.Deck;

namespace CardPrinter.Console;

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
                AddImage(deck.Dimensions, deck.CuttingLines, margin, card.imagePath, pdfDoc, content, ref pageCardIndex);
            }
        }

        pdfDoc.Close();
    }

    private static void AddImage(CardDimensions dimensions, CuttingLineDimensions cuttingLines, float margin, string imagePath, Document pdfDoc, PdfContentByte content, ref int pageCardIndex)
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

            if (cuttingLines != null)
                AddCuttingLines(content, positionX - (x * clip * 2), positionY + (y * clip * 2) + (2 * clip), cardWidth - (2 * clip), cardHeight - (2 * clip), cardsPerRow, cardsPerColumn, x, y);
        }
        else
        {
            image.SetAbsolutePosition(positionX, positionY);
            pdfDoc.Add(image);

            if (cuttingLines != null)
                AddCuttingLines(content, positionX, positionY, cardWidth, cardHeight, cardsPerRow, cardsPerColumn, x, y);
        }

        pageCardIndex++;
    }

    private static void AddCuttingLines(PdfContentByte content, float x, float y, float width, float height, int cardsPerRow, int cardsPerColumn, int xIndex, int yIndex)
    {
        var borderX = 1.1f * PixelsPerMillimeter;
        var borderY = 1.5f * PixelsPerMillimeter;
        var length = 10 * PixelsPerMillimeter;
        var thickness = 0.1f * PixelsPerMillimeter;

        content.SetLineWidth(thickness);
        content.SetColorStroke(new BaseColor(0,0,0));

        var isLeftMost = xIndex  == 0;
        var isRightMost = xIndex == cardsPerRow - 1;
        var isTopMost = yIndex == 0;
        var isBottomMost = yIndex == cardsPerColumn - 1;

        // Lower left corner
        content.MoveTo(x + borderX, isBottomMost ? 0 : y);
        content.LineTo(x + borderX, y + length);
        content.Stroke();

        content.MoveTo(isLeftMost ? 0 : x, y + borderY);
        content.LineTo(x + length, y + borderY);
        content.Stroke();

        // Lower right corner
        content.MoveTo(x + width - borderX, isBottomMost ? 0 : y);
        content.LineTo(x + width - borderX, y + length);
        content.Stroke();

        content.MoveTo(isRightMost ? 10000 : x + width, y + borderY);
        content.LineTo(x + width - length, y + borderY);
        content.Stroke();

        // Top left corner
        content.MoveTo(x + borderX, isTopMost ? 10000 : y + height);
        content.LineTo(x + borderX, y + height - length);
        content.Stroke();

        content.MoveTo(isLeftMost ? 0 : x, y + height - borderY);
        content.LineTo(x + length, y + height - borderY);
        content.Stroke();

        // Top right corner
        content.MoveTo(x + width - borderX, isTopMost ? 10000 : y + height);
        content.LineTo(x + width - borderX, y + height - length);
        content.Stroke();

        content.MoveTo(isRightMost ? 10000 : x + width, y + height - borderY);
        content.LineTo(x + width - length, y + height - borderY);
        content.Stroke();
    }
}
