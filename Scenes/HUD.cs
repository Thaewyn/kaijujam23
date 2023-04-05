using Godot;
using System;

public class HUD : CanvasLayer {

  [Export] public NodePath statsTextBox;
  private Label stb;
  [Signal] delegate void TestButtonPressed();
  [Signal] delegate void ResetButtonPressed();
  [Signal] delegate void DifficultySliderChanged(int value);
  // [Export] public NodePath cameraPath;
  // private MainCam camera;
  private int difficultySliderValue;

  public override void _Ready() {
    stb = (Label) GetNodeOrNull(statsTextBox);
  }

  public bool UpdateStatsText(string text) {
    // GD.Print("HUD was told to update stats text.");
    stb.Text = text;

    return true;
  }

  public bool AppendStatsText(string text) {
    string current = stb.Text;
    stb.Text = current + text;

    return true;
  }

  public void _on_TestButton_pressed() {
    GD.Print("Pressed test button!");
    EmitSignal("TestButtonPressed");
    // camera = GetParent().GetNodeOrNull<MainCam>("MainCam");
    // camera.Shake();
    // TODO: screen shake, dust cloud particle emitter, thud sound effect
  }

  public void _on_ResetButton_pressed() {
    EmitSignal("ResetButtonPressed");
  }

  public void _on_DifficultySlider_drag_ended(bool changed) {
    if (changed) {
      // HSlider s = GetNode<HSlider>("DifficultySlider");
      // GD.Print(s.Value);
      EmitSignal("DifficultySliderChanged", difficultySliderValue);
    }
  }

  public void _on_DifficultySlider_value_changed(float val) {
    difficultySliderValue = (int) val;
  }

}


