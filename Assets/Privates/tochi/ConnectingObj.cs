using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingObj : MonoBehaviour
{
    public GameObject targetObj;
    private Vector3 _Rotation = new Vector3(0,0,1);

    // Start is called before the first frame update
    void Start()
    {
        _Rotation = targetObj.transform.position;
        Debug.Log("_Rotation: " + _Rotation);
    }

    // Update is called once per frame
    void Update () {
        transform.RotateAround (_Rotation, Vector3.forward, 100*Time.deltaTime);
    }
}
