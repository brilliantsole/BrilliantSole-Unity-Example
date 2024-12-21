using UnityEngine;

public class BS_DemoManager : MonoBehaviour
{
    public GameObject[] demos;

    public void ShowDemo(GameObject demoToShow)
    {
        foreach (GameObject demo in demos)
        {
            demo.SetActive(false);
        }

        if (!demoToShow.activeSelf)
        {
            demoToShow.SetActive(true);
        }

    }
}
