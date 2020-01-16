using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {

    // All variables/attributes held by the player such as health, score, or 
    // pixel amount goes here

    public int tankHealth;
    public int baseHealth;

    // Start is called before the first frame update
    void Start() {
        tankHealth = 100;
        baseHealth = 100;
    }

    // Update is called once per frame
    void Update() {

    }

    public int ChangeStat(string statName, int change) {
        if (statName == "tankHealth") {
            tankHealth += change;
            return tankHealth;
        } else if (statName == "baseHealth") {
            baseHealth += change;
            return baseHealth;
        }

        Debug.LogError("Invalid stat name: " + statName);
        return -1;
    }

    public int SetStat(string statName, int set) {
        if (statName == "tankHealth") {
            tankHealth = set;
            return tankHealth;
        } else if (statName == "baseHealth") {
            baseHealth = set;
            return baseHealth;
        }

        Debug.LogError("Invalid stat name: " + statName);
        return -1;
    }

    public int GetPlayerNum() {
        return GetComponent<PlayerController>().playerNum;
    }
}