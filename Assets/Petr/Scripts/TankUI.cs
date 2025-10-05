using UnityEngine;
using UnityEngine.UI;


public class TankUI : MonoBehaviour
{
    [SerializeField] Slider slider;

    // Update is called once per frame
    public void UpdateSlider()
    {
        slider.value++;
    }
}
