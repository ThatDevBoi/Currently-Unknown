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

public class DB_Base_Class : MonoBehaviour
{
    [SerializeField]
    private BoxCollider PC_RightHand_BC;
    [SerializeField]
    private BoxCollider PC_LightHand_BC;
    private CharacterController PC_CC;
    protected float Speed = 5;
    [SerializeField]
    private GameObject PC_Eyes_Camera;
    [SerializeField]
    protected bool canMove = true;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Add IDE components 
        PC_RightHand_BC = gameObject.transform.FindChild("Arm_right/Hand_right").gameObject.AddComponent<BoxCollider>();// Add BoxCollider on a child object
        PC_RightHand_BC.size = new Vector3(1, 1, 0.16f);    // resize BoxCollider
        PC_LightHand_BC = gameObject.transform.FindChild("Arm_left/Hand_left").gameObject.AddComponent<BoxCollider>();// Add BoxCollider on a child object
        PC_LightHand_BC.size = new Vector3(1, 1, 0.16f);    // Resize Boxcollider
        PC_RightHand_BC.isTrigger = false;
        PC_LightHand_BC.isTrigger = false;
        PC_CC = gameObject.AddComponent<CharacterController>();     // Adds a character controller componenet

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        // Call Functions
        Movement();
    }

    protected virtual void Movement()
    {
        // Adding input to the movement Vector
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        PC_CC.Move(move * Time.deltaTime * Speed);
        
        

        // This can be removed when a better and smoother system can be made
        //if (move != Vector3.zero)
        //    transform.forward = move;

        
    }

    protected virtual void Melee_Combat()
    {

    }

    protected virtual void PickUp_Weapon()
    {

    }

    // Use this for melee combat however dont allow the player to see meshs clipping through eachother
    protected virtual void OnTriggerEnter(Collider other)
    {
        
    }
}
