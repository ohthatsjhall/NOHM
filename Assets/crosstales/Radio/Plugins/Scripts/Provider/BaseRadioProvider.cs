using UnityEngine;
using System.Collections;
using System.Linq;

namespace Crosstales.Radio.Provider
{
    /// <summary>Base class for radio providers.</summary>
    [ExecuteInEditMode]
    public abstract class BaseRadioProvider : MonoBehaviour
    {

        #region Variables

        [Header("General Settings")]

        /// <summary>Global RadioFilter (active if no explicit filter is given).</summary>
        [Tooltip("Global RadioFilter (active if no explicit filter is given).")]
        public Model.RadioFilter Filter;


        [Header("Load Behaviour")]

        /// <summary>Clears all existing stations on 'Load' (default: true).</summary>
        [Tooltip("Clears all existing stations on 'Load' (default: true).")]
        public bool ClearStationsOnLoad = true;

        /// <summary>Calls 'Load' on Start (default: true).</summary>
        [Tooltip("Calls 'Load' on Start (default: true).")]
        public bool LoadOnStart = true;

        /// <summary>Calls 'Load' on Start in Editor (default: true).</summary>
        [Tooltip("Calls 'Load' on Start in Editor (default: true).")]
        public bool LoadOnStartInEditor = true;


        //protected System.Collections.Generic.List<System.Guid> coRoutines = new System.Collections.Generic.List<System.Guid>();
        protected System.Collections.Generic.List<string> coRoutines = new System.Collections.Generic.List<string>();

        private int stationIndex = -1;
        private System.Collections.Generic.List<Model.RadioStation> randomStations = new System.Collections.Generic.List<Model.RadioStation>(200);
        private int randomStationIndex = -1;

        private bool initRandom = false;
        private bool loadedInEditor = true;

        //m3u
        private const string extM3UIdentifier = "#EXTM3U";
        private const string extInfIdentifier = "#EXTINF";

        //pls
        private const string fileIdentifier = "file";
        private const string titleIdentifier = "title";
        private const string lengthIdentifier = "length";

        // split chars
        private static char[] splitCharEquals = new char[] { '=' };
        private static char[] splitCharText = new char[] { ';' };
        private static char[] splitCharColon = new char[] { ':' };
        private static char[] splitCharComma = new char[] { ',' };

        private System.Collections.Generic.List<Model.RadioStation> stations = new System.Collections.Generic.List<Model.RadioStation>(200);

        #endregion


        #region Properties

        /// <summary>Returns the list of all RadioEntry.</summary>
        /// <returns>>List of all RadioEntry.</returns>
        public abstract System.Collections.Generic.List<Model.Entry.BaseRadioEntry> RadioEntries
        {
            get;
        }

        /// <summary>Returns the list of all loaded RadioStation.</summary>
        /// <returns>List of all loaded RadioStation.</returns>
        public System.Collections.Generic.List<Model.RadioStation> Stations
        {
            get { return stations; }
            protected set { stations = value; }
        }

        /// <summary>Is this provider ready (= data loaded)?</summary>
        /// <returns>True if this provider is ready.</returns>
        public bool isReady
        {
            get
            {
                if (Util.Helper.isEditorMode)
                {
                    return loadedInEditor;
                }
                else
                {
                    return coRoutines.Count == 0;
                }
            }
        }

        #endregion


        #region MonoBehaviour methods

        public virtual void Start()
        {
            //Debug.Log("Start called!");

            if ((LoadOnStart && !Util.Helper.isEditorMode) || (LoadOnStartInEditor && Util.Helper.isEditorMode))
            {
                Load();
            }

            OnValidate();
        }

        //public virtual void Start()
        //{
        //    OnValidate();
        //}

        public virtual void Update()
        {
            if (!initRandom && isReady)
            {
                initRandom = true;
                RandomizeStations();

                //OnValidate();
            }
        }

        public virtual void OnValidate()
        {
            foreach (Model.Entry.BaseRadioEntry entry in RadioEntries)
            {
                if (entry != null)
                {
                    if (!entry.isInitalized)
                    {
                        entry.Format = Model.Enum.AudioFormat.MP3;
                        entry.EnableSource = true;

                        entry.isInitalized = true;
                    }

                    if (entry.Bitrate <= 0)
                    {
                        entry.Bitrate = Util.Config.DEFAULT_BITRATE;
                    }
                    else
                    {
                        entry.Bitrate = Util.Helper.NearestBitrate(entry.Bitrate, entry.Format);
                    }

                    if (entry.ChunkSize <= 0)
                    {
                        entry.ChunkSize = Util.Config.DEFAULT_CHUNKSIZE;
                    }
                    else if (entry.ChunkSize > Util.Config.MAX_CACHESTREAMSIZE)
                    {
                        entry.ChunkSize = Util.Config.MAX_CACHESTREAMSIZE;
                    }

                    if (entry.BufferSize <= 0)
                    {
                        entry.BufferSize = Util.Config.DEFAULT_BUFFERSIZE;
                    }
                    else
                    {
                        if (entry.Format == Model.Enum.AudioFormat.MP3)
                        {
                            if (entry.BufferSize < Util.Config.DEFAULT_BUFFERSIZE / 4)
                            {
                                entry.BufferSize = Util.Config.DEFAULT_BUFFERSIZE / 4;
                            }
                        }
                        else if (entry.Format == Model.Enum.AudioFormat.OGG)
                        {
                            if (entry.BufferSize < Util.Constants.MIN_OGG_BUFFERSIZE)
                            {
                                entry.BufferSize = Util.Constants.MIN_OGG_BUFFERSIZE;
                            }
                        }

                        if (entry.BufferSize < entry.ChunkSize)
                        {
                            entry.BufferSize = entry.ChunkSize;
                        }
                        else if (entry.BufferSize > Util.Config.MAX_CACHESTREAMSIZE)
                        {
                            entry.BufferSize = Util.Config.MAX_CACHESTREAMSIZE;
                        }
                    }
                }
            }
        }

