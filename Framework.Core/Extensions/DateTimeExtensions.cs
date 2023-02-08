namespace Framework.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime DateTimeConvertion(this DateTime dateTime, string format)
        {
            try
            {
                DateTime DateValue = DateTime.Parse(dateTime.ToString(format));
                return DateValue;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}