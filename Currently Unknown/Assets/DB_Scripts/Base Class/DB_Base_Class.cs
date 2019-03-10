using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Things to consider firstly
// Inspired from Urban Champion 
// What will Player Characters AN AI share in common?
// [Movement] <Moving could be in a boxing ring so in circles or moving straight with a street surrounding them>
// [Fighting] <basic attacks would be punches to the head and stomach. However can also use elbow attacks when a multiplyer is full>
// [Defending] <Holding hands up to the face or defending the body>
// [Intensity Muliplyer] <Can be filled by taking damage or combos. More damage can mean further push back> (Inspired by smash)
// [Picking Up Objects] <Objects can be spawned by boxes or thrown into the fighting area have durability and eventully break move to PC or NPC hand>
// [Magical Attacks] <Can use different abilities that involve certain amount of magic> (May be changed if not relevant)
// [Edge Restrictions] <When a Player OR AI are at the right coodinates like on the edge they can be hit down>


// Decision Making Before start of script https://medium.com/ironequal/unity-character-controller-vs-rigidbody-a1e243591483 <--- Rigidbody vs CC
// Using A Character Controller would make moving more smooth
// First Person Camera
// No Rotation Just Moving at an Angle

public abstract class DB_Base_Class : MonoBehaviour
{
    [SerializeField]
    protected SphereCollider PC_RightHand_SC;
    [SerializeField]
    protected SphereCollider PC_LeftHand_SC;
    [SerializeField]
    protected CharacterController PC_CC;
    [SerializeField]
    protected float speed = 5;
    [SerializeField]
    protected float Side_step = 3;
    [SerializeField]
    protected Vector3 moveDirection = Vector3.zero;
    [SerializeField]    // Remove
    protected bool canMove = true;
    [SerializeField]    // Remove
    private Animator anim;
    [SerializeField]
    protected bool bodyPunch = false;

    // Melee combat Variables
    [SerializeField]
    protected float fl_knockBackForce;
    [SerializeField]
    protected float fl_knockBackTime;
    protected float fl_knockBackCounter;

    public int currentStamina;
    public int maxStamina = 200;



    #region Camera Variables 
    public List<Transform> targets;
    public Vector3 offset;
    public float smoothTime = .5f;

    public float minZoom = 40f;
    public float maxZoom = 10f;
    public float zoomLimiter = 50f;

    protected Camera cam;

    private Vector3 velocity;
    #endregion

    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Add IDE Components
        #region Right Hand Size
        PC_RightHand_SC = gameObject.transform.FindChild("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand").gameObject.AddComponent<SphereCollider>();// Add BoxCollider on a child object
        PC_RightHand_SC.center = new Vector3(0.09f, 0, 0.01f);    // resize SphereCollider
        PC_RightHand_SC.radius = 0.06f; // Set Sphere collider Radius
        #endregion
        #region Left Hand Size
        PC_LeftHand_SC = gameObject.transform.FindChild("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand").gameObject.AddComponent<SphereCollider>();// Add BoxCollider on a child object
        PC_LeftHand_SC.center = new Vector3(-0.07f, 0, 0.01f);    // Resize SphereCollider
        PC_LeftHand_SC.radius = 0.07f;
        #endregion
        #region Character Controller
        PC_CC = gameObject.AddComponent<CharacterController>();     // Adds a character controller componenet
        PC_CC.center = new Vector3(0, 1f, 0);
        PC_CC.skinWidth = 0.0325f;
        #endregion

        anim = gameObject.GetComponent<Animator>();

