using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelMove : MonoBehaviour {
    public ModelBase modelBase;//基础脚本
    private IEnumerator moveCoroutine;//
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
}
