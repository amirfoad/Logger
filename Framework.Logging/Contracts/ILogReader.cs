namespace Framework.Logging
{
    public interface ILogReader<T> where T : class
    {
        Task<IList<Log<T>>> Read(DateTime date);

        Task<IList<Log<T>>> Read(DateTime date, LogLevel level);

        Task<IList<Log<T>>> Read(string category, DateTime date);

        Task<IList<Log<T>>> Debug(DateTime date);

        Task<IList<Log<T>>> Info(DateTime date);

        Task<IList<Log<T>>> Warning(DateTime date);

        Task<IList<Log<T>>> Trace(DateTime date);

        Task<IList<Log<T>>> Error(DateTime date);

        Task<IList<Log<T>>> Fatal(DateTime date);

        Task<IList<Log<T>>> Read(DateTime fromDate, DateTime toDate);

        Task<IList<Log<T>>> Read(DateTime fromDate, DateTime toDate, LogLevel level);

        Task<IList<Log<T>>> Debug(DateTime fromDate, DateTime toDate);

        Task<IList<Log<T>>> Info(DateTime fromDate, DateTime toDate);

        Task<IList<Log<T>>> Warning(DateTime fromDate, DateTime toDate);

        Task<IList<Log<T>>> Trace(DateTime fromDate, DateTime toDate);

        Task<IList<Log<T>>> Error(DateTime fromDate, DateTime toDate);

        Task<IList<Log<T>>> Fatal(DateTime fromDate, DateTime toDate);
    }
}