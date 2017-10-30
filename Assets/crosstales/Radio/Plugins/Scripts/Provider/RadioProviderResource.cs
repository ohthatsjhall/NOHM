using UnityEngine;
using System.Linq;

namespace Crosstales.Radio.Provider
{
    /// <summary>Provider for resources of radio stations in various formats.</summary>
    //[ExecuteInEditMode]
    [HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_provider_1_1_radio_provider_resource.html")]
    public class RadioProviderResource : BaseRadioProvider
    {

        #region Variables

        [Header("Source Settings")]
        /// <summary>All source radio station entries.</summary>
        [Tooltip("All source radio station entries.")]
        public System.Collections.Generic.List<Model.Entry.RadioEntryResource> Entries = new System.Collections.Generic.List<Model.Entry.RadioEntryResource>();

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

            foreach (Model.Entry.RadioEntryResource entry in Entries)
            {
                if (entry != null && entry.EnableSource)
                {
                    if (entry.Resource != null)
                    {
                        StartCoroutine(loadResource(addCoRoutine(), entry));
                    }
                    else
                    {
                        Debug.LogWarning(entry + ": resource field 'Resource' is null or empty!" + System.Environment.NewLine + "Please add a valid resource.");
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

                foreach (Model.Entry.RadioEntryResource entry in Entries)
                {
                    if (entry != null && entry.EnableSource)
                    {

                        if (entry.Resource != null)
                        {
                            loadResourceInEditor(entry);
                        }
                        else
                        {
                            Debug.LogWarning(entry + ": resource field 'Resource' is null or empty!" + System.Environment.NewLine + "Please add a valid resource.");
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