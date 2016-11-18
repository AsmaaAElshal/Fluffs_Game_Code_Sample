using UnityEngine;
using System.Collections;
using System;

public class ProjectileCollider : MonoBehaviour
{
	
    /*
     * This script is responsible for detecting the 
	 * user's touch on the Projectilecollider 
    */
		
	
    //Public
    public GameObject Projectile;
    public GameObject Mouth;
    public  int NumberOfProjectiles = 5;
    public static int staticNumberOfProjectiles;
    public TextMesh ProjectileCounterText;
    public float FlickingDistance = 0;
    public int ProjectileSpeed = 0;


    //Private
    GameObject NewProjectile;

    Ray ray;
    RaycastHit hit;
    int projectileFullnumbers;

    bool fluffclicked = false;
	
    Vector3 initpos;
    Vector3 currtpos;
		
    private TouchInfo[] touchInfoArray;
    public GameObject FluffWrapper;

    #region SoundVariables

    GameObject SoundManagereObj;
    SoundManagerClass SoundManScript;

    #endregion

    void Start()
    {

        //Sound MAnager 
        SoundManagereObj = GameObject.FindGameObjectWithTag("SoundManager");
        SoundManScript = (SoundManagerClass)SoundManagereObj.GetComponent("SoundManagerClass");

        // adding the rest of projectiles from previous level .
        NumberOfProjectiles += PlayerPrefs.GetInt((Application.loadedLevel -1).ToString());
		
        projectileFullnumbers = NumberOfProjectiles;
		
        ProjectileCounterText.text = NumberOfProjectiles.ToString() + "/" + projectileFullnumbers.ToString();
        staticNumberOfProjectiles = NumberOfProjectiles;
		
        touchInfoArray = new TouchInfo[5];
    }
	
    // Update is called once per frame
    void Update()
    {
				
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        foreach (Touch t in	Input.touches)
        {
            ray = Camera.main.ScreenPointToRay(new Vector3(t.position.x, t.position.y, 0));
            if (touchInfoArray[t.fingerId] == null)
            {
                touchInfoArray[t.fingerId] = new TouchInfo();
            }
            if (t.phase == TouchPhase.Began)
            {
                if (!PauseMenu.pausebool)
                {
                    if (Physics.Raycast(ray.origin, ray.direction, out hit))
                    {	//Debug.Log("HIT THE FLUFF");
                        if (hit.collider.gameObject.name == "ProjectileCollider")
                        {
                            touchInfoArray[t.fingerId].touchPosition = t.position;
                            touchInfoArray[t.fingerId].timeSwipeStarted = Time.time;
                            fluffclicked = true;	
                            initpos = Mouth.transform.position;	
                            currtpos = initpos;
                        }
                    }
                }
            }
            if (t.phase == TouchPhase.Moved)
            {
                if (!PauseMenu.pausebool)
                {
                    if (fluffclicked)
                    {   // Checking which direction should fire the projectile and change the player direction
                        if (t.position.x < (touchInfoArray[t.fingerId].touchPosition.x))
                        {
                            //Rotate Fluff to the other direction. (left)
                            iTween.RotateTo(FluffWrapper, new Vector3(0, 180, FluffWrapper.transform.position.z), 0);		 
                        }
                        else
                        {
                            // Just keep it as it , or if its already rotated 180 , so it should be rotated to be in default rotation 0,0,0
                            iTween.RotateTo(FluffWrapper, new Vector3(0, 0, FluffWrapper.transform.position.z), 0);
							 
                        }
                        currtpos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                        initpos.z = 0;
                        currtpos.z = 0;
						
                        // Check if the current distance is greater than the flicking distance or not
                        if ((currtpos - initpos).magnitude >= FlickingDistance)
                        {
                            FireProjectile();

                        }
                    }
                }
            }
            if (t.phase == TouchPhase.Ended)
            {
                OnMouseUp();
            }
        }
    }

	
    void OnMouseDown()
    {
        if (!PauseMenu.pausebool)
        {
            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {	
                if (hit.collider.gameObject.name == "ProjectileCollider")
                {
                    fluffclicked = true;	
                    initpos = Mouth.transform.position;	
                    currtpos = initpos;
                }
            }
        }
    }

    void OnMouseDrag()
    {
        if (!PauseMenu.pausebool)
        {
            if (fluffclicked)
            {
                currtpos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                initpos.z = 0;
                currtpos.z = 0;
						
                if ((currtpos - initpos).magnitude >= FlickingDistance)
                {
                    FireProjectile();
                }
					
            }
        }
    }

    void FireProjectile()
    {
        
        PackedSprite FluffSprite = (PackedSprite)(gameObject.transform.parent.GetComponent("PackedSprite"));
        FluffSprite.PlayAnim(8);

        // Create projectile and Update UI
        if (--NumberOfProjectiles >= 0)
        {
            //Update the UI , update the current avialable number of projectiles on HUD
            ProjectileCounterText.text = NumberOfProjectiles.ToString() + "/" + projectileFullnumbers.ToString();
            staticNumberOfProjectiles = NumberOfProjectiles;

            NewProjectile = (GameObject)Instantiate(Projectile, initpos, Quaternion.identity);

            //GameObject.FindGameObjectWithTag("FluffProjCollider").active = false;
            Vector3 dir = currtpos - initpos;
            dir.Normalize();
            NewProjectile.rigidbody.velocity = dir * ProjectileSpeed;
        }
        GameObject.FindGameObjectWithTag("FluffProjCollider").active = false;

        fluffclicked = false;   

        #region ProjectileSound     
        SoundManScript.Scene_Source.clip = SoundManScript.ProjctileClipe;
        SoundManScript.Scene_Source.Play();             
        #endregion
    }

    void OnMouseUp()
    {
    }
}

