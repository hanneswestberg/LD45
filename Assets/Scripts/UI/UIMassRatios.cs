using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMassRatios : MonoBehaviour
{
    [SerializeField]
    private RectTransform background;
    [SerializeField]
    private RectTransform regularMatter;
    [SerializeField]
    private RectTransform antiMatter;
    [SerializeField]
    private RectTransform darkMatter;

    // Update is called once per frame
    void Update()
    {
        if(BangManager.instance.CurrentPhase != null) {
            regularMatter.sizeDelta = new Vector2(-24f, BangManager.instance.CurrentMassRatioRegular * 256f);
            antiMatter.sizeDelta = new Vector2(-24f, BangManager.instance.CurrentMassRatioAnti * 256f);
            darkMatter.sizeDelta = new Vector2(-24f, BangManager.instance.CurrentMassRatioDark * 256f);

            antiMatter.anchoredPosition = new Vector2(12f, - 12f - regularMatter.sizeDelta.y);
            darkMatter.anchoredPosition = new Vector2(12f, - 12f - regularMatter.sizeDelta.y - antiMatter.sizeDelta.y);
        }
    }
}
