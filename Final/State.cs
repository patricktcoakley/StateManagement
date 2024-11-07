using Godot;

namespace StatePatternExample.Final;

/// <summary>
///     The State class changes from the previous version by having a parent StateMachine injected, allowing it to
///     transition to another state and have access to the owner without having to have a direct reference to it.
///     Outside of that, we aren't returning a new State instance each time, but instead reusing existing ones in the
///     parent. This code could be refactored even further in a few ways, including using the Name property to match the
///     animation, as well as extract some of the remaining repeated logic into separate methods.
/// </summary>
public abstract class State<TOwner>(StateMachine<TOwner> parent)
{
    protected readonly StateMachine<TOwner> Parent = parent;

    public abstract string Name { get; }
    public abstract void Enter(TOwner owner);
    public abstract void PhysicsProcess(TOwner owner, double delta);
}

public class StandingState(StateMachine<PlayerFinal> parent) : State<PlayerFinal>(parent)
{
    public override string Name => nameof(StandingState);

    public override void Enter(PlayerFinal player)
    {
        player.Sprite2D.Play("stand");
        player.Velocity = Vector2.Zero;
    }

    public override void PhysicsProcess(PlayerFinal player, double delta)
    {
        if (Input.IsActionJustPressed("jump"))
        {
            Parent.TransitionTo<JumpingState>();
            return;
        }

        if (Input.IsActionJustPressed("duck"))
        {
            Parent.TransitionTo<DuckingState>();
            return;
        }

        if (!Mathf.IsZeroApprox(player.Direction))
        {
            Parent.TransitionTo<MovingState>();
        }
    }
}

public class JumpingState(StateMachine<PlayerFinal> parent) : State<PlayerFinal>(parent)
{
    public override string Name => nameof(JumpingState);

    public override void Enter(PlayerFinal player)
    {
        player.Velocity = player.Velocity with { Y = PlayerFinal.JumpVelocity };
        player.Sprite2D.Play("jump");
    }

    public override void PhysicsProcess(PlayerFinal player, double delta)
    {
        if (player.IsOnFloor())
        {
            Parent.TransitionTo<StandingState>();
            return;
        }

        if (Input.IsActionJustPressed("duck"))
        {
            Parent.TransitionTo<DivingState>();
            return;
        }

        player.Velocity = player.Velocity with { X = player.Direction * PlayerFinal.Speed };
        player.Sprite2D.FlipH = player.Direction < 0;
    }
}

public class DuckingState(StateMachine<PlayerFinal> parent) : State<PlayerFinal>(parent)
{
    public override string Name => nameof(DuckingState);

    public override void Enter(PlayerFinal player)
    {
        player.Sprite2D.Play("duck");
        player.Velocity = Vector2.Zero;
    }

    public override void PhysicsProcess(PlayerFinal player, double delta)
    {
        if (Input.IsActionJustReleased("duck"))
        {
            Parent.TransitionTo<StandingState>();
        }
    }
}

public class DivingState(StateMachine<PlayerFinal> parent) : State<PlayerFinal>(parent)
{
    public override string Name => nameof(DivingState);

    public override void Enter(PlayerFinal player)
    {
        player.Velocity = new Vector2(0, player.Velocity.Y + PlayerFinal.DiveVelocity);
        player.Sprite2D.Play("dive");
    }

    public override void PhysicsProcess(PlayerFinal player, double delta)
    {
        if (!player.IsOnFloor())
        {
            return;
        }

        if (Input.IsActionPressed("duck"))
        {
            Parent.TransitionTo<DuckingState>();
            return;
        }

        Parent.TransitionTo<StandingState>();
    }
}

public class MovingState(StateMachine<PlayerFinal> parent) : State<PlayerFinal>(parent)
{
    public override string Name => nameof(MovingState);

    public override void Enter(PlayerFinal player)
    {
        player.Sprite2D.Play("move");
    }

    public override void PhysicsProcess(PlayerFinal player, double delta)
    {
        if (Input.IsActionJustPressed("jump"))
        {
            Parent.TransitionTo<JumpingState>();
            return;
        }

        if (Input.IsActionPressed("duck"))
        {
            Parent.TransitionTo<DuckingState>();
            return;
        }

        if (Mathf.IsZeroApprox(player.Direction))
        {
            Parent.TransitionTo<StandingState>();
            return;
        }

        player.Velocity = player.Velocity with { X = player.Direction * PlayerFinal.Speed };
        player.Sprite2D.FlipH = player.Direction < 0;
    }
}
