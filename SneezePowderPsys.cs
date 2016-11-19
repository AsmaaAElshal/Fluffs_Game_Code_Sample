using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
This script is applied on Sneeze Powder ParticalSystem
that it means if this sneeze powder fall down on catapult plant , this plant will Sneeze :) :D
and while sneezing it will throw leaf , this leaf will cut specifice kind of vine to open the road between Fluff and his baby. :)
*/
public class SneezePowderPsys : MonoBehaviour
{
    //Public
    public GameObject catapultTreeProjectile;
    public int forceInX = 1000;
    public int forceInY = 300;

    //Private
    Transform catapultPlantTransform;
    private GameObject newCatapultTreeProjectile;
    private PackedSprite plantPackedSprite;
    GameObject soundManagereObj;
    SoundManagerClass soundMangScript;
    string[] collidersNames = { "TopCollider", "Bodycollider", "TopColliderSleeping", "BodyColliderSleeping" };

    void Start()
    {
        soundManagereObj = GameObject.FindGameObjectWithTag("SoundManager");
        soundMangScript = (SoundManagerClass)soundManagereObj.GetComponent("SoundManagerClass");
    }

    void OnParticleCollision(GameObject col)
    {
        //Debug.Log(col.tag);
        if (col.CompareTag("CatapultPlant"))
        {
            // Gettign Packed Sprite of CatapultPlant game object
            plantPackedSprite = (PackedSprite)(col.transform.parent.GetComponent("PackedSprite"));
            catapultPlantTransform = col.transform.parent.gameObject.transform;

            if (!plantPackedSprite.IsAnimating())
            {
                soundMangScript.Scene_Source.clip = soundMangScript.CatapultPlantSneezeClip;
                soundMangScript.Scene_Source.Play();
                DeactiveColliders();
                StartCoroutine(playShootingAnimation(plantPackedSprite.GetAnim("Sleeping").GetLength()));
            }
        }
    }


    IEnumerator playShootingAnimation(float time)
    {
        yield return new WaitForSeconds(time);
        plantPackedSprite.PlayAnim("beforeShooting");
        StartCoroutine(playRestAnimation(plantPackedSprite.GetAnim("beforeShooting").GetLength()));
    }

    IEnumerator playRestAnimation(float time)
    {
        yield return new WaitForSeconds(time);

        Vector3 initPos;
        Vector3 catapultPlantPosition = catapultPlantTransform.position;
        int catapultPlantRotation_y = (int)catapultPlantTransform.rotation.eulerAngles.y;

        if (catapultPlantRotation_y == 180)
        {
            initPos = new Vector3(catapultPlantPosition.x + 5.5f, catapultPlantPosition.y - 3f, 0);
            newCatapultTreeProjectile = (GameObject)Instantiate(catapultTreeProjectile, initPos, Quaternion.identity);
            newCatapultTreeProjectile.rigidbody.AddForce(new Vector3(-forceInX, forceInY, 0));
        }
        else
        {
            initPos = new Vector3(catapultPlantPosition.x - 3.5f, catapultPlantPosition.y - 3f, 0);
            newCatapultTreeProjectile = (GameObject)Instantiate(catapultTreeProjectile, initPos, Quaternion.identity);
            newCatapultTreeProjectile.rigidbody.AddForce(new Vector3(forceInX, forceInY, 0));
        }

        newCatapultTreeProjectile.transform.rotation = catapultPlantTransform.rotation;

        plantPackedSprite.PlayAnim("restOfShooting");
        StartCoroutine(ActivateColliders(plantPackedSprite.GetAnim("restOfShooting").GetLength()));

    }

    void DeactiveColliders()
    {

        foreach (string colliderName in collidersNames)
        {
            catapultPlantTransform.FindChild(colliderName).gameObject.SetActiveRecursively(false);

        }
    }

    IEnumerator ActivateColliders(float time)
    {
        //Debug.Log("Activate Colliders");
        yield return new WaitForSeconds(time * 3);
        catapultPlantTransform.FindChild("TopCollider").gameObject.SetActiveRecursively(true);
        catapultPlantTransform.FindChild("Bodycollider").gameObject.SetActiveRecursively(true);

    }

}
