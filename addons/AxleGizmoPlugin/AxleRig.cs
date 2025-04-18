using Godot;
using Godot.Collections;
using System;
using System.Linq;

[Tool]
public partial class AxleRig : Node3D
{
    public bool autoSpawnWheels = true;
    private Array<Axle> _axles;

    [Export] public Array<VehicleWheel3D> wheels;
    [Export] public Array<Axle> Axles
    {
        get => _axles;
        set
        { 
            if (_axles != null)
                foreach (Axle axle in _axles)
                {
                    if (axle != null)
                        axle.Changed -= UpdateWheels;
                }
            _axles = value;
            if (_axles == null) return;
            foreach (Axle axle in _axles)
            {
                if (axle != null)
                    axle.Changed += UpdateWheels;
            }
        }
    }

    [Export] public VehicleBody3D parentVehicle;

    public AxleRig() {
        Axles = new Array<Axle>();
        wheels = new Array<VehicleWheel3D>();

    }

    public override void _Ready() {
        if(Engine.IsEditorHint()) {
            // TODO: When first adding the axle rig node, changing the axle values
            // doesn't update the wheels for some reason!
            Axles.Clear();
            for(int i = 0; i < 2; i++) {
                Axles.Add(new Axle());
            }
            FindParentVehicle();
            UpdateWheels();
        }


        if(autoSpawnWheels)
            SpawnWheels();
    }

    private bool FindParentVehicle() {
        GD.Print("Searching for parent VehicleBody3D...");
        if(parentVehicle == null) {
            Node3D parent = GetParent<Node3D>();
            while(parent != null) {
                if(parent is VehicleBody3D) {
                    parentVehicle = (VehicleBody3D)parent;
                    GD.Print("Found parent vehicle!", parentVehicle.Name);
                    break;
                } else {
                    parent = parent.GetParent<Node3D>();
                }
            }
            if(parent == null)
                return false;
        }
        return true;
    }

    private void UpdateWheels() {
        if(parentVehicle == null && !FindParentVehicle()) {
            GD.Print("No parent vehicle found, cannot update wheels.");
            return;
        }
        GD.Print("Updating wheels...");
        // Wheels array controls how many wheels are children of this node.
        // Do not allow wheel Nodes to be deleted in the inspector, or let the array size to be changed.
        // everything is controlled by the AxleRig's Array of Axles.

        // check how many wheel nodes are a child of this AxleRig's vehicle node.
        GD.Print("Vehicle children: ", string.Join(", ", parentVehicle.GetChildren().Select(c => c.GetType())));
        var wheelNodes = parentVehicle.FindChildren("*", typeof(VehicleWheel3D).Name);
        if(wheelNodes == null) {
            wheelNodes = new();
        }
        GD.Print("wheel nodes: ", wheelNodes.Count);
        // If the number of wheels in the wheelNodes array, and the number of wheels in the wheels array do not
        // match what is in the Axles array, then we need to update the wheels.
        var expectedWheelCount = Axles.Sum(axle => Axle.WHEEL_COUNT);
        GD.Print("Expected wheel count: ", expectedWheelCount);

        if(wheelNodes.Count != expectedWheelCount) {
            GD.Print("Wheels do not match, clearing wheels...");
            // Remove all child wheels
            foreach(var wheel in wheelNodes) {
                GD.Print("Removing wheel ", wheel.Name);
                parentVehicle.RemoveChild(wheel);
                wheel.QueueFree();
            }
            wheels.Clear();
            wheelNodes.Clear();

            // Add the expected wheels
            for(int i = 0; i < expectedWheelCount; i++) {
                GD.Print("Adding wheel ", i);
                var wheel = new VehicleWheel3D();
                parentVehicle.AddChild(wheel);

                // Critical for editor functionality
                if(Engine.IsEditorHint())
                    wheel.Owner = parentVehicle;

                wheels.Add(wheel);
            }
        }
        // now there should be the expected number of wheels
        GD.Print("Repositioning wheels...");
        var wheelPositions = Axles.SelectMany(axle => axle.GetWheelPositions()).ToArray();
        for(int i = 0; i < wheels.Count; i++) {
            int axleIndex = i / Axle.WHEEL_COUNT;
            wheels[i].Position = wheelPositions[i];
            wheels[i].Rotation = Vector3.Up * Axles[axleIndex].axleAngle;
            wheels[i].WheelRadius = Axles[axleIndex].wheelRadius;
        }

        UpdateGizmos();
    }

    public void SpawnWheels() {
        foreach(Axle axle in Axles) {
            //axle.SpawnWheels();
        }
    }
}
