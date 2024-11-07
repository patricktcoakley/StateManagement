using Godot;

namespace StatePatternExample.Basic;

/// <summary>
///     The following is intentionally messy code that is meant to represent a typical example of a beginner-level state
///     management script, with the idea that it  will further be refactored multiple times to introduce ways to organize
///     the code better. One of the most common ways to handle state is to use "flags" (booleans) to get and set different
///     states, which can work fine enough depending on the scope, but it can quickly become hard to maintain and extend
///     over time.
///     To keep this example simple, the Player can do the following:
///     - Stand -> Duck or Jump
///     - Duck -> Stand
///     - Move -> Jump or Duck
///     - Jump -> Dive or Stand
///     The main difficulty with using flags is that the more you add, the more places you have to check and (re)set them.
/// </summary>
public partial class PlayerBasic : CharacterBody2D
{
    private const float Speed = 300.0f;
    private const float JumpVelocity = -650.0f;
    private const float DiveVelocity = -JumpVelocity * 0.75f;

    private bool _isDiving;
    private bool _isDucking;
    private bool _isJumping;

    private AnimatedSprite2D _sprite2D;

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

        if (Input.IsActionJustPressed("jump") && !_isDucking)
        {
            velocity.Y = JumpVelocity;
            _sprite2D.Play("jump");
            _isJumping = true;
        }
        else if (Input.IsActionPressed("duck"))
        {
            if (_isJumping && !_isDiving)
            {
                _sprite2D.Play("dive");
                velocity.X = 0;
                velocity.Y += DiveVelocity;
                _isDiving = true;
            }

            if (_isDiving)
            {
                if (IsOnFloor())
                {
                    _isDiving = false;
                    if (Input.IsActionPressed("duck"))
                    {
                        _sprite2D.Play("duck");
                    }
                    else
                    {
                        _sprite2D.Play("stand");
                        velocity = Vector2.Zero;
                    }

                }
            }
            else if (!_isJumping)
            {
                _sprite2D.Play("duck");
                velocity = Vector2.Zero;
                _isDucking = true;
            }
        }
        else if (Input.IsActionJustReleased("duck") && _isDucking)
        {
            _isDucking = false;
        }
        else if (IsOnFloor() && Mathf.IsZeroApprox(direction) && !_isDucking)
        {
            velocity = Vector2.Zero;
            _sprite2D.Play("stand");
            _isDiving = false;
            _isDucking = false;
            _isJumping = false;
        }
        else
        {
            if (IsOnFloor())
            {
                _isDiving = false;
                _isJumping = false;
            }

            if (!_isJumping)
            {
                _sprite2D.Play("move");
            }

            velocity.X = direction * Speed;
            _sprite2D.FlipH = direction < 0;
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}
