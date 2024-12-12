using NewHorizons.Utility.OWML;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;

namespace KerbalWilds
{
    public class GreenMonolith : MonoBehaviour
    {
        public static float FadeTime = 0.5f;
        public static float MorphTime = 0.2f;
        public static Color Color1 = Color.green;
        public static Color Color2 = Color.red;
        public static string Color1String = "<color=lime>";
        public static string Color2String = "<color=red>";

        public MeshRenderer[] Renderers;

        private NomaiTranslator _nomaiTranslator;

        private bool _isReading;
        private bool _useSecondColor;
        private float _fade;
        private float _morph;

        private NomaiText _text;
        private Color[] _colors;

        public void Awake()
        {
            GlobalMessenger.AddListener("EquipTranslator", OnEquipTranslator);
            GlobalMessenger.AddListener("UnequipTranslator", OnUnequipTranslator);
        }

        public void Start()
        {
            _nomaiTranslator = Locator.GetToolModeSwapper().GetTranslator();

            var recorder = transform.Find("Prefab_NOM_Recorder");
            recorder.Find("Props_NOM_Recorder").gameObject.SetActive(false);
            recorder.Find("PointLight_NOM_Recorder").gameObject.SetActive(false);
            recorder.Find("Audio_Recorder").gameObject.SetActive(false);
            
            _text = recorder.GetComponentInChildren<NomaiText>();
            _colors = new Color[_text.GetNumTextBlocks()];
            for (int i = 0; i < _colors.Length; i++)
            {
                var node = _text.GetTextNode(i + 1);
                if (node.IndexOf(Color1String) != -1)
                {
                    _colors[i] = Color1;
                }
                else if (node.IndexOf(Color2String) != -1)
                {
                    _colors[i] = Color2;
                }
                else
                {
                    _colors[i] = Color.black;
                    KerbalWilds.Instance.ModHelper.Console.WriteLine($"erm no monolith text color", MessageType.Error);
                }
            }
            
        }

        public void Update()
        {
            if (_isReading)
            {
                _useSecondColor = _colors[_nomaiTranslator._translatorProp._currentTextID - 1] == Color2;
            }

            var change = false;

            if (_isReading && _fade < FadeTime)
            {
                _fade = Mathf.Min(FadeTime, _fade + Time.deltaTime);
                change = true;
            }
            else if (!_isReading && _fade > 0f)
            {
                _fade = Mathf.Max(0f, _fade - Time.deltaTime);
                change = true;
            }

            if (_useSecondColor && _morph < MorphTime)
            {
                _morph = Mathf.Min(MorphTime, _morph + Time.deltaTime);
                change = true;
            }
            else if (!_useSecondColor && _morph > 0f)
            {
                _morph = Mathf.Max(0f, _morph - Time.deltaTime);
                change = true;
            }

            if (change)
            {
                ApplyChange();
            }
        }

        public void ApplyChange()
        {
            var value = Mathf.Min(_fade / FadeTime, Mathf.Abs(Mathf.Cos(Mathf.PI * _morph / MorphTime)));
            var color = _morph < MorphTime * 0.5f ? Color1 : Color2;
            color *= new Vector4(value, value, value, 1);

            for (int i = 0; i < Renderers.Length; i++)
            {
                Renderers[i].material.SetColor("_EmissionColor", color);
            }
        }

        public void OnEquipTranslator()
        {
            if (_nomaiTranslator._translatorProp._nomaiTextComponent.Equals(_text))
            {
                _isReading = true;
            }
        }

        public void OnUnequipTranslator()
        {
            _isReading = false;
        }
    }
}