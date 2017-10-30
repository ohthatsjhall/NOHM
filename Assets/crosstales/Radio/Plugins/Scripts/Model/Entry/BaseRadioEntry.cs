using UnityEngine;

namespace Crosstales.Radio.Model.Entry
{
    /// <summary>Base class for radio entries.</summary>
    [System.Serializable]
    public abstract class BaseRadioEntry
    {

        #region Variables

        [Header("General Settings")]
        /// <summary>Name of the file or radio station.</summary>
        [Tooltip("Name of the radio station.")]
        public string Name;

        /// <summary>Force the name of the station to this name (default: false).</summary>
        [Tooltip("Force the name of the station to this name (default: false).")]
        public bool ForceName = false;

        /// <summary>Enable the source in this provider (default: true).</summary>
        [Tooltip("Enable the source in this provider (default: true).")]
        public bool EnableSource = true;

        [Header("Meta Data")]
        /// <summary>Provider of the radio stations (optional).</summary>
        [Tooltip("Provider of the radio stations (optional).")]
        public string Station;

        /// <summary>Genres of the radios (optional).</summary>
        [Tooltip("Genres of the radios (optional).")]
        public string Genres;

        /// <summary>Your rating of the radios.</summary>
        [Tooltip("Your rating of the radios.")]
        [Range(0, 5f)]
        public float Rating;

        /// <summary>Description of the radio stations (optional).</summary>
        [Tooltip("Description of the radio stations (optional).")]
        [Multiline]
        public string Description;

        /// <summary>Icon to represent the radio stations (optional).</summary>
        [Tooltip("Icon to represent the radio stations (optional).")]
        public Sprite Icon;


        [Header("Stream Settings")]
        /// <summary>Default audio format of the stations (default: AudioFormat.MP3).</summary>
        [Tooltip("Default audio format of the stations (default: AudioFormat.MP3).")]
        public Enum.AudioFormat Format = Enum.AudioFormat.MP3;

        /// <summary>Default bitrate in kbit/s (default: 128).</summary>
        [Tooltip("Default bitrate in kbit/s (default: 128).")]
        public int Bitrate = 128; // Util.Config.DEFAULT_BITRATE;

        /// <summary>Default size of the streaming-chunk in KB (default: 32).</summary>
        [Tooltip("Default size of the streaming-chunk in KB (default: 32).")]
        public int ChunkSize = 32; // Util.Config.DEFAULT_CHUNKSIZE;

        /// <summary>Default size of the local buffer in KB (default: 48).</summary>
        [Tooltip("Default size of the local buffer in KB (default: 48).")]
        public int BufferSize = 48; // Util.Config.DEFAULT_BUFFERSIZE;


        [Header("Codec Settings")]
        /// <summary>Exclude this station if the current RadioPlayer codec is equals this one (default: AudioCodec.None).</summary>
        [Tooltip("Exclude this station if the current RadioPlayer codec is equals this one (default: AudioCodec.None).")]
        public Enum.AudioCodec ExcludedCodec = Enum.AudioCodec.None;

        /// <summary>Is this entry initalized?.</summary>
        [HideInInspector]
        public bool isInitalized = false;

        #endregion


        #region Constructors

        /// <summary>Instantiate the class (default).</summary>
        public BaseRadioEntry()
        {
            //default
        }

        /// <summary>Instantiate the class.</summary>
        /// <param name="name">Name of the radio station.</param>
        /// <param name="forceName">Force the name of the station to this name.</param>
        /// <param name="enableSource">Enable the source in this provider.</param>
        /// <param name="station">Name of the station.</param>
        /// <param name="genres">Genres of the radio.</param>
        /// <param name="rating">Your rating of the radio.</param>
        /// <param name="desc">Description of the radio station.</param>
        /// <param name="icon">Icon of the radio station.</param>
        /// <param name="format">AudioFormat of the station.</param>
        /// <param name="bitrate">Bitrate in kbit/s.</param>
        /// <param name="chunkSize">Size of the streaming-chunk in KB.</param>
        /// <param name="bufferSize">Size of the local buffer in KB.</param>
        /// <param name="excludeCodec">Excluded codec.</param>
        public BaseRadioEntry(string name, bool forceName, bool enableSource, string station, string genres, float rating, string desc, Sprite icon, Enum.AudioFormat format, int bitrate, int chunkSize, int bufferSize, Enum.AudioCodec excludeCodec)
        {
            Name = name;
            ForceName = forceName;
            EnableSource = enableSource;
            Station = station;
            Genres = genres;
            Rating = rating;
            Description = desc;
            Icon = icon;
            Format = format;
            Bitrate = bitrate;
            ChunkSize = chunkSize;
            BufferSize = bufferSize;
            ExcludedCodec = excludeCodec;
        }

        #endregion


        #region Overridden methods

        public override string ToString()
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();

            result.Append(GetType().Name);
            result.Append(Util.Constants.TEXT_TOSTRING_START);

            result.Append("Name='");
            result.Append(Name);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("ForceName='");
            result.Append(ForceName);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("EnableSource='");
            result.Append(EnableSource);
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

            result.Append("isInitalized='");
            result.Append(isInitalized);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER_END);

            result.Append(Util.Constants.TEXT_TOSTRING_END);

            return result.ToString();
        }

        #endregion
    }
}
// © 2016-2017 crosstales LLC (https://www.crosstales.com)