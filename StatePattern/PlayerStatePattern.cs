using Godot;

namespace StatePatternExample.StatePattern;

/// <summary>
///     In comparison to the previous, we are able to separate the state management logic
///     into its own base class to create a reusable container of sorts. This lets a developer create new states
///     independently of any other while still allowing them to transition between one another.
/// </summary>
public partial class PlayerStatePattern : CharacterBody2D
{
    public const float Speed = 300.0f;
    public const float JumpVelocity = -650.0f;
    public const float DiveVelocity = -JumpVelocity * 0.75f;

    public AnimatedSprite2D Sprite2D { get; private set; }
    private State? _currentState { get; set; }

    public float Direction => Input.GetAxis("move_left", "move_right");

    public override void _Ready()
    {
        Sprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _currentState = new StandingState();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsOnFloor())
        {
            Velocity += GetGravity() * (float)delta;
        }

        var newState = _currentState?.PhysicsProcess(this, delta);
        if (newState is not null && newState != _currentState)
        {
            _currentState = newState;
            _currentState.Enter(this);
        }

        MoveAndSlide();
    }
}
