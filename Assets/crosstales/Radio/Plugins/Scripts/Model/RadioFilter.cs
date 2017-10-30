using UnityEngine;

namespace Crosstales.Radio.Model
{
    /// <summary>Filter for radio stations.</summary>
    [System.Serializable]
    public class RadioFilter
    {

        #region Variables

        /// <summary>Part of the radio names.</summary>
        [Tooltip("Part of the radio names.")]
        public string Name;

        /// <summary>Part of the radio URL.</summary>
        [Tooltip("Part of the radio URL.")]
        public string Url;

        /// <summary>Part of the radio stations.</summary>
        [Tooltip("Part of the radio stations.")]
        public string Station;

        /// <summary>Part of the radio genres.</summary>
        [Tooltip("Part of the radio genres.")]
        public string Genres;

        /// <summary>Minimal rating (default: 0).</summary>
        [Tooltip("Minimal rating (default: 0).")]
        public float RatingMin = 0f;

        /// <summary>Maximal rating (default: 5).</summary>
        [Tooltip("Maximal rating (default: 5).")]
        public float RatingMax = 5f;

        /// <summary>Part of the radio formats.</summary>
        [Tooltip("Part of the radio formats.")]
        public string Format;

        /// <summary>Minimal bitrate in kbit/s (default: 32).</summary>
        [Tooltip("Minimal bitrate in kbit/s (default: 32).")]
        public int BitrateMin = 32;

        /// <summary>Maximal bitrate in kbit/s (default: 500).</summary>
        [Tooltip("Maximal bitrate in kbit/s (default: 500).")]
        public int BitrateMax = 500;

        /// <summary>Exclude radio stations with unsupported codecs (default: true).</summary>
        [Tooltip("Exclude radio stations with unsupported codecs (default: true).")]
        public bool ExcludeUnsupportedCodecs = true;

        #endregion


        #region Properties

        /// <summary>Are filter parameters set and active?</summary>
        /// <returns>True if filter parameters are set and active.</returns>
        public bool isFiltering
        {
            get
            {
                return !string.IsNullOrEmpty(Name) ||
                    !string.IsNullOrEmpty(Url) ||
                    !string.IsNullOrEmpty(Station) ||
                    !string.IsNullOrEmpty(Genres) ||
                    RatingMin > 0f ||
                    RatingMax < 5f ||
                    !string.IsNullOrEmpty(Format) ||
                    BitrateMin > 32 ||
                    BitrateMax < 500 ||
                    !ExcludeUnsupportedCodecs;
            }
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

            result.Append("Url='");
            result.Append(Url);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("Station='");
            result.Append(Station);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("Genre='");
            result.Append(Genres);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("RatingMin='");
            result.Append(RatingMin);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("RatingMax='");
            result.Append(RatingMax);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("Format='");
            result.Append(Format);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("BitrateMin='");
            result.Append(BitrateMin);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("BitrateMax='");
            result.Append(BitrateMax);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("ExcludeUnsupportedCodecs='");
            result.Append(ExcludeUnsupportedCodecs);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER_END);

            result.Append(Util.Constants.TEXT_TOSTRING_END);

            return result.ToString();
        }

        #endregion
    }
}
// © 2016-2017 crosstales LLC (https://www.crosstales.com)