using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
Destroy eachother or castle!
Go to points. 
     
*/

public class GameControllerScript : MonoBehaviour {

    public Transform body;
    public Transform hpBar;
    public Transform textWinner;
    public List<float> playerHP = new List<float>(){100f, 100f, 100f, 100f };
    public List<Transform> playerBarHP = new List<Transform>();
    public AudioClip music;

    private int playerDied;
    private Transform guiC;
    private AudioSource bgm;

    // Use this for initialization
    void Start () {
        guiC = GameObject.Find("GUIhelper").transform;
        makeHpBars();
        for(int i=0; i<4; ++i)
        {
            StartCoroutine(spawnP(i));
        }
        playerDied = 0;
        bgm = this.gameObject.AddComponent<AudioSource>();
        bgm.clip = music;
        bgm.loop = true;
        bgm.Play();
    }
	
	// Update is called once per frame
	void Update () {
        // Reset the game
        if (Input.GetButtonDown("Reset"))
        {
            SceneManager.LoadScene("level0");
        }


        if(playerDied >= 3)
        {
            float highest = -1f;
            int player = 0;
            for (int i = 0; i < 4; ++i)
            {
                if (playerHP[i] > highest)
                {
                    highest = playerHP[i];
                    player = i;
                }
            }
            winGame(player);
        }

        for (int i = 0; i < 4; ++i)
        {
            float hp = playerHP[i];
            hp = Mathf.Clamp(hp, 0f, 100f);
            playerBarHP[i].transform.localScale = new Vector3(hp * 0.01f, 1f, 1f);
            if(hp < 20)
            {
                playerBarHP[i].GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f);
            } else if (hp < 50)
            {
                playerBarHP[i].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 0f);
            } else
            {
                playerBarHP[i].GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0f);
            }
        }

    }

    IEnumerator spawnP(int playerNumber)
    {
        yield return new WaitForSeconds(0.2f);
        GameObject r = GameObject.Find("Respawn" + playerNumber);
        Transform newOne = Instantiate(body, r.transform.position, r.transform.rotation) as Transform;
        newOne.GetComponent<PlayerBehaviour>().player = playerNumber;
        newOne.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
    }

    public float getPlayerHP(int player)
    {
        return playerHP[player];
    }

    public void setPlayerHP(int player, float value)
    {
        playerHP[player] = value;
    }

    public void dmgPlayerHP(int player, float value)
    {
        playerHP[player] -= value;
        if (playerHP[player] <= 0f) {
            playerDied++;
        }
    }

    void makeHpBars()
    {
        for (int i=0; i<4; ++i)
        {
            playerBarHP.Add(Instantiate(hpBar, 
                new Vector3(guiC.position.x + 4 + (i * 17), guiC.position.y - 4, guiC.position.z),
                guiC.rotation) as Transform);
        }
    }

    void winGame(int player)
    {
        textWinner.GetComponent<Text>().text = "Player " + (player+1) + " wins!\n\nPress 0 (zero) to restart the game.";
        Time.timeScale = 0f;
    }
}
