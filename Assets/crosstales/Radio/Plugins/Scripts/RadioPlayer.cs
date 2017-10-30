using UnityEngine;
using System.Collections;

namespace Crosstales.Radio
{
    /// <summary>Player for a radio station.</summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(AudioSource))]
    [HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_radio_player.html")]
    public class RadioPlayer : MonoBehaviour
    {

        #region Variables

        [Header("Radio Station")]
        /// <summary>Radio station for this RadioPlayer.</summary>
        [Tooltip("Radio station for this RadioPlayer.")]
        public Model.RadioStation Station;

        [Header("Behaviour Settings")]
        /// <summary>Play the radio on start on/off (default: false).</summary>
        [Tooltip("Play the radio on start on/off (default: false).")]
        public bool PlayOnStart = false;

        [Header("General Settings")]
        /// <summary>Size of cache stream in KB (default: 512).</summary>
        [Tooltip("Size of cache stream in KB (default: 512).")]
        public int CacheStreamSize = 512;

        /// <summary>Enable or disable legacy mode. This disables all record informations, but is more stable (default: false).</summary>
        [Tooltip("Enable or disable legacy mode. This disables all record informations, but is more stable (default: false).")]
        public bool LegacyMode = false;

        /// <summary>Capture the encoded PCM-stream from this RadioPlayer (default: false).</summary>
        [Tooltip("Capture the encoded PCM-stream from this RadioPlayer (default: false).")]
        public bool CaptureDataStream = false;
        
        private bool error;
        private string errorMessage;

        private NLayer.MpegFile nLayerReader;
        private NAudio.Wave.Mp3FileReader nAudioReader;
        private NVorbis.VorbisReader nVorbisReader;

        private int oggCacheCleanFrameCount;

        private bool stopped = true;
        private bool bufferAvailable = false;

        private bool playback = false;
        private bool restarted = false;

        private float maxPlayTime;

        private System.Threading.Thread worker;

        private Model.RecordInfo recordInfo = new Model.RecordInfo();
        private Model.RecordInfo nextRecordInfo = new Model.RecordInfo();

        //private Model.RecordInfo lastRecordInfo;
        private Model.RecordInfo lastNextRecordInfo;

        private float nextRecordDelay = 0f;

        //private static int biggestRead = 0;

        private System.IO.Stream ms;
        
        private static bool loggedUnsupportedPlatform = false;
        private static int playCounter = 0;

        #endregion


        #region Properties

        /// <summary>Enable or disable legacy mode. This disables all record informations, but is more stable (main use is for UI).</summary>
        public bool isLegacyMode
        {
            get
            {
                return LegacyMode;
            }

            set
            {
                LegacyMode = value;
            }
        }
        
        /// <summary>Capture the encoded PCM-stream from this RadioPlayer (main use is for UI).</summary>
        public bool isCaptureDataStream
        {
            get
            {
                return CaptureDataStream;
            }

            set
            {
                CaptureDataStream = value;
            }
        }

        /// <summary>AudioSource of for this RadioPlayer.</summary>
        public AudioSource Source { get; private set; }

        /// <summary>Codec of for this RadioPlayer.</summary>
        public Model.Enum.AudioCodec Codec { get; private set; }

        /// <summary>Current playtime of this RadioPlayer.</summary>
        public float PlayTime { get; private set; }

        /// <summary>Current buffer progress.</summary>
        public float BufferProgress { get; private set; }

        /// <summary>Is this RadioPlayer in playback-mode?</summary>
        public bool isPlayback
        {
            get
            {
                return playback;
            }
            //            private set
            //            {
            //                playback = value;
            //            }
        }

        /// <summary>Is this RadioPlayer playing audio?</summary>
        /// <returns>True if this RadioPlayer is playing audio.</returns>
        public bool isAudioPlaying
        {
            get
            {
                //return Source.isPlaying;
                return playback && !isBuffering;
            }
        }

        /// <summary>Is this RadioPlayer buffering?</summary>
        /// <returns>True if this RadioPlayer is buffering.</returns>
        public bool isBuffering
        {
            get
            {
                //return isPlayback && !Source.isPlaying;
                return !bufferAvailable;
            }
        }

        /// <summary>Playtime of the current audio record.</summary>
        public float RecordPlayTime { get; private set; }

        /// <summary>Returns the information about the current audio record.</summary>
        /// <returns>Information about the current audio record.</returns>
        public Model.RecordInfo RecordInfo
        {
            get
            {
                return recordInfo;
            }
        }

        /// <summary>Returns the information about the next audio record. This information is updated a few seconds before a new record starts.</summary>
        /// <returns>Information about the next audio record.</returns>
        public Model.RecordInfo NextRecordInfo
        {
            get
            {
                return nextRecordInfo;
            }
        }

        /// <summary>Returns the current delay in seconds until the next audio record starts.</summary>
        /// <returns>Current delay in seconds until the next audio record starts.</returns>
        public float NextRecordDelay
        {
            get
            {
                return nextRecordDelay;
            }
        }

        /// <summary>Returns the size of the current buffer in KB.</summary>
        /// <returns>Size of the current buffer in KB.</returns>
        public long CurrentBufferSize
        {
            get
            {
                if (ms != null)
                {
                    return ms.Length - ms.Position;
                }

                return 0;
            }
        }

        /// <summary>Returns the current downloads speed in Bytes per second.</summary>
        /// <returns>current downloads speed in Bytes per second.</returns>
        public long CurrentDownloadSpeed
        {
            get
            {
                if (ms != null && PlayTime > 0f)
                {
                    return (long)((float)ms.Length / PlayTime);
                }

                return 0;
            }
        }

        /// <summary>Checks if any RadioPlayer is playing on this system.</summary>
        /// <returns>True if RadioPlayer is playing on this system.</returns>
        public static bool isPlaying
        {
            get
            {
                return playCounter > 0;
            }
        }

        /// <summary>Encoded PCM-stream from this RadioPlayer.</summary>
        public Util.MemoryCacheStream DataStream { get; private set; }

        /// <summary>Current audio channels of the station.</summary>
        public int Channels { get; private set; }

        /// <summary>Current audio sample rate of the station.</summary>
        public int SampleRate { get; private set; }

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

        private ErrorInfo _errorInfo;

        #endregion


        #region MonoBehaviour methods

        public void Awake()
        {
            Channels = 2;
            SampleRate = 44100;

            oggCacheCleanFrameCount = Random.Range(Util.Constants.OGG_CLEAN_INTERVAL_MIN, Util.Constants.OGG_CLEAN_INTERVAL_MAX);

            Source = GetComponent<AudioSource>();
            Source.playOnAwake = false;
            Source.Stop(); //always stop the AudioSource at startup
        }

        public void Start()
        {
            if (PlayOnStart && !Util.Helper.isEditorMode)
            {
                Play();
            }
        }

        public void Update()
        {
            if (isAudioPlaying && !restarted)
            {
                //                if (lastRecordInfo == null)
                //                {
                //                    lastRecordInfo = RecordInfo;
                //
                //                  onRecordChange(Station, RecordInfo);
                //                }
                //                else
                //                {
                //                    if (!lastRecordInfo.Equals(RecordInfo))
                //                    {
                //                        lastRecordInfo = RecordInfo;
                //
                //                      onRecordChange(Station, RecordInfo);
                //                    }
                //                }

                if (lastNextRecordInfo == null || !lastNextRecordInfo.Equals(NextRecordInfo))
                {
                    lastNextRecordInfo = NextRecordInfo;

                    onNextRecordChange(Station, NextRecordInfo, nextRecordDelay);
                }

                float _pitchedTime = Time.deltaTime * Source.pitch;
                Util.Context.TotalPlayTime += _pitchedTime;
                Station.TotalPlayTime += _pitchedTime;
                PlayTime += _pitchedTime;
                RecordPlayTime += _pitchedTime;
                nextRecordDelay -= _pitchedTime;

                onAudioPlayTimeUpdate(Station, PlayTime);
                onRecordPlayTimeUpdate(Station, RecordInfo, RecordPlayTime);
                onNextRecordDelayUpdate(Station, NextRecordInfo, nextRecordDelay);

                if (PlayTime > maxPlayTime)
                {
                    restarted = true;
                    Restart();
                }
            }
        }

        public void OnDisable()
        {
            Stop();
        }

        public void OnValidate()
        {
            if (Station != null)
            {
                if (Station.Bitrate <= 0)
                {
                    Station.Bitrate = Util.Config.DEFAULT_BITRATE;
                }
                else
                {
                    Station.Bitrate = Util.Helper.NearestBitrate(Station.Bitrate, Station.Format);
                }

                if (Station.ChunkSize <= 0)
                {
                    Station.ChunkSize = Util.Config.DEFAULT_CHUNKSIZE;
                }
                else if (Station.ChunkSize > Util.Config.MAX_CACHESTREAMSIZE)
                {
                    Station.ChunkSize = Util.Config.MAX_CACHESTREAMSIZE;
                }

                if (Station.BufferSize <= 0)
                {
                    Station.BufferSize = Util.Config.DEFAULT_BUFFERSIZE;
                }
                else
                {

                    if (Station.Format == Model.Enum.AudioFormat.MP3)
                    {
                        if (Station.BufferSize < Util.Config.DEFAULT_BUFFERSIZE / 4)
                        {
                            Station.BufferSize = Util.Config.DEFAULT_BUFFERSIZE / 4;
                        }
                    }
                    else if (Station.Format == Model.Enum.AudioFormat.OGG)
                    {
                        if (Station.BufferSize < Util.Constants.MIN_OGG_BUFFERSIZE)
                        {
                            Station.BufferSize = Util.Constants.MIN_OGG_BUFFERSIZE;
                        }
                    }

                    if (Station.BufferSize < Station.ChunkSize)
                    {
                        Station.BufferSize = Station.ChunkSize;
                    }
                    else if (Station.BufferSize > Util.Config.MAX_CACHESTREAMSIZE)
                    {
                        Station.BufferSize = Util.Config.MAX_CACHESTREAMSIZE;
                    }
                }
            }

            if (CacheStreamSize <= 0)
            {
                CacheStreamSize = Util.Config.DEFAULT_CACHESTREAMSIZE;
            }
            else if (Station != null && CacheStreamSize <= Station.BufferSize)
            {
                CacheStreamSize = Station.BufferSize;
            }
            else if (CacheStreamSize > Util.Config.MAX_CACHESTREAMSIZE)
            {
                CacheStreamSize = Util.Config.MAX_CACHESTREAMSIZE;
            }
        }


        //        public void OnDrawGizmosSelected()
        //        {
        //            Gizmos.DrawIcon(transform.position, "Radio/radio_player.png");
        //        }

        #endregion


        #region Public methods

        /// <summary>Plays the radio-station.</summary>
        public void Play()
        {
            if (Util.Helper.isSupportedPlatform)
            {
                if (stopped)
                {
                    if (Tool.InternetCheck.isInternetAvailable)
                    {
                        if (Util.Helper.isSane(ref Station))
                        {

                            Codec = Util.Helper.AudioCodecForAudioFormat(Station.Format);

                            if (Codec == Model.Enum.AudioCodec.None)
                            {
                                errorMessage = Station + System.Environment.NewLine + "Audio format not supported - cant play station: " + Station.Format;
                                Debug.LogError(errorMessage);
                                onErrorInfo(Station, errorMessage);

                                return;
                            }

                            if (Station.ExcludedCodec == Codec)
                            {
                                errorMessage = Station + System.Environment.NewLine + "Excluded codec matched - can't play station: " + Codec;
                                Debug.LogError(errorMessage);
                                onErrorInfo(Station, errorMessage);
                            }
                            else
                            {
                                StartCoroutine(playAudioFromUrl());
                            }
                        }
                        else
                        {
                            errorMessage = Station + System.Environment.NewLine + "Could not start playback. Please verify the station settings.";
                            Debug.LogError(errorMessage);
                            onErrorInfo(Station, errorMessage);
                        }
                    }
                    else
                    {
                        errorMessage = "No internet connection available! Can't play (stream) any stations!";
                        Debug.LogError(errorMessage);
                        onErrorInfo(Station, errorMessage);
                    }
                }
                else
                {
                    errorMessage = Station + System.Environment.NewLine + "Station is already playing!";
                    Debug.LogWarning(errorMessage);
                    onErrorInfo(Station, errorMessage);
                }
            }
            else
            {
                logUnsupportedPlatform();
            }
        }

        /// <summary>Stops the playback of the radio-station.</summary>
        public void Stop()
        {
            playback = false;

            if (Source != null) // could already be destroyed
            {
                Source.Stop();
                Source.clip = null;
            }

            stopped = true;

            //PlayTime = 0f;
            //RecordPlayTime = 0f;
            //nextRecordDelay = 0f;
            //recordInfo = new Model.RecordInfo ();
            //nextRecordInfo = new Model.RecordInfo ();
        }

        /// <summary>Silences the AudioSource on the RadioPlayer-component.</summary>
        public void Silence()
        {
            Source.volume = 0f;
        }

        /// <summary>Restarts the playback of the radio-station.</summary>
        public void Restart()
        {
            Stop();

            //this.CTInvoke(() => Play(), Util.Constants.INVOKE_DELAY);
            Invoke("Play", Util.Constants.INVOKE_DELAY);
        }

        public string ToShortString()
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();

            result.Append("Station='");
            result.Append(Station);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("PlayOnStart='");
            result.Append(PlayOnStart);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("CacheStreamSize='");
            result.Append(CacheStreamSize);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER_END);

            return result.ToString();
        }

