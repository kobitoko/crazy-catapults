using UnityEngine;
using System.Collections;

public class HpBarBehaviour : MonoBehaviour {

    public Transform backBar;
    private Transform backBarInst;

	// Use this for initialization
	void Start () {
        backBarInst = Instantiate(backBar) as Transform;
	}
	
	// Update is called once per frame
	void Update () {
        backBarInst.transform.position = this.transform.position;
	}

    public void scaleBackBar(float scale)
    {
        backBarInst.transform.localScale = new Vector3(1f, 1f, 0f) * scale;
    }

}