        #endregion


        #region Public methods

        /// <summary>Loads all stations from this provider.</summary>
        public void Load()
        {
            if (Util.Helper.isEditorMode)
            {
#if UNITY_EDITOR
                initInEditor();
#endif
            }
            else
            {
                init();
            }
        }

        /// <summary>Saves all stations from this provider as text-file with streams.</summary>
        /// <param name="path">Path to the text-file.</param>
        public void Save(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {

                try
                {
                    path = path.Replace(Util.Constants.PREFIX_FILE, ""); //remove file://-prefix

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(path))
                    {
                        file.WriteLine("# " + Util.Constants.ASSET_NAME + " " + Util.Constants.ASSET_VERSION);
                        file.WriteLine("# © 2015-2017 by " + Util.Constants.ASSET_AUTHOR + " (" + Util.Constants.ASSET_AUTHOR_URL + ")");
                        file.WriteLine("#");
                        file.WriteLine("# List of all radio stations from '" + GetType().Name + "'");
                        file.WriteLine("# Created: " + System.DateTime.Now.ToString("dd.MM.yyyy"));
                        file.WriteLine("# Name;Url;DataFormat;AudioFormat;Station (optional);Genres (optional);Bitrate (in kbit/s, optional);Rating (0-5, optional);Description (optional);ExcludeCodec (optional);ChunkSize (in KB, optional);BufferSize (in KB, optional)");

                        foreach (Model.RadioStation rs in StationsByStation())
                        {
                            file.WriteLine(rs.ToTextLine());
                        }
                    }
                }
                catch (System.IO.IOException ex)
                {
                    Debug.LogError("Could not write file: " + ex);
                }
            }
            else
            {
                Debug.LogWarning("'path' was null or empty! Could not save the data!");
            }
        }

        /// <summary>Randomize all radio stations.</summary>
        /// <param name="resetIndex">Reset the index of the random radio stations (default: true, optional)</param>
        public void RandomizeStations(bool resetIndex = true)
        {
            randomStations.CTShuffle();

            if (resetIndex)
            {
                randomStationIndex = 0;
            }
        }

        /// <summary>Count all RadioStation for a given RadioFilter.</summary>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>Number of all RadioStation for a given RadioFilter.</returns>
        public int CountStations(Model.RadioFilter filter = null)
        {
            if (getFilter(filter) == null)
            {
                return Stations.Count();
            }
            else
            {
                return filterStations(false, getFilter(filter)).ToList<Model.RadioStation>().Count();
            }
        }

        /// <summary>Next (normal/random) radio station from this provider.</summary>
        /// <param name="random">Return a random radio station. (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>Next radio station.</returns>
        public Model.RadioStation Next(bool random = false, Model.RadioFilter filter = null)
        {
            if (Stations != null && Stations.Count > 0)
            {
                return next(random, getFilter(filter));
            }
            else
            {
                Debug.LogWarning("No 'Stations' found: returning null.");
            }

            return null;
        }

        /// <summary>Previous (normal/random) radio station from this provider.</summary>
        /// <param name="random">Return a random radio station. (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>Previous radio station.</returns>
        public Model.RadioStation Previous(bool random = false, Model.RadioFilter filter = null)
        {
            if (Stations != null && Stations.Count > 0)
            {
                return previous(random, getFilter(filter));
            }
            else
            {
                Debug.LogWarning("No 'Stations' found: returning null.");
            }

            return null;
        }

        /// <summary>Returns all radio stations of this provider ordered by name.</summary>
        /// <param name="desc">Descending order (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>All radios of this provider ordered by name.</returns>
        public System.Collections.Generic.List<Model.RadioStation> StationsByName(bool desc = false, Model.RadioFilter filter = null)
        {
            System.Collections.Generic.IEnumerable<Model.RadioStation> entries = filterStations(false, getFilter(filter));

            if (desc)
            {
                return new System.Collections.Generic.List<Model.RadioStation>(entries.OrderByDescending(entry => entry.Name));
            }
            else
            {
                return new System.Collections.Generic.List<Model.RadioStation>(entries.OrderBy(entry => entry.Name));
            }
        }

