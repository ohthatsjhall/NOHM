using UnityEngine;
using System.Linq;

namespace Crosstales.Radio.Util
{
    /// <summary>Various helper functions.</summary>
    public static class Helper
    {

        #region Static variables

        private static readonly System.Text.RegularExpressions.Regex lineEndingsRegex = new System.Text.RegularExpressions.Regex(@"\r\n|\r|\n");

        private static readonly int[] mp3Bitrates = new int[]
      {
         32,
         40,
         48,
         56,
         64,
         80,
         96,
         112,
         128,
         160,
         192,
         224,
         256,
         320
      };

        private static readonly int[] oggBitrates = new int[]
        {
         32,
         45,
         48,
         64,
         80,
         96,
         112,
         128,
         160,
         192,
         224,
         256,
         320,
         500
      };

        #endregion


        #region Static properties

        /// <summary>Checks if the current platform is Windows.</summary>
        /// <returns>True if the current platform is Windows.</returns>
        public static bool isWindowsPlatform
        {
            get
            {
                return Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;
            }
        }

        /// <summary>Checks if the current platform is macOS.</summary>
        /// <returns>True if the current platform is macOS.</returns>
        public static bool isMacOSPlatform
        {
            get
            {
#if UNITY_5_4_OR_NEWER
                return Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor;
#else
                return Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXDashboardPlayer;
#endif
            }
        }

        /// <summary>Checks if the current platform is Linux.</summary>
        /// <returns>True if the current platform is Linux.</returns>
        public static bool isLinuxPlatform
        {
            get
            {
#if UNITY_5_5_OR_NEWER
                    return Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.LinuxEditor;
#else
                return Application.platform == RuntimePlatform.LinuxPlayer;
#endif
            }
        }


        /// <summary>Checks if the current platform is standalone (Windows, macOS or Linux).</summary>
        /// <returns>True if the current platform is standalone (Windows, macOS or Linux).</returns>
        public static bool isStandalonePlatform
        {
            get
            {
                return isWindowsPlatform || isMacOSPlatform || isLinuxPlatform;
            }
        }

        /// <summary>Checks if the current platform is Android.</summary>
        /// <returns>True if the current platform is Android.</returns>
        public static bool isAndroidPlatform
        {
            get
            {
                return Application.platform == RuntimePlatform.Android;
            }
        }

        /// <summary>Checks if the current platform is iOS.</summary>
        /// <returns>True if the current platform is iOS.</returns>
        public static bool isIOSPlatform
        {
            get
            {
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
                return Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.tvOS;
#else
                return Application.platform == RuntimePlatform.IPhonePlayer;
#endif
            }
        }

        /// <summary>Checks if the current platform is WSA.</summary>
        /// <returns>True if the current platform is WSA.</returns>
        public static bool isWSAPlatform
        {
            get
            {
                return Application.platform == RuntimePlatform.WSAPlayerARM ||
                    Application.platform == RuntimePlatform.WSAPlayerX86 ||
                    Application.platform == RuntimePlatform.WSAPlayerX64 ||
#if !UNITY_5_4_OR_NEWER
                    Application.platform == RuntimePlatform.WP8Player ||
#endif
#if !UNITY_5_5_OR_NEWER
                    Application.platform == RuntimePlatform.XBOX360 ||
#endif
                    Application.platform == RuntimePlatform.XboxOne;
            }
        }

        /// <summary>Checks if the current platform is WebGL.</summary>
        /// <returns>True if the current platform is WebGL.</returns>
        public static bool isWebGLPlatform
        {
            get
            {
                return Application.platform == RuntimePlatform.WebGLPlayer;
            }
        }

        /// <summary>Checks if the current platform is WebPlayer.</summary>
        /// <returns>True if the current platform is WebPlayer.</returns>
        public static bool isWebPlayerPlatform
        {
            get
            {
#if UNITY_5_4_OR_NEWER
                return false;
#else
                return Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer;
#endif
            }
        }

