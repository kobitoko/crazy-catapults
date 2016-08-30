using UnityEngine;
using System.Collections.Generic;

public class CastleBehaviour : MonoBehaviour {

    // The player this belongs to
    public int owner;
    public float maxHp = 1000.0f;
    public Transform healthBar;
    public float hpBarScale = 0.5f;
    public Vector3 offset = new Vector3(-2.2f, -3.5f, 0.0f);
    public List<Material> playerColor;
    public Transform flag;

    private float hp;
    private Transform hpBarInstance;
    private Vector3 hpScale;
    private Transform flagInst;
    private Animator anim;

    // Use this for initialization
    void Start () {
        anim = this.GetComponent<Animator>() as Animator;
        // Setup the health
        hp = maxHp;
        hpBarInstance = Instantiate(healthBar) as Transform;
        // Setup appropriate flag colour
        flagInst = Instantiate(flag, this.transform.position, this.transform.rotation) as Transform;
        flagInst.GetComponent<SpriteRenderer>().color = playerColor[owner].GetColor("_EmissionColor");
        // Create appropriate respawn points
        GameObject respawners = new GameObject();
        respawners.name = "Respawn" + owner;
        respawners.transform.position = this.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        hp = Mathf.Clamp(hp, 0f, maxHp);
        float scaledToOne = hp / maxHp;
        if (hp < maxHp * 0.2f)
        {
            hpBarInstance.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f);
        }
        else if (hp < maxHp * 0.05f)
        {
            hpBarInstance.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 0f);
        }
        else
        {
            hpBarInstance.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0f);
        }
        hpBarInstance.transform.localScale = new Vector3(scaledToOne, 1f, 0f) * hpBarScale;
        hpBarInstance.GetComponent<HpBarBehaviour>().scaleBackBar(hpBarScale);
        hpBarInstance.transform.position = this.transform.position + offset;

        if (hp <= 0f && anim.GetBool("alive") == true)
        {
            anim.SetTrigger("breaks");
            Destroy(flagInst.gameObject);
            this.GetComponent<PolygonCollider2D>().enabled = false;
        }
        anim.SetBool("alive", hp > 0f);

    }
    
    void OnTriggerEnter2D(Collider2D collider) {
        BaseBoulder b = collider.gameObject.GetComponent<BaseBoulder>();
        if (b && b.getPlayer() != owner)
        {
            if(b.getMagnitude() > 7.0f)
            {
                hp -= b.getDamage();
                Destroy(b.gameObject);
            }
        }
    }

}
