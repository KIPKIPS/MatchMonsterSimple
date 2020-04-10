using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	//单例
	private static GameManager _instance;
	public GameManager instance { get; set; }
	public int xCol;//列
	public int yRow;//行
	public GameObject gridPrefab;
	void Awake() {
		instance = this;
	}
	void Start () {
		for(int i=0;)
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
