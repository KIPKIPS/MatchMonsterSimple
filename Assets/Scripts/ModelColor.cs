using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ModelColor : MonoBehaviour {
    public enum ColorType {
        Blue,Green,Red,Purple,Pink,Award,Count
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

    public Animator anim;
    private IEnumerator ie;
    void Awake() {
        anim = GetComponent<Animator>();
        ie = Idle();
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
        StartCoroutine(ie);
    }

    IEnumerator Idle() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(0, 50));
        anim.SetTrigger("idle");
        }
        
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

    void OnDestroy() {
        StopCoroutine(ie);
    }
}
