using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

public class PlayerStats : PlayerStatsBehavior {

    // All variables/attributes held by the player such as health, score, or 
    // pixel amount goes here

    // In networkobject: tankHealth, baseHealth

    // Start is called before the first frame update
    protected override void NetworkStart() {
        base.NetworkStart();
        if (networkObject.IsOwner) {
            networkObject.tankHealth = 100;
            networkObject.baseHealth = 100;
        }
    }

    // Update is called once per frame
    void Update() {

    }

    public int ChangeStat(string statName, int change) {
        if (networkObject.IsOwner) {
            if (statName == "tankHealth") {
                networkObject.tankHealth += change;
                return networkObject.tankHealth;
            } else if (statName == "baseHealth") {
                networkObject.baseHealth += change;
                return networkObject.baseHealth;
            }

            Debug.LogError("Invalid stat name: " + statName);
        }
        return -1;
    }

    public int SetStat(string statName, int set) {
        if (networkObject.IsOwner) {
            if (statName == "tankHealth") {
                networkObject.tankHealth = set;
                return networkObject.tankHealth;
            } else if (statName == "baseHealth") {
                networkObject.baseHealth = set;
                return networkObject.baseHealth;
            }

            Debug.LogError("Invalid stat name: " + statName);
        }
        return -1;
    }

    public int GetPlayerNum() {
        return GetComponent<PlayerController>().networkObject.playerNum;
    }

    public static PlayerStats getPlayerStatsFromNumber(int playerNum) {
        PlayerStats[] stats = GameObject.FindObjectsOfType<PlayerStats>();
        foreach (PlayerStats stat in stats)
            print(stat.GetPlayerNum());
        return GameObject.FindObjectsOfType<PlayerStats>().Where((player) => (player.GetPlayerNum() == playerNum)).First();
    }
}