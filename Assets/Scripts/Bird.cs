using System;
using UnityEngine;

public class Bird : MonoBehaviour
{
    // Start is called before the first frame update
    private const float JUMP_FORCE = 100f;
    private Rigidbody2D rb;
    public event EventHandler onDied;
    public event EventHandler onStartedPlaying;
    public static Bird instance;
    private State state;

    private enum State
    {
        WatingToStart,
        Playing,
        Dead,
    }

    public static Bird getInstance()
    {
        return instance;
    }
    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
    }
    private void Update()
    {
        switch (state)
        {
            default:
            case State.WatingToStart:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    state = State.Playing;
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    jump();
                    if (onStartedPlaying != null) onStartedPlaying(this, EventArgs.Empty);
                }
                break;
            case State.Playing:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    jump();
                }
                break;
            case State.Dead:
                break;
        }
    }

    private void jump()
    {
        rb.velocity = Vector2.up * JUMP_FORCE;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        rb.bodyType = RigidbodyType2D.Static;
        if (onDied != null) onDied(this, EventArgs.Empty);
    }

}
