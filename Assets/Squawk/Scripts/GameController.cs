using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

//single game controller instance for managing other game features
//include the pre-prepared scene "0 GameController" to include this and other important game level objects
//This scene must be scene 0 in the unity build settings
public class GameController : MonoBehaviour
{    
    private Camera gcCamera; //starting camera used for game menu and loading bar
    private MainMenu mainMenu; //reference to the main menu
    [HideInInspector]
    public Chapter currentChapter; //reference to the chapter
    
    private IEnumerator loadingCoroutine;
        
    public Tooltip tooltip; //interactble tooltips and pop up description text box
    public CursorController cursors; //the different game cursors
    public RectTransform crossfader; //for crossfading between locations
    public RectTransform fadeToColour; //for bringing up an overlay to fade out a location or cutscene
    public RectTransform loadingPanel; //panel containing the loading bar
    private LoadingBar loadingBar; //the loading bar    

    //reference to the scene containing the main menu - included in a pre-prepared scene
    private const string mainMenuSceneName = "1 MainMenu"; 
    
    //change this to whatever you call your scene containing your chapter/location and interactable objects
    //currently only support for a single chapter + cutscenes
    public string chapterSceneName = "XmasAdventure";

    //check that there's only a single gamecontroller
    void Awake()
    {                
        if (FindObjectsOfType<GameController>().Length > 1)
        {
            Debug.LogError("Error, multiple GameControllers");
        }
        
    }

    private void Start()
    {
        //initialise important objects 
        gcCamera = gameObject.GetComponent<Camera>();
        gcCamera.depth = 10; //make sure gcCamera is at the front
        gcCamera.enabled = true;
        tooltip.Initialise(this);
        cursors.Initialise();
        crossfader.gameObject.SetActive(false);
        fadeToColour.gameObject.SetActive(false);
        loadingBar = loadingPanel.GetComponentInChildren<LoadingBar>();
        loadingPanel.gameObject.SetActive(false);

        //load menu or chapter
        //for game testing purposes, if the chapter scene is already open, then the menu is closed and the chapter starts immediately
        if ((mainMenu = FindObjectOfType<MainMenu>()) == null)
        {
            loadingCoroutine = LoadMenu();
            StartCoroutine(loadingCoroutine);
        }
        else if (FindObjectOfType<Chapter>() != null)
        {
            //Debug.Log("Already in BeachChapter scene - closing menu and initialising chapter");
            mainMenu.gameObject.SetActive(false);
            StartNewChapter();
        }
    }

    /// <summary>
    /// Loads the main menu scene asynchronously
    /// </summary>    
    IEnumerator LoadMenu()
    {
        //Debug.Log("Loading MainMenu Scene");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mainMenuSceneName, LoadSceneMode.Additive);
        
        while (!asyncLoad.isDone)
        {
            //Debug.Log("Waiting for main menu");
            yield return null;
        }
        mainMenu = FindObjectOfType<MainMenu>();
        mainMenu.Initialise(this);

        //if a chapter is already loaded, play it - used to simplify testing game
        if (FindObjectOfType<Chapter>() != null)
        {
            //Debug.Log("Chapter scene open - closing menu and initialising chapter");
            mainMenu.gameObject.SetActive(false);
            StartNewChapter();
        }
    }

    /// <summary>
    /// Opens the scene containing the chapter referened by chapterSceneName
    /// </summary>
    public void StartNewChapter()
    {                
        //load chapter
        if (FindObjectOfType<Chapter>() == null)
        {
            //Debug.Log("Loading BeachChapter");
            loadingCoroutine = AsyncLoadChapter(chapterSceneName);
            StartCoroutine(loadingCoroutine);
        }
        else
        {
            //Debug.Log("Already in chapter - initialising");
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(chapterSceneName));
            InitChapter();
        }
    }

    /// <summary>
    /// Loads the chapter scene asynchronously
    /// </summary>    
    private IEnumerator AsyncLoadChapter(string chapterName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(chapterName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            //Debug.Log("Waiting for chapter to load");
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(chapterName));
        //Debug.Log(chapterName + " loaded - initialising");
        InitChapter();
    }

    /// <summary>
    /// Initialises the chapter scene
    /// </summary>
    private void InitChapter()
    {        
        currentChapter = FindObjectOfType<Chapter>();

        loadingPanel.gameObject.SetActive(true);
        loadingBar.UpdateProgress(0f);
        currentChapter.InitialiseChapter(this);        
    }

    /// <summary>
    /// updates the loading panel as the chapter initialises
    /// </summary>    
    public void UpdateLoadingPanel(float progress)
    {        
        loadingBar.UpdateProgress(progress);
    }

    /// <summary>
    /// When the chapter is initialised, turn off the main menu, loading bar and GC camera
    /// </summary>
    public void ChapterInitComplete()
    {
        //deactivate main menu
        mainMenu.gameObject.SetActive(false);

        loadingPanel.gameObject.SetActive(false); //turn off loading bar panel
        
        gcCamera.enabled = false; //turn off the gamecontroller camera        
    }     

}
