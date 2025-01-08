using NewHorizons.Utility.OWML;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;

namespace KerbalWilds
{
    public class Vallhenge : MonoBehaviour
    {
        public static float RiseTime = 2f;
        public static float RotateTime = 4f;
        public static float OrbitSpeed = 180f;
        public static float OrbitSpeedFalloff = 5f;
        public static Color EmissionColor = new Color(0.5f, 0f, 1f);
        
        public MeshRenderer[] EmissionRenderers;
        public Light[] Lights;
        public Animator[] PyramidAnimators;
        public Animator OctahedronAnimator;
        public Transform[] OctahedronPivots;

        private bool _isActivated = false;
        private bool _isRising = false;
        private float _rise = 0f;
        private float _rotate = 0f;

        public void Awake()
        {
            
        }

        public void Start()
        {
            for (int i = 0; i < PyramidAnimators.Length; i++)
            {
                PyramidAnimators[i].speed = -1f / RiseTime;
            }
            OctahedronAnimator.speed = -1f / RiseTime;
        }

        public void Update()
        {
            _isActivated = DialogueConditionManager.SharedInstance.GetConditionState("ActivateVallhenge");

            var change = false;

            if (_isActivated)
            {
                if (_rise < RiseTime)
                {
                    _rise = Mathf.Min(RiseTime, _rise + Time.deltaTime);
                    _isRising = true;
                    change = true;
                }
                else if (_rotate < RotateTime)
                {
                    _rotate = Mathf.Min(RotateTime, _rotate + Time.deltaTime);
                }
            }
            else
            {
                if (_rotate > 0f)
                {
                    _rotate = Mathf.Max(0f, _rotate - Time.deltaTime);
                }
                else if (_rise > 0f)
                {
                    _rise = Mathf.Max(0f, _rise - Time.deltaTime);
                    _isRising = false;
                    change = true;
                }
            }

            if (change)
            {
                ApplyChange();
            }

            for (int i = 0; i < OctahedronPivots.Length; i++)
            {
                var angle = OctahedronPivots[i].localRotation.eulerAngles;
                var orbit = Time.time * OrbitSpeed / ((i + 1) * OrbitSpeedFalloff);
                angle.y = Mathf.LerpAngle(0f, orbit, _rotate / RotateTime);
                OctahedronPivots[i].localRotation = Quaternion.Euler(angle);
            }
        }

        public void ApplyChange()
        {
            var rise = _rise / RiseTime;

            if (_isRising)
            {
                for (int i = 0; i < PyramidAnimators.Length; i++)
                {
                    PyramidAnimators[i].SetTrigger("Open");
                    PyramidAnimators[i].ResetTrigger("Close");
                }
                OctahedronAnimator.SetTrigger("Rise");
                OctahedronAnimator.ResetTrigger("Lower");
            }
            else
            {
                for (int i = 0; i < PyramidAnimators.Length; i++)
                {
                    PyramidAnimators[i].SetTrigger("Close");
                    PyramidAnimators[i].ResetTrigger("Open");
                }
                OctahedronAnimator.SetTrigger("Lower");
                OctahedronAnimator.ResetTrigger("Rise");
            }

            for (int i = 0; i < EmissionRenderers.Length; i++)
            {
                EmissionRenderers[i].material.SetColor("_EmissionColor", EmissionColor * rise);
            }
            for (int i = 0; i < Lights.Length; i++)
            {
                Lights[i].intensity = rise;
            }
        }
    }
}