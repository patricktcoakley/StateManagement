using Godot;
using System;
using System.Collections.Generic;

namespace StatePatternExample.Final;

/// <summary>
///     The StateMachine's primary purpose is to handle the registration and transitions of its various states. It uses
///     generics to use the type of state to retrieve the instance. You could do the same with any other method, such as
///     strings or enums, but
///     leveraging the type system has the added benefit of intellisense and correctness at the cost of slightly more
///     complicated code.
/// </summary>
public abstract class StateMachine<TOwner>(TOwner owner)
{
    protected Dictionary<Type, State<TOwner>> _states { get; init; } = new();
    public State<TOwner>? CurrentState { get; private set; }
    private TOwner Owner => owner;

    public void TransitionTo<TState>() where TState : State<TOwner>
    {
        var state = typeof(TState);
        if (!_states.TryGetValue(state, out var newState))
        {
            GD.PrintErr($"Could not find {typeof(TState).Name}"); // typically you might want to handle this instead of just logging it
            return;
        }

        if (CurrentState == newState)
        {
            return;
        }

        CurrentState = newState;
        CurrentState?.Enter(Owner);
    }

    public void PhysicsProcess(double delta)
    {
        CurrentState?.PhysicsProcess(Owner, delta);
    }
}

public class PlayerStateMachine : StateMachine<PlayerFinal>
{
    public PlayerStateMachine(PlayerFinal owner) : base(owner)
    {
        _states = new Dictionary<Type, State<PlayerFinal>>
        {
            { typeof(StandingState), new StandingState(this) },
            { typeof(JumpingState), new JumpingState(this) },
            { typeof(DuckingState), new DuckingState(this) },
            { typeof(DivingState), new DivingState(this) },
            { typeof(MovingState), new MovingState(this) }
        };

        TransitionTo<StandingState>();
    }
}
