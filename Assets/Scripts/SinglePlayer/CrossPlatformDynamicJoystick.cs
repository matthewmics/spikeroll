using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CrossPlatformDynamicJoystick : MonoBehaviour
{
    DynamicJoystick dj;
 
    // Start is called before the first frame update
    void Start()
    {
        dj = GetComponent<DynamicJoystick>();
    }
    public bool TestRun;
    // Update is called once per frame
    void Update()
    {

        if (Application.isEditor && !TestRun)
        {
            return;
        }
        CrossPlatformInputManager.SetAxis("Vertical", dj.Vertical);
        CrossPlatformInputManager.SetAxis("Horizontal", dj.Horizontal);
    }
}