        /// <summary>Returns all radio stations of this provider ordered by URL.</summary>
        /// <param name="desc">Descending order (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>All radios of this provider ordered by URL.</returns>
        public System.Collections.Generic.List<Model.RadioStation> StationsByURL(bool desc = false, Model.RadioFilter filter = null)
        {
            System.Collections.Generic.IEnumerable<Model.RadioStation> entries = filterStations(false, getFilter(filter));

            if (desc)
            {
                return new System.Collections.Generic.List<Model.RadioStation>(entries.OrderByDescending(entry => entry.Url).ThenBy(entry => entry.Name));
            }
            else
            {
                return new System.Collections.Generic.List<Model.RadioStation>(entries.OrderBy(entry => entry.Url).ThenBy(entry => entry.Name));
            }
        }

        /// <summary>Returns all radio stations of this provider ordered by audio format.</summary>
        /// <param name="desc">Descending order (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>All radios of this provider ordered by audio format.</returns>
        public System.Collections.Generic.List<Model.RadioStation> StationsByFormat(bool desc = false, Model.RadioFilter filter = null)
        {
            System.Collections.Generic.IEnumerable<Model.RadioStation> entries = filterStations(false, getFilter(filter));

            if (desc)
            {
                return new System.Collections.Generic.List<Model.RadioStation>(entries.OrderByDescending(entry => entry.Format).ThenBy(entry => entry.Name));
            }
            else
            {
                return new System.Collections.Generic.List<Model.RadioStation>(entries.OrderBy(entry => entry.Format).ThenBy(entry => entry.Name));
            }
        }

        /// <summary>Returns all radio stations of this provider ordered by station.</summary>
        /// <param name="desc">Descending order (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>All radios of this provider ordered by station.</returns>
        public System.Collections.Generic.List<Model.RadioStation> StationsByStation(bool desc = false, Model.RadioFilter filter = null)
        {
            System.Collections.Generic.IEnumerable<Model.RadioStation> entries = filterStations(false, getFilter(filter));

            if (desc)
            {
                return new System.Collections.Generic.List<Model.RadioStation>(entries.OrderByDescending(entry => entry.Station).ThenBy(entry => entry.Name));
            }
            else
            {
                return new System.Collections.Generic.List<Model.RadioStation>(entries.OrderBy(entry => entry.Station).ThenBy(entry => entry.Name));
            }
        }

        /// <summary>Returns all radio stations of this provider ordered by bitrate.</summary>
        /// <param name="desc">Descending order (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>All radios of this provider ordered by bitrate.</returns>
        public System.Collections.Generic.List<Model.RadioStation> StationsByBitrate(bool desc = false, Model.RadioFilter filter = null)
        {
            System.Collections.Generic.IEnumerable<Model.RadioStation> entries = filterStations(false, getFilter(filter));

            if (desc)
            {
                return new System.Collections.Generic.List<Model.RadioStation>(entries.OrderByDescending(entry => entry.Bitrate).ThenBy(entry => entry.Name));
            }
            else
            {
                return new System.Collections.Generic.List<Model.RadioStation>(entries.OrderBy(entry => entry.Bitrate).ThenBy(entry => entry.Name));
            }
        }

        /// <summary>Returns all radio stations of this provider ordered by genre.</summary>
        /// <param name="desc">Descending order (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>All radios of this provider ordered by genre.</returns>
        public System.Collections.Generic.List<Model.RadioStation> StationsByGenre(bool desc = false, Model.RadioFilter filter = null)
        {
            System.Collections.Generic.IEnumerable<Model.RadioStation> entries = filterStations(false, getFilter(filter));

            if (desc)
            {
                return new System.Collections.Generic.List<Model.RadioStation>(entries.OrderByDescending(entry => entry.Genres).ThenBy(entry => entry.Name));
            }
            else
            {
                return new System.Collections.Generic.List<Model.RadioStation>(entries.OrderBy(entry => entry.Genres).ThenBy(entry => entry.Name));
            }
        }

        /// <summary>Returns all radio stations of this provider ordered by rating.</summary>
        /// <param name="desc">Descending order (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>All radios of this provider ordered by rating.</returns>
        public System.Collections.Generic.List<Model.RadioStation> StationsByRating(bool desc = false, Model.RadioFilter filter = null)
        {
            System.Collections.Generic.IEnumerable<Model.RadioStation> entries = filterStations(false, getFilter(filter));

            if (desc)
            {
                return new System.Collections.Generic.List<Model.RadioStation>(entries.OrderByDescending(entry => entry.Rating).ThenBy(entry => entry.Name));
            }
            else
            {
                return new System.Collections.Generic.List<Model.RadioStation>(entries.OrderBy(entry => entry.Rating).ThenBy(entry => entry.Name));
            }
        }

        #endregion


        #region Private methods

        protected virtual void init()
        {
            initRandom = false;

            if (ClearStationsOnLoad)
            {
                Stations.Clear();
                randomStations.Clear();
            }
        }

