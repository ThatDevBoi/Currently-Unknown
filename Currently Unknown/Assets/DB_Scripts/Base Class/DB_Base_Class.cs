#region Unity Engine Using
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion
#region Research and information about class
// Basic flat diagram logic
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
// Helped with referee movement choice //https://answers.unity.com/questions/1179375/placing-an-object-between-2-objects.html
#endregion
#region Base Class
public abstract class DB_Base_Class : MonoBehaviour
{
    #region Player and NPC variables
    #region Physics
    [SerializeField]
    protected SphereCollider rightHand_SC;
    [SerializeField]
    protected SphereCollider leftHand_SC;
    [SerializeField]
    protected CapsuleCollider body_CapsuleCollider;
    [SerializeField]
    protected SphereCollider head_Collider;
    [SerializeField]
    protected BoxCollider body_Collider;
    [SerializeField]
    protected Rigidbody playerRB;
    [SerializeField]
    protected float gravity = 15;
    [SerializeField]
    protected float rbMass = 2;
    #endregion

    #region Movement
    [SerializeField]    // Remove
    protected Animator anim;
    [SerializeField]
    protected float speed = 5;
    [SerializeField]
    protected float maxSpeed = 3;
    // Floats that carry a movement direction so it can be passed through to derived classes
    // This is needed mainly for the animations which will be done in ther derived classes
    protected float Vertical;
    protected float Horizontal;
    [SerializeField]
    protected Vector3 moveDirection = Vector3.zero;
    #endregion

    #region Fighting 
    //[SerializeField]
    //protected float Hitmass = 5;    // Used to devide the force depending on the players stamina

    // This is a value that pushes the players and NPCs back when punches. 
    // However the stamina value will effect the vlaue of force that players and NPCs can apply so it adds
    // Strategic gameplay that allows players to back away
    // Also there will be a system where players can upgarde their stamina
    [SerializeField]
    protected float pushForce = 30;
    [SerializeField]
    protected float fl_knockBackTime;   // Used so players can get spammed with punches
    protected float fl_knockBackCounter;    // 
    // The current amount of stamina the player or NPC has.
    // We use this to place the correct amount of stamina value each time the game is played
    [SerializeField]
    protected int currentStamina;
    [SerializeField]
    protected int coreHealth;
    // This value will be passed onto the currentStamina value
    protected int maxStamina = 200;
    [SerializeField]
    protected int maxCore_Health = 500;
    [SerializeField]
    protected int damage_Stamina = 5;
    [SerializeField]
    protected int increase_Stamina = 5;
    [SerializeField]
    protected float increase_Stamina_timer = 5f;
    #endregion
    #endregion


    #region Camera Movement Class
    [SerializeField]
    public class Camera_Movement : MonoBehaviour
    {
        #region Camera Variables 
        public List<Transform> targets; // List of transforms that the Camera can reference from
        public Vector3 offset;  // Vector3 used to let the camera know where its going to be angled
        public float smoothTime = .5f;  // a float value that lets the position of the camera move slowly acorss to whatever direction it needs to go to

        public float minZoom = 40f;  // the start zoom the camera references from when Objects are far apart
        public float maxZoom = 10f; // Max value the camera can zoom into the Transforms being reference
        public float zoomLimiter = 50f; // The cap limit for the zoom 

        protected Camera cam;   // The camera we are using 

        private Vector3 velocity;   
        #endregion

        #region Camera Functions
        #region LateUpdate Function
        protected virtual void LateUpdate()
        {
            // when the targets havent been applied
            if (targets.Count == 0)
                return; // return so the rest of the code isnt runed
            // Functions that need to be called
            CameraMove();
            Zoom();
        }
        #endregion

        #region Camera Move
        void CameraMove()
        {
            // the center point Vector is the point where the camera needs to be so it can see all references Transforms
            Vector3 centerPoint = GetCenterPoint();
            // the newPosition vector allowing the camera to know where it needs to be next depending on any movement on any reference Transform
            Vector3 newPosition = centerPoint + offset; // We set offset in the IDE so the camera moves to the offset position
            // The position of the camera is monitoring its current position and also the Vector of where it could be next
            transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
        }
        #endregion

