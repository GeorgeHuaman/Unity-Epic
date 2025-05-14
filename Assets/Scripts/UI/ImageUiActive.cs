using UnityEngine;

public class ImageUiActive : MonoBehaviour
{
    public GameObject imageWorld;
    public Transform player;
    float timer;
    void Update()
    {
        if (timer >= 0.5f)
        {
            if (Input.anyKeyDown)
            {
                gameObject.SetActive(false);
                timer = 0;
            }

            if (Input.touchCount > 0)
            {
                gameObject.SetActive(false);
                timer = 0;
            }
        }
        else
        {
            timer += Time.deltaTime;
        }


    }

}
