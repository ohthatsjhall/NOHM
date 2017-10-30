using UnityEngine;
using Crosstales.Radio.Util;

namespace Crosstales.Radio.Demo.Util
{
    /// <summary>Random color changer.</summary>
    [HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_demo_1_1_util_1_1_random_color.html")]
    [RequireComponent(typeof(Renderer))]
    public class RandomColor : MonoBehaviour
    {
        #region Variables

        public Vector2 ChangeInterval = new Vector2(5, 15);
        public float Alpha = 0.25f;
        public bool ChangeMaterial = false;
        public Material Material;

        private float elapsedTime = 0f;
        private float changeTime = 0f;
        private Renderer currentRenderer;

        private Color32 startColor;
        private Color32 endColor;

        private float lerpProgress = 0f;

        #endregion


        #region MonoBehaviour methods

        public void Start()
        {
            elapsedTime = changeTime = Random.Range(ChangeInterval.x, ChangeInterval.y);

            if (ChangeMaterial)
            {
                startColor = Material.GetColor("_Color");
            }
            else
            {
                currentRenderer = GetComponent<Renderer>();
                startColor = currentRenderer.material.color;
            }
        }

        public void Update()
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime > changeTime)
            {
                endColor = Helper.HSVToRGB(Random.Range(0.0f, 360f), 1, 1, Alpha);

                changeTime = Random.Range(ChangeInterval.x, ChangeInterval.y);

                lerpProgress = elapsedTime = 0f;
            }

            if (ChangeMaterial)
            {
                Material.SetColor("_Color", Color.Lerp(startColor, endColor, lerpProgress));
            }
            else
            {
                currentRenderer.material.color = Color.Lerp(startColor, endColor, lerpProgress);
            }

            if (lerpProgress < 1f)
            {
                lerpProgress += Time.deltaTime / (changeTime - 0.1f);
                //lerpProgress += Time.deltaTime / changeTime;
            }
            else
            {
                if (ChangeMaterial)
                {
                    startColor = Material.GetColor("_Color");
                }
                else
                {
                    startColor = currentRenderer.material.color;
                }
            }
        }

        #endregion
    }
}
// © 2015-2017 crosstales LLC (https://www.crosstales.com)