        #region Zoom Function
        void Zoom()
        {
            // newZoom goes between the 2 zoom fgloat values and using the GreatestDistance float and deviding it by the cap so we know that the camera wont 
            // go past its limit and takes into consideration of the min and max so it knows when to stop zoming and where to start
            float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
            // We are setting the camera field of view to the newZoom
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
        }
        #endregion

        #region Greatest Distance Functions
        float GetGreatestDistance()
        {
            // We are finding the centre from all the references Transforms
            var bounds = new Bounds(targets[0].position, Vector3.zero);
            // for every target Transform in the List
            for (int i = 0; i < targets.Count; i++)
            {
                // we take that into consideration and have a new Reference to monitor and considering its position
                bounds.Encapsulate(targets[i].position);
            }

            return bounds.size.x;
        }
        #endregion

        #region CenterPoint Function
        protected virtual Vector3 GetCenterPoint()
        {
            // if there is only 1 reference 
            if (targets.Count == 1)
            {
                // we only follow the position of target 0 as lists start from 0 and up
                return targets[0].position;
            }
            // Makes it so that the targets position is the centre
            var bounds = new Bounds(targets[0].position, Vector3.zero);
            // We monitor how many Reference Transforms are in the list
            for (int i = 0; i < targets.Count; i++)
            {
                // Calculate the list Transforms and their positions in the world
                bounds.Encapsulate(targets[i].position);
            }
            // So we can find the centre point of all these Transforms and feed it back
            return bounds.center;
        }
        #endregion
        #endregion
    }
    #endregion

    #region AI Referee Class
    [SerializeField]
    public class Referee : MonoBehaviour
    {
        #region Referee Variables 
        [SerializeField]
        protected Vector3 vec_playerFighter;    // Player fighter GameObject Vector3 which will be monitored via its position in the world
        [SerializeField]
        protected Vector3 vec_NPCFighter;   // NPC fighter GameObject Vector3 reference that will be monitored via its position in the world
        [SerializeField]
        protected Vector3 centerPoint;  // This Vector3 Reference tells the referee GameObject in the scene where to be depending on the PC and NPC position
        // There needs to be a Vector which allows the Referee to move between the 2 fighters when an offender does a illegal move

        // Booleans offenderMissed and offenderCaught can change state of game
        // if offenderCaught eventually = true then the referee GameObject steps between the fighters. 
        private bool offenderMissed = true;     // referee didnt see PC or NPC elbowing
        private bool offenderCaught = false;    // Referee did see the PC/NPC elbowing
        #endregion

        #region Referee Logic void
        protected virtual void RefereeMovement()
        {
            // Side note for next time
            // make boolean for when offender is caught make the referee go between both fighters. This way no one can attack and both fighters go back to their corners


            // Find the 2 Objects that the ref needs to stay between
            centerPoint = (vec_playerFighter + vec_NPCFighter) * 0.5f;
            //transform.position = new Vector3(-3, transform.position.y, transform.position.z);
            transform.position = centerPoint;
            // 0.568 on the y is the value needed to be set so the referee doesnt go through the ground
            // This was tested with a cube so it might change later This depends on if i change scale of object. position of Z value or using a 3D model
            transform.position = new Vector3(-3, 0.568f, transform.position.z);
        }
        #endregion
    }
    #endregion

    public class AI_Crowed : MonoBehaviour
    {
        // Variables 
        public GameObject object_To_Throw;
        public bool canThrowObject;
        public float speed;
        public float gravity;
        public float damageHealth;

        // Logic for the GameObject that will be thrown from the crowed
        public void PhysicsObject()
        {

        }

