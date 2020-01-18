using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

public class PlayerController : PlayerControllerBehavior {

    [SerializeField] private float maxSpeed;
    [SerializeField] private float maxAngularSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float angularAccerlation;
    private float speed;
    private float angularSpeed;
    private float translate;
    private float turn;
    private float direction;
    private Vector3 changeVector;

    public NetworkingPlayer owner;
    public int playerNum;

    // Start is called before the first frame update
    protected override void NetworkStart() {
        base.NetworkStart();
        speed = 0;
        angularSpeed = 0;
        // Reset z to 0
        transform.Translate(0, 0, -transform.position.z);
        // networkObject.AssignOwnership(owner);
        // Let player pass through their own base
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Barrier")) {
            if (obj.GetComponent<BarrierBlock>().ownerNum == playerNum) {
                Physics.IgnoreCollision(obj.GetComponent<BoxCollider>(), GetComponent<BoxCollider>());
            }
        }
    }

    void FixedUpdate() {
        if (networkObject.IsOwner) {
            Turning();
            Movement();
        }
        ServerUpdate();
    }

    void ServerUpdate() {
        if (networkObject.IsOwner) {
            networkObject.position = transform.position;
            networkObject.rotation = transform.rotation;
        } else {
            transform.position = networkObject.position;
            transform.rotation = networkObject.rotation;
        }
    }

    void Movement() {
        // Fix so that the player can't change direction the frame let go
        // and go the same speed
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S)) {
            speed = 0;
        } else if (Input.GetKey(KeyCode.W)) {
            speed += Accelerate(speed, maxSpeed, acceleration);
            translate = 1f * speed;
        } else if (Input.GetKey(KeyCode.S)) {
            speed += Accelerate(speed, maxSpeed, acceleration);
            translate = -1f * speed;
        } else {
            translate = 0;
            speed = 0;
        }
        if (translate > 0) { transform.position += transform.up * speed; }
        if (translate < 0) { transform.position -= transform.up * speed; }

    }

    void Turning() {
        // Fix so that the player can't change direction the frame let go
        // and go the same speed
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) {
            angularSpeed = 0;
        } else if (Input.GetKey(KeyCode.A)) {
            angularSpeed += Accelerate(angularSpeed, maxAngularSpeed, angularAccerlation);
            turn = 1f * angularSpeed;
        } else if (Input.GetKey(KeyCode.D)) {
            angularSpeed += Accelerate(angularSpeed, maxAngularSpeed, angularAccerlation);
            turn = -1f * angularSpeed;
        } else {
            turn = 0;
            angularSpeed = 0;
        }
        //changeVector = new Vector3(0, 0 , turn);
        //Debug.Log(turn);
        transform.Rotate(Vector3.forward * turn);
    }

    float Accelerate(float s, float m, float a) {
        if (s < m) {
            return a;
        }
        return 0;
    }

}