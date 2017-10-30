﻿using UnityEditor;
using Crosstales.Radio.EditorUtil;

namespace Crosstales.Radio.EditorTask
{
    /// <summary>Gather some telemetry data for the asset.</summary>
    [InitializeOnLoad]
    public static class Telemetry
    {
        #region Constructor

        static Telemetry()
        {
            string lastDate = string.Empty;
            if (Util.CTPlayerPrefs.HasKey(Util.Constants.KEY_TELEMETRY_DATE))
            {
                lastDate = Util.CTPlayerPrefs.GetString(Util.Constants.KEY_TELEMETRY_DATE);
            }
            //string lastDate = EditorPrefs.GetString(Util.Constants.KEY_TELEMETRY_DATE);

            string date = System.DateTime.Now.ToString("yyyyMMdd"); // every day
            //string date = System.DateTime.Now.ToString("yyyyMMddHHmm"); // every minute (for tests)

            if (!date.Equals(lastDate))
            {
                GAApi.Event(typeof(Telemetry).Name, "Startup");

                Util.CTPlayerPrefs.SetString(Util.Constants.KEY_TELEMETRY_DATE, date);
                //EditorPrefs.SetString(Util.Constants.KEY_TELEMETRY_DATE, date);
            }
        }

        #endregion

    }
}
// © 2017 crosstales LLC (https://www.crosstales.com)