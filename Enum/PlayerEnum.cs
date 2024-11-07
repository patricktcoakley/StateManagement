using Godot;

namespace StatePatternExample.Enum;

/// <summary>
///     Building off of the previous example,`PlayerBasic.cs`, this implementation
///     handles state by using enums and consists of a primitive finite state machine.
///     This change alone can make a huge impact on organizing your state code together without having to do large if/else
///     statements, with the added benefit of also making much easier to understand the transitions
///     between each state. The primary downside to this method is that every new state requires modifying everywhere it
///     interacts with, and also requires you to handle all the state in this switch statement.
///     This is pattern is where many folks end up stopping, but there are further improvements you can make by using the
///     state pattern.
/// </summary>
public partial class PlayerEnum : CharacterBody2D
{
    private const float Speed = 300.0f;
    private const float JumpVelocity = -650.0f;
    private const float DiveVelocity = -JumpVelocity * 0.75f;

    private AnimatedSprite2D _sprite2D;
    private State _state = State.Standing;

    public override void _Ready()
    {
        _sprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        var velocity = Velocity;

        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }

        var direction = Input.GetAxis("move_left", "move_right");

        switch (_state)
        {
            case State.Standing:
                if (Input.IsActionJustPressed("jump"))
                {
                    velocity.Y = JumpVelocity;
                    _state = State.Jumping;
                }

                if (Input.IsActionJustPressed("duck"))
                {
                    _sprite2D.Play("duck");
                    _state = State.Ducking;
                }
                else if (!Mathf.IsZeroApprox(direction))
                {
                    _sprite2D.Play("move");
                    velocity.X = direction * Speed;
                    _sprite2D.FlipH = direction < 0;
                    _state = State.Moving;
                }

                break;
            case State.Jumping:
                if (IsOnFloor())
                {
                    _sprite2D.Play("stand");
                    velocity = Vector2.Zero;
                    _state = State.Standing;
                }
                else if (Input.IsActionJustPressed("duck"))
                {
                    _sprite2D.Play("dive");
                    velocity.X = 0;
                    velocity.Y += DiveVelocity;
                    _state = State.Diving;
                }
                else
                {
                    velocity.X = direction * Speed;
                    _sprite2D.FlipH = direction < 0;
                }

                break;
            case State.Ducking:
                velocity = Vector2.Zero;
                if (Input.IsActionJustReleased("duck"))
                {
                    _sprite2D.Play("stand");
                    _state = State.Standing;
                }

                break;
            case State.Diving:
                if (IsOnFloor())
                {
                    if (Input.IsActionPressed("duck"))
                    {
                        _sprite2D.Play("duck");
                        _state = State.Ducking;
                    }
                    else
                    {
                        _sprite2D.Play("stand");
                        velocity = Vector2.Zero;
                        _state = State.Standing;
                    }
                }

                break;
            case State.Moving:
                if (Input.IsActionJustPressed("jump"))
                {
                    velocity.Y = JumpVelocity;
                    _state = State.Jumping;
                }
                else if (Input.IsActionPressed("duck"))
                {
                    _sprite2D.Play("duck");
                    _state = State.Ducking;
                }
                else if (Mathf.IsZeroApprox(direction))
                {
                    _sprite2D.Play("stand");
                    _state = State.Standing;
                    velocity = Vector2.Zero;
                }
                else
                {
                    velocity.X = direction * Speed;
                    _sprite2D.FlipH = direction < 0;
                }

                break;
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    private enum State
    {
        Standing,
        Jumping,
        Ducking,
        Diving,
        Moving
    }
}
