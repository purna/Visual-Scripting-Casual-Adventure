using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialsSpriteShaker : MonoBehaviour
{
    public SpriteRenderer sprite;
    private float shakeAmount;
    private float defaultShakeAmount = 10f;
    private IEnumerator anim;

    // Start is called before the first frame update
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void ActivateSprite(bool active)
    {
        sprite.enabled = active;
    }

    public void ShakeSprite(float _shakeAmount)
    {
        if (_shakeAmount >= 0f)
        {
            shakeAmount = _shakeAmount;
        }
        else
        {
            shakeAmount = defaultShakeAmount;
        }
        anim = ShakeAnimation();
        StartCoroutine(anim);
    }

    public void StopShake()
    {
        shakeAmount = 0f;
    }

    public void ActivateAndShake(float _shakeAmount)
    {
        ActivateSprite(true);
        ShakeSprite(_shakeAmount);
    }

    private IEnumerator ShakeAnimation()
    {
        Vector3 startPos = sprite.transform.localPosition;
        while (true)
        {
            sprite.transform.localPosition = startPos + new Vector3(Random.Range(-shakeAmount, shakeAmount), Random.Range(-shakeAmount, shakeAmount), 0f);
            if (shakeAmount == 0f)
            {
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
