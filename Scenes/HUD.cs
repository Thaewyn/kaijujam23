using Godot;
using System;

public class HUD : CanvasLayer {

  [Export] public NodePath statsTextBox;
  private Label stb;
  [Signal] delegate void TestButtonPressed();
  // [Export] public NodePath cameraPath;
  // private MainCam camera;

  public override void _Ready() {
    stb = (Label) GetNodeOrNull(statsTextBox);
  }

  public bool UpdateStatsText(string text) {
    GD.Print("HUD was told to update stats text.");
    stb.Text = text;

    return true;
  }

  public void _on_TestButton_pressed() {
    GD.Print("Pressed test button!");
    EmitSignal("TestButtonPressed");
    // camera = GetParent().GetNodeOrNull<MainCam>("MainCam");
    // camera.Shake();
    // TODO: screen shake, dust cloud particle emitter, thud sound effect
  }
}


