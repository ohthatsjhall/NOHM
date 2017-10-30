using UnityEngine;

namespace Crosstales.Radio.Model
{
    /// <summary>Contains information about the current audio record from a radio station (for Icecast-servers).</summary>
    [System.Serializable]
    public class RecordInfo
    {

        #region Variables

        /// <summary>Original, unparsed information as string.</summary>
        [Tooltip("Original, unparsed information as string.")]
        public string Info = string.Empty;

        /// <summary>Duration of the record in seconds (after playback).</summary>
        [HideInInspector]
        [Tooltip("Duration of the record in seconds (after playback).")]
        public float Duration = 0f;

        private string title;
        private string artist;
        private string streamTitle;
        private string streamUrl;
        private string spotifyUrl;
        //private string itunesUrl;

        private const string streamTitleString = "StreamTitle='";
        private const string streamUrlString = "StreamUrl='";

        private static readonly char[] splitChar = new char[] { '-' };

        private System.DateTime created = System.DateTime.Now;

        #endregion


        #region Properties

        /// <summary>Returns the title of the audio record.</summary>
        /// <returns>Title of the audio record.</returns>
        public string Title
        {
            get
            {
                if (title == null)
                {
                    parseStreamTitle();
                }

                return title;
            }
        }

        /// <summary>Returns the artist of the audio record.</summary>
        /// <returns>Artist of the audio record.</returns>
        public string Artist
        {
            get
            {
                if (artist == null)
                {
                    parseStreamTitle();
                }

                return artist;
            }
        }

        /// <summary>Returns the content of the 'StreamTitle'-tag.</summary>
        /// <returns>Content of the 'StreamTitle'-tag.</returns>
        public string StreamTitle
        {
            get
            {
                if (streamTitle == null)
                {
                    parseStreamTitle();
                }

                return streamTitle;
            }
        }


        /// <summary>Returns the content of the 'StreamUrl'-tag.</summary>
        /// <returns>Content of the 'StreamUrl'-tag.</returns>
        public string StreamUrl
        {
            get
            {
                if (streamUrl == null)
                {
                    parseStreamUrl();
                }

                return streamUrl;
            }
        }

        /// <summary>Returns the Spotify-url for the record.</summary>
        /// <returns>Spotify-url for the record.</returns>
        public string SpotifyUrl
        {
            get
            {
                if (spotifyUrl == null)
                {
                    parseSpotifyUrl();
                }

                return spotifyUrl;
            }
        }

        /*
        /// <summary>Returns the iTunes-url for the record.</summary>
        /// <returns>iTunes-url for the record.</returns>
        public string iTunesUrl
        {
            get
            {
                if (itunesUrl == null)
                {
                    parseITunesUrl();
                }

                return itunesUrl;
            }
        }
        */

        /// <summary>Returns the creation time of the RecordInfo.</summary>
        /// <returns>Creation time of the RecordInfo.</returns>
        public System.DateTime Created
        {
            get
            {
                return created;
            }
        }

        #endregion


        #region Constructors

        /// <summary>Instantiate the class (default).</summary>
        public RecordInfo()
        {
            //empty
        }

        /// <summary>Instantiate the class.</summary>
        /// <param name="info">Information as string.</param>
        public RecordInfo(string info)
        {
            Info = info.Trim();
            //Info = System.Text.RegularExpressions.Regex.Replace(info.Trim(), @"\s+", " ");
        }


        //      /// <summary>Instantiate the class (default).</summary>
        //      /// <param name="clone">Crate a new clone from the given instance.</param>
        //      public RecordInfo(RecordInfo clone) : this(clone.Info)
        //      {
        //          //empty
        //      }

        #endregion


        #region Public methods

        /// <summary>ToString()-variant for displaying the object in the Editor.</summary>
        /// <returns>Text description of the object.</returns>
        public string ToShortString()
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();

            result.Append("Title='");
            result.Append(Title);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("Artist='");
            result.Append(Artist);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("Duration='");
            result.Append(Duration);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("Created='");
            result.Append(Created);
            result.Append("'");

            return result.ToString();
        }

        #endregion


        #region Overridden methods

        public override string ToString()
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();

            result.Append(GetType().Name);
            result.Append(Util.Constants.TEXT_TOSTRING_START);

            result.Append("Info='");
            result.Append(Info);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("Duration='");
            result.Append(Duration);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("Title='");
            result.Append(Title);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("Artist='");
            result.Append(Artist);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("StreamTitle='");
            result.Append(StreamTitle);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("StreamUrl='");
            result.Append(StreamUrl);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("SpotifyUrl='");
            result.Append(SpotifyUrl);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("Created='");
            result.Append(Created);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER_END);

            result.Append(Util.Constants.TEXT_TOSTRING_END);

            return result.ToString();
        }

        #endregion


        #region Overridden methods

        private void parseStreamTitle()
        {
            title = string.Empty;
            artist = string.Empty;
            streamTitle = string.Empty;

            int startIndex = Info.IndexOf(streamTitleString, System.StringComparison.OrdinalIgnoreCase);

            if (startIndex != -1)
            {
                string part = Info.Substring(startIndex + streamTitleString.Length);

                int endIndex = part.IndexOf("';");

                if (endIndex != -1)
                {
                    streamTitle = part.Substring(0, endIndex).Replace('˗', '-').Replace('@', ' ').Replace('*', ' ').Replace('+', ' ').Trim();

                    string[] parts = streamTitle.Split(splitChar, 2);
                    if (parts.Length >= 2)
                    {
                        artist = parts[0].Trim().CTToTitleCase();

                        //handle special chars |
                        string titleTemp = parts[parts.Length - 1];
                        int charLocation = titleTemp.IndexOf('|');

                        if (charLocation > 0)
                        {
                            titleTemp = titleTemp.Substring(0, charLocation);
                        }
                        title = titleTemp.Trim().CTToTitleCase();
                    }
                    else if (parts.Length == 1)
                    {
                        title = parts[0].Trim().CTToTitleCase();
                    }
                }
            }
        }

        private void parseStreamUrl()
        {
            streamUrl = string.Empty;

            int startIndex = Info.IndexOf(streamUrlString, System.StringComparison.OrdinalIgnoreCase);

            if (startIndex != -1)
            {
                string part = Info.Substring(startIndex + streamUrlString.Length);

                int endIndex = part.IndexOf("'");

                if (endIndex != -1)
                {
                    streamUrl = part.Substring(0, endIndex); //.Replace('˗', '-');
                }
            }
        }

        private void parseSpotifyUrl()
        {
            spotifyUrl = "spotify:search:" + Artist.Replace(' ', '+') + '+' + Title.Replace(' ', '+');

            //spotifyUrl = "spotify:search:" + Artist + Title;
        }
/*
        private void parseITunesUrl()
        {
            itunesUrl = "itms://phobos.apple.com/WebObjects/MZSearch.woa/wa/advancedSearchResults?artistTerm=" + Artist.Replace(' ', '+') + "&songTerm=" + Title.Replace(' ', '+');
            //itunesUrl = "https://search.itunes.apple.com/WebObjects/MZSearch.woa/wa/advancedSearch?genreIndex=1&amp;media=music&amp;restrict=true&amp;submit=seeAllLockups&amp;entity=songs&amp;allArtistNames=" + Artist.Replace(' ', '+') + "&amp;allTitle=" + Title.Replace(' ', '+');
        }
*/
        #endregion

    }
}
// © 2016-2017 crosstales LLC (https://www.crosstales.com)