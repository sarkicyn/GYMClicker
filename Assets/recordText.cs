using System.Collections;
using TMPro;
using UnityEngine;

public class recordText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float appearTime = 0.25f;
    public float stayTime = 0.2f;
    public float fadeTime = 2f;
    public float shakePower = 0.02f ;

    private Vector2 startPos;

    void Start()
    {
        startPos = text.rectTransform.anchoredPosition;

        text.enabled = false;
    }

    public void Play(string message)
    {
        StopAllCoroutines();

        text.text = message;

        text.enabled = true;

        StartCoroutine(EffectRoutine());
    }

    IEnumerator EffectRoutine()
    {
        RectTransform rect = text.rectTransform;

        Color color = text.color;
        color.a = 1f;
        text.color = color;

        rect.localScale = Vector3.zero;
        rect.anchoredPosition = startPos;

        float timer = 0f;

        // ===== ��������� =====
        while (timer < appearTime)
        {
            timer += Time.deltaTime;

            float t = timer / appearTime;

            rect.localScale = Vector3.Lerp(
                Vector3.zero,
                Vector3.one * 1.08f,
                t
            );

            yield return null;
        }

        rect.localScale = Vector3.one;

        timer = 0f;

        // ===== ������ =====
        while (timer < stayTime)
        {
            timer += Time.deltaTime;

            rect.anchoredPosition =
                startPos +
                Random.insideUnitCircle * shakePower;

            yield return null;
        }

        rect.anchoredPosition = startPos;

        timer = 0f;

        // ===== ������������ =====
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;

            float t = timer / fadeTime;

            color.a = 1f - t;
            text.color = color;

            rect.localScale = Vector3.Lerp(
                Vector3.one,
                Vector3.one * 1.15f,
                t
            );

            yield return null;
        }

        text.enabled = false;

        rect.localScale = Vector3.one;
        rect.anchoredPosition = startPos;
    }
}