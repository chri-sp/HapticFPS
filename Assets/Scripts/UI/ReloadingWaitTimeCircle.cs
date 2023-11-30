using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadingWaitTimeCircle : MonoBehaviour
{

    [SerializeField] private Slider slider;

    private bool isReloading = false;

    private Weapon weapon;
    private WeaponManager weaponManager;
    private Image fastReloadingImage;
    private Color initialReloadingColor;
    float lowerBoundRange;
    float higherBoundRange;
    private bool failedFastReloading = false;

    Coroutine countDownCoroutine;

    void Start()
    {
        weapon = GameObject.FindWithTag("Weapon").GetComponentInChildren<Weapon>();
        fastReloadingImage = transform.GetChild(2).GetChild(0).GetComponent<Image>();
        initialReloadingColor = fastReloadingImage.color;
        fastReloadingEvent();
        weaponManager = GameObject.FindWithTag("WeaponHolder").GetComponent<WeaponManager>();
        weaponManager.onWeaponChanged += weaponChanged;
    }

    void weaponChanged(Weapon newWeapon)
    {
        fastReloadingCompleted();
        weapon = newWeapon;
    }

    void Update()
    {
        if (slider.value > lowerBoundRange && slider.value < higherBoundRange)
        {
            changeColorFastReloading();
            if (Input.GetKeyDown("r"))
            {
                weapon.fastReloading();
                fastReloadingCompleted();
            }
        }
        else if (Input.GetKeyDown("r"))
        {
            fastReloadingFailed();
        }
        else if (!failedFastReloading)
            resetColor();
    }

    private void fastReloadingFailed()
    {
        lowerBoundRange = 1;
        higherBoundRange = 1;
        failedFastReloading = true;
        fastReloadingImage.color = new Color32(37, 150, 190, 255);
    }

    private void fastReloadingCompleted()
    {
        if (countDownCoroutine!= null)
            StopCoroutine(countDownCoroutine);
        fastReloadingEvent();
        failedFastReloading = false;
        resetColor();
        isReloading = false;
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        isReloading = false;
        slider.value = 0;
        fastReloadingEvent();
    }

    public void startReloadTime(Weapon newWeapon)
    {
        weapon = newWeapon;
        countDownCoroutine = StartCoroutine(countDown());
    }

    private void fastReloadingEvent()
    {
        //trovo il range per cui vale l'evento di ricarica rapida
        lowerBoundRange = Random.Range(.15f, .4f);
        higherBoundRange = Random.Range(.15f + lowerBoundRange, .25f + lowerBoundRange);
    }

    private void changeColorFastReloading()
    {
        fastReloadingImage.color = new Color32(255, 80, 80, 255);
    }

    private void resetColor()
    {
        fastReloadingImage.color = initialReloadingColor;
    }

    private IEnumerator countDown()
    {
        if (!isReloading)
        {
            isReloading = true;

            float duration = weapon.reloadTime;
            float normalizedTime = 0;
            while (normalizedTime <= 1f)
            {
                slider.value = normalizedTime;
                normalizedTime += Time.deltaTime / duration;
                yield return null;
            }

            slider.value = 1;
            fastReloadingCompleted();
        }
    }
}