using System;
using UnityEngine;
using UnityEngine.UI;

public class RestartButton : MonoBehaviour
{
    private Image restartButtonImage;
    [SerializeField] private Sprite normalRestartButtonSprite;
    [SerializeField] private Sprite deadRestartButtonSprite;
    [SerializeField] private Sprite coolRestartButtonSprite;

    
    void Start()
    {
        restartButtonImage = gameObject.GetComponent<Image>();

    }

    public void ChangeSprite(GameLogic.MinesweeperGameState gameState)
    {
        switch (gameState)
        {
            case GameLogic.MinesweeperGameState.NORMAL:
                restartButtonImage.sprite = normalRestartButtonSprite;
                break;
            case GameLogic.MinesweeperGameState.WIN:
                restartButtonImage.sprite = coolRestartButtonSprite;
                break;
            case GameLogic.MinesweeperGameState.LOSE:
                restartButtonImage.sprite = deadRestartButtonSprite;
                break;
        }
    }
    
}
