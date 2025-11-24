using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public List<MenuWindow> windows;

    public void ChangeWindow(string windowName)
    {
        foreach (var menu in windows)
        {
            if (menu.windowObject == null)
                Debug.LogError("Menu window is missing or not exist!");

            menu.windowObject.SetActive(menu.windowName == windowName);
        }
    }
}

[System.Serializable]
public class MenuWindow
{
    public string windowName;
    public GameObject windowObject;
}
