using System.Web;

namespace Candle_API.Extentions
{
    public static class ValidationExtensions
    {
        public static string Sanitize(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Eliminar caracteres peligrosos
            return HttpUtility.HtmlEncode(input);
        }
    }
}
