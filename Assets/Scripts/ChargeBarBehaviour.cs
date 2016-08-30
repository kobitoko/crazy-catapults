using UnityEngine;
using System.Collections;

public class ChargeBarBehaviour : MonoBehaviour {

    public Transform follower;
    public Vector3 offset = new Vector3(-2.0f, -1.5f, 0.0f);
    private float valueP;
	// Use this for initialization
	void Start () {
        valueP = 0.0f;	
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.position = follower.transform.position + offset;
        this.GetComponent<Transform>().localScale = new Vector3(valueP, 1f, 1f);
    }

    public void setBar(float value)
    {
        valueP = value;
    }

}
