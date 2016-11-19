using UnityEngine;
using System.Collections;

/*
The Fluff has two weapons :) , Projectile and FireBall .
this fire ball can burn obstacles like Bush(Level10 in Fluff_GDD.pdf the big green bush in the middle)
and it also can heat PopcornFruit(Level 14 and Level 10 - the yellow fruit ) .

*/
public class FireBall : MonoBehaviour
{
    // public
    public GameObject reactionPsys;
    public float delayTimeReactionPsys;
    public GameObject flame;


    //Private
    PackedSprite bushSprite;
    GameObject flameobj;
    float fadeDuration = 1f;
    bool fadeout = false;
    static bool once = true;

    GameObject bushGameObject;
    GameObject SoundManagereObj;
    SoundManagerClass soundManScript;

    void Start()
    {
        SoundManagereObj = GameObject.FindGameObjectWithTag("SoundManager");
        soundManScript = (SoundManagerClass)SoundManagereObj.GetComponent("SoundManagerClass");
        once = true;
    }

    void Update()
    {
        if (fadeout)
        {
            FadeAndDie();
            fadeDuration -= Time.deltaTime;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        // FireBall hit the Bush only once
        if (col.gameObject.CompareTag("Bush") && once)
        {
            once = false;
            bushGameObject = col.gameObject.transform.parent.gameObject;
            // Burn the Bush and create the flame ParticalSystem .
            flameobj = (GameObject)Instantiate(flame, bushGameObject.transform.position, Quaternion.identity);
            fadeout = true;

            gameObject.collider.isTrigger = true;
            renderer.enabled = false;
            gameObject.tag = null;

            bushSprite = (PackedSprite)bushGameObject.GetComponent("PackedSprite");
            Destroy(col.gameObject);
        }
        // if the collider is anything else "PopcornFruit" ,"Bush" ,"ActualPsys" then just fade it out with its reaction.
        else if (col.gameObject.tag != "PopcornFruit" && col.gameObject.tag != "Bush" && col.gameObject.tag != "ActualPsys")
        {
            GameObject ReactionFireball = (GameObject)Instantiate(reactionPsys, transform.position, Quaternion.identity);
            gameObject.collider.isTrigger = true;
            renderer.enabled = false;
            gameObject.tag = null;
            Destroy(ReactionFireball, delayTimeReactionPsys);

        }
        else if (col.gameObject.tag == "BabyFluff")
        {
            soundManScript.Scene_Source.clip = soundManScript.BabyFluffHitClip;
            soundManScript.Scene_Source.Play();
            //Debug.Log("baby");
            Destroy(gameObject);

            if (BabyFluff.PlayDefaultAnimation)
            {
                PackedSprite mypackedsprite = (PackedSprite)(col.transform.parent.gameObject.GetComponent("PackedSprite"));
                mypackedsprite.PlayAnim(4);
            }
        }
    }

    void FadeAndDie()
    {
        if (fadeDuration > 0)
            bushSprite.SetColor(new Color(1, 1, 1, fadeDuration));
        else
        {
            flameobj.transform.FindChild("SparkleParticles").particleEmitter.emit = false;
            flameobj.transform.FindChild("SparkleParticlesSecondary").particleEmitter.emit = false;
            Destroy(bushGameObject);
        }
    }
}
