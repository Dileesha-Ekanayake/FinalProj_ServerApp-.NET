namespace ServerApp.Util
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PatternAttribute : Attribute
    {
        public string Pattern { get; }
        public string ErrorMessage { get; }

        public PatternAttribute(string pattern, string errorMessage)
        {
            Pattern = pattern;
            ErrorMessage = errorMessage;
        }
    }
}
