using UnityEngine;
using TMPro;

public class FadeText : MonoBehaviour
{
    public float fadeSpeed = 1.5f;

    private TextMeshProUGUI textUI;
    private float timer = 0f;

    void Start()
    {
        textUI = GetComponent<TextMeshProUGUI>();
        textUI.fontStyle = FontStyles.Normal;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > 2f)
        {
            Color color = textUI.color;

            color.a -= fadeSpeed * Time.deltaTime;

            textUI.color = color;

            if (color.a <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}