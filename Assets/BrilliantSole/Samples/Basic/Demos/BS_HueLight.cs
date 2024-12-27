using UnityEngine;


[RequireComponent(typeof(HueLamp))]
public class BS_HueLight : MonoBehaviour
{
    private HueLamp HueLamp;

    void Start()
    {
        HueLamp = GetComponent<HueLamp>();
    }
}
