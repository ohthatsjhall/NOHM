using UnityEngine;

namespace Crosstales.Radio.Demo.Util
{
    /// <summary>Orbit an object (with random rotation).</summary>
    [HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_demo_1_1_util_1_1_orbit.html")]
    public class Orbit : MonoBehaviour
    {
        #region Variables

        public Transform Target;
        public bool RotateX = false;
        public bool RotateY = true;
        public bool RotateZ = false;
        public Vector3 Speed = Vector3.zero;
        public Vector3 ChangeTimeMin = Vector3.zero;
        public Vector3 ChangeTime = Vector3.zero;

        private float speedX;
        private float speedY;
        private float speedZ;
        private float changeTimeX = 0f;
        private float changeTimeY = 0f;
        private float changeTimeZ = 0f;
        private float elapsedTimeX = 0f;
        private float elapsedTimeY = 0f;
        private float elapsedTimeZ = 0f;

        #endregion


        #region MonoBehaviour methods

        public void Start()
        {
            if (RotateX)
            {
                changeTimeX = Random.Range(ChangeTimeMin.x, Mathf.Abs(ChangeTime.x));

                speedX = Random.Range(-Mathf.Abs(Speed.x), Mathf.Abs(Speed.x));
            }

            if (RotateY)
            {
                changeTimeY = Random.Range(ChangeTimeMin.y, Mathf.Abs(ChangeTime.y));

                speedY = Random.Range(-Mathf.Abs(Speed.y), Mathf.Abs(Speed.y));
            }

            if (RotateZ)
            {
                changeTimeZ = Random.Range(ChangeTimeMin.z, Mathf.Abs(ChangeTime.z));

                speedZ = Random.Range(-Mathf.Abs(Speed.z), Mathf.Abs(Speed.z));
            }
        }

        public void Update()
        {
            if (RotateX)
            {
                elapsedTimeX += Time.deltaTime;

                if (elapsedTimeX > changeTimeX)
                {
                    speedX = Random.Range(-Mathf.Abs(Speed.x), Mathf.Abs(Speed.x) + 1f);

                    elapsedTimeX = 0f;
                    changeTimeX = Random.Range(Mathf.Abs(ChangeTimeMin.x), Mathf.Abs(ChangeTime.x));
                }

                transform.RotateAround(Target.position, Vector3.right, speedX * Time.deltaTime);
            }

            if (RotateY)
            {
                elapsedTimeY += Time.deltaTime;

                if (elapsedTimeY > changeTimeY)
                {
                    speedY = Random.Range(-Mathf.Abs(Speed.y), Mathf.Abs(Speed.y) + 1f);

                    elapsedTimeY = 0f;
                    changeTimeY = Random.Range(Mathf.Abs(ChangeTimeMin.y), Mathf.Abs(ChangeTime.y));
                }

                transform.RotateAround(Target.position, Vector3.up, speedY * Time.deltaTime);
            }

            if (RotateZ)
            {
                elapsedTimeZ += Time.deltaTime;

                if (elapsedTimeZ > changeTimeZ)
                {
                    speedZ = Random.Range(-Mathf.Abs(Speed.z), Mathf.Abs(Speed.z) + 1f);

                    elapsedTimeZ = 0f;
                    changeTimeZ = Random.Range(Mathf.Abs(ChangeTimeMin.z), Mathf.Abs(ChangeTime.z));
                }

                transform.RotateAround(Target.position, Vector3.back, speedZ * Time.deltaTime);
            }
        }

        #endregion
    }
}
// © 2015-2017 crosstales LLC (https://www.crosstales.com)