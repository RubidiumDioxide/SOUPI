using System.Text.RegularExpressions;


namespace SOUPIShared.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex Sha1Regex = new Regex(@"\b[0-9a-f]{40}\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool IsValidCommitHash(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;
            return Sha1Regex.IsMatch(input);
        }
    }
}
