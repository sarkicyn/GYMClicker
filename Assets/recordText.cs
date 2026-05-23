using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Отдельный эффект для важных сообщений, например повышения уровня.
/// gameLogic.CheckLevelUp() вызывает Play("новый уровень!"),
/// после чего текст появляется, немного трясется и плавно исчезает.
/// </summary>
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
        // Запоминаем исходную позицию, чтобы после shake/fade вернуть текст на место.
        startPos = text.rectTransform.anchoredPosition;

        text.enabled = false;
    }

    public void Play(string message)
    {
        // Новый важный текст прерывает старый эффект, чтобы сообщения не накладывались друг на друга.
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

        // Появление: текст вырастает от нуля до нормального размера.
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

        // Короткое удержание с небольшим shake-эффектом.
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

        // Исчезновение: текст становится прозрачным и слегка увеличивается.
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

        // Сбрасываем трансформации, чтобы следующий Play стартовал из чистого состояния.
        rect.localScale = Vector3.one;
        rect.anchoredPosition = startPos;
    }
}
