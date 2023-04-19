using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInformation_UI : MonoBehaviour
{ 
    public Text Information;
    public Image health_slider;
    public Image exp_slider;


    private void Awake()
    {
        //health_slider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        //level_text = transform.GetChild(2).GetComponent<TextMeshPro>();
        //exp_slider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        health_slider.fillAmount = 1;
        exp_slider.fillAmount = 0;
    }

    void Update()
    {
        health_slider.fillAmount = Player_UI.life;
        exp_slider.fillAmount = Player_UI.exp;
        Information.text = $"{Player_UI.Name}  Level: {Player_UI.lv}  Hp: {Player_UI.actual_hp} / {Player_UI.Max_hp}  Exp: {Player_UI.actual_exp} / {Player_UI.Max_exp}";
    }

}
