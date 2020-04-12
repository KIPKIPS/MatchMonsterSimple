using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public ModelBase lastSelectModel;
    public ModelBase selectModel;//鼠标点击的当前对象
    public ModelBase targetModel;//玩家目的移动对象
    //种类
    public enum ModelType {
        //空,默认,障碍物,行消除,列消除,彩虹道具
        Empty, Normal, Wall, CrossClear, RainBow, Count//count为标记类型
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
    public ModelBase[,] models;//二维数组
    public float fillTime = 0.1f;//填充时间间隔
    //audio
    public AudioClip[] audios;
    //UI控制相关
    public GameObject pausePanel;
    public GameObject aboutUs;
    public GameObject audioOnOff;
    public bool canAudio;
    public Text scoreText;//score
    public Text restTimeText;//time
    public float gameTime = 60f;
    private bool gameover;
    public int score;
    public GameObject gameoverPanel;
    public Text overScoreText;
    public Text historySoreText;
    public GameObject breakRecord;
    public Transform spawn;
    public GameObject[] excellent;
    void Awake() {
        //PlayerPrefs.SetInt("HistoryHighestScore", 0);
        gameover = false;
        instance = this;
        canAudio = true;
        if (canAudio) {
            Camera.main.GetComponent<AudioSource>().Play();
        }
        else {
            Camera.main.GetComponent<AudioSource>().Stop();
        }
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
                CreatNewModel(x, y, ModelType.Empty);
            }
        }
        Destroy(models[4, 4].gameObject);
        CreatNewModel(4, 4, ModelType.Wall);
        StartCoroutine(FillAll());
    }

    void Update() {
        if (gameover) {
            return;
        }
        gameTime -= Time.deltaTime;
        if (gameTime <= 0) {
            gameTime = 0;
            //TODO:失败处理
            gameover = true;
            gameoverPanel.SetActive(true);
            GameOver();
            return;
        }
        restTimeText.text = gameTime.ToString("0");//0取整,0.0保留一位小数,0.00保留两位小数......
        scoreText.text = score + "";
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
        bool needFill = true;
        while (needFill) {
            yield return new WaitForSeconds(fillTime);
            while (Fill()) {
                yield return new WaitForSeconds(fillTime);
            }
            //清除匹配的model
            needFill = ClearAllMatchModels();
        }
    }
    //分布填充
    public bool Fill() {
        bool notFinished = false;//本次填充是否完成
        for (int y = yRow - 2; y >= 0; y--) {
            for (int x = 0; x < xCol; x++) {
                ModelBase model = models[x, y];//当前元素的基础组件
                //向下填充空缺
                if (model.CanMove()) {
                    ModelBase modelBelow = models[x, y + 1];//正下方model组件
                    if (modelBelow.Type == ModelType.Empty) {//垂直填充
                        Destroy(modelBelow.gameObject);
                        model.ModelMoveComponent.Move(x, y + 1, fillTime);//向下移动
                        models[x, y + 1] = model;//正下方的组件指向当前组件
                        CreatNewModel(x, y, ModelType.Empty);//当前元素置空
                        notFinished = true;
                    }
                    //斜向填充,用于解决存在障碍物的情况
                    else {
                        for (int down = -1; down <= 1; down++) {
                            if (down != 0) {
                                int downX = x + down;
                                if (downX >= 0 && downX < xCol) {//排除最右侧
                                    ModelBase downModel = models[downX, y + 1];
                                    if (downModel.Type == ModelType.Empty) {
                                        bool canFill = true;//是否满足垂直填充
                                        for (int aboveY = y; aboveY >= 0; aboveY--) {
                                            ModelBase modelAbove = models[downX, aboveY];
                                            if (modelAbove.CanMove()) {
                                                break;
                                            }
                                            else if (!modelAbove.CanMove() && modelAbove.Type != ModelType.Empty) {
                                                canFill = false;
                                                break;
                                            }
                                        }
                                        //斜向填充
                                        if (!canFill) {
                                            Destroy(downModel.gameObject);
                                            model.ModelMoveComponent.Move(downX, y + 1, fillTime);
                                            models[downX, y + 1] = model;
                                            CreatNewModel(x, y, ModelType.Empty);
                                            notFinished = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        //最底下的一层
        for (int x = 0; x < xCol; x++) {
            ModelBase model = models[x, 0];//当前元素的基础组件
            if (model.Type == ModelType.Empty) {
                //在y坐标为-1的位置生成
                GameObject newModel = Instantiate(modelPrefabDict[ModelType.Normal], CalGridPos(x, -1), Quaternion.identity);
                newModel.transform.parent = this.transform;//设置父物体
                models[x, 0] = newModel.GetComponent<ModelBase>();//更新基础组件位置
                models[x, 0].Init(x, -1, this, ModelType.Normal);//初始化
                if (models[x, 0].CanMove()) {
                    models[x, 0].ModelMoveComponent.Move(x, 0, fillTime);//向下移动
                }
                //随机一个颜色
                models[x, 0].ModelColorComponent.SetColor((ModelColor.ColorType)Random.Range(0, models[x, 0].ModelColorComponent.Nums));
                notFinished = true;
            }
        }
        return notFinished;
    }
    //是否相邻判定
    public bool IsNeighbor(ModelBase m1, ModelBase m2) {
        return m1.X == m2.X && Mathf.Abs(m1.Y - m2.Y) == 1 || m1.Y == m2.Y && Mathf.Abs(m1.X - m2.X) == 1;
    }
    //交换model
    private void ExchangeModel(ModelBase m1, ModelBase m2) {
        //Debug.Log(selectModel.ModelColorComponent.Color+" "+targetModel.ModelColorComponent.Color);
        if (m1.CanMove() && m2.CanMove()) {
            models[m1.X, m1.Y] = m2;
            models[m2.X, m2.Y] = m1;
            int tempX = m1.X;
            int tempY = m1.Y;
            if (MatchModels(m2, m1.X, m1.Y) != null || MatchModels(m1, m2.X, m2.Y) != null) {
                //Debug.Log("可以交换");
                m1.ModelMoveComponent.Move(m2.X, m2.Y, fillTime);//交换
                m2.ModelMoveComponent.Move(tempX, tempY, fillTime);
                ClearAllMatchModels();//清除所有匹配的model
                StartCoroutine(FillAll());//将消除后的空位进行填充
            }
            else {
                //Debug.Log("不可以交换");
                //还原基础脚本
                models[m1.X, m1.Y] = m1;
                models[m2.X, m2.Y] = m2;
                models[m1.X, m1.Y].ModelMoveComponent.Undo(m1, m2, fillTime);//交换位置再还原
            }
        }

    }


    //选中对象
    public void SelectModel(ModelBase m) {
        if (gameover) {
            return;
        }
        lastSelectModel = selectModel;
        selectModel = m;
    }
    //目标对象
    public void TargetModel(ModelBase m) {
        if (gameover) {
            return;
        }
        targetModel = m;
    }
    //鼠标抬起,model交换
    public void ReleaseModel() {
        if (gameover) {
            return;
        }
        if (IsNeighbor(selectModel, targetModel)) {
            ExchangeModel(selectModel, targetModel);
        }
    }
    //匹配model
    public List<ModelBase> MatchModels(ModelBase model, int newX, int newY) {
        if (model.CanColor()) {
            ModelColor.ColorType color = model.ModelColorComponent.Color;
            List<ModelBase> matchRow = new List<ModelBase>();//存取行
            List<ModelBase> matchCol = new List<ModelBase>();//存取列
            List<ModelBase> match = new List<ModelBase>();//存取全部可消除的列表
            //行匹配
            matchRow.Add(model);
            //i=0代表往左，i=1代表往右
            for (int i = 0; i <= 1; i++) {
                for (int xDistance = 1; xDistance < xCol; xDistance++) {
                    int x;
                    if (i == 0) {
                        x = newX - xDistance;
                    }
                    else {
                        x = newX + xDistance;
                    }
                    if (x < 0 || x >= xCol) {
                        break;
                    }
                    if (models[x, newY].CanColor() && models[x, newY].ModelColorComponent.Color == color) {
                        matchRow.Add(models[x, newY]);
                    }
                    else {
                        break;
                    }
                }
            }
            if (matchRow.Count >= 3) {
                for (int i = 0; i < matchRow.Count; i++) {
                    match.Add(matchRow[i]);
                }
            }
            //L T型匹配
            //检查一下当前行遍历列表中的元素数量是否大于3
            if (matchRow.Count >= 3) {
                for (int i = 0; i < matchRow.Count; i++) {
                    //行匹配列表中满足匹配条件的每个元素上下依次进行列遍历
                    // 0代表上方 1代表下方
                    for (int j = 0; j <= 1; j++) {
                        for (int yDistance = 1; yDistance < yRow; yDistance++) {
                            int y;
                            if (j == 0) {
                                y = newY - yDistance;
                            }
                            else {
                                y = newY + yDistance;
                            }
                            if (y < 0 || y >= yRow) {
                                break;
                            }
                            if (models[matchRow[i].X, y].CanColor() && models[matchRow[i].X, y].ModelColorComponent.Color == color) {
                                matchCol.Add(models[matchRow[i].X, y]);
                            }
                            else {
                                break;
                            }
                        }
                    }
                    if (matchCol.Count < 2) {
                        matchCol.Clear();
                    }
                    else {
                        for (int j = 0; j < matchCol.Count; j++) {
                            match.Add(matchCol[j]);
                        }
                        break;
                    }
                }
            }
            //if (match.Count >= 3) {
            //    return match;
            //}
            matchRow.Clear();
            matchCol.Clear();
            matchCol.Add(model);
            //列匹配
            //i=0代表往左，i=1代表往右
            for (int i = 0; i <= 1; i++) {
                for (int yDistance = 1; yDistance < yRow; yDistance++) {
                    int y;
                    if (i == 0) {
                        y = newY - yDistance;
                    }
                    else {
                        y = newY + yDistance;
                    }
                    if (y < 0 || y >= yRow) {
                        break;
                    }
                    if (models[newX, y].CanColor() && models[newX, y].ModelColorComponent.Color == color) {
                        matchCol.Add(models[newX, y]);
                    }
                    else {
                        break;
                    }
                }
            }
            if (matchCol.Count >= 3) {
                for (int i = 0; i < matchCol.Count; i++) {
                    match.Add(matchCol[i]);
                }
            }
            //L T型匹配
            //检查一下当前行遍历列表中的元素数量是否大于3
            if (matchCol.Count >= 3) {
                for (int i = 0; i < matchCol.Count; i++) {
                    //行匹配列表中满足匹配条件的每个元素上下依次进行列遍历
                    // 0代表上方 1代表下方
                    for (int j = 0; j <= 1; j++) {
                        for (int xDistance = 1; xDistance < xCol; xDistance++) {
                            int x;
                            if (j == 0) {
                                x = newY - xDistance;
                            }
                            else {
                                x = newY + xDistance;
                            }
                            if (x < 0 || x >= xCol) {
                                break;
                            }
                            if (models[x, matchCol[i].Y].CanColor() && models[x, matchCol[i].Y].ModelColorComponent.Color == color) {
                                matchRow.Add(models[x, matchCol[i].Y]);
                            }
                            else {
                                break;
                            }
                        }
                    }
                    if (matchRow.Count < 2) {
                        matchRow.Clear();
                    }
                    else {
                        for (int j = 0; j < matchRow.Count; j++) {
                            match.Add(matchRow[j]);
                        }
                        break;
                    }
                }
            }
            if (match.Count >= 3) {
                return match;
            }
        }
        return null;
    }
    //清除model
    public bool ClearModel(int x, int y) {
        //当前model可以清除并且没有正在清除
        if (models[x, y].CanClear() && models[x, y].ModelClearComponent.IsClearing == false) {
            if (models[x, y].Type!=ModelType.CrossClear) {
                models[x, y].ModelClearComponent.Clear();//将model清除掉
                CreatNewModel(x, y, ModelType.Empty);//原地生成一个新的空类型
                ClearRoadblock(x, y);//清除障碍物
                return true;
            }
        }
        return false;
    }
    //清除障碍物
    public void ClearRoadblock(int x, int y) {//被消除model的坐标
        for (int nearX = x - 1; nearX <= x + 1; nearX++) {
            //若不为自身,未超出格子边界,类型为wall,可以清除
            if (nearX != x && nearX >= 0 && nearX < xCol) {
                if (models[nearX, y].CanClear() && models[nearX, y].Type == ModelType.Wall) {
                    //Debug.Log("clear");
                    models[nearX, y].ModelClearComponent.Clear();//消除障碍物
                    CreatNewModel(nearX, y, ModelType.Empty);//原地置空等待填充
                }
            }
        }
        for (int nearY = y - 1; nearY <= y + 1; nearY++) {
            //若不为自身,未超出格子边界,类型为wall,可以清除
            if (nearY != y && nearY >= 0 && nearY < yRow) {
                if (models[x, nearY].CanClear() && models[x, nearY].Type == ModelType.Wall) {
                    //Debug.Log("clear");
                    models[x, nearY].ModelClearComponent.Clear();//消除障碍物
                    CreatNewModel(x, nearY, ModelType.Empty);//原地置空等待填充
                }

            }
        }
    }
    //清除匹配的model列表
    public bool ClearAllMatchModels() {
        bool needFill = false;
        for (int y = 0; y < yRow; y++) {
            for (int x = 0; x < xCol; x++) {
                if (models[x, y].CanClear()) {
                    List<ModelBase> matchList = MatchModels(models[x, y], x, y);
                    if (matchList != null) {
                        ModelType specialModelType = ModelType.Count;//是否产生特殊奖励
                        ModelBase model = matchList[0];
                        int specialModelX = model.X;
                        int specialModelY = model.Y;
                        if (matchList.Count == 4) {
                            specialModelType = ModelType.CrossClear;
                        }
                        foreach (var m in matchList) {
                            if (ClearModel(m.X, m.Y)) {
                                needFill = true;
                            }
                        }
                        if (specialModelType != ModelType.Count) {
                            Destroy(models[specialModelX,specialModelY].gameObject);
                            ModelBase newModel = CreatNewModel(specialModelX, specialModelY, specialModelType);
                            models[specialModelX, specialModelY] = newModel;
                        }
                        //根据消除个数处理分数
                        int num = matchList.Count;
                        switch (num) {
                            case 3:
                                score += 10;
                                break;
                            case 4:
                                Excellent(0);
                                score += 20; break;
                            case 5:
                                score += 30;
                                Excellent(1);
                                break;
                            case 6: score += 60; break;
                            case 7: score += 150; break;
                            case 8: score += 300; break;
                            case 9: score += 500; break;
                        }
                    }
                }
            }
        }
        return needFill;
    }

    public void ClearCross(int x, int y) {
        Debug.Log("cross");
        for (int i = 0; i < xCol; i++) {
            ClearModel(i, y);
        }
        for (int j = 0; j < yRow; j++) {
            if (j!=y) {
                ClearModel(x, j);
            }
        }
        score += 200;
        models[x, y].ModelClearComponent.Clear();
        models[x, y] = CreatNewModel(x, y, ModelType.Empty);
        StartCoroutine(FillAll());
    }
    //处理UI界面的事件
    public void Excellent(int index) {
        Instantiate(excellent[index], spawn);
    }
    public void Pause() {
        pausePanel.SetActive(true);
        pausePanel.GetComponent<Animator>().SetTrigger("open");
        if (canAudio) {
            AudioSource.PlayClipAtPoint(audios[0], Camera.main.transform.position, 1);
        }
    }

    public void Resume() {
        Time.timeScale = 1;
        pausePanel.GetComponent<Animator>().SetTrigger("close");
        if (canAudio) {
            AudioSource.PlayClipAtPoint(audios[1], Camera.main.transform.position, 1);
        }
    }

    public void Replay() {
        Time.timeScale = 1;
        gameoverPanel.GetComponent<Animator>().SetTrigger("close");
        SceneManager.LoadScene(1);
    }
    public void Quit() {
        Time.timeScale = 1;
        gameoverPanel.GetComponent<Animator>().SetTrigger("close");
        SceneManager.LoadScene(0);
    }
    public void AboutUsDisplay() {
        Time.timeScale = 1;
        pausePanel.GetComponent<Animator>().SetTrigger("close");
        aboutUs.SetActive(true);
        aboutUs.GetComponent<Animator>().SetTrigger("display");
        if (canAudio) {
            AudioSource.PlayClipAtPoint(audios[0], Camera.main.transform.position, 1);
        }
    }
    public void AboutUsClose() {
        Time.timeScale = 1;
        aboutUs.GetComponent<Animator>().SetTrigger("close");
        if (canAudio) {
            AudioSource.PlayClipAtPoint(audios[1], Camera.main.transform.position, 1);
        }
    }
    public void AudioController() {
        if (canAudio) {
            canAudio = false;
            audioOnOff.SetActive(true);
            Camera.main.GetComponent<AudioSource>().Stop();
        }
        else {
            canAudio = true;
            audioOnOff.SetActive(false);
            Camera.main.GetComponent<AudioSource>().Play();
        }
    }

    public void GameOver() {
        if (score > PlayerPrefs.GetInt("HistoryHighestScore", 0)) {
            PlayerPrefs.SetInt("HistoryHighestScore", score);
            breakRecord.SetActive(true);
        }
        gameoverPanel.GetComponent<Animator>().SetTrigger("display");
        overScoreText.text = score.ToString();
        historySoreText.text = PlayerPrefs.GetInt("HistoryHighestScore").ToString();
    }
}
