using System;
using Godot;

namespace GoodCamera;

[GlobalClass]
public partial class GoodCamera : Node2D
{
    [Export] public int Priority { get; set; }
    [Export] public FollowMode FollowMode { get; set; } = FollowMode.None;
    [Export] public TransitionConfig TransitionConfig { get; set; } = new();
    
    [Export]
    public Node2D? Target
    {
        get => _target;
        set
        {
            if (IsInstanceValid(_target)
                && _target != value)
            {
                ChangeTarget(value!);
            }

            _target = value;
        }
    }

    private Node2D? _target;
    
    private enum State
    {
        Normal,
        Moving
    }
    
    private State CurrentState { get; set; } = State.Normal;
    

    private void ChangeTarget(Node2D target)
    {
        CurrentState = State.Moving;
        var tween = CreateTween();
        tween.SetTrans(TransitionConfig.TransitionType);
        tween.TweenProperty(
            this,
            "global_position",
            target.GlobalPosition,
            TransitionConfig.Duration
            );
        tween.Finished += () =>
        {
            CurrentState = State.Normal;
        };
    }

    public override void _Ready()
    {
        base._Ready();
        GoodBrain.AddCamera(this);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (FollowMode != FollowMode.FollowSingle) return;
#if TOOLS
        if (!IsInstanceValid(Target))
        {
            GD.PrintErr("[GoodCamera] Target is not valid!");
        }
#endif

        switch (CurrentState)
        {
            case State.Normal:
                GlobalPosition = Target!.GlobalPosition;
                break;
            case State.Moving:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        GoodBrain.RemoveCamera(this);
    }
}