using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Logic.Match3
{
    public class ColorPiece : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _sprite;
        [FormerlySerializedAs("colorSprites")] 
        [SerializeField] private ColorSprite[] _colorSprites;
        
        private Dictionary<ColorType, Sprite> _colorSpriteDict;
        private ColorType _color;
        
        private void OnValidate()
        {
            if (_sprite == null)
            {
                Transform pieceTransform = transform.Find("piece");
                if (pieceTransform != null)
                {
                    _sprite = pieceTransform.GetComponent<SpriteRenderer>();
                }
                else
                {
                    _sprite = GetComponent<SpriteRenderer>();
                }
            }
        }

        private void Awake()
        {
            _colorSpriteDict = new Dictionary<ColorType, Sprite>();

            for (int i = 0; i < _colorSprites.Length; i++)
            {
                if (!_colorSpriteDict.ContainsKey (_colorSprites[i].color))
                {
                    _colorSpriteDict.Add(_colorSprites[i].color, _colorSprites[i].sprite);
                }
            }
        }

        public ColorType Color
        {
            get => _color;
            set => SetColor(value);
        }

        public int NumColors => _colorSprites.Length;
        
        public void SetColor(ColorType newColor)
        {
            _color = newColor;

            if (_colorSpriteDict.ContainsKey(newColor))
            {
                _sprite.sprite = _colorSpriteDict[newColor];
            }
        }
	
        [System.Serializable]
        public struct ColorSprite
        {
            public ColorType color;
            public Sprite sprite;
        }
    }
}
