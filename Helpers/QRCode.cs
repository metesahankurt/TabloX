using Net.Codecrete.QrCodeGenerator;
using Microsoft.Extensions.Logging;

namespace TabloX2.Helpers
{
    public static class QRCode
    {
        public static string GenerateQRCode(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("QR kod içeriği boş olamaz.", nameof(text));
            }

            try
            {
                var qr = QrCode.EncodeText(text, QrCode.Ecc.Medium);
                var svg = qr.ToSvgString(4);
                return $"data:image/svg+xml;base64,{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(svg))}";
            }
            catch (Exception ex)
            {
                // Hata durumunda alternatif yöntem
                try
                {
                    var qr = QrCode.EncodeText(text, QrCode.Ecc.Low);
                    var svg = qr.ToSvgString(2);
                    return $"data:image/svg+xml;base64,{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(svg))}";
                }
                catch (Exception innerEx)
                {
                    throw new Exception($"QR kod oluşturma hatası: {ex.Message} - Alternatif yöntem de başarısız: {innerEx.Message}", ex);
                }
            }
        }
    }
} 