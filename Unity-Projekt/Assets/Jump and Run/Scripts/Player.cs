﻿using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public GameObject[] Element;
	public GameManager hatGameManager;

    private Vector3 spawn;

    public float moveSpeed;
	private Vector3 input;
	private float maxSpeed=5f;
    private bool onFloor=false;

	public float jumpHeight;
    public float jumpSpeed;
    public float gravity;
	private int zJumpZahl=0;
    private bool zJump;

    public float climpSpeed;

    public float mudFactor;

    public float trampolineHight;

    private bool zMunition;
    private int zWait;

    private int zElementZahl = 0; //für vorgeschriebene Reihenfolge der Elemente
	public int zElementGroesse;
    


    // Use this for initialization
    void Start () {
		spawn = transform.position;
		//Instantiate(Element[zElementZahl],new Vector2(0,0),Quaternion.Euler (0,0,0));
		zElementZahl++;

        Physics.gravity = new Vector3(0, gravity, 0);
	}
    void Update()
    {
        if (zMunition)
        {
            zWait++;
            if (Input.GetButtonDown("PickUp") &&  zWait > 1)
            {
                zMunition = false;
                zWait = 0;
                GameObject child = transform.GetChild(0).gameObject;
                child.transform.parent = null;

                while (child.transform.position.x > hatGameManager.transform.position.x - zElementGroesse)
                {
                    child.transform.position = new Vector3(child.transform.position.x - 0.001f, child.transform.position.y, 0);
                }
                hatGameManager.slowCamera();

            }
        }
    }
	// Update is called once per frame
	void FixedUpdate () {

        if (zJumpZahl == 0)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector3(Input.GetAxisRaw("Horizontal")*moveSpeed, GetComponent<Rigidbody2D>().velocity.y, 0);
        }
        else if (zJumpZahl != 0 && Input.GetAxisRaw("Horizontal") != 0)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector3(Input.GetAxisRaw("Horizontal") * jumpSpeed, GetComponent<Rigidbody2D>().velocity.y, 0);
        }
		
		if (Input.GetAxisRaw("Jump")==1&&zJump==false) 
		{
			zJump=true;
			jump(); 
		}
		else if(Input.GetAxisRaw("Jump")==0)
		{
			zJump=false;
		}

        

		if (transform.position.y < -20) 
		{  this.die ();	}
	}

	void OnCollisionEnter2D(Collision2D other)	
	{
		if (other.transform.tag == "floor") 
		{
			zJumpZahl=0;
            onFloor = true;
		}

		if (other.transform.tag == "platform"||other.transform.tag=="moving_platform"||other.transform.tag=="disappearing_platform") 
		{
            double unterkanteSpieler = transform.position.y - (transform.localScale.y / 2);
            double oberkantePlatform = other.transform.position.y + other.transform.localScale.y/2;
            if (unterkanteSpieler>=oberkantePlatform) 
			{zJumpZahl = 0;	} 
			//else 
			//{zJumpZahl = 2;	}

            if (other.transform.tag == "moving_platform")
            {
                transform.parent = other.transform;
            }
		}

        if (other.transform.tag == "wall"&&zJumpZahl!=0)
        {
            zJumpZahl = 2;   
        }

		if(other.transform.tag=="enemy"||other.transform.tag=="spikes")
		{
            die();
		}

        if (other.transform.tag == "one-way-trigger")
        {
            if (transform.position.y - (GetComponent<Collider2D>().bounds.size.y / 2) > other.transform.position.y)
            {
                other.collider.isTrigger = false;
                zJumpZahl = 0;
            }
            else
            { other.collider.isTrigger = true; }
        }
    }

    void OnCollisionStay2D(Collision2D other)
    {
       /* if (other.transform.tag == "floor")
        {
            zJumpZahl = 0;
        }*/
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.transform.tag == "moving_platform")
        {
            transform.parent = null;
        }

    }

   
   
    void OnTriggerEnter2D(Collider2D other)
	{
		if (other.transform.tag == "nextElement") 
		{
            //int pNummer = Random.Range(0, Element.Length);
            //Element[pNummer] wird durch Element[zElementZahl] ersetzt (siehe OnTrigger oben)
            //(Element[pNummer],new Vector2(zElementGroesse*zElementZahl,0),Quaternion.Euler (0,0,0));
            //zElementZahl++;
            //Destroy(other.gameObject);
            other.GetComponent<Renderer>().material.color = Color.cyan;
            other.transform.tag = "Untagged";
		}

        if(other.transform.tag=="item")
		{
			hatGameManager.zPunkte++;
			Destroy(other.gameObject);
		}

        if (other.transform.tag == "ladder")
        {
            GetComponent<Rigidbody2D>().gravityScale=0;
            zJumpZahl = 0;
        }

        if (other.transform.tag == "mud")
        {
            moveSpeed /= mudFactor;
            jumpSpeed /= mudFactor;
            jumpHeight /= mudFactor;
            zJumpZahl = 1;
        }

        if (other.transform.tag == "trampoline")
        {
            if (zJumpZahl != 0)
            {
                 GetComponent<Rigidbody2D>().velocity = new Vector3(GetComponent<Rigidbody2D>().velocity.x, trampolineHight, 0);
                 zJumpZahl = 2;
            }
        }

        if (other.transform.tag == "information")
        {
            hatGameManager.setMoveSpeed(0);
        }

        if (other.transform.tag == "coin")
        {
            hatGameManager.zPunkte++;
            Destroy(other.gameObject);
        }

        if (other.transform.tag == "spikes")
        {
            die();
        }

        if (other.transform.tag == "save")
        {
            spawn = transform.position;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.transform.tag == "ladder")
        {
            if (zJumpZahl == 0)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector3(Input.GetAxisRaw("Horizontal") * moveSpeed, Input.GetAxisRaw("Vertical") * climpSpeed, 0);//Grab on to ladder by itself
            }
            else
            { GetComponent<Rigidbody2D>().gravityScale = gravity; }
        }

        if (other.transform.tag == "apple")
        {
            if (Input.GetButtonDown("PickUp")&&zMunition==false)
            {
                other.transform.parent = transform;
                other.transform.position = new Vector3(other.transform.parent.transform.position.x, other.transform.parent.transform.position.y+ 2, 0);
                zMunition = true;
                zWait = 0;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.tag == "ladder")
        {
            GetComponent<Rigidbody2D>().gravityScale=gravity;

        }

        if (other.transform.tag == "mud")
        {
            moveSpeed *= mudFactor;
            jumpSpeed *= mudFactor;
            jumpHeight *= mudFactor;
        }

        if (other.transform.tag == "information")
        {
            hatGameManager.resetMoveSpeed();
            other.transform.tag = "Untagged";
        }

        if (other.transform.tag == "one-way-trigger")
        {
            if (transform.position.y > other.transform.position.y)
            { other.isTrigger = false; }
            else if (transform.position.y < other.transform.position.y)
            { other.isTrigger = true; }
        }
    }

	void jump()
	{
        //Vector3 jump = GetComponent<Rigidbody>().velocity;
        Vector3 jump = new Vector3(Input.GetAxisRaw("Horizontal")*jumpSpeed, 0, 0);
		jump.y = jumpHeight;
		if (zJumpZahl < 1) 
		{
			GetComponent<Rigidbody2D>().velocity=jump;
		} 
		else if (zJumpZahl == 1) 
		{
			GetComponent<Rigidbody2D>().velocity=jump;
		}
		zJumpZahl++;
        onFloor = false;
	}

	void gameOver()
	{
		hatGameManager.gameOver ();
	}

	public void die()
	{  
		transform.position=spawn;
	}
}
