using Godot;
using System;

public class MonsterFootprintTile : SingleTile {

  [Export] public Material flagMaterial;
  [Export] public int tileBaseSurface;
  private bool flagged = false;

  public void FlagTile() {
    //told to be flagged. Turn tile base red.
    GetNode<MeshInstance>("MeshInstance").SetSurfaceMaterial(tileBaseSurface,flagMaterial);
    flagged = true;
  }

  public bool IsFlagged() {
    return flagged;
  }
}
