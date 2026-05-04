    using System.Text.RegularExpressions;


namespace SOUPIShared.Extensions
{
    public static class StringExtensions
    {
        public static bool IsValidCommitHash(this string? input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Count() != 40) return false;

            Regex Sha1Regex = new Regex(@"\b[0-9a-f]{40}\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            return Sha1Regex.IsMatch(input);
        }

        public static bool IsValidGitHubUsername(this string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            // ^[a-z\d] start with alphanumeric
            // (?:[a-z\d]|-(?=[a-z\d])) allows alphanumeric or a hyphen followed by alphanumeric (prevents double hyphens and ending hyphens)
            // {0,38} total length between 1 and 39 (1 char for start + 38 more)
            // $ end of string
            string pattern = @"^[a-z\d](?:[a-z\d]|-(?=[a-z\d])){0,38}$";

            return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase);
        }

        public static bool IsValidGitHubRepositoryName(this string? input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Length > 100) return false;

            // no reserved names 
            if (input.Trim() == "." || input.Trim() == "..") return false;

            // ^ start of string
            // (?!.*\.git$) negative lookahead (ensure string doesn't end with ".git")
            // [a-zA-Z0-9._-]+ match one or more allowed characters
            // $ end of string
            string pattern = @"^(?!.*\.git$)[a-zA-Z0-9._-]+$";

            return Regex.IsMatch(input, pattern);
        }

        public static bool DoesConsistOfNumbersCyrillicLatin(this string? input)
        {
            if (string.IsNullOrEmpty(input)) return true;

            const string cyrillic = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
            const string latin = "abcdefghijklmnopqrstuvwxyz";
            const string numbers = "0123456789";
            const string extras = " ,.!?()[]-_";
            var allowedSet = (cyrillic + latin + numbers + extras).ToHashSet();

            foreach (var c in input)
            {
                if (!allowedSet.Contains(char.ToLower(c)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
