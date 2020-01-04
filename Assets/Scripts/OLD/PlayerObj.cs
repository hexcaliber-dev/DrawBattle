using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

public class PlayerObj : PlayerObjBehavior {
    public int playerNum;
    public float speed;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if(networkObject.IsOwner) {
            Vector3 translation = Vector3.right * Time.deltaTime * speed;

            if(playerNum == 0)
                transform.position += translation;
            else if(playerNum == 1)
                transform.position -= translation;
            
            networkObject.position = transform.position;
        }

        else {
            transform.position = networkObject.position;
        }
    }
}