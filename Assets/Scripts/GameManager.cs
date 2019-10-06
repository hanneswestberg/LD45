﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    private bool debugMode = false;
    public bool DebugMode { get { return debugMode; } }

    // Singleton
    private void Awake() {
        if(instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        if(BangManager.instance == null) {
            Debug.LogError("No BangManager Instance in the scene, the game will not work!");
            return;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public IEnumerator StartAnimation() {
        if(!debugMode) {
            yield return new WaitForSeconds(1f);
            UIManager.instance.UpdatePanelText("> start bigbang.exe", 2f, true, true);
            yield return new WaitForSeconds(3f);
            UIManager.instance.UpdatePanelText("\n...", 1f, false);
            yield return new WaitForSeconds(2f);
            UIManager.instance.UpdatePanelText("\n> big bang creator loaded", 1f, false);
            yield return new WaitForSeconds(2f);
            UIManager.instance.UpdatePanelText("\n> opening up parallel dimension", 1f, false);
            yield return new WaitForSeconds(2f);
            UIManager.instance.UpdatePanelText("\n> initialize big bang by going to phase 5", 1f, false);
            yield return new WaitForSeconds(3f);
            UIManager.instance.UpdatePanelText("\n> please specify action:", 1f, false);
            yield return new WaitForSeconds(2f);

        }

        // Start the game
        BangManager.instance.StartNewBigBang();
        yield return null;
    }
}
