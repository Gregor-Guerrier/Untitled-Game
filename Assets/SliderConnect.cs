using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderConnect : MonoBehaviour
{
    private Slider slider;
    private Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        slider = transform.parent.GetComponentInChildren<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = Mathf.Round(slider.value  * 10f)/10f + "";
    }
}
