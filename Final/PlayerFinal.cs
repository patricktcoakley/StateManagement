using Godot;

namespace StatePatternExample.Final;

/// <summary>
///     This example code further abstracts the state code so that a dedicated state machine is able to handle
///     transitioning between the states, encapsulating the state management code in a dedicated class away from the
///     caller. Now the Player class doesn't  have to know anything about the different phases of states and can just
///     delegate that to the PlayerStateMachine. This allows for a separation of concerns.
/// </summary>
public partial class PlayerFinal : CharacterBody2D
{
    public const float Speed = 300.0f;
    public const float JumpVelocity = -650.0f;
    public const float DiveVelocity = -JumpVelocity * 0.75f;

    private Label _stateLabel;
    private PlayerStateMachine PlayerStateMachine;
    public AnimatedSprite2D Sprite2D { get; private set; }

    public float Direction => Input.GetAxis("move_left", "move_right");

    public override void _Ready()
    {
        Sprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        PlayerStateMachine = new PlayerStateMachine(this);
        _stateLabel = GetNode<Label>("Label");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsOnFloor())
        {
            Velocity += GetGravity() * (float)delta;
        }

        PlayerStateMachine.PhysicsProcess(delta);
        _stateLabel.Text = PlayerStateMachine.CurrentState?.Name;

        MoveAndSlide();
    }
}
