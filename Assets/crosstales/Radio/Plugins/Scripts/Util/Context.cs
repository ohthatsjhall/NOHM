namespace Crosstales.Radio.Util
{
    /// <summary>Context for the asset.</summary>
    public static class Context
    {
        #region Changable variables

        /// <summary>Total downloaded data size in bytes for all radio stations.</summary>
        public static long TotalDataSize = 0;
        //public static volatile int TotalDataSize = 0;

        /// <summary>Total number of data requests for all radio stations.</summary>
        public static int TotalDataRequests = 0;
        //public static volatile int TotalDataRequests = 0;

        /// <summary>Total playtime in seconds for all radio stations.</summary>
        public static double TotalPlayTime = 0;
        //public static volatile float TotalPlayTime = 0;

        #endregion
    }
}
// © 2017 crosstales LLC (https://www.crosstales.com)