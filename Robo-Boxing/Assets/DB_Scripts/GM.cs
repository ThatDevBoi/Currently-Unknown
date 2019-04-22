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
    // Start is called before the first frame update
    void Awake()
    {
        Time.timeScale = 0;

        InitiateGame();
    }

    // Update is called once per frame
    void Update()
    {
        roundText.text = "Round:" + currentRound.ToString();
        currentRound = roundIncreaser;

        DB_PC_Controller PCscript = player.GetComponent<DB_PC_Controller>();
        if (PCscript.imDead == true)
            StartCoroutine(GameOver());

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
        Vector3 Refspawnpoint = new Vector3(-3, 0.5f, 0);
        GameObject Ref = Instantiate(referee, Refspawnpoint, Quaternion.identity) as GameObject;
        Ref.name = "Referee";
        // Crowed
        Vector3 CrowedspawnPoint = new Vector3(-9.986048f, 0.7f, 10);
        GameObject crowedThrower = Instantiate(crowed, CrowedspawnPoint, Quaternion.identity) as GameObject;
        crowedThrower.name = "Crowed Thrower";

        // UI
        //Lose Screen Turn off
        GameObject LoseScreen = GameObject.FindGameObjectWithTag("EndScreen");
        LoseScreen.SetActive(false);

        //Main Menu Screen
        GameObject mainMenu = GameObject.FindGameObjectWithTag("MainMenu");
        mainMenu.SetActive(true);
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        GameObject mainMenu = GameObject.FindGameObjectWithTag("MainMenu");
        mainMenu.SetActive(false);
    }

    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(2);
        GameObject LoseScreen = GameObject.FindGameObjectWithTag("EndScreen");
        LoseScreen.SetActive(true);
        Time.timeScale = 0;

        
    }

    public void RestartGame()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void ExitApplication()
    {
        Application.Quit();
        Debug.Log("Game Ended");
    }

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
