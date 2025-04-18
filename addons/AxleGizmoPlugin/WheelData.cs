using Godot;
using Godot.Collections;
using System;

[GlobalClass, Tool]
public partial class WheelData : Resource
{
    [ExportGroup("Tires")]
    [Export(PropertyHint.Range, "0.01, 10, 0.05")] public float Radius { get; private set; } = 0.5f;
    [Export(PropertyHint.Range, "0, 5, 0.05")] public float Friction { get; private set; } = 1.0f;


    [ExportGroup("Drivetrain")]
    private bool _isSteerable;
    private bool _isPowered;
    [Export] public bool IsSteerable {
        get => _isSteerable;
        set{
            _isSteerable = value;
            NotifyPropertyListChanged();
        }
    }
    [Export] public float MaxSteerAngle { get; private set; } = 45.0f;
    [Export] public bool IsPowered {
        get => _isPowered;
        set {
            _isPowered = value;
            NotifyPropertyListChanged();
        }
    }
    [Export] public float HorsePower { get; private set; } = 10.0f;
    [Export] public float MaxRPM { get; private set; } = 8000f;
    [Export] public float BrakeTorque { get; private set; } = 1500f;


    [ExportGroup("Suspension")]
    [Export] public float SuspensionLength { get; private set; } = 0.5f;
    [Export] public float SuspensionStiffness { get; private set; } = 10000f;
    [Export] public float SuspensionDamping { get; private set; } = 1000f;

    public override void _ValidateProperty(Dictionary property) {
        StringName propName = property["name"].AsStringName();
        Array<StringName> pNames = [ PropertyName.HorsePower, PropertyName.MaxRPM ];

        bool updatePowered = pNames.Contains(propName) && !IsPowered;
        bool updateSteerable = propName == PropertyName.MaxSteerAngle && !IsSteerable;

        if(updatePowered || updateSteerable) {
            var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ReadOnly;
            property["usage"] = (int)usage;
        }
    }


    public WheelData() {
        IsSteerable = false;
        IsPowered = false;
    }

    public void TogglePower(bool value) {
        IsPowered = value;
    }

    public void TogglePower() {
        TogglePower(!IsPowered);
    }

    public void SetWheelRadius(float radius) {
        Radius = radius;
    }


}
