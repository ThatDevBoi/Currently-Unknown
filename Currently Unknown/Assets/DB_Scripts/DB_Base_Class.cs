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


public class DB_Base_Class : MonoBehaviour
{
    [SerializeField]
    private BoxCollider PC_RightHand_BC;
    private BoxCollider PC_LightHand_BC;
    // Start is called before the first frame update
    void Start()
    {
        // Add IDE components 
        PC_RightHand_BC = gameObject.transform.FindChild("Arm_right/Hand_right").gameObject.AddComponent<BoxCollider>();
        PC_RightHand_BC.size = new Vector3(1, 1, 0.16f);
        PC_LightHand_BC = gameObject.transform.FindChild("Arm_left/Hand_left").gameObject.AddComponent<BoxCollider>();
        PC_LightHand_BC.size = new Vector3(1, 1, 0.16f);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
