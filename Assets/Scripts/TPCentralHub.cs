using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPCentralHub : MonoBehaviour
{
    public Transform player;

    public Transform temas, centroIA, centro;

    public GameObject panelMapa;

    public void TpTemas()
    {
        player.position = temas.position;
        StartCoroutine(WaitAndOpenMap());

    }

    public void TpCentroIA()
    {
        player.position = centroIA.position;
        StartCoroutine(WaitAndOpenMap());
    }

    public void TpCentro()
    {
        player.position = centro.position;
        StartCoroutine(WaitAndOpenMap());
    }

    public void ButtonMapa()
    {
        GameManager gm = GameManager.Instance;
        gm.SetIsCanvasOpen(!panelMapa.activeSelf);
        panelMapa.SetActive(gm.IsCanvasOpen());
    }

    private IEnumerator WaitAndOpenMap()
    {
        yield return new WaitForSeconds(0.2f);
        ButtonMapa();
    }
}
