using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class animationgrif : MonoBehaviour
{
    public GameObject stategrif;
    public GameObject activegrif;

    public Animator anim;
    public gameLogic logi;

    public enum TutorState
    {
        Locked,
        CanTrain,
        CanStop
    }

    public TutorState state = TutorState.Locked;

    public bool isPlaying = false;

    public int count = 0;

    public float time;
    public float needtime = 0.4f;

    public float pauseBeforeSecondAnim = 1f;
    public float secondAnimLength = 2f;

    public Coroutine benchCoroutine;
    public Coroutine stopCoroutine;

    void Start()
    {
        stategrif.SetActive(true);
        activegrif.SetActive(false);

    
    }

    public void OnMouseDown()
    {
        

        if (!logi.Panel.activeSelf)
        {
            state = TutorState.CanTrain;
        }

        if (Time.time - time <= needtime && state == TutorState.CanStop)
        {
           
            logi.StaminaUU.SetActive(false);

            StopBenchAnimation();

            stopCoroutine = StartCoroutine(FinishUpCoroutine());
        }
        else if(!logi.Panel.activeSelf &&Time.time - time <= needtime){
        
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
           
            return;
        }

        if (logi.stamina <= 0)
        {
            
            return;
        }

        if (isPlaying)
        {
         
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
            anim.Play("walk to grif");
        }
        else if (count == 2)
        {
           

            take();

            yield return new WaitForSeconds(pauseBeforeSecondAnim / 2);

            anim.CrossFade("bench up_001", 0.4f, 0, 0f);
            logi.Train();
        }
        else
        {
        

            take();

            yield return new WaitForSeconds(pauseBeforeSecondAnim / 2);

            anim.CrossFade("bench up_002", 0.65f, 0, 0f);
            logi.Train();
        }

        logi.Updates();

        yield return new WaitForSeconds(secondAnimLength);

        isPlaying = false;
        benchCoroutine = null;

    }

    IEnumerator FinishUpCoroutine()
    {


        isPlaying = true;

        take();
        logi.StaminaUU.SetActive(false);
        anim.CrossFade("finishUp", 0.2f, 0, 0f);

        yield return new WaitForSeconds(1f);

        notake();

        anim.CrossFade("move away", 0.2f, 0, 0f);

        logi.StaminaUI.SetActive(false);

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
      

        stategrif.SetActive(false);
        activegrif.SetActive(true);
    }

    public void notake()
    {

        stategrif.SetActive(true);
        activegrif.SetActive(false);
    }
}
