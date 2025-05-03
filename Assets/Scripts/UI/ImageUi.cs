using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageUi : MonoBehaviour
{
    public static ImageUi instance;
    public GameObject imageWorldParent;
    public GameObject imageUiParent;
    public List<Images> images = new List<Images>();

    private void Start()
    {
        if (imageWorldParent.transform.childCount != imageUiParent.transform.childCount)
        {
            Debug.LogError("Los objetos hijos de imageWorldParent e imageUiParent no coinciden en cantidad.");
            return;
        }
        images.Clear();
        for (int i = 0; i < imageWorldParent.transform.childCount; i++)
        {
            GameObject worldChild = imageWorldParent.transform.GetChild(i).gameObject;
            GameObject uiChild = imageUiParent.transform.GetChild(i).gameObject;

            Images newImage = new Images
            {
                name = uiChild.name,
                imageWorld = worldChild,
                imageUi = uiChild
            };

            // Añade la instancia a la lista
            images.Add(newImage);
        }
        instance = this;
    }
    public void OpenWordUi(string name)
    {
        foreach (Images image in images)
        {
            if (name == image.name)
            {
                image.imageUi.SetActive(true);
                image.imageUi.GetComponent<ImageUiActive>().imageWorld = image.imageWorld;
            }
        }
    }

    [System.Serializable]
    public class Images
    {
        public string name;
        public GameObject imageWorld;
        public GameObject imageUi;
    }
}
