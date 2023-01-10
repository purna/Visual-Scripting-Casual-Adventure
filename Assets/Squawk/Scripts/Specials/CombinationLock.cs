using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

//Generic combination lock of any number of digits.
//Add CombinationDigit objects as children and populate the list in the unity editor with those objects and the correct value for each to unlock
//Add a flowMachine to this object with an unlocked 
public class CombinationLock : MonoBehaviour
{
    public List<Combination> combinationLock = new List<Combination>();

    [System.Serializable]
    public class Combination
    {
        public CombinationDigit combinationDigit;
        public int expectedResult;
    }

    public void CheckCombination()
    {
        bool solved = true;
        foreach (Combination combination in combinationLock)
        {
            if(combination.combinationDigit.currentDigit != combination.expectedResult)
            {
                solved = false;
            }
        }
        if (solved)
        {
            CustomEvent.Trigger(gameObject, "unlocked");
        }
        else
        {
            CustomEvent.Trigger(gameObject, "locked");
        }
    }
    
}
