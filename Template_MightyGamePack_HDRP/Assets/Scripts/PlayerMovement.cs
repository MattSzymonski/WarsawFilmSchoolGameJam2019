using MightyGamePack;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    MightyGameManager gameManager;

    Rigidbody rb;
    ParticleSystem particles;
    Transform shoutArea;
    TransformJuicer effectJuicer;

    //-------Movement--------//
    [Header("Settings")]
    public int playerNumber;
    public float shoutStrength;
    public float directionAngle;
  
    public float halfExtentsOfShoutBox = 1.0f;
    [Range(0, .3f)]
    public float movementSmoothing = 0.05f;



   

    Animator anim;
    public float drownHeightThreshold = -2;



    Vector3 lookDirection;
    Vector3 movementDirection;

    public float movementSpeed = 5;
    float shoutTimer;
    public float shoutTime = 2;
    public bool readyToShout = false;

    public Renderer stoneRenderer;
    public TransformJuicer flaotingJuicer;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        particles = GetComponentInChildren<ParticleSystem>();
        effectJuicer = GetComponent<TransformJuicer>();
        shoutArea = GetComponentsInChildren<Transform>().Where(col => col.tag == "ShoutArea").SingleOrDefault();
        gameManager = GameObject.Find("GameManager").GetComponent<MightyGameManager>();
        anim = GetComponentInChildren<Animator>();
        Vector3 shoutPosition = shoutArea.transform.position;
        shoutPosition.z += 2.5f; //for now just twice wide as player
        shoutArea.transform.position = shoutPosition;
        flaotingJuicer.StartJuicing();
    }

    void Update()
    {
        if (gameManager.gameState == GameState.Playing)
        {
            Move();
            Rotation();
            Shout();
            CheckDrown();

            if(playerNumber == 1)
            {
                stoneRenderer.sharedMaterial.SetColor("_EmissiveColor", new Color(1f, 0.2f, 0.2f, 1) * shoutTimer * 60);
            }
            if (playerNumber == 2)
            {
                stoneRenderer.sharedMaterial.SetColor("_EmissiveColor", new Color(0.2f, 0.2f, 1f, 1) * shoutTimer * 60);
            }
        }
    }

    void CheckDrown()
    {
        if (transform.position.y < drownHeightThreshold)
        {
            if (playerNumber == 1)
            {
                MightyGameManager.gameManager.GameOver(2);
            }
            if (playerNumber == 2)
            {
                MightyGameManager.gameManager.GameOver(1);
            }
        }
    }



    void Move() //Interpreting player controllers input
    {

        if (playerNumber == 1)
        {
            movementDirection = new Vector3(Input.GetAxis("Controller1 Left Stick Horizontal"), 0, -Input.GetAxis("Controller1 Left Stick Vertical")) * movementSpeed;
            DebugExtension.DebugArrow(transform.position, movementDirection);
            float yVel = rb.velocity.y;
            rb.velocity = new Vector3(movementDirection.x, yVel, movementDirection.z);
        }
        if (playerNumber == 2)
        {
            movementDirection = new Vector3(Input.GetAxis("Controller2 Left Stick Horizontal"), 0, -Input.GetAxis("Controller2 Left Stick Vertical")) * movementSpeed;
            DebugExtension.DebugArrow(transform.position, movementDirection);
            float yVel = rb.velocity.y;
            rb.velocity = new Vector3(movementDirection.x, yVel, movementDirection.z);
        }
    }
   


    void Rotation() // Calculating angle between player joystick right stick declension
    {
        if (playerNumber == 1)
        {
            lookDirection = new Vector3(Input.GetAxis("Controller1 Right Stick Horizontal"), 0, -Input.GetAxis("Controller1 Right Stick Vertical"));
            DebugExtension.DebugArrow(transform.position, lookDirection * 10, Color.yellow);
            transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        }
        if (playerNumber == 2)
        {
            lookDirection = new Vector3(Input.GetAxis("Controller2 Right Stick Horizontal"), 0, -Input.GetAxis("Controller2 Right Stick Vertical"));
            DebugExtension.DebugArrow(transform.position, lookDirection * 10, Color.yellow);
            transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);

        }
    }

    public void Shout()
    {
        if (playerNumber == 1)
        {
            if (Input.GetAxis("Controller1 Triggers") != 0)
            {
                if (shoutTimer < shoutTime)
                {
                    shoutTimer += 1 * Time.deltaTime;
                }
                else
                {
                    if (!readyToShout)
                    {
                        effectJuicer.StartJuicing();
                    }
                    readyToShout = true;
                }
            }

            if (Input.GetAxis("Controller1 Triggers") == 0)
            {
                if (readyToShout)
                {
                    ShoutImpl();
                    readyToShout = false;
                }
                shoutTimer = 0;
            }
        }

        if (playerNumber == 2)
        {
            if (Input.GetAxis("Controller2 Triggers") != 0)
            {
                if (shoutTimer < shoutTime)
                {
                    shoutTimer += 1 * Time.deltaTime;
                }
                else
                {
                    if (!readyToShout)
                    {
                        effectJuicer.StartJuicing();
                    }
                    readyToShout = true;
                }
            }

            if (Input.GetAxis("Controller2 Triggers") == 0)
            {
                if (readyToShout)
                {
                    ShoutImpl();
                    readyToShout = false;
                }
                shoutTimer = 0;
            }
        }
    }
 
   private void ShoutImpl()
    {
        //for now just react for every object in collision regardless of distance from source
        //TODO: add cooldown
        // add reduction of power further from me
        // ignore objects not in cone -> add additional box for objects close by
        // add visual cue that it is ready
        Vector3 boxBounds = transform.localScale * 4.0f;
        foreach (var obj in Physics.OverlapBox(shoutArea.transform.position, boxBounds / 2.0f, transform.rotation)) //slightly bigger than current gizmo, when tweaking remember to tweak corresponding gizmo
        {     
            if (obj.tag != "Sheep") continue; // for now ignore shouting at other player
            //Vector3 direction = transform.rotation * Vector3.forward;
            //direction.y = directionAngle;

            float distanceX = Mathf.Abs(transform.position.x - obj.transform.position.x);
            float distanceZ = Mathf.Abs(transform.position.z - obj.transform.position.z);
            float distanceRoot = Mathf.Sqrt(distanceX * distanceX + distanceZ * distanceZ);
            float shoutDecrease = Mathf.Pow((distanceRoot + 1), -2.0f) * 10.0f;

            obj.GetComponent<Rigidbody>().AddForce(lookDirection * shoutStrength
                * shoutDecrease, ForceMode.Impulse);
            obj.GetComponent<Rigidbody>().AddTorque(GenerateRandomRotation() * 0.3f, ForceMode.Impulse);

            Debug.Log("FUS RO DAH! on: " + obj.name);
        }
        particles.Play();
        //add visual cue
    }

    private Vector3 GenerateRandomRotation()
    {
        int rotateX = Random.Range(0, 2);
        int rotateY = Random.Range(0, 2);
        int rotateZ = Random.Range(0, 2);
        return new Vector3(rotateX, rotateY, rotateZ);
    }

    private void OnDrawGizmos()
    {
        if (!shoutArea) return;
        Gizmos.matrix = Matrix4x4.TRS(shoutArea.transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, transform.localScale * 4);
    }
}
