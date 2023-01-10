using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//A single controller for cutscenes in a chapter. Add as a component to a root object in your chapter scene
//Add one or more child objects having cutsceneReference components on them to act as references to the different cutscenes
//Cutscenes are separate Unity scenes that are loaded and played as required.
public class CutsceneController : MonoBehaviour
{    
    private Scene chapterScene; //what's the name of the unity scene the chapter is in, so can return to it after finishing the cutscene
    [HideInInspector]
    private List<CutsceneReference> cutscenes = new List<CutsceneReference>(); //list of cutscene references
    [HideInInspector]
    public Chapter chapter;
    private IEnumerator loadCutscene;
    private IEnumerator unloadCutscene;

    private void Start()
    {
        //populate the list
        GetComponentsInChildren<CutsceneReference>(true, cutscenes);

        //make sure the scenes aren't loaded at the start of the game to avoid interfering with the game itself
        foreach (CutsceneReference cutscene in cutscenes)
        {
            string cutsceneName = cutscene.cutsceneName;
            if (SceneManager.GetSceneByName(cutsceneName).isLoaded)
            {
                unloadCutscene = AsyncUnloadCutscene(cutsceneName);
                StartCoroutine(unloadCutscene);
            }
        }
    }

    /// <summary>
    /// simple initialisation method
    /// </summary>
    public void Initialise(Chapter _chapter)
    {
        chapter = _chapter;
        chapterScene = gameObject.scene;
    }

    /// <summary>
    /// loads a cutscene and immediately plays it
    /// </summary>    
    public void LoadAndPlayCutscene(string cutsceneName)
    {
        if (cutscenes.Exists(x => x.cutsceneName == cutsceneName))
        {
            loadCutscene = AsyncLoadAndPlayCutscene(cutsceneName);
            StartCoroutine(loadCutscene);
        }
        else
        {
            Debug.LogError("No such cutscene as:" + cutsceneName);
        }
    }

    /// <summary>
    /// ienumerator for loading and immediately playing a cutscene
    /// </summary>    
    private IEnumerator AsyncLoadAndPlayCutscene(string cutsceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(cutsceneName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {            
            yield return null;
        }

        PlayCutscene(cutsceneName);
    }

    /// <summary>
    /// loads a cutscene in the background. This is the usual method 
    /// </summary>
    /// <param name="cutsceneName"></param>
    public void LoadCutscene(string cutsceneName)
    {
        //search for the cutscene in the list of available cutscenes
        if (cutscenes.Exists(x => x.cutsceneName == cutsceneName))
        {
            //check whether it's already loaded
            if (SceneManager.GetSceneByName(cutsceneName).isLoaded)
            {
                Debug.Log("already loaded: " + cutsceneName);
            }
            else
            {
                //load the cutscene asynchronously
                loadCutscene = AsyncLoadCutscene(cutsceneName);
                StartCoroutine(loadCutscene);
            }
        }
        else
        {
            Debug.LogError("No such cutscene as:" + cutsceneName);
        }
    }

    private IEnumerator AsyncLoadCutscene(string cutsceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(cutsceneName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        cutscenes.Find(x => x.cutsceneName == cutsceneName).CutsceneLoadedFlowTrigger();
    }

    /// <summary>
    /// plays a loaded cutscene. Usually called by a cutsceneReference when triggered by a bolt macro
    /// </summary>
    public void PlayCutscene(string cutsceneName)
    {
        //make sure scene exists and is loaded
        if (cutscenes.Exists(x => x.cutsceneName == cutsceneName))
        {
            if (SceneManager.GetSceneByName(cutsceneName).isLoaded)
            {
                //chapter.ActivateCurrentLocation(false); //turns off location and inventory cameras
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(cutsceneName));

                //NOTE. The Cutscene Player must be a component on a ROOT gameobject in the cutscene
                //Best if it's put on the FIRST root object in the scene
                bool playerFound = false; //for error checking
                //search for a CutscenePlayer. If found, start the cutscene and break out of the foreach loop
                foreach (GameObject obj in SceneManager.GetActiveScene().GetRootGameObjects())
                {                    
                    if (obj.GetComponent<CutscenePlayer>() != null)
                    {
                        obj.GetComponent<CutscenePlayer>().StartCutscene(this, cutsceneName);
                        playerFound = true;
                        break;
                    }
                }
                //error check
                if (!playerFound)
                {
                    Debug.LogError("No CutscenePlayer in root of cutscene: " + cutsceneName);
                }
                
            }
            else
            {
                Debug.LogError("Cutscene not loaded: " + cutsceneName);
            }
        }
        else
        {
            Debug.LogError("No such cutscene as:" + cutsceneName);
        }        
    }

    /// <summary>
    /// Ends a cutscene by telling the correct CutsceneReference object to trigger the endCutscene Bolt flow. Usually called by the CutscenePlayer in the cutscene
    /// </summary>
    public void EndCutscene(string cutsceneName)
    {
        SceneManager.SetActiveScene(chapterScene);
        cutscenes.Find(x => x.cutsceneName == cutsceneName).CutsceneEndFlowTrigger();        
    }    

    /// <summary>
    /// unloads a cutscene
    /// </summary>    
    public void UnloadCutscene(string cutsceneName)
    {
        if (SceneManager.GetActiveScene() == chapterScene)
        {
            unloadCutscene = AsyncUnloadCutscene(cutsceneName);
            StartCoroutine(unloadCutscene);
        }
        else
        {
            Debug.LogError("Need to make chapter scene active before unloading cutscene");
        }
        
    }

    private IEnumerator AsyncUnloadCutscene(string cutsceneName)
    {
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(cutsceneName);
        while (!asyncUnload.isDone)
        {            
            yield return null;
        }

        Resources.UnloadUnusedAssets();
    }
}
