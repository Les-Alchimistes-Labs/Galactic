using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInformation_UI : MonoBehaviour
{ 
    Text level_text;
    Image health_slider;
    Image exp_slider;
    public float slider_life;
    public float slider_exp;
    public int level;
    

    private void Awake()
    {
        health_slider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        level_text = transform.GetChild(2).GetComponent<Text>();
        exp_slider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        slider_life = 0;
        slider_exp = 0;
        level = 0;
    }

    void Update()
    {
        health_slider.fillAmount = slider_life;
        exp_slider.fillAmount = slider_exp;
        level_text.text = $"Level: {level}";
        

    }

}
