#if TOOLS
using Godot;

[Tool]
public partial class AxleGizmoPlugin : EditorPlugin
{
    private AxleGizmo _plugin;
    Script script = GD.Load<Script>("res://addons/AxleGizmoPlugin/AxleRig.cs");
    Texture2D texture = GD.Load<Texture2D>("res://addons/AxleGizmoPlugin/axle.png");
    public override void _EnterTree()
    {
        AddCustomType("AxleRig", "Node3D", script, texture);
        _plugin = new AxleGizmo();
        AddNode3DGizmoPlugin(_plugin);
        
    }

    public override void _ExitTree()
    {
        RemoveNode3DGizmoPlugin(_plugin);
        RemoveCustomType("AxleRig");
    }

    private void SelectionChange()
    {
        GD.Print("Selection changed");
    }

}
#endif
