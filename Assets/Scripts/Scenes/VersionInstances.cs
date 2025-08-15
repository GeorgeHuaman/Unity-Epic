using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class VersionInstances : MonoBehaviour
{
    public int version; // 0 = Npcs Kids Genuine y 1 = Npcs Next Old y 2 = Ninguno
    public AssetReference _assetReferenceOne;
    public AssetReference _assetReferenceTwo;
    public GameObject gameObject; // El Objeto creado se instancia hacia esta variable
    public static VersionInstances instance;
    // Start is called before the first frame update
    void Start()
    {
        var asyncReferencOne = _assetReferenceOne.LoadAssetAsync<GameObject>();
        var asyncReferenctwo = _assetReferenceTwo.LoadAssetAsync<GameObject>();
        Instance();
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

    public void DeleteAddressableScene()
    {
        Addressables.ReleaseInstance(gameObject);
    }
}
