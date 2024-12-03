using UnityEngine;

public class BS_MenuManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject backToMainMenuButton;
    public GameObject[] menus;

    public void ShowMenu(GameObject menuToShow)
    {
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
}