using Godot;
using System;

public class MainMenu : CanvasLayer {

  [Signal] public delegate void StartButtonPressed();

  public override void _Ready() {
      
  }

  public void _on_StartButton_pressed() {
    GD.Print("Pressed start button!");
    EmitSignal("StartButtonPressed");
  }

}
