namespace Framework.Core
{
    public interface IValueValidator<in TValue>
    {
        #region Methods

        /// <summary>
        /// Determines whether the specified value is valid.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if is valid, otherwise <c>false</c>.
        /// </returns>
        bool IsValid(TValue @value);

        #endregion Methods
    }
}