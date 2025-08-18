using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class VersionInstances : MonoBehaviour
{
    [Tooltip("0 = Npcs Kids Genuine y 1 = Npcs Next Old y 2 = Ninguno")]
    public int version;
    public AssetReference _assetReferenceOne;
    public AssetReference _assetReferenceTwo;
    [HideInInspector]public GameObject gameObject; // El Objeto creado se instancia hacia esta variable
    public static VersionInstances instance;
    private int index;
    public SpriteRenderer cartel;
    public Image cartelImage;
    public Texture2D[] cartelTexture;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        var asyncReferencOne = _assetReferenceOne.LoadAssetAsync<GameObject>();
        var asyncReferenctwo = _assetReferenceTwo.LoadAssetAsync<GameObject>();
        version = ProgressLevelSystem.instance.currentNpcsNumber;
        index = ProgressLevelSystem.instance.currentCartelIndex-1;
        Debug.Log(index);
        Instance();
        SetCartel(index);
    }
    public void Instance()
    { 
        switch(version)
        {
            case 0:
                _assetReferenceOne.InstantiateAsync();
                break;
            case 1:
                _assetReferenceTwo.InstantiateAsync();
                break;
            case 2:
                Debug.Log("Escena sin Npcs");
                break;
        }
    }
    public void SetCartel(int index)
    {
        // Convierte la textura en un sprite
        Sprite sprite = Sprite.Create(
            cartelTexture[index],
            new Rect(0, 0, cartelTexture[index].width, cartelTexture[index].height),
            new Vector2(0.5f, 0.5f) // pivot al centro
        );
        Debug.Log(cartelTexture[index].name);
        cartel.sprite = sprite;
        cartelImage.sprite = sprite;
    }
    public void DeleteAddressableScene()
    {
        Addressables.ReleaseInstance(gameObject);
    }
}
