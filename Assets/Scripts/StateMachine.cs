using System;
using UnityEngine;
using UnityEngine.Experimental.XR;

public class StateMachine<T>
{
    private State<T> currentState;

    public StateMachine(State<T> state, T owner)
    {
        SetState(state, owner);
    }

    public void SetState(State<T> state, T owner)
    {
        currentState = state;
        currentState.Enter(owner);
    }

    
    
    public void ChangeState(State<T> newState, T owner)
    {
        currentState?.Exit(owner);
        currentState = newState;
        currentState.Enter(owner);
    }

    // Physics Update
    public void OnFixedUpdate(T owner)
    {
        currentState.FixedUpdate(owner);
    }
    
    public void OnCollisionEnter(Collider2D other, T owner)
    {
        currentState.OnCollisionEnter(other, owner);
    }

    public void OnAnimationEnd(string clipName, T owner)
    {
        currentState.OnAnimationEnd(clipName, owner);
    }
}

public abstract class State<T>
{
    protected void print(object message) => Debug.Log(message);

    public abstract void Enter(T owner);
    public abstract void FixedUpdate(T owner);
    public abstract void Exit(T owner);

    public abstract void OnCollisionEnter(Collider2D other, T owner);

    public abstract void OnAnimationEnd(string clipName, T owner);
}