        /// <summary>Checks if the current platform is Web (WebPlayer or WebGL).</summary>
        /// <returns>True if the current platform is Web (WebPlayer or WebGL).</returns>
        public static bool isWebPlatform
        {
            get
            {
                return isWebPlayerPlatform || isWebGLPlatform;
            }
        }

        /// <summary>Checks if the current platform is Windows-based (Windows standalone or WSA).</summary>
        /// <returns>True if the current platform is Windows-based (Windows standalone or WSA).</returns>
        public static bool isWindowsBasedPlatform
        {
            get
            {
                return isWindowsPlatform || isWSAPlatform;
            }
        }

        /// <summary>Checks if the current platform is Apple-based (macOS standalone or iOS).</summary>
        /// <returns>True if the current platform is Apple-based (macOS standalone or iOS).</returns>
        public static bool isAppleBasedPlatform
        {
            get
            {
                return isMacOSPlatform || isIOSPlatform;
            }
        }

        /// <summary>Checks if we are inside the Editor.</summary>
        /// <returns>True if we are inside the Editor.</returns>
        public static bool isEditor
        {
            get
            {
#if UNITY_5_5_OR_NEWER
                return Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.LinuxEditor;
#else
                return Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor;
#endif
            }
        }

        /// <summary>Checks if we are in Editor mode.</summary>
        /// <returns>True if in Editor mode.</returns>
        public static bool isEditorMode
        {
            get
            {
                return isEditor && !Application.isPlaying;
            }
        }

        /// <summary>Checks if the current platform is supported.</summary>
        /// <returns>True if the current platform is supported.</returns>
        public static bool isSupportedPlatform
        {
            get
            {
                //return WindowsPlatform();
                return true;
            }
        }

        #endregion


        #region Public methods

#if !UNITY_WSA || UNITY_EDITOR
        /// <summary>HTTPS-certification callback.</summary>
        public static bool RemoteCertificateValidationCallback(System.Object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;

#if UNITY_5_4_OR_NEWER
        // If there are errors in the certificate chain, look at each error to determine the cause.
        if (sslPolicyErrors != System.Net.Security.SslPolicyErrors.None)
        {
        for (int i = 0; i < chain.ChainStatus.Length; i++)
        {
        if (chain.ChainStatus[i].Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.RevocationStatusUnknown)
        {
        chain.ChainPolicy.RevocationFlag = System.Security.Cryptography.X509Certificates.X509RevocationFlag.EntireChain;
        chain.ChainPolicy.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.Online;
        chain.ChainPolicy.UrlRetrievalTimeout = new System.TimeSpan(0, 1, 0);
        chain.ChainPolicy.VerificationFlags = System.Security.Cryptography.X509Certificates.X509VerificationFlags.AllFlags;

        isOk = chain.Build((System.Security.Cryptography.X509Certificates.X509Certificate2)certificate);
        }
        }
        }
#endif

            return isOk;
        }
#endif