        protected IEnumerator loadWeb(string uid, Model.Entry.RadioEntryURL entry, bool suppressDoubleStations = false)
        {

            //yield return new WaitForSeconds(Random.Range(0, Constants.RANDOM_WAIT_TIME));

            if (!string.IsNullOrEmpty(entry.FinalURL))
            {
                using (WWW www = new WWW(entry.FinalURL))
                {
                    do
                    {
                        yield return www;
                    } while (!www.isDone);

                    if (string.IsNullOrEmpty(www.error) && !string.IsNullOrEmpty(www.text))
                    {
                        System.Collections.Generic.List<string> list = Util.Helper.SplitStringToLines(www.text);

                        yield return null;

                        if (list.Count > 0)
                        {
                            if (entry.DataFormat == Model.Enum.DataFormatURL.M3U)
                            {
                                fillStationsFromM3U(list, entry, entry.ReadNumberOfStations, suppressDoubleStations);
                            }
                            else if (entry.DataFormat == Model.Enum.DataFormatURL.PLS)
                            {
                                fillStationsFromPLS(list, entry, entry.ReadNumberOfStations, suppressDoubleStations);
                            }
                            else if (entry.DataFormat == Model.Enum.DataFormatURL.Text)
                            {
                                fillStationsFromText(list, entry, entry.ReadNumberOfStations, suppressDoubleStations);
                            }
                            else
                            {
                                Debug.LogWarning("Not implemented!");
                            }
                        }
                        else
                        {
                            Debug.LogWarning(entry + " - URL: '" + entry.FinalURL + "' does not contain any active radio stations!");
                        }
                    }
                    else
                    {
                        Debug.LogWarning(entry + " - Could not load source: '" + entry.FinalURL + "'" + System.Environment.NewLine + www.error + System.Environment.NewLine + "Did you set the correct 'URL'?");
                    }
                }
                //www.Dispose();
            }
            else
            {
                Debug.LogWarning(entry + ": 'URL' is null or empty!" + System.Environment.NewLine + "Please add a valid URL.");
            }

            coRoutines.Remove(uid);
        }

        protected IEnumerator loadResource(string uid, Model.Entry.RadioEntryResource entry, bool suppressDoubleStations = false)
        {
            if (entry.Resource != null)
            {
                //List<string> list = Helper.SplitStringToLines(Resource.text, SkipHeaderLines, SkipFooterLines);
                System.Collections.Generic.List<string> list = Util.Helper.SplitStringToLines(entry.Resource.text);

                yield return null;

                if (list.Count > 0)
                {
                    if (entry.DataFormat == Model.Enum.DataFormatResource.M3U)
                    {
                        fillStationsFromM3U(list, entry, entry.ReadNumberOfStations, suppressDoubleStations);
                    }
                    else if (entry.DataFormat == Model.Enum.DataFormatResource.PLS)
                    {
                        fillStationsFromPLS(list, entry, entry.ReadNumberOfStations, suppressDoubleStations);
                    }
                    else if (entry.DataFormat == Model.Enum.DataFormatResource.Text)
                    {
                        fillStationsFromText(list, entry, entry.ReadNumberOfStations, suppressDoubleStations);
                    }
                    else
                    {
                        Debug.LogWarning("Not implemented!");
                    }
                }
                else
                {
                    Debug.LogWarning(entry + " - Resource: '" + entry.Resource + "' does not contain any active radio stations!");
                }
            }
            else
            {
                Debug.LogWarning(entry + ": resource field 'Resource' is null or empty!" + System.Environment.NewLine + "Please add a valid resource.");
            }


            coRoutines.Remove(uid);
        }

        protected IEnumerator loadShoutcast(string uid, Model.Entry.RadioEntryShoutcast entry, bool suppressDoubleStations = false)
        {
            //yield return new WaitForSeconds(Random.Range(0, Constants.RANDOM_WAIT_TIME));

            using (WWW www = new WWW(Util.Constants.SHOUTCAST + entry.ShoutcastID.Trim()))
            {
                do
                {
                    yield return www;
                } while (!www.isDone);

                if (string.IsNullOrEmpty(www.error) && !string.IsNullOrEmpty(www.text))
                {
                    System.Collections.Generic.List<string> list = Util.Helper.SplitStringToLines(www.text);

                    yield return null;

                    if (list.Count > 0)
                    {
                        fillStationsFromPLS(list, entry, 1, suppressDoubleStations);
                    }
                    else
                    {
                        Debug.LogWarning(entry + " - Shoutcast-ID: '" + entry.ShoutcastID + "' does not contain any active radio stations!");
                    }
                }
                else
                {
                    Debug.LogWarning(entry + " - Could not load Shoutcast-ID: '" + entry.ShoutcastID + "'" + System.Environment.NewLine + www.error + System.Environment.NewLine + "Did you set the correct 'Shoutcast-ID'?");
                }
            }
            //www.Dispose();

            coRoutines.Remove(uid);
        }

