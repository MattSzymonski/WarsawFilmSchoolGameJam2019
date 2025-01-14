﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

using MightyGamePack;

public class Sheep : MonoBehaviour
{

    [ReadOnly] public int owner = 0;
    [ReadOnly] public int territory = 0; //0 - neutral, 1 - player1's, 2 - player2's S
    public float colliderDetectRadius = 1;
    public float drownHeightThreshold = -2;
    public bool hideGizmos = false;
    [ReadOnly] public float sheepStrength = 1;
    public float feedSpeed = 0.35f;

    Quaternion startBackOnFoursAngle = Quaternion.identity;
    Quaternion finalBackOnFoursAngle = Quaternion.identity;
    bool backOnFoursLerp;
    bool canPlayThud = false;


    [Header("Sheeps update")]
    [ReadOnly] public float sheepUpdateTimer = 0; //Counting to one (eg.) second, in this time all sheeps will be updated
    public float sheepsUpdateTime = 1.0f; //One second

    [Header("Sheeps back on fours")]
    [ReadOnly] public float sheepBackOnFoursTimer = 0; //Counting to one (eg.) second, in this time all sheeps will be updated
    public float sheepspBackOnFoursTime = 3.0f; //One second


    Rigidbody rb;
    Vector3 previousVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        previousVelocity = rb.velocity;
        canPlayThud = true;
        startBackOnFoursAngle = finalBackOnFoursAngle = GetComponent<Transform>().rotation;
    }


    private void Update()
    {
        if(MightyGamePack.MightyGameManager.gameManager.gameState == GameState.Playing)
        {
            if (territory == 0) //maybe first one is delayed
            {
                previousVelocity = rb.velocity;
                canPlayThud = true;
            }
            else if (rb.velocity != previousVelocity && canPlayThud)
            {
                MightyGamePack.MightyGameManager.gameManager.audioManager.PlayRandomSound("thud1", "thud2", "thud3");
                MightyGamePack.MightyGameManager.gameManager.particleEffectsManager.SpawnParticleEffect(transform.position + new Vector3(0, -0.7f, 0), Quaternion.identity, 4, 0, "SheepGroundHit");
                canPlayThud = false;
            }
            if (sheepUpdateTimer < sheepsUpdateTime) //One second
            {
                sheepUpdateTimer += 1 * Time.deltaTime;
            }
            else
            {
                CheckDrown();
                CheckTerritory();  
                DamageTerritory();
                BackOnFours();
                Feed();
             
                if (Random.Range(0, 100) < 15)
                {
                    GetComponent<TransformJuicer>().StartJuicing();
                }

                sheepUpdateTimer = 0;
            }



            if (sheepBackOnFoursTimer < sheepspBackOnFoursTime) //One second
            {
                sheepBackOnFoursTimer += 1 * Time.deltaTime;
                BackOnFoursLerp();
            }
        } 
    }

    [Button]
    public void BackOnFours()
    {
        if(sheepBackOnFoursTimer > sheepspBackOnFoursTime)
        {
            if (transform.localEulerAngles.x > 40 || transform.localEulerAngles.x < -40 || transform.localEulerAngles.z > 40 || transform.localEulerAngles.z < -40)
            {
                startBackOnFoursAngle = transform.rotation;
                finalBackOnFoursAngle = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f);

                if (rb.velocity.magnitude < 0.1f && rb.angularVelocity.magnitude < 0.1f)
                {
                    sheepBackOnFoursTimer = 0;
                    transform.position += new Vector3(0, 1, 0);
                }
               
            }
        }
       
    }

    void BackOnFoursLerp()
    {
       // transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0f,2.0f,0f), sheepBackOnFoursTimer * 2.5f);
        transform.rotation = Quaternion.Lerp(startBackOnFoursAngle, finalBackOnFoursAngle, sheepBackOnFoursTimer * 1.5f);
    }


    void CheckTerritory()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, colliderDetectRadius + sheepStrength / 3);

        for (int i = 0; i < hitColliders.Length; ++i)
        {
            if(hitColliders[i].transform.tag == "TerritoryPlayer1")
            {
                territory = 1;
                return;
            }
            else if (hitColliders[i].transform.tag == "TerritoryPlayer2")
            {
                territory = 2;
                return;
            }
            else if (hitColliders[i].transform.tag == "TerritoryNeutral")
            {
                territory = 3;
                return;
            }
            else
            {
                territory = 0;
            }
        }
    }

    void CheckDrown()
    {
        if (transform.localPosition.y < drownHeightThreshold)
        {
            Die();
        }
    }

    void Die()
    {
        MightyGamePack.MightyGameManager.gameManager.audioManager.PlayRandomSound("bleat1Die", "bleat2Die", "bleat3Die", "bleat4Die");
        MightyGamePack.MightyGameManager.gameManager.sheepDrownToSpawn++;
        Destroy(gameObject);
    }
   
    void DamageTerritory()
    {
        if (!MightyGamePack.MightyGameManager.gameManager) return;
        if (owner == 1 && territory == 2)
        {
            MightyGamePack.MightyGameManager.gameManager.healthPlayer2 -= Mathf.FloorToInt(sheepStrength);
            return;
        }

        if (owner == 2 && territory == 1)
        {
            MightyGamePack.MightyGameManager.gameManager.healthPlayer1 -= Mathf.FloorToInt(sheepStrength);
            return;
        }
    }

    void Feed()
    {
        if (territory == 3)
        {
            if(sheepStrength < 5)
            {
                sheepStrength += feedSpeed;
            }          
        }
        if (sheepStrength > 1) { transform.localScale = Vector3.one + new Vector3(sheepStrength, sheepStrength, sheepStrength) / 4f; }
    }

    void OnDrawGizmos()
    {
        if(!hideGizmos)
        {
            DebugExtension.DrawCircle(transform.position, Vector3.up, Color.yellow, colliderDetectRadius + sheepStrength / 3);
            DebugExtension.DrawCircle(transform.position, Vector3.right, Color.yellow, colliderDetectRadius + sheepStrength / 3);
            DebugExtension.DrawCircle(transform.position, Vector3.forward, Color.yellow, colliderDetectRadius + sheepStrength / 3);
        }
      
    }
    
}