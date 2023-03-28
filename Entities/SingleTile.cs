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

  private bool isFlipped = false;
  
  public override void _Ready() {
    flip_anim = GetNode<AnimationPlayer>("FlipAnimations");
    mesh = GetNode<MeshInstance>("MeshInstance");

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
      
      // GD.Print("Tile got clicked, info:");
      // // GD.Print("cam = "+cam.Name);
      // GD.Print(e);
      // GD.Print(b.ButtonIndex); //1 is left mouse, 2 is right mouse, 3 is middle mouse.
      // GD.Print(b.Pressed); //true on mousedown, false on mouseup
      if (b.ButtonIndex == 1 && b.Pressed == false) {
        //only trigger event on mouse button *release*
        // GD.Print("Do The Thing.");
        // FlipMe();
        // ToggleColor();
        GameController.GetInstance().TileWasClicked(this);
      }
      return true;
    }

    return false;
  }

  public void FlipMe() {
    GD.Print("Tile received 'flip' call");
    flip_anim.Play("FlipZHalf");

  }

  public void ToggleColor() {
    whichColor++;
    switch(whichColor) {
      case 1:
        mesh.SetSurfaceMaterial(0,red);
      break;
      case 2:
        mesh.SetSurfaceMaterial(0,green);
      break;
      case 0:
      default:
        mesh.SetSurfaceMaterial(0,blue);
        whichColor = 0;
      break;
    }
  }

}
