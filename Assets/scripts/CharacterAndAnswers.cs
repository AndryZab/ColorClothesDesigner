using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.CompilerServices;

public class CharactersAndAnswers : MonoBehaviour
{
    private Victory victory;
    public CharacterChoiceClothes[] scriptObjects;
    private float timerDuration = 20f; 
    private Coroutine timerCoroutine;
    public TextMeshProUGUI timerText;
    public GameObject[] characters;
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;
    public TextMeshProUGUI text3;
    public TextMeshProUGUI text4;
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;

    private Audiomanager audiomanager; 
    private int currentCharacterIndex = 0;
    private bool isCoroutineRunning = false;
    private bool activateTimerSound = false;
    private string[] veryBadQuotes = new string[]
    {
        "This outfit is completely unflattering.",
        "This look is an absolute miss.",
        "This style does not suit you at all.",
        "You’re not doing this outfit any favors.",
        "This outfit is far from your best look.",
        "The fit is awkward and unappealing.",
        "This choice is a fashion disaster.",
        "The style looks outdated and uninteresting.",
        "This doesn’t suit your figure well.",
        "The color choice is unappealing.",
        "This outfit looks mismatched.",
        "The design doesn’t complement you.",
        "You’re not making a good impression with this.",
        "This style is completely wrong for you.",
        "This choice looks very unappealing.",
        "The outfit is a complete letdown.",
        "This look does not highlight your best features.",
        "The style is completely out of place.",
        "This choice is incredibly disappointing.",
        "This outfit fails to impress.",
        "The fit is poor and uncomfortable.",
        "This color choice does not work for you.",
        "The design lacks any redeeming qualities.",
        "This outfit is a real eyesore.",
        "The style does nothing for your appearance.",
        "This choice is a major fashion faux pas.",
        "The fit and design are both unsatisfactory.",
        "This look is utterly unflattering.",
        "The color is completely unappealing.",
        "This outfit is a huge letdown."
    };

    private string[] badQuotes = new string[]
    {
        "This outfit is not the best choice for you.",
        "The style doesn’t quite fit you.",
        "This choice could be improved.",
        "The fit seems a bit off.",
        "This color isn’t the most flattering.",
        "The design doesn’t really suit you.",
        "This outfit isn’t highlighting your strengths.",
        "The look isn’t very impressive.",
        "This outfit is a bit of a miss.",
        "The fit isn’t quite right.",
        "This color doesn’t do you justice.",
        "The style isn’t working as well as it could.",
        "This choice is less flattering than expected.",
        "The outfit doesn’t stand out as much.",
        "The design is somewhat off.",
        "This look isn’t quite right for you.",
        "The color could be more suitable.",
        "This outfit isn’t very eye-catching.",
        "The style could be more aligned with your look.",
        "The fit could use some adjustments.",
        "This choice doesn’t enhance your appearance.",
        "The design is not quite what you need.",
        "The look is a bit dull.",
        "This outfit doesn’t bring out your best features.",
        "The style isn’t very complementary.",
        "This outfit could use a bit of improvement.",
        "The fit isn’t very flattering.",
        "This color isn’t very appealing.",
        "The design seems a bit off.",
        "This choice is a bit of a letdown.",
        "The style isn’t quite right for you."
    };

    private string[] normalQuotes = new string[]
    {
        "This outfit is decent.",
        "The color looks alright.",
        "It’s a reasonable choice for you.",
        "You look pretty good in this.",
        "The fit is acceptable.",
        "This style works fairly well.",
        "The outfit is neither great nor terrible.",
        "This choice is quite satisfactory.",
        "You’re looking alright in this.",
        "The color choice is passable.",
        "This design is suitable.",
        "It’s a fair choice for your style.",
        "This outfit is acceptable.",
        "The fit is decent enough.",
        "The style is reasonable.",
        "This look is fairly good.",
        "The color works moderately well.",
        "This outfit suits you adequately.",
        "It’s a middle-of-the-road choice.",
        "The style is fairly fitting.",
        "The outfit looks decent on you.",
        "The color is okay.",
        "This style isn’t too bad.",
        "The fit is acceptable for the occasion.",
        "The outfit is pretty good.",
        "This look is fairly appropriate.",
        "The color choice is satisfactory.",
        "The design is acceptable.",
        "This outfit is reasonable for you.",
        "The style is neither bad nor great.",
        "The outfit is fairly decent."
    };

