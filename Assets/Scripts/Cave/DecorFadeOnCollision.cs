﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorFadeOnCollision : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Collision"); //Collision is true
        StartCoroutine(FadeAlphaToZero(GetComponent<SpriteRenderer>(), 0.2f));
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        StartCoroutine(UnFadeAlpha(GetComponent<SpriteRenderer>(), 0.2f));
        
    }

    IEnumerator FadeAlphaToZero(SpriteRenderer renderer, float duration)
    {
        Color startColor = renderer.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b,0.4f);
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            renderer.color = Color.Lerp(startColor, endColor, time / duration);
            yield return null;
        }
    }

    IEnumerator UnFadeAlpha(SpriteRenderer renderer, float duration)
    {
        Color startColor = renderer.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1);
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            renderer.color = Color.Lerp(startColor, endColor, time / duration);
            yield return null;
        }
    }
}
