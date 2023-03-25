using Godot;
using System;

public class HUD : CanvasLayer {

  [Export] public NodePath statsTextBox;
  private Label stb;

  public override void _Ready() {
    stb = (Label) GetNodeOrNull(statsTextBox);
  }

  public bool UpdateStatsText(string text) {
    GD.Print("HUD was told to update stats text.");
    stb.Text = text;

    return true;
  }

}
