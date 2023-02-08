using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Framework.Core.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex _matchAllTags =
            new Regex(@"<(.|\n)*?>", options: RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex _matchArabicHebrew =
           new Regex(@"[\u0600-\u06FF,\u0590-\u05FF]", options: RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private const char RleChar = (char)0x202B;

        /// <summary>
        /// convert Base64String to byte array.
        /// </summary>
        public static byte[] ToByteArray(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return new byte[2];
            else
            {
                var bytes = Convert.FromBase64String(str);

                return bytes;
            }
        }

        /// <summary>
        /// Is string contains farsi character?
        /// </summary>
        /// <returns>return true if string is contains farsi character.otherwise return false.</returns>
        public static bool ContainsFarsi(this string txt)
        {
            return !string.IsNullOrEmpty(txt) &&
                _matchArabicHebrew.IsMatch(txt.StripHtmlTags().Replace(",", ""));
        }

        /// <summary>
        /// Remove HTML tags from string.
        /// </summary>
        public static string StripHtmlTags(this string text)
        {
            return string.IsNullOrEmpty(text) ?
                        string.Empty :
                        _matchAllTags.Replace(text, " ").Replace("&nbsp;", " ");
        }

        /// <summary>
        /// if string comtains farsi words wraped with rtl div tag.
        /// </summary>
        public static string WrapInDirectionalDiv(this string body, string fontFamily = "tahoma", string fontSize = "9pt")
        {
            if (string.IsNullOrWhiteSpace(body))
                return string.Empty;

            if (ContainsFarsi(body))
                return $"<div style='text-align: right; font-family:{fontFamily}; font-size:{fontSize};' dir='rtl'>{body}</div>";
            return $"<div style='text-align: left; font-family:{fontFamily}; font-size:{fontSize};' dir='ltr'>{body}</div>";
        }

        /// <summary>
        ///  Applies RLE to the text if it contains Persian words.
        /// </summary>
        public static string ApplyRle(this string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            return text.ContainsFarsi() ? $"{RleChar}{text}" : text;
        }

        /// <summary>
        /// Remove diacritics from text
        /// </summary>
        public static string RemoveDiacritics(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var normalizedString = text.Normalize(NormalizationForm.FormKC);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// remove underlines from text.
        /// </summary>
        public static string CleanUnderLines(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            const char chr1600 = (char)1600; //ـ=1600
            const char chr8204 = (char)8204; //‌=8204

            return text.Replace(chr1600.ToString(), "")
                       .Replace(chr8204.ToString(), "");
        }

        /// <summary>
        /// remove punctuation from text.
        /// </summary>
        public static string RemovePunctuation(this string text)
        {
            return string.IsNullOrWhiteSpace(text) ?
                string.Empty :
                new string(text.Where(c => !char.IsPunctuation(c)).ToArray());
        }

        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }
    }
}