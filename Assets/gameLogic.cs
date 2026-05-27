using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

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
    bool tutorialRunning = false;
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
public Button settings;


    // Последовательность обучения. Некоторые строки распознаются TutorUI и включают новые возможности.
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

    // Количество активных всплывающих сообщений; нужно, чтобы новые сообщения смещались вниз.
    private int messageCount = 0;

    public void Start()
    {  
        // Стартуем с tutorial-панели: обычный игровой UI скрыт, тренировка заблокирована.
        settings.onClick.AddListener(SettingsOpen);
      
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
           lightCont.SetActive(true);
        if (Panel.activeSelf)
        {
            // Запускаем асинхронную цепочку обучающих сообщений.
            ShowTutorMessages();
        }
        Updates();

    }

    void Update()
    {
        
        if (stamina <= 0)
        {
            if (Time.time - staminaTime >= 1f)
            {
                // После полного истощения stamina восстанавливается один раз через 10 секунд.
                Recover();
                Stamina.value = stamina;
                Stamina.maxValue = maxStamina;

              

                Updates();
            }
        }

        // Одна и та же подпись уровня работает и для tutorial UI, и для основного UI.
        checkLevel.text = Panel.activeSelf
    ? $"{progressBar1.value}/{progressBar1.maxValue}"
    : $"{progressBar.value}/{progressBar.maxValue}";

    }
        public async Task Recover()     
    {
        await Task.Delay(1000);
        while (stamina != 100)
        {
            stamina+=20;
            await Task.Delay(2000);
            Stamina.value= stamina;
            Updates();
        }
                train = true;
                  ShowMessage("вы восстановились!");
    }

    public void Train()
    {
        if (!train)
        {
            // Например, обучение еще не дошло до шага, где тренировка разрешена.
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
           
            stamina -= 20;
              stamina2.maxValue = maxStamina;
            stamina2.value = stamina;
           
        }
        if (!Panel.activeSelf)
        {
            // После обучения используем основной stamina UI.
            Stamina.maxValue = maxStamina;
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

    public async UniTask ShowTutorMessages()
    {
        tutorialRunning = true;

        // fadeScript печатает каждую фразу и параллельно через TutorUI включает нужные элементы обучения.
        for (int i = 0; i < tutorMessage.Count; i++)
        {
            //  pos.Position(TutorText, tutorMessage[i]);
            await fade.Typing(TutorText, tutorMessage[i]);

            countTutor++;
        }

        await UniTask.Delay(2000);

        // После обучения переключаем интерфейс с учебного состояния на обычную игру.
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
train =false;
PanelSettings.gameObject.SetActive(true);
    }
}
