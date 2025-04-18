using Godot;
using System;

public partial class Followcam : Camera3D
{
    [Export] NodePath lookTargetPath;
    [Export] Node3D followPos;
    [Export] float followSpeed = 0.1f;
    [Export] float lookSpeed = 0.1f;

    private Transform3D _targetTransform;
    private Node3D _lookTarget;

    public override void _Ready(){
        if (lookTargetPath != null){
            _lookTarget = GetNode<Node3D>(lookTargetPath);
        }
    }

    public override void _PhysicsProcess(double delta){
        if (_lookTarget == null)
            return;

        // Get the target position with offset
        var gt = followPos.GlobalPosition;
        var yPos = _lookTarget.GlobalPosition.Y + Mathf.Abs(followPos.Position.Y);
        var targetPosition = new Vector3(gt.X, yPos, gt.Z);

        // Smoothly move the camera towards the target position
        GlobalPosition = GlobalPosition.Lerp(targetPosition, followSpeed);

        // Always look at the target
        LookAt(_lookTarget.GlobalPosition, Vector3.Up);
    }
}