using System.Collections;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class fadeScript : MonoBehaviour
{
    [Header("Typing settings")]
    public float typeSpeed = 0.02f;
    public TutorUI tutor;
    [Header("Hold before fade")]
    public float holdTime = 3f;
    public Coroutine typing;
    [Header("Fade settings")]
    public float fadeSpeed = 1f;

    


    public async UniTask Typing(TextMeshProUGUI textUI, string message) {
        textUI.gameObject.SetActive(true);
        Color color = textUI.color;
        color.a = 1f;
        textUI.color = color;

        textUI.text = "";

        // 2. TYPEWRITER EFFECT
        tutor.CheckTutor(message);
        for (int i = 0; i < message.Length; i++)
        {
            textUI.text += message[i];
            await UniTask.Delay(50);
        }

        // 3. HOLD
        await UniTask.Delay(3000);

        // 4. FADE OUT
        while (textUI.color.a > 0f)
        {
            color.a -= Time.deltaTime * fadeSpeed;
            textUI.color = color;
            await UniTask.Yield();
        }

        textUI.gameObject.SetActive(false);

        Debug.Log("END MESSAGE: " + message);
    } 
}