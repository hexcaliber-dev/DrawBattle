using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour {
    ScoreboardEntry[] entries;
    PlayerStats[] players;

    public Text myTankHealth, myBaseHealth;
    public Image myTankImg, myBaseImg;
    PlayerStats myPlayer;

    // Start is called before the first frame update
    void Start() {
        entries = GetComponentsInChildren<ScoreboardEntry>();
    }

    public void InitScoreboard(PlayerStats player) {
        players = GameObject.FindObjectsOfType<PlayerStats>();
        // Hide extra scoreboard entries
        print("Scoreboard detected " + players.Length + " players");
        for (int i = 1; i < entries.Length; i++) {
            if (i >= players.Length)
                entries[i].gameObject.SetActive(false);
        }
        myPlayer = player;

        myTankImg.color = ServerInfo.PLAYER_COLOR_PRESETS[myPlayer.GetPlayerNum() - 1];
        myBaseImg.color = ServerInfo.PLAYER_COLOR_PRESETS[myPlayer.GetPlayerNum() - 1];
        StartCoroutine(SortEntriesPeriodically());
    }

    // Update is called once per frame
    void Update() {
        if(myPlayer != null) {
            myTankHealth.text = myPlayer.tankHealth.ToString();
            myBaseHealth.text = myPlayer.baseHealth.ToString();
        }
    }

    // Sorts players by base health, descending
    // Interval = seconds in between updates
    IEnumerator SortEntriesPeriodically(float interval = 0.25f) {
        while (true) {
            List<PlayerStats> sortedPlayers = players.OfType<PlayerStats>().ToList();
            sortedPlayers.Sort((a, b) => (a.baseHealth.CompareTo(b.baseHealth)));
            for (int i = 0; i < sortedPlayers.Count; i++) {
                entries[i].UpdateEntry(sortedPlayers[i]);
            }
            yield return new WaitForSeconds(interval);
        }
    }
}