using System.Collections;
using UnityEngine;
using TMPro;

public class TextRevealAnimation : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public float revealDuration = 2.0f; // Duration in seconds

    private void Start()
    {
        // Start the text reveal animation
        StartCoroutine(AnimateTextReveal());
    }

    private IEnumerator AnimateTextReveal()
    {
        textMeshPro.maxVisibleCharacters = 0;
        int totalCharacters = textMeshPro.text.Length;
        float elapsedTime = 0.0f;

        while (elapsedTime < revealDuration)
        {
            int visibleCharacters = (int)(totalCharacters * (elapsedTime / revealDuration));
            textMeshPro.maxVisibleCharacters = visibleCharacters;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure all characters are visible at the end of the animation
        textMeshPro.maxVisibleCharacters = totalCharacters;
    }
}