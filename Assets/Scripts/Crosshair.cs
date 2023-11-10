﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Crosshair : MonoBehaviour
{

    [Range(0, 100)]
    public float Value;
    public float speed;

    public float margin;
    public float multiplier;

    public RectTransform Top, Bottom, Left, Right, Center;

    private float crosshairSize;

    public void setCrosshairSize(float spreadFactor)
    {
        crosshairSize = spreadFactor * 1000;
    }

    void Start() {
        setMargin();
    }


    // Update is called once per frame
    void Update()
    {

        Value = crosshairSize * multiplier;

        float TopValue, BottomValue, LeftValue, RightValue;

        TopValue = Mathf.Lerp(Top.position.y, Center.position.y + margin + Value, speed * Time.deltaTime);
        BottomValue = Mathf.Lerp(Bottom.position.y, Center.position.y - margin - Value, speed * Time.deltaTime);

        LeftValue = Mathf.Lerp(Left.position.x, Center.position.x - margin - Value, speed * Time.deltaTime);
        RightValue = Mathf.Lerp(Right.position.x, Center.position.x + margin + Value, speed * Time.deltaTime);

        Top.position = new Vector2(Top.position.x, TopValue);
        Bottom.position = new Vector2(Bottom.position.x, BottomValue);

        Left.position = new Vector2(LeftValue, Center.position.y);
        Right.position = new Vector2(RightValue, Center.position.y);
    }

    private void setMargin() { 
        float startResolution = 2560;

        float percentage = (Screen.width / startResolution) * 100;

        float surplusValue = (1 - (percentage / 100)) * margin;

        margin = margin - surplusValue;
    }
}
