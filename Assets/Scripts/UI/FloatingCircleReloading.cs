using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingCircleReloading : MonoBehaviour {

    [SerializeField] private Slider slider;

    public void UpdateReloadingCircle(int ammo, int maxammo)
    {
        slider.value = (float) ammo/maxammo;

        if (slider.value <= 0)
        {
            slider.value = 0;
        }
    }
}
