using System.Collections;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
public class TutorUI : MonoBehaviour
{
    public gameLogic logic;
    public animationgrif Anime;

    public Coroutine tutorCoroutine;


    public void  CheckTutor(string message)
    {


        

         if (message == "это твоя статистика")
        {
            
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
                logic.light.gameObject.SetActive(true);

                logic.light.transform.position = new Vector3(3281.5f, 12f, 3675.857f);
                
                Anime.state = animationgrif.TutorState.CanTrain;
                logic.train = true;
                logic.StaminaUU.SetActive(true);
                
            }
        }
        else if (message == "чтобы прекратить тренировку кликни дважды по объекту")
        {
            Anime.state = animationgrif.TutorState.CanStop;
            
        }
 

    }


}