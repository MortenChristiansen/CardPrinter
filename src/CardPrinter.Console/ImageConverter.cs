using ImageMagick;
using System.IO;

namespace CardPrinter.Console
{
    static class ImageConverter
    {
        public static void ConvertImages(Deck deck)
        {
            if (!Directory.Exists("output"))
                Directory.CreateDirectory("output");

            foreach (var card in deck.Cards)
            {
                var cardPath = deck.GetPath(card);
                using (var image = new MagickImage(cardPath))
                {
                    var cardFileName = Path.ChangeExtension(Path.GetFileName(cardPath), "png");
                    var newFilePath = Path.Combine("output", cardFileName);
                    if (File.Exists(newFilePath))
                        File.Delete(newFilePath);

                    image.Write(newFilePath);
                }
            }
        }
    }
}
