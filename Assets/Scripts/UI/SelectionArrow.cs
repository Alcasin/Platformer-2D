using UnityEngine;
using UnityEngine.UI;

public class SelectionArrow : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private RectTransform[] buttons;

    [Header("Audio")]
    [SerializeField] private AudioClip changeSound;
    [SerializeField] private AudioClip interactSound;

    private RectTransform arrow;
    private int currentIndex;

    private void Awake()
    {
        arrow = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        currentIndex = 0;
        UpdateArrowPosition();
    }

    private void Update()
    {
        HandleNavigationInput();
        HandleInteractionInput();
    }

    private void HandleNavigationInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            ChangeSelection(-1);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            ChangeSelection(1);
        }
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void ChangeSelection(int direction)
    {
        currentIndex += direction;

        if (currentIndex < 0)
        {
            currentIndex = buttons.Length - 1;
        }
        else if (currentIndex >= buttons.Length)
        {
            currentIndex = 0;
        }

        PlaySound(changeSound);
        UpdateArrowPosition();
    }

    private void UpdateArrowPosition()
    {
        if (buttons.Length == 0) return;

        Vector3 targetPosition = arrow.position;

        targetPosition.y = buttons[currentIndex].position.y;

        arrow.position = targetPosition;
    }

    private void Interact()
    {
        PlaySound(interactSound);

        Button selectedButton = buttons[currentIndex].GetComponent<Button>();

        if (selectedButton != null)
        {
            selectedButton.onClick.Invoke();
        }
        else
        {
            Debug.LogWarning("Selected object does not contain a Button component.");
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (SoundManager.instance != null && clip != null)
        {
            SoundManager.instance.PlaySound(clip);
        }
    }
}