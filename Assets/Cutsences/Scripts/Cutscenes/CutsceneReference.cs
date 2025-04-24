using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

//References and cutscene. Add it as a component to a child object of the CutsceneController
//The name of the object must be the name of the Unity scene that contains the cutscene
[RequireComponent(typeof(ScriptMachine))]
public class CutsceneReference : MonoBehaviour
{
    [HideInInspector]
    public string cutsceneName; //name the gameobject with the correct cutscene scene name
    private CutsceneController controller;

    private void Start()
    {                
        cutsceneName = gameObject.name;

        //check for errors in parenting of CutsceneReference and CutsceneController
        if (GetComponentInParent<CutsceneController>() == null)
        {
            Debug.LogError("Misplaced cutscene: " + cutsceneName + ". CutsceneReference object should be a child of the CutsceneController object");
        }
        else
        {
            controller = GetComponentInParent<CutsceneController>();
        }
    }

    /// <summary>
    /// Triggers the loadCutscene event in "CutsceneReferenceStartAndEndFlow" and "CutsceneReferenceSimplePlayer". Normally called by a InteractableGoToCutscene flow macro
    /// </summary>
    public void CutsceneLoadFlowTrigger()
    {
        CustomEvent.Trigger(gameObject, "loadCutscene");        
    }

    /// <summary>
    /// Triggers the endCutscene event in "CutsceneReferenceStartAndEndFlow" and "CutsceneReferenceSimplePlayer". 
    /// Normally called by the CutsceneController once the cutscene has finished 
    /// </summary>
    public void CutsceneEndFlowTrigger()
    {
        CustomEvent.Trigger(gameObject, "endCutscene");
    }    

    /// <summary>
    /// Load and immediately play a cutscene. Not recommended. Better to use the cutscene flow macros
    /// </summary>
    public void LoadAndPlayCutscene()
    {        
        controller.LoadAndPlayCutscene(cutsceneName);
    }

    /// <summary>
    /// loads a cutscene. Normally called by CutsceneReferenceSimplePlayer flow macro
    /// </summary>
    public void LoadCutscene()
    {
        controller.chapter.ActivateCurrentLocation(false); //turns off location and inventory cameras
        controller.LoadCutscene(cutsceneName);
    }

    /// <summary>
    /// Triggers the cutsceneLoaded event in "CutsceneReferenceStartAndEndFlow" and "CutsceneReferenceSimplePlayer". 
    /// Normally called once the CutsceneController has finished loading the cutscene
    /// </summary>
    public void CutsceneLoadedFlowTrigger()
    {
        CustomEvent.Trigger(gameObject, "cutsceneLoaded");
    }

    /// <summary>
    /// plays a cutscene. Normally called by CutsceneReferenceSimplePlayer flow macro
    /// </summary>
    public void PlayCutscene()
    {
        //controller.chapter.ActivateCurrentLocation(false); //turns off location and inventory cameras
        controller.PlayCutscene(cutsceneName);
    }

    /// <summary>
    /// unloads and ends a cutscene. Normally called by CutsceneReferenceSimplePlayer flow macro
    /// </summary>
    public void EndCutscene()
    {
        controller.chapter.ActivateCurrentLocation(true); //turns on location and inventory cameras
        controller.UnloadCutscene(cutsceneName);
    }
    
}

    
