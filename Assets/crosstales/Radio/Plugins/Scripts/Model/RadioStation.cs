using UnityEngine;

namespace Crosstales.Radio.Model
{
    /// <summary>Model for a radio station.</summary>
    [System.Serializable]
    public class RadioStation
    {
        #region Variables

        [Header("Station Settings")]
        /// <summary>Name of the radio station.</summary>
        [Tooltip("Name of the radio station.")]
        public string Name;

        /// <summary>URL of the station.</summary>
        [Tooltip("URL of the station.")]
        public string Url;

        [Header("Meta Data")]
        /// <summary>Name of the station.</summary>
        [Tooltip("Name of the station.")]
        public string Station;

        /// <summary>Genres of the radio.</summary>
        [Tooltip("Genres of the radio.")]
        public string Genres;

        /// <summary>Your rating of the radio.</summary>
        [Tooltip("Your rating of the radio.")]
        [Range(0, 5f)]
        public float Rating;

        /// <summary>Description of the radio station.</summary>
        [Tooltip("Description of the radio station.")]
        [Multiline]
        public string Description;

        /// <summary>Icon to represent the radio station.</summary>
        [Tooltip("Icon to represent the radio station.")]
        public Sprite Icon;


        [Header("Stream Settings")]
        /// <summary>Audio format of the station (default: AudioFormat.MP3).</summary>
        [Tooltip("Audio format of the station.")]
        public Enum.AudioFormat Format = Enum.AudioFormat.MP3;

        /// <summary>Bitrate in kbit/s (default: 128).</summary>
        [Tooltip("Bitrate in kbit/s (default: 128).")]
        public int Bitrate = Util.Config.DEFAULT_BITRATE;

        /// <summary>Size of the streaming-chunk in KB (default: 32).</summary>
        [Tooltip("Size of the streaming-chunk in KB (default: 32).")]
        public int ChunkSize = Util.Config.DEFAULT_CHUNKSIZE;

        /// <summary>Size of the local buffer in KB (default: 48).</summary>
        [Tooltip("Size of the local buffer in KB (default: 48).")]
        public int BufferSize = Util.Config.DEFAULT_BUFFERSIZE;


        [Header("Codec Settings")]
        /// <summary>Exclude this station if the current RadioPlayer codec is equals this one (default: AudioCodec.None).</summary>
        [Tooltip("Exclude this station if the current RadioPlayer codec is equals this one (default: AudioCodec.None).")]
        public Enum.AudioCodec ExcludedCodec = Enum.AudioCodec.None;

        /// <summary>Total downloaded data size in bytes.</summary>
        [HideInInspector]
        public long TotalDataSize = 0;

        /// <summary>Total number of data requests.</summary>
        [HideInInspector]
        public int TotalDataRequests = 0;

        /// <summary>Total playtime in seconds.</summary>
        [HideInInspector]
        public float TotalPlayTime = 0;

        /// <summary>List of all played records.</summary>
        //[HideInInspector]
        public readonly System.Collections.Generic.List<RecordInfo> PlayedRecords = new System.Collections.Generic.List<RecordInfo>();

        private const char splitCharText = ';';

        #endregion


        #region Constructors

        /// <summary>Instantiate the class (default).</summary>
        public RadioStation()
        {
            //empty     
        }

        /// <summary>Instantiate the class.</summary>
        /// <param name="name">Name of the radio station.</param>
        /// <param name="url">Stream-URL of the station.</param>
        /// <param name="format">AudioFormat of the station.</param>
        public RadioStation(string name, string url, Enum.AudioFormat format)
        {
            Name = name.CTToTitleCase();
            Url = url;
            Format = format;
        }

