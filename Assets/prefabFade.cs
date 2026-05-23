using UnityEngine;
using TMPro;

/// <summary>
/// Плавно скрывает временный TextMeshPro-текст.
/// gameLogic.ShowMessage() создает копию textNew, добавляет к ней FadeText,
/// а этот скрипт сам уменьшает прозрачность и уничтожает объект.
/// </summary>
public class FadeText : MonoBehaviour
{
    public float fadeSpeed = 1.5f;

    private TextMeshProUGUI textUI;
    private float timer = 0f;

    void Start()
    {
        // Скрипт вешается на уже созданный TextMeshProUGUI, поэтому компонент берем на том же объекте.
        textUI = GetComponent<TextMeshProUGUI>();
        textUI.fontStyle = FontStyles.Normal;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > 2f)
        {
            // Первые 2 секунды сообщение просто висит, затем начинает исчезать.
            Color color = textUI.color;

            color.a -= fadeSpeed * Time.deltaTime;

            textUI.color = color;

            if (color.a <= 0f)
            {
                // Удаляем только временный клон текста, созданный gameLogic.ShowMessage().
                Destroy(gameObject);
            }
        }
    }
}
