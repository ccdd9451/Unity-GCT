using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour {

    public Camera[] cameras;
	
	void Update () {
        
        for (int i = 0; i < cameras.Length; i++) {
            if (Input.GetKeyDown(i.ToString())) {
                for (int j = 0; j < cameras.Length; j++) {
                    cameras[j].gameObject.SetActive(false);
                }
                cameras[i].gameObject.SetActive(true);
                break;
            }
        }
	}
}