    private string[] goodQuotes = new string[]
    {
        "You look absolutely stunning!",
        "This color enhances your features beautifully.",
        "You have an amazing sense of style.",
        "This outfit fits you perfectly.",
        "You look fabulous in this outfit.",
        "The style is a perfect match for you.",
        "You’re really rocking this look!",
        "This outfit is perfect for you.",
        "You look exceptionally stylish.",
        "The design is absolutely great on you.",
        "This outfit highlights your best features.",
        "You’re making a fashion statement!",
        "The color suits you wonderfully.",
        "You look fantastic in this.",
        "The outfit is a perfect fit for you.",
        "This style is spot on!",
        "You look incredible!",
        "This outfit is a great choice.",
        "You’re looking very chic.",
        "The color complements you beautifully.",
        "This design is exceptional.",
        "You look stunning in this outfit.",
        "This style brings out the best in you.",
        "You’re absolutely dazzling!",
        "This choice is impeccable.",
        "You wear this outfit with confidence.",
        "The fit is perfect and flattering.",
        "This look is truly outstanding.",
        "The design fits you like a glove.",
        "You look amazing in this outfit.",
        "This style enhances your appearance perfectly."
    };

    private List<Button> buttons = new List<Button>();
    private Dictionary<Button, (string quote, string prize)> buttonToQuoteMap = new Dictionary<Button, (string, string)>();

