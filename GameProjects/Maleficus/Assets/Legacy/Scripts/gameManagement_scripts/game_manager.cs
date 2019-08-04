using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class game_manager : MonoBehaviour
{

    // enums
    public enum gameMode
    {
        respawn,
        deathmode,
        teamup,
        rapidfire
    };


    //restart Button and blur

    public UnityEngine.UI.Button restartButton;
    public Image restartIm;

    //Player winning Text

    public Text p1Won_text;
    public Text p2Won_text;
    public Text p3Won_text;
    public Text p4Won_text;

    //players script
    public PsController player1;
    public Ps2Controller player2;
    public Ps3Controller player3;
    public Ps4Controller player4;

   
    //Audio 
    public AudioSource inGameSong;
    public AudioSource crowdSoundEffect;
    // menu
    public bool started = false;
    private bool settings = false;
    public bool alreadyfalse = false;
    public int playerCount = 0;
    public gameMode game_Mode = gameMode.respawn;
    public bool p1_ready = false;
    public bool p2_ready = false;
    public bool p3_ready = false;
    public bool p4_ready = false;
    private bool gameStarted = false;

    //mode
    public UnityEngine.UI.Button s_respawn;
    public UnityEngine.UI.Button s_deathmode;
    public UnityEngine.UI.Button s_team_up;
    public UnityEngine.UI.Button s_rapidfire;

    private int lifes_at_start;

    // dropdown
    public Material mat_blue;
    public Material mat_red;
    public Material mat_green;
    public Material mat_yellow;
    public GameObject drop_l_1;
    public GameObject drop_l_2;
    public GameObject drop_l_3;
    public GameObject drop_l_4;
    private int count1 = 0;
    private int count2 = 0;
    private int count3 = 0;
    private int count4 = 0;
    private static Dropdown p1drop;
    private static List<Dropdown.OptionData> p1dropd;
    private static List<Dropdown.OptionData> newdrop1;

    private static Dropdown p2drop;
    private static List<Dropdown.OptionData> p2dropd;
    private static List<Dropdown.OptionData> newdrop2;

    private static Dropdown p3drop;
    private static List<Dropdown.OptionData> p3dropd;
    private static List<Dropdown.OptionData> newdrop3;

    private static Dropdown p4drop;
    private static List<Dropdown.OptionData> p4dropd;
    private static List<Dropdown.OptionData> newdrop4;

    //player ready
    private RawImage ready_blu;
    private RawImage ready_red;
    private RawImage ready_green;
    private RawImage ready_yellow;

    // general

    public GameObject playerManager;
    public GameObject itemManager;

    // Player IN GAME UI

    public Canvas InGameCanvas;
    public Canvas startCanvas;

    // in-Game
    public GameObject pauseUI;
    public bool pause = false;


    void Start()
    {
        //mode
        s_respawn.onClick.AddListener(delegate { set_player_cd(1f, 3f, 4f); });
        s_deathmode.onClick.AddListener(delegate { set_player_cd(1f, 3f, 4f); });
        s_team_up.onClick.AddListener(delegate { set_player_cd(1f, 3f, 4f); });
        s_rapidfire.onClick.AddListener(delegate { set_player_cd(0.2f, 2f, 2.5f); });

        // color dropdowns
        p1drop = GameObject.Find("p1_drop").GetComponent<Dropdown>();
        p1dropd = p1drop.options;
        newdrop1 = new List<Dropdown.OptionData>();

        p2drop = GameObject.Find("p2_drop").GetComponent<Dropdown>();
        p2dropd = p2drop.options;
        newdrop2 = new List<Dropdown.OptionData>();

        p3drop = GameObject.Find("p3_drop").GetComponent<Dropdown>();
        p3dropd = p3drop.options;
        newdrop3 = new List<Dropdown.OptionData>();

        p4drop = GameObject.Find("p4_drop").GetComponent<Dropdown>();
        p4dropd = p4drop.options;
        newdrop4 = new List<Dropdown.OptionData>();

        //ready
        ready_blu = Resources.Load("Assets / Low Poly UI Kit - v.1.1c / UI Kit / Buttons / PNG / Button_12") as RawImage;
        ready_red = Resources.Load("Assets / Low Poly UI Kit - v.1.1c / UI Kit / Buttons / PNG / Button_11_red") as RawImage;
        ready_green = Resources.Load("Assets / Low Poly UI Kit - v.1.1c / UI Kit / Buttons / PNG / Button_14") as RawImage;
        ready_yellow = Resources.Load("Assets / Low Poly UI Kit - v.1.1c / UI Kit / Buttons / PNG / Button_15") as RawImage;

        //Audio
        crowdSoundEffect.enabled = false;
        inGameSong.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            set_pause();
        }

        if (settings && !started)
        {
            // deactivate other color options
            if (p1_ready && count1 == 0)
            {
                PopulateDropdown(1, drop_l_1.GetComponent<Text>().text);
                ++count1;
            }
            if (p2_ready && count2 == 0)
            {
                PopulateDropdown(2, drop_l_2.GetComponent<Text>().text);
                ++count2;
            }
            if (p3_ready && count3 == 0)
            {
                PopulateDropdown(3, drop_l_3.GetComponent<Text>().text);
                ++count3;
            }
            if (p4_ready && count4 == 0)
            {
                PopulateDropdown(4, drop_l_4.GetComponent<Text>().text);
                ++count4;
            }
            switch (playerCount)
            {
                case 1:
                    if (!alreadyfalse)
                    {
                        set_p1_color(drop_l_1.GetComponent<Text>().text);
                        GameObject.Find("p1_drop").GetComponent<Image>().enabled = enabled;
                        drop_l_1.GetComponent<Text>().enabled = enabled;
                        GameObject.Find("p1_settings").GetComponent<RawImage>().enabled = enabled;
                        GameObject.Find("p1_settings").GetComponentInChildren<Text>().enabled = enabled;
                        alreadyfalse = true;
                    }
                    if (p1_ready)
                    {
                        playerManager.GetComponent<PlayerManagerOld>().p1.gameObject.SetActive(true);
                        move_ui_invisible("blur_player_settings");
                        itemManager.SetActive(true);
                        InGameCanvas.gameObject.SetActive(true);
                        InGameCanvas.transform.GetChild(0).gameObject.SetActive(true);
                        change_life_image_visability();
                        started = true;
                        crowdSoundEffect.enabled = true;
                        inGameSong.enabled = true;
                        gameStarted = true;
                    }
                    break;
                case 2:


                    if (!alreadyfalse)
                    {
                        GameObject.Find("p1_drop").GetComponent<Image>().enabled = enabled;
                        drop_l_1.GetComponent<Text>().enabled = enabled;
                        GameObject.Find("p1_settings").GetComponent<RawImage>().enabled = enabled;
                        GameObject.Find("p1_settings").GetComponentInChildren<Text>().enabled = enabled;
                        GameObject.Find("p2_drop").GetComponent<Image>().enabled = enabled;
                        drop_l_2.GetComponent<Text>().enabled = enabled;
                        GameObject.Find("p2_settings").GetComponent<RawImage>().enabled = enabled;
                        GameObject.Find("p2_settings").GetComponentInChildren<Text>().enabled = enabled;
                        alreadyfalse = true;
                    }
                    if (p1_ready && p2_ready)
                    {
                        set_p1_color(drop_l_1.GetComponent<Text>().text);
                        set_p2_color(drop_l_2.GetComponent<Text>().text);
                        playerManager.GetComponent<PlayerManagerOld>().p1.gameObject.SetActive(true);
                        playerManager.GetComponent<PlayerManagerOld>().p2.gameObject.SetActive(true);
                        move_ui_invisible("blur_player_settings");
                        itemManager.SetActive(true);
                        InGameCanvas.gameObject.SetActive(true);
                        InGameCanvas.transform.GetChild(0).gameObject.SetActive(true);
                        InGameCanvas.transform.GetChild(1).gameObject.SetActive(true);
                        change_life_image_visability();
                        started = true;
                        crowdSoundEffect.enabled = true;
                        inGameSong.enabled = true;
                        gameStarted = true;
                    }
                    break;
                case 3:
                    if (!alreadyfalse)
                    {
                        GameObject.Find("p1_drop").GetComponent<Image>().enabled = enabled;
                        drop_l_1.GetComponent<Text>().enabled = enabled;
                        GameObject.Find("p1_settings").GetComponent<RawImage>().enabled = enabled;
                        GameObject.Find("p1_settings").GetComponentInChildren<Text>().enabled = enabled;
                        GameObject.Find("p2_drop").GetComponent<Image>().enabled = enabled;
                        drop_l_2.GetComponent<Text>().enabled = enabled;
                        GameObject.Find("p2_settings").GetComponent<RawImage>().enabled = enabled;
                        GameObject.Find("p2_settings").GetComponentInChildren<Text>().enabled = enabled;
                        GameObject.Find("p3_drop").GetComponent<Image>().enabled = enabled;
                        drop_l_3.GetComponent<Text>().enabled = enabled;
                        GameObject.Find("p3_settings").GetComponent<RawImage>().enabled = enabled;
                        GameObject.Find("p3_settings").GetComponentInChildren<Text>().enabled = enabled;
                        alreadyfalse = true;
                    }
                    if (p1_ready && p2_ready && p3_ready)
                    {
                        set_p1_color(drop_l_1.GetComponent<Text>().text);
                        set_p2_color(drop_l_2.GetComponent<Text>().text);
                        set_p3_color(drop_l_3.GetComponent<Text>().text);
                        playerManager.GetComponent<PlayerManagerOld>().p1.gameObject.SetActive(true);
                        playerManager.GetComponent<PlayerManagerOld>().p2.gameObject.SetActive(true);
                        playerManager.GetComponent<PlayerManagerOld>().p3.gameObject.SetActive(true);
                        move_ui_invisible("blur_player_settings");
                        itemManager.SetActive(true);
                        InGameCanvas.gameObject.SetActive(true);
                        InGameCanvas.transform.GetChild(0).gameObject.SetActive(true);
                        InGameCanvas.transform.GetChild(1).gameObject.SetActive(true);
                        InGameCanvas.transform.GetChild(2).gameObject.SetActive(true);
                        change_life_image_visability();
                        started = true;
                        crowdSoundEffect.enabled = true;
                        inGameSong.enabled = true;
                        gameStarted = true;
                    }
                    break;
                case 4:
                    if (!alreadyfalse)
                    {
                        GameObject.Find("p1_drop").GetComponent<Image>().enabled = enabled;
                        drop_l_1.GetComponent<Text>().enabled = enabled;
                        GameObject.Find("p1_settings").GetComponent<RawImage>().enabled = enabled;
                        GameObject.Find("p1_settings").GetComponentInChildren<Text>().enabled = enabled;
                        GameObject.Find("p2_drop").GetComponent<Image>().enabled = enabled;
                        drop_l_2.GetComponent<Text>().enabled = enabled;
                        GameObject.Find("p2_settings").GetComponent<RawImage>().enabled = enabled;
                        GameObject.Find("p2_settings").GetComponentInChildren<Text>().enabled = enabled;
                        GameObject.Find("p3_drop").GetComponent<Image>().enabled = enabled;
                        drop_l_3.GetComponent<Text>().enabled = enabled;
                        GameObject.Find("p3_settings").GetComponent<RawImage>().enabled = enabled;
                        GameObject.Find("p3_settings").GetComponentInChildren<Text>().enabled = enabled;
                        GameObject.Find("p4_drop").GetComponent<Image>().enabled = enabled;
                        drop_l_4.GetComponent<Text>().enabled = enabled;
                        GameObject.Find("p4_settings").GetComponent<RawImage>().enabled = enabled;
                        GameObject.Find("p4_settings").GetComponentInChildren<Text>().enabled = enabled;
                        alreadyfalse = true;
                    }
                    if (p1_ready && p2_ready && p3_ready && p4_ready)
                    {
                        set_p1_color(drop_l_1.GetComponent<Text>().text);
                        set_p2_color(drop_l_2.GetComponent<Text>().text);
                        set_p3_color(drop_l_3.GetComponent<Text>().text);
                        set_p4_color(drop_l_4.GetComponent<Text>().text);
                        playerManager.GetComponent<PlayerManagerOld>().p1.gameObject.SetActive(true);
                        playerManager.GetComponent<PlayerManagerOld>().p2.gameObject.SetActive(true);
                        playerManager.GetComponent<PlayerManagerOld>().p3.gameObject.SetActive(true);
                        playerManager.GetComponent<PlayerManagerOld>().p4.gameObject.SetActive(true);
                        move_ui_invisible("blur_player_settings");
                        itemManager.SetActive(true);
                        InGameCanvas.gameObject.SetActive(true);
                        InGameCanvas.transform.GetChild(0).gameObject.SetActive(true);
                        InGameCanvas.transform.GetChild(1).gameObject.SetActive(true);
                        InGameCanvas.transform.GetChild(2).gameObject.SetActive(true);
                        InGameCanvas.transform.GetChild(3).gameObject.SetActive(true);
                        change_life_image_visability();
                        started = true;
                        crowdSoundEffect.enabled = true;
                        inGameSong.enabled = true;
                        gameStarted = true;
                    }
                    break;
            }
        }
       CheckForTheWinner();
    }

    // update dropdowns
    void PopulateDropdown(int p, string delete_op)
    {
        if (playerCount > 1)
        {
            switch (p)
            {
                case 1:
                    newdrop2.Clear();
                    newdrop3.Clear();
                    newdrop4.Clear();
                    foreach (Dropdown.OptionData p2dropone in p2dropd)
                    {
                        if (p2dropone.text != delete_op)
                        {
                            newdrop2.Add(p2dropone);
                        }
                    }
                    p2drop.ClearOptions();
                    p2drop.AddOptions(newdrop2);

                    foreach (Dropdown.OptionData p3dropone in p3dropd)
                    {
                        if (p3dropone.text != delete_op)
                        {
                            newdrop3.Add(p3dropone);
                        }
                    }
                    p3drop.ClearOptions();
                    p3drop.AddOptions(newdrop3);

                    foreach (Dropdown.OptionData p4dropone in p4dropd)
                    {
                        if (p4dropone.text != delete_op)
                        {
                            newdrop4.Add(p4dropone);
                        }
                    }
                    p4drop.ClearOptions();
                    p4drop.AddOptions(newdrop4);
                    break;
                case 2:
                    newdrop2.Clear();
                    newdrop3.Clear();
                    newdrop4.Clear();
                    foreach (Dropdown.OptionData p1dropone in p1dropd)
                    {
                        if (p1dropone.text != delete_op)
                        {
                            newdrop1.Add(p1dropone);
                        }
                    }
                    p1drop.ClearOptions();
                    p1drop.AddOptions(newdrop1);

                    foreach (Dropdown.OptionData p3dropone in p3dropd)
                    {
                        if (p3dropone.text != delete_op)
                        {
                            newdrop3.Add(p3dropone);
                        }
                    }
                    p3drop.ClearOptions();
                    p3drop.AddOptions(newdrop3);

                    foreach (Dropdown.OptionData p4dropone in p4dropd)
                    {
                        if (p4dropone.text != delete_op)
                        {
                            newdrop4.Add(p4dropone);
                        }
                    }
                    p4drop.ClearOptions();
                    p4drop.AddOptions(newdrop4);
                    break;
                case 3:
                    newdrop2.Clear();
                    newdrop1.Clear();
                    newdrop4.Clear();
                    foreach (Dropdown.OptionData p2dropone in p2dropd)
                    {
                        if (p2dropone.text != delete_op)
                        {
                            newdrop2.Add(p2dropone);
                        }
                    }
                    p2drop.ClearOptions();
                    p2drop.AddOptions(newdrop2);

                    foreach (Dropdown.OptionData p1dropone in p1dropd)
                    {
                        if (p1dropone.text != delete_op)
                        {
                            newdrop1.Add(p1dropone);
                        }
                    }
                    p1drop.ClearOptions();
                    p1drop.AddOptions(newdrop1);

                    foreach (Dropdown.OptionData p4dropone in p4dropd)
                    {
                        if (p4dropone.text != delete_op)
                        {
                            newdrop4.Add(p4dropone);
                        }
                    }
                    p4drop.ClearOptions();
                    p4drop.AddOptions(newdrop4);
                    break;
                case 4:
                    newdrop2.Clear();
                    newdrop3.Clear();
                    newdrop1.Clear();
                    foreach (Dropdown.OptionData p2dropone in p2dropd)
                    {
                        if (p2dropone.text != delete_op)
                        {
                            newdrop2.Add(p2dropone);
                        }
                    }
                    p2drop.ClearOptions();
                    p2drop.AddOptions(newdrop2);

                    foreach (Dropdown.OptionData p3dropone in p3dropd)
                    {
                        if (p3dropone.text != delete_op)
                        {
                            newdrop3.Add(p3dropone);
                        }
                    }
                    p3drop.ClearOptions();
                    p3drop.AddOptions(newdrop3);

                    foreach (Dropdown.OptionData p1dropone in p1dropd)
                    {
                        if (p1dropone.text != delete_op)
                        {
                            newdrop1.Add(p1dropone);
                        }
                    }
                    p1drop.ClearOptions();
                    p1drop.AddOptions(newdrop1);
                    break;
            }
        }
    }

    // menu getter/setter
    public void CheckForTheWinner()
    {
        if (gameStarted) {
          if (p1_ready && !p2_ready && !p3_ready && !p4_ready)
            {
                if(player1.lifes <= 0)
                {
                    p1Won_text.gameObject.SetActive(true);
                    playerManager.GetComponent<PlayerManagerOld>().p1.gameObject.SetActive(false);
                    restartButton.gameObject.SetActive(true);
                    restartIm.gameObject.SetActive(true);
                }
            }
         else if (p1_ready && p2_ready && !p3_ready && !p4_ready)
            {
                if (player2.lifes <= 0)
                {
                    //player 1 won
                    Debug.Log("player 1 won");
                    p1Won_text.gameObject.SetActive(true);
                    playerManager.GetComponent<PlayerManagerOld>().p1.gameObject.SetActive(false);
                    restartButton.gameObject.SetActive(true);
                    restartIm.gameObject.SetActive(true);

                }
                else if (player1.lifes <= 0)
                {
                    // player 2 won
                    p2Won_text.gameObject.SetActive(true);
                    playerManager.GetComponent<PlayerManagerOld>().p2.gameObject.SetActive(false);
                    restartButton.gameObject.SetActive(true);
                    restartIm.gameObject.SetActive(true);
                }
            }
            else if (p1_ready && p2_ready && p3_ready && !p4_ready)
            {
                if (player2.lifes <= 0 && player3.lifes <= 0)
                {
                    //player 1 won
                    p1Won_text.gameObject.SetActive(true);
                    playerManager.GetComponent<PlayerManagerOld>().p1.gameObject.SetActive(false);
                    restartButton.gameObject.SetActive(true);
                    restartIm.gameObject.SetActive(true);

                }
                else if (player1.lifes <= 0 && player3.lifes <= 0)
                {
                    // player 2 won
                    p2Won_text.gameObject.SetActive(true);
                    playerManager.GetComponent<PlayerManagerOld>().p2.gameObject.SetActive(false);
                    restartButton.gameObject.SetActive(true);
                    restartIm.gameObject.SetActive(true);
                }
                else if (player1.lifes <= 0 && player2.lifes <= 0)
                {
                    //player 3 won 
                    p3Won_text.gameObject.SetActive(true);
                    playerManager.GetComponent<PlayerManagerOld>().p3.gameObject.SetActive(false);
                    restartButton.gameObject.SetActive(true);
                    restartIm.gameObject.SetActive(true);

                }
            }
            else if (p1_ready && p2_ready && p3_ready && p4_ready)
            {
                if (player2.lifes <= 0 && player3.lifes <= 0 && player4.lifes <= 0)
                {
                    //player 1 won
                    p1Won_text.gameObject.SetActive(true);
                    playerManager.GetComponent<PlayerManagerOld>().p1.gameObject.SetActive(false);
                    restartButton.gameObject.SetActive(true);
                    restartIm.gameObject.SetActive(true);
                }
                else if (player1.lifes <= 0 && player3.lifes <= 0 && player4.lifes <= 0)
                {
                    // player 2 won
                    p2Won_text.gameObject.SetActive(true);
                    playerManager.GetComponent<PlayerManagerOld>().p2.gameObject.SetActive(false);
                    restartButton.enabled = true;
                    restartButton.gameObject.SetActive(true);
                    restartIm.gameObject.SetActive(true);
                }
                else if (player1.lifes <= 0 && player2.lifes <= 0 && player4.lifes <= 0)
                {
                    //player 3 won 

                    p3Won_text.gameObject.SetActive(true);
                    playerManager.GetComponent<PlayerManagerOld>().p3.gameObject.SetActive(false);
                    restartButton.enabled = true;
                    restartButton.gameObject.SetActive(true);
                    restartIm.gameObject.SetActive(true);
                }
                else if (player1.lifes <= 0 && player2.lifes <= 0 && player3.lifes <= 0)
                {
                    //player 4 won 
                    p4Won_text.gameObject.SetActive(true);
                    playerManager.GetComponent<PlayerManagerOld>().p4.gameObject.SetActive(false);
                    restartButton.enabled = true;
                    restartButton.gameObject.SetActive(true);
                    restartIm.gameObject.SetActive(true);
                }
            }
        }
    }

    public void set_playerCount(int count)
    {
        playerCount = count;
    }

    public void set_game_Mode(int gm)
    {
        game_Mode = (gameMode)gm;
        settings = true;
    }

    public void set_p1_ready()
    {
        if (!p1_ready)
        {
            p1_ready = true;
        }
        else
        {
            p1_ready = false;
        }

    }

    public void set_p2_ready()
    {
        if (!p2_ready)
        {
            p2_ready = true;
        }
        else
        {
            p2_ready = false;
        }

    }

    public void set_p3_ready()
    {
        if (!p3_ready)
        {
            p3_ready = true;
        }
        else
        {
            p3_ready = false;
        }

    }

    public void set_p4_ready()
    {
        if (!p4_ready)
        {
            p4_ready = true;
        }
        else
        {
            p4_ready = false;
        }

    }

    public void set_p1_color(string cl)
    {
        switch (cl)
        {
            case "Blue":
                playerManager.GetComponent<PlayerManagerOld>().p1.transform.Find("Toon Wizard").GetComponent<Renderer>().material = mat_blue;
                break;
            case "Red":
                playerManager.GetComponent<PlayerManagerOld>().p1.transform.Find("Toon Wizard").GetComponent<Renderer>().material = mat_red;
                break;
            case "Green":
                playerManager.GetComponent<PlayerManagerOld>().p1.transform.Find("Toon Wizard").GetComponent<Renderer>().material = mat_green;
                break;
            case "Yellow":
                playerManager.GetComponent<PlayerManagerOld>().p1.transform.Find("Toon Wizard").GetComponent<Renderer>().material = mat_yellow;
                break;
        }
    }

    public void set_p2_color(string cl)
    {
        switch (cl)
        {
            case "Blue":
                playerManager.GetComponent<PlayerManagerOld>().p2.transform.Find("Toon Wizard").GetComponent<Renderer>().material = mat_blue;
                break;
            case "Red":
                playerManager.GetComponent<PlayerManagerOld>().p2.transform.Find("Toon Wizard").GetComponent<Renderer>().material = mat_red;
                break;
            case "Green":
                playerManager.GetComponent<PlayerManagerOld>().p2.transform.Find("Toon Wizard").GetComponent<Renderer>().material = mat_green;
                break;
            case "Yellow":
                playerManager.GetComponent<PlayerManagerOld>().p2.transform.Find("Toon Wizard").GetComponent<Renderer>().material = mat_yellow;
                break;
        }
    }

    public void set_p3_color(string cl)
    {
        switch (cl)
        {
            case "Blue":
                playerManager.GetComponent<PlayerManagerOld>().p3.transform.Find("Toon Wizard").GetComponent<Renderer>().material = mat_blue;
                break;
            case "Red":
                playerManager.GetComponent<PlayerManagerOld>().p3.transform.Find("Toon Wizard").GetComponent<Renderer>().material = mat_red;
                break;
            case "Green":
                playerManager.GetComponent<PlayerManagerOld>().p3.transform.Find("Toon Wizard").GetComponent<Renderer>().material = mat_green;
                break;
            case "Yellow":
                playerManager.GetComponent<PlayerManagerOld>().p3.transform.Find("Toon Wizard").GetComponent<Renderer>().material = mat_yellow;
                break;
        }
    }

    public void set_p4_color(string cl)
    {
        switch (cl)
        {
            case "Blue":
                playerManager.GetComponent<PlayerManagerOld>().p4.transform.Find("Toon Wizard").GetComponent<Renderer>().material = mat_blue;
                break;
            case "Red":
                playerManager.GetComponent<PlayerManagerOld>().p4.transform.Find("Toon Wizard").GetComponent<Renderer>().material = mat_red;
                break;
            case "Green":
                playerManager.GetComponent<PlayerManagerOld>().p4.transform.Find("Toon Wizard").GetComponent<Renderer>().material = mat_green;
                break;
            case "Yellow":
                playerManager.GetComponent<PlayerManagerOld>().p4.transform.Find("Toon Wizard").GetComponent<Renderer>().material = mat_yellow;
                break;
        }
    }



    //restart game
 /*   public void restart()
    {
        Scene loadedLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(loadedLevel.buildIndex);

    } */

   
    //move ui
    public void move_ui_visible(string name)
    {
        GameObject.Find(name).gameObject.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void move_ui_invisible(string name)
    {
        GameObject.Find(name).gameObject.transform.position = new Vector3(4000, 0, 0);
    }

   public void change_life_image_visability()
    {
        Image[] children = InGameCanvas.GetComponentsInChildren<Image>();
        switch (lifes_at_start)
        {
            case 0:

                foreach (Image child in children)
                {
                    if (child.name == "life_2" || child.name == "life_3" || child.name == "life_4" || child.name == "life_5")
                    {
                        child.SetTransparency(0f);
                    }
                    if (child.name == "life_1")
                    {
                        child.SetTransparency(1f);
                    }
                }
                break;
            case 1:
                foreach (Image child in children)
                {
                    if (child.name == "life_1" || child.name == "life_2")
                    {
                        Color c = child.color;
                        child.SetTransparency(1f);
                    }
                    if (child.name == "life_3" || child.name == "life_4" || child.name == "life_5")
                    {
                        Color c = child.color;
                        child.SetTransparency(0f);
                    }
                }
                break;
            case 2:
                foreach (Image child in children)
                {
                    if (child.name == "life_1" || child.name == "life_2" || child.name == "life_3" )
                    {
                        Color c = child.color;
                        child.SetTransparency(1f);
                    }
                    if (child.name == "life_4" || child.name == "life_5")
                    {
                        Color c = child.color;
                        child.SetTransparency(0f);
                    }

                }
                break;
            case 3:
                foreach (Image child in children)
                {
                    if (child.name == "life_1" || child.name == "life_2" || child.name == "life_3" || child.name == "life_4" )
                    {
                        Color c = child.color;
                        child.SetTransparency(1f);
                    }
                    if (child.name == "life_5")
                    {
                        Color c = child.color;
                        child.SetTransparency(0f);
                    }
                }
                break;
            case 4:
                foreach (Image child in children)
                {
                    if (child.name == "life_1" || child.name == "life_2" || child.name == "life_3"|| child.name == "life_4" || child.name == "life_5")
                    {
                        Color c = child.color;
                        child.SetTransparency(1f);
                    }
                }
                break;
        }
    }
    

    // in-Game getter/setter

    public bool get_pause()
    {
        return pause;
    }

    public void set_pause()
    {
        pause = !pause;
        if (pause)
        {
            Time.timeScale = 0;
            pauseUI.SetActive(true);
            pause = true;
        }
        else
        {
            Time.timeScale = 1;
            pauseUI.SetActive(false);
            pause = false;
        }
    }

    public void set_player_cd(float FCD, float ICD, float TCD)
    {
        playerManager.GetComponent<PlayerManagerOld>().p1.GetComponent<PsController>().fireballCD = FCD;
        playerManager.GetComponent<PlayerManagerOld>().p2.GetComponent<Ps2Controller>().fireballCD = FCD;
        playerManager.GetComponent<PlayerManagerOld>().p3.GetComponent<Ps3Controller>().fireballCD = FCD;
        playerManager.GetComponent<PlayerManagerOld>().p4.GetComponent<Ps4Controller>().fireballCD = FCD;

        playerManager.GetComponent<PlayerManagerOld>().p1.GetComponent<PsController>().iceballCD = ICD;
        playerManager.GetComponent<PlayerManagerOld>().p2.GetComponent<Ps2Controller>().iceballCD = ICD;
        playerManager.GetComponent<PlayerManagerOld>().p3.GetComponent<Ps3Controller>().iceballCD = ICD;
        playerManager.GetComponent<PlayerManagerOld>().p4.GetComponent<Ps4Controller>().iceballCD = ICD;

        playerManager.GetComponent<PlayerManagerOld>().p1.GetComponent<PsController>().teleportationCD = TCD;
        playerManager.GetComponent<PlayerManagerOld>().p2.GetComponent<Ps2Controller>().teleportationCD = TCD;
        playerManager.GetComponent<PlayerManagerOld>().p3.GetComponent<Ps3Controller>().teleportationCD = TCD;
        playerManager.GetComponent<PlayerManagerOld>().p4.GetComponent<Ps4Controller>().teleportationCD = TCD;
    }

    public void set_player_lifes(int lifes)
    {
        playerManager.GetComponent<PlayerManagerOld>().p1.GetComponent<PsController>().lifes = lifes;
        playerManager.GetComponent<PlayerManagerOld>().p2.GetComponent<Ps2Controller>().lifes = lifes;
        playerManager.GetComponent<PlayerManagerOld>().p3.GetComponent<Ps3Controller>().lifes = lifes;
        playerManager.GetComponent<PlayerManagerOld>().p4.GetComponent<Ps4Controller>().lifes = lifes;

        lifes_at_start = lifes;
    }
}
public static class Extensions
{
    public static void SetTransparency(this UnityEngine.UI.Image p_image, float p_transparency)
    {
        if (p_image != null)
        {
            UnityEngine.Color __alpha = p_image.color;
            __alpha.a = p_transparency;
            p_image.color = __alpha;
        }
    }
}