        /// <summary>Checks if the given RadioStation is sane.</summary>
        /// <returns>True if the given RadioStation is sane.</returns>
        public static bool isSane(ref Model.RadioStation station)
        {
            if (station == null)
            {
                Debug.LogError("'Station' is null!" + System.Environment.NewLine + "Add a valid station to the player.");

                return false;
            }

            //         if (string.IsNullOrEmpty(station.Name)) {
            //            Debug.LogWarning(station + Environment.NewLine + "'Name' is null or empty!" + Environment.NewLine + "To identiy your various radios, it's recommended to add a name.");
            //         }

            if (string.IsNullOrEmpty(station.Url))
            {
                Debug.LogError(station + System.Environment.NewLine + "'Url' is null or empty!" + System.Environment.NewLine + "Cannot start playback -> please add a valid url of a radio station (see 'Radios.txt' for some examples).");

                return false;
            }

			if (!isValidURL(station.Url))
			{
				Debug.LogError(station + System.Environment.NewLine + "'Url' is not valid!" + System.Environment.NewLine + "Cannot start playback -> please add a valid url of a radio station (see 'Radios.txt' for some examples).");

				return false;
			}

            if (!isValidFormat(station.Format))
            {
                Debug.LogError(station + System.Environment.NewLine + "'Format' is invalid: '" + station.Format + "'" + System.Environment.NewLine + "Cannot start playback -> please add a valid audio format for a radio station (see 'Radios.txt' for some examples).");

                return false;
            }

            //         if (string.IsNullOrEmpty(station.Station)) {
            //            Debug.LogWarning(station + Environment.NewLine + "'Station' is null or empty!" + Environment.NewLine + "To identiy your various radios, it's recommended to add a name for the station.");
            //         }
            //
            //         if (string.IsNullOrEmpty(station.Genre)) {
            //            Debug.LogWarning(station + Environment.NewLine + "'Genere' is null or empty!" + Environment.NewLine + "To identiy your various radios, it's recommended to add a genere for the station.");
            //         }

            if (!isValidBitrate(station.Bitrate, station.Format))
            {
                Debug.LogWarning(station + System.Environment.NewLine + "'Bitrate' is invalid: " + station.Bitrate + System.Environment.NewLine + "Autmatically using " + Config.DEFAULT_BITRATE + " kbit/s for playback.");
                station.Bitrate = Config.DEFAULT_BITRATE;
            }

            if (station.ChunkSize < 1)
            {
                Debug.LogWarning(station + System.Environment.NewLine + "'ChunkSize' is smaller than 1KB!" + System.Environment.NewLine + "Autmatically using " + Config.DEFAULT_CHUNKSIZE + "KB for playback.");
                station.ChunkSize = Config.DEFAULT_CHUNKSIZE;
            }

            if (station.Format == Model.Enum.AudioFormat.MP3)
            {
                if (station.BufferSize < Config.DEFAULT_BUFFERSIZE / 4)
                {
                    Debug.LogWarning(station + System.Environment.NewLine + "'BufferSize' is smaller than DEFAULT_BUFFERSIZE/4!" + System.Environment.NewLine + "Autmatically using " + (Config.DEFAULT_BUFFERSIZE / 4) + "KB for playback.");
                    station.BufferSize = Config.DEFAULT_BUFFERSIZE / 4;
                }
            }
            else
            {
                if (station.BufferSize < Constants.MIN_OGG_BUFFERSIZE)
                {
                    //Debug.LogWarning(station + Environment.NewLine + "'BufferSize' is smaller than MIN_OGG_BUFFERSIZE!" + Environment.NewLine + "Autmatically using " + Constants.MIN_OGG_BUFFERSIZE + "KB for playback.");
                    station.BufferSize = Constants.MIN_OGG_BUFFERSIZE;
                }
            }

            if (station.BufferSize < station.ChunkSize)
            {
                Debug.LogWarning(station + System.Environment.NewLine + "'BufferSize' is smaller than 'ChunkSize'!" + System.Environment.NewLine + "Autmatically using " + station.ChunkSize + "KB for playback.");
                station.BufferSize = station.ChunkSize;
            }

            return true;
        }

        /// <summary>Validates a given path and add missing slash.</summary>
        /// <param name="path">Path to validate</param>
        /// <returns>Valid path</returns>
        public static string ValidatePath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                string pathTemp = path.Trim();
                string result = null;

                if (isWindowsPlatform)
                {
                    result = pathTemp.Replace('/', '\\');

                    if (!result.EndsWith(Constants.PATH_DELIMITER_WINDOWS))
                    {
                        result += Constants.PATH_DELIMITER_WINDOWS;
                    }
                }
                else
                {
                    result = pathTemp.Replace('\\', '/');

                    if (!result.EndsWith(Constants.PATH_DELIMITER_UNIX))
                    {
                        result += Constants.PATH_DELIMITER_UNIX;
                    }
                }

