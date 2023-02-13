namespace Framework.Logging
{
    public interface ILogReader<T> where T : class
    {
        IList<Log<T>> Read(DateTime date);

        IList<Log<T>> Read(DateTime date, LogLevel level);

        IList<Log<T>> Read(string category, DateTime date);

        IList<Log<T>> Debug(DateTime date);
        IList<Log<T>> Info(DateTime date);
        IList<Log<T>> Warning(DateTime date);
        IList<Log<T>> Trace(DateTime date);
        IList<Log<T>> Error(DateTime date);
        IList<Log<T>> Fatal(DateTime date);


        IList<Log<T>> Read(DateTime fromDate, DateTime toDate);

        IList<Log<T>> Read(DateTime fromDate, DateTime toDate, LogLevel level);

        IList<Log<T>> Debug(DateTime fromDate, DateTime toDate);
        IList<Log<T>> Info(DateTime fromDate, DateTime toDate);
        IList<Log<T>> Warning(DateTime fromDate, DateTime toDate);
        IList<Log<T>> Trace(DateTime fromDate, DateTime toDate);
        IList<Log<T>> Error(DateTime fromDate, DateTime toDate);

        IList<Log<T>> Fatal(DateTime fromDate, DateTime toDate);




    }
}