    void Start()
    {
        audiomanager = FindObjectOfType<Audiomanager>();
        victory = FindAnyObjectByType<Victory>();
        foreach (CharacterChoiceClothes panel in scriptObjects)
        {
            if (panel != null)
            {
                if (panel.SellSlots > 0)
                {
                    panel.PanelForActivate.SetActive(true);
                }
                else
                {
                    panel.PanelForActivate.SetActive(false);
                }
            }
        }
        buttons.Add(button1);
        buttons.Add(button2);
        buttons.Add(button3);
        buttons.Add(button4);

        List<TextMeshProUGUI> texts = new List<TextMeshProUGUI> { text1, text2, text3, text4 };

        AssignRandomQuotes(texts);
        ActivateRandomCharacter();
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i].activeSelf)
            {
                currentCharacterIndex = i;
                break;
            }
        }
        StartTimer(); 

    }

    void ActivateRandomCharacter()
    {
        foreach (GameObject character in characters)
        {
            if (character != null)
                character.SetActive(false);
        }

        if (characters.Length > 0)
        {
            int randomIndex = Random.Range(0, characters.Length);
            GameObject selectedCharacter = characters[randomIndex];
            if (selectedCharacter != null)
                selectedCharacter.SetActive(true);
        }
    }

    void AssignRandomQuotes(List<TextMeshProUGUI> texts)
    {
        List<string> chosenVeryBadQuotes = GetUniqueRandomQuotes(veryBadQuotes, 1);
        List<string> chosenBadQuotes = GetUniqueRandomQuotes(badQuotes, 1);
        List<string> chosenNormalQuotes = GetUniqueRandomQuotes(normalQuotes, 1);
        List<string> chosenGoodQuotes = GetUniqueRandomQuotes(goodQuotes, 1);

        List<string> combinedQuotes = new List<string>();
        combinedQuotes.AddRange(chosenVeryBadQuotes);
        combinedQuotes.AddRange(chosenBadQuotes);
        combinedQuotes.AddRange(chosenNormalQuotes);
        combinedQuotes.AddRange(chosenGoodQuotes);

        ShuffleList(combinedQuotes);

        for (int i = 0; i < texts.Count; i++)
        {
            texts[i].text = combinedQuotes[i];
            Button button = buttons[i];
            string quote = combinedQuotes[i];
            string prize = GetPrizeType(quote);
            buttonToQuoteMap[button] = (quote, prize);

            int index = i;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnButtonClicked(buttonToQuoteMap[button]));
        }
    }

    List<string> GetUniqueRandomQuotes(string[] quotes, int count)
    {
        List<string> shuffledQuotes = new List<string>(quotes);
        ShuffleList(shuffledQuotes);
        return shuffledQuotes.GetRange(0, count);
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public string GetPrizeType(string quote)
    {
        if (veryBadQuotes.Contains(quote)) return "Very Bad";
        if (badQuotes.Contains(quote)) return "Bad";
        if (normalQuotes.Contains(quote)) return "Normal";
        if (goodQuotes.Contains(quote)) return "Good";
        return "Unknown";
    }

    void NextCharacter()
    {
        if (characters.Length == 0) return;

        foreach (GameObject character in characters)
        {
            if (character != null)
                character.SetActive(false);
        }

        if (characters.Length > 0)
        {
            currentCharacterIndex = (currentCharacterIndex + 1) % characters.Length;
            GameObject selectedCharacter = characters[currentCharacterIndex];
            if (selectedCharacter != null)
                selectedCharacter.SetActive(true);
        }
    }
    private void ResetAnim()
    {
        foreach (GameObject character in characters)
        {
            if (character != null)
            {
                Animator animator = character.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetInteger("Character", 0);
                    int currentAnimationValue = animator.GetInteger("Character");
                }
            }
        }
    }
    private void TextForEvalutation(string prize)
    {
        foreach (CharacterChoiceClothes script in scriptObjects)
        {
            if (script != null && script.gameObject.activeSelf)
            {
                switch (prize)
                {
                    case "Very Bad":
                        script.textToPrice("Very Bad");
                        break;
                    case "Bad":
                        script.textToPrice("Bad");
                        break;
                    case "Normal":
                        script.textToPrice("Normal");
                        break;
                    case "Good":
                        script.textToPrice("Good");
                        break;
                    default:
                        break;
                }
            }
        }
    }
    void StartTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine); 
        }
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }
    private IEnumerator TimerCoroutine()
    {
        float timeRemaining = timerDuration;
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            
            timerText.text = $"Time Remaining: {timeRemaining:F1}";

            yield return null;

            if (timeRemaining <= 19 && timeRemaining >= 18)
            {
                activateTimerSound = true;
            }
            if (timeRemaining < 4 && activateTimerSound)
            {
                audiomanager.PlaySFX(audiomanager.timer);
                activateTimerSound = false;
            }
        }

        OnButtonClicked(("Timeout: Automatic Bad Choice", "Bad"));
    }

    public void OnButtonClicked((string quote, string prize) buttonInfo)
    {
        audiomanager.PlaySFX(audiomanager.button);
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine); 
        }

        string quote = buttonInfo.quote;
        string prize = buttonInfo.prize;

        HandleCharacterAnimation(prize);
        TextForEvalutation(prize);
        foreach (CharacterChoiceClothes obj in scriptObjects)
        {
            if (obj != null && obj.gameObject.activeSelf)
            {
                obj.ChildObject();
            }
        }
        if (!isCoroutineRunning)
        {
            StartCoroutine(delayForMethods());
        }

    }
    private void Update()
    {
        if (victory.winBoard.activeSelf)
        {
            StopCoroutine(timerCoroutine);
        }
    }
    private IEnumerator delayForMethods()
    {
        isCoroutineRunning = true;

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        button1.enabled = false;
        button2.enabled = false;
        button3.enabled = false;
        button4.enabled = false;

        yield return new WaitForSeconds(1.37f);

        List<TextMeshProUGUI> texts = new List<TextMeshProUGUI> { text1, text2, text3, text4 };
        AssignRandomQuotes(texts);
        NextCharacter();
        ResetAnim();

        StartTimer();

        button1.enabled = true;
        button2.enabled = true;
        button3.enabled = true;
        button4.enabled = true;

        isCoroutineRunning = false;
    }

    void HandleCharacterAnimation(string prize)
    {
        int animationParameter = 0;

        switch (prize)
        {
            case "Very Bad":
                animationParameter = 4;
                break;
            case "Bad":
                animationParameter = 3;
                break;
            case "Normal":
                animationParameter = 1;
                break;
            case "Good":
                animationParameter = 2;
                break;
            default:
                break;
        }

        for (int i = 0; i < characters.Length; i++)
        {
            GameObject character = characters[i];
            if (character != null && character.activeSelf)
            {
                Animator animator = character.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetInteger("Character", animationParameter);
                }
            }
        }
    }



}
