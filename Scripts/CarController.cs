using Godot;


public partial class CarController : VehicleBody3D
{
    //public readonly PackedScene wheelScn = ResourceLoader.Load<PackedScene>("res://Scenes/wheel.tscn");

    [Export] public float maxForce = 100f;
    [Export(PropertyHint.Range, "-1.5, 1.5")]
    public float steerAmount = 0.4f;
    [Export] public float steerSpeed = 10f;
    [Export] public float maxSpeed = 50f;
    [Export] public float reverseSpeedMult = 0.85f;
    [Export] private Marker3D camPosF;
    [Export] private Marker3D camPosR;
    [Export] public Marker3D lerpingCamPos;
    private float forwardSpeed { 
        get {
            return -GlobalTransform.Basis.Z.Dot(LinearVelocity);
        }
    }
    private AudioStreamPlayer carAudio;

    private Label speedLabel;

    public override void _Ready() {
        speedLabel = UIManager.instance.speedLabel;
        carAudio = GetNode<AudioStreamPlayer>("CarAudio");
    }

    public override void _PhysicsProcess(double delta) {
        Steering = Mathf.Lerp(Steering, Input.GetAxis("steer_right", "steer_left") * steerAmount, (float)delta * steerSpeed);

        UpdateInput();
        UpdateCamera();

        if(!Engine.IsEditorHint()) return;
        DrawLine3D.Instance.DrawRay(GlobalPosition, GlobalTransform.Basis.Z * 10f, new Color(1f, 0f, 0f));

    }

    private void UpdateCamera() {
        LinearVelocity.Length();
        //svar targetCamPos =
        // Get the vehicle's forward (Z-axis) velocity
        Vector3 localVelocity = GlobalTransform.Basis.Inverse() * LinearVelocity;
        float forwardSpeed = localVelocity.Z;  // Positive = forward, Negative = reverse

        speedLabel.Text = "Speed: " + forwardSpeed.ToString("n2");
        var targetPos = forwardSpeed > -2 ? camPosF.GlobalPosition : camPosR.GlobalPosition;
        lerpingCamPos.GlobalPosition = lerpingCamPos.GlobalPosition.Lerp(targetPos, 0.1f);
    }

    private void UpdateInput() {

        var blWheel = GetNode<VehicleWheel3D>("BackLeftWheel");
        var brWheel = GetNode<VehicleWheel3D>("BackRightWheel");

        //TODO: ramp up the speed
        blWheel.EngineForce = calcEngineForce(blWheel);
        brWheel.EngineForce = calcEngineForce(brWheel);
         
        //Update rpm ui label
        var avgRpm = (blWheel.GetRpm() + brWheel.GetRpm()) / 2;
        carAudio.PitchScale = Mathf.Lerp(1, 2f, Mathf.Abs(avgRpm) / maxSpeed);
        UIManager.instance.rpmLabel.Text = "RPM: " + avgRpm.ToString("n2");
    }

    private float calcEngineForce(VehicleWheel3D wheel) {
        if(!wheel.IsInContact()) return 0; // Better to return 0 for wheelies

        float accelerationInput = Input.GetAxis("reverse", "accelerate");
        float currentRpm = wheel.GetRpm();

        // Different max speeds for forward/reverse if desired
        float effectiveMaxSpeed = accelerationInput > 0 ? maxSpeed : maxSpeed * reverseSpeedMult;

        // TODO: maxSpeed vs maxForce not really making a difference

        // Direction-aware RPM limiting
        float rpmFactor = 1 - Mathf.Abs(currentRpm) / effectiveMaxSpeed;

        // Apply input direction
        return accelerationInput * maxForce * rpmFactor;
    }

    private void UpdateGizmo() {
        GD.Print("Updating gizmo");
        NotifyPropertyListChanged();
    }
}