        private System.Collections.Generic.IEnumerable<Model.RadioStation> filterStations(bool random = false, Model.RadioFilter filter = null)
        {
            if (random)
            {
                if (filter != null && filter.isFiltering)
                {
                    return from entry in randomStations
                           where (filter == null) || ((entry.Name == null || entry.Name.CTContainsAny(filter.Name)) &&
                              (entry.Station == null || entry.Station.CTContainsAny(filter.Station)) &&
                              (entry.Url == null || entry.Url.CTContainsAll(filter.Url)) &&
                              (entry.Genres == null || entry.Genres.CTContainsAny(filter.Genres)) &&
                              (entry.Format.ToString().CTContainsAny(filter.Format)) &&
                              (entry.Bitrate >= filter.BitrateMin && entry.Bitrate <= filter.BitrateMax) &&
                              (entry.Rating >= filter.RatingMin && entry.Rating <= filter.RatingMax)) &&
                              ((!filter.ExcludeUnsupportedCodecs || entry.ExcludedCodec == Model.Enum.AudioCodec.None) || entry.ExcludedCodec != Util.Helper.AudioCodecForAudioFormat(entry.Format))
                           select entry;
                }
                else
                {
                    return randomStations;
                }
            }
            else
            {
                if (filter != null && filter.isFiltering)
                {
                    return from entry in Stations
                           where (filter == null) || ((entry.Name == null || entry.Name.CTContainsAny(filter.Name)) &&
                              (entry.Station == null || entry.Station.CTContainsAny(filter.Station)) &&
                              (entry.Url == null || entry.Url.CTContainsAll(filter.Url)) &&
                              (entry.Genres == null || entry.Genres.CTContainsAny(filter.Genres)) &&
                              (entry.Format.ToString().CTContainsAny(filter.Format)) &&
                              (entry.Bitrate >= filter.BitrateMin && entry.Bitrate <= filter.BitrateMax) &&
                              (entry.Rating >= filter.RatingMin && entry.Rating <= filter.RatingMax)) &&
                              ((!filter.ExcludeUnsupportedCodecs || entry.ExcludedCodec == Model.Enum.AudioCodec.None) || entry.ExcludedCodec != Util.Helper.AudioCodecForAudioFormat(entry.Format))
                           select entry;
                }
                else
                {
                    return Stations;
                }
            }
        }

        protected void fillStationsFromM3U(System.Collections.Generic.List<string> list, Model.Entry.BaseRadioEntry entry, int readNumberOfStations = 0, bool suppressDoubleStations = false)
        {
            string[] extsplit;
            string[] ext2split;
            string line;

            string file;
            string title;
            //int length;

            int stationCount = 0;

            Model.RadioStation station;

            for (int ii = 0; ii < list.Count;)
            {

                file = string.Empty;
                title = string.Empty;

                line = list[ii].Trim();

                if (ii == 0 && !line.CTEquals (extM3UIdentifier)) {
                    Debug.LogWarning("File is not in the M3U-format!");

                    break;
                }

                if (!line.CTContains(extM3UIdentifier))
                { //EXTM3U?
                    if (line.CTContains(extInfIdentifier))
                    { //EXTINF?

                        extsplit = line.Split(splitCharColon, System.StringSplitOptions.RemoveEmptyEntries);

                        if (extsplit.Length > 1)
                        {
                            ext2split = extsplit[1].Split(splitCharComma, System.StringSplitOptions.RemoveEmptyEntries);

                            if (ext2split.Length > 1)
                            {

                                //int.TryParse(ext2split [0], out length); //ignore length for streams
                                title = ext2split[1];
                            }
                        }

                        if (ii + 1 < list.Count)
                        {
                            ii++;
                            line = list[ii];

                            file = line;
                        }
                    }
                    else if (!string.IsNullOrEmpty(line))
                    {
                        file = line;
                    }

                    if (!string.IsNullOrEmpty(file))
                    {
                        station = new Model.RadioStation((entry.ForceName ? entry.Name : (string.IsNullOrEmpty(title) ? entry.Name : title.Trim())), file.Trim(), (entry.Format == Model.Enum.AudioFormat.UNKNOWN ? Util.Helper.AudioFormatFromString(file) : entry.Format), entry.Station, entry.Genres.ToLower(), entry.Bitrate, entry.Rating, entry.Description, entry.Icon, entry.ChunkSize, entry.BufferSize, entry.ExcludedCodec);

                        if (!Stations.Contains(station))
                        {

                            Stations.Add(station);
                            randomStations.Add(station);

                            stationCount++;

                            if (Util.Constants.DEV_DEBUG)
                                Debug.Log("Station added: " + station);

                            if (readNumberOfStations == stationCount)
                            {
                                break;
                            }
                        }
                        else
                        {
                            if (!suppressDoubleStations)
                            {
                                Debug.LogWarning("Station already added: '" + entry + "'");
                            }
                        }
                    }
                }

                ii++;
            }

            RandomizeStations();
        }

