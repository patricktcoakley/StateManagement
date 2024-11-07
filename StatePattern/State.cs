using Godot;

namespace StatePatternExample.StatePattern;

/// <summary>
///     This example of the state pattern is loosely based on the initial implementation presented in
///     Game Programming Patterns (https://gameprogrammingpatterns.com/state.html). The primary State class only
///     implements `Enter` and `PhysicsProcess`, but you might also want to consider `Exit` or `Process` methods as well.
///     For example, maybe you need certain sounds to play or some kind of property change to occur independent of physics
///     processing.
///     Another thing to note is that you could also handle changing states in a different way, such as returning new
///     instances when states require a unique instance, or use a data structure to hold re-usable instances, like a
///     Dictionary; the next and final example includes a way to handle reusable instances of state objects in a type-safe manner.
///     However, to keep things simple we will just keep static instances inside the base class.
/// </summary>
public abstract class State
{
    public abstract void Enter(PlayerStatePattern player);
    public abstract State? PhysicsProcess(PlayerStatePattern player, double delta);

    protected static readonly StandingState StandingState = new();
    protected static readonly JumpingState JumpingState = new();
    protected static readonly DuckingState DuckingState = new();
    protected static readonly DivingState DivingState = new();
    protected static readonly MovingState MovingState = new();
}

public class StandingState : State
{
    public override void Enter(PlayerStatePattern player)
    {
        player.Sprite2D.Play("stand");
        player.Velocity = Vector2.Zero;
    }

    public override State? PhysicsProcess(PlayerStatePattern player, double delta)
    {
        if (Input.IsActionJustPressed("jump"))
        {
            return JumpingState;
        }

        if (Input.IsActionJustPressed("duck"))
        {
            return DuckingState;
        }

        if (!Mathf.IsZeroApprox(player.Direction))
        {
            return MovingState;
        }

        return null;
    }
}

public class JumpingState : State
{
    public override void Enter(PlayerStatePattern player)
    {
        player.Velocity = player.Velocity with { Y = PlayerStatePattern.JumpVelocity };
        player.Sprite2D.Play("jump");
    }

    public override State? PhysicsProcess(PlayerStatePattern player, double delta)
    {
        if (player.IsOnFloor())
        {
            return StandingState;
        }

        if (Input.IsActionJustPressed("duck"))
        {
            return DivingState;
        }

        player.Velocity = player.Velocity with { X = player.Direction * PlayerStatePattern.Speed };
        player.Sprite2D.FlipH = player.Direction < 0;

        return null;
    }
}

public class DuckingState : State
{
    public override void Enter(PlayerStatePattern player)
    {
        player.Sprite2D.Play("duck");
        player.Velocity = Vector2.Zero;
    }

    public override State? PhysicsProcess(PlayerStatePattern player, double delta)
    {
        if (Input.IsActionJustReleased("duck"))
        {
            return StandingState;
        }

        return null;
    }
}

public class DivingState : State
{
    public override void Enter(PlayerStatePattern player)
    {
        player.Velocity = new Vector2(0, player.Velocity.Y + PlayerStatePattern.DiveVelocity);
        player.Sprite2D.Play("dive");
    }

    public override State? PhysicsProcess(PlayerStatePattern player, double delta)
    {
        if (!player.IsOnFloor())
        {
            return null;
        }

        if (Input.IsActionPressed("duck"))
        {
            return DuckingState;
        }

        return StandingState;
    }
}

public class MovingState : State
{
    public override void Enter(PlayerStatePattern player)
    {
        player.Sprite2D.Play("move");
    }

    public override State? PhysicsProcess(PlayerStatePattern player, double delta)
    {
        if (Input.IsActionPressed("jump"))
        {
            return JumpingState;
        }

        if (Input.IsActionPressed("duck"))
        {
            return DuckingState;
        }

        if (Mathf.IsZeroApprox(player.Direction))
        {
            return StandingState;
        }

        player.Velocity = player.Velocity with { X = player.Direction * PlayerStatePattern.Speed };
        player.Sprite2D.FlipH = player.Direction < 0;

        return null;
    }
}
