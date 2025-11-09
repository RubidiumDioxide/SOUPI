namespace SOUPIShared.Exceptions
{
    public class SoupiException : Exception
    {
        public SoupiException()
        {
        }

        public SoupiException(string message)
            : base(message)
        {
        }

        public SoupiException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
