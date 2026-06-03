using System.Collections;
using UnityEngine;

public class animationgrif : MonoBehaviour
{
    public GameObject stategrif;
    public GameObject activegrif;
    public Animator anim;
    public gameLogic logi;

    public Camera clickCamera;

    public TutorState state = TutorState.Locked;
    public bool isPlaying;
    public int count;

    public float time;
    public float needtime = 0.4f;
    public float pauseBeforeSecondAnim = 0.5f;
    public float secondAnimLength = 2f;

    private Coroutine benchCoroutine;

    public enum TutorState
    {
        Locked,
        CanTrain,
        CanStop
    }

    void Start()
    {
        stategrif?.SetActive(true);
        activegrif?.SetActive(false);
    }

    void Update()
    {
        if (logi?.Panel != null && !logi.Panel.activeSelf)
        {
            state = TutorState.CanTrain;
        }
       
        if (TryGetClickPosition(out Vector2 position) && HitsBarbell(position))///если вызванные методы вернут true то вызываем метод анимации
        {
            OnBarbellClick();
        }
    }

    private bool TryGetClickPosition(out Vector2 position)//передаем переменную позиции,которую заполняем в методе
    {
        if (Input.GetMouseButtonDown(0))///проверяем был клик для МЫШИ и заипсываем кооординаты
        {
            position = Input.mousePosition;///назначаем переменную позициии клика
            return true;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)/// проверяем был ли таб по экрану пальцем для мобилки
        {
            position = Input.GetTouch(0).position;///назначаем переменную позициии клика для мобилы 
            return true;
        }

        position = default; ///не было клика/касания = значения дефолт
        return false;
    }

    private bool HitsBarbell(Vector2 position)
    {
        return clickCamera != null && HitsBarbellFromCamera(clickCamera, position); ///проверям если камера назначена верно и метод возвращает true
    }

    private bool HitsBarbellFromCamera(Camera cameraToCheck, Vector2 position) //принимает в качестве аргумента объект камеры и позицию клика 
    {
        RaycastHit[] hits = Physics.RaycastAll( //создаем массив попаданий луча
            cameraToCheck.ScreenPointToRay(position),
            Mathf.Infinity,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Collide);

            foreach (RaycastHit hit in hits) ///проверяем каждое попадание луча на то,чтобы попадание было совершено на объекты стойки грифа
            {
                Transform hitTransform = hit.transform;

                if (IsSameOrRelated(hitTransform, stategrif?.transform) ||  
                    IsSameOrRelated(hitTransform, activegrif?.transform) ||
                    IsSameOrRelated(hitTransform, transform))
                {
                    return true;
                }
        }

        return false;
    }

    private bool IsSameOrRelated(Transform hit, Transform root)
    {
        return hit != null &&
               root != null &&
               (hit == root);
    }

    public void OnBarbellClick()    
    {
        bool doubleClick = Time.time - time <= needtime;
        bool canStop = state == TutorState.CanStop || (logi?.Panel != null && !logi.Panel.activeSelf);

        if (doubleClick && canStop)
        {
            StopTraining();
        }
        else
        {
            StartTraining();
        }

        time = Time.time;
    }

    private void StartTraining()
    {
        if (state == TutorState.Locked || isPlaying || (logi != null && logi.stamina <= 0))
        {
            return;
        }

        logi?.exercise?.gameObject.SetActive(true);

        if (logi != null)
        {
            logi.exer = "Barbell bicep curl";
            logi.Updates();
        }

        benchCoroutine = StartCoroutine(PlayBenchAnimation());
    }

    private void StopTraining()
    {
        logi?.StaminaUU?.SetActive(false);
        logi?.StaminaUI?.SetActive(false);
        logi?.exercise?.gameObject.SetActive(false);

        if (logi != null)
        {
            logi.exer = "none exercise";
            logi.Updates();
        }

        StopBenchAnimation();
        StartCoroutine(FinishUpCoroutine());
    }

    private IEnumerator PlayBenchAnimation()
    {
        isPlaying = true;
        count++;

        if (count == 1)
        {
            anim?.Play("walk to grif");
        }
        else
        {
            take();
            yield return new WaitForSeconds(pauseBeforeSecondAnim / 2);

            anim?.CrossFade(count == 2 ? "bench up_001" : "bench up_002", count == 2 ? 0.4f : 0.65f, 0, 0f);
            logi?.StaminaUI?.SetActive(true);
            logi?.Train();
        }

        logi?.Updates();

        yield return new WaitForSeconds(secondAnimLength);

        isPlaying = false;
        benchCoroutine = null;
    }

    private IEnumerator FinishUpCoroutine()
    {
        isPlaying = true;

        take();
        anim?.CrossFade("finishUp", 0.2f, 0, 0.1f);

        yield return new WaitForSeconds(1f);

        notake();
        anim?.CrossFade("move away", 0.2f, 0, 0.1f);

        yield return new WaitForSeconds(1f);

        count = 0;
        isPlaying = false;
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
        stategrif?.SetActive(false);
        activegrif?.SetActive(true);
    }

    public void notake()
    {
        stategrif?.SetActive(true);
        activegrif?.SetActive(false);
    }
}
