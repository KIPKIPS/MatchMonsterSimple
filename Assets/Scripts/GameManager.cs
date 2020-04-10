using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    //单例
    private static GameManager _instance;
    public static GameManager instance {
        get { return _instance; }
        set { _instance = value; }
    }
    public int xCol;//列
    public int yRow;//行
    public GameObject gridPrefab;

    //种类
    public enum ModelType {
        //空,默认,障碍物,行消除,列消除,彩虹道具
        Empty, Normal, Wall, RowClear, ColClear, RainBow, Count//count为标记类型
    }
    //通过字典查找对应类型的预制体
    public Dictionary<ModelType, GameObject> modelPrefabDict;
    [System.Serializable]
    public struct ModelPrefab {
        public ModelType type;
        public GameObject prefab;
    }
    //结构体数组
    public ModelPrefab[] modelPrefabs;
    //二维数组
    public ModelBase[,] models;
    public float fillTime=0.1f;
    void Awake() {
        instance = this;
    }
    void Start() {
        models = new ModelBase[xCol, yRow];
        //为字典赋值
        modelPrefabDict = new Dictionary<ModelType, GameObject>();
        foreach (var mp in modelPrefabs) {
            if (!modelPrefabDict.ContainsKey(mp.type)) {
                modelPrefabDict.Add(mp.type, mp.prefab);
            }
        }
        //实例化格子
        for (int x = 0; x < xCol; x++) {
            for (int y = 0; y < yRow; y++) {
                //生成格子
                GameObject grid = Instantiate(gridPrefab, CalGridPos(x, y), Quaternion.identity);
                grid.transform.parent = this.transform;//将格子的父物体设置为GameManager
            }
        }
        //实例化模型
        for (int x = 0; x < xCol; x++) {
            for (int y = 0; y < yRow; y++) {
                //GameObject newModel = Instantiate(modelPrefabDict[ModelType.Normal], CalGridPos(x, y), Quaternion.identity);
                //newModel.transform.parent = this.transform;//将格子的父物体设置为GameManager
                //models[x, y] = newModel.GetComponent<ModelBase>();//获取组件
                //models[x, y].Init(x, y, this, ModelType.Normal);//初始化
                //models[x,y].ModelMoveComponent.Move(x,0);//移动测试
                //随机颜色
                //if (models[x, y].CanColor()) {
                //    models[x, y].ModelColorComponent.SetColor((ModelColor.ColorType)Random.Range(0, models[x, y].ModelColorComponent.Nums));
                //}
                CreatNewModel(x, y, ModelType.Empty);
            }
        }
        Destroy(models[4,4].gameObject);
        CreatNewModel(4, 4, ModelType.Wall);
        StartCoroutine(FillAll());
    }
    //计算格子的位置坐标
    public Vector3 CalGridPos(int x, int y) {
        return new Vector3(transform.position.x - xCol / 2f * 0.56f + x * 0.65f, transform.position.y + yRow / 2f * 0.15f - y * 0.65f);
    }

    //产生model的方法
    public ModelBase CreatNewModel(int x, int y, ModelType type) {
        GameObject newModel = Instantiate(modelPrefabDict[type], CalGridPos(x, y), Quaternion.identity);
        newModel.transform.parent = transform;
        models[x, y] = newModel.GetComponent<ModelBase>();
        models[x, y].Init(x, y, this, type);
        return models[x, y];
    }
    //全部填充
    public IEnumerator FillAll() {
        while (Fill()) {
            yield return new WaitForSeconds(fillTime);
        }
    }
    //分布填充
    public bool Fill() {
        bool notFinished = false;//本次填充是否完成
        for (int y = yRow-2; y >=0 ; y--) {
            for (int x = 0; x < xCol; x++) {
                ModelBase model = models[x, y];//当前元素的基础组件
                if (model.CanMove()) {
                    ModelBase modelBelow = models[x, y + 1];//正下方model组件
                    if (modelBelow.Type==ModelType.Empty) {
                        Destroy(modelBelow.gameObject);
                        model.ModelMoveComponent.Move(x,y+1,fillTime);//向下移动
                        models[x, y + 1] = model;//正下方的组件指向当前组件
                        CreatNewModel(x, y, ModelType.Empty);//当前元素置空
                        notFinished = true;
                    }
                }
            }
        }
        //最底下的一层
        for (int x = 0; x <xCol ; x++) {
            ModelBase model = models[x, 0];//当前元素的基础组件
            if (model.Type==ModelType.Empty) {
                //在y坐标为-1的位置生成
                GameObject newModel=Instantiate(modelPrefabDict[ModelType.Normal], CalGridPos(x, -1), Quaternion.identity);
                newModel.transform.parent = this.transform;//设置父物体
                models[x, 0] = newModel.GetComponent<ModelBase>();//更新基础组件位置
                models[x,0].Init(x,-1,this,ModelType.Normal);//初始化
                if (models[x, 0].CanMove()) {
                    models[x,0].ModelMoveComponent.Move(x,0,fillTime);//向下移动
                }
                //随机一个颜色
                models[x, 0].ModelColorComponent.SetColor((ModelColor.ColorType)Random.Range(0, models[x, 0].ModelColorComponent.Nums));
                notFinished = true;
            }
        }
        return notFinished;
    }
}
