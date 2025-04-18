using Godot;
using Godot.Collections;
using System.Linq;


[GlobalClass, Tool]
public partial class Axle : Resource
{
    public const int WHEEL_COUNT = 2;

    private Vector3 _axlePosition;
    private float _axleLength;
    private float _axleAngle;
    private float _wheelRadius;

    [Export]
    public Vector3 axlePosition {
        get => _axlePosition;
        set {
            _axlePosition = value;
            EmitChanged();
        }
    }

    [Export]
    public float wheelRadius {
        get => _wheelRadius;
        set {
            _wheelRadius = value;
            EmitChanged();
        }
    }
    [Export]
    public float axleLength {
        get => _axleLength;
        set {
            _axleLength = value;
            EmitChanged();
        }
    }
    [Export]
    public float axleAngle {
        get => _axleAngle;
        set {
            _axleAngle = value;
            EmitChanged();
        }
    }

    public Axle() : this(0.5f, 2f, Vector3.Zero, 0) { }
    public Axle(float wheelRadius, float axleLength, Vector3 axlePosition, float axleAngle = 0f) {
        this.wheelRadius = wheelRadius;
        this.axleLength = axleLength;
        this.axlePosition = axlePosition;
        this.axleAngle = axleAngle;
    }

    public Array<Vector3> GetWheelPositions() {
        int wheelCount = WHEEL_COUNT;

        if(wheelCount == 0) {
            GD.PrintErr("No wheels found on axle.");
            return new Array<Vector3>();
        } else if(wheelCount == 1) {
            return new Array<Vector3> { axlePosition };
        }

        Quaternion axleRotator = new Quaternion(Vector3.Up, Mathf.DegToRad(axleAngle));
        Vector3 leftmost = axlePosition + (axleRotator * Vector3.Left) * (axleLength / 2f);

        Array<Vector3> wheelPositions = new Array<Vector3>();
        for(int i = 0; i < wheelCount; i++) {
            Vector3 position = leftmost + axleRotator * Vector3.Right * (i / (wheelCount - 1) * axleLength);
            wheelPositions.Add(position);
        }
        return wheelPositions;
    }
}