        /// <summary>Loads the RadioPlayer.</summary>
        public void Load()
        {
            //TODO implement!
            Debug.LogWarning("Not implemented!");
        }

        /// <summary>Saves the RadioPlayer.</summary>
        public void Save()
        {
            //TODO implement!
            Debug.LogWarning("Not implemented!");
        }

        #endregion


        #region Private methods

        private IEnumerator playAudioFromUrl()
        {
            Model.RadioStation _station = Station;

            onPlaybackStart(_station);

            playback = false;
            restarted = false;
            error = false;
            errorMessage = string.Empty;

            PlayTime = 0f;

            //Debug.LogWarning("RecordPlayTime set!");
            RecordPlayTime = 0f;
            BufferProgress = 0f;
            bufferAvailable = false;
            float _bufferCurrentProgress = 0f;

            recordInfo = new Model.RecordInfo();
            nextRecordInfo = new Model.RecordInfo();
            nextRecordDelay = 0f;

            onBufferingStart(_station);

            onBufferingProgressUpdate(_station, BufferProgress);

            using (ms = new Util.MemoryCacheStream(CacheStreamSize * Util.Constants.FACTOR_KB, Util.Config.MAX_CACHESTREAMSIZE * Util.Constants.FACTOR_KB))
            {
                if (LegacyMode)
                {
                    worker = new System.Threading.Thread(() => readStreamLegacy(ref _station, ref playback, ref ms, ref error, ref errorMessage));
                }
                else
                {
                    worker = new System.Threading.Thread(() => readStream(ref _station, ref playback, ref ms, ref error, ref errorMessage, ref nextRecordInfo, ref nextRecordDelay));
                }
                worker.Start();

                // Waiting for stream
                do
                {
                    yield return null;
                } while (!playback && !stopped && !error);

                int bufferSize = _station.BufferSize * Util.Constants.FACTOR_KB + _station.ChunkSize * Util.Constants.FACTOR_KB;

                // Pre-buffering some data to allow start playing
                do
                {
                    BufferProgress = (float)ms.Length / bufferSize;

                    if (BufferProgress != _bufferCurrentProgress)
                    {
                        onBufferingProgressUpdate(_station, BufferProgress);
                        _bufferCurrentProgress = BufferProgress;
                    }

                    yield return null;

                } while (playback && !stopped && ms.Length < bufferSize);

                BufferProgress = 1f;
                onBufferingProgressUpdate(_station, BufferProgress);
                bufferAvailable = true;

                onBufferingEnd(_station);

                if (playback && !stopped)
                {
                    bool _success = false;

                    try
                    {
                        if (Codec == Model.Enum.AudioCodec.MP3_NLayer)
                        {
                            nLayerReader = new NLayer.MpegFile(ms);

                            SampleRate = nLayerReader.SampleRate;
                            Channels = nLayerReader.Channels;
                        }
                        else if (Codec == Model.Enum.AudioCodec.MP3_NAudio)
                        {
                            nAudioReader = new NAudio.Wave.Mp3FileReader(ms);

                            SampleRate = nAudioReader.WaveFormat.SampleRate;
                            Channels = nAudioReader.WaveFormat.Channels;
                        }
                        else if (Codec == Model.Enum.AudioCodec.OGG_NVorbis)
                        {
                            nVorbisReader = new NVorbis.VorbisReader(ms, false);

                            SampleRate = nVorbisReader.SampleRate;
                            Channels = nVorbisReader.Channels;
                        }

                        _success = true;
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError(_station + System.Environment.NewLine + "Could not read data from url!" + System.Environment.NewLine + ex);
                    }

                    if (!_success)
                    {
                        error = true;
                        errorMessage = _station + System.Environment.NewLine + "Could not play the stream -> Please try another station!";
                        Debug.LogError(errorMessage);

                        playback = false;
                    }
                    else
                    {
                        if (Codec == Model.Enum.AudioCodec.OGG_NVorbis)
                        {
                            ms.Position = 0;
                        }

                        maxPlayTime = int.MaxValue / SampleRate - 240; //reserve of 4 minutes

                        DataStream = new Util.MemoryCacheStream(128 * Util.Constants.FACTOR_KB, 512 * Util.Constants.FACTOR_KB);

                        AudioClip myClip = AudioClip.Create(_station.Name, int.MaxValue, Channels, SampleRate, true, readPCMData);
                        Source.clip = myClip;

                        Source.Play();

                        onAudioStart(_station);
                    }

                    do
                    {
                        yield return null;

                        if (Codec == Model.Enum.AudioCodec.OGG_NVorbis && Time.frameCount % oggCacheCleanFrameCount == 0)
                        {
                            if (Util.Constants.DEV_DEBUG)
                                Debug.Log("Clean cache: " + oggCacheCleanFrameCount + " - " + PlayTime);

                            NVorbis.Mdct.ClearSetupCache();
                        }
                    } while (playback && !stopped);

                    if (_success && !error)
                    {
                        onAudioEnd(_station);
                    }

                    Source.Stop();
                    Source.clip = null;

                    if (Codec == Model.Enum.AudioCodec.MP3_NLayer)
                    {
                        if (nLayerReader != null)
                        {
                            nLayerReader.Dispose();
                            nLayerReader = null;
                        }
                    }
                    else if (Codec == Model.Enum.AudioCodec.MP3_NAudio)
                    {
                        if (nAudioReader != null)
                        {
                            nAudioReader.Dispose();
                            nAudioReader = null;
                        }
                    }
                    else if (Codec == Model.Enum.AudioCodec.OGG_NVorbis)
                    {
                        if (nVorbisReader != null)
                        {
                            nVorbisReader.Dispose();
                            nVorbisReader = null;

                            NVorbis.Mdct.ClearSetupCache();
                        }
                    }
                }
            }

            if (DataStream != null)
            {
                DataStream.Dispose();
            }

            if (error)
            {
                onErrorInfo(_station, errorMessage);
            }

            onPlaybackEnd(_station);
        }

