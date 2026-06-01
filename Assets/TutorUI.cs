using System.Collections;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

/// <summary>
/// Реагирует на конкретные сообщения обучения.
/// fadeScript печатает текст, а перед печатью вызывает CheckTutor,
/// чтобы нужные элементы UI и возможность тренировки включались в правильный момент.
/// </summary>
public class TutorUI : MonoBehaviour
{
    public gameLogic logic;

    public animationgrif Anime;

    public Coroutine tutorCoroutine;

    public void CheckTutor(string message)
    {
        if (message == "это твоя статистика")
        {
            logic?.statsText1?.gameObject.SetActive(true);
            logic?.progressContainer1?.gameObject.SetActive(true);
            logic?.checkLevel?.gameObject.SetActive(true);
        }

        else if (message == "кликни на объект для тренировки")
        {
            if (logic?.light == null || logic?.target == null || Anime == null)
                return;

            logic.light.gameObject.SetActive(true);

            logic.light.transform.position =
                new Vector3(3281.5f, 12f, 3675.857f);

            Anime.state = animationgrif.TutorState.CanTrain;

            logic.train = true;

            logic?.StaminaUU?.SetActive(true);
        }

        else if (message == "чтобы прекратить тренировку кликни дважды по объекту")
        {
            if (Anime == null)
                return;

            Anime.state = animationgrif.TutorState.CanStop;
        }
        else if(message=="это настройки игры")
        {
            logic.settingsBtn.gameObject.SetActive(true);
        }
        
    }
    public void Update()
    {
        if (logic.isPaused)
        {
            return;
        }
    }
}