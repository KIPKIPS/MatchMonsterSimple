using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelBase : MonoBehaviour {
    private int x;
    public int X {
        get { return x;}
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
    public ModelColor ModelColorComponent {
        get { return modelColorComponent; }
    }
    void Awake() {
        modelMoveComponent = GetComponent<ModelMove>();
        modelColorComponent = GetComponent<ModelColor>();
    }
    public bool CanMove() {
        return modelMoveComponent != null;
    }
    public bool CanColor() {
        return modelColorComponent != null;
    }
    void Start () {
		
	}

	void Update () {
		
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
    }
    public void OnMouseUp() {
        manager.ReleaseModel();
    }
}