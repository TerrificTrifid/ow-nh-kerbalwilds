using NewHorizons.Utility.OWML;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;

namespace KerbalWilds
{
    public class Vallhenge : MonoBehaviour
    {
        public static float RiseTime = 3f;
        public static float RotateTime = 4f;
        public static float OrbitSpeed = 180f;
        public static float OrbitSpeedFalloff = 10f;
        public static Color EmissionColor = new Color(0.5f, 0f, 1f);
        
        public MeshRenderer[] EmissionRenderers;
        public Light[] Lights;
        public Animator[] PyramidAnimators;
        public Animator OctahedronAnimator;
        public Transform[] OctahedronPivots;

        private bool _isActivated = false;
        private bool _isOpen = false;
        private bool _isClosed = false;
        private float _rise = 0f;
        private float _rotate = 0f;

        public void Awake()
        {
            
        }

        public void Start()
        {
            for (int i = 0; i < EmissionRenderers.Length; i++)
            {
                EmissionRenderers[i].material.SetColor("_EmissionColor", Color.black);
            }
            for (int i = 0; i < Lights.Length; i++)
            {
                Lights[i].intensity = 0f;
            }

            for (int i = 0; i < PyramidAnimators.Length; i++)
            {
                PyramidAnimators[i].speed = 1f / RiseTime;
            }
            OctahedronAnimator.speed = 1f / RiseTime;
            ClosePyramids();
            _isClosed = true;
        }

        public void Update()
        {
            _isActivated = DialogueConditionManager.SharedInstance.GetConditionState("ActivateVallhenge");

            var visualChange = false;

            if (_isActivated)
            {
                if (_rise < RiseTime)
                {
                    _rise = Mathf.Min(RiseTime, _rise + Time.deltaTime);
                    visualChange = true;

                    _isClosed = false;
                    if (!_isOpen)
                    {
                        OpenPyramids();
                        _isOpen = true;
                    }
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
                    visualChange = true;

                    _isOpen = false;
                    if (!_isClosed)
                    {
                        ClosePyramids();
                        _isClosed = true;
                    }
                }
            }

            if (visualChange)
            {
                ApplyVisualChange();
            }

            for (int i = 0; i < OctahedronPivots.Length; i++)
            {
                var angle = OctahedronPivots[i].localRotation.eulerAngles;
                var orbit = (Time.time - _rotate) * OrbitSpeed / ((i + 1) * OrbitSpeedFalloff);
                angle.y = Mathf.LerpAngle(0f, orbit, _rotate / RotateTime);
                OctahedronPivots[i].localRotation = Quaternion.Euler(angle);
            }
        }

        public void ApplyVisualChange()
        {
            var rise = _rise / RiseTime;

            for (int i = 0; i < EmissionRenderers.Length; i++)
            {
                EmissionRenderers[i].material.SetColor("_EmissionColor", EmissionColor * rise);
            }
            for (int i = 0; i < Lights.Length; i++)
            {
                Lights[i].intensity = rise;
            }
        }

        public void OpenPyramids()
        {
            for (int i = 0; i < PyramidAnimators.Length; i++)
            {
                PyramidAnimators[i].SetTrigger("Open");
                PyramidAnimators[i].ResetTrigger("Close");
            }
            OctahedronAnimator.SetTrigger("Rise");
            OctahedronAnimator.ResetTrigger("Lower");
        }

        public void ClosePyramids()
        {
            for (int i = 0; i < PyramidAnimators.Length; i++)
            {
                PyramidAnimators[i].SetTrigger("Close");
                PyramidAnimators[i].ResetTrigger("Open");
            }
            OctahedronAnimator.SetTrigger("Lower");
            OctahedronAnimator.ResetTrigger("Rise");
        }
    }
}