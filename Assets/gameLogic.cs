using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
public class gameLogic : MonoBehaviour
{
    public int strength = 0;

    public int stamina = 100;
    public int maxStamina = 100;
    public Slider progressBar;
    public GameObject progressContainer;
    public GameObject progressContainer1;
    public Slider progressBar1;
    public Slider Stamina;
    public GameObject StaminaUI;
    public int level = 1;
    public int powerPoints = 0;
    public TextMeshProUGUI checkLevel;
    public bool train = true;
    public recordText record;
    public float staminaTime;
    public GameObject Panel;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI statsText1;
    public TextMeshProUGUI textNew;
    public TextMeshProUGUI TutorText;
    public int countTutor= 0 ;
    public fadeScript fade;
    bool tutorialRunning = false;
    public float targetValue = 100f;
    public GameObject StaminaUU;
    public Slider stamina2;
    public GameObject target;
    public Light light;
    public string messageList;
    public positionTutor pos;
    public List<string> tutorMessage = new List<string>()
{
    "привет,новенький",
    "ты оказался в месте,где из тебя сделают мужчину",
    "это твоя статистика",
    "кликни на объект для тренировки",
    "следи за шкалой выносливости.Она восстанавливается в течении 10 секунд после изнеможения",
    "чтобы прекратить тренировку кликни дважды по объекту"
};
    public Transform canv;

    private int messageCount = 0;

    public void Start()
    {
        Panel.SetActive(true);
        train = false;    
        statsText.gameObject.SetActive(false);
        textNew.gameObject.SetActive(false);
        StaminaUI.SetActive(false);
        StaminaUU.SetActive(false);
        light.gameObject.SetActive(false);
        statsText1.gameObject.SetActive(false);
        progressContainer1.gameObject.SetActive(false);
        checkLevel.gameObject.SetActive(false);
        progressContainer.gameObject.SetActive(false);
        if (Panel.activeSelf)
        {
            ShowTutorMessages();
        }
        Updates();

    }

    void Update()
    {
        if (stamina <= 0)
        {
            if (Time.time - staminaTime >= 10f)
            {
                stamina = maxStamina;
                Stamina.maxValue = maxStamina;
                Stamina.value = stamina;
                train = true;

                ShowMessage("вы восстановились!");

                Updates();
            }
        }
        checkLevel.text = Panel.activeSelf
    ? $"{progressBar1.value}/{progressBar1.maxValue}"
    : $"{progressBar.value}/{progressBar.maxValue}";

    }

    public void Train()
    {
        if (!train)
        {
            ShowMessage("нельзя тренироваться");
            return;
        }
        strength += 10;
        powerPoints += 5;

        CheckLevelUp();
        if (Panel.activeSelf)
        {
            
            StaminaUU.SetActive(true);
            stamina -= 20;
            stamina2.maxValue = maxStamina;
            stamina2.value = stamina;
           
        }
        if (!Panel.activeSelf)
        {
        StaminaUI.SetActive(true);
            stamina -= 20;
            Stamina.maxValue = maxStamina;
            Stamina.value = stamina;
        }

        if (stamina <= 0)
        {
            stamina = 0;
            train = false;
            staminaTime = Time.time;

            ShowMessage("силы кончились, подождите 10 секунд");
            
        }

        Updates();
    }

    public void CheckLevelUp()
    {
        int needStrength = level * 100 + level * level * 25;

        if (strength >= needStrength)
        {
            level++;

            record.Play("новый уровень!");
        }
    }

    public void ShowMessage(string message)
    {


            TextMeshProUGUI newText = Instantiate(textNew, canv);

            newText.text = message;
            newText.color = new Color(newText.color.r, newText.color.g, newText.color.b, 1f);
            newText.fontStyle = FontStyles.Normal;

            RectTransform rect = newText.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            rect.anchoredPosition = new Vector2(0f, 0f - messageCount * 50f);

            messageCount++;

            if (newText.GetComponent<FadeText>() == null)
            {
                newText.gameObject.AddComponent<FadeText>();
            }

            StartCoroutine(RemoveMessageSlot());
        
    }

    IEnumerator RemoveMessageSlot()
    {
        yield return new WaitForSeconds(4f);

        messageCount--;

        if (messageCount < 0)
        {
            messageCount = 0;
        }
    }

    public async UniTask ShowTutorMessages()
    {
        tutorialRunning = true;

        for (int i = 0; i < tutorMessage.Count; i++)
        {
            //  pos.Position(TutorText, tutorMessage[i]);
            await fade.Typing(TutorText, tutorMessage[i]);

            countTutor++;
        }

        await UniTask.Delay(2000);

        Panel.SetActive(false);
        light.gameObject.SetActive(false);
        StaminaUU.SetActive(false);
        statsText.gameObject.SetActive(true);
        statsText1.gameObject.SetActive(false);
        textNew.gameObject.SetActive(true);
        StaminaUI.SetActive(true);
        progressContainer.gameObject.SetActive(true);
        progressContainer1.gameObject.SetActive(false);

        train = true;

        tutorialRunning = false;
    }
    public void Updates()
    {
        int needStrength =
        level * 100 + level * level * 25; 

        int previousNeedStrength =
        (level - 1) * 100 +
        (level - 1) * (level - 1) * 25;

        float progressValue =
        (float)(strength - previousNeedStrength) /
        (needStrength - previousNeedStrength);
        progressBar.value =
strength - previousNeedStrength;

        progressBar.maxValue =
        needStrength - previousNeedStrength;
        progressBar1.maxValue =needStrength - previousNeedStrength;
        progressBar1.value =
strength - previousNeedStrength;

        progressBar1.maxValue =
        needStrength - previousNeedStrength;
        progressBar1.maxValue = needStrength - previousNeedStrength;
       

        statsText1.text = $@"
Level: {level}
Strength: {strength}
Power Points: {powerPoints}
Stamina: {stamina}/{maxStamina}
";


        statsText.text = $@"
Level: {level}
Strength: {strength}
Power Points: {powerPoints}
Stamina: {stamina}/{maxStamina}
";
    }
}