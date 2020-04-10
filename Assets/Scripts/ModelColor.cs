using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ModelColor : MonoBehaviour {
    public enum ColorType {
        Blue,Green,Red,Purple,Pink,Yellow,Award,Count
    }

    private ColorType color;
    public ColorType Color {
        get { return color;}
        set { SetColor(value); } }
    public Dictionary<ColorType, Sprite> colorSpriteDict;
    [System.Serializable]
    public struct ColorSprite {
        public ColorType color;
        public Sprite sprite;
    }
    public ColorSprite[] colorSprites;
    public SpriteRenderer sprite;
    public int Nums {
        get { return colorSprites.Length; }
    }
    void Awake() {
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        //为字典填充值
        colorSpriteDict=new Dictionary<ColorType, Sprite>();
        foreach (var cs in colorSprites) {
            if (!colorSpriteDict.ContainsKey(cs.color)) {
                colorSpriteDict.Add(cs.color,cs.sprite);
            }
        }

    }
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void SetColor(ColorType newColor) {
        color = newColor;
        if (colorSpriteDict.ContainsKey(newColor)) {
            sprite.sprite =colorSpriteDict[newColor];
        }
    }
}
