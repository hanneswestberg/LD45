using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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

        // Show tutorial here

        // Start the game
        BangManager.instance.StartNewBigBang();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
