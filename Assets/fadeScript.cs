using System.Collections;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEditor.Rendering;

/// <summary>
/// Печатает обучающие сообщения по буквам, удерживает их на экране и плавно скрывает.
/// gameLogic передает сюда текст из списка tutorMessage, а TutorUI получает тот же текст,
/// чтобы открыть нужные элементы обучения в момент появления сообщения.
/// </summary>
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
public gameLogic logic;
    

    public async UniTask Typing(TextMeshProUGUI textUI, string message) {
            while (logic.isPaused)
    {
        await UniTask.Yield();
    }
        // Готовим TextMeshProUGUI к новому сообщению: показываем, возвращаем прозрачность и очищаем текст.
        textUI.gameObject.SetActive(true);
        Color color = textUI.color;
        color.a = 1f;
        textUI.color = color;

        textUI.text = "";

        // Перед печатью синхронизируем tutorial-логику с текущей фразой.
        tutor.CheckTutor(message);

        // Эффект печатной машинки: добавляем символы по одному.
        for (int i = 0; i < message.Length; i++)
        {
            textUI.text += message[i];
            await UniTask.Delay(50);
        }

        // Даем игроку прочитать сообщение.
        await UniTask.Delay(3000);

        // Плавно скрываем текст, после чего gameLogic сможет показать следующую фразу.
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
