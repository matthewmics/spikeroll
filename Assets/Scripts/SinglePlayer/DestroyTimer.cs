using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{

    public float destroyTime = 0.75f;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyThisObject",0.75f);
    }

    void DestroyThisObject()
    {
       // Debug.Log("destroyed");
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
