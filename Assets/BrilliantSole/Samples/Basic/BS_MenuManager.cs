using System.Linq;
using UnityEngine;

public class BS_MenuManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject backToMainMenuButton;
    public GameObject defaultMenu;
    public GameObject[] menus;

    public void ShowMenu(GameObject menuToShow)
    {
        if (!menus.Contains(menuToShow))
        {
            Debug.LogError("menuToShow is not a valid menu");
            return;
        }

        foreach (GameObject menu in menus)
        {
            menu.SetActive(false);
        }

        menuToShow.SetActive(true);
        mainMenu.SetActive(false);
        backToMainMenuButton.SetActive(true);
    }

    public void ShowMainMenu()
    {
        foreach (GameObject menu in menus)
        {
            menu.SetActive(false);
        }
        mainMenu.SetActive(true);
        backToMainMenuButton.SetActive(false);
    }

    public void Awake()
    {
        if (defaultMenu != null && menus.Contains(defaultMenu))
        {
            ShowMenu(defaultMenu);
        }
        else
        {
            ShowMainMenu();
        }
    }
}