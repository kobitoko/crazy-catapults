using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerBehaviour : BasePlayer
{
    public float speed = 7.0f;
    public float chargeRate = 30f;
    public Transform wheel;
    public Transform aimer;
    public Transform chargeBar;
    public Transform ammo;
    public List<Material> playerColor;

    private float power;
    private Transform aimObj;
    private Transform chargeObj;
    private float aim;
    private List<List<string>> playerCmd;
    private WheelJoint2D[] wheels;
    private bool isRespawning;
    private Animator anim;

        // Use this for initialization
    void Start()
    {
        // Create wheels
        Transform w1 = Instantiate(wheel,this.transform.position, this.transform.rotation) as Transform;
        Transform w2 = Instantiate(wheel, this.transform.position, this.transform.rotation) as Transform;
        w1.GetComponent<WheelBehaviour>().player = this.player;
        w2.GetComponent<WheelBehaviour>().player = this.player;
        // Attach wheels to body
        wheels = this.GetComponents<WheelJoint2D>();
        wheels[0].connectedBody = w1.GetComponent<Rigidbody2D>();
        wheels[1].connectedBody = w2.GetComponent<Rigidbody2D>();
        // Create aimer
        aimObj = Instantiate(aimer, this.transform.position, this.transform.rotation) as Transform;
        // Set aimObj color gotten from the Material's emission color.
        aimObj.GetComponent<SpriteRenderer>().color = playerColor[player].GetColor("_EmissionColor");
        // Create Charger
        chargeObj = Instantiate(chargeBar, this.transform.position, this.transform.rotation) as Transform;
        chargeObj.GetComponent<ChargeBarBehaviour>().follower = this.transform;
        // Init private vars
        power = 0.0f;
        aim = 0.0f;
        isRespawning = false;
        anim = this.GetComponent<Animator>() as Animator;
        // Init strings of player virtual input multidemensional list
        playerCmd = new List<List<string>>();
        playerCmd.Add(new List<string>() { "P1Horizontal", "P1Vertical", "P1Fire", "P1Respawn"});
        playerCmd.Add(new List<string>() { "P2Horizontal", "P2Vertical", "P2Fire", "P2Respawn"});
        playerCmd.Add(new List<string>() { "P3Horizontal", "P3Vertical", "P3Fire", "P3Respawn"});
        playerCmd.Add(new List<string>() { "P4Horizontal", "P4Vertical", "P4Fire", "P4Respawn"});

    }

    // Update is called once per frame
    void Update()
    {
        // Check if its alive
        GameObject gc = GameObject.FindGameObjectWithTag("GameController");
        GameControllerScript gcs = gc.GetComponent<GameControllerScript>();
        if (gcs.getPlayerHP(player) <= 0.0f)
        {
            died();
        }
        else
        {
            if (!isRespawning)
            {
                move();
                aiming();
                shoot();
            }
            respawn();
        }
    }

    // Make aim controllers that makes more sense? move left to look left etc. 
    // and Aim up and down 180degree only using actually Up and Down respectively?
    // BUT then cannot strafe! Solution for controllers:
    // the right axis stick determines relative aiming angle (it is "locked" in orientation to catapults angle)
    // so when aiming up, right axis is up, and when catapult is upside down, it is then aiming downwards.
    void move()
    {
        float horAxis = 0.0f;
        wheels[0].useMotor = true;
        wheels[1].useMotor = true;
        JointMotor2D m1 = wheels[0].motor;
        JointMotor2D m2 = wheels[1].motor;

        // Make sure Input Manager's axis Gravity = 100 for fast snappy response.
        // Make sure sensitivity = 10 (rate at how fast it goes to 1 or -1)
        if (Input.GetAxis(playerCmd[player][0]) != 0)
        {
            horAxis = Input.GetAxis(playerCmd[player][0]);
            m1.motorSpeed = -horAxis * 10000 * speed * Time.deltaTime;
            m2.motorSpeed = m1.motorSpeed;
        }
        else
        {
            // Deaccelerate it.
            m1.motorSpeed = m1.motorSpeed * 0.97f * Time.deltaTime;
            m2.motorSpeed = m1.motorSpeed;
        }

        // Assign back the modified motor for it to take effect.
        wheels[0].motor = m1;
        wheels[1].motor = m1;
    }

    void aiming()
    {
        // Aiming sighter folows this object and has rotation
        aimObj.position = this.transform.position;

        Vector3 dummy;
        float angBody = 0;
        this.transform.rotation.ToAngleAxis(out angBody, out dummy);

        aim += Input.GetAxis(playerCmd[player][1]);
        aim = ((aim + Input.GetAxis(playerCmd[player][1]) * 3)) % 360;
        aim = (aim < 0 ? 360.0f : aim);

        // flip sprite if aim is one side
        if (aim < 180)
        {
            this.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().flipX = false;
        }

        // Add the aim angle to the rotation.
        aimObj.rotation = this.transform.rotation * Quaternion.AngleAxis(aim, new Vector3(0f, 0f, 1f));
    }

    void shoot()
    {
        // Visualize the power charging bar
        chargeObj.GetComponent<ChargeBarBehaviour>().setBar(power*0.01f);

        if (Input.GetButton(playerCmd[player][2]))
        {
            power += (power <= 100.0f ? chargeRate * Time.deltaTime : 0.0f);
            if(power > 50.0f)
            {
                anim.SetTrigger("shake");
            }
        }
        else if (Input.GetButtonUp(playerCmd[player][2]))
        {
            anim.ResetTrigger("shake");
            anim.SetTrigger("launch");
            // Make sure it spawns at far end of catapult.
            Vector3 throwSide = (aim < 180 ? this.transform.right : -this.transform.right);
            Vector2 spawnPos = this.transform.position + (this.transform.up * 1.0f) + (throwSide * 0.7f);
            // Create it and add the shooting power.
            Transform b = Instantiate(ammo, spawnPos, this.transform.rotation) as Transform;
            // Set player and their respective trail
            b.GetComponent<BaseBoulder>().setPlayer(player);
            b.GetComponent<Rigidbody2D>().velocity = aimObj.up * power;
            // Reset shooting power.
            power = 0.0f;
            //anim.ResetTrigger("launch");
        }
    }

    void respawn()
    {
        if(isRespawning)
        {
            // chage alpha over updates.
            float rate = 0.03f;
            Color w1 = wheels[0].connectedBody.GetComponent<SpriteRenderer>().color;
            Color w2 = wheels[1].connectedBody.GetComponent<SpriteRenderer>().color;
            Color pc = this.gameObject.GetComponent<SpriteRenderer>().color;
            w1 = new Color(w1.r, w1.g, w1.b, w1.a - rate);
            w2 = new Color(w1.r, w1.g, w1.b, w1.a - rate);
            pc = new Color(pc.r, pc.g, pc.b, pc.a - rate);
            wheels[0].connectedBody.GetComponent<SpriteRenderer>().color = w1;
            wheels[1].connectedBody.GetComponent<SpriteRenderer>().color = w2;
            this.gameObject.GetComponent<SpriteRenderer>().color = pc;
        }
        else if(Input.GetButtonDown(playerCmd[player][3]))
        {
            //Debug.Log("Player" + player + " pressed respawn!");
            Destroy(aimObj.gameObject);
            Destroy(chargeObj.gameObject);
            StartCoroutine(actualRespawn());
            isRespawning = true;
        }
    }

    IEnumerator actualRespawn()
    {
        yield return new WaitForSeconds(1.0f);
        GameObject r = GameObject.Find("Respawn" + player);
        Transform newOne = Instantiate(this.transform, r.transform.position, r.transform.rotation) as Transform;
        newOne.GetComponent<PlayerBehaviour>().player = player;
        newOne.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
        Destroy(this.gameObject);
        Destroy(wheels[0].connectedBody.gameObject);
        Destroy(wheels[1].connectedBody.gameObject);
    }

    public float getPower()
    {
        return power;
    }

    void died()
    {
        float rate = 0.03f;
        Color w1 = wheels[0].connectedBody.GetComponent<SpriteRenderer>().color;
        Color w2 = wheels[1].connectedBody.GetComponent<SpriteRenderer>().color;
        Color pc = this.gameObject.GetComponent<SpriteRenderer>().color;
        w1 = new Color(w1.r, w1.g, w1.b, 1f);
        w2 = new Color(w1.r, w1.g, w1.b, 1f);
        pc = new Color(pc.r, pc.g, pc.b, 1f);
        wheels[0].connectedBody.GetComponent<SpriteRenderer>().color = w1;
        wheels[1].connectedBody.GetComponent<SpriteRenderer>().color = w2;
        this.gameObject.GetComponent<SpriteRenderer>().color = pc;
        Destroy(aimObj.gameObject);
        Destroy(chargeObj.gameObject);
        WheelJoint2D[] asa = this.GetComponents<WheelJoint2D>();
        asa[0].enabled = false;
        asa[1].enabled = false;
        this.enabled = false;
    }

}
