using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.SettingsManagement;
using UnityEngine.EventSystems;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;

/// <summary>
/// Центральный скрипт проекта: хранит характеристики игрока, stamina, уровень,
/// состояние обучения и ссылки на основные элементы UI.
/// animationgrif вызывает Train(), fadeScript показывает обучение,
/// FadeText и recordText отвечают за короткие текстовые эффекты.
/// </summary>
public class gameLogic : MonoBehaviour
{
    // Основные характеристики игрока, которые меняются во время тренировки.
    public int strength = 0;

    public int stamina = 100;
    public int maxStamina = 100;

    // Основная шкала прогресса и ее учебная копия для стартовой панели.
    public Slider progressBar;
    public GameObject progressContainer;
    public GameObject progressContainer1;
    public Slider progressBar1;

    // Основной stamina UI после обучения.
    public Slider Stamina;
    public GameObject StaminaUI;
    public int level = 1;
    public int powerPoints = 0;
    public TextMeshProUGUI checkLevel;

    // Если train false, animationgrif может проиграть анимацию, но Train() не начислит прогресс.
    public bool train = true;

    // Эффект крупного сообщения, например "новый уровень!".
    public recordText record;

    // Время, когда stamina закончилась; Update использует его для восстановления через 10 секунд.
    public float staminaTime;

    // Panel активна во время обучения. По ней другие скрипты понимают, идет tutorial или обычная игра.
    public GameObject Panel;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI statsText1;
    public TextMeshProUGUI textNew;
    public TextMeshProUGUI TutorText;
    public int countTutor= 0 ;

    // fadeScript печатает сообщения из tutorMessage и вызывает TutorUI.CheckTutor().
    public fadeScript fade;
    public bool tutorialRunning = false;
    public float targetValue = 100f;

    // Учебная stamina UI, которая показывается только на стартовой панели.
    public GameObject StaminaUU;
    public Slider stamina2;

    // target/light используются обучением для подсветки объекта тренировки.
    public GameObject target;
    public Light light;
    public GameObject lightCont;
    public string messageList;
    public positionTutor pos;
    public animationgrif anim;
  
    public GameObject PanelSettings; 
public TextMeshProUGUI exercise;
public string exer;
public int Weight=20;
public Button settingsBtn;
public bool setting = false;
public EventTrigger settingsTrigger;
public CancellationTokenSource StopSet;
public CancellationToken SetToken;
public bool isPaused = false;
public bool StaminaHill = false;
private Coroutine tutorCoroutine;
    // Последовательность обучения. Некоторые строки распознаются TutorUI и включают новые возможности.
    public List<string> tutorMessage = new List<string>()
{
    "привет,новенький",
    "ты оказался в месте,где из тебя сделают мужчину",
    "это твоя статистика",
    "это настройки игры",
    "кликни на объект для тренировки",
    "следи за шкалой выносливости.Она восстанавливается в течении 10 секунд после изнеможения",
    "чтобы прекратить тренировку кликни дважды по объекту"
};
    public Transform canv;

    // Количество активных всплывающих сообщений; нужно, чтобы новые сообщения смещались вниз.
    private int messageCount = 0;

public void Start()
{
    StartTutor();
    StartCoroutine(ListenSettings());

    lightCont.SetActive(true);

    EventTrigger.Entry entry = new EventTrigger.Entry();
    entry.eventID = EventTriggerType.PointerClick;

    entry.callback.AddListener((d) =>
    {
        SettingsOpen();
    });

    settingsTrigger.triggers.Add(entry);

}
 
    void Update()
    {
   // Одна и та же подпись уровня работает и для tutorial UI, и для основного UI.
        checkLevel.text = Panel.activeSelf
    ? $"{progressBar1.value}/{progressBar1.maxValue}"
    : $"{progressBar.value}/{progressBar.maxValue}";
Updates();
    }
        public async Task Recover()     
{   StaminaHill = true;
anim.state= animationgrif.TutorState.Locked;
        await Task.Delay(4000);
              Stamina.maxValue = maxStamina;
        while (stamina <maxStamina)
        { 
            stamina+=10;
            Stamina.value= stamina;
            if (stamina > maxStamina)
            {
                stamina =maxStamina;
                break;
            }
            
            Updates();
            await Task.Delay(1000);
             
              
        }
        StaminaHill = false;
                train = true;
                  ShowMessage("вы восстановились!");
                  anim.state = animationgrif.TutorState.CanTrain;
    }
    

    public async Task Train()
    {
        if (!train)
        {
            
            ShowMessage("нельзя тренироваться");
            return;
        }

        // Один успешный повтор увеличивает силу и очки.
    
        strength += 10;
        powerPoints += 5;

        CheckLevelUp();
        if (Panel.activeSelf)
        {
            // Во время обучения используем отдельную stamina-шкалу.
            StaminaUU.SetActive(true);
           StaminaUI.SetActive(false);
            stamina -= 20;
              stamina2.maxValue = maxStamina;
            stamina2.value = stamina;
           
        }
        if (!Panel.activeSelf)
        {
            // После обучения используем основной stamina UI.
            Stamina.maxValue = maxStamina;
              StaminaUU.SetActive(false);
            StaminaUI.SetActive(true);
            stamina -= 20;
            Stamina.maxValue = maxStamina;
            Stamina.value = stamina;
        }   

        if (stamina <= 0)
        {
            // Блокируем дальнейшие тренировки до восстановления в Update().
            stamina = 0;
            train = false;
            staminaTime = Time.time;

            ShowMessage("силы кончились, подождите 10 секунд");
            await Recover();
        }

        Updates();
    }

