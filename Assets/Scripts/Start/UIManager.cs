using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public static UIManager instance {
        get { return _instance; }
    }
    private static UIManager _instance;
    public GameObject settingPanel;
    public GameObject aboutUsPanel;
    public GameObject audioOff;
    public AudioClip[] audios;
    public bool canAudio;
    public Animator[] anims;
    public MapController curMap;

    public GameObject mapPanel;
    void Awake() {
        _instance = this;
        canAudio = PlayerPrefs.GetInt("Audio",1) == 1;
        if (canAudio) {
            Camera.main.GetComponent<AudioSource>().Play();
        }
        else {
            Camera.main.GetComponent<AudioSource>().Stop();
        }
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        canAudio = PlayerPrefs.GetInt("Audio") == 1;
    }
    //更换地图
    public void MapChange() {
        if (canAudio) {
            AudioSource.PlayClipAtPoint(audios[0], Camera.main.transform.position, 0.5f);
            AudioSource.PlayClipAtPoint(audios[1], Camera.main.transform.position, 1);
        }
        //EventSystem.current.currentSelectedGameObject.GetComponent<Animator>().SetTrigger("select");
        curMap = EventSystem.current.currentSelectedGameObject.GetComponent<MapController>();
        Debug.Log(curMap.Index);
        for (int i = 0; i < 3; i++) {
            if ( i != curMap.Index) {
                anims[i].SetTrigger("hide");
            }
            else {
                anims[i].transform.GetComponent<UnityEngine.UI.Button>().enabled = false;
                anims[i].SetTrigger("select");
            }
        }
    }

    public void MapClose() {
        if (canAudio) {
            AudioSource.PlayClipAtPoint(audios[0], Camera.main.transform.position, 0.5f);
            AudioSource.PlayClipAtPoint(audios[2], Camera.main.transform.position, 1);
        }
        for (int i = 0; i < 3; i++) {
            if ( i != curMap.Index) {
                anims[i].SetTrigger("show");
                anims[i].transform.GetComponent<UnityEngine.UI.Button>().enabled = true;
            }
            else {
                anims[i].transform.GetComponent<UnityEngine.UI.Button>().enabled = true;
                anims[i].SetTrigger("close");
            }
        }
    }

    public void StartGame() {
        if (canAudio) {
            AudioSource.PlayClipAtPoint(audios[0], Camera.main.transform.position, 0.5f);
            AudioSource.PlayClipAtPoint(audios[1], Camera.main.transform.position, 1);
        }
        mapPanel.GetComponent<Animator>().SetTrigger("tomap");
        for (int i = 0; i < 3; i++) {
            if (curMap!=null) {
                if (i != curMap.Index) {
                    anims[i].SetTrigger("show");
                    anims[i].transform.GetComponent<UnityEngine.UI.Button>().enabled = true;
                }
                else {
                    anims[i].transform.GetComponent<UnityEngine.UI.Button>().enabled = true;
                    anims[i].SetTrigger("close");
                }
            }
        }
    }

    public void Setting() {
        settingPanel.SetActive(true);
        if (canAudio) {
            audioOff.SetActive(false);
            AudioSource.PlayClipAtPoint(audios[0], Camera.main.transform.position, 0.5f);
            AudioSource.PlayClipAtPoint(audios[1], Camera.main.transform.position, 1);
        }
        else {
            audioOff.SetActive(true);
        }
        settingPanel.GetComponent<Animator>().SetTrigger("open");
    }

    public void SettingPanelClose() {
        if (canAudio) {
            AudioSource.PlayClipAtPoint(audios[0], Camera.main.transform.position, 0.5f);
            AudioSource.PlayClipAtPoint(audios[2], Camera.main.transform.position, 1);
        }
        
        settingPanel.GetComponent<Animator>().SetTrigger("close");
    }

    public void AboutUs() {
        if (canAudio) {
            AudioSource.PlayClipAtPoint(audios[0], Camera.main.transform.position, 0.5f);
            AudioSource.PlayClipAtPoint(audios[1], Camera.main.transform.position, 1);
        }
        
        settingPanel.SetActive(false);
        aboutUsPanel.GetComponent<Animator>().SetTrigger("open");
    }
    public void AboutUsClose() {
        if (canAudio) {
            AudioSource.PlayClipAtPoint(audios[0], Camera.main.transform.position, 0.5f);
            AudioSource.PlayClipAtPoint(audios[2], Camera.main.transform.position, 1);
        }
        
        aboutUsPanel.GetComponent<Animator>().SetTrigger("close");
    }

    public void AudioOnOff() {
        if (canAudio) {
            AudioSource.PlayClipAtPoint(audios[0], Camera.main.transform.position, 0.5f);
        }
        if (canAudio) {
            audioOff.SetActive(true);
            PlayerPrefs.SetInt("Audio",0);
            Camera.main.GetComponent<AudioSource>().Stop();
        }
        else {
            audioOff.SetActive(false);
            PlayerPrefs.SetInt("Audio", 1);
            Camera.main.GetComponent<AudioSource>().Play();
        }
    }

    public void BackToMenu() {
        if (canAudio) {
            AudioSource.PlayClipAtPoint(audios[0], Camera.main.transform.position, 0.5f);
            AudioSource.PlayClipAtPoint(audios[2], Camera.main.transform.position, 1);
        }
        mapPanel.GetComponent<Animator>().SetTrigger("tomenu");
    }

    public void SelectLevel() {
        Debug.Log(EventSystem.current.currentSelectedGameObject.name);
        SceneManager.LoadScene(1);
    }
}
