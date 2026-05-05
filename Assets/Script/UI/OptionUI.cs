using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
     public static OptionUI Instance { get; private set; }
     private const string PLAYER_PREFS_QUALITY = "quality";
     private const string PLAYER_PREFS_RESOLUTION = "resolution";

    [SerializeField] private Button soundEffectsButton;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Button MusicButton;
    [SerializeField] private Slider MusicSlider;

    [SerializeField] private Button CloseButton;
    [SerializeField] private Button MoveUpButton;
    [SerializeField] private Button MoveDownButton;
    [SerializeField] private Button MoveLeftButton;
    [SerializeField] private Button MoveRightButton;
    [SerializeField] private Button InteractButton;
    [SerializeField] private Button InteractAlternateButton;
    [SerializeField] private Button PauseButton;
    [SerializeField] private Button DashButton;
    [SerializeField] private Button GamepadInteractButton;
    [SerializeField] private Button GamepadInteractAlternateButton;
    [SerializeField] private Button GamepadPauseButton;
    [SerializeField] private Button GamePadDashButton;
    [SerializeField] private Button ResetToDefaultButton;

    [SerializeField] private TextMeshProUGUI MoveUpText;
    [SerializeField] private TextMeshProUGUI MoveDownText;
    [SerializeField] private TextMeshProUGUI MoveLeftText;
    [SerializeField] private TextMeshProUGUI MoveRightText;
    [SerializeField] private TextMeshProUGUI InteractText;
    [SerializeField] private TextMeshProUGUI InteractAlternateText;
    [SerializeField] private TextMeshProUGUI PauseText;
    [SerializeField] private TextMeshProUGUI DashText;
    [SerializeField] private TextMeshProUGUI GamepadInteractText;
    [SerializeField] private TextMeshProUGUI GamepadInteractAlternateText;
    [SerializeField] private TextMeshProUGUI GamepadPauseText;
    [SerializeField] private TextMeshProUGUI GamepadDashText;

    [SerializeField] private Transform PressToRebindingTransform;

    private Action onCloseButtonAction;

    [SerializeField] private TMP_Dropdown resolutionsDropDown;
    [SerializeField] private TMP_Dropdown qualityDropdown;

    Resolution[] resolutions;
    private void Awake()
    {
        Instance = this;
        soundEffectsButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.ToggleSound();
            UpdateVisual();
        });

        MusicButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ToggleMusic();
            UpdateVisual();
        });
        CloseButton.onClick.AddListener(() =>
        {
            Hide();
            onCloseButtonAction();
        });
        ResetToDefaultButton.onClick.AddListener(() =>
        {
            ResetToDefault();
        });

        MoveUpButton.onClick.AddListener(()=> { RebindBinding(GameInput.Binding.Move_Up);});
        MoveDownButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Move_Down); });
        MoveLeftButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Move_Left); });
        MoveRightButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Move_Right); });
        InteractButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Interact); });
        InteractAlternateButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.InteractAlternate); });
        PauseButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Pause); });
        DashButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Dash); });

        GamepadInteractButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.GamePad_Interact); });
        GamepadInteractAlternateButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.GamePad_InteractAlternate); });
        GamepadPauseButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.GamePad_Pause); });
        GamePadDashButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.GamePad_Dash); });
    }

    private void Start()
    {
        KitchenGameManager.Instance.OnLocalGameUnPause += KitchenGameManager_OnGameResume;
        ShowResolutionDropDown();

        int savedQuality = PlayerPrefs.GetInt(PLAYER_PREFS_QUALITY, 2);
        qualityDropdown.value = savedQuality;
        QualitySettings.SetQualityLevel(savedQuality);

        UpdateVisual();
        HidePressToRebindingKey();
        Hide();
    }

    private void KitchenGameManager_OnGameResume(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void UpdateVisual()
    {
        MoveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        MoveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down);
        MoveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
        MoveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);
        InteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        InteractAlternateText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternate);
        PauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
        DashText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Dash);

        GamepadInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamePad_Interact);
        GamepadInteractAlternateText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamePad_InteractAlternate);
        GamepadPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamePad_Pause);
        GamepadDashText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamePad_Dash);
    }

    private void ShowResolutionDropDown()
    {
        resolutions = Screen.resolutions;
        resolutionsDropDown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionsDropDown.AddOptions(options);
        resolutionsDropDown.value = currentResolutionIndex;
        resolutionsDropDown.RefreshShownValue();
    }
    public void Show(Action onCloseButtonAction)
    {
        this.onCloseButtonAction = onCloseButtonAction;
        gameObject.SetActive(true);

        ResetToDefaultButton.Select();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ShowPressToRebindingKey()
    {
        PressToRebindingTransform.gameObject.SetActive(true);
    }

    private void HidePressToRebindingKey()
    {
        PressToRebindingTransform.gameObject.SetActive(false);
    }

    private void RebindBinding(GameInput.Binding biding)
    {
        ShowPressToRebindingKey();
        GameInput.Instance.RebindBinding(biding, () =>
        {
            HidePressToRebindingKey();
            UpdateVisual();
        });
    }

    public void ResetToDefault()
    {
        GameInput.Instance.ResetBindingsToDefault();

        UpdateVisual();
    }
    public void SetFullScreen(bool IsFullScreen)
    {
        Screen.fullScreen = IsFullScreen;
    }
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt(PLAYER_PREFS_QUALITY, qualityIndex);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt(PLAYER_PREFS_RESOLUTION, resolutionIndex);
    }

    public void SetSoundVolume()
    {
        SoundManager.Instance.SetVolume(soundSlider.value);
    }

    public void SetMusicVolume()
    {
        MusicManager.Instance.SetVolume(MusicSlider.value);
    }
}
