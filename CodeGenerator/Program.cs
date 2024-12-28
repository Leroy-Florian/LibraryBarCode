using NetBarcode;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

class Program
{
    static void Main(string[] args)
    {
        var prefix = "50810";
        var required_count = 5000;
        var start = 0;

        var outputPdf = "Barcodes_2Columns.pdf";

        PdfDocument document = new();
        document.Info.Title = "Barcodes";

        int barcodesPerPage = 14;
        int currentCount = 0 + start;

        double labelWidth = 180;
        double labelHeight = 100;
        double horizontalSpacing = 100;
        double verticalSpacing = 10;
        double marginLeft = 70;
        double marginTop = 50;

        PdfPage? page = null;
        XGraphics? gfx = null;
        XFont headerFont = new("Arial", 10);
        XFont codeFont = new("Arial", 8);

        while (currentCount < required_count)
        {
            if (currentCount % barcodesPerPage == 0)
            {
                page = document.AddPage();
                page.Size = PdfSharp.PageSize.A4;
                gfx = XGraphics.FromPdfPage(page);
            }

            int indexOnPage = currentCount % barcodesPerPage;
            int row = indexOnPage / 2;
            int column = indexOnPage % 2;

            double startX = marginLeft + column * (labelWidth + horizontalSpacing);
            double startY = marginTop + row * (labelHeight + verticalSpacing);

            var barcodeData = prefix + (currentCount + start).ToString().PadLeft(7, '0');

            Barcode barcode = new(barcodeData, NetBarcode.Type.Code128);
            var base64Image = barcode.GetBase64Image();
            var barcodeImage = ConvertBase64ToXImage(base64Image);

            gfx.DrawString("Médiathèque Saint-Jean d'Elle", headerFont, XBrushes.Black, new XRect(startX, startY, labelWidth, 20), XStringFormats.TopCenter);
            gfx.DrawImage(barcodeImage, startX, startY + 20, labelWidth, 53);
            gfx.DrawString(barcodeData, codeFont, XBrushes.Black, new XRect(startX, startY + 75, labelWidth, 20), XStringFormats.TopCenter);

            currentCount++;
        }

        document.Save(outputPdf);
        Console.WriteLine($"PDF généré : {outputPdf}");
    }

    private static XImage ConvertBase64ToXImage(string base64Image)
    {
        byte[] imageBytes = Convert.FromBase64String(base64Image);
        string tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".png");
        File.WriteAllBytes(tempFilePath, imageBytes);
        XImage image = XImage.FromFile(tempFilePath);
        File.Delete(tempFilePath);
        return image;
    }
}