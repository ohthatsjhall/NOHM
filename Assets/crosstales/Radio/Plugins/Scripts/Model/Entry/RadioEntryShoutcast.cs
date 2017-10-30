using UnityEngine;

namespace Crosstales.Radio.Model.Entry
{
    /// <summary>Model for a Shoutcast entry.</summary>
    [System.Serializable]
    public class RadioEntryShoutcast : BaseRadioEntry
    {

        #region Variables

        [Header("Source Settings")]
        /// <summary>Shoutcast-ID for the radio.</summary>
        [Tooltip("Shoutcast-ID for the radio.")]
        public string ShoutcastID;

        #endregion


        #region Constructor

        /// <summary>Instantiate the class (default).</summary>
        /// <param name="entry">RadioStation as base.</param>
        /// <param name="shoutcastID">Shoutcast-ID from the radio station.</param>
        public RadioEntryShoutcast(RadioStation entry, string shoutcastID) : base(entry.Name, true, true, entry.Station, entry.Genres, entry.Rating, entry.Description, entry.Icon, entry.Format, entry.Bitrate, entry.ChunkSize, entry.BufferSize, entry.ExcludedCodec)
        {
            ShoutcastID = shoutcastID;
        }

        #endregion


        #region Overridden methods

        public override string ToString()
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder(base.ToString());

            result.Append(GetType().Name);
            result.Append(Util.Constants.TEXT_TOSTRING_START);

            result.Append("ShoutcastID='");
            result.Append(ShoutcastID);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER_END);

            result.Append(Util.Constants.TEXT_TOSTRING_END);

            return result.ToString();
        }

        #endregion
    }
}
// © 2016-2017 crosstales LLC (https://www.crosstales.com)