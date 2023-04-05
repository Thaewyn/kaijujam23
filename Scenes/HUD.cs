using Godot;
using System;

public class HUD : CanvasLayer {

  [Export] public NodePath statsTextBox;
  private RichTextLabel stb;
  // [Signal] delegate void TestButtonPressed();
  [Signal] delegate void ResetButtonPressed();
  [Signal] delegate void LevelupButtonPressed();
  [Signal] delegate void PlayagainButtonPressed();
  [Signal] delegate void DifficultySliderChanged(int value);
  // [Export] public NodePath cameraPath;
  // private MainCam camera;
  private int difficultySliderValue;

  [Export] public NodePath restartButton;
  [Export] public NodePath levelupButton;
  [Export] public NodePath playagainButton;
  private Button restartBtn;
  private Button levelupBtn;
  private Button playagainBtn;

  public override void _Ready() {
    stb = (RichTextLabel) GetNodeOrNull(statsTextBox);
    restartBtn = (Button) GetNodeOrNull(restartButton);
    levelupBtn = (Button) GetNodeOrNull(levelupButton);
    playagainBtn = (Button) GetNodeOrNull(playagainButton);
    HideResetButton();
    HideLevelupButton();
    HidePlayagainButton();
  }

  public bool UpdateStatsText(string text) {
    // GD.Print("HUD was told to update stats text.");
    stb.Text = text;

    return true;
  }

  public bool AppendStatsText(string text) {
    string current = stb.Text;
    stb.Text = current + "\n\n" + text;

    return true;
  }

  // public void _on_TestButton_pressed() {
  //   GD.Print("Pressed test button!");
  //   EmitSignal("TestButtonPressed");
  //   // camera = GetParent().GetNodeOrNull<MainCam>("MainCam");
  //   // camera.Shake();
  //   // TODO: screen shake, dust cloud particle emitter, thud sound effect
  // }

  public void _on_ResetButton_pressed() {
    if(restartBtn.Visible) {
      EmitSignal("ResetButtonPressed");
    } else {
      GD.Print("reset button pressed when invisible. ignore.");
    }
  }

  public void _on_DifficultyUpBtn_pressed() {
    if(levelupBtn.Visible) {
      EmitSignal("LevelupButtonPressed");
    } else {
      GD.Print("levelup button pressed when invisible. ignore.");
    }
  }

  public void _on_PlayAgainBtn_pressed() {
    if(playagainBtn.Visible) {
      EmitSignal("PlayagainButtonPressed");
    } else {
      GD.Print("playagain button pressed when invisible. ignore.");
    }
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

  public void ShowResetButton() {
    restartBtn.Visible = true;
  }

  public void ShowLevelupButton() {
    levelupBtn.Visible = true;
  }

  public void ShowPlayagainButton() {
    playagainBtn.Visible = true;
  }
  
  public void HideResetButton() {
    restartBtn.Visible = false;
  }

  public void HideLevelupButton() {
    levelupBtn.Visible = false;
  }

  public void HidePlayagainButton() {
    playagainBtn.Visible = false;
  }

  public void HideAllButtons() {
    HideResetButton();
    HideLevelupButton();
    HidePlayagainButton();
  }

}


