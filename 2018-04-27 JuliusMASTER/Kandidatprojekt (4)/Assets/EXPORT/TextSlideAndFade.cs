using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSlideAndFade : MonoBehaviour {

    public float Speed = 10;
    public float FadeOutSpeed = 0.1f;
    public Vector3 MoveToPosition = new Vector3(0, -10);
    public float distanceOffset = 1;
    public RectTransform RectTransform;
    public Text CustomText;

    private Vector3 startPos;

    private float timer = 0;
    private float duration = 2;
    public bool enableText = false;

    // Use this for initialization
    void Start()
    {
        startPos = CustomText.rectTransform.position;
    }

    private void Update()
    {
        if (enableText)
        {
            if (timer > 0f)
            {
                timer -= Time.deltaTime;
                RectTransform.position += Vector3.up * Time.deltaTime * Speed;
                CustomText.color -= new Color(0, 0, 0, Time.deltaTime/duration);

            }
            else
            {
                enableText = false;
                CustomText.gameObject.SetActive(false);
                CustomText.rectTransform.position = startPos;
                CustomText.color += new Color(0, 0, 0, 1);

            }
        }
    }

    public void startSlideAndFade()
    {
        timer = duration;
        enableText = true;
        CustomText.gameObject.SetActive(true);

    }
}
