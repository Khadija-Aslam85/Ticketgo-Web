using QRCoder;

namespace TicketGo.Services
{
    public interface IQrService
    {
        string CreateSvg(string text);
    }

    public class QrService : IQrService
    {
        // returns the QR code as an SVG image
        public string CreateSvg(string text)
        {
            var generator = new QRCodeGenerator();
            var data = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            var svg = new SvgQRCode(data);
            return svg.GetGraphic(6);
        }
    }
}
