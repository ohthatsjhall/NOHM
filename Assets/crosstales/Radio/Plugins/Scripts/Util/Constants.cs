namespace Crosstales.Radio.Util
{
    /// <summary>Collected constants of very general utility for the asset.</summary>
    public static class Constants
    {

        #region Constant variables

        /// <summary>Is PRO-version?</summary>
        public static readonly bool isPro = true;

        /// <summary>Name of the asset.</summary>
        public const string ASSET_NAME = "Radio PRO";
        //public const string ASSET_NAME = "Radio";

        /// <summary>Version of the asset.</summary>
        public const string ASSET_VERSION = "2.8.5";

        /// <summary>Build number of the asset.</summary>
        public const int ASSET_BUILD = 285;

        /// <summary>Create date of the asset (YYYY, MM, DD).</summary>
        public static readonly System.DateTime ASSET_CREATED = new System.DateTime(2015, 2, 25);

        /// <summary>Change date of the asset (YYYY, MM, DD).</summary>
        public static readonly System.DateTime ASSET_CHANGED = new System.DateTime(2017, 10, 19);

        /// <summary>Author of the asset.</summary>
        public const string ASSET_AUTHOR = "crosstales LLC";

        /// <summary>URL of the asset author.</summary>
        public const string ASSET_AUTHOR_URL = "https://www.crosstales.com";

        /// <summary>URL of the crosstales assets in UAS.</summary>
        public const string ASSET_CT_URL = "https://www.assetstore.unity3d.com/#!/list/42213-crosstales?aid=1011lNGT&pubref=" + ASSET_NAME; // crosstales list

        /// <summary>URL of the PRO asset in UAS.</summary>
        public const string ASSET_PRO_URL = "https://www.assetstore.unity3d.com/#!/content/32034?aid=1011lNGT&pubref=" + ASSET_NAME;

        /// <summary>URL of the 3rd party assets in UAS.</summary>
        public const string ASSET_3P_URL = "https://www.assetstore.unity3d.com/#!/list/42211-radio-friends?aid=1011lNGT&pubref=" + ASSET_NAME; // Radio&Friends list

        /// <summary>URL for update-checks of the asset</summary>
        public const string ASSET_UPDATE_CHECK_URL = "https://www.crosstales.com/media/assets/radio_versions.txt";

        /// <summary>Contact to the owner of the asset.</summary>
        public const string ASSET_CONTACT = "radio@crosstales.com";

        /// <summary>URL of the asset manual.</summary>
        public const string ASSET_MANUAL_URL = "https://www.crosstales.com/media/data/assets/radio/Radio-doc.pdf";

        /// <summary>URL of the asset API.</summary>
        public const string ASSET_API_URL = "http://goo.gl/G0hu6n";
        //public const string ASSET_API_URL = "http://www.crosstales.com/en/assets/radio/api";

        /// <summary>URL of the asset forum.</summary>
        public const string ASSET_FORUM_URL = "http://goo.gl/HxgngH";
        //public const string ASSET_FORUM_URL = "http://forum.unity3d.com/threads/radio-mp3-and-ogg-streaming-solution.334604/";

        /// <summary>URL of the asset in crosstales.</summary>
        public const string ASSET_WEB_URL = "https://www.crosstales.com/en/portfolio/radio/";

        /// <summary>URL of the promotion video of the asset (Youtube).</summary>
        public const string ASSET_VIDEO_PROMO = "https://youtu.be/1ZsxY788w-w?list=PLgtonIOr6Tb41XTMeeZ836tjHlKgOO84S";

        /// <summary>URL of the tutorial video of the asset (Youtube).</summary>
        public const string ASSET_VIDEO_TUTORIAL = "https://youtu.be/E0s0NVRX-ec?list=PLgtonIOr6Tb41XTMeeZ836tjHlKgOO84S";

        /// <summary>URL of the crosstales Facebook-profile.</summary>
        public const string ASSET_SOCIAL_FACEBOOK = "https://www.facebook.com/crosstales/";

        /// <summary>URL of the crosstales Twitter-profile.</summary>
        public const string ASSET_SOCIAL_TWITTER = "https://twitter.com/crosstales";

        /// <summary>URL of the crosstales Youtube-profile.</summary>
        public const string ASSET_SOCIAL_YOUTUBE = "https://www.youtube.com/c/Crosstales";

        /// <summary>URL of the crosstales LinkedIn-profile.</summary>
        public const string ASSET_SOCIAL_LINKEDIN = "https://www.linkedin.com/company/crosstales";

        /// <summary>URL of the crosstales XING-profile.</summary>
        public const string ASSET_SOCIAL_XING = "https://www.xing.com/companies/crosstales";

        /// <summary>URL of the 3rd party asset "PlayMaker".</summary>
        public const string ASSET_3P_PLAYMAKER = "https://www.assetstore.unity3d.com/#!/content/368?aid=1011lNGT&pubref=" + ASSET_NAME;

        /// <summary>URL of the 3rd party asset "Audio Visualizer".</summary>
        public const string ASSET_3P_AUDIO_VISUALIZER = "https://www.assetstore.unity3d.com/#!/content/47866?aid=1011lNGT&pubref=" + ASSET_NAME;

        /// <summary>URL of the 3rd party asset "Complete Sound Suite".</summary>
        public const string ASSET_3P_SOUND_SUITE = "https://www.assetstore.unity3d.com/#!/content/19994?aid=1011lNGT&pubref=" + ASSET_NAME;

        /// <summary>URL of the 3rd party asset "Visualizer Studio".</summary>
        public const string ASSET_3P_VISUALIZER_STUDIO = "https://www.assetstore.unity3d.com/#!/content/1761?aid=1011lNGT&pubref=" + ASSET_NAME;

        /// <summary>Factor for kilo bytes.</summary>
        public const int FACTOR_KB = 1024;

        /// <summary>Factor for mega bytes.</summary>
        public const int FACTOR_MB = FACTOR_KB * 1024;

        /// <summary>Factor for giga bytes.</summary>
        public const int FACTOR_GB = FACTOR_MB * 1024;

        /// <summary>Float value of 32768.</summary>
        public const float FLOAT_32768 = 32768f;

        /// <summary>ToString for two decimal places.</summary>
        public const string FORMAT_TWO_DECIMAL_PLACES = "0.00";

        /// <summary>ToString for no decimal places.</summary>
        public const string FORMAT_NO_DECIMAL_PLACES = "0";

        /// <summary>ToString for percent.</summary>
        public const string FORMAT_PERCENT = "0%";

        /// <summary>URL of the Internet availability check for all systems.</summary>
        public const string INTERNET_CHECK_URL = "http://start.ubuntu.com/connectivity-check";

        /// <summary>URL of the Internet availability check for Windows-based systems.</summary>
        public const string INTERNET_CHECK_URL_WINDOWS = "http://www.msftncsi.com/ncsi.txt";

        /// <summary>URL of the Internet availability check for Apple-based systems.</summary>
        public const string INTERNET_CHECK_URL_APPLE = "https://www.apple.com/library/test/success.html";

        /// <summary>URL of the fall-back Internet availability check.</summary>
        public const string INTERNET_CHECK_URL_FALLBACK = "https://crosstales.com/media/downloads/up.txt";

        // Keys for the configuration of the asset
        private const string KEY_PREFIX = "RADIO_CFG_";
        public const string KEY_ASSET_PATH = KEY_PREFIX + "ASSET_PATH";
        public const string KEY_DEBUG = KEY_PREFIX + "DEBUG";
        public const string KEY_UPDATE_CHECK = KEY_PREFIX + "UPDATE_CHECK";
        public const string KEY_UPDATE_OPEN_UAS = KEY_PREFIX + "UPDATE_OPEN_UAS";
        public const string KEY_REMINDER_CHECK = KEY_PREFIX + "REMINDER_CHECK";
        public const string KEY_TELEMETRY = KEY_PREFIX + "TELEMETRY";
        public const string KEY_PREFAB_AUTOLOAD = KEY_PREFIX + "PREFAB_AUTOLOAD";

        public const string KEY_HIERARCHY_ICON = KEY_PREFIX + "HIERARCHY_ICON";

        public const string KEY_DEFAULT_BITRATE = KEY_PREFIX + "DEFAULT_BITRATE";
        public const string KEY_DEFAULT_CHUNKSIZE = KEY_PREFIX + "DEFAULT_CHUNKSIZE";
        public const string KEY_DEFAULT_BUFFERSIZE = KEY_PREFIX + "DEFAULT_BUFFERSIZE";
        public const string KEY_DEFAULT_CACHESTREAMSIZE = KEY_PREFIX + "DEFAULT_CACHESTREAMSIZE";
        public const string KEY_MAX_CACHESTREAMSIZE = KEY_PREFIX + "MAX_CACHESTREAMSIZE";

        public const string KEY_UPDATE_DATE = KEY_PREFIX + "UPDATE_DATE";

        public const string KEY_REMINDER_DATE = KEY_PREFIX + "REMINDER_DATE";
        public const string KEY_REMINDER_COUNT = KEY_PREFIX + "REMINDER_COUNT";

        public const string KEY_LAUNCH = KEY_PREFIX + "LAUNCH";

        public const string KEY_TELEMETRY_DATE = KEY_PREFIX + "TELEMETRY_DATE";

        // Default values
        public const string DEFAULT_ASSET_PATH = "/crosstales/Radio/";
        public const bool DEFAULT_DEBUG = false;
        public const bool DEFAULT_UPDATE_CHECK = true;
        public const bool DEFAULT_UPDATE_OPEN_UAS = false;
        public const bool DEFAULT_REMINDER_CHECK = true;
        public const bool DEFAULT_TELEMETRY = true;
        public const bool DEFAULT_DONT_DESTROY_ON_LOAD = true;
        public const bool DEFAULT_PREFAB_AUTOLOAD = false;

        public const bool DEFAULT_HIERARCHY_ICON = true;

        public const int DEFAULT_DEFAULT_BITRATE = 128;
        public const int DEFAULT_DEFAULT_CHUNKSIZE = 32;
        public const int DEFAULT_DEFAULT_BUFFERSIZE = 64;
        public const int DEFAULT_DEFAULT_CACHESTREAMSIZE = 512;
        public const int DEFAULT_MAX_CACHESTREAMSIZE = 262144;

        /// <summary>Minimal buffer-size for OGG-streams.</summary>
        public const int MIN_OGG_BUFFERSIZE = 64;

        /// <summary>Path delimiter for Windows.</summary>
        public const string PATH_DELIMITER_WINDOWS = @"\";

        /// <summary>Path delimiter for Unix.</summary>
        public const string PATH_DELIMITER_UNIX = "/";

        /// <summary>InternetCheck prefab scene name.</summary>
        public const string INTERNETCHECK_SCENE_OBJECT_NAME = "InternetCheck";

        /// <summary>Proxy prefab scene name.</summary>
        public const string SURVIVOR_SCENE_OBJECT_NAME = "SurviveSceneSwitch";

        /// <summary>Proxy prefab scene name.</summary>
        public const string PROXY_SCENE_OBJECT_NAME = "Proxy";

        #endregion


        #region Changable variables

        /// <summary>Development debug logging for the asset.</summary>
        public static bool DEV_DEBUG = false;

        // Technical settings
        /// <summary>Default MP3-codec.</summary>
        public static Model.Enum.AudioCodec DEFAULT_CODEC_MP3 = Model.Enum.AudioCodec.MP3_NLayer;

        /// <summary>Default MP3-codec under Windows.</summary>
        public static Model.Enum.AudioCodec DEFAULT_CODEC_MP3_WINDOWS = Model.Enum.AudioCodec.MP3_NAudio;
        //public static Model.Enum.AudioCodec DEFAULT_CODEC_MP3_WINDOWS = Model.Enum.AudioCodec.MP3_NLayer;

        /// <summary>URL for the Shoutcast-Query.</summary>
        public static string SHOUTCAST = "http://yp.shoutcast.com/sbin/tunein-station.pls?id=";

        /// <summary>Random wait time for co-routines in seconds.</summary>
        //public static float RANDOM_WAIT_TIME = 10f;

        /// <summary>Delay for Invoke-calls (typically between a "Stop"- and "Play"-call).</summary>
        public static float INVOKE_DELAY = 0.4f;

        /// <summary>Maximal load wait time in seconds.</summary>
        public static float MAX_LOAD_WAIT_TIME = 5f;

        /// <summary>Maximal load time for web resources in seconds.</summary>
        public static float MAX_WEB_LOAD_WAIT_TIME = 5f;

        /// <summary>Maximal load time for Shoutcast resources in seconds.</summary>
        public static float MAX_SHOUTCAST_LOAD_WAIT_TIME = 5f;

        /// <summary>Defines the speed of 'Play'-calls in seconds.</summary>
        public static float PLAY_CALL_SPEED = 0.6f;

        /// <summary>Sub-path to the prefabs.</summary>
        public static string PREFAB_SUBPATH = "Prefabs/";

        /// <summary>Minimal interval for the OGG clean in frames.</summary>
        public static int OGG_CLEAN_INTERVAL_MIN = 1000;

        /// <summary>Maximal interval for the OGG clean in frames.</summary>
        public static int OGG_CLEAN_INTERVAL_MAX = 5000;

        // Text fragments for the asset
        public static string TEXT_TOSTRING_END = "}";
        public static string TEXT_TOSTRING_DELIMITER = "', ";
        public static string TEXT_TOSTRING_DELIMITER_END = "'";
        public static string TEXT_TOSTRING_START = " {";
        public static string TEXT_BUFFER = "Buffer: ";
        //public static string TEXT_PLAY = "  Play";
        //public static string TEXT_NEXT = "  Next";
        //public static string TEXT_PREVIOUS = "  Previous";
        public static string TEXT_STOPPED = "stopped";
        public static string TEXT_QUESTIONMARKS = "???";

        // Prefixes for URLs and paths
        public static string PREFIX_HTTP = "http://";
        public static string PREFIX_HTTPS = "https://";
        //public static string PREFIX_FILE = "file:///";
        public static string PREFIX_FILE = "file://";
        //public static string PREFIX_PERSISTENT_DATA_PATH = Application.persistentDataPath;
        //public static string PREFIX_DATA_PATH = Application.dataPath;
        public static string PREFIX_TEMP_PATH = System.IO.Path.GetTempPath();

        #endregion


        #region Properties

        /// <summary>Returns the URL of the asset in UAS.</summary>
        /// <returns>The URL of the asset in UAS.</returns>
        public static string ASSET_URL
        {
            get
            {

                if (isPro)
                {
                    return ASSET_PRO_URL;
                }
                else
                {
                    return "https://www.assetstore.unity3d.com/#!/content/48398?aid=1011lNGT&pubref=" + ASSET_NAME;
                }
            }
        }

        /// <summary>Returns the UID of the asset.</summary>
        /// <returns>The UID of the asset.</returns>
        public static System.Guid ASSET_UID
        {
            get
            {
                if (isPro)
                {
                    return new System.Guid("a233f682-6ab9-408d-aef0-0dc71b27bbb1");
                }
                else
                {
                    return new System.Guid("0306683a-a9b0-4dd1-afe9-5df19d93afeb");
                }
            }
        }

        #endregion
    }
}
// © 2015-2017 crosstales LLC (https://www.crosstales.com)