using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Scoreboard : MonoBehaviour {
    ScoreboardEntry[] entries;
    PlayerStats[] players;

    // Start is called before the first frame update
    void Start() {
        entries = GetComponentsInChildren<ScoreboardEntry>();
    }

    public void InitScoreboard() {
        players = GameObject.FindObjectsOfType<PlayerStats>();
        for(int i = 1; i < entries.Length; i++) {
            if(i >= players.Length)
                entries[i].gameObject.SetActive(false);
        }
        StartCoroutine(SortEntriesPeriodically());
    }

    // Update is called once per frame
    void Update() {

    }

    // Sorts players by base health, descending
    // Interval = seconds in between updates
    IEnumerator SortEntriesPeriodically(float interval = 1f) {
        while(true) {
            List<PlayerStats> sortedPlayers = players.OfType<PlayerStats>().ToList();
            sortedPlayers.Sort((a,b) => (a.baseHealth.CompareTo(b.baseHealth)));
            for(int i = 0; i < sortedPlayers.Count; i++) {
                entries[i].UpdateEntry(sortedPlayers[i]);
            }
            yield return new WaitForSeconds(interval);
        }
    }
}