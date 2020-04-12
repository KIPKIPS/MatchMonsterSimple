﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelBase : MonoBehaviour {
    private int x;
    public int X {
        get { return x; }
        set {
            if (CanMove()) {
                x = value;
            }
        }
    }
    private int y;
    public int Y {
        get { return y; }
        set {
            if (CanMove()) {
                y = value;
            }
        }
    }
    private GameManager.ModelType type;
    public GameManager.ModelType Type {
        get { return type; }
        set { type = value; }
    }
    public GameManager manager;
    private ModelMove modelMoveComponent;
    public ModelMove ModelMoveComponent {
        get { return modelMoveComponent; }
    }
    private ModelColor modelColorComponent;
    public GameObject effect_Row;
    public GameObject effect_Col;
    public ModelColor ModelColorComponent {
        get { return modelColorComponent; }
    }
    private ModelClear modelClearComponent;
    public ModelClear ModelClearComponent {
        get { return modelClearComponent; }
    }
    void Awake() {
        modelClearComponent = GetComponent<ModelClear>();
        modelMoveComponent = GetComponent<ModelMove>();
        modelColorComponent = GetComponent<ModelColor>();
    }
    public bool CanClear() {
        return modelClearComponent != null;
    }
    public bool CanMove() {
        return modelMoveComponent != null;
    }
    public bool CanColor() {
        return modelColorComponent != null;
    }
    void Start() {

    }

    void Update() {

    }
    //初始化方法
    public void Init(int _x, int _y, GameManager _manager, GameManager.ModelType _type) {
        x = _x;
        y = _y;
        manager = _manager;
        type = _type;
    }

    public void OnMouseEnter() {
        manager.TargetModel(this);//设置目标
    }
    public void OnMouseDown() {
        manager.SelectModel(this);//选中model
        if (GameManager.instance.lastSelectModel != GameManager.instance.selectModel) {
            if (GameManager.instance.lastSelectModel != null) {
                ModelColorComponent.UnSelect(GameManager.instance.lastSelectModel.ModelColorComponent.highlight);
            }
            ModelColorComponent.Select(GameManager.instance.selectModel.ModelColorComponent.highlight);
        }
        else {
            ModelColorComponent.Select(GameManager.instance.selectModel.ModelColorComponent.highlight);
        }
        if (Type == GameManager.ModelType.CrossClear) {
            manager.ClearCross(x,y);
            //TODO:特效
            Instantiate(effect_Row, new Vector3(0, 1.2f-0.65f*y, 0), Quaternion.Euler(new Vector3(0,0,90)));
            Instantiate(effect_Col,new Vector3(-2.2f + 0.65f * x, -1.6f, 0) , Quaternion.identity);
        }
    }
    public void OnMouseUp() {
        manager.ReleaseModel();
    }
}