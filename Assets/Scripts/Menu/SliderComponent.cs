using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderComponent : MonoBehaviour
{
    public enum Type
    {
        BGM,
        UI,
        GAME
    }

    public Type type;
    public Text text;

    Slider slider;

    AudioSource bgmSource;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(delegate { ValidateSlider(); });

        slider.value = PlayerPrefs.GetInt("SoundSetting" + type, 5);
    }

    void ValidateSlider()
    {
        text.text = "" + slider.value;
        PlayerPrefs.SetInt("SoundSetting" + type, (int)slider.value);

        if (type == Type.BGM)
        {
            if (bgmSource == null)
            {
                bgmSource = GameObject.Find("BGM Source").GetComponent<AudioSource>();
            }

            bgmSource.volume = slider.value * 0.1f;
        }
    }
}