        /// <summary>Instantiate the class.</summary>
        /// <param name="name">Name of the radio station.</param>
        /// <param name="url">Stream-URL of the station.</param>
        /// <param name="format">AudioFormat of the station.</param>
        /// <param name="station">Name of the station.</param>
        /// <param name="genres">Genres of the radio.</param>
        /// <param name="bitrate">Bitrate in kbit/s.</param>
        /// <param name="rating">Your rating of the radio.</param>
        /// <param name="description">Description of the radio station.</param>
        /// <param name="icon">Icon of the radio station.</param>
        /// <param name="chunkSize">Size of the streaming-chunk in KB (default: 64, optional).</param>
        /// <param name="bufferSize">Size of the local buffer in KB (default: 64, optional).</param>
        /// <param name="excludeCodec">Excluded codec (default: AudioCodec.NONE, optional).</param>
        public RadioStation(string name, string url, Enum.AudioFormat format, string station, string genres, int bitrate, float rating, string description, Sprite icon, int chunkSize = 64, int bufferSize = 64, Enum.AudioCodec excludeCodec = Enum.AudioCodec.None) : this(name, url, format)
        {
            Station = station;
            Genres = genres;
            Bitrate = bitrate;
            Rating = rating;
            Description = description;
            Icon = icon;
            ChunkSize = chunkSize;
            BufferSize = bufferSize;
            ExcludedCodec = excludeCodec;
        }

        #endregion


        #region Public methods

        /// <summary>ToString()-variant for exporting the object.</summary>
        /// <param name="detailed">Detailed export with Chunk- and Buffer-size.</param>
        /// <returns>Text-line of the object.</returns>
        public string ToTextLine(bool detailed = false)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();

            result.Append(Name);
            result.Append(splitCharText);

            result.Append(Url);
            result.Append(splitCharText);

            result.Append("Stream");
            result.Append(splitCharText);

            result.Append(Format);
            result.Append(splitCharText);

            result.Append(Station);
            result.Append(splitCharText);

            result.Append(Genres);
            result.Append(splitCharText);

            result.Append(Bitrate);
            result.Append(splitCharText);

            result.Append(Rating);
            result.Append(splitCharText);

            result.Append(Description);
            result.Append(splitCharText);

            result.Append(ExcludedCodec);

            if (detailed)
            {
                result.Append(splitCharText);

                result.Append(ChunkSize);
                result.Append(splitCharText);

                result.Append(BufferSize);
            }

            return result.ToString();
        }

        /// <summary>ToString()-variant for displaying the object in the Editor.</summary>
        /// <returns>Text description of the object.</returns>
        public string ToShortString()
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();

            result.Append("Name='");
            result.Append(Name);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("Url='");
            result.Append(Url);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("Station='");
            result.Append(Station);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("Genres='");
            result.Append(Genres);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("Rating='");
            result.Append(Rating);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("Description='");
            result.Append(Description);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("Icon='");
            result.Append(Icon);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("AudioFormat='");
            result.Append(Format);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("Bitrate='");
            result.Append(Bitrate);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("ChunkSize='");
            result.Append(ChunkSize);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("BufferSize='");
            result.Append(BufferSize);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("ExcludedCodec='");
            result.Append(ExcludedCodec);
            result.Append("'");

            return result.ToString();
        }

        #endregion


        #region Overridden methods

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            RadioStation p = obj as RadioStation;
            if ((System.Object)p == null)
            {
                return false;
            }

            //Debug.Log(Url + " - " + p.Url + ":" + (Url == p.Url));
            return (Url == p.Url);
        }

        public override int GetHashCode()
        {
            return this.Url.GetHashCode();
        }

        public override string ToString()
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();

            result.Append(GetType().Name);
            result.Append(Util.Constants.TEXT_TOSTRING_START);

            result.Append("Name='");
            result.Append(Name);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("Url='");
            result.Append(Url);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("Station='");
            result.Append(Station);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("Genres='");
            result.Append(Genres);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("Rating='");
            result.Append(Rating);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("Description='");
            result.Append(Description);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("Icon='");
            result.Append(Icon);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("AudioFormat='");
            result.Append(Format);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("Bitrate='");
            result.Append(Bitrate);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("ChunkSize='");
            result.Append(ChunkSize);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("BufferSize='");
            result.Append(BufferSize);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("ExcludedCodec='");
            result.Append(ExcludedCodec);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("TotalDataSize='");
            result.Append(TotalDataSize);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("TotalDataRequests='");
            result.Append(TotalDataRequests);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("TotalPlayTime='");
            result.Append(TotalPlayTime);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER_END);

            result.Append(Util.Constants.TEXT_TOSTRING_END);

            return result.ToString();
        }

        #endregion
    }
}
// © 2015-2017 crosstales LLC (https://www.crosstales.com)