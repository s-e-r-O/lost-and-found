using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RadarUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text text;
    [SerializeField] private GameObject progressBar;
    [SerializeField] private Image progressBarProgress;
    [SerializeField] private Sprite searchingIcon;
    [SerializeField] private Sprite inactiveIcon;
    [SerializeField] private Sprite foundIcon;
    [SerializeField] private Sprite notFoundIcon;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Searching()
    {
        progressBar.SetActive(false);
        icon.sprite = searchingIcon;
        text.text = "SEARCHING";
    }

    public void Found()
    {
        icon.sprite = foundIcon;
        text.text = "FOUND!";
    }

    public void Cooldown(float duration)
    {
        icon.sprite = inactiveIcon;
        text.text = "";
        progressBar.SetActive(true);
        progressBarProgress.fillAmount = 0f;
        //DOTween.To(() => progressBarProgress.fillAmount, amount => progressBarProgress.fillAmount = amount, 1f, duration);
        progressBarProgress.DOFillAmount(1f, duration).SetEase(Ease.Linear);
    }

    public void NotFound()
    {
        icon.sprite = notFoundIcon;
        text.text = "NOT FOUND";
    }
}
