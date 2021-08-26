using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private Snake snake;
    private LevelGrid levelGrid;

    private Vector2 vec = new Vector2(-537, -300);
    void Start()
    {
        Debug.Log("GameHandler.Start");

        levelGrid = new LevelGrid(10, 10);

        snake.Setup(levelGrid);
        levelGrid.Setup(snake);

        CMDebug.ButtonUI(vec, "Restart", () => {
                Loader.Load(Loader.Scene._Scene_Game);
        });
    }
}
