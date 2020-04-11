using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelMove : MonoBehaviour {
    public ModelBase modelBase;//基础脚本
    private IEnumerator moveCoroutine;//
    public IEnumerator originCoroutine;
    void Awake() {
        modelBase = GetComponent<ModelBase>();
    }
    void Start() {

    }
    void Update() {

    }
    //开启或者结束协程
    public void Move(int newX, int newY, float time) {
        //更改属性
        if (moveCoroutine!=null) {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = MoveCoroutine(newX, newY, time);
        StartCoroutine(moveCoroutine);
    }

    private IEnumerator MoveCoroutine(int newX, int newY,float time) {
        modelBase.X = newX;
        modelBase.Y = newY;
        //每一帧都去移动
        Vector3 startPos = transform.position;
        Vector3 endPos = modelBase.manager.CalGridPos(newX, newY);
        for (float t=0;t<time;t+=Time.deltaTime) {
            modelBase.transform.position = Vector3.Lerp(startPos, endPos, t / time);
            yield return 0;
        }
        modelBase.transform.position = endPos;
    }

    public bool isPlaying=false;
    public void Origin(ModelBase m1,ModelBase m2, float time) {
        //更改属性
        originCoroutine = OriginCoroutine(m1,m2,time);
        if (isPlaying==false) {
            StartCoroutine(originCoroutine);
        }
    }

    private IEnumerator OriginCoroutine(ModelBase m1, ModelBase m2, float time) {
        isPlaying = true;
        //每一帧都去移动
        Vector3 m1pos = GameManager.instance.CalGridPos(m1.X,m1.Y);
        Vector3 m2pos = GameManager.instance.CalGridPos(m2.X, m2.Y);
        for (float t = 0; t < time; t += Time.deltaTime) {
            m1.transform.position= Vector3.Lerp(m1pos, m2pos, t / time*1.5f);
            m2.transform.position = Vector3.Lerp(m2pos, m1pos, t / time * 1.5f);
            yield return 0;
        }
        yield return new WaitForSeconds(0.1f);
        for (float t = 0; t <time; t += Time.deltaTime) {
            m1.transform.position = Vector3.Lerp(m2pos, m1pos, t / time * 1.5f);
            m2.transform.position = Vector3.Lerp(m1pos, m2pos, t / time * 1.5f);
            yield return 0;
        }
        isPlaying = false;
        //Debug.Log(m1pos+" "+m2pos);
    }
}