        // Function called to allow the NPC to throw the physics object
        public void ActivateTheThrow()
        {
            if (DB_NPC_Fighter.illegalElbow == true && DB_RefereeAI.saw_Elbow == false)
                canThrowObject = true;

            if(canThrowObject)
            {
                // Throw the object at the correct NPCs
            }
        }
        // When a Fighter does an illegal move
        // Decide a random object PC, Ref or NPC, Ref and throw an object at them
        // If the object hits stun the fighter or ref
        // Use add force to throw an object at them from where the camera is facing
    }

    // Add this if time is relevant
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

    #region Start Function
    // Start is called before the first frame update
    protected virtual void Start()
    {
        // These 2 regions are for placing a sphere collider on the Player or NPC hand. Which will allow for trigger/collision enters
        // We need both hands with colliders as one attack is a jab with one hand and another attack is with the opposite hand
        #region Right Hand Size
        rightHand_SC = gameObject.transform.FindChild("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand").gameObject.AddComponent<SphereCollider>();// Add BoxCollider on a child object
        rightHand_SC.isTrigger = true;
        rightHand_SC.center = new Vector3(0.09f, 0, 0.01f);    // resize SphereCollider
        rightHand_SC.radius = 0.06f; // Set Sphere collider Radius
        #endregion
        #region Left Hand Size
        leftHand_SC = gameObject.transform.FindChild("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand").gameObject.AddComponent<SphereCollider>();// Add BoxCollider on a child object
        leftHand_SC.isTrigger = true;
        leftHand_SC.center = new Vector3(-0.07f, 0, 0.01f);    // Resize SphereCollider
        leftHand_SC.radius = 0.07f;
        #endregion
        #region Head Sphere Collider SetUp
        head_Collider = gameObject.transform.FindChild("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head").gameObject.AddComponent<SphereCollider>();
        head_Collider.center = new Vector3(0, 0.06f, 0.05f);
        head_Collider.radius = 0.16f;
        #endregion
        #region Body Box Collider SetUp
        body_Collider = gameObject.transform.FindChild("mixamorig:Hips").gameObject.AddComponent<BoxCollider>();
        body_Collider.center = new Vector3(0, 0, 0);
        body_Collider.size = new Vector3(0.48f, 0.28f, 0.37f);
        #endregion
        // This is the set up for the Rigidbody (our physics) which we apply and edit to our needs
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
        // This is the set up for the PC/NPC capsule collider
        #region Capsule Collider Setup
        // Set up the capsule collider which can be used for player and NPC
        // Because the 2 fighters will be the same model just different colours
        body_CapsuleCollider = gameObject.AddComponent<CapsuleCollider>();
        body_CapsuleCollider.isTrigger = false;
        body_CapsuleCollider.center = new Vector3(0, 1, 0);
        // This collider needs to be this small for there are other colliders that are more important. Like the head collider that the NPC or player needs to punch. 
        // Or the body collider that the player can punch
        body_CapsuleCollider.radius = 0.03f;
        body_CapsuleCollider.height = 2f;
        // Set direction to be y 0 = x, 1 = y, 2 = z
        body_CapsuleCollider.direction = 1;
        #endregion
        // This region is for any components we dont apply through script
        // however want to find and store there reference
        #region Finding any IDE components
        anim = GetComponent<Animator>();
        anim.applyRootMotion = false;
        #endregion
        #region Sets values 
        // the current stamina the player has during the game always starts with the max stamina value
        // This is here just in case it gets changed through testing or by acciedent 
        currentStamina = maxStamina;
        // The current core health (Core health being the main health like blood) is used to actually make the fight stop. 
        // the fighter can only be hurt badly if the stamina value is less than 0 showinfg the fighter has no energy so no way to defend themselves
        coreHealth = maxCore_Health;
        #endregion
        #region MUST READ FOR DEBUGGING
        // All these debugging calls need to be left at the bottom. if not this could causes issues. 
        // Say we are checking if an animator component has been found or a value is the correct number
        // if we dont find these IDE components or set these values before the debug code is run
        // Then the debugs declare they havent been found when they are being declared before the Start function can even find or set them
        // this leaves us with the idea that there is an issue when there isnt. the code is just in the wrong order
        #endregion
        // This is used to set if statements to find any bugs that could occure
        // during later development
        // Like not finding a reference or making sure a value is set to thr right amount
        #region Debugging errors
        // if the animator reference in this script or passed onto any derived class is not found
        if (anim == null)
        {
            // Write a message in the console to notify any developer what the issue is
            Debug.LogWarning("There is no animator applies to this GameObject. There needs to be one attached through the IDE. Remeber to find the component in this script");
        }
        // if our current stamina value is less than 200 on start
        if(currentStamina < 200f || currentStamina > 200f)
        {
            // write this message in the console. that the start stamina is not correct. However maybe they want to change the value when testing
            Debug.LogWarning("The current stamina value you've started with is not right. it needs to be 200. However if you need to change the start stamina value, then change the if logic so it meets your needs");
        }
        #endregion
    }
    #endregion