        protected void fillStationsFromPLS(System.Collections.Generic.List<string> list, Model.Entry.BaseRadioEntry entry, int readNumberOfStations = 0, bool suppressDoubleStations = false)
        {
            string[] filesplit;
            string[] titlesplit;
            //string[] lengthsplit;
            string line;

            string file;
            string title;
            //int length;

            int stationCount = 0;

            Model.RadioStation station;

            for (int ii = 0; ii < list.Count;)
            {

                line = list[ii].Trim();

                if (ii == 0 && !line.CTEquals ("[playlist]")) {
                    Debug.LogWarning("File is not in the PLS-format!");

                    break;
                }

                file = string.Empty;
                title = string.Empty;
                //length = 0;

                if (line.CTContains(fileIdentifier))
                { //File?

                    filesplit = line.Split(splitCharEquals, System.StringSplitOptions.RemoveEmptyEntries);

                    if (filesplit.Length > 1)
                    {
                        file = filesplit[1];

                        if (ii + 1 < list.Count)
                        {
                            ii++;
                            line = list[ii];

                            if (line.CTContains(titleIdentifier))
                            { // Title?
                                titlesplit = line.Split(splitCharEquals, System.StringSplitOptions.RemoveEmptyEntries);

                                if (titlesplit.Length > 1)
                                {
                                    title = titlesplit[1];

                                    ii++;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(file))
                        {
                            station = new Model.RadioStation((entry.ForceName ? entry.Name : (string.IsNullOrEmpty(title) ? entry.Name : title.Trim())), file.Trim(), (entry.Format == Model.Enum.AudioFormat.UNKNOWN ? Util.Helper.AudioFormatFromString(file) : entry.Format), entry.Station, entry.Genres.ToLower(), entry.Bitrate, entry.Rating, entry.Description, entry.Icon, entry.ChunkSize, entry.BufferSize, entry.ExcludedCodec);

                            if (!Stations.Contains(station))
                            {
                                Stations.Add(station);
                                randomStations.Add(station);

                                stationCount++;

                                if (Util.Constants.DEV_DEBUG)
                                    Debug.Log("Station added: " + station);

                                if (readNumberOfStations == stationCount)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                if (!suppressDoubleStations)
                                {
                                    Debug.LogWarning("Station already added: '" + entry + "'");
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning(entry + ": No URL found for '" + fileIdentifier + "': " + line);
                    }
                }
                else
                {
                    ii++;
                }
            }

            RandomizeStations();
        }

        protected void fillStationsFromText(System.Collections.Generic.List<string> list, Model.Entry.BaseRadioEntry entry, int readNumberOfStations = 0, bool suppressDoubleStations = false)
        {
            Model.RadioStation station;
            int bitrate;
            float rating;
            int chunkSize;
            int bufferSize;

            int stationCount = 0;

            foreach (string line in list)
            {
                string[] content = line.Split(splitCharText, System.StringSplitOptions.None);

                if (content.Length >= 4 && content.Length <= 12)
                {

                    station = new Model.RadioStation((entry.ForceName ? entry.Name : (string.IsNullOrEmpty(content[0]) ? entry.Name : content[0].Trim())), content[1].Trim(), Util.Helper.AudioFormatFromString(content[3].Trim()));

                    bitrate = entry.Bitrate;
                    rating = entry.Rating;
                    chunkSize = entry.ChunkSize;
                    bufferSize = entry.BufferSize;

                    if (content.Length >= 5)
                    {
                        station.Station = content[4].Trim();
                    }

                    if (content.Length >= 6)
                    {
                        station.Genres = content[5].Trim().ToLower();
                    }

                    if (content.Length >= 7)
                    {
                        station.Bitrate = int.TryParse(content[6].Trim(), out bitrate) ? bitrate : entry.Bitrate;
                    }

                    if (content.Length >= 8)
                    {
                        station.Rating = float.TryParse(content[7].Trim(), out rating) ? rating : entry.Rating;
                    }

                    if (content.Length >= 9)
                    {
                        station.Description = content[8].Trim();
                    }

                    if (content.Length >= 10)
                    {
                        station.ExcludedCodec = Util.Helper.AudioCodecFromString(content[9].Trim());
                    }

                    if (content.Length >= 11)
                    {
                        station.ChunkSize = int.TryParse(content[10].Trim(), out chunkSize) ? chunkSize : entry.ChunkSize;
                    }

                    if (content.Length == 12)
                    { //all parameters
                        station.BufferSize = int.TryParse(content[11].Trim(), out bufferSize) ? bufferSize : entry.BufferSize;
                    }

                    if (station.Format == Model.Enum.AudioFormat.OGG && station.BufferSize < Util.Constants.MIN_OGG_BUFFERSIZE)
                    {
                        if (Util.Config.DEBUG)
                            Debug.Log("Adjusted buffer size: " + station);
                        station.BufferSize = Util.Constants.MIN_OGG_BUFFERSIZE;
                    }


                    stationCount++;

                    if (content[2].CTEquals("stream"))
                    {
                        if (!Stations.Contains(station))
                        {

                            Stations.Add(station);
                            randomStations.Add(station);

                            if (Util.Config.DEBUG)
                                Debug.Log("Station added: " + station);
                        }
                        else
                        {
                            if (!suppressDoubleStations)
                            {
                                Debug.LogWarning("Station already added: '" + entry + "'");
                            }
                        }
                    }
                    else if (content[2].CTContains("pls"))
                    {
                        if (Util.Helper.isEditorMode)
                        {
#if UNITY_EDITOR
                            loadWebInEditor(new Model.Entry.RadioEntryURL(station, content[1].Trim(), Model.Enum.DataFormatURL.PLS, 1 /*readNumberOfStations*/));
#endif
                        }
                        else
                        {
                            StartCoroutine(loadWeb(addCoRoutine(), new Model.Entry.RadioEntryURL(station, content[1].Trim(), Model.Enum.DataFormatURL.PLS, 1 /*readNumberOfStations*/)));
                        }

                    }
                    else if (content[2].CTContains("m3u"))
                    {
                        if (Util.Helper.isEditorMode)
                        {
#if UNITY_EDITOR
                            loadWebInEditor(new Model.Entry.RadioEntryURL(station, content[1].Trim(), Model.Enum.DataFormatURL.M3U, 1 /*readNumberOfStations*/));
#endif
                        }
                        else
                        {
                            StartCoroutine(loadWeb(addCoRoutine(), new Model.Entry.RadioEntryURL(station, content[1].Trim(), Model.Enum.DataFormatURL.M3U, 1 /*readNumberOfStations*/)));
                        }
                    }
                    else if (content[2].CTContains("shoutcast"))
                    {
                        if (Util.Helper.isEditorMode)
                        {
#if UNITY_EDITOR
                            loadShoutcastInEditor(new Model.Entry.RadioEntryShoutcast(station, content[1].Trim()));
#endif
                        }
                        else
                        {
                            StartCoroutine(loadShoutcast(addCoRoutine(), new Model.Entry.RadioEntryShoutcast(station, content[1].Trim())));
                        }

                    }
                    else
                    {
                        Debug.LogWarning("Could not determine URL for station: '" + entry + "'" + System.Environment.NewLine + line);
                        stationCount--;
                    }

                    if (readNumberOfStations == stationCount)
                    {
                        break;
                    }

                }
                else
                {
                    Debug.LogWarning("Invalid station description: '" + entry + "'" + "'" + System.Environment.NewLine + line);
                }
            }

            RandomizeStations();
        }

        private Model.RadioStation next(bool random = false, Model.RadioFilter filter = null)
        {
            System.Collections.Generic.List<Model.RadioStation> _stations;

            if (random)
            {
                _stations = new System.Collections.Generic.List<Model.RadioStation>(filterStations(random, filter));

                if (randomStationIndex > -1 && randomStationIndex + 1 < _stations.Count)
                {
                    randomStationIndex++;
                }
                else
                {
                    randomStationIndex = 0;
                }

                if (_stations.Count > 0)
                {
                    return _stations[randomStationIndex];
                }
            }
            else
            {
                _stations = StationsByName(false, filter);

                if (stationIndex > -1 && stationIndex + 1 < _stations.Count)
                {
                    stationIndex++;
                }
                else
                {
                    stationIndex = 0;
                }

                if (_stations.Count > 0)
                {
                    return _stations[stationIndex];
                }
            }

            return null;
        }

        private Model.RadioStation previous(bool random = false, Model.RadioFilter filter = null)
        {
            System.Collections.Generic.List<Model.RadioStation> _stations;

            if (random)
            {
                _stations = new System.Collections.Generic.List<Model.RadioStation>(filterStations(random, filter));

                if (randomStationIndex > 0 && randomStationIndex < _stations.Count)
                {
                    randomStationIndex--;
                }
                else
                {
                    randomStationIndex = _stations.Count - 1;
                }

                if (_stations.Count > 0)
                {
                    return _stations[randomStationIndex];
                }
            }
            else
            {
                _stations = StationsByName(false, filter);

                if (stationIndex > 0 && stationIndex < _stations.Count)
                {
                    stationIndex--;
                }
                else
                {
                    stationIndex = _stations.Count - 1;
                }

                if (_stations.Count > 0)
                {
                    return _stations[stationIndex];
                }
            }

            return null;
        }

        protected string addCoRoutine()
        {
            string uid = System.Guid.NewGuid().ToString();
            coRoutines.Add(uid);

            return uid;
        }

        private Model.RadioFilter getFilter(Model.RadioFilter filter)
        {
            if (filter != null && filter.isFiltering)
            {
                return filter;
            }
            else if (Filter.isFiltering)
            {
                return Filter;
            }

            return null;
        }

        #endregion


        #region Editor-only methods

#if UNITY_EDITOR

        protected virtual void initInEditor()
        {
            if (Util.Helper.isEditorMode)
            {
                Stations.Clear();
                randomStations.Clear();
            }
        }

        protected void loadWebInEditor(Model.Entry.RadioEntryURL entry, bool suppressDoubleStations = false)
        {
            if (Util.Helper.isEditorMode)
            {
                loadedInEditor = false;

                if (!string.IsNullOrEmpty(entry.FinalURL))
                {
                    using (WWW www = new WWW(entry.FinalURL))
                    {
                        float startTime = Time.realtimeSinceStartup; //time in seconds
                        bool isTimeout = false;

                        while (!www.isDone)
                        {
                            if (Time.realtimeSinceStartup - startTime > Util.Constants.MAX_WEB_LOAD_WAIT_TIME)
                            {
                                isTimeout = true;
                                break;
                            }
                        }

                        if (!isTimeout && string.IsNullOrEmpty(www.error) && !string.IsNullOrEmpty(www.text))
                        {
                            System.Collections.Generic.List<string> list = Util.Helper.SplitStringToLines(www.text);

                            if (list.Count > 0)
                            {
                                if (entry.DataFormat == Model.Enum.DataFormatURL.M3U)
                                {
                                    fillStationsFromM3U(list, entry, entry.ReadNumberOfStations, suppressDoubleStations);
                                }
                                else if (entry.DataFormat == Model.Enum.DataFormatURL.PLS)
                                {
                                    fillStationsFromPLS(list, entry, entry.ReadNumberOfStations, suppressDoubleStations);
                                }
                                else if (entry.DataFormat == Model.Enum.DataFormatURL.Text)
                                {
                                    fillStationsFromText(list, entry, entry.ReadNumberOfStations, suppressDoubleStations);
                                }
                                else
                                {
                                    Debug.LogWarning("Not implemented!");
                                }
                            }
                            else
                            {
                                Debug.LogWarning(entry + " - URL: '" + entry.FinalURL + "' does not contain any active radio stations!");
                            }
                        }
                        else
                        {
                            Debug.LogWarning(entry + " - Could not load source: '" + entry.FinalURL + "'" + System.Environment.NewLine + www.error + System.Environment.NewLine + "Did you set the correct 'URL'?");
                        }
                    }
                    //www.Dispose();
                }
                else
                {
                    Debug.LogWarning(entry + ": 'URL' is null or empty!" + System.Environment.NewLine + "Please add a valid URL.");
                }

                loadedInEditor = true;
            }
        }

        protected void loadResourceInEditor(Model.Entry.RadioEntryResource entry, bool suppressDoubleStations = false)
        {
            if (Util.Helper.isEditorMode)
            {
                loadedInEditor = false;

                if (entry.Resource != null)
                {
                    System.Collections.Generic.List<string> list = Util.Helper.SplitStringToLines(entry.Resource.text);

                    if (list.Count > 0)
                    {
                        if (entry.DataFormat == Model.Enum.DataFormatResource.M3U)
                        {
                            fillStationsFromM3U(list, entry, entry.ReadNumberOfStations, suppressDoubleStations);
                        }
                        else if (entry.DataFormat == Model.Enum.DataFormatResource.PLS)
                        {
                            fillStationsFromPLS(list, entry, entry.ReadNumberOfStations, suppressDoubleStations);
                        }
                        else if (entry.DataFormat == Model.Enum.DataFormatResource.Text)
                        {
                            fillStationsFromText(list, entry, entry.ReadNumberOfStations, suppressDoubleStations);
                        }
                        else
                        {
                            Debug.LogWarning("Not implemented!");
                        }
                    }
                    else
                    {
                        Debug.LogWarning(entry + " - Resource: '" + entry.Resource + "' does not contain any active radio stations!");
                    }
                }
                else
                {
                    Debug.LogWarning(entry + ": resource field 'Resource' is null or empty!" + System.Environment.NewLine + "Please add a valid resource.");
                }

                loadedInEditor = true;
            }
        }

        protected void loadShoutcastInEditor(Model.Entry.RadioEntryShoutcast entry, bool suppressDoubleStations = false)
        {
            if (Util.Helper.isEditorMode)
            {
                loadedInEditor = false;

                using (WWW www = new WWW(Util.Constants.SHOUTCAST + entry.ShoutcastID.Trim()))
                {
                    float startTime = Time.realtimeSinceStartup; //time in seconds
                    bool isTimeout = false;

                    while (!www.isDone)
                    {
                        if (Time.realtimeSinceStartup - startTime > Util.Constants.MAX_SHOUTCAST_LOAD_WAIT_TIME)
                        {
                            isTimeout = true;
                            break;
                        }
                    }

                    //Debug.Log(www.error + " - " + www.text);

                    if (!isTimeout && string.IsNullOrEmpty(www.error) && !string.IsNullOrEmpty(www.text))
                    {
                        System.Collections.Generic.List<string> list = Util.Helper.SplitStringToLines(www.text);

                        if (list.Count > 0)
                        {
                            fillStationsFromPLS(list, entry, 1, suppressDoubleStations);
                        }
                        else
                        {
                            Debug.LogWarning(entry + " - Shoutcast-ID: '" + entry.ShoutcastID + "' does not contain any active radio stations!");
                        }
                    }
                    else
                    {
                        Debug.LogWarning(entry + " - Could not load Shoutcast-ID: '" + entry.ShoutcastID + "'" + System.Environment.NewLine + www.error + System.Environment.NewLine + "Did you set the correct 'Shoutcast-ID'?");
                    }
                }

                loadedInEditor = true;
            }
        }

#endif

        #endregion

    }
}
// © 2015-2017 crosstales LLC (https://www.crosstales.com)