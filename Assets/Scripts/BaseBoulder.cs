using UnityEngine;
using System.Collections.Generic;

public class BaseBoulder : MonoBehaviour
{
    // How long this object should stay around in sec.
    public float lifeTime = 10.0f;
    // How long in sec until this is allowed to damage to self if hit.
    public float friendlyTime = 2.0f;
    // All the different trails
    public List<Material> trails;

    public float damage = 5.0f;
    public int playerID = -1;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;
        Color c = this.GetComponent<SpriteRenderer>().color;
        c = new Color(c.r, c.g, c.b, lifeTime * 0.5f);
        this.GetComponent<SpriteRenderer>().color = c;

        if (friendlyTime > 0.0f)
            friendlyTime -= Time.deltaTime;
        if (lifeTime < 0)
        {
            Destroy(this.gameObject);
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<BasePlayer>())
        {
            int playerHit = collision.gameObject.GetComponent<BasePlayer>().player;
            GameControllerScript gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
            if ((friendlyTime < 0.0f || playerHit != playerID) && getMagnitude() > 3.0f)
            {
                if(gc.getPlayerHP(playerHit) > 0f)
                    gc.dmgPlayerHP(playerHit, getDamage());
                Destroy(this.gameObject);
            }
        }
    }

    public void setPlayer(int playerNumber)
    {
        playerID = playerNumber;
        setTrail(playerNumber);
    }

    public int getPlayer()
    {
        return playerID;
    }

    public float getDamage()
    {
        float damageMultiplier = getMagnitude() * 0.3f;
        damageMultiplier = Mathf.Clamp(damageMultiplier, 0.5f, 10.0f);
        return damage * damageMultiplier;
    }

    public float getMagnitude()
    {
        return this.GetComponent<Rigidbody2D>().velocity.magnitude;
    }

    // Can change the trail that shows different player.
    public void setTrail(int playerNumber)
    {
        switch (playerNumber)
        {
            case 0:
                this.GetComponent<TrailRenderer>().material = trails[0];
                break;
            case 1:
                this.GetComponent<TrailRenderer>().material = trails[1];
                break;
            case 2:
                this.GetComponent<TrailRenderer>().material = trails[2];
                break;
            case 3:
                this.GetComponent<TrailRenderer>().material = trails[3];
                break;
        }
    }
}
