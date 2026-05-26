using System.Collections;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

/// <summary>
/// Реагирует на конкретные сообщения обучения.
/// fadeScript печатает текст, а перед печатью вызывает CheckTutor,
/// чтобы нужные элементы UI и возможность тренировки включались в правильный момент.
/// </summary>
public class TutorUI : MonoBehaviour
{
    // Центральная логика игры: хранит UI, статы, stamina и флаги обучения.
    public gameLogic logic;

    // Скрипт объекта тренировки: TutorUI переводит его в состояние, где можно тренироваться или остановиться.
    public animationgrif Anime;

    // Оставлено под возможное управление корутиной обучения из инспектора/другого скрипта.
    public Coroutine tutorCoroutine;

    public void CheckTutor(string message)
    {
        if (message == "это твоя статистика")
        {
            // На этом шаге обучения показываем учебную версию блока статистики.
            logic.statsText1.gameObject.SetActive(true);
            logic.progressContainer1.gameObject.SetActive(true);
            logic.checkLevel.gameObject.SetActive(true);
        }
        else if (message == "кликни на объект для тренировки")
        {
            if (logic == null || logic.light == null || logic.target == null)
            {
                Debug.LogWarning("TutorUI: logic, light или target не назначены в инспекторе.");
            }
            else
            {
                // Подсветка ведет игрока к объекту, после чего animationgrif разрешает запуск тренировки.
                logic.light.gameObject.SetActive(true);

                logic.light.transform.position =
                    new Vector3(3281.5f, 12f, 3675.857f);

                Anime.state = animationgrif.TutorState.CanTrain;

                logic.train = true;
                logic.StaminaUU.SetActive(true);
            }
        }
        else if (message == "чтобы прекратить тренировку кликни дважды по объекту")
        {
            // Следующий двойной клик по объекту тренировки будет воспринят как команда остановиться.
            Anime.state = animationgrif.TutorState.CanStop;
        }
    }
}