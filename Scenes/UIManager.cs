using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class UIManager : CanvasLayer
{
    private static UIManager _instance;
    public static  UIManager instance {
        get {
            if(_instance == null)
                GD.PrintErr("UIManager instance is null. Make sure to initialize it before use.");
            return _instance;
        }
        private set {
            _instance = value;
        }
    }
    [Export] public Label speedLabel;
    [Export] public Label rpmLabel;

    public override void _EnterTree() {
        if(_instance != null) {
            GD.PrintErr("There is already an instance of UIManager in the scene!");
        }
        _instance = this;
    }
    public override void _Ready() {

    }

}
