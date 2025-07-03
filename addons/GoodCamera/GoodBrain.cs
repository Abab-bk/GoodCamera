using System.Collections.Generic;
using System.Linq;
using Godot;

namespace GoodCamera;

[GlobalClass]
public partial class GoodBrain : Node2D
{
    [Export] public Camera2D Camera2D { get; set; } = default!;
    private static GoodBrain Instance { get; set; } = default!;
    
    public GoodCamera? CurrentCamera { get; private set; }
    
    private static List<GoodCamera> Cameras { get; } = new();

    public override void _Ready()
    {
        base._Ready();
        if (IsInstanceValid(Instance))
        {
            GD.PrintErr("[GoodCamera] GoodBrain already exists!");
            return;
        }

        Instance = this;
        
        UpdateCurrentCamera();
    }

    public static void AddCamera(GoodCamera camera)
    {
        Cameras.Add(camera);
        Instance.UpdateCurrentCamera();
    }

    public static void RemoveCamera(GoodCamera camera)
    {
        Cameras.Remove(camera);
        Instance.UpdateCurrentCamera();
    }

    private void UpdateCurrentCamera()
    {
        var camera = Cameras
            .OrderBy(c => c.Priority)
            .FirstOrDefault();
        if (!IsInstanceValid(camera)) return;
        CurrentCamera = camera;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (!IsInstanceValid(CurrentCamera)) return;
        
        Camera2D.GlobalPosition = CurrentCamera.GlobalPosition;
    }
}