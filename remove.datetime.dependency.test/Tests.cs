namespace Tests
{
    using System;
    using MyServices;
    using Xunit;

    public class Tests
    {
        /// <summary>
        /// If this test is run after noon it will fail cause of the dependency on the system time.
        /// </summary>
        [Fact]
        public void Will_Fail_After_Noon()
        {
            var svc = new TimeDependentService();
            Assert.True(svc.OldIsMorning());
        }

        /// <summary>
        /// This test will run OK no matter the systems time, cause we are using the Ayende 
        /// approach to tests with time dependencies.
        /// </summary>
        [Fact]
        public void Will_Run_OK_No_Matter_The_Real_Time()
        {
            // Force the time to 11 AM
            SystemDateTime.Now = () => new DateTime(2016, 10, 10, 11, 0, 0);
            var svc = new TimeDependentService();
            Assert.True(svc.IsMorning());
        }
    }
}