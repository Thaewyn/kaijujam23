using Godot;
using System;

public class GameController : Spatial {

  [Export] public NodePath tileContainer;
  private Position3D tilePositions;
  [Export] public PackedScene singleTile;

  public override void _Ready() {
    tilePositions = (Position3D) GetNodeOrNull(tileContainer);

    if(tilePositions != null) {
      PopulateTiles();
    }

    if(instance == null) {
      instance = this;
    } else {
      //duplicate, remove this.
    }
  }

  private bool PopulateTiles() {
    //for each position in tilePositions
    // spawn a new tile

    foreach( Position3D pos in tilePositions.GetChildren() ) {
      GD.Print(pos.Name);
      Node inst = singleTile.Instance();
      pos.AddChild(inst);
      //While building grab a reference to everything so we can call their stuff later!
      // we can use the Pos3D's information to locate it correctly
    }

    return false;
  }

  public void TileWasClicked(Node whichTile) {
    GD.Print("Game Controller received TileWasClicked:");
    GD.Print(whichTile.Name);
    // Vector3 pos = whichTile.GetParent().Position;
  }

  //singleton pattern.
  private static GameController instance;
  public static GameController GetInstance() {
    return instance;
  }

}
