using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

/// <summary>
/// Управляет кликами по объекту тренировки и анимациями грифа.
/// Скрипт получает разрешение на тренировку от TutorUI/gameLogic,
/// а при успешном повторении вызывает gameLogic.Train(), чтобы начислить прогресс и потратить stamina.
/// </summary>
public class animationgrif : MonoBehaviour
{
    // Два визуальных состояния грифа: лежит на стойке или находится в руках/активной анимации.
    public GameObject stategrif;
    public GameObject activegrif;

    public Animator anim;

    // Главная игровая логика, через нее проверяется stamina и обновляются статы.
    public gameLogic logi;
/// <summary>
/// Состояние обучения ограничивает, что можно сделать кликом:
/// Locked - тренировка закрыта, CanTrain - можно начать, CanStop - двойной клик остановит тренировку.
/// </summary>
    public enum TutorState
    {
        Locked,
        CanTrain,
        CanStop
    }

    public TutorState state = TutorState.Locked;

    public bool isPlaying = false;

    // count определяет, какую анимацию запускать: первый клик - подход к грифу, следующие - повторы.
    public int count = 0;

    // time/needtime используются для распознавания двойного клика.
    public float time;
    public float needtime = 0.4f;

    public float pauseBeforeSecondAnim = 1f;
    public float secondAnimLength = 2f;

    public Coroutine benchCoroutine;
    public Coroutine stopCoroutine;

    void Start()
    {
        // В начале гриф лежит на стойке, активная версия скрыта.
        stategrif.SetActive(true);
        activegrif.SetActive(false);

    
    }
public void Update()
    {
        if (state == TutorState.CanStop)
        {
         
            StopBenchAnimation();
   
        }
        if (!logi.Panel.activeSelf)
        {
            // После окончания обучения обычная тренировка всегда доступна, если хватает stamina.
            state = TutorState.CanTrain;
        }
    }
    public void OnMouseDown()
    {
        


        if (Time.time - time <= needtime && state == TutorState.CanStop)
        {
            // Во время обучения двойной клик завершает тренировку и запускает уход от грифа.
                    logi.StaminaUU.SetActive(false);
        

            StopBenchAnimation();

            stopCoroutine = StartCoroutine(FinishUpCoroutine());
        }
        else if(!logi.Panel.activeSelf &&Time.time - time <= needtime){
            // После обучения двойной клик тоже используется как остановка текущей серии.
                logi.StaminaUU.SetActive(false);
        logi.StaminaUI.SetActive(false);
            StopBenchAnimation();

            stopCoroutine = StartCoroutine(FinishUpCoroutine());
        
        }
        else
        {
            Player();
        }
        time = Time.time;
    }

    public void Player()
    {
        

        if (state == TutorState.Locked)
        {
            // Пока TutorUI не разрешил тренировку, клики игнорируются.
            return;
        }

        if (logi.stamina <= 0)
        {
            // gameLogic восстановит stamina через таймер, здесь только запрещаем старт анимации.
            return;
        }

        if (isPlaying)
        {
            // Не даем наложить одну тренировочную анимацию на другую.
            return;
        }

      

        benchCoroutine = StartCoroutine(PlayBenchAnimation());
    }

    IEnumerator PlayBenchAnimation()
    {

        isPlaying = true;

        count++;


        if (count == 1)
        {
            // Первый клик подводит персонажа к грифу без начисления тренировки.
            anim.Play("walk to grif");
        }
        else if (count == 2)
        {
           

            take();

            yield return new WaitForSeconds(pauseBeforeSecondAnim / 2);

            anim.CrossFade("bench up_001", 0.4f, 0, 0f);
            // Повтор засчитывается в gameLogic: сила растет, stamina уменьшается, UI обновляется.
            logi.Train();
        }
        else
        {
        

            take();

            yield return new WaitForSeconds(pauseBeforeSecondAnim / 2);

            anim.CrossFade("bench up_002", 0.65f, 0, 0f);
            // Все следующие повторы используют другую анимацию, но ту же игровую логику тренировки.
            logi.Train();
        }

        // Синхронизируем UI даже после первого подхода к грифу.
        logi.Updates();

        yield return new WaitForSeconds(secondAnimLength);

        isPlaying = false;
        benchCoroutine = null;

    }

    IEnumerator FinishUpCoroutine()
    {


        isPlaying = true;

        // Завершение серии: персонаж кладет гриф, уходит, stamina UI скрывается.
        take();
        anim.CrossFade("finishUp", 0.2f, 0, 0f);

        yield return new WaitForSeconds(1f);

        notake();

        anim.CrossFade("move away", 0.2f, 0, 0f);

        yield return new WaitForSeconds(1f);

        count = 0;
        isPlaying = false;

        // После ухода нужен новый разрешающий шаг/клик для старта следующей серии.
        state = TutorState.Locked;

    }

    public void StopBenchAnimation()
    {
       

        if (benchCoroutine != null)
        {
            StopCoroutine(benchCoroutine);
            benchCoroutine = null;

        }

        isPlaying = false;
    }

    public void take()
    {
      

        // Переключаем модель грифа из состояния "на стойке" в активное состояние.
        stategrif.SetActive(false);
        activegrif.SetActive(true);
    }

    public void notake()
    {

        // Возвращаем гриф на стойку после завершения анимации.
        stategrif.SetActive(true);
        activegrif.SetActive(false);
    }
}
