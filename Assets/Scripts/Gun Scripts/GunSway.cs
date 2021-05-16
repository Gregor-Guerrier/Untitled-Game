using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSway : MonoBehaviour
{
    public float amount;
    public float maxAmount;
    public float smoothAmount;
    private Vector3 initialPosition;
    private MouseLook mouseLook;
    // Start is called before the first frame update
    void Start()
    {
        mouseLook = GameObject.FindObjectOfType<MouseLook>();
        initialPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        float movementX = -Input.GetAxis("Mouse X") * amount * (mouseLook.mouseSensitivity/300);
        float movementY = -Input.GetAxis("Mouse Y") * amount * (mouseLook.mouseSensitivity/300);
        movementX = Mathf.Clamp(movementX, -maxAmount, maxAmount);
        movementY = Mathf.Clamp(movementY, -maxAmount, maxAmount);
        Vector3 finalPosition = new Vector3(movementX, movementY, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * smoothAmount);
    }
}