    public void CheckLevelUp()
    {
        int needStrength = level * 100 + level * level * 25;

        if (strength >= needStrength)
        {
            level++;

            // recordText показывает отдельный визуальный эффект поверх обычных всплывающих сообщений.
            record.Play("новый уровень!");
        }
    }

    public void ShowMessage(string message)
    {


            // Создаем отдельный экземпляр текста, чтобы несколько сообщений могли жить одновременно.
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
                // FadeText сам уничтожит этот текст после плавного исчезновения.
                newText.gameObject.AddComponent<FadeText>();
            }

            StartCoroutine(RemoveMessageSlot());
        
    }

    IEnumerator RemoveMessageSlot()
    {   
        yield return new WaitForSeconds(4f);

        // Освобождаем место в "стеке" сообщений после исчезновения текста.
        messageCount--;

        if (messageCount < 0)
        {
            messageCount = 0;
        }
    }

    public async UniTask ShowTutorMessages(CancellationToken token = default)
    { 
        //  Debug.Log(" ShowTutorMessages работает");
        //     Debug.Log("ShowTutorMessages CLICK");
      
        tutorialRunning = true;

        // fadeScript печатает каждую фразу и параллельно через TutorUI включает нужные элементы обучения.
        try
        {
            for (int i = 0; i < tutorMessage.Count; i++)
            {
                token.ThrowIfCancellationRequested();
                //  pos.Position(TutorText, tutorMessage[i]);
             
                await UniTask.Delay(10,cancellationToken:token);
                await fade.Typing(TutorText, tutorMessage[i], token);

                countTutor++;
            }

            await UniTask.Delay(2000, cancellationToken: token);
        }
        catch (System.OperationCanceledException) when (token.IsCancellationRequested)
        {
            tutorialRunning = false;
            return;
        }

        // После обучения переключаем интерфейс с учебного состояния на обычную игру.
        Panel.SetActive(false);
        light.gameObject.SetActive(false);
        StaminaUU.SetActive(false);
        statsText.gameObject.SetActive(true);
        statsText1.gameObject.SetActive(false);
        textNew.gameObject.SetActive(true);
        // StaminaUI.SetActive(true);
        progressContainer.gameObject.SetActive(true);
        progressContainer1.gameObject.SetActive(false);
         
settingsBtn.gameObject.SetActive(true);
        train = true;

        tutorialRunning = false;
    }
    public void Updates()
    {exercise.text = 
    $@"{exer}
    weight:{Weight}
    ";
        // Формула прогресса уровня: чем выше level, тем больше strength нужно до следующего уровня.
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
       

        // Обновляем оба блока статистики, чтобы tutorial UI и основной UI показывали одинаковые значения.
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
    public void SettingsOpen()
    {
        setting = !setting;

        PanelSettings.gameObject.SetActive(setting);
        isPaused = setting;
       
    }
    public IEnumerator ListenSettings()
    {
        while (true)
        {
            isPaused = PanelSettings != null && PanelSettings.activeSelf;

            yield return null;
        }
    }

    public void StartTutor()
    {Debug.Log("StartTutor работает");
        StopTutor();

        setting = false;
        isPaused = false;
        PanelSettings?.SetActive(false);

        StopSet = new CancellationTokenSource();
        SetToken = StopSet.Token;
        tutorCoroutine = StartCoroutine(TutorActive());
    }

    private void StopTutor()
    {
        if (tutorCoroutine != null)
        {
            StopCoroutine(tutorCoroutine);
            tutorCoroutine = null;
        }

        if (StopSet != null)
        {
            StopSet.Cancel();
            StopSet.Dispose();
            StopSet = null;
        }

        tutorialRunning = false;
    }

    public IEnumerator TutorActive()
    {
        Debug.Log("tutorActive работает");
        CancellationToken token = SetToken;
         
        settingsBtn.gameObject.SetActive(false);
        PanelSettings.gameObject.SetActive(false);
        Panel.SetActive(true);

        exercise.gameObject.SetActive(false);
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

        stamina2.maxValue = maxStamina;
        Stamina.maxValue = maxStamina;
        stamina2.value = stamina;
        Stamina.value = stamina;
    
        if (Panel.activeSelf)
        {
            yield return ShowTutorMessages(token).ToCoroutine();
        }

        if (!token.IsCancellationRequested)
        {
            Updates();
        }

        tutorCoroutine = null;
    }

    private void OnDestroy()
    {
        StopTutor();
    }
}
