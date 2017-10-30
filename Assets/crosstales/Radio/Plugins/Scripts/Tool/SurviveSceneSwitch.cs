using UnityEngine;

namespace Crosstales.Radio.Tool
{
    /// <summary>Allows any Unity gameobject to survive a scene switch. This is especially useful to keep the music playing while loading a new scene.</summary>
    [DisallowMultipleComponent]
    [HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_tool_1_1_survive_scene_switch.html")]
    public class SurviveSceneSwitch : MonoBehaviour
    {
        #region Variables

		public GameObject[] Survivors; //any object, like a RadioPlayer

        private Transform tf;

        private const float ensureParentTime = 1.5f;
        private float ensureParentTimer = 0f;

        #endregion


        #region MonoBehaviour methods

        public void Awake()
        {
            tf = transform;
            DontDestroyOnLoad(tf.root.gameObject);
        }

        public void Start()
        {
            ensureParentTimer = ensureParentTime;
        }

        public void Update()
        {
            ensureParentTimer += Time.deltaTime;

			if (Survivors != null && ensureParentTimer > ensureParentTime)
            {
                ensureParentTimer = 0f;

                foreach (GameObject go in Survivors)
                {
					if (go != null)
                    	go.transform.SetParent(tf);
                }
            }

            if (Util.Helper.isEditorMode)
            {
                name = Util.Constants.SURVIVOR_SCENE_OBJECT_NAME; //ensure name
            }
        }

        #endregion
    }
}
// © 2016-2017 crosstales LLC (https://www.crosstales.com)