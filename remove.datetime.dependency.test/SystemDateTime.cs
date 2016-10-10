namespace MyServices
{
    using System;

    /// <summary>
    /// Helper class that will return DateTime.Now, but can be changed to deal with tests.
    /// https://ayende.com/blog/3408/dealing-with-time-in-tests
    /// </summary>
    public static class SystemDateTime
    {
        
        /// <summary>
        /// Gets a function that when evaluated returns a local date and time.   
        /// </summary>
        /// <returns>A function that when evaluated returns a local date and time.</returns>
        public static Func<DateTime> Now = () => DateTime.Now;
    }
}