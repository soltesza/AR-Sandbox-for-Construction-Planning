using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserController : MonoBehaviour
{
    public Camera Main_Camera;
    public GameObject Canvas;
    public float speed = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        Canvas.gameObject.SetActive(false);

        if (!Main_Camera.isActiveAndEnabled)
        {
            Main_Camera.gameObject.SetActive(true);
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (Canvas.gameObject.activeSelf)
            {
                Canvas.gameObject.SetActive(false);
            }
            else
            {
                Canvas.gameObject.SetActive(true);
            }
                
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            Main_Camera.transform.Translate(new Vector3(0.0f, speed * Time.deltaTime, 0.0f));
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            Main_Camera.transform.Translate(new Vector3(0.0f, -speed * Time.deltaTime, 0.0f));
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            Main_Camera.transform.Translate(new Vector3(-speed * Time.deltaTime, 0.0f, 0.0f));
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            Main_Camera.transform.Translate(new Vector3(speed * Time.deltaTime, 0.0f, 0.0f));
        }

        if (Input.GetKey(KeyCode.PageDown))
        {
            Main_Camera.transform.Translate(new Vector3(0.0f, 0.0f, speed * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.PageUp))
        {
            Main_Camera.transform.Translate(new Vector3(0.0f, 0.0f, -speed * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            Main_Camera.transform.LookAt(Input.mousePosition);
        }
    }
}
