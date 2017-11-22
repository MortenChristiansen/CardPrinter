using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Linq;

namespace CardPrinter.Console
{
    static class PdfFormatter
    {
        private static readonly float PixelsPerMillimeter = PageSize.A4.Height / 297;

        public static void Format(Deck deck, DeckInfo deckInfo)
        {
            var image1 = deckInfo.CardCounts.First();
            var pdfName = $"output/{deck.Name}.pdf";

            if (File.Exists(pdfName))
                File.Delete(pdfName);

            var margin = 5 * PixelsPerMillimeter;
            var pdfDoc = new Document(PageSize.A4, margin, margin, margin, margin);
            var writer = PdfWriter.GetInstance(pdfDoc, new FileStream(pdfName, FileMode.OpenOrCreate));

            pdfDoc.Open();
            var content = writer.DirectContent;
            var path = Path.GetFullPath(image1.imagePath);
            var image = Image.GetInstance(path);
            image.SetAbsolutePosition(margin, margin);
            image.ScaleAbsolute(69.9f * PixelsPerMillimeter, 95.3f * PixelsPerMillimeter);
            pdfDoc.Add(image);

            pdfDoc.Close();
        }
    }
}
