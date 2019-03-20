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
// Fighters move toe to toe    // https://www.youtube.com/watch?v=tcJBMqU_KzA <-- Example of toe to toe 
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public abstract class DB_Base_Class : MonoBehaviour
{
    [SerializeField]
    protected SphereCollider PC_RightHand_SC;
    [SerializeField]
    protected SphereCollider PC_LeftHand_SC;
    [SerializeField]
    protected CapsuleCollider player_Capsule;
    [SerializeField]
    protected Rigidbody playerRB;
    [SerializeField]
    protected float speed = 5;
    [SerializeField]
    protected float maxSpeed = 3;
    [SerializeField]
    protected float gravity = 15;
    [SerializeField]
    protected float RBmass = 2;

    //[SerializeField]
    //protected float Side_step = 3;
    [SerializeField]
    protected Vector3 moveDirection = Vector3.zero;
    [SerializeField]
    protected Vector3 impactDirection = Vector3.zero;
    [SerializeField]    // Remove
    protected bool canMove = true;
    [SerializeField]    // Remove
    private Animator anim;
    [SerializeField]
    protected bool bodyPunch = false;

    // Melee combat Variables
    [SerializeField]
    protected float Hitmass = 5;    // Used to devide the force depending on the players stamina
    //[SerializeField]
    //protected float force = 3;  // Remove
    // This is a value that puses the players and NPCs back when punches. 
    // However the stamina value will effect the vlaue of force that players and NPCs can apply so it adds
    // Strategic gameplay that allows players to back away
    // Also there will be a system where players can upgarde their stamina
    [SerializeField]
    protected float pushForce = 30;
    [SerializeField]
    protected float fl_knockBackTime;   // Used so players can get spammed with punches
    protected float fl_knockBackCounter;    // 

    public int currentStamina;
    public int maxStamina = 200;

    #region Camera Movement Class
    [SerializeField]
    public class Camera_Movement : MonoBehaviour
    {
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
            if (targets.Count == 1)
            {
                return targets[0].position;
            }

            var bounds = new Bounds(targets[0].position, Vector3.zero);
            for (int i = 0; i < targets.Count; i++)
            {
                bounds.Encapsulate(targets[i].position);
            }
            return bounds.center;
        }
        #endregion
        #endregion
    }
    #endregion

    #region Upgrade Character Class
    [SerializeField]
    public class Upgarde_PC_stamina : MonoBehaviour
    {
        #region Upgrade Variable

        #endregion

        #region Shop System

        #endregion

        #region Upgarde Stamina Vlaue

        #endregion

        #region Upgarde Health Value

        #endregion
    }
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
        #region Rigidbody Setup
        playerRB = gameObject.AddComponent<Rigidbody>();     // Adds Rigidbody compoenent into the IDE we need it to apply physics 
        playerRB.useGravity = true;
        playerRB.isKinematic = false;
        // This makes sure on start the Rigidbodys rotation turns off. We want fights to move in straight lines standing toe to toe
        // If we apply rotation then objects wont always be facing eachother
        playerRB.constraints = RigidbodyConstraints.FreezeRotation;
        playerRB.mass = 2f;
        playerRB.drag = 0;
        playerRB.angularDrag = 0.05f;
        #endregion

        player_Capsule = gameObject.AddComponent<CapsuleCollider>();
        player_Capsule.isTrigger = false;
        player_Capsule.center = new Vector3(0, 1, 0);
        player_Capsule.radius = .5f;
        player_Capsule.height = 2f;
        // Set direction to be y 0 = x, 1 = y, 2 = z
        player_Capsule.direction = 1;

        anim = GetComponent<Animator>();

        if(anim == null)
        {
            Debug.LogWarning("There is no animator applies to this GameObject. There needs to be one attached through the IDE. Remeber to find the component in this script");
        }

        
        
        // the current stamina the player has during the game always starts with the max stamina value
        // This is here just in case it gets changed through testing or by acciedent 
        currentStamina = maxStamina;

    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        // Call Functions
        Movement();
        Melee_Combat();
    }

    #region Movement Function
    protected virtual void Movement()
    {
        #region old Movement
        // if the knockBackCounter equals 0 or less than we can move the object again
        // This is used so the object is not constantly attacked when pushing the object back
        //if (fl_knockBackCounter <= 0)
        //{
        //    // Movement Logic
        //    // Movedirection carries the logic of what inout players need to press to move around the scene and in what direction can the object move
        //    // This being left right forward and back. 
        //    moveDirection = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
        //    // Allow the vector to return as 1 so the speed floats are not a greater value. 
        //    // i also found it made the movement system along side animations to run smooth
        //    moveDirection = moveDirection.normalized * speed * Side_step;
        //    // This restricts the model from rising on the +y axis. 
        //    // I did this because i dont need any form of jumping or ascending on the Y axis
        //    moveDirection.y = moveDirection.y + Physics.gravity.y;  // Make sure CC isnt allowed to move current model up

        //    if (moveDirection.magnitude > 0.2f) // Apply the impact force
        //        playerRB.Move(moveDirection * Time.deltaTime); // Allows the player to move forward in the x and z axis

        //    moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, 5 * Time.deltaTime);
        //}
        //else
        //{
        //    // Decrease the time if we arent at 0 or passed it on the knockBackCounter
        //    fl_knockBackTime -= Time.deltaTime;
        //}

        ////// Animations being detected
        ////anim.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Vertical")));      // Value in script of speed is now the animation float value of speed
        ////// Allow for animator parameter to use side step so blend tree knows when to change animation left = -1 | idle = 0 | right = 1
        ////anim.SetFloat("Side Speed", Mathf.Abs(Input.GetAxis("Horizontal")));

        ////// if the A, D, Left Arrow or Right Arrow are pushed down well a Animator Parameter boolean sets to true
        ////if (Input.GetKeyDown(KeyCode.A) | Input.GetKeyDown(KeyCode.D) | Input.GetKeyDown(KeyCode.LeftArrow) | Input.GetKeyDown(KeyCode.RightArrow))
        ////{
        ////    // The gameObjects animator in dervied classes finds boolean parameter and sets it true
        ////    anim.SetBool("SideStrife", true);
        ////}
        ////// However any of the A, D, Left Arrowor Right Arrow are no longer held down
        ////else if(Input.GetKeyUp(KeyCode.A) | Input.GetKeyUp(KeyCode.D) | Input.GetKeyUp(KeyCode.LeftArrow) | Input.GetKeyUp(KeyCode.RightArrow))
        ////{
        ////    // parameter animator boolean is not true and new animation plays depending on what you set next
        ////    anim.SetBool("SideStrife", false);
        ////}
        #endregion


        #region New Movement
        // Take into consideration how the player will be moving around
        float Horizontal = Input.GetAxis("Horizontal"); // Carries Z axis
        float Vertical = Input.GetAxis("Vertical"); // Carries X axis


        // Remove Later
        if(Input.GetKeyDown(KeyCode.E))
        {
            anim.applyRootMotion = true;
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            anim.applyRootMotion = false;
        }

        // Allows the moveDirection to access the inputs the player can press
        moveDirection = (transform.forward * Vertical + (transform.right * Horizontal)) * Time.deltaTime * speed;

        //moveDirection = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal")) * Time.deltaTime;
        moveDirection = moveDirection.normalized * speed;
        moveDirection.y = moveDirection.y + Physics.gravity.y;

        // Move the players physics compoenent Rigidbody with velocity using the Vector3 moveDirection and multiplying by speed
        playerRB.velocity = moveDirection;

        // Limiting the speed 
        if (playerRB.velocity.x > maxSpeed)
            playerRB.velocity = new Vector3(maxSpeed, playerRB.velocity.y, playerRB.velocity.z);

        if (playerRB.velocity.x < -maxSpeed)
            playerRB.velocity = new Vector3(-maxSpeed, playerRB.velocity.y, playerRB.velocity.z);

        if (playerRB.velocity.z > maxSpeed)
            playerRB.velocity = new Vector3(playerRB.velocity.x, playerRB.velocity.y, maxSpeed);

        if (playerRB.velocity.z < -maxSpeed)
            playerRB.velocity = new Vector3(playerRB.velocity.x, playerRB.velocity.y, -maxSpeed);

        // Animations being detected
        anim.SetFloat("Speed", Vertical * speed);      // Value in script of speed is now the animation float value of speed
        // Allow for animator parameter to use side step so blend tree knows when to change animation left = -1 | idle = 0 | right = 1
        anim.SetFloat("Side Speed", Horizontal * speed);

        // if the A, D, Left Arrow or Right Arrow are pushed down well a Animator Parameter boolean sets to true
        if (Input.GetButton("Strafe") | Input.GetKeyDown(KeyCode.LeftArrow) | Input.GetKeyDown(KeyCode.RightArrow))
        {
            // The gameObjects animator in dervied classes finds boolean parameter and sets it true
            anim.SetBool("SideStrife", true);
        }
        // However any of the A, D, Left Arrowor Right Arrow are no longer held down
        else if (Input.GetKeyUp(KeyCode.A) | Input.GetKeyUp(KeyCode.D) | Input.GetKeyUp(KeyCode.LeftArrow) | Input.GetKeyUp(KeyCode.RightArrow))
        {
            // parameter animator boolean is not true and new animation plays depending on what you set next
            anim.SetBool("SideStrife", false);
        }

        #endregion

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
        //if (Input.GetButtonDown("Jump"))
        //{
        //    anim.applyRootMotion = true;
        //}
        //else
        //{
        //    if(Input.GetButtonUp("Jump"))
        //    {
        //        anim.applyRootMotion = false;
        //    }
        //}

        if (Input.GetButtonDown("Jump"))
        {
            currentStamina -= 5;
            anim.SetBool("Jabbing", true);
        }
        else
        {
            if (Input.GetButtonUp("Jump"))
            {
                anim.SetBool("Jabbing", false);
            }
        }

        // Change this button later
        if (Input.GetButtonDown("Fire1"))
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

    protected virtual void KnockBack()
    {
        #region Old code version 1.0
        // Knock Back Logic
        // For however long the knock back timer is we dont want to move
        //fl_knockBackCounter = fl_knockBackTime;
        //moveDirection = direction * fl_knockBackForce;
        #endregion
        #region old code version 1.1
        //impactDirection = moveDirection;
        //impactDirection.Normalize();
        //if (impactDirection.z < 0)
        //    impactDirection.z = impactDirection.z;      // Refect the force down
        //moveDirection += impactDirection.normalized * force / mass;

        //Debug.Log(moveDirection += impactDirection.normalized * force / mass);
        #endregion

        //impactDirection = PC_CC.velocity;
        //moveDirection = impactDirection;
        //impactDirection = new Vector3(transform.position.x, transform.position.y, PC_CC.velocity.z);

       // impactDirection = new Vector3(transform.position.x, 0, transform.forward.z);
    }

    //// This function will be used to monitor how much energy is left in the fighter
    //// PC and NPC figters will have this bar using a UI slider to visually show it.
    //// More stamina you have the more power in your punch
    //// No stamina means you cant move and your punches are pointless 
    //// It leaves more strategic thinking on the players behalf
    //#endregion
    #region Stamina Monitor Function
    protected virtual void StaminaForce()
    {
        // This will calculate how much power the player pushes the Enemy depending on the current stamina 
    }
    #endregion
}
