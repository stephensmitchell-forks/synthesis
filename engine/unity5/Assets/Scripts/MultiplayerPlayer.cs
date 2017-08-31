using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MultiplayerPlayer : NetworkBehaviour {

    private const float Speed = 2.0f;

	// Use this for initialization
	void Start () {
        transform.position = new Vector3(0f, 1f, 0f);
	}
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
            return;

        float xSpeed = 0f;
        float zSpeed = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
            xSpeed = Speed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.RightArrow))
            xSpeed = -Speed * Time.deltaTime;

        if (Input.GetKey(KeyCode.UpArrow))
            zSpeed = -Speed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.DownArrow))
            zSpeed = Speed * Time.deltaTime;

        transform.Translate(new Vector3(xSpeed, 0f, zSpeed));
	}
}