        private void readPCMData(float[] data)
        {
            if (playback && !stopped && bufferAvailable)
            {
                if (Codec == Model.Enum.AudioCodec.MP3_NLayer)
                {
                    if (nLayerReader != null)
                    {
                        try
                        {
                            if (nLayerReader.ReadSamples(data, 0, data.Length) > 0)
                            {
                                //do nothing
                            }
                            else
                            {
                                logNoMoreData();
                            }
                        }
                        catch (System.Exception ex)
                        {
                            logDataError(ex);
                        }
                    }
                }
                else if (Codec == Model.Enum.AudioCodec.MP3_NAudio)
                {
                    if (nAudioReader != null)
                    {
                        byte[] buffer = new byte[data.Length * 2];
                        int count;

                        try
                        {
                            if ((count = nAudioReader.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                System.Buffer.BlockCopy(Util.Helper.ConvertByteArrayToFloatArray(buffer, count), 0, data, 0, data.Length * 4);
                            }
                            else
                            {
                                logNoMoreData();
                            }
                        }
                        catch (System.Exception ex)
                        {
                            logDataError(ex);
                        }
                    }
                }
                else if (Codec == Model.Enum.AudioCodec.OGG_NVorbis)
                {
					if (nVorbisReader != null) {
						try {
							if (nVorbisReader.ReadSamples (data, 0, data.Length) > 0) {
								//do nothing
							} else {
								logNoMoreData ();
							}
						} catch (System.Exception ex) {
							logDataError (ex);
						}
					}
                }
            }
            else
            {
                System.Buffer.BlockCopy(new float[data.Length], 0, data, 0, data.Length * 4);
            }

            if (CaptureDataStream && DataStream != null)
            {
                byte[] bytes = Util.Helper.ConvertFloatArrayToByteArray(data, data.Length);
                DataStream.Write(bytes, 0, bytes.Length);
            }
        }

        private void readStream(ref Model.RadioStation _station, ref bool _playback, ref System.IO.Stream _ms, ref bool _error, ref string _errorMessage, ref Model.RecordInfo _nextRecordInfo, ref float _nextRecordDelay)
        {
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = Util.Helper.RemoteCertificateValidationCallback;

                using (Util.CTWebClient client = new Util.CTWebClient(int.MaxValue))
                {
                    System.Net.HttpWebRequest _request = (System.Net.HttpWebRequest)client.CTGetWebRequest(_station.Url);
                    //System.Net.HttpWebRequest _request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(_station.Url);

                    // clear old request header and build own header to receive ICY-metadata
                    _request.Headers.Clear();
                    //request.Headers.Add("GET", "/ HTTP/1.0");
                    _request.Headers.Add("GET", "/ HTTP/1.1");
                    _request.Headers.Add("Icy-MetaData", "1"); // needed to receive metadata informations
                    _request.UserAgent = "WinampMPEG/5.09";
                    //request.KeepAlive = true;

                    using (System.Net.HttpWebResponse _response = (System.Net.HttpWebResponse)_request.GetResponse())
                    {
                        // read blocksize to find metadata header
                        int _metaint = string.IsNullOrEmpty(_response.GetResponseHeader("icy-metaint")) ? int.MaxValue : System.Convert.ToInt32(_response.GetResponseHeader("icy-metaint"));

                        if (Util.Constants.DEV_DEBUG)
                            Debug.LogWarning("metaint: " + _metaint);

                        using (System.IO.Stream _stream = _response.GetResponseStream())
                        {
                            if (_stream != null)
                            {
                                int _metadataLength = 0; // length of metadata header

                                byte[] _buffer = new byte[_station.ChunkSize * Util.Constants.FACTOR_KB];
                                int _read;
                                _playback = true;
                                Util.Context.TotalDataRequests++;
                                _station.TotalDataRequests++;

                                int _offset = 0;
                                int _status = 0;
                                bool _isFirsttime = true;

                                _nextRecordDelay = 0f;

                                do
                                {
                                    if ((_read = _stream.Read(_buffer, 0, _buffer.Length)) > 0)
                                    {
                                        Util.Context.TotalDataSize += _read;
                                        _station.TotalDataSize += _read;

                                        _offset = 0;

                                        if (_metaint > 0 && _read + _status > _metaint)
                                        {

                                            for (int ii = 0; ii < _read && _playback;)
                                            {
                                                if (_status == _metaint)
                                                {
                                                    _status = 0;

                                                    _ms.Write(_buffer, _offset, ii - _offset);
                                                    _offset = ii;

                                                    _metadataLength = System.Convert.ToInt32(_buffer[ii]) * 16;
                                                    ii++;
                                                    _offset++;

                                                    if (_metadataLength > 0)
                                                    {
                                                        if (_metadataLength + _offset <= _read)
                                                        {
                                                            byte[] metaDataBuffer = new byte[_metadataLength];

                                                            System.Array.Copy(_buffer, ii, metaDataBuffer, 0, _metadataLength);

                                                            //bufferRead = 0;

                                                            _nextRecordInfo = new Model.RecordInfo(System.Text.Encoding.UTF8.GetString(metaDataBuffer));
                                                            //_nextRecordInfo = new Model.RecordInfo(System.Text.Encoding.Default.GetString(metaDataBuffer));

                                                            if (!_isFirsttime)
                                                            {
                                                                _nextRecordDelay = (float)(_ms.Length - _ms.Position) / (Station.Bitrate * 125);
                                                            }
                                                            else
                                                            {
                                                                _isFirsttime = false;
                                                            }

                                                            ii += _metadataLength;
                                                            _offset += _metadataLength;

                                                            if (Util.Constants.DEV_DEBUG)
                                                                Debug.LogWarning("RecordInfo read: " + _nextRecordInfo);

                                                        }
                                                        else
                                                        {
                                                            if (Util.Constants.DEV_DEBUG)
                                                                Debug.LogError("Info-frame outside of the buffer!");

                                                            ii = _read;
                                                            _status = _read - (_metadataLength + _offset);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    _status++;
                                                    ii++;
                                                }


                                            }

                                            if (_offset < _read)
                                            {
                                                _ms.Write(_buffer, _offset, _read - _offset);
                                            }
                                        }
                                        else
                                        {
                                            _status += _read;
                                            _ms.Write(_buffer, 0, _read);
                                        }

                                        //                                    // handles delay of the RecordInfo
                                        //                                  if ((isFirsttime || bufferRead > _currentBufferSize) && _nextRecordInfo != null && !_nextRecordInfo.Equals(_recordInfo))
                                        //                                    {
                                        //                                        if (Util.Config.DEBUG)
                                        //                                            Debug.LogWarning("RecordInfo set: " + System.DateTime.Now + System.Environment.NewLine + _nextRecordInfo);
                                        //
                                        //                                        isFirsttime = false;
                                        //
                                        //                                        bufferRead = 0;
                                        //
                                        //                                        _recordInfo.Duration = RecordPlayTime; //update the duration of the last entry!
                                        //                                        _recordInfo = _nextRecordInfo;
                                        //
                                        //                                        if (!string.IsNullOrEmpty(_recordInfo.Info))
                                        //                                        {
                                        //                                            //Debug.LogWarning("Record added: '" + recordInfo.Info + "'");
                                        //                                            Station.PlayedRecords.Add(_recordInfo);
                                        //                                        }
                                        //                                    }
                                    }
                                } while (_playback);
                            }
                        }

                    }
                }
            }
            catch (System.Exception ex)
            {
                _error = true;
                _errorMessage = _station.Name + System.Environment.NewLine + "Could not read url after " + Util.Helper.FormatSecondsToHourMinSec(PlayTime) + "!" + System.Environment.NewLine + ex;
                Debug.LogError(_errorMessage);

                _playback = false;
            }
        }

        private void readStreamLegacy(ref Model.RadioStation _station, ref bool _playback, ref System.IO.Stream _ms, ref bool _error, ref string _errorMessage)
        {
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = Util.Helper.RemoteCertificateValidationCallback;

                using (Util.CTWebClient client = new Util.CTWebClient(int.MaxValue))
                {
                    using (System.Net.WebResponse _response = client.CTGetWebRequest(_station.Url).GetResponse())
                    {
                        using (System.IO.Stream _stream = _response.GetResponseStream())
                        {
                            if (_stream != null)
                            {
                                byte[] _buffer = new byte[_station.ChunkSize * Util.Constants.FACTOR_KB];
                                int _read;
                                _playback = true;
                                Util.Context.TotalDataRequests++;
                                _station.TotalDataRequests++;

                                do
                                {
                                    if ((_read = _stream.Read(_buffer, 0, _buffer.Length)) > 0)
                                    {
                                        Util.Context.TotalDataSize += _read;
                                        _station.TotalDataSize += _read;

                                        if (_playback)
                                        {
                                            _ms.Write(_buffer, 0, _read);
                                        }
                                    }
                                    else
                                    {
                                        //Debug.LogWarning(Station + Environment.NewLine + "Could no longer read data from url!" + Environment.NewLine + "Now using buffer -> Please restart this station after the buffer is empty!");
                                        break;
                                    }
                                } while (_playback);
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                _error = true;
                _errorMessage = _station.Name + System.Environment.NewLine + "Could not read url after " + Util.Helper.FormatSecondsToHourMinSec(PlayTime) + "!" + System.Environment.NewLine + ex;
                Debug.LogError(_errorMessage);

                _playback = false;
            }
        }

        private void logNoMoreData()
        {
            error = true;
            errorMessage = Station.Name + System.Environment.NewLine + "No more data to read after " + Util.Helper.FormatSecondsToHourMinSec(PlayTime) + "! Please restart this station or choose another one.";
            Debug.LogError(errorMessage);

            playback = false;
        }

        private void logDataError(System.Exception ex)
        {
            error = true;
            errorMessage = Station.Name + System.Environment.NewLine + "Could not read audio after " + Util.Helper.FormatSecondsToHourMinSec(PlayTime) + "! This is typically a sign of a buffer underun -> Please try to increment the 'ChunkSize' and 'BufferSize':" + System.Environment.NewLine + ex;
            Debug.LogError(errorMessage);

            playback = false;
        }

        private void logUnsupportedPlatform()
        {
            if (!loggedUnsupportedPlatform)
            {
                errorMessage = "'Radio' is not supported on your platform!";
                Debug.LogWarning(errorMessage);
                onErrorInfo(Station, errorMessage);
            }
        }

        #endregion


        #region Event-trigger methods

        private void onPlaybackStart(Model.RadioStation station)
        {
            stopped = false;
            playCounter++;

            if (Util.Config.DEBUG)
                Debug.Log("onPlaybackStart: " + station);

            if (_playbackStart != null)
            {
                _playbackStart(station);
            }
        }

        private void onPlaybackEnd(Model.RadioStation station)
        {
            stopped = true;
            playCounter--;

            if (Util.Config.DEBUG)
                Debug.Log("onPlaybackEnd: " + station);

            if (recordInfo != null)
            {
                //Debug.Log(station.Name + "-onPlaybackEnd - recordInfo.Duration: " + RecordPlayTime + " - " + recordInfo);
                recordInfo.Duration = RecordPlayTime; //update the duration of the last entry!

                recordInfo = new Model.RecordInfo();
            }

            if (_playbackEnd != null)
            {
                _playbackEnd(station);
            }
        }

        private void onBufferingStart(Model.RadioStation station)
        {
            if (Util.Config.DEBUG)
                Debug.Log("onBufferingStart: " + station);

            if (_bufferingStart != null)
            {
                _bufferingStart(station);
            }
        }

        private void onBufferingEnd(Model.RadioStation station)
        {
            if (Util.Config.DEBUG)
                Debug.Log("onBufferingEnd: " + station);

            if (_bufferingEnd != null)
            {
                _bufferingEnd(station);
            }
        }

        private void onBufferingProgressUpdate(Model.RadioStation station, float progress)
        {
            //            if (Util.Config.DEBUG)
            //              Debug.Log("onBufferingProgressUpdate: " + station + " - " + progress);

            if (_bufferingProgressUpdate != null)
            {
                _bufferingProgressUpdate(station, progress);
            }
        }

        private void onAudioStart(Model.RadioStation station)
        {
            if (Util.Config.DEBUG)
                Debug.Log("onAudioStart: " + station);

            if (_audioStart != null)
            {
                _audioStart(station);
            }
        }

        private void onAudioEnd(Model.RadioStation station)
        {
            if (Util.Config.DEBUG)
                Debug.Log("onAudioEnd: " + station);

            if (_audioEnd != null)
            {
                _audioEnd(station);
            }
        }

        private void onAudioPlayTimeUpdate(Model.RadioStation station, float playtime)
        {
            //          if (Util.Config.DEBUG)
            //              Debug.Log("onAudioPlayTimeUpdate: " + station + " - " + playtime);

            if (_audioPlayTimeUpdate != null)
            {
                _audioPlayTimeUpdate(station, playtime);
            }
        }

        private void onErrorInfo(Model.RadioStation station, string info)
        {
            if (Util.Config.DEBUG)
                Debug.Log("onErrorInfo: " + station + " - " + info);

            if (_errorInfo != null)
            {
                _errorInfo(station, info);
            }
        }

        private void onRecordChange(Model.RadioStation station, Model.RecordInfo newRecord)
        {
            if (!newRecord.Equals(recordInfo))
            {
                if (Util.Config.DEBUG)
                    Debug.Log("onRecordChange: " + station + " - " + newRecord);

                if (recordInfo != null)
                {
                    //Debug.Log(station.Name + "-onRecordChange - recordInfo.Duration: " + RecordPlayTime + " - " + recordInfo);
                    recordInfo.Duration = RecordPlayTime; //update the duration of the last entry!
                }

                recordInfo = newRecord;

                //Debug.LogWarning("RecordPlayTime set!");
                RecordPlayTime = 0f;
                //nextRecordDelay = 0f;

                if (!string.IsNullOrEmpty(recordInfo.Info))
                {
                    //Debug.LogWarning("Record added: '" + recordInfo.Info + "'");
                    Station.PlayedRecords.Add(recordInfo);
                }

                if (_recordChange != null)
                {
                    _recordChange(station, recordInfo);
                }
            }
        }

        private void onRecordPlayTimeUpdate(Model.RadioStation station, Model.RecordInfo record, float playtime)
        {
            //          if (Util.Config.DEBUG)
            //              Debug.Log("onRecordPlayTimeUpdate: " + station + " - " + record + " - " + playtime);

            if (_recordPlayTimeUpdate != null)
            {
                _recordPlayTimeUpdate(station, record, playtime);
            }
        }

        private void onNextRecordChange(Model.RadioStation station, Model.RecordInfo nextRecord, float delay)
        {
            if (Util.Config.DEBUG)
                Debug.Log("onNextRecordChange: " + station + " - " + nextRecord);

            if (_nextRecordChange != null)
            {
                _nextRecordChange(station, nextRecord, delay);
            }
        }

        private void onNextRecordDelayUpdate(Model.RadioStation station, Model.RecordInfo nextRecord, float delay)
        {
            if (delay > 0f)
            {
                //              if (Util.Config.DEBUG)
                //                  Debug.Log ("onNextRecordDelayUpdate: " + station + " - " + nextRecord + " - " + delay);

                if (_nextRecordDelayUpdate != null)
                {
                    _nextRecordDelayUpdate(station, nextRecord, delay);
                }
            }
            else
            {
                onRecordChange(station, nextRecord);
            }
        }

        #endregion


        #region Overridden methods

        public override string ToString()
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();

            result.Append(GetType().Name);
            result.Append(Util.Constants.TEXT_TOSTRING_START);

            result.Append("Station='");
            result.Append(Station);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("PlayOnStart='");
            result.Append(PlayOnStart);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

            result.Append("CacheStreamSize='");
            result.Append(CacheStreamSize);
            result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER_END);

            result.Append(Util.Constants.TEXT_TOSTRING_END);

            return result.ToString();
        }

        #endregion


        #region Editor-only methods

#if UNITY_EDITOR

        /// <summary>Plays the radio-station (Editor only).</summary>
        /// <param name="channels">Number of audio channels (default: 2, optional)</param>
        /// <param name="sampleRate">Sample rate of the audio (default: 44100, optional)</param>
        public void PlayInEditor(int channels = 2, int sampleRate = 44100)
        {
            Channels = channels;
            SampleRate = sampleRate;

            if (Util.Helper.isEditorMode)
            {
                if (stopped)
                {
                    if (Tool.InternetCheck.isInternetAvailable)
                    {
                        if (Util.Helper.isSane(ref Station))
                        {
                            if (Channels > 0)
                            {
                                if (SampleRate >= 8000)
                                {
                                    Codec = Util.Helper.AudioCodecForAudioFormat(Station.Format);

                                    if (Codec == Model.Enum.AudioCodec.None)
                                    {
                                        errorMessage = Station + System.Environment.NewLine + "Audio format not supported - cant play station: " + Station.Format;
                                        Debug.LogError(errorMessage);

                                        return;
                                    }

                                    if (Station.ExcludedCodec == Codec)
                                    {
                                        errorMessage = Station + System.Environment.NewLine + "Excluded codec matched - can't play station: " + Codec;
                                        Debug.LogError(errorMessage);
                                    }
                                    else
                                    {
                                        maxPlayTime = int.MaxValue / (2 * SampleRate * Channels) - 240; //reserve of 4 minutes
                                        Source.clip = AudioClip.Create(Station.Name, int.MaxValue, Channels, SampleRate, true, readPCMData);

                                        System.Threading.Thread worker = new System.Threading.Thread(() => playAudioFromUrlInEditor());
                                        worker.Start();

                                        Source.Play();
                                    }
                                }
                                else
                                {
                                    errorMessage = Station + System.Environment.NewLine + "The 'sampleRate' must be greater than 8000!";
                                    Debug.LogError(errorMessage);
                                }
                            }
                            else
                            {
                                errorMessage = Station + System.Environment.NewLine + "The number of 'channels' must be greater than 0!";
                                Debug.LogError(errorMessage);
                            }
                        }
                        else
                        {
                            errorMessage = Station + System.Environment.NewLine + "Could not start playback. Please verify the station settings.";
                            Debug.LogError(errorMessage);
                        }
                    }
                    else
                    {
                        errorMessage = "No internet connection available! Can't play (stream) any stations!";
                        Debug.LogError(errorMessage);
                    }
                }
                else
                {
                    errorMessage = Station + System.Environment.NewLine + "Station is already playing!";
                    Debug.LogWarning(errorMessage);
                }
            }
            else
            {
                Debug.LogWarning("'PlayInEditor()' works only inside the Unity Editor!");
            }
        }

        private void playAudioFromUrlInEditor()
        {
            playback = false;
            stopped = false;
            restarted = false;
            error = false;
            errorMessage = System.String.Empty;

            PlayTime = 0f;
            RecordPlayTime = 0f;
            BufferProgress = 0f;
            bufferAvailable = false;
            float _bufferCurrentProgress = 0f;

            recordInfo = new Model.RecordInfo();
            nextRecordInfo = new Model.RecordInfo();
            nextRecordDelay = 0f;

            using (ms = new Util.MemoryCacheStream(CacheStreamSize * Util.Constants.FACTOR_KB, Util.Config.MAX_CACHESTREAMSIZE * Util.Constants.FACTOR_KB))
            {
                if (LegacyMode)
                {
                    worker = new System.Threading.Thread(() => readStreamLegacy(ref Station, ref playback, ref ms, ref error, ref errorMessage));
                }
                else
                {
                    worker = new System.Threading.Thread(() => readStream(ref Station, ref playback, ref ms, ref error, ref errorMessage, ref nextRecordInfo, ref nextRecordDelay));
                }

                worker.Start();

                // Waiting for stream
                do
                {
                    System.Threading.Thread.Sleep(20);
                    //System.Threading.Thread.CurrentThread.Join(30);
                } while (!playback && !stopped && !error);

                int _bufferSize = Station.BufferSize * Util.Constants.FACTOR_KB + Station.ChunkSize * Util.Constants.FACTOR_KB;

                // Pre-buffering some data to allow start playing
                do
                {

                    BufferProgress = (float)ms.Length / _bufferSize;

                    if (BufferProgress != _bufferCurrentProgress)
                    {
                        _bufferCurrentProgress = BufferProgress;
                    }

                    //System.Threading.Thread.CurrentThread.Join(50);
                    System.Threading.Thread.Sleep(50);
                } while (playback && !stopped && ms.Length < _bufferSize);

                BufferProgress = 1f;
                bufferAvailable = true;

                if (playback && !stopped)
                {
                    try
                    {
                        if (Codec == Model.Enum.AudioCodec.MP3_NLayer)
                        {
                            nLayerReader = new NLayer.MpegFile(ms);
                        }
                        else if (Codec == Model.Enum.AudioCodec.MP3_NAudio)
                        {
                            nAudioReader = new NAudio.Wave.Mp3FileReader(ms);
                        }
                        else if (Codec == Model.Enum.AudioCodec.OGG_NVorbis)
                        {
                            nVorbisReader = new NVorbis.VorbisReader(ms, false);
                        }

                        if (Codec == Model.Enum.AudioCodec.OGG_NVorbis)
                        {
                            ms.Position = 0;
                        }

                        int _iterations = 0;
                        do
                        {
                            if (Codec == Model.Enum.AudioCodec.OGG_NVorbis && _iterations % oggCacheCleanFrameCount == 0)
                            {
                                if (Util.Constants.DEV_DEBUG)
                                    Debug.Log("Clean cache: " + oggCacheCleanFrameCount + " - " + PlayTime);

                                NVorbis.Mdct.ClearSetupCache();
                            }

                            _iterations++;

                            //System.Threading.Thread.CurrentThread.Join(50);
                            System.Threading.Thread.Sleep(50);
                        } while (playback && !stopped);

                    }
                    catch (System.Exception)
                    {
                        //Debug.LogError (Station + System.Environment.NewLine + "Could not read data from url!" + System.Environment.NewLine + ex);

                        error = true;
                        errorMessage = Station + System.Environment.NewLine + "Could not play the stream -> Please try another station!";
                        Debug.LogError(errorMessage);

                        playback = false;
                    }
                    finally
                    {
                        if (Codec == Model.Enum.AudioCodec.MP3_NLayer)
                        {
                            if (nLayerReader != null)
                            {
                                nLayerReader.Dispose();
                                nLayerReader = null;
                            }
                        }
                        else if (Codec == Model.Enum.AudioCodec.MP3_NAudio)
                        {
                            if (nAudioReader != null)
                            {
                                nAudioReader.Dispose();
                                nAudioReader = null;
                            }
                        }
                        else if (Codec == Model.Enum.AudioCodec.OGG_NVorbis)
                        {
                            if (nVorbisReader != null)
                            {
                                nVorbisReader.Dispose();
                                nVorbisReader = null;

                                NVorbis.Mdct.ClearSetupCache();
                            }
                        }
                    }
                }
            }
        }

#endif

        #endregion
    }
}
// © 2015-2017 crosstales LLC (https://www.crosstales.com)