using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialsFrameAnimationPlayer : MonoBehaviour
{
    public List<Sprite> animFrame = new List<Sprite>();
    private bool playAnim;
    private IEnumerator coroutine;
    private SpriteRenderer rend;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        rend.sprite = null;
    }

    public void StartAnim(float frameTime)
    {
        if (frameTime <= 0)
        {
            frameTime = .1f;
        }
        if (playAnim)
        {
            StopAnim();
        }
        playAnim = true;
        coroutine = AnimPlayer(frameTime);
        StartCoroutine(coroutine);
    }

    private IEnumerator AnimPlayer(float frameTime)
    {
        int frame = 0;
        while (playAnim)
        {
            rend.sprite = animFrame[frame];
            frame++;
            if (frame >= animFrame.Count)
            {
                frame = 0;
            }
            yield return new WaitForSeconds(frameTime);
        }
    }

    public void PauseAnim()
    {
        playAnim = false;
    }

    public void StopAnim()
    {
        if (playAnim)
        {
            StopCoroutine(coroutine);
            playAnim = false;
        }            
        rend.sprite = null;
        
    }
}
