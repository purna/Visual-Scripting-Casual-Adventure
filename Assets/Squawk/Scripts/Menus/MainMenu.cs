using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private GameController gc;

    private void Awake()
    {  
        //make sure the manu screen has the right sort order
        GetComponent<Canvas>().sortingOrder = 5;        
    }

    public void Initialise(GameController _gc)
    {
        //Debug.Log("Initialising MainMenu");
        gc = _gc;
    }

    /// <summary>
    /// called when the "play" button is clicked. The button deactivates itself to prevent multiple clicks.
    /// </summary>
    public void PlayGame()
    {
        //Debug.Log("PlayGame button");
        gc.StartNewChapter();       
    }
}
