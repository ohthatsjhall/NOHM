using System;
using UnityEngine;

namespace Crosstales.Radio
{
    /// <summary>Simple player.</summary>
    [ExecuteInEditMode]
    [HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_simple_player.html")]
    public class SimplePlayer : MonoBehaviour
    {

        #region Variables

        [Header("General Settings")]
        /// <summary>'RadioPlayer' from the scene.</summary>
        [Tooltip("'RadioPlayer' from the scene.")]
        public RadioPlayer Player;

        /// <summary>'RadioManager' from the scene.</summary>
        [Tooltip("'RadioManager' from the scene.")]
        public RadioManager Manager;

        /// <summary>Global RadioFilter (active if no explicit filter is given).</summary>
        [Tooltip("Global RadioFilter (active if no explicit filter is given).")]
        public Model.RadioFilter Filter;


        [Header("Retry Settings")]

        /// <summary>Retry to start the radio on an error (default: false).</summary>
        [Tooltip("Retry to start the radio on an error (default: false).")]
        public bool RetryOnError = false;

        /// <summary>How many times should the radio station restart after an error before giving up (default: 3).</summary>
        [Tooltip("How many times should the radio station restart after an error before giving up (default: 3).")]
        public int Retries = 3;


        [Header("Behaviour Settings")]

        /// <summary>Play a radio on start (default: false).</summary>
        [Tooltip("Play a radio on start (default: false).")]
        public bool PlayOnStart = false;

        /// <summary>Play the radio stations in random order(default: false).</summary>
        [Tooltip("Play the radio stations in random order (default: false).")]
        public bool PlayRandom = false;

        private int invokeDelayCounter = 1;
        private bool started = false;
        //private int playtime = 0;
        private float lastPlaytime = float.MinValue;

        #endregion


        #region Properties

        /// <summary>Retry to start the radio on an error  (main use is for UI).</summary>
        public bool isRetryOnError
        {
            get
            {
                return RetryOnError;
            }
            set
            {
                RetryOnError = value;
            }
        }

        /// <summary>Play the radio stations in random order (main use is for UI).</summary>
        public bool isPlayRandom
        {
            get
            {
                return PlayRandom;
            }
            set
            {
                PlayRandom = value;
            }
        }

        /// <summary>Radio station of this player.</summary>
        public Model.RadioStation Station
        {
            get
            {
                if (Player != null)
                {
                    return Player.Station;
                }
                //Debug.Log("Returning null!");

                return null;
            }

            set
            {
                if (Player != null)
                {
                    Player.Station = value;
                }
            }
        }

        /// <summary>Returns the AudioSource of for this player.</summary>
        /// <returns>The AudioSource for this player.</returns>
        public AudioSource Source
        {
            get
            {
                if (Player != null)
                {
                    return Player.Source;
                }
                return null;
            }
        }

        /// <summary>Returns the codec of for this player.</summary>
        /// <returns>The codec for this player.</returns>
        public Model.Enum.AudioCodec Codec
        {
            get
            {
                if (Player != null)
                {
                    return Player.Codec;
                }
                return Model.Enum.AudioCodec.MP3_NLayer;
            }
        }

        /// <summary>Returns the current playtime of this player.</summary>
        /// <returns>The current playtime of this player.</returns>
        public float PlayTime
        {
            get
            {
                if (Player != null)
                {
                    return Player.PlayTime;
                }
                return 0f;
            }
        }

        /// <summary>Returns the current buffer progress in percent.</summary>
        /// <returns>The current buffer progress in percent.</returns>
        public float BufferProgress
        {
            get
            {
                if (Player != null)
                {
                    return Player.BufferProgress;
                }
                return 0f;
            }
        }

        /// <summary>Returns the list of all loaded RadioStation from all providers of this manager.</summary>
        /// <returns>List of all loaded RadioStation from all providers of this manager.</returns>
        public System.Collections.Generic.List<Model.RadioStation> Stations
        {
            get
            {
                if (Manager != null)
                {
                    if (Filter.isFiltering)
                    {
                        return Manager.StationsByName(false, Filter);
                    }
                    else
                    {
                        return Manager.StationsByName(false, null);
                    }
                    //return Manager.Stations;
                }
                return new System.Collections.Generic.List<Model.RadioStation>();
            }
        }

        /// <summary>Returns the list of all instantiated RadioPlayer.</summary>
        /// <returns>List of all instantiated RadioPlayer.</returns>
        public System.Collections.Generic.List<RadioPlayer> Players
        {
            get
            {
                if (Manager != null)
                {
                    if (Filter.isFiltering)
                    {
                        return Manager.PlayersByName(false, Filter);
                    }
                    else
                    {
                        return Manager.PlayersByName(false, null);
                    }
                    //return Manager.Players;
                }
                return new System.Collections.Generic.List<RadioPlayer>();
            }
        }

        /// <summary>Is this player in playback-mode?</summary>
        /// <returns>True if this player is in playback-mode.</returns>
        public bool isPlayback
        {
            get
            {
                if (Player != null)
                {
                    return Player.isPlayback;
                }
                return false;
            }
        }

        /// <summary>Is this player playing audio?</summary>
        /// <returns>True if this player is playing audio.</returns>
        public bool isAudioPlaying
        {
            get
            {
                if (Player != null)
                {
                    return Player.isAudioPlaying;
                }
                return false;
            }
        }

        /// <summary>Is this player buffering?</summary>
        /// <returns>True if this player is buffering.</returns>
        public bool isBuffering
        {
            get
            {
                if (Player != null)
                {
                    return Player.isBuffering;
                }
                return false;
            }
        }

        /// <summary>Returns the playtime of the current audio record.</summary>
        /// <returns>Playtime of the current audio record.</returns>
        public float RecordPlayTime
        {
            get
            {
                if (Player != null)
                {
                    return Player.RecordPlayTime;
                }
                return 0f;
            }

        }

        /// <summary>Returns the information about the current audio record.</summary>
        /// <returns>Information about the current audio record.</returns>
        public Model.RecordInfo RecordInfo
        {
            get
            {
                if (Player != null)
                {
                    return Player.RecordInfo;
                }
                return new Model.RecordInfo();
            }
        }

        /// <summary>Are all providers of this player ready (= data loaded)?</summary>
        /// <returns>True if all providers of this player are ready.</returns>
        public bool isReady
        {
            get
            {
                if (Manager != null)
                {
                    return Manager.isReady;
                }
                return false;
            }
        }

        #endregion


        #region Events

        public delegate void PlaybackStart(Model.RadioStation station);
        public delegate void PlaybackEnd(Model.RadioStation station);

        public delegate void BufferingStart(Model.RadioStation station);
        public delegate void BufferingEnd(Model.RadioStation station);
        public delegate void BufferingProgressUpdate(Model.RadioStation station, float progress);

        public delegate void AudioStart(Model.RadioStation station);
        public delegate void AudioEnd(Model.RadioStation station);
        public delegate void AudioPlayTimeUpdate(Model.RadioStation station, float playtime);

        public delegate void RecordChange(Model.RadioStation station, Model.RecordInfo newRecord);
        public delegate void RecordPlayTimeUpdate(Model.RadioStation station, Model.RecordInfo record, float playtime);

        public delegate void NextRecordChange(Model.RadioStation station, Model.RecordInfo nextRecord, float delay);
        public delegate void NextRecordDelayUpdate(Model.RadioStation station, Model.RecordInfo nextRecord, float delay);

        public delegate void ProviderReady();

        public delegate void StationChange(Model.RadioStation newStation);

        public delegate void ErrorInfo(Model.RadioStation station, string info);

        /// <summary>An event triggered whenever the playback starts.</summary>
        public event PlaybackStart OnPlaybackStart
        {
            add { _playbackStart += value; }
            remove { _playbackStart -= value; }
        }

        /// <summary>An event triggered whenever the playback ends.</summary>
        public event PlaybackEnd OnPlaybackEnd
        {
            add { _playbackEnd += value; }
            remove { _playbackEnd -= value; }
        }

        /// <summary>An event triggered whenever the buffering starts.</summary>
        public event BufferingStart OnBufferingStart
        {
            add { _bufferingStart += value; }
            remove { _bufferingStart -= value; }
        }

        /// <summary>An event triggered whenever the buffering ends.</summary>
        public event BufferingEnd OnBufferingEnd
        {
            add { _bufferingEnd += value; }
            remove { _bufferingEnd -= value; }
        }

        /// <summary>An event triggered whenever the buffering progress changes.</summary>
        public event BufferingProgressUpdate OnBufferingProgressUpdate
        {
            add { _bufferingProgressUpdate += value; }
            remove { _bufferingProgressUpdate -= value; }
        }

        /// <summary>An event triggered whenever the audio starts.</summary>
        public event AudioStart OnAudioStart
        {
            add { _audioStart += value; }
            remove { _audioStart -= value; }
        }

        /// <summary>An event triggered whenever the audio ends.</summary>ry>
        public event AudioEnd OnAudioEnd
        {
            add { _audioEnd += value; }
            remove { _audioEnd -= value; }
        }

        /// <summary>An event triggered whenever the audio playtime changes.</summary>
        public event AudioPlayTimeUpdate OnAudioPlayTimeUpdate
        {
            add { _audioPlayTimeUpdate += value; }
            remove { _audioPlayTimeUpdate -= value; }
        }

        /// <summary>An event triggered whenever an audio record changes.</summary>
        public event RecordChange OnRecordChange
        {
            add { _recordChange += value; }
            remove { _recordChange -= value; }
        }

        /// <summary>An event triggered whenever the audio record playtime changes.</summary>
        public event RecordPlayTimeUpdate OnRecordPlayTimeUpdate
        {
            add { _recordPlayTimeUpdate += value; }
            remove { _recordPlayTimeUpdate -= value; }
        }

        /// <summary>An event triggered whenever the next record information is available.</summary>
        public event NextRecordChange OnNextRecordChange
        {
            add { _nextRecordChange += value; }
            remove { _nextRecordChange -= value; }
        }

        /// <summary>An event triggered whenever the next record delay time changes.</summary>
        public event NextRecordDelayUpdate OnNextRecordDelayUpdate
        {
            add { _nextRecordDelayUpdate += value; }
            remove { _nextRecordDelayUpdate -= value; }
        }


        /// <summary>An event triggered whenever all providers are ready.</summary>
        public event ProviderReady OnProviderReady
        {
            add { _providerReady += value; }
            remove { _providerReady -= value; }
        }

        /// <summary>An event triggered whenever an radio station changes.</summary>
        public event StationChange OnStationChange
        {
            add { _stationChange += value; }
            remove { _stationChange -= value; }
        }

        /// <summary>An event triggered whenever an error occurs.</summary>
        public event ErrorInfo OnErrorInfo
        {
            add { _errorInfo += value; }
            remove { _errorInfo -= value; }
        }

        private PlaybackStart _playbackStart;
        private PlaybackEnd _playbackEnd;

        private BufferingStart _bufferingStart;
        private BufferingEnd _bufferingEnd;
        private BufferingProgressUpdate _bufferingProgressUpdate;

        private AudioStart _audioStart;
        private AudioEnd _audioEnd;
        private AudioPlayTimeUpdate _audioPlayTimeUpdate;

        private RecordChange _recordChange;
        private RecordPlayTimeUpdate _recordPlayTimeUpdate;

        private NextRecordChange _nextRecordChange;
        private NextRecordDelayUpdate _nextRecordDelayUpdate;

        private ProviderReady _providerReady;

        private StationChange _stationChange;

        private ErrorInfo _errorInfo;

        #endregion


        #region MonoBehaviour methods

        public void OnEnable()
        {
            //Debug.Log(System.Environment.Version);

            if (Player != null && Manager != null)
            {
                // Subscribe event listeners
                Player.OnPlaybackStart += onPlaybackStart;
                Player.OnPlaybackEnd += onPlaybackEnd;
                Player.OnAudioStart += onAudioStart;
                Player.OnAudioEnd += onAudioEnd;
                Player.OnAudioPlayTimeUpdate += onAudioPlayTimeUpdate;
                Player.OnBufferingStart += onBufferingStart;
                Player.OnBufferingEnd += onBufferingEnd;
                Player.OnBufferingProgressUpdate += onBufferingProgressUpdate;
                Player.OnErrorInfo += onErrorInfo;
                Player.OnRecordChange += onRecordChange;
                Player.OnRecordPlayTimeUpdate += onRecordPlayTimeUpdate;
                Player.OnNextRecordChange += onNextRecordChange;
                Player.OnNextRecordDelayUpdate += onNextRecordDelayUpdate;
                Manager.OnProviderReady += onProviderReady;
            }
            else
            {
                if (!Util.Helper.isEditorMode)
                {
                    Debug.LogError("'Player' or 'Manager' are null!");
                }
            }
        }

        public void OnDisable()
        {
            if (Player != null && Manager != null)
            {
                // Unsubscribe event listeners
                Player.OnPlaybackStart -= onPlaybackStart;
                Player.OnPlaybackEnd -= onPlaybackEnd;
                Player.OnAudioStart += onAudioStart;
                Player.OnAudioEnd += onAudioEnd;
                Player.OnAudioPlayTimeUpdate -= onAudioPlayTimeUpdate;
                Player.OnBufferingStart -= onBufferingStart;
                Player.OnBufferingEnd -= onBufferingEnd;
                Player.OnBufferingProgressUpdate -= onBufferingProgressUpdate;
                Player.OnErrorInfo -= onErrorInfo;
                Player.OnRecordChange -= onRecordChange;
                Player.OnRecordPlayTimeUpdate -= onRecordPlayTimeUpdate;
                Player.OnNextRecordChange -= onNextRecordChange;
                Player.OnNextRecordDelayUpdate -= onNextRecordDelayUpdate;
                Manager.OnProviderReady -= onProviderReady;
            }
        }

        #endregion


        #region Public methods

        /// <summary>Plays a radio (main use is for UI).</summary>
        public void Play()
        {
            Play(PlayRandom);
        }

        /// <summary>Plays a (normal/random) radio.</summary>
        /// <param name="random">Play a random radio station (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        public void Play(bool random, Model.RadioFilter filter = null)
        {
            if (Player != null && Manager != null)
            {
                if (lastPlaytime + Util.Constants.PLAY_CALL_SPEED < Time.realtimeSinceStartup)
                {
                    lastPlaytime = Time.realtimeSinceStartup;

                    Stop();

                    //Player.Station = Manager.NextStation(random, getFilter(filter));

                    if (Util.Helper.isEditorMode)
                    {
#if UNITY_EDITOR
                        Player.PlayInEditor();
#endif
                    }
                    else
                    {
                        //this.CTInvoke(() => play(), Util.Constants.INVOKE_DELAY);
                        Invoke("play", Util.Constants.INVOKE_DELAY);
                    }
                }
                else
                {
                    Debug.LogWarning("Play called to fast - please slow down.");
                }
            }
        }

        /// <summary>Plays the next radio (main use for UI).</summary>
        public void Next()
        {
            Next(PlayRandom);
        }

        /// <summary>Plays the next (normal/random) radio.</summary>
        /// <param name="random">Play a random radio station (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        public void Next(bool random, Model.RadioFilter filter = null)
        {
            Player.Station = Manager.NextStation(random, getFilter(filter));

            Play(random, getFilter(filter));
        }

        /// <summary>Plays the previous radio (main use for UI).</summary>
        public void Previous()
        {
            Previous(PlayRandom);
        }

        /// <summary>Plays the previous radio.</summary>
        /// <param name="random">Play a random radio station (default: false, optional)</param>
        /// <param name="filter">Filter (default: null, optional)</param>
        public void Previous(bool random, Model.RadioFilter filter = null)
        {
            if (Player != null && Manager != null)
            {
                //                Stop();

                Player.Station = Manager.PreviousStation(random, getFilter(filter));

                Play(random, getFilter(filter));

                /*
                                if (Util.Helper.isEditorMode)
                                {
                #if UNITY_EDITOR
                                    Player.PlayInEditor();
                #endif
                                }
                                else
                                {
                                    //this.CTInvoke(() => play(), Util.Constants.INVOKE_DELAY);
                                    Invoke("play", Util.Constants.INVOKE_DELAY);
                                }
                */
            }
        }

        /// <summary>Stops the radio station playback.</summary>
        public void Stop()
        {
            if (Player != null)
            {
                Player.Stop();
            }
        }

        #endregion


        #region Private methods

        private void play()
        {
            Player.Play();

            onStationChange(Player.Station);
        }

        private void playInvoker()
        {
            if (started)
            {
                Play();
            }
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


        #region Callback & event-trigger methods

        private void onPlaybackStart(Model.RadioStation station)
        {
            if (_playbackStart != null)
            {
                _playbackStart(station);
            }
        }

        private void onPlaybackEnd(Model.RadioStation station)
        {
            if (_playbackEnd != null)
            {
                _playbackEnd(station);
            }
        }

        private void onAudioStart(Model.RadioStation station)
        {
            started = true;

            if (_audioStart != null)
            {
                _audioStart(station);
            }
        }

        private void onAudioEnd(Model.RadioStation station)
        {
            started = false;

            if (_audioEnd != null)
            {
                _audioEnd(station);
            }
        }

        private void onAudioPlayTimeUpdate(Model.RadioStation station, float _playtime)
        {
            if (_playtime > 30f) //reset restartCounter after 30 seconds
            {
                invokeDelayCounter = 1;
            }

            if (_audioPlayTimeUpdate != null)
            {
                _audioPlayTimeUpdate(station, _playtime);
            }
        }

        private void onBufferingStart(Model.RadioStation station)
        {
            if (_bufferingStart != null)
            {
                _bufferingStart(station);
            }
        }

        private void onBufferingEnd(Model.RadioStation station)
        {
            if (_bufferingEnd != null)
            {
                _bufferingEnd(station);
            }
        }

        private void onBufferingProgressUpdate(Model.RadioStation station, float progress)
        {
            if (_bufferingProgressUpdate != null)
            {
                _bufferingProgressUpdate(station, progress);
            }
        }

        private void onErrorInfo(Model.RadioStation station, string info)
        {
            Stop();
            //onPlayBackEnd();

            if (RetryOnError && Tool.InternetCheck.isInternetAvailable)
            {
                if (started)
                {
                    if (invokeDelayCounter < Retries)
                    {
                        Debug.LogWarning("Error occoured -> Restarting station." + Environment.NewLine + info);
                        //this.CTInvoke(() => playInvoker(), Util.Constants.INVOKE_DELAY * invokeDelayCounter);
                        Invoke("playInvoker", Util.Constants.INVOKE_DELAY * invokeDelayCounter);

                        invokeDelayCounter++;
                    }
                    else
                    {
                        Debug.LogError("Restarting station failed more than " + Retries + " times - giving up!" + Environment.NewLine + info);
                    }
                }
                else
                {
                    Debug.LogError("Could not start the station '" + station.Name + "'! Please try another station. " + Environment.NewLine + info);
                }
            }

            if (_errorInfo != null)
            {
                _errorInfo(station, info);
            }
        }

        private void onProviderReady()
        {
            if (Util.Config.DEBUG)
                Debug.Log("Provider ready - all stations loaded.");

            //Player.Station = Manager.StationByIndex(PlayRandom, 0, Filter);
            Player.Station = Manager.NextStation(PlayRandom, Filter);

            if (!Util.Helper.isEditorMode && PlayOnStart)
            {
                Play(PlayRandom);
            }

            if (_providerReady != null)
            {
                _providerReady();
            }
        }

        private void onStationChange(Model.RadioStation newStation)
        {
            if (_stationChange != null)
            {
                _stationChange(newStation);
            }
        }

        private void onRecordChange(Model.RadioStation station, Model.RecordInfo newRecord)
        {
            if (_recordChange != null)
            {
                _recordChange(station, newRecord);
            }
        }

        private void onRecordPlayTimeUpdate(Model.RadioStation station, Model.RecordInfo record, float playtime)
        {
            if (_recordPlayTimeUpdate != null)
            {
                _recordPlayTimeUpdate(station, record, playtime);
            }
        }

        private void onNextRecordChange(Model.RadioStation station, Model.RecordInfo nextRecord, float delay)
        {
            if (_nextRecordChange != null)
            {
                _nextRecordChange(station, nextRecord, delay);
            }
        }

        private void onNextRecordDelayUpdate(Model.RadioStation station, Model.RecordInfo nextRecord, float delay)
        {
            if (_nextRecordDelayUpdate != null)
            {
                _nextRecordDelayUpdate(station, nextRecord, delay);
            }
        }

        #endregion
    }
}
// © 2016-2017 crosstales LLC (https://www.crosstales.com)