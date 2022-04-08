using CodeMonkey;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using UnityEngine;
using CodeMonkey.Utils;

public class Level : MonoBehaviour
{
    private const float CAMERA_HALF_HEIGHT = 50f;
    private const float WALL_WIDTH = 8f;
    private const float WALL_WIDTH_STATIC = 200f;
    private const float WALL_SPEED = 25f;
    private const float WALL_DESTROY_X = -100f;
    private const float WALL_SPAWN_X = +100f;
    private const float BIRD_X_POS = 0f;
    private List<Wall> walls;
    private float wallSpawnTime;
    private float wallSpawnTimeMax;
    private float gapSize;
    private int wallsSpawned;
    private int wallsPassedBird;
    private State state;

    private static Level instance;

    public static Level getInstance()
    {
        return instance;
    }

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Impossible,
    }

    private enum State
    {
        WaitingToStart,
        Playing,
        BirdDead,
    }

    private void Awake()
    {
        instance = this;
        walls = new List<Wall>();
        wallSpawnTimeMax = 1.5f;
        setDifficulty(Difficulty.Easy);
        state = State.WaitingToStart;
    }
    private void Start()
    {
        createTopWall(5f, 0f, true);
        createTopWall(5f, 0f, false);
        Bird.getInstance().onDied += bird_onDied;
        Bird.getInstance().onStartedPlaying += bird_onStartPlaying;
    }

    private void bird_onDied(object sender, System.EventArgs e)
    {
        CMDebug.TextPopupMouse("Dead");
        state = State.BirdDead;
    }
    private void bird_onStartPlaying(object sender, System.EventArgs e)
    {
        state = State.Playing;
    }
    private void createTopWall(float height, float xPos, bool bottom)
    {
        //Make a Wall
        Transform wall = Instantiate(GameAssets.getInstance().pfWall);
        float wallYPos;
        if (bottom)
        {
            wallYPos = -CAMERA_HALF_HEIGHT -10f;
        }
        else
        {
            wallYPos = +CAMERA_HALF_HEIGHT +10f;
            wall.localScale = new Vector3(1, -1, 1);
        }
        wall.position = new Vector3(xPos, wallYPos);
        SpriteRenderer wallRender = wall.GetComponent<SpriteRenderer>();
        wallRender.size = new Vector2(WALL_WIDTH_STATIC, height);
        BoxCollider2D wallCollider = wall.GetComponent<BoxCollider2D>();
        wallCollider.size = new Vector2(WALL_WIDTH_STATIC, height);
        wallCollider.offset = new Vector2(0f, height * 0.5f);
    }

    private void createGapWall(float gapY, float gapSize, float xPos)
    {
        createWall(gapY - gapSize * 0.5f, xPos, true);
        createWall(CAMERA_HALF_HEIGHT * 2f - gapY - gapSize * 0.5f, xPos, false);
        wallsSpawned++;
        setDifficulty(getDifficulty());
    }
    private void Update()
    {
        if(state == State.Playing)
        {
            wallMovement();
            wallSpawning();
        }
    }

    private void wallSpawning()
    {
        wallSpawnTime -= Time.deltaTime;
        if(wallSpawnTime < 0)
        {
            wallSpawnTime += wallSpawnTimeMax;
            float heightEdgeLimit = 5f;
            float minHeight = gapSize * 0.5f + heightEdgeLimit;
            float totalHeight = CAMERA_HALF_HEIGHT * 2f;
            float maxHeight = totalHeight - gapSize * 0.5f - heightEdgeLimit;
            float height = Random.Range(minHeight, maxHeight);
            createGapWall(height, gapSize, WALL_SPAWN_X);
        }
    }

    private void wallMovement()
    {
        for (int i = 0; i < walls.Count; i++)
        {
            Wall wall = walls[i];
            bool isRightOfBird = wall.getX() > BIRD_X_POS;
            wall.move();
            if(isRightOfBird && wall.getX() <= BIRD_X_POS && wall.isBottom())
            {
                wallsPassedBird++;
            }
            if(wall.getX() < WALL_DESTROY_X)
            {
                //Destory wall
                wall.destroy();
                walls.Remove(wall);
                i--;
            }
        }
    }

    private void setDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                gapSize = 50f;
                break;
            case Difficulty.Medium:
                gapSize = 40f;
                break;
            case Difficulty.Hard:
                gapSize = 33f;
                break;
            case Difficulty.Impossible:
                gapSize = 25f;
                break;
        }
    }
    private Difficulty getDifficulty()
    {
        if (wallsSpawned > 30) return Difficulty.Impossible;
        if (wallsSpawned > 20) return Difficulty.Hard;
        if (wallsSpawned > 10) return Difficulty.Medium;
        return Difficulty.Easy;
    }
    private void createWall(float height, float xPos, bool bottom)
    {
        //Make a Wall
        Transform wall = Instantiate(GameAssets.getInstance().pfWall);
        float wallYPos;
        if (bottom)
        {
            wallYPos = -CAMERA_HALF_HEIGHT;
        }
        else
        {
            wallYPos = +CAMERA_HALF_HEIGHT;
            wall.localScale = new Vector3(1, -1, 1);
        }
        wall.position = new Vector3(xPos, wallYPos);
        SpriteRenderer wallRender = wall.GetComponent<SpriteRenderer>();
        wallRender.size = new Vector2(WALL_WIDTH, height);
        BoxCollider2D wallCollider = wall.GetComponent<BoxCollider2D>();
        wallCollider.size = new Vector2(WALL_WIDTH, height);
        wallCollider.offset = new Vector2(0f, height * 0.5f);

        Wall newwall = new Wall(wall, bottom);
        walls.Add(newwall);
    }

    public int getWallsSpawned()
    {
        return wallsSpawned;
    }
    public int getWallsPassed()
    {
        return wallsPassedBird;
    }

    private class Wall
    {
        private Transform wall;
        private bool IsBottom;

        public Wall(Transform wall, bool IsBottom)
        {
            this.wall = wall;
            this.IsBottom = IsBottom;
        }
        public void move()
        {
            wall.position += new Vector3(-1, 0, 0) * WALL_SPEED * Time.deltaTime;
        }

        public float getX()
        {
            return wall.position.x;
        }

        public bool isBottom()
        {
            return IsBottom;
        }

        public void destroy()
        {
            Destroy(wall.gameObject);
        }
    }
}
