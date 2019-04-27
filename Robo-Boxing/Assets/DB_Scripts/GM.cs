using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Gm is gonna hold all UI 
// going to hold all sounds
// Going to hold Game states 
// Going to hold DQ monitors 
public class GM : MonoBehaviour
{
    // Objects to spawn on Awake
    public GameObject player, npc, referee, crowed;
    public Text roundText;
    public int currentRound;
    public int roundIncreaser = 1;
    public GameObject LoseScreen;
    // Start is called before the first frame update
    void Awake()
    {
        Time.timeScale = 0; // Start the game at 0 time so nothing moves 

        InitiateGame(); // Spawn the game
    }

    // Update is called once per frame
    void Update()
    {
        // the round text UI element writes Round: and showing the currentRound int value
        roundText.text = "Round:" + currentRound.ToString();
        currentRound = roundIncreaser;  // the current Round value equals to the Round Increaser value

        player = GameObject.FindGameObjectWithTag("Player");    // Find the player
        DB_PC_Controller PCscript = player.GetComponent<DB_PC_Controller>();    // Find the players Derived Class
        if (PCscript.imDead == true)    // if the PC is dead
        {
            StartCoroutine(GameOver()); // Call Gameover
        }

    }

    // Spawns relevant objects on awake
    private void InitiateGame()
    {
        SpawnNPC();
        // Player 
        Vector3 spawnPoint = new Vector3(0, 0, -3.96f); // Spawn Position
        GameObject PC = Instantiate(player, spawnPoint, Quaternion.identity) as GameObject; // Spawn Object
        PC.name = "Player_Fighter"; // Name of GameObject when spawned
        // Referee
        Vector3 Refspawnpoint = new Vector3(-3, 0.5f, 0);   // Set the spawn point for start
        GameObject Ref = Instantiate(referee, Refspawnpoint, Quaternion.identity) as GameObject;    // Spawn the prefab in the correct place and rotation
        Ref.name = "Referee";   // Name the instance of the prefab Referee
        // Crowed
        Vector3 CrowedspawnPoint = new Vector3(-9.986048f, 0.7f, 10);   // Set up the start position for the Game Object that will be spawned
        GameObject crowedThrower = Instantiate(crowed, CrowedspawnPoint, Quaternion.identity) as GameObject;    // Spawn the prefab instance
        crowedThrower.name = "Crowed Thrower";  // Name the instance of the prefab Crowed Thrower

        // UI
        //Lose Screen Turn off
        LoseScreen.SetActive(false);

        //Main Menu Screen
        GameObject mainMenu = GameObject.FindGameObjectWithTag("MainMenu");
        mainMenu.SetActive(true);
    }
    // Used for the UI Start Button
    public void StartGame()
    {
        // Time goes back to normal so that Objects can move
        Time.timeScale = 1; 
        // Find and turn off the Main Menu
        GameObject mainMenu = GameObject.FindGameObjectWithTag("MainMenu");
        mainMenu.SetActive(false);
    }
    // Used for when player is dead
    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(4);
        LoseScreen.SetActive(true);
        Time.timeScale = 0;
    }
    // Used for restart Button on end screen UI
    public void RestartGame()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
    // Used for all exit game buttons
    public void ExitApplication()
    {
        Application.Quit();
        Debug.Log("Game Ended");
    }
    // NPC spawn this is seperate so we can use it for our knockout Coroutine
    void SpawnNPC()
    {
        // NPC
        Vector3 NPCspawnpoint = new Vector3(0, -4.120264e-17f, 3.79f);
        GameObject NPC = Instantiate(npc, NPCspawnpoint, Quaternion.identity) as GameObject;
        NPC.transform.Rotate(0, -180, 0);
        NPC.name = "NPC_Fighter";
    }

    // Respawns a new NPC
    public IEnumerator KnockedOut()
    {
        // Find the current NPC Fighter in the scene
        GameObject CurrentNPC = GameObject.FindGameObjectWithTag("NPC_Fighter");
        // Make a local variable for the animator attached to the NPC in scene
        // We cant access the base class Anim variable so we need a new one
        Animator anim = CurrentNPC.GetComponent<Animator>();
        // Make the animation play so the NPC is knocked out
        anim.SetBool("KnockedOut", true);
        // Wait some time so the aniimation can play and end
        yield return new WaitForSeconds(4);
        // Make a local variable for the NPC fighter Derived class. 
        DB_NPC_Fighter script = CurrentNPC.GetComponent<DB_NPC_Fighter>();
        // Turn on the Health script so when the fighter spawns the GameObject has all the relevant data to acess for its variables
        script.healthSlider.gameObject.SetActive(true);
        script.staminaSlider.gameObject.SetActive(true);
        // Spawn the New NPC
        SpawnNPC();
        // Increase Round
        roundIncreaser++;
        // Remove the old one
        Destroy(CurrentNPC);
        // End solution
        yield break;    
    }
}