    #region Removed FixedUpdate
    // Removed for it was clashing with the NPC
    //// Update is called once per frame
    //protected virtual void FixedUpdate()
    //{
    //    // Call Functions
    //    Movement();
    //    Melee_Combat();
    //}
    #endregion

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
        Horizontal = Input.GetAxis("Horizontal"); // Carries Z axis
        Vertical = Input.GetAxis("Vertical"); // Carries X axis

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
        // Removed animation from movement function as it was a messy way to have the movement Function
        #endregion
    }
    #endregion

    #region What is this function for
    // This function is used for when NPC and PC are in combat
    // Players can input a key (Space Bar) and deliver a jab (This will be showed different in the NPC_Fighter Dervied Class)
    // However players can use the base class version in the Player Controller
    //The Function is allows players and NPC to push eachother back with force.
    #endregion
    #region Melee Combat Function
    protected virtual void Melee_Combat()
    {
        if (Input.GetButtonDown("Jump"))
        {
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



    #region What Is This Function
    // This function is used in the derived class for the AI referee
    // In this class we will make sure the referee GameObject stays between 2 fighters.
    // We will also be monitoring if any NPC or PC fighters do an illegal attack
    // We check this so the referee can generate a number and decide if the Player/NPC gets punished for their actions
    // We also want a state where a random fan can throw an object at the Referee to stun and stop the Object. 
    // We are going to do this so there is a point in Gameplay where NPC and PC can just attack however they please. 

    // Side note if the referee decides to notice an illegal attack both fighters go back to their corners
    // the offender will also get a strike 3 strikes and you lose (DQ)
    // this effect also allows for a different style of Gameplay. 
    // Players might want to be an caught by the ref although it adds risk for them to be DQ both sides will generate stamina back
    // players could be fighting a hard opponent get to a point and then lose all stamina 
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

    #region Stamina Bar
    protected virtual void Fighter_Stamina()
    {
        if(currentStamina > 0)
        {
            currentStamina -= damage_Stamina;
            return;
        }
        else
        {
            coreHealth -= damage_Stamina;
        }

        #region Monitoring Stamina for less or more force
        // The more stamina the more force pushing Fighters and NPCs back 
        // This will calculate how much power the player pushes the Enemy depending on the current stamina



        #endregion
    }
    #endregion

    #region Increase Stamina
    protected virtual void Regenerate_Stamina()
    {
        // Decrease Timer
        increase_Stamina_timer -= Time.deltaTime;
        // if the timer meets 0 or more
        if(increase_Stamina_timer <= 0)
        {
            // if the stamina value of the fighter is less than 200
            if (currentStamina < 200)
            {
                // reset the timer
                increase_Stamina_timer = 5f;
                // Increase the current stamina value
                currentStamina += increase_Stamina;
            }
            // However if the current stamina is not less than the specific value
            else
            {
                //Reset the timer
                increase_Stamina_timer = 5f;
                    // Return nothing else
                    return;
            }
                
        }
    }
    #endregion
}
#endregion
