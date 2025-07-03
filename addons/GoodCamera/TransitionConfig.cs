using Godot;

namespace GoodCamera;

[GlobalClass]
public partial class TransitionConfig : Resource
{
    [Export] public Tween.TransitionType TransitionType = Tween.TransitionType.Cubic;
    [Export] public float Duration = 0.2f;
}