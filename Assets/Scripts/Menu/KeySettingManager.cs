using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CustomKeyCode
{
    LEFT,
    RIGHT,
    UP,
    DOWN,
    USE_ITEM,
    SIZE
}

public class InputManager
{
    public Dictionary<CustomKeyCode, KeyCode> keyDic = new Dictionary<CustomKeyCode, KeyCode>();

    public KeyCode[] defaultKeyCode = { KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S, KeyCode.Space };

    private InputManager()
    {
        for (int i = 0; i < (int)CustomKeyCode.SIZE; i++)
        {
            KeyCode keyCode = (KeyCode)PlayerPrefs.GetInt("CustomKeyCode" + i, (int)defaultKeyCode[i]);
            keyDic.Add((CustomKeyCode)i, keyCode);
        }
    }

    private static InputManager instance;
    public static InputManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new InputManager();
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }

}

public class KeySettingManager : MonoBehaviour
{
    public ToggleGroup toggleGroup;
    public List<KeySetComponent> keySets;

    private void OnEnable()
    {
        for (int i = 0; i < keySets.Count; i++)
        {
            keySets[i].keyCodeText.text = ((KeyCode)PlayerPrefs.GetInt("CustomKeyCode" + (int)keySets[i].action, (int)InputManager.Instance.defaultKeyCode[i])).ToString();
            
        }
    }

    private void Update()
    {
        if (Input.anyKeyDown && toggleGroup.AnyTogglesOn())
        {
            KeySetComponent selectedKeySet = null;

            for (int i = 0; i < keySets.Count; i++)
            {
                if (keySets[i].toggle.isOn)
                {
                    selectedKeySet = keySets[i];
                }
            }

            KeyCode pressedKeyCode = KeyCode.None;

            var allKeys = System.Enum.GetValues(typeof(KeyCode));
            foreach (KeyCode key in allKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    pressedKeyCode = key;
                }
            }

            selectedKeySet.keyCodeText.text = pressedKeyCode.ToString();

            PlayerPrefs.SetInt("CustomKeyCode" + (int)selectedKeySet.action, (int)pressedKeyCode);
            InputManager.Instance.keyDic[selectedKeySet.action] = pressedKeyCode;

            selectedKeySet.toggle.isOn = false;
        }
    }
}
