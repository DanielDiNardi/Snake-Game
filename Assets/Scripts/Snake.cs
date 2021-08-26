using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey;
using CodeMonkey.Utils;


public class Snake : MonoBehaviour
{
    private enum Direction{
        Left,
        Right,
        Up,
        Down
    }
    private enum State{
        Alive,
        Dead
    }
    private State state;
    private Direction gridMoveDirection;
    private Vector2Int gridPosition;
    private float gridMoveTimer;
    private float gridMoveTimerMax;
    private LevelGrid levelGrid;
    private int snakeBodySize;
    private List<SnakeMovePosition> snakeMovePositionList;
    private List<SnakeBodyPart> snakeBodyPartList;


    public void Setup(LevelGrid levelGrid){
        this.levelGrid = levelGrid;
    }

    private void Awake(){
        gridPosition = new Vector2Int(0, 0);
        gridMoveTimerMax = 0.3f;
        gridMoveTimer = gridMoveTimerMax;
        gridMoveDirection = Direction.Right;

        snakeMovePositionList = new List<SnakeMovePosition>();
        snakeBodySize = 0;

        snakeBodyPartList = new List<SnakeBodyPart>();

        state = State.Alive;
    }   

    public Text scoreGT;
    void Start()
    {
        GameObject scoreGO = GameObject.Find("ScoreCount");
        scoreGT = scoreGO.GetComponent<Text>();
        scoreGT.text = "0";
    } 

    private void Update(){
        switch(state){
            case State.Alive:
                HandleInput();
                HandleGridMovement();
                break;
            case State.Dead:
                break;
        }
        
    }

    private void HandleInput(){
        if(Input.GetKeyDown("w")){
            if(gridMoveDirection != Direction.Down){
                gridMoveDirection = Direction.Up; 
            }
        }
        if(Input.GetKeyDown("a")){
            if(gridMoveDirection != Direction.Right){
                gridMoveDirection = Direction.Left;
            }
        }
        if(Input.GetKeyDown("s")){
            if(gridMoveDirection != Direction.Up){
                gridMoveDirection = Direction.Down;
            }
        }
        if(Input.GetKeyDown("d")){
            if(gridMoveDirection != Direction.Left){
                gridMoveDirection = Direction.Right;
            }
        }
    }

    private void HandleGridMovement(){
        gridMoveTimer += Time.deltaTime;
        if(gridMoveTimer >= gridMoveTimerMax){
            gridMoveTimer -= gridMoveTimerMax;

            SnakeMovePosition snakeMovePosition = new SnakeMovePosition(gridPosition, gridMoveDirection);
            snakeMovePositionList.Insert(0, snakeMovePosition);

            Vector2Int gridMoveDirectionVector;
            switch(gridMoveDirection){
                default:
                case Direction.Right:
                    gridMoveDirectionVector = new Vector2Int(1, 0);
                    break;
                case Direction.Left:
                    gridMoveDirectionVector = new Vector2Int(-1, 0);
                    break;
                case Direction.Up:
                    gridMoveDirectionVector = new Vector2Int(0, 1);
                    break;
                case Direction.Down:
                    gridMoveDirectionVector = new Vector2Int(0, -1);
                    break;
                
            }

            gridPosition += gridMoveDirectionVector;

            gridPosition = levelGrid.ValidateGridPosition(gridPosition);

            bool snakeAteFood = levelGrid.TrySnakeEatFood(gridPosition);
            if(snakeAteFood){
                snakeBodySize++;

                int score = int.Parse(scoreGT.text);
                score += 100;
                scoreGT.text = score.ToString();

                // Tracks the highscore
                if(score > HighScore.score){
                    HighScore.score = score;
            }

                CreateSnakeBodyPart();
            }

            if(snakeMovePositionList.Count >= snakeBodySize + 1){
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }

            UpdateSnakeBodyParts();

            foreach(SnakeBodyPart snakeBodyPart in snakeBodyPartList){
                Vector2Int snakeBodyPartGridPosition = snakeBodyPart.GetGridPosition();

                if(gridPosition == snakeBodyPartGridPosition){
                    print("Game Over");
                    state = State.Dead;
                }
            }

            transform.position = new Vector3(gridPosition.x, gridPosition.y);
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector) - 90);
        }
    }

    private void CreateSnakeBodyPart(){
        snakeBodyPartList.Add(new SnakeBodyPart(snakeBodyPartList.Count));
    }

    private void UpdateSnakeBodyParts(){
        for(int i = 0; i < snakeBodyPartList.Count; i++){
            snakeBodyPartList[i].SetSnakeMovePosition(snakeMovePositionList[i]);
        }
    }

    private float GetAngleFromVector(Vector2Int dir){
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if(n < 0){
            n += 360;
            return n;
        }
        return n;
    }

    public List<Vector2Int> GetFullSnakeGridPosition(){
        List<Vector2Int> gridPositionList = new List<Vector2Int>() {gridPosition};
        foreach(SnakeMovePosition snakeMovePosition in snakeMovePositionList){
            gridPositionList.Add(snakeMovePosition.GetGridPosition());
        }
        return gridPositionList;
    }

    private class SnakeBodyPart{
        private SnakeMovePosition snakeMovePosition;
        private Transform transform;

        public SnakeBodyPart(int bodyIndex){
            GameObject snakeBodyGO = new GameObject("SnakeBody", typeof(SpriteRenderer));
            snakeBodyGO.GetComponent<SpriteRenderer>().sprite = GameAssets.inst.snakeBodySprite;
            snakeBodyGO.GetComponent<SpriteRenderer>().sortingOrder = -bodyIndex;
            transform = snakeBodyGO.transform;
        }

        public void SetSnakeMovePosition(SnakeMovePosition snakeMovePosition){
            this.snakeMovePosition = snakeMovePosition;
            transform.position = new Vector3(snakeMovePosition.GetGridPosition().x, snakeMovePosition.GetGridPosition().y);

            float angle;
            switch(snakeMovePosition.GetDirection()){
                default:
                case Direction.Up:
                    angle = 0;
                    break;
                case Direction.Down:
                    angle = 180;
                    break;
                case Direction.Left:
                    angle = -90;
                    break;
                case Direction.Right:
                    angle = 90;
                    break;
                
            }

            transform.eulerAngles = new Vector3(0, 0, angle);
        }

        public Vector2Int GetGridPosition(){
            return snakeMovePosition.GetGridPosition();
        }
    }

    private class SnakeMovePosition{
        // private SnakeMovePosition previousSnakeMovePosition;
        private Vector2Int gridPosition;
        private Direction direction;

        public SnakeMovePosition(Vector2Int gridPosition, Direction direction){
            // this.previousSnakeMovePosition = previousSnakeMovePosition;
            this.gridPosition = gridPosition;
            this.direction = direction;
        }

        public Vector2Int GetGridPosition(){
            return gridPosition;
        }

        public Direction GetDirection(){
            return direction;
        }
    }
}
