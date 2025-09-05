using UnityEngine;
using UnityEngine.UI;

public class MinorInformationText : MonoBehaviour
{
    [SerializeField] private Text informationText;

    private float clearTextTimer = 0f;
    private float clearTextDelay = 0f;
    private bool isClearTimerActive = false;

    private float unstoppableTimeTimer = 0f;
    private float unstoppableTimeSet = 0f;
    private bool isUnstoppableTimerActive = false;

    private void OnEnable()
    {
        ServiceLocator.Services.EventAggregator.MinorInformationTextReceived += UpdateInformationText;
        informationText.text = "";
    }

    private void OnDisable()
    {
        ServiceLocator.Services.EventAggregator.MinorInformationTextReceived -= UpdateInformationText;
    }

    private void Update()
    {
        if (isClearTimerActive)
        {
            clearTextTimer += Time.deltaTime;

            if (clearTextTimer >= clearTextDelay)
            {
                informationText.text = "";
                clearTextTimer = 0f;
                isClearTimerActive = false;
            }
        }

        if (isUnstoppableTimerActive)
        {
            unstoppableTimeTimer += Time.deltaTime;

            if (unstoppableTimeTimer >= unstoppableTimeSet)
            {
                unstoppableTimeSet = 0f; 
                unstoppableTimeTimer = 0f;
                isUnstoppableTimerActive = false;
            }
        }
    }

    private void UpdateInformationText(string text, float? delay = null, float? unstoppableTime = null)
    {
        if (informationText != null && !isUnstoppableTimerActive)
        {
            informationText.text = text;

            if (delay.HasValue)
            {
                clearTextDelay = delay.Value;
                clearTextTimer = 0f;
                isClearTimerActive = true;
            }
            else
            {
                isClearTimerActive = false;
            }

            if (unstoppableTime.HasValue)
            {
                unstoppableTimeTimer = 0f;
                unstoppableTimeSet = unstoppableTime.Value;
                isUnstoppableTimerActive = true;
            }
            else
            {
                isUnstoppableTimerActive = false;
            }

        }
    }
}