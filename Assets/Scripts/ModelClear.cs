using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelClear : MonoBehaviour {
    public AnimationClip destoryAnim;
    private bool isClearing;//当前model是否正在被清除
    public bool IsClearing {
        get { return isClearing; }
    }
    public ModelBase modelBase;

    public virtual void Clear() {
        isClearing = true;
        StartCoroutine(ClearCoroutine());
    }

    public IEnumerator ClearCoroutine() {
        Animator animator = GetComponent<Animator>();
        if (animator!=null) {
            animator.SetTrigger("clear");
            //animator.Play(destoryAnim.name);//播放清除动画
            //玩家分数,音效
            yield return new WaitForSeconds(destoryAnim.length);//等待清除动画播放的时间
            Destroy(this.gameObject);
        }
    }
    void Start() {

    }
    void Update() {

    }
}
