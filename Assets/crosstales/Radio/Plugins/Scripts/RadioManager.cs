using UnityEngine;
using System.Collections;
using System.Linq;

namespace Crosstales.Radio
{
    /// <summary>Radio manager for multiple radio players.</summary>
    [ExecuteInEditMode]
    [HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_radio_manager.html")]
    public class RadioManager : MonoBehaviour
    {

        #region Variables

        [Header("General Settings")]

        /// <summary>Radio station providers for this manager.</summary>
        [Tooltip("Radio station providers for this manager.")]
        public Provider.BaseRadioProvider[] Providers;

        /// <summary>Global RadioFilter (active if no explicit filter is given).</summary>
        [Tooltip("Global RadioFilter (active if no explicit filter is given).")]
        public Model.RadioFilter Filter;


        [Header("Behaviour Settings")]

        /// <summary>Calls 'Load' on Start (default: true).</summary>
        [Tooltip("Calls 'Load' on Start (default: true).")]
        public bool LoadOnStart = true;

        /// <summary>Calls 'Load' on Start in Editor (default: true).</summary>
        [Tooltip("Calls 'Load' on Start in Editor (default: true).")]
        public bool LoadOnStartInEditor = true;


        [Header("Player Settings")]

        /// <summary>Instantiate RadioPlayer (default: false).</summary>
        [Tooltip("Instantiate RadioPlayers (default: false).")]
        public bool InstantiateRadioPlayers = false;

        /// <summary>Prefab of the RadioPlayer.</summary>
        [Tooltip("Prefab of the RadioPlayer.")]
        public GameObject RadioPrefab;

        private int stationIndex = -1;
        private System.Collections.Generic.List<Model.RadioStation> randomStations = new System.Collections.Generic.List<Model.RadioStation>(200);
        private int randomStationIndex = -1;

        private int playerIndex = -1;
        private System.Collections.Generic.List<RadioPlayer> randomPlayers = new System.Collections.Generic.List<RadioPlayer>(200);
        private int randomPlayerIndex = -1;

        private RadioPlayer currentRadioPlayer = null;

        private System.Collections.Generic.List<Model.RadioStation> stations = new System.Collections.Generic.List<Model.RadioStation>(200);
        private System.Collections.Generic.List<RadioPlayer> players = new System.Collections.Generic.List<RadioPlayer>(200);

        #endregion


        #region Properties

        /// <summary>List of all loaded RadioStation from all providers.</summary>
        public System.Collections.Generic.List<Model.RadioStation> Stations
        {
            get { return stations; }
            protected set { stations = value; }
        }

        /// <summary>List of all instantiated RadioPlayer.</summary>
        public System.Collections.Generic.List<RadioPlayer> Players
        {
            get { return players; }
            protected set { players = value; }
        }

        /// <summary>Are all providers of this manager ready (= data loaded)?</summary>
        /// <returns>True if all providers of this manager are ready.</returns>
        public bool isReady
        {
            //get;
            //protected set;


            get
            {
                if (Providers != null)
                {
                    foreach (Provider.BaseRadioProvider provider in Providers)
                    {
                        if (provider != null && !provider.isReady)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        /// <summary>Is any of the RadioPlayers in playback-mode?</summary>
        /// <returns>True if any of the RadioPlayers is in playback-mode.</returns>
        public bool isPlayback
        {
            get
            {
                foreach (RadioPlayer rp in Players)
                {
                    if (rp != null && rp.isPlayback)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>Is any of the RadioPlayers playing audio?</summary>
        /// <returns>True if any of the RadioPlayers is playing audio.</returns>
        public bool isAudioPlaying
        {
            get
            {
                foreach (RadioPlayer rp in Players)
                {
                    if (rp != null && rp.isAudioPlaying)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>Is any of the RadioPlayers buffering?</summary>
        /// <returns>True if any of the RadioPlayers is buffering.</returns>
        public bool isBuffering
        {
            get
            {
                foreach (RadioPlayer rp in Players)
                {
                    if (rp != null && rp.isBuffering)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        #endregion


        #region Events

        public delegate void ProviderReady();

        /// <summary>An event triggered whenever all providers are ready.</summary>
        public event ProviderReady OnProviderReady
        {
            add { _providerReady += value; }
            remove { _providerReady -= value; }
        }

        private ProviderReady _providerReady;

        #endregion


        #region MonoBehaviour methods

        public void Start()
        {
            if (LoadOnStart && !Util.Helper.isEditorMode)
            {
                //this.CTInvoke(() => Load(), 0.1f);
                Invoke("Load", 0.1f);
            }

            if (LoadOnStartInEditor && Util.Helper.isEditorMode)
            {
                Load();
            }
        }

        #endregion


        #region Public methods

        /// <summary>Loads all stations from this manager (via providers).</summary>
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
                StartCoroutine(init());
            }
        }

        /// <summary>Saves all stations from this manager as text-file with streams.</summary>
        /// <param name="path">Path to the text-file.</param>
        /// <param name="filter">Filter (default: null, optional)</param>
		public void Save(string path, Model.RadioFilter filter = null) //TODO add more file formats (M3U&PLS)
        {
            //Debug.LogWarning("Save not implemented!");

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

                        foreach (Model.RadioStation rs in StationsByStation(false, getFilter(filter)))
                        {
                            file.WriteLine(rs.ToTextLine());
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Could not save file: " + path + System.Environment.NewLine + ex);
                }
            }
            else
            {
                Debug.LogWarning("'path' was null or empty! Could not save the data!");
            }
        }

        /// <summary>Randomize all radio players.</summary>
        /// <param name="resetIndex">Reset the index of the random radio stations (default: true, optional)</param>
        public void RandomizePlayers(bool resetIndex = true)
        {
            randomPlayers.CTShuffle();

            if (resetIndex)
            {
                randomPlayerIndex = 0;
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

        /// <summary>Play all radios of this manager at once.</summary>
        public void PlayAll()
        {
            StopAll();

            foreach (RadioPlayer rp in Players)
            {
                if (rp != null)
                {
                    rp.Play();
                }
            }
        }

        /// <summary>Radio player by index (normal/random) from this manager.</summary>
        /// <param name="random">Return a random radio player (default: false, optional)</param>
        /// <param name="index">Index of the radio player (default: 0, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>Radio player by index.</returns>
        public RadioPlayer PlayerByIndex(bool random = false, int index = 0, Model.RadioFilter filter = null)
        {
            return playerByIndex(random, index, filter);
        }
        
        /// <summary>Next (normal/random) radio from this manager.</summary>
        /// <param name="random">Return a random radio player (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <param name="stopAll">Stops all radios of this manager (default: true, optional)</param>
        /// <param name="playImmediately">Plays the radio (default: true, optional)</param>
        /// <returns>Next radio station.</returns>
        public RadioPlayer Next(bool random = false, Model.RadioFilter filter = null, bool stopAll = true, bool playImmediately = true)
        {
            //RadioPlayer rp = null;

            if (Players != null && Players.Count > 0)
            {
                if (stopAll)
                {
                    StopAll();
                }

                currentRadioPlayer = nextPlayer(random, getFilter(filter));

                if (stopAll && playImmediately)
                {
                    //this.CTInvoke(() => play(), Util.Constants.INVOKE_DELAY);
                    Invoke("play", Util.Constants.INVOKE_DELAY);
                }
                else
                {
                    play();
                }
            }
            else
            {
                Debug.LogWarning("No 'Players' found. Can't play next radio station and returning null.");
            }

            return currentRadioPlayer;
        }

        /// <summary>Previous (normal/random) radio from this manager.</summary>
        /// <param name="random">Return a random radio player (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <param name="stopAll">Stops all radios of this manager (default: true, optional)</param>
        /// <param name="playImmediately">Plays the radio (default: true, optional)</param>
        /// <returns>Previous radio station.</returns>
        public RadioPlayer Previous(bool random = false, Model.RadioFilter filter = null, bool stopAll = true, bool playImmediately = true)
        {
            //RadioPlayer rp = null;

            if (Players != null && Players.Count > 0)
            {
                if (stopAll)
                {
                    StopAll();
                }

                currentRadioPlayer = previousPlayer(random, getFilter(filter));

                if (stopAll && playImmediately)
                {
                    //this.CTInvoke(() => play(), Util.Constants.INVOKE_DELAY);
                    Invoke("play", Util.Constants.INVOKE_DELAY);
                }
                else
                {
                    play();
                }
            }
            else
            {
                Debug.LogWarning("No 'Players' found. Can't play previous radio station and returning null.");
            }

            return currentRadioPlayer;
        }

        /// <summary>Radio station by index (normal/random) from this manager.</summary>
        /// <param name="random">Return a random radio station (default: false, optional)</param>
        /// <param name="index">Index of the radio station (default: 0, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>Radio station by index.</returns>
        public Model.RadioStation StationByIndex(bool random = false, int index = 0, Model.RadioFilter filter = null)
        {
            return stationByIndex(random, index, filter);
        }
        
        /// <summary>Next (normal/random) radio station from this manager.</summary>
        /// <param name="random">Return a random radio station (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>Next radio station.</returns>
        public Model.RadioStation NextStation(bool random = false, Model.RadioFilter filter = null)
        {
            if (Stations != null && Stations.Count > 0)
            {
                return nextStation(random, getFilter(filter));
            }
            else
            {
                Debug.LogWarning("No 'Stations' found: returning null.");
            }

            return null;
        }

        /// <summary>Previous (normal/random) radio station from this manager.</summary>
        /// <param name="random">Return a random radio station (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>Previous radio station.</returns>
        public Model.RadioStation PreviousStation(bool random = false, Model.RadioFilter filter = null)
        {
            if (Stations != null && Stations.Count > 0)
            {
                return previousStation(random, getFilter(filter));
            }
            else
            {
                Debug.LogWarning("No 'Stations' found: returning null.");
            }

            return null;
        }

        /// <summary>Stops all radios of this manager at once.</summary>
        /// <param name="resetIndex">Reset the index of the radio stations (default: false, optional)</param>
        public void StopAll(bool resetIndex = false)
        {
            foreach (RadioPlayer rp in Players)
            {
                if (rp != null)
                {
                    rp.Stop();
                }
            }

            if (resetIndex)
            {
                playerIndex = randomPlayerIndex = 0;
                //         } else {
                //            playerIndex--;
                //            randomPlayerIndex--;
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

        /// <summary>Count all RadioPlayer for a given RadioFilter.</summary>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>Number of all RadioPlayer for a given RadioFilter.</returns>
        public int CountPlayers(Model.RadioFilter filter = null)
        {
            if (getFilter(filter) == null)
            {
                return Players.Count();
            }
            else
            {
                return filterPlayers(false, getFilter(filter)).ToList<RadioPlayer>().Count();
            }
        }


        /// <summary>Returns all radios of this manager ordered by name.</summary>
        /// <param name="desc">Descending order (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>All radios of this manager ordered by name.</returns>
        public System.Collections.Generic.List<RadioPlayer> PlayersByName(bool desc = false, Model.RadioFilter filter = null)
        {
            System.Collections.Generic.IEnumerable<RadioPlayer> entries = filterPlayers(false, getFilter(filter));

            if (desc)
            {
                return new System.Collections.Generic.List<RadioPlayer>(entries.OrderByDescending(entry => entry.Station.Name));
            }
            else
            {
                return new System.Collections.Generic.List<RadioPlayer>(entries.OrderBy(entry => entry.Station.Name));
            }
        }

        /// <summary>Returns all radios of this manager ordered by URL.</summary>
        /// <param name="desc">Descending order (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>All radios of this manager ordered by URL.</returns>
        public System.Collections.Generic.List<RadioPlayer> PlayersByURL(bool desc = false, Model.RadioFilter filter = null)
        {
            System.Collections.Generic.IEnumerable<RadioPlayer> entries = filterPlayers(false, getFilter(filter));

            if (desc)
            {
                return new System.Collections.Generic.List<RadioPlayer>(entries.OrderByDescending(entry => entry.Station.Url).ThenBy((entry => entry.Station.Name)));
            }
            else
            {
                return new System.Collections.Generic.List<RadioPlayer>(entries.OrderBy(entry => entry.Station.Url).ThenBy((entry => entry.Station.Name)));
            }
        }

        /// <summary>Returns all radios of this manager ordered by audio format.</summary>
        /// <param name="desc">Descending order (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>All radios of this manager ordered by audio format.</returns>
        public System.Collections.Generic.List<RadioPlayer> PlayersByFormat(bool desc = false, Model.RadioFilter filter = null)
        {
            System.Collections.Generic.IEnumerable<RadioPlayer> entries = filterPlayers(false, getFilter(filter));

            if (desc)
            {
                return new System.Collections.Generic.List<RadioPlayer>(entries.OrderByDescending(entry => entry.Station.Format).ThenBy((entry => entry.Station.Name)));
            }
            else
            {
                return new System.Collections.Generic.List<RadioPlayer>(entries.OrderBy(entry => entry.Station.Format).ThenBy((entry => entry.Station.Name)));
            }
        }

        /// <summary>Returns all radios of this manager ordered by station.</summary>
        /// <param name="desc">Descending order (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>All radios of this manager ordered by station.</returns>
        public System.Collections.Generic.List<RadioPlayer> PlayersByStation(bool desc = false, Model.RadioFilter filter = null)
        {
            System.Collections.Generic.IEnumerable<RadioPlayer> entries = filterPlayers(false, getFilter(filter));

            if (desc)
            {
                return new System.Collections.Generic.List<RadioPlayer>(entries.OrderByDescending(entry => entry.Station.Station).ThenBy((entry => entry.Station.Name)));
            }
            else
            {
                return new System.Collections.Generic.List<RadioPlayer>(entries.OrderBy(entry => entry.Station.Station).ThenBy((entry => entry.Station.Name)));
            }
        }

        /// <summary>Returns all radios of this manager ordered by bitrate.</summary>
        /// <param name="desc">Descending order (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>All radios of this manager ordered by bitrate.</returns>
        public System.Collections.Generic.List<RadioPlayer> PlayersByBitrate(bool desc = false, Model.RadioFilter filter = null)
        {
            System.Collections.Generic.IEnumerable<RadioPlayer> entries = filterPlayers(false, getFilter(filter));

            if (desc)
            {
                return new System.Collections.Generic.List<RadioPlayer>(entries.OrderByDescending(entry => entry.Station.Bitrate).ThenBy((entry => entry.Station.Name)));
            }
            else
            {
                return new System.Collections.Generic.List<RadioPlayer>(entries.OrderBy(entry => entry.Station.Bitrate).ThenBy((entry => entry.Station.Name)));
            }
        }

        /// <summary>Returns all radios of this manager ordered by genres.</summary>
        /// <param name="desc">Descending order (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>All radios of this manager ordered by genre.</returns>
        public System.Collections.Generic.List<RadioPlayer> PlayersByGenres(bool desc = false, Model.RadioFilter filter = null)
        {
            System.Collections.Generic.IEnumerable<RadioPlayer> entries = filterPlayers(false, getFilter(filter));

            if (desc)
            {
                return new System.Collections.Generic.List<RadioPlayer>(entries.OrderByDescending(entry => entry.Station.Genres).ThenBy((entry => entry.Station.Name)));
            }
            else
            {
                return new System.Collections.Generic.List<RadioPlayer>(entries.OrderBy(entry => entry.Station.Genres).ThenBy((entry => entry.Station.Name)));
            }
        }

        /// <summary>Returns all radios of this manager ordered by rating.</summary>
        /// <param name="desc">Descending order (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>All radios of this manager ordered by rating.</returns>
        public System.Collections.Generic.List<RadioPlayer> PlayersByRating(bool desc = false, Model.RadioFilter filter = null)
        {
            System.Collections.Generic.IEnumerable<RadioPlayer> entries = filterPlayers(false, getFilter(filter));

            if (desc)
            {
                return new System.Collections.Generic.List<RadioPlayer>(entries.OrderByDescending(entry => entry.Station.Rating).ThenBy((entry => entry.Station.Name)));
            }
            else
            {
                return new System.Collections.Generic.List<RadioPlayer>(entries.OrderBy(entry => entry.Station.Rating).ThenBy((entry => entry.Station.Name)));
            }
        }

        /// <summary>Returns all radio stations of this manager ordered by name.</summary>
        /// <param name="desc">Descending order (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>All radios of this manager ordered by name.</returns>
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

        /// <summary>Returns all radio stations of this manager ordered by URL.</summary>
        /// <param name="desc">Descending order (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>All radios of this manager ordered by URL.</returns>
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

        /// <summary>Returns all radio stations of this manager ordered by audio format.</summary>
        /// <param name="desc">Descending order (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>All radios of this manager ordered by audio format.</returns>
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

        /// <summary>Returns all radio stations of this manager ordered by station.</summary>
        /// <param name="desc">Descending order (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>All radios of this manager ordered by station.</returns>
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

        /// <summary>Returns all radio stations of this manager ordered by bitrate.</summary>
        /// <param name="desc">Descending order (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>All radios of this manager ordered by bitrate.</returns>
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

        /// <summary>Returns all radio stations of this manager ordered by genres.</summary>
        /// <param name="desc">Descending order (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>All radios of this manager ordered by genre.</returns>
        public System.Collections.Generic.List<Model.RadioStation> StationsByGenres(bool desc = false, Model.RadioFilter filter = null)
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

        /// <summary>Returns all radio stations of this manager ordered by rating.</summary>
        /// <param name="desc">Descending order (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        /// <returns>All radios of this manager ordered by rating.</returns>
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

        private IEnumerator init()
        {

            Stations.Clear();
            randomStations.Clear();

            Players.Clear();
            randomPlayers.Clear();

            if (Providers != null)
            {

                while (!isReady)
                {
                    yield return null;
                }

                foreach (Provider.BaseRadioProvider provider in Providers)
                {

                    if (provider != null)
                    {
                        foreach (Model.RadioStation station in provider.Stations)
                        {
                            if (!Stations.Contains(station))
                            {
                                Stations.Add(station);
                                //Debug.Log("STATION: " + station);
                                randomStations.Add(station);

                                if (InstantiateRadioPlayers)
                                {
                                    if (RadioPrefab != null)
                                    {

                                        GameObject go = Instantiate(RadioPrefab) as GameObject;
                                        go.transform.parent = transform;

                                        RadioPlayer rp = go.GetComponent<RadioPlayer>();

                                        rp.Station = station;

                                        Players.Add(rp);
                                        randomPlayers.Add(rp);

                                        if (Util.Config.DEBUG)
                                            Debug.Log("Radio station found: " + rp);
                                    }
                                }

                            }
                            else
                            {
                                Debug.LogWarning("Station already added: '" + station + "'");
                            }
                        }
                    }

                    yield return null;
                }

                RandomizePlayers();
                RandomizeStations();
            }

            //isReady = true;

            if (_providerReady != null)
            {
                _providerReady();
            }
        }

        private System.Collections.Generic.IEnumerable<RadioPlayer> filterPlayers(bool random = false, Model.RadioFilter filter = null)
        {
            if (random)
            {
                if (filter != null && filter.isFiltering)
                {
                    return from entry in randomPlayers
                           where (filter == null) || ((string.IsNullOrEmpty(entry.Station.Name) || entry.Station.Name.CTContainsAny(filter.Name)) &&
                              (string.IsNullOrEmpty(entry.Station.Station) || entry.Station.Station.CTContainsAny(filter.Station)) &&
                              (string.IsNullOrEmpty(entry.Station.Url) || entry.Station.Url.CTContainsAll(filter.Url)) &&
                              (string.IsNullOrEmpty(entry.Station.Genres) || entry.Station.Genres.CTContainsAny(filter.Genres)) &&
                              (entry.Station.Format.ToString().CTContainsAny(filter.Format)) &&
                              (entry.Station.Bitrate >= filter.BitrateMin && entry.Station.Bitrate <= filter.BitrateMax) &&
                              (entry.Station.Rating >= filter.RatingMin && entry.Station.Rating <= filter.RatingMax)) &&
                        ((!filter.ExcludeUnsupportedCodecs || entry.Station.ExcludedCodec == Model.Enum.AudioCodec.None) || entry.Station.ExcludedCodec != Util.Helper.AudioCodecForAudioFormat(entry.Station.Format))
                           select entry;
                }
                else
                {
                    return randomPlayers;
                }

            }
            else
            {
                if (filter != null && filter.isFiltering)
                {
                    return from entry in Players
                           where (filter == null) || ((string.IsNullOrEmpty(entry.Station.Name) || entry.Station.Name.CTContainsAny(filter.Name)) &&
                              (string.IsNullOrEmpty(entry.Station.Station) || entry.Station.Station.CTContainsAny(filter.Station)) &&
                              (string.IsNullOrEmpty(entry.Station.Url) || entry.Station.Url.CTContainsAll(filter.Url)) &&
                              (string.IsNullOrEmpty(entry.Station.Genres) || entry.Station.Genres.CTContainsAny(filter.Genres)) &&
                              (entry.Station.Format.ToString().CTContainsAny(filter.Format)) &&
                              (entry.Station.Bitrate >= filter.BitrateMin && entry.Station.Bitrate <= filter.BitrateMax) &&
                              (entry.Station.Rating >= filter.RatingMin && entry.Station.Rating <= filter.RatingMax)) &&
                        ((!filter.ExcludeUnsupportedCodecs || entry.Station.ExcludedCodec == Model.Enum.AudioCodec.None) || entry.Station.ExcludedCodec != Util.Helper.AudioCodecForAudioFormat(entry.Station.Format))
                           select entry;
                }
                else
                {
                    return Players;
                }
            }
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

        private void play()
        {
            currentRadioPlayer.Play();
        }

        private RadioPlayer playerByIndex(bool random = false, int index = 0, Model.RadioFilter filter = null)
        {
            System.Collections.Generic.List<RadioPlayer> players;

            if (random)
            {
                players = new System.Collections.Generic.List<RadioPlayer>(filterPlayers(random, filter));

                if (index > -1 && index < players.Count)
                {
                    randomPlayerIndex = index;
                    return players[index];
                }
            }
            else
            {
                players = PlayersByName(false, filter);

                if (index > -1 && index < players.Count)
                {
                    playerIndex = index;
                    return players[index];
                }
            }

            return null;
        }
        
        private RadioPlayer nextPlayer(bool random = false, Model.RadioFilter filter = null)
        {
            System.Collections.Generic.List<RadioPlayer> players;

            if (random)
            {
                players = new System.Collections.Generic.List<RadioPlayer>(filterPlayers(random, filter));

                if (randomPlayerIndex > -1 && randomPlayerIndex + 1 < players.Count)
                {
                    randomPlayerIndex++;
                }
                else
                {
                    randomPlayerIndex = 0;
                }

                if (players.Count > 0)
                {
                    return players[randomPlayerIndex];
                }
            }
            else
            {
                players = PlayersByName(false, filter);

                if (playerIndex > -1 && playerIndex + 1 < players.Count)
                {
                    playerIndex++;
                }
                else
                {
                    playerIndex = 0;
                }

                if (players.Count > 0)
                {
                    return players[playerIndex];
                }
            }

            return null;
        }

        private RadioPlayer previousPlayer(bool random = false, Model.RadioFilter filter = null)
        {
            System.Collections.Generic.List<RadioPlayer> players;

            if (random)
            {
                players = new System.Collections.Generic.List<RadioPlayer>(filterPlayers(random, filter));

                if (randomPlayerIndex > 0 && randomPlayerIndex < players.Count)
                {
                    randomPlayerIndex--;
                }
                else
                {
                    randomPlayerIndex = players.Count - 1;
                }

                if (players.Count > 0)
                {
                    return players[randomPlayerIndex];
                }
            }
            else
            {
                players = PlayersByName(false, filter);

                if (playerIndex > 0 && playerIndex < players.Count)
                {
                    playerIndex--;
                }
                else
                {
                    playerIndex = players.Count - 1;
                }

                if (players.Count > 0)
                {
                    return players[playerIndex];
                }
            }

            return null;
        }
        
        private Model.RadioStation stationByIndex(bool random = false, int index = 0, Model.RadioFilter filter = null)
        {
            System.Collections.Generic.List<Model.RadioStation> stations;

            if (random)
            {
                stations = new System.Collections.Generic.List<Model.RadioStation>(filterStations(random, filter));

                if (index > -1 && index < stations.Count)
                {
                    randomStationIndex = index;
                    return stations[index];
                }
            }
            else
            {
                stations = StationsByName(false, filter);

                if (index > -1 && index < stations.Count)
                {
                    stationIndex = index;
                    return stations[index];
                }
            }

            return null;
        }
        
        private Model.RadioStation nextStation(bool random = false, Model.RadioFilter filter = null)
        {
            System.Collections.Generic.List<Model.RadioStation> stations;

            if (random)
            {
                stations = new System.Collections.Generic.List<Model.RadioStation>(filterStations(random, filter));

                if (randomStationIndex > -1 && randomStationIndex + 1 < stations.Count)
                {
                    randomStationIndex++;
                }
                else
                {
                    randomStationIndex = 0;
                }

                if (stations.Count > 0)
                {
                    return stations[randomStationIndex];
                }
            }
            else
            {
                stations = StationsByName(false, filter);

                if (stationIndex > -1 && stationIndex + 1 < stations.Count)
                {
                    stationIndex++;
                }
                else
                {
                    stationIndex = 0;
                }

                if (stations.Count > 0)
                {
                    return stations[stationIndex];
                }
            }

            return null;
        }

        private Model.RadioStation previousStation(bool random = false, Model.RadioFilter filter = null)
        {
            System.Collections.Generic.List<Model.RadioStation> stations;

            if (random)
            {
                stations = new System.Collections.Generic.List<Model.RadioStation>(filterStations(random, filter));

                if (randomStationIndex > 0 && randomStationIndex < stations.Count)
                {
                    randomStationIndex--;
                }
                else
                {
                    randomStationIndex = stations.Count - 1;
                }

                if (stations.Count > 0)
                {
                    return stations[randomStationIndex];
                }
            }
            else
            {
                stations = StationsByName(false, filter);

                if (stationIndex > 0 && stationIndex < stations.Count)
                {
                    stationIndex--;
                }
                else
                {
                    stationIndex = stations.Count - 1;
                }

                if (stations.Count > 0)
                {
                    return stations[stationIndex];
                }
            }

            return null;
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


        #region Overridden methods

        public override string ToString()
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();

            result.Append(GetType().Name);
            result.Append(Util.Constants.TEXT_TOSTRING_START);

            result.Append("Providers='");
            result.Append(Providers);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("LoadOnStart='");
            result.Append(LoadOnStart);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("LoadOnStartInEditor='");
            result.Append(LoadOnStartInEditor);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("InstantiateRadioPlayers='");
            result.Append(InstantiateRadioPlayers);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("RadioPrefab='");
            result.Append(RadioPrefab);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER_END);

            result.Append(Util.Constants.TEXT_TOSTRING_END);

            return result.ToString();
        }

        #endregion


        #region Editor-only methods

#if UNITY_EDITOR

        private void initInEditor()
        {

            Stations.Clear();
            randomStations.Clear();

            Players.Clear();
            randomPlayers.Clear();

            if (Providers != null)
            {
                float time = Time.realtimeSinceStartup;

                do
                {
                    // waiting...
                } while (Time.realtimeSinceStartup - time < Util.Constants.MAX_LOAD_WAIT_TIME && !isReady);

                //Debug.Log("Manager waited: " + (Time.realtimeSinceStartup - time));

                foreach (Provider.BaseRadioProvider provider in Providers)
                {

                    if (provider != null)
                    {

                        foreach (Model.RadioStation station in provider.Stations)
                        {
                            if (!Stations.Contains(station))
                            {
                                Stations.Add(station);
                                randomStations.Add(station);
                            }
                            else
                            {
                                Debug.LogWarning("Station already added: '" + station + "'");
                            }
                        }
                    }
                }

                RandomizeStations();

                //isReady = true;

                if (_providerReady != null)
                {
                    _providerReady();
                }
            }
        }

#endif

        #endregion
    }
}
// © 2015-2017 crosstales LLC (https://www.crosstales.com)