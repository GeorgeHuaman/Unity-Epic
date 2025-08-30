using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public static UIManager singleton
    {
        get => _singleton;
        set
        {
            if (value == null)
                _singleton = null;
            else if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Destroy(value);
            }
        }
    }

    private static UIManager _singleton;
    [SerializeField] private TextMeshProUGUI gameStateText;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private LeaderBoardItem[] leaderBoardItems;

    private void Awake()
    {
        singleton = this;
    }
    private void OnDestroy()
    {
        if (singleton == this)
            singleton = null;
    }

    public void DidSetReady()
    {
        instructionText.text = "Waiting for other player to be ready";
    }
    public void SetWait(GameState state, PlayerFusion win)
    {
        if(state == GameState.Wait)
        {
            if(win == null)
            {
                //TEXTO que pasa
            }
            else
            {
                //texto que pasa
            }
        }
    }

    public void UpdateLeaderBoard(KeyValuePair<Fusion.PlayerRef, PlayerFusion>[] player)
    {
        for(int i = 0;i < leaderBoardItems.Length;i++)
        {
            LeaderBoardItem item = leaderBoardItems[i];
            if(i< player.Length)
            {
                item.nameText.text = player[i].Value.name;
                item.hightText.text = $"{player[i].Value.score}M";
            }
            else
            {
                item.nameText.text = "";
                item.hightText.text = "";

            }
        }
    }
    [Serializable]
    public struct LeaderBoardItem
    {
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI hightText;
    }
}
