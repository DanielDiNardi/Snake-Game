using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid
{
    private Vector2Int foodGridPosition;
    private int width;
    private int height;
    GameObject foodGO;
    private Snake snake;

    public LevelGrid(int width, int height){
        this.width = width;
        this.height = height;
    }

    public void Setup(Snake snake){
        this.snake = snake;

        SpawnFood();
    }
    private void SpawnFood(){
        do{
            foodGridPosition = new Vector2Int(Random.Range(-9, width - 1), Random.Range(-9, height - 1));
        }
        while(snake.GetFullSnakeGridPosition().IndexOf(foodGridPosition) != -1);

        foodGO = new GameObject("Food", typeof(SpriteRenderer));
        foodGO.GetComponent<SpriteRenderer>().sprite = GameAssets.inst.foodSprite;
        foodGO.transform.position = new Vector3(foodGridPosition.x, foodGridPosition.y);
    }

    public bool TrySnakeEatFood(Vector2Int snakeGridPosition){
        if(snakeGridPosition == foodGridPosition){
            Object.Destroy(foodGO);
            SpawnFood();

            return true;
        }
        else{
            return false;
        }
    }

    public Vector2Int ValidateGridPosition(Vector2Int gridPosition){
        if(gridPosition.x < -width + 1){
            gridPosition.x = width - 1;
        }
        if(gridPosition.y < -height + 1){
            gridPosition.y = height - 1;
        }
        if(gridPosition.x > width - 1){
            gridPosition.x = -width + 1;
        }
        if(gridPosition.y > height -1){
            gridPosition.y = -height + 1;
        }
        return gridPosition;
    }
}
