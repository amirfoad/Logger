namespace Framework.Logging
{
    public interface ILogger<T> where T : class
    {
        Task Info(T message, string traceId, string sectionId, string serviceId, string category = "");

        Task Log(string level, T message, string traceId, string sectionId, string serviceId, string category = "");

        Task Debug(T message, string traceId, string sectionId, string serviceId, string category = "");

        Task Trace(T message, string traceId, string sectionId, string serviceId, string category = "");

        Task Warning(T message, string traceId, string sectionId, string serviceId, string category = "");

        Task Error(T message, string traceId, string sectionId, string serviceId, string category = "");

        Task Fatal(T message, string traceId, string sectionId, string serviceId, string category = "");
    }
}