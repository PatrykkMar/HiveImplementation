using HiveGame.Core.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InGamePlayerMoveStateStrategy : IStateStrategy
{
    public enum PlayerMoveStateAction
    {
        None, PutInsect, MoveInsect
    }

    public PlayerMoveStateAction CurrentAction { get; set; }

    public InsectType? InsectToPut { get; set; }

    public VertexDTO? HexFromMove { get; set; }

    public List<ButtonHelper> GetAvailableButtonsList()
    {
        return new List<ButtonHelper>
        {
            //new ButtonHelper("Put insect", async () => await ServiceLocator.Services.HubService.PutInsectAsync(PlayerInsectView.ChosenInsect.Value, null)),
            //new ButtonHelper("Move insect", async () => await ServiceLocator.Services.HubService.MoveInsectAsync())
        };
    }

    public string InformationText => "Your move\nTo put an insect, click an insect button and choose one of the half-transparent hex on the board\nTo move an insect, click on a insect and choose one of the half-transparent hex on the board";

    public string Scene => Scenes.GameScene;

    public void OnInsectButtonClick(InsectType insect)
    {
        Debug.Log("OnInsectButtonClick InGamePlayermoveStateStrategy invoked");

        switch(CurrentAction)
        {
            case PlayerMoveStateAction.None:
            case PlayerMoveStateAction.MoveInsect:
            case PlayerMoveStateAction.PutInsect:
                if (insect != InsectType.Queen && !Board.Instance.QueenRuleMet)
                {
                    ServiceLocator.Services.EventAggregator.InvokeMinorInformationTextReceived("It's 4th turn, you have to put the queen now", 5);
                    Board.Instance.CancelHighlighing();
                    return;
                }
                Board.Instance.CancelHighlighing();
                Board.Instance.HighlightHexesToPutInsects();
                SetPlayerAction(PlayerMoveStateAction.PutInsect, insectToPut: insect);
            break;
        }
    }

    public void OnHexClick(VertexDTO hex)
    {
        Debug.Log("InGame onhexclick");
        switch (CurrentAction)
        {
            case PlayerMoveStateAction.None:
                if (!hex.isempty && hex.isthisplayerinsect)
                {
                    SetPlayerAction(PlayerMoveStateAction.MoveInsect, hex);
                    Board.Instance.CancelHighlighing();
                    Board.Instance.HighlightHexesToMoveInsects(hex);
                }
                break;
            case PlayerMoveStateAction.MoveInsect:

                if (hex.isempty && hex.highlighted)
                {
                    ServiceLocator.Services.HubService.MoveInsectAsync((HexFromMove.x, HexFromMove.y, HexFromMove.z), (hex.x, hex.y, hex.z));
                    SetPlayerAction(PlayerMoveStateAction.None);
                    Board.Instance.CancelHighlighing();
                }
                else if (!hex.isempty && hex.isthisplayerinsect)
                {
                    SetPlayerAction(PlayerMoveStateAction.MoveInsect, hex);
                    Board.Instance.CancelHighlighing();
                    Board.Instance.HighlightHexesToMoveInsects(hex);
                }
                else
                {
                    SetPlayerAction(PlayerMoveStateAction.None);
                }
                    break;
            case PlayerMoveStateAction.PutInsect:
                if(hex.highlighted)
                {
                    ServiceLocator.Services.HubService.PutInsectAsync(InsectToPut.Value, (hex.x, hex.y, hex.z));
                }
                else if(!hex.isempty && hex.isthisplayerinsect) 
                {
                    SetPlayerAction(PlayerMoveStateAction.MoveInsect, hexToMove: hex);
                }
                else
                {
                    SetPlayerAction(PlayerMoveStateAction.None);
                }
                Board.Instance.CancelHighlighing();
                break;
        }
    }

    public void SetPlayerAction(PlayerMoveStateAction action, VertexDTO hexToMove = null, InsectType? insectToPut = null)
    {
        switch (action)
        {
            case PlayerMoveStateAction.MoveInsect:
                if (hexToMove == null)
                {
                    ServiceLocator.Services.EventAggregator.InvokeMinorInformationTextReceived("Hex to move not chosen", 5);
                    return;
                }
                if(hexToMove != null && !string.IsNullOrEmpty(hexToMove.reasonwhymoveimpossible))
                {
                    ServiceLocator.Services.EventAggregator.InvokeMinorInformationTextReceived("You can't move this insect. " + hexToMove.reasonwhymoveimpossible, 5);
                    return;
                }
                break;
            case PlayerMoveStateAction.PutInsect:
                if (!insectToPut.HasValue)
                {
                    ServiceLocator.Services.EventAggregator.InvokeMinorInformationTextReceived("Insect to put not chosen", 5);
                    return;
                }
                break;
            case PlayerMoveStateAction.None:
                break;
        }
        ServiceLocator.Services.EventAggregator.InvokeMinorInformationTextReceived("", 0.1f);
        CurrentAction = action;
        HexFromMove = hexToMove;
        InsectToPut = insectToPut;

        Debug.Log("Action: " + Enum.GetName(typeof(PlayerMoveStateAction), action));
    }


    public void OnStateEntry()
    {
        SetPlayerAction(PlayerMoveStateAction.None);
    }
}
