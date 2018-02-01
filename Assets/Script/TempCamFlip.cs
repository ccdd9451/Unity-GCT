using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempCamFlip : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Camera camera = GetComponent<Camera>();
        Matrix4x4 mat = camera.projectionMatrix;
        mat *= Matrix4x4.Scale(new Vector3(-1, 1, 1));
        camera.projectionMatrix = mat;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
