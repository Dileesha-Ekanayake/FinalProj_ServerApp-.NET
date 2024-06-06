using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;

namespace ServerApp.Util
{
    public class RegexProvider
    {
        public static Dictionary<string, Dictionary<string, string>> Get<T>(T t)
        {
            Dictionary<string, Dictionary<string, string>> DcRegexes = new Dictionary<string, Dictionary<string, string>>();
            try
            {
                PropertyInfo[] properties = typeof(T).GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    RegularExpressionAttribute regexAttribute = (RegularExpressionAttribute)Attribute.GetCustomAttribute(property, typeof(RegularExpressionAttribute));

                    if (regexAttribute != null)
                    {
                        string propertyName = property.Name.ToLower();
                        string regex = regexAttribute.Pattern;
                        string msg = regexAttribute.ErrorMessage;

                        Dictionary<string, string> rgxvalues = new Dictionary<string, string>();
                        rgxvalues.Add("regex", regex);
                        rgxvalues.Add("message", msg);
                        DcRegexes.Add(propertyName, rgxvalues);
                    }
                }
                return DcRegexes;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                return null;
            }
        }
    }
}
