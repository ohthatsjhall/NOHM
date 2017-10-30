using UnityEngine;
using System.Linq;

namespace Crosstales.Radio.Provider
{
    /// <summary>Provider for URLs of radio stations in various formats.</summary>
    //[ExecuteInEditMode]
    [HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_provider_1_1_radio_provider_u_r_l.html")]
    public class RadioProviderURL : BaseRadioProvider
    {

        #region Variables

        [Header("Source Settings")]
        /// <summary>All source radio station entries.</summary>
        [Tooltip("All source radio station entries.")]
        public System.Collections.Generic.List<Model.Entry.RadioEntryURL> Entries = new System.Collections.Generic.List<Model.Entry.RadioEntryURL>();

        #endregion


        #region Properties

        public override System.Collections.Generic.List<Model.Entry.BaseRadioEntry> RadioEntries
        {
            get
            {
                return Entries.Cast<Model.Entry.BaseRadioEntry>().ToList();
            }
        }

        #endregion


        #region Private methods

        protected override void init()
        {
            base.init();

            Model.RadioStation station;

            foreach (Model.Entry.RadioEntryURL entry in Entries)
            {
                if (entry != null && entry.EnableSource)
                {
                    if (!string.IsNullOrEmpty(entry.FinalURL))
                    {
                        if (entry.DataFormat == Model.Enum.DataFormatURL.Stream)
                        {
                            station = new Model.RadioStation(entry.Name, entry.FinalURL, entry.Format, entry.Station, entry.Genres.ToLower(), entry.Bitrate, entry.Rating, entry.Description, entry.Icon, entry.ChunkSize, entry.BufferSize, entry.ExcludedCodec);

                            if (!Stations.Contains(station))
                            {
                                Stations.Add(station);
                            }
                            else
                            {
                                Debug.LogWarning("Station already added: '" + entry + "'");
                            }
                        }
                        else
                        {
                            StartCoroutine(loadWeb(addCoRoutine(), entry));
                        }
                    }
                    else
                    {
                        Debug.LogWarning(entry + ": 'URL' is null or empty!" + System.Environment.NewLine + "Please add a valid URL.");
                    }
                }
            }
        }

        #endregion


        #region Editor-only methods

#if UNITY_EDITOR

        protected override void initInEditor()
        {
            if (Util.Helper.isEditorMode)
            {
                base.initInEditor();

                Model.RadioStation station;

                foreach (Model.Entry.RadioEntryURL entry in Entries)
                {
                    if (entry != null && entry.EnableSource)
                    {

                        if (!string.IsNullOrEmpty(entry.FinalURL))
                        {
                            if (entry.DataFormat == Model.Enum.DataFormatURL.Stream)
                            {
                                station = new Model.RadioStation(entry.Name, entry.FinalURL, entry.Format, entry.Station, entry.Genres.ToLower(), entry.Bitrate, entry.Rating, entry.Description, entry.Icon, entry.ChunkSize, entry.BufferSize, entry.ExcludedCodec);

                                if (!Stations.Contains(station))
                                {
                                    Stations.Add(station);
                                }
                                else
                                {
                                    Debug.LogWarning("Station already added: '" + entry + "'");
                                }
                            }
                            else
                            {
                                loadWebInEditor(entry);
                            }
                        }
                        else
                        {
                            Debug.LogWarning(entry + ": 'URL' is null or empty!" + System.Environment.NewLine + "Please add a valid URL.");
                        }
                    }
                }
            }
        }

#endif

        #endregion

    }
}
// © 2016-2017 crosstales LLC (https://www.crosstales.com)