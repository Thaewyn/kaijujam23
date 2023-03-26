using Godot;
using System;

public class GameController : Spatial {

  [Export] public NodePath tileContainer;
  private Position3D tilePositions;
  [Export] public PackedScene singleTile;
  // [Export] public NodePath mainCamera;
  // private MainCam mainCam_actual;
  private CanvasLayer mainMenu;
  private HUD hud;
  private AnimationPlayer anim;
  private int clickCounter = 0;
  // private SceneTreeTween flipTween;

  public override void _Ready() {
    tilePositions = (Position3D) GetNodeOrNull(tileContainer);
    // mainCam_actual = (MainCam) GetNodeOrNull(mainCamera);
    anim = GetNode<AnimationPlayer>("AnimationPlayer");

    mainMenu = GetNode<CanvasLayer>("MainMenu");
    hud = GetNode<HUD>("HUD");
    // flipTween = GetTree().CreateTween();//GetNode<Tween>("FlipTweener");

    if(tilePositions != null) {
      PopulateTiles();
    }

    if(instance == null) {
      instance = this;
    } else {
      //duplicate, remove this.
    }
  }

  public void _on_MainMenu_StartButtonPressed() {
    GD.Print("Got 'startbuttonpressed' from mainmenu - tell camera to animate.");
    // mainCam_actual.StartGame();
    anim.Play("StartGameSwoosh");
  }

  public void _on_HUD_TestButtonPressed() {
    GD.Print("Got 'testbuttonpressed' from hud - test the juice.");
  }

  private bool PopulateTiles() {
    //for each position in tilePositions
    // spawn a new tile

    foreach( Position3D pos in tilePositions.GetChildren() ) {
      // GD.Print(pos.Name);

      Vector3 gridPosition = IdentifyTilePosition(pos);

      Node inst = singleTile.Instance();
      SingleTile tile = inst as SingleTile;
      tile.SetLocationInformation(gridPosition);
      tile.AddToGroup("tiles");
      pos.AddChild(tile);
      //While building grab a reference to everything so we can call their stuff later!
      // we can use the Pos3D's information to locate it correctly
    }

    return false;
  }

  private Vector3 IdentifyTilePosition(Position3D loc) {
    //given a position3d, use transform values to compute actual useful mathmatical location.
    //x 1s, z 1.65s
    // z position is Columns. Columns is the first part of the vector3.

    int column;
    switch(loc.Translation.z) {
      case 4.95f:
        column = 0;
        break;
      case 3.3f:
        column = 1;
        break;
      case 1.65f:
        column = 2;
        break;
      case 0f:
        column = 3;
        break;
      case -1.65f:
        column = 4;
        break;
      case -3.3f:
        column = 5;
        break;
      case -4.95f:
        column = 6;
        break;
      default:
        column = -1;
        break;
    }

    return new Vector3(-1, -1, column);

  }

  public void TileWasClicked(Node whichTile) {
    GD.Print("Game Controller received TileWasClicked:");
    GD.Print(whichTile.Name);
    // Vector3 pos = whichTile.GetParent().Position;
    SingleTile tile = whichTile as SingleTile;
    float col = tile.GetColumn();
    FlipTilesInColumn(col);

    clickCounter++;
    string update = "Clicks: "+clickCounter+"\nColumn: "+col;
    hud.UpdateStatsText(update);
  }

  private void FlipTilesInColumn(float whichColumn) {
    Godot.Collections.Array tiles = GetTree().GetNodesInGroup("tiles");
    int i = 0;
    foreach (Node t in tiles) {
      // GD.Print(tile.Name);
      SingleTile tile = t as SingleTile;
      //start with the clicked tile's row, find the rows, run coroutines in sequence
      if(tile.GetColumn() == whichColumn) {
        SceneTreeTween tw = GetTree().CreateTween();
        i++; //get row
        GD.Print("attempting to call interpolateCallback on tile "+tile.Name+" with i="+i);
        tw.TweenCallback(tile, "FlipMe").SetDelay((0.1f * i));
        //flipTween.InterpolateCallback(tile, (0.1f * i), "FlipMe");
        // tile.FlipMe();
      }
    }
  }

  //clicking a tile is put into an input queue / buffer
  // after each input is resolved, do gamestate things (process tiles, apply damage, collect items, whatever)
  // *then* resolve the next item in the queue. Only one queued move allowed, and any new clicks while waiting *replace* the queued move.

  //singleton pattern.
  private static GameController instance;
  public static GameController GetInstance() {
    return instance;
  }

}
