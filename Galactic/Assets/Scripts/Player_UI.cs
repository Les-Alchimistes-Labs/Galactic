using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player_UI : MonoBehaviour
{
    private PhotonView view;
    private Player2 _player2;
    public Canvas canvas;
    
    void Start()
    {
        view = GetComponent<PhotonView>();
        _player2 = GetComponent<Player2>();   
    }

    
    void Update()
    {
        if (view.IsMine)
        {
            canvas.GetComponent<PlayerInformation_UI>().slider_life  = (float)_player2.Personnage.Getlife / _player2.Personnage.GetMaxLife();
            canvas.GetComponent<PlayerInformation_UI>().slider_exp =
                (float)_player2.Personnage.GetXP() / _player2.Personnage.Maxxp;
            canvas.GetComponent<PlayerInformation_UI>().level = _player2.Personnage.level;
        }

        
    }
}
