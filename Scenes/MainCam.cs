using Godot;
using System;

public class MainCam : Camera {

  public void _on_VScrollBar_value_changed(float val) {
    ColorRect screenbuffer = GetNode<ColorRect>("CanvasLayer/ColorRect");
    ShaderMaterial shader = screenbuffer.Material as ShaderMaterial;
    shader.SetShaderParam("offset", val);
  }

  // private AnimationPlayer anim;
  // private MainCam camera;
  // private OpenSimplexNoise noise = new OpenSimplexNoise();
  // private float xNoise = 0.0f;
  // private float yNoise = 0.0f;
  // private Vector2 offset = Vector2.Zero;

  // public override void _Ready() {
  //   // anim = GetNode<AnimationPlayer>("AnimationPlayer");
  //   camera = GetNodeOrNull<MainCam>("MainCam");
  //   noise.Seed = (int)GD.Randi();
  //   noise.Octaves = 4;
  //   noise.Period = 20.0f;
  //   noise.Persistence = 0.8f;
  // }

    // public void StartGame() {
  //   GD.Print("was told to start game by ?");
  //   anim.Play("StartGameSwoosh");
  // }

  // public void Shake() {
  //   GD.Print("shaking the screen!");
  //   while(true) {
  //     xNoise = noise.GetNoise1d(OS.GetTicksMsec() * 0.1f);
  //     yNoise = noise.GetNoise1d(OS.GetTicksMsec() * 0.1f + 100.0f);
  //     offset = new Vector2(xNoise, yNoise);
  //     GD.Print("x noise = " + xNoise);
  //     GD.Print("y noise = " + yNoise);
  //     GD.Print("offset = " + offset);
  //   }
  // }
}
