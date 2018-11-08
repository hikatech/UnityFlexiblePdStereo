using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitKey : MonoBehaviour {

    [SerializeField]
    KeyCode m_trigger = KeyCode.Escape;
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyUp(m_trigger)){
            Application.Quit();
        }
	}
}
