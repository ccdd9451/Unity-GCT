using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentConfStartUp : MonoBehaviour {

	// Use this for initialization
	void Start () {
        XMLParser.Parse();
        Time.timeScale = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 1;
        }
	}
}
