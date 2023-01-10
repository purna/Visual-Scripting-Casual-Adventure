using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinationDigit : MonoBehaviour
{
    public Sprite[] digitSprites = new Sprite[10];
    public int currentDigit = 0; //can set starting digits in unity editor if desired
    private SpriteRenderer digitRenderer;
    [HideInInspector]
    public CombinationLock comLock;

    private void Start()
    {
        digitRenderer = GetComponentInChildren<SpriteRenderer>();
        comLock = GetComponentInParent<CombinationLock>();
        DisplayCurrentDigit();                
    }

    public void GetNextDigit()
    {
        currentDigit++;
        if(currentDigit>=10)
        { 
            currentDigit = 0;
        }
        DisplayCurrentDigit();
        CheckCombination();
    }

    private void DisplayCurrentDigit()
    {
        digitRenderer.sprite = digitSprites[currentDigit];        
    }

    private void CheckCombination()
    {
        comLock.CheckCombination();
    } 

}
