using UnityEngine;

public class ImageUiActive : MonoBehaviour
{
    public GameObject imageWorld;
    public Transform player;
    float timer;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
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
            if (Vector3.Distance(player.transform.position, imageWorld.transform.position) >= 6f)
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
