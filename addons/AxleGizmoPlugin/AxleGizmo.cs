#if TOOLS
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[Tool]
public partial class AxleGizmo : EditorNode3DGizmoPlugin
{

    public AxleGizmo() {
        CreateMaterial("axle_line", new Color(0, 1, 0));
        CreateMaterial("wheel_position", new Color(1, 0, 0, 0.25f));
    }

    public override string _GetGizmoName() {
        return "Axle Gizmo";
    }


    public override bool _HasGizmo(Node3D node) {
        return node is AxleRig;
    }

    public override void _Redraw(EditorNode3DGizmo gizmo) {
        //GD.Print("Redrawing Gizmo for Node: " + gizmo.GetNode3D().Name);
        gizmo.Clear();
        var node = gizmo.GetNode3D() as AxleRig;
        if(node == null || node.Axles == null) return;
        foreach(Axle axle in node.Axles) {
            DrawAxleGizmo(gizmo, node.GetParentNode3D(), axle);
        }
    }

    private void DrawAxleGizmo(EditorNode3DGizmo gizmo, Node3D parentNode, Axle axle) {
        if(axle == null) return;

        var wheelPositions = axle.GetWheelPositions();
        if(wheelPositions.Count == 0) return;

        // Draw axle line
        if(wheelPositions.Count > 1) {
            var points = new Vector3[] {
                parentNode.ToGlobal(wheelPositions[0]),
                parentNode.ToGlobal(wheelPositions[^1])
            };
            gizmo.AddLines(points, GetMaterial("axle_line", gizmo));
        }

        // Draw wheel positions
        foreach(var pos in wheelPositions) {
            var globalPos = parentNode.ToGlobal(pos);
            gizmo.AddMesh(
                GetSphereMesh(axle.wheelRadius/3f),
                GetMaterial("wheel_position", gizmo),
                Transform3D.Identity.TranslatedLocal(globalPos));
        }
    }

    private static Mesh GetSphereMesh(float radius) {
        var sphere = new SphereMesh();
        sphere.Radius = radius;
        sphere.Height = radius * 2;
        return sphere;
    }
}
#endif