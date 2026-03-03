using System;
using StarterAssets;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    State currentState;
    PlayerInputs input;

    public Animator animator;
    public PlayerMover mover;
    public string _currentState;

    Health health;
    MovementState movementState;

    void Awake()
    {
        movementState = GetComponent<MovementState>();
        currentState = movementState;
        input = GetComponent<PlayerInputs>();
        health = GetComponent<Health>();
    }

    void OnEnable()
    {
        health.OnDead.AddListener(OnDead);
    }

    void OnDisable()
    {
        health.OnDead.RemoveListener(OnDead);
    }

    void Update()
    {
        State nextState = currentState.UpdateState(this, input);
        if (nextState != null)
        {
            currentState.Exit(this);
            currentState = nextState;
            currentState.Enter(this);
        }
        _currentState = currentState.GetType().ToString();
    }

    private void OnDead()
    {
        if (currentState != null)
        {
            currentState.Exit(this);
        }
        currentState = movementState;
        currentState.Enter(this);
        _currentState = currentState.GetType().ToString();
        animator.Rebind();
        animator.Update(0f);
        mover.SetHorizontalVelocity(Vector3.zero);
        this.enabled = false;
    }

    void LateUpdate()
    {
        if (currentState != null) currentState.LateUpdateState(this);
        animator.SetFloat("MoveInput", input.move.magnitude);
    }

    public State GetCurrentState()
    {
        return currentState;
    }
}