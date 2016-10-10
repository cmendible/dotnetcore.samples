namespace MyServices
{
    using System;

    /// <summary>
    /// A time dependent on DateTime.Now 
    /// </summary>
    public class TimeDependentService
    {

        /// <summary>
        /// Tells if it's morning.
        /// </summary>
        /// <returns>true if it's before 12</returns>
        public bool OldIsMorning()
        {
            return DateTime.Now.Hour < 12;
        }

        /// <summary>
        /// Tells if it's morning using the helper by Ayende. 
        /// </summary>
        /// <returns>true if it's before 12</returns>
        public bool IsMorning()
        {
            return SystemDateTime.Now().Hour < 12;
        }
    }
}