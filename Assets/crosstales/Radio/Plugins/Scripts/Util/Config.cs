namespace Crosstales.Radio.Util
{
    public static class Config
    {

        #region Changable variables

        /// <summary>Path to the asset inside the Unity project.</summary>
        public static string ASSET_PATH = Constants.DEFAULT_ASSET_PATH;

        /// <summary>Enable or disable debug logging for the asset.</summary>
        public static bool DEBUG = Constants.DEFAULT_DEBUG;

        /// <summary>Enable or disable update-checks for the asset.</summary>
        public static bool UPDATE_CHECK = Constants.DEFAULT_UPDATE_CHECK;

        /// <summary>Open the UAS-site when an update is found.</summary>
        public static bool UPDATE_OPEN_UAS = Constants.DEFAULT_UPDATE_OPEN_UAS;

        /// <summary>Enable or disable reminder-checks for the asset.</summary>
        public static bool REMINDER_CHECK = Constants.DEFAULT_REMINDER_CHECK;

        /// <summary>Enable or disable anonymous telemetry data.</summary>
        public static bool TELEMETRY = Constants.DEFAULT_TELEMETRY;

        /// <summary>Don't destroy the objects during scene switches.</summary>
        public static bool DONT_DESTROY_ON_LOAD = Constants.DEFAULT_DONT_DESTROY_ON_LOAD;

        /// <summary>Automatically load and add the prefabs to the scene.</summary>
        public static bool PREFAB_AUTOLOAD = Constants.DEFAULT_PREFAB_AUTOLOAD;

        /// <summary>Enable or disable the icon in the hierarchy.</summary>
        public static bool HIERARCHY_ICON = Constants.DEFAULT_HIERARCHY_ICON;

        /// <summary>Default bitrate for a RadioPlayer.</summary>
        public static int DEFAULT_BITRATE = Constants.DEFAULT_DEFAULT_BITRATE;

        /// <summary>Default chunk-size for a RadioPlayer.</summary>
        public static int DEFAULT_CHUNKSIZE = Constants.DEFAULT_DEFAULT_CHUNKSIZE;

        /// <summary>Default buffer-size for a RadioPlayer.</summary>
        public static int DEFAULT_BUFFERSIZE = Constants.DEFAULT_DEFAULT_BUFFERSIZE;

        /// <summary>Default cachestream-size for a RadioPlayer.</summary>
        public static int DEFAULT_CACHESTREAMSIZE = Constants.DEFAULT_DEFAULT_CACHESTREAMSIZE;

        /// <summary>Maximal cachestream-size for a RadioPlayer.</summary>
        public static int MAX_CACHESTREAMSIZE = Constants.DEFAULT_MAX_CACHESTREAMSIZE;

        /// <summary>Is the configuration loaded?</summary>
        public static bool isLoaded = false;

        #endregion

/*
        #region Constructor

        static Config()
        {
            if (!isLoaded) {
                Load();

                if (DEBUG)
                    UnityEngine.Debug.Log("Config data loaded");
            }
        }

        #endregion
*/

        #region Properties

        /// <summary>Returns the path of the prefabs.</summary>
        /// <returns>The path of the prefabs.</returns>
        public static string PREFAB_PATH
        {
            get
            {
                return ASSET_PATH + Constants.PREFAB_SUBPATH;
            }
        }

        #endregion


        #region Public static methods

        /// <summary>Resets all changable variables to their default value.</summary>
        public static void Reset()
        {
            ASSET_PATH = Constants.DEFAULT_ASSET_PATH;
            DEBUG = Constants.DEFAULT_DEBUG;
            UPDATE_CHECK = Constants.DEFAULT_UPDATE_CHECK;
            UPDATE_OPEN_UAS = Constants.DEFAULT_UPDATE_OPEN_UAS;
            REMINDER_CHECK = Constants.DEFAULT_REMINDER_CHECK;
            TELEMETRY = Constants.DEFAULT_TELEMETRY;
            DONT_DESTROY_ON_LOAD = Constants.DEFAULT_DONT_DESTROY_ON_LOAD;
            PREFAB_AUTOLOAD = Constants.DEFAULT_PREFAB_AUTOLOAD;

            HIERARCHY_ICON = Constants.DEFAULT_HIERARCHY_ICON;

            DEFAULT_BITRATE = Constants.DEFAULT_DEFAULT_BITRATE;
            DEFAULT_CHUNKSIZE = Constants.DEFAULT_DEFAULT_CHUNKSIZE;
            DEFAULT_BUFFERSIZE = Constants.DEFAULT_DEFAULT_BUFFERSIZE;
            DEFAULT_CACHESTREAMSIZE = Constants.DEFAULT_DEFAULT_CACHESTREAMSIZE;
            MAX_CACHESTREAMSIZE = Constants.DEFAULT_MAX_CACHESTREAMSIZE;
        }

        /// <summary>Loads all changable variables.</summary>
        public static void Load()
        {
            if (CTPlayerPrefs.HasKey(Constants.KEY_ASSET_PATH))
            {
                ASSET_PATH = CTPlayerPrefs.GetString(Constants.KEY_ASSET_PATH);
            }

            if (CTPlayerPrefs.HasKey(Constants.KEY_DEBUG))
            {
                DEBUG = CTPlayerPrefs.GetBool(Constants.KEY_DEBUG);
            }

            if (CTPlayerPrefs.HasKey(Constants.KEY_UPDATE_CHECK))
            {
                UPDATE_CHECK = CTPlayerPrefs.GetBool(Constants.KEY_UPDATE_CHECK);
            }

            if (CTPlayerPrefs.HasKey(Constants.KEY_UPDATE_OPEN_UAS))
            {
                UPDATE_OPEN_UAS = CTPlayerPrefs.GetBool(Constants.KEY_UPDATE_OPEN_UAS);
            }

            if (CTPlayerPrefs.HasKey(Constants.KEY_REMINDER_CHECK))
            {
                REMINDER_CHECK = CTPlayerPrefs.GetBool(Constants.KEY_REMINDER_CHECK);
            }

            if (CTPlayerPrefs.HasKey(Constants.KEY_TELEMETRY))
            {
                TELEMETRY = CTPlayerPrefs.GetBool(Constants.KEY_TELEMETRY);
            }

            //if (CTPlayerPrefs.HasKey(Constants.KEY_DONT_DESTROY_ON_LOAD))
            //{
            //    DONT_DESTROY_ON_LOAD = CTPlayerPrefs.GetBool(Constants.KEY_DONT_DESTROY_ON_LOAD);
            //}

            if (CTPlayerPrefs.HasKey(Constants.KEY_PREFAB_AUTOLOAD))
            {
                PREFAB_AUTOLOAD = CTPlayerPrefs.GetBool(Constants.KEY_PREFAB_AUTOLOAD);
            }

            if (CTPlayerPrefs.HasKey(Constants.KEY_HIERARCHY_ICON))
            {
                HIERARCHY_ICON = CTPlayerPrefs.GetBool(Constants.KEY_HIERARCHY_ICON);
            }

            if (CTPlayerPrefs.HasKey(Constants.KEY_DEFAULT_BITRATE))
            {
                DEFAULT_BITRATE = CTPlayerPrefs.GetInt(Constants.KEY_DEFAULT_BITRATE);
            }

            if (CTPlayerPrefs.HasKey(Constants.KEY_DEFAULT_CHUNKSIZE))
            {
                DEFAULT_CHUNKSIZE = CTPlayerPrefs.GetInt(Constants.KEY_DEFAULT_CHUNKSIZE);
            }

            if (CTPlayerPrefs.HasKey(Constants.KEY_DEFAULT_BUFFERSIZE))
            {
                DEFAULT_BUFFERSIZE = CTPlayerPrefs.GetInt(Constants.KEY_DEFAULT_BUFFERSIZE);
            }

            if (CTPlayerPrefs.HasKey(Constants.KEY_DEFAULT_CACHESTREAMSIZE))
            {
                DEFAULT_CACHESTREAMSIZE = CTPlayerPrefs.GetInt(Constants.KEY_DEFAULT_CACHESTREAMSIZE);
            }

            if (CTPlayerPrefs.HasKey(Constants.KEY_MAX_CACHESTREAMSIZE))
            {
                MAX_CACHESTREAMSIZE = CTPlayerPrefs.GetInt(Constants.KEY_MAX_CACHESTREAMSIZE);
            }

            isLoaded = true;
        }

        /// <summary>Saves all changable variables.</summary>
        public static void Save()
        {
            CTPlayerPrefs.SetString(Constants.KEY_ASSET_PATH, ASSET_PATH);
            CTPlayerPrefs.SetBool(Constants.KEY_DEBUG, DEBUG);
            CTPlayerPrefs.SetBool(Constants.KEY_UPDATE_CHECK, UPDATE_CHECK);
            CTPlayerPrefs.SetBool(Constants.KEY_UPDATE_OPEN_UAS, UPDATE_OPEN_UAS);
            CTPlayerPrefs.SetBool(Constants.KEY_REMINDER_CHECK, REMINDER_CHECK);
            CTPlayerPrefs.SetBool(Constants.KEY_TELEMETRY, TELEMETRY);
            //CTPlayerPrefs.SetBool(Constants.KEY_DONT_DESTROY_ON_LOAD, DONT_DESTROY_ON_LOAD);
            CTPlayerPrefs.SetBool(Constants.KEY_PREFAB_AUTOLOAD, PREFAB_AUTOLOAD);

            CTPlayerPrefs.SetBool(Constants.KEY_HIERARCHY_ICON, HIERARCHY_ICON);

            CTPlayerPrefs.SetInt(Constants.KEY_DEFAULT_BITRATE, DEFAULT_BITRATE);
            CTPlayerPrefs.SetInt(Constants.KEY_DEFAULT_CHUNKSIZE, DEFAULT_CHUNKSIZE);
            CTPlayerPrefs.SetInt(Constants.KEY_DEFAULT_BUFFERSIZE, DEFAULT_BUFFERSIZE);
            CTPlayerPrefs.SetInt(Constants.KEY_DEFAULT_CACHESTREAMSIZE, DEFAULT_CACHESTREAMSIZE);
            CTPlayerPrefs.SetInt(Constants.KEY_MAX_CACHESTREAMSIZE, MAX_CACHESTREAMSIZE);

            CTPlayerPrefs.Save();
        }

        #endregion
    }
}
// © 2017 crosstales LLC (https://www.crosstales.com)