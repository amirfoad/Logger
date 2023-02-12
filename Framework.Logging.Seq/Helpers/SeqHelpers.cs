using System.Reflection;
using Utf8Json;

namespace Framework.Logging.Seq
{
    internal static class SeqHelpers
    {
        internal static string GenerateLogMessage(this object message, int messageNumber, string level = "Information")
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>
            {
                {
                    "@t",
                    DateTime.Now.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz")
                },
                {
                    "@i",
                    ++messageNumber
                },
                {
                    "@l",
                    level
                }
            };
            if (message is string)
            {
                dictionary.Add("@m", message);
            }
            else
            {
                PropertyInfo[] properties = message.GetType().GetProperties();
                foreach (PropertyInfo propertyInfo in properties)
                {
                    if (propertyInfo.GetIndexParameters().Length != 0)
                    {
                        continue;
                    }

                    object value = propertyInfo.GetValue(message);
                    if (value != null)
                    {
                        switch (propertyInfo.Name.ToLower())
                        {
                            case "message":
                            case "msg":
                            case "m":
                                dictionary.Add((value is string) ? "@m" : propertyInfo.Name, value);
                                break;

                            case "e":
                                dictionary.Add("@x", JsonSerializer.ToJsonString(value));
                                break;

                            case "messagetemplate":
                            case "mt":
                                dictionary.Add("@mt", value);
                                break;

                            default:
                                dictionary.Add(propertyInfo.Name, value);
                                break;
                        }
                    }
                }
            }

            return JsonSerializer.ToJsonString(dictionary);
        }

        internal static string NormalizeServerBaseAddress(this string serverUrl)
        {
            string text = serverUrl;
            if (!text.EndsWith("/"))
            {
                text += "/";
            }

            return text;
        }
    }
}