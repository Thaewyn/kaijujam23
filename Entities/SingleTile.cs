using Godot;
using System;

public class SingleTile : KinematicBody {

  [Export] public string TileType = "default";
  private AnimationPlayer flip_anim;
  [Export] public Material red;
  [Export] public Material green;
  [Export] public Material blue;
  private int whichColor = 0;
  private MeshInstance mesh;

  private float column;
  private float row_northeast;
  private float row_southeast;

  private bool isFaceUp = false;
  
  public override void _Ready() {
    flip_anim = GetNode<AnimationPlayer>("FlipAnimations");
    mesh = GetNode<MeshInstance>("MeshInstance");
    isFaceUp = true;
  }

  public bool SetLocationInformation(Vector3 info) {
    row_northeast = info.x;
    row_southeast = info.y;
    column = info.z;

    return true;
  }

  public float GetColumn() {
    return column;
  }

  public bool _on_SingleTile_input_event(Node cam, InputEvent e, Vector3 position, Vector3 normal, int shape_idx) {
    // GD.Print("any input event");
    if(e is InputEventMouseButton) {
      InputEventMouseButton b = e as InputEventMouseButton;
      if (b.ButtonIndex == 1 && b.Pressed == false) { //left click release
        GameController.GetInstance().TileWasClicked(this);
      } else if (b.ButtonIndex == 2 && b.Pressed == false) { //right click release
        GameController.GetInstance().TileFlagAttempt(this);
      }
      return true;
    }

    return false;
  }

  public void FlipMe() {
    GD.Print("Tile received 'flip' call");
    if(isFaceUp) {
      flip_anim.Play("FlipFaceDown");
      isFaceUp = false;
    } else {
      flip_anim.Play("FlipFaceUp");
      isFaceUp = true;
    }
  }

  public bool GetIsFaceUp() {
    return isFaceUp;
  }

  // public void ToggleColor() {
  //   whichColor++;
  //   switch(whichColor) {
  //     case 1:
  //       mesh.SetSurfaceMaterial(0,red);
  //     break;
  //     case 2:
  //       mesh.SetSurfaceMaterial(0,green);
  //     break;
  //     case 0:
  //     default:
  //       mesh.SetSurfaceMaterial(0,blue);
  //       whichColor = 0;
  //     break;
  //   }
  // }

}
