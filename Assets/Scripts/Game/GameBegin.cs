using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBegin : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void BeginEnd() {
        Time.timeScale = 1;
        this.gameObject.SetActive(false);
    }
    public void BeginStart() {
        Time.timeScale = 0;
    }
}