        currentStamina = maxStamina;

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        // Call Functions
        Movement();
        Melee_Combat();

    }

    #region Camera Functions
    #region LateUpdate Function
    protected virtual void LateUpdate()
    {
        if (targets.Count == 0)
            return;

        CameraMove();
        Zoom();
    }
    #endregion

    #region Camera Move
    void CameraMove()
    {
        Vector3 centerPoint = GetCenterPoint();

        Vector3 newPosition = centerPoint + offset;

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }
    #endregion

    #region Zoom Function
    void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
    }
    #endregion

    #region Greatest Distance Functions
    float GetGreatestDistance()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.size.x;
    }
    #endregion

    #region CenterPoint Function
    protected virtual Vector3 GetCenterPoint()
    {
        if(targets.Count == 1)
        {
            return targets[0].position;
        }

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for(int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.center;
    }
    #endregion
    #endregion

    #region Movement Function
    protected virtual void Movement()
    {
        // if the knockBackCounter equals 0 or less than we can move
        if (fl_knockBackCounter <= 0)
        {
            // Movement Logic
            moveDirection = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
            moveDirection = moveDirection.normalized * speed * Side_step;
            moveDirection.y = moveDirection.y + Physics.gravity.y;  // Make sure CC isnt allowed to move current model up
            PC_CC.Move(moveDirection * Time.deltaTime);     // Allows the player to move forward in the x and z axis
        }
        else
        {
            // Decrease the time if we arent at 0 or passed it on the knockBackCounter
            fl_knockBackTime -= Time.deltaTime;
        }

        // Animations being detected
        anim.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Vertical")));      // Value in script of speed is now the animation float value of speed
        // Allow for animator parameter to use side step so blend tree knows when to change animation left = -1 | idle = 0 | right = 1
        anim.SetFloat("Side Speed", Mathf.Abs(Input.GetAxis("Horizontal")));

        // if the A, D, Left Arrow or Right Arrow are pushed down well a Animator Parameter boolean sets to true
        if (Input.GetKeyDown(KeyCode.A) | Input.GetKeyDown(KeyCode.D) | Input.GetKeyDown(KeyCode.LeftArrow) | Input.GetKeyDown(KeyCode.RightArrow))
        {
            // The gameObjects animator in dervied classes finds boolean parameter and sets it true
            anim.SetBool("SideStrife", true);
        }
        // However any of the A, D, Left Arrowor Right Arrow are no longer held down
        else if(Input.GetKeyUp(KeyCode.A) | Input.GetKeyUp(KeyCode.D) | Input.GetKeyUp(KeyCode.LeftArrow) | Input.GetKeyUp(KeyCode.RightArrow))
        {
            // parameter animator boolean is not true and new animation plays depending on what you set next
            anim.SetBool("SideStrife", false);
        }
        
        
    }
    #endregion

    #region Notes
    // This function is used for when NPC and PC are in combat
    // Players can input a key (Space Bar) and deliver a jab (This will be showed different in the NPC_Fighter Dervied Class)
    // However players can use the base class version in the Player Controller
    //The Function is allows players and NPC to push eachother back with force.
    #endregion
    #region Melee Combat Function
    protected virtual void Melee_Combat()
    {
        // List of different attacks via animations
        // Change this button later
        if (Input.GetButtonDown("Jump"))
        {
            currentStamina -= 5;
            anim.SetBool("Jabbing", true);
        }
        else
        {
            if(Input.GetButtonUp("Jump"))
            {
                anim.SetBool("Jabbing", false);
            }
        }
        // Change this button later
        if(Input.GetButtonDown("Fire1"))
        {
            currentStamina -= 10;
            anim.SetBool("Body Punch", true);
        }
        else
        {
            if(Input.GetButtonUp("Fire1"))
            {
                anim.SetBool("Body Punch", false);
            }
        }


    }
    #endregion

    #region notes
    // This function allows NPC or Player to pick up an object to use in a fight
    // However might be taken away and instead use power up pickups
    #endregion
    #region PickUp Weapon
    protected virtual void PickUp_Weapon()
    {

    }
    #endregion

    public void KnockBack(Vector3 direction)
    {
        // Knock Back Logic
        // For however long the knock back timer is we dont want to move
        fl_knockBackCounter = fl_knockBackTime;
        moveDirection = direction * fl_knockBackForce;
    }

    //// This function will be used to monitor how much energy is left in the fighter
    //// PC and NPC figters will have this bar using a UI slider to visually show it.
    //// More stamina you have the more power in your punch
    //// No stamina means you cant move and your punches are pointless 
    //// It leaves more strategic thinking on the players behalf
    //#endregion
    #region Stamina Monitor Function
    protected virtual void StaminaBar()
    {
        // This will calculate how much power the player pushes the Enemy depending on the current stamina 
    }
    #endregion
}
