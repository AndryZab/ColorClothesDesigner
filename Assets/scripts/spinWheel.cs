using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class WheelFreeSpin : MonoBehaviour
{
    public float spinDuration = 5.0f;
    public float initialSpinSpeed = 1000.0f;
    public float continueSpinSpeed = 10.0f;
    public TextMeshProUGUI resultText;
    public Button spinButton;
    public TextMeshProUGUI spinCountText;
    public int spinsLeft;
    private bool isSpinning = false;
    private float currentSpinTime = 0.0f;
    private float currentSpinSpeed;
    private bool hasWon = false;
    public ParticleSystem particleEffects;
    public Image effectForStartSpin;
    public PrizeProbability[] prizes;
    private float declarationRate = 1f;
    private float currentAngle = 0f;
    public Button[] buttonClose;
    public TextMeshProUGUI coinsbalance;
    private int coinsINT;
    private Audiomanager audiomanager;

    private void Start()
    {
        audiomanager = FindAnyObjectByType<Audiomanager>();
    }
    public void StartSpin()
    {
        if (!isSpinning && spinsLeft > 0)
        {
            if (audiomanager != null)
            {
               audiomanager.PlaySFX(audiomanager.WheelFortune);

            }
            foreach (Button buttonclose in buttonClose)
            {

               buttonclose.interactable = false;
            }

            particleEffects.gameObject.SetActive(false);
            
            isSpinning = true;
            currentSpinTime = 0.0f;
            currentSpinSpeed = initialSpinSpeed;
            hasWon = false;
            UpdateSpinsLeftText();
            StartCoroutine(SpinWheel());
            StartCoroutine(FillImageOverTime(5f));
        }
    }
    private IEnumerator FillImageOverTime(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float fillAmount = Mathf.Clamp01(elapsedTime / duration);
            effectForStartSpin.fillAmount = fillAmount;
            yield return null;
        }
        effectForStartSpin.fillAmount = 1f;
        effectForStartSpin.fillAmount = 0f;
    }


    private void Update()
    {
        UpdateSpinsLeftText();
    }

    private IEnumerator SpinWheel()
    {
        DecreaseSpinCount();
        AnimationCurve slowdownCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        float targetAngle = GetTargetAngleBasedOnProbability();
        float totalRotation = 360 * currentSpinSpeed + targetAngle;

        float startAngle = currentAngle;
        float startRotation = transform.eulerAngles.z;

        float currentSpinTime = 0f;
        float currentRotation = startRotation;

        
        while (currentSpinTime < spinDuration)
        {
            currentSpinTime += Time.deltaTime;

            float t = Mathf.Clamp01(currentSpinTime / spinDuration);

            float curveValue = slowdownCurve.Evaluate(t);

            currentRotation = Mathf.Lerp(startRotation, totalRotation, curveValue);

            transform.eulerAngles = new Vector3(0, 0, currentRotation);

            yield return null;
        }

        transform.eulerAngles = new Vector3(0, 0, targetAngle);
       
       
        isSpinning = false;
        hasWon = true;
        string prizeName = DeterminePrize(targetAngle);

        resultText.text = string.Format("Reward: " + prizeName);
        coinsINT = int.Parse(prizeName);

        Debug.Log(coinsINT);

        int coinsSava = PlayerPrefs.GetInt("CoinsBalance") + coinsINT;

        PlayerPrefs.SetInt("CoinsBalance", coinsSava);
        PlayerPrefs.Save();
        coinsbalance.text = coinsSava.ToString();
        particleEffects.gameObject.SetActive(false);
        if (audiomanager != null)
        {
            if (coinsINT <= 1000)
            {
                audiomanager.PlaySFX(audiomanager.rewardcommon);

            }
            else if (coinsINT > 1000 && coinsINT <= 2000)
            {
                audiomanager.PlaySFX(audiomanager.rewardrare);

            }
            else if (coinsINT > 2000 && coinsINT <= 5000)
            {
                audiomanager.PlaySFX(audiomanager.rewardlegendary);

            }
        }
      
        particleEffects.gameObject.SetActive(true);



        foreach (Button buttonclose in buttonClose)
        {

            buttonclose.interactable = true;
        }
        yield return new WaitForSeconds(3f);

        while (!isSpinning)
        {
            currentAngle += continueSpinSpeed * Time.deltaTime;
            transform.Rotate(0, 0, continueSpinSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private float GetTargetAngleBasedOnProbability()
    {
        float totalProbability = 0f;
        foreach (PrizeProbability prize in prizes)
        {
            totalProbability += prize.probability;
        }

        float randomPoint = Random.Range(0f, totalProbability);
        float cumulativeProbability = 0f;

        foreach (PrizeProbability prize in prizes)
        {
            cumulativeProbability += prize.probability;
            if (randomPoint <= cumulativeProbability)
            {
                float angle = Random.Range(prize.minAngle, prize.maxAngle);
                return angle;
            }
        }

        return 0f;
    }

    private string DeterminePrize(float angle)
    {
        foreach (PrizeProbability prize in prizes)
        {
            if (angle >= prize.minAngle && angle <= prize.maxAngle)
            {
                return prize.prizeName;
            }
        }
        return null;
    }

    private void UpdateSpinsLeftText()
    {
        spinsLeft = PlayerPrefs.GetInt("0_Spin") + PlayerPrefs.GetInt("1_Spin") + PlayerPrefs.GetInt("2_Spin");
        spinCountText.text = "Spins: " + spinsLeft;
    }
    private void DecreaseSpinCount()
    {
        for (int i = 0; i < 3; i++)
        {
            string key = i + "_Spin";
            int count = PlayerPrefs.GetInt(key);
            if (count > 0)
            {
                count--;
                PlayerPrefs.SetInt(key, count);
                PlayerPrefs.Save();
                break;
            }
        }
    }
}

[System.Serializable]
public class PrizeProbability
{
    public string prizeName;
    public float probability;
    public float minAngle;
    public float maxAngle;
}
