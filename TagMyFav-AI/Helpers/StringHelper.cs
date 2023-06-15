using System.Text.RegularExpressions;

namespace TagMyFav_AI.Helpers
{
    public class StringHelper
    {
        public static string RemoveHtmlTags(string input, string appName)
        {
            // Remove HTML tags using regular expressions
            string result = Regex.Replace(input, "<.*?>", string.Empty);
            result = result.Replace(appName, string.Empty);
            result = result.Trim();
            return result;
        }
        public static string TakeJsonResult(string inputText)
        {
            if (string.IsNullOrEmpty(inputText))
                return string.Empty;
            inputText = inputText.Trim();
            if (inputText.StartsWith("Result:"))
            {
                inputText = inputText.Replace("Result:", string.Empty);
            }
            return inputText;
        }
    }
}
