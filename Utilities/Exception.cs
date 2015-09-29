namespace IBM.Watson.Utilities
{
    class Exception : System.Exception
    {
        public Exception(string message) : base(message)
        {
            // TODO: hook up logging
        }
        public Exception(string message, Exception innerException ) : base(message, innerException )
        {
            // TODO: hook up logging
        }
    }
}
