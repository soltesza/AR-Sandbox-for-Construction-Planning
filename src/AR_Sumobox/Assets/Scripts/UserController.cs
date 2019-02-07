using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserController : MonoBehaviour
{
    public Camera Topdown_Camera;
    public Camera NetworkManager_Camera;

    // Start is called before the first frame update
    void Start()
    {
        Topdown_Camera.gameObject.SetActive(false);
        NetworkManager_Camera.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (NetworkManager_Camera.isActiveAndEnabled)
            {
                Topdown_Camera.gameObject.SetActive(true);
                NetworkManager_Camera.gameObject.SetActive(false);
            }
            else
            {
                Topdown_Camera.gameObject.SetActive(false);
                NetworkManager_Camera.gameObject.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (Camera.current == Topdown_Camera)
            {
                Vector3 currpos = Topdown_Camera.transform.position;
                Vector3 newpos = new Vector3(currpos.x, currpos.y, currpos.z + 1.0f);
                Topdown_Camera.transform.position = newpos;
            }
            else
            {
                Vector3 currpos = NetworkManager_Camera.transform.position;
                Vector3 newpos = new Vector3(currpos.x, currpos.y + 1.0f, currpos.z);
                NetworkManager_Camera.transform.position = newpos;
            }

        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (Camera.current == Topdown_Camera)
            {
                Vector3 currpos = Topdown_Camera.transform.position;
                Vector3 newpos = new Vector3(currpos.x, currpos.y, currpos.z - 1.0f);
                Topdown_Camera.transform.position = newpos;
            }
            else
            {
                Vector3 currpos = NetworkManager_Camera.transform.position;
                Vector3 newpos = new Vector3(currpos.x, currpos.y - 1.0f, currpos.z);
                NetworkManager_Camera.transform.position = newpos;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (Camera.current == Topdown_Camera)
            {
                Vector3 currpos = Topdown_Camera.transform.position;
                Vector3 newpos = new Vector3(currpos.x - 1.0f, currpos.y, currpos.z);
                Topdown_Camera.transform.position = newpos;
            }
            else
            {
                Vector3 currpos = NetworkManager_Camera.transform.position;
                Vector3 newpos = new Vector3(currpos.x - 1.0f, currpos.y, currpos.z);
                NetworkManager_Camera.transform.position = newpos;
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (Camera.current == Topdown_Camera)
            {
                Vector3 currpos = Topdown_Camera.transform.position;
                Vector3 newpos = new Vector3(currpos.x + 1.0f, currpos.y, currpos.z);
                Topdown_Camera.transform.position = newpos;
            }
            else
            {
                Vector3 currpos = NetworkManager_Camera.transform.position;
                Vector3 newpos = new Vector3(currpos.x + 1.0f, currpos.y, currpos.z);
                NetworkManager_Camera.transform.position = newpos;
            }
        }
    }
}
