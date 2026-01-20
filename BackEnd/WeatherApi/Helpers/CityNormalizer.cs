using System.Globalization;
using System.Text;

namespace WeatherApi.Helpers
{
    public static class CityNormalizer
    {
        public static string Normalize(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return string.Empty;

            var normalized = city.Trim().ToLowerInvariant();

            normalized = normalized.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                if (Char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