                return result;
            }

            return path;
        }

        /// <summary>Validates a given file.</summary>
        /// <param name="path">File to validate</param>
        /// <returns>Valid file path</returns>
        public static string ValidateFile(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {

                string result = ValidatePath(path);

                if (result.EndsWith(Constants.PATH_DELIMITER_WINDOWS) || result.EndsWith(Constants.PATH_DELIMITER_UNIX))
                {
                    result = result.Substring(0, result.Length - 1);
                }

                return result;
            }

            return path;
        }

        /// <summary>Cleans a given URL.
        /// <param name="url">URL to clean</param>
        /// <param name="removeProtocol">Remove the protocol, e.g. http:// (default: true, optional).</param>
        /// <param name="removeWWW">Remove www (default: true, optional).</param>
        /// <param name="removeSlash">Remove slash at the end (default: true, optional)</param>
        /// <returns>Clean URL</returns>
        public static string CleanUrl(string url, bool removeProtocol = true, bool removeWWW = true, bool removeSlash = true)
        {
            string result = url.Trim();

            if (!string.IsNullOrEmpty(url))
            {
                if (removeProtocol)
                {
                    result = result.Substring(result.IndexOf("//") + 2);
                }

                if (removeProtocol)
                {
                    result = result.CTReplace("www.", string.Empty);
                }

                if (removeSlash && result.EndsWith(Constants.PATH_DELIMITER_UNIX))
                {
                    result = result.Substring(0, result.Length - 1);
                }

                /*
                if (urlTemp.StartsWith("http://"))
                {
                    result = urlTemp.Substring(7);
                }
                else if (urlTemp.StartsWith("https://"))
                {
                    result = urlTemp.Substring(8);
                }
                else
                {
                    result = urlTemp;
                }

                if (result.StartsWith("www."))
                {
                    result = result.Substring(4);
                }
                */
            }

            return result;
        }

        /// <summary>Split the given text to lines and return it as list.</summary>
        /// <param name="text">Complete text fragment</param>
        /// <param name="ignoreCommentedLines">Ignore commente lines (default: true, optional)</param>
        /// <param name="skipHeaderLines">Number of skipped header lines (default: 0, optional)</param>
        /// <param name="skipFooterLines">Number of skipped footer lines (default: 0, optional)</param>
        /// <returns>Splitted lines as array</returns>
        public static System.Collections.Generic.List<string> SplitStringToLines(string text, bool ignoreCommentedLines = true, int skipHeaderLines = 0, int skipFooterLines = 0)
        {
            System.Collections.Generic.List<string> result = new System.Collections.Generic.List<string>(100);

            if (string.IsNullOrEmpty(text))
            {
                Debug.LogWarning("Parameter 'text' is null or empty!" + System.Environment.NewLine + "=> 'SplitStringToLines()' will return an empty string list.");
            }
            else
            {
                string[] lines = lineEndingsRegex.Split(text);

                for (int ii = 0; ii < lines.Length; ii++)
                {
                    if (ii + 1 > skipHeaderLines && ii < lines.Length - skipFooterLines)
                    {
                        if (!string.IsNullOrEmpty(lines[ii]))
                        {
                            if (ignoreCommentedLines)
                            {
                                if (!lines[ii].StartsWith("#", System.StringComparison.OrdinalIgnoreCase))
                                { //valid and not disabled line?
                                    result.Add(lines[ii]);
                                }
                            }
                            else
                            {
                                result.Add(lines[ii]);
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>Format byte-value to Human-Readable-Form.</summary>
        /// <param name="bytes">Value in bytes</param>
        /// <returns>Formatted byte-value in Human-Readable-Form.</returns>
        public static string FormatBytesToHRF(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            return System.String.Format("{0:0.##} {1}", len, sizes[order]);
        }

        /// <summary>Returns a HH:mm:ss-formatted string from seconds.</summary>
        /// <param name="seconds">Value in seconds</param>
        /// <returns>HH:mm:ss-formatted string from seconds.</returns>
        public static string FormatSecondsToHourMinSec(double seconds)
        {
            int totalSeconds = (int)seconds;
            int calcSeconds = totalSeconds % 60;

            if (seconds >= 3600)
            {
                int calcHours = totalSeconds / 3600;
                int calcMinutes = (totalSeconds - calcHours * 3600) / 60;

                return calcHours + ":" + (calcMinutes < 10 ? "0" + calcMinutes.ToString() : calcMinutes.ToString()) + ":" + (calcSeconds < 10 ? "0" + calcSeconds.ToString() : calcSeconds.ToString());
            }
            else
            {
                int calcMinutes = totalSeconds / 60;

                return calcMinutes + ":" + (calcSeconds < 10 ? "0" + calcSeconds.ToString() : calcSeconds.ToString());
            }
        }

        /// <summary>Converts a given byte-array to a float-array.</summary>
        /// <param name="bytes">byte-array to convert</param>
        /// <param name="count">Number of bytes to convert</param>
        /// <returns>Converted float-array.</returns>
        public static float[] ConvertByteArrayToFloatArray(byte[] bytes, int count)
        {
            //float[] floats = new float[bytes.Length / 4];
            //System.Buffer.BlockCopy(bytes, 0, floats, 0, bytes.Length);

            float[] floats = new float[count / 2];

            int ii = 0;
            for (int zz = 0; zz < count; zz += 2)
            {
                floats[ii] = bytesToFloat(bytes[zz], bytes[zz + 1]);
                ii++;
            }

            return floats;
        }

        /// <summary>Converts a given float-array to a byte-array.</summary>
        /// <returns>Converted byte-array.</returns>
        /// <param name="floats">float-array to convert</param>
        /// <param name="count">Number of floats to convert</param>
        public static byte[] ConvertFloatArrayToByteArray(float[] floats, int count)
        {
            var bytes = new byte[count * 2];
            int sampleIndex = 0;
            int byteIndex = 0;

            while (sampleIndex < count)
            {

                var outsample = (short)(floats[sampleIndex] * short.MaxValue);

                bytes[byteIndex] = (byte)(outsample & 0xff);

                bytes[byteIndex + 1] = (byte)((outsample >> 8) & 0xff);

                sampleIndex++;
                byteIndex += 2;
            }

            return bytes;
        }

        /// <summary>
        /// Generate nice HSV colors.
        /// Based on https://gist.github.com/rje/6206099
        /// </summary>
        /// <param name="h">Hue</param>
        /// <param name="s">Saturation</param>
        /// <param name="v">Value</param>
        /// <param name="a">Alpha (optional)</param>
        /// <returns>True if the current platform is supported.</returns>
        public static Color HSVToRGB(float h, float s, float v, float a = 1f)
        {
            if (s == 0f)
            {
                return new Color(v, v, v, a);
            }

            h /= 60f;
            var sector = Mathf.FloorToInt(h);
            var fact = h - sector;
            var p = v * (1f - s);
            var q = v * (1f - s * fact);
            var t = v * (1 - s * (1 - fact));

            switch (sector)
            {
                case 0:
                    return new Color(v, t, p, a);
                case 1:
                    return new Color(q, v, p, a);
                case 2:
                    return new Color(p, v, t, a);
                case 3:
                    return new Color(p, q, v, a);
                case 4:
                    return new Color(t, p, v, a);
                default:
                    return new Color(v, p, q, a);
            }
        }

        /// <summary>Converts a string to an AudioFormat. If the format couldn't be determined, the method returns AudioFormat.MP3.</summary>
        /// <param name="format">Audio format as string to convert</param>
        /// <returns>Converted AudioFormat.</returns>
        public static Model.Enum.AudioFormat AudioFormatFromString(string format)
        {
            //Model.Enum.AudioFormat result = Model.Enum.AudioFormat.UNKNOWN;
            Model.Enum.AudioFormat result = Model.Enum.AudioFormat.MP3; //set MP3 as default!

            if (!string.IsNullOrEmpty(format))
            {
                if (format.CTEquals("ogg"))
                {
                    result = Model.Enum.AudioFormat.OGG;
                }
            }
            else
            {
                //Debug.LogWarning("'format' is null or empty! Could not determine audio format!");
            }
            return result;
        }

        /// <summary>Converts a string to an AudioCodec. If the codec couldn't be determined, the method returns AudioCodec.None.</summary>
        /// <param name="codec">Audio codec as string to convert</param>
        /// <returns>Converted AudioCodec.</returns>
        public static Model.Enum.AudioCodec AudioCodecFromString(string codec)
        {
            Model.Enum.AudioCodec result = Model.Enum.AudioCodec.None; //set NONE as default!

            if (!string.IsNullOrEmpty(codec))
            {
                if (codec.CTEquals("MP3_NLayer"))
                {
                    result = Model.Enum.AudioCodec.MP3_NLayer;
                }
                else if (codec.CTEquals("MP3_NAudio"))
                {
                    result = Model.Enum.AudioCodec.MP3_NAudio;
                }
                else if (codec.CTEquals("OGG_NVorbis"))
                {
                    result = Model.Enum.AudioCodec.OGG_NVorbis;
                }
                else if (codec.CTEquals("None"))
                {
                    //result = Model.Enum.AudioCodec.OGG_NVorbis;
                }
                else
                {
                    Debug.LogWarning("Could not determine audio codec: " + codec);
                }
            }
            else
            {
                //Debug.LogWarning("'codec' is null or empty! Could not determine audio codec!");
            }
            return result;
        }

        /// <summary>Converts an AudioFormat to an AudioCodec for the current platform. If the codec couldn't be determined, the method returns AudioCodec.None.</summary>
        /// <param name="format">AudioFormat to conver</param>
        /// <returns>Converted AudioCodec.</returns>
        public static Model.Enum.AudioCodec AudioCodecForAudioFormat(Model.Enum.AudioFormat format)
        {
            if (format == Model.Enum.AudioFormat.MP3)
            {
                if (Helper.isWindowsPlatform)
                {
                    return Constants.DEFAULT_CODEC_MP3_WINDOWS;
                }
                else
                {
                    return Model.Enum.AudioCodec.MP3_NLayer;
                }
            }
            else if (format == Model.Enum.AudioFormat.OGG)
            {
                return Model.Enum.AudioCodec.OGG_NVorbis;
            }

            return Model.Enum.AudioCodec.None;
        }

        //      /// <summary>Converts a string to an data format. If the format couldn't be determined, the method returns 'Format.Stream'.</summary>
        //      /// <param name="format">Audio format as string</param>
        //      /// <returns>Converted audio format.</returns>
        //      public static AudioFormat DataFormatFromString(string format) {
        //         //Format result = Format.UNKNOWN;
        //         AudioFormat result = AudioFormat.MP3; //set MP3 as default!
        //
        //         if (!string.IsNullOrEmpty(format)) {
        //            if (format.CTContains("ogg")) {
        //               result = AudioFormat.OGG;
        //            }
        //         } else {
        //            //Debug.LogWarning("'format' is null or empty! Could not determine audio format!");
        //         }
        //         return result;
        //      }

        /// <summary>Checks if an AudioFormat is valid.</summary>
        /// <param name="format">AudioFormat to check</param>
        /// <returns>True if the AudioFormat is valid.</returns>
        public static bool isValidFormat(Model.Enum.AudioFormat format)
        {
            return (format == Model.Enum.AudioFormat.MP3 || format == Model.Enum.AudioFormat.OGG);
        }

        /// <summary>Returns the nearest bitrate for a given value and an AudioFormat.</summary>
        /// <param name="bitrate">Bitrate value as base value for the bitrate</param>
        /// <param name="format">AudioFormat for the bitrate definition</param>
        /// <returns>The nearest bitrate for the given value and AudioFormat.</returns>
        public static int NearestBitrate(int bitrate, Model.Enum.AudioFormat format)
        {
            int result = 128;

            if (format == Model.Enum.AudioFormat.MP3)
            {
                result = NearestMP3Bitrate(bitrate);
            }
            else if (format == Model.Enum.AudioFormat.OGG)
            {
                result = NearestOGGBitrate(bitrate);
            }
            else
            {
                //Debug.LogWarning("The format was invalid: " + format);
            }

            return result;
        }

        /// <summary>Returns the nearest bitrate for a given value and MP3.</summary>
        /// <param name="bitrate">Bitrate value as base value for the bitrate</param>
        /// <returns>The nearest bitrate for the given value and MP3.</returns>
        public static int NearestMP3Bitrate(int bitrate)
        {
            return mp3Bitrates.Aggregate((x, y) => System.Math.Abs(x - bitrate) < System.Math.Abs(y - bitrate) ? x : y);
        }

        /// <summary>Returns the nearest bitrate for a given value and OGG.</summary>
        /// <param name="bitrate">Bitrate value as base value for the bitrate</param>
        /// <returns>The nearest bitrate for the given value and OGG.</returns>
        public static int NearestOGGBitrate(int bitrate)
        {
            return oggBitrates.Aggregate((x, y) => System.Math.Abs(x - bitrate) < System.Math.Abs(y - bitrate) ? x : y);
        }


        /// <summary>Checks if a bitrate for an AudioFormat is valid.</summary>
        /// <param name="bitrate">Bitrate to check</param>
        /// <param name="format">AudioFormat to check</param>
        /// <returns>True if the bitrate for the AudioFormat is valid.</returns>
        public static bool isValidBitrate(int bitrate, Model.Enum.AudioFormat format)
        {
            bool result = false;

            if (format == Model.Enum.AudioFormat.MP3)
            {
                result = isValidMP3Bitrate(bitrate);
            }
            else if (format == Model.Enum.AudioFormat.OGG)
            {
                result = isValidOGGBitrate(bitrate);
            }
            else
            {
                //Debug.LogWarning("The format was invalid: " + format);
            }

            return result;
        }

        /// <summary>Checks if the MP3 bitrate is valid.</summary>
        /// <param name="bitrate">Bitrate to check</param>
        /// <returns>True if the MP3 bitrate is valid.</returns>
        public static bool isValidMP3Bitrate(int bitrate)
        {
            return mp3Bitrates.Contains(bitrate);
        }

        /// <summary>Checks if the OGG bitrate is valid.</summary>
        /// <param name="bitrate">Bitrate to check</param>
        /// <returns>True if the OGG bitrate is valid.</returns>
        public static bool isValidOGGBitrate(int bitrate)
        {
            return oggBitrates.Contains(bitrate);
        }

		/// <summary>Checks if the URL is valid.</summary>
		/// <param name="url">URL to check</param>
		/// <returns>True if the URL is valid.</returns>
		public static bool isValidURL(string url)
		{
			return string.IsNullOrEmpty(url) ? false :  url.StartsWith (Constants.PREFIX_FILE) || url.StartsWith (Constants.PREFIX_HTTP) || url.StartsWith (Constants.PREFIX_HTTPS);
		}

        #endregion


        #region Private methods

        private static float bytesToFloat(byte firstByte, byte secondByte)
        {
            // convert two bytes to one short (little endian) and convert it to range from -1 to (just below) 1
            return ((short)((secondByte << 8) | firstByte)) / Constants.FLOAT_32768;
        }

        #endregion
    }
}
// © 2015-2017 crosstales LLC (https://www.crosstales.com)