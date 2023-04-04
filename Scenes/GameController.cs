using Godot;
using System;

public class GameController : Spatial {

  [Export] public NodePath tileContainer;
  private Position3D tilePositions;
  [Export] public PackedScene singleTile;
  [Export] public PackedScene cityTile;
  [Export] public PackedScene forestTile;
  [Export] public PackedScene lakeTile;
  [Export] public PackedScene mountainTile;
  [Export] public PackedScene monsterTile;
  [Export] public PackedScene humanTile;
  [Export] public NodePath mainCamera;
  private Camera mainCam_actual;
  private CanvasLayer mainMenu;
  private HUD hud;
  private AnimationPlayer anim;
  private int clickCounter = 0;
  // private SceneTreeTween flipTween;

  private float shakeDuration = 0.0f;
  [Export] public float shakeMaxDuration = 1.0f;
  [Export] public float maxShakeH = 5.0f;
  [Export] public float maxShakeV = 5.0f;
  private OpenSimplexNoise noise = new OpenSimplexNoise();

  public override void _Ready() {
    tilePositions = (Position3D) GetNodeOrNull(tileContainer);
    mainCam_actual = (Camera) GetNodeOrNull(mainCamera);
    anim = GetNode<AnimationPlayer>("AnimationPlayer");

    mainMenu = GetNode<CanvasLayer>("MainMenu");
    hud = GetNode<HUD>("HUD");
    // flipTween = GetTree().CreateTween();//GetNode<Tween>("FlipTweener");
    noise.Seed = (int)GD.Randi();
    noise.Octaves = 4;
    noise.Period = 20.0f;
    noise.Persistence = 0.8f;

    if(tilePositions != null) {
      PopulateTiles();
    }

    if(instance == null) {
      instance = this;
    } else {
      //duplicate, remove this.
    }
  }

  public override void _Process(float delta) {
    if (shakeDuration > 0) {
      shakeDuration -= delta;
      mainCam_actual.HOffset = noise.GetNoise1d(OS.GetTicksMsec() * 0.1f) * shakeDuration * maxShakeH;
      mainCam_actual.VOffset = noise.GetNoise1d(OS.GetTicksMsec() * 0.1f + 100.0f) * shakeDuration * maxShakeV;
      
      //add chromatic aberration
      ColorRect screenbuffer = mainCam_actual.GetNode<ColorRect>("CanvasLayer/ColorRect");
      ShaderMaterial shader = screenbuffer.Material as ShaderMaterial;
      shader.SetShaderParam("offset", shakeDuration * 30);
    }
  }

  public void _on_MainMenu_StartButtonPressed() {
    GD.Print("Got 'startbuttonpressed' from mainmenu - tell camera to animate.");
    // mainCam_actual.StartGame();
    anim.Play("StartGameSwoosh");
  }

  public void _on_HUD_TestButtonPressed() {
    GD.Print("Got 'testbuttonpressed' from hud - test the juice.");
    shakeDuration = shakeMaxDuration;
  }

  // private void slideInCamera() {
  //   SceneTreeTween tw = GetTree().CreateTween();
  //   tw.TweenProperty(mainCam_actual, "h_offset", 0.0f, shakeDuration).From(maxShakeH).SetTrans(Tween.TransitionType.Bounce).SetEase(Tween.EaseType.Out);
  // }

  private bool PopulateTiles() {
    //for each position in tilePositions
    // spawn a new tile

    foreach( Position3D pos in tilePositions.GetChildren() ) {
      // GD.Print(pos.Name);

      Vector3 gridPosition = IdentifyTilePosition(pos);

      var random = new Random();
      int whichTile = random.Next(1,7); //holy cow we can do more intelligent tile picking than this, but that requires more work.
      Node inst;
      switch(whichTile) {
        case 1:
        inst = cityTile.Instance();
        break;
        case 2:
        inst = mountainTile.Instance();
        break;
        case 3:
        inst = lakeTile.Instance();
        break;
        case 4:
        inst = forestTile.Instance();
        break;
        case 5:
        inst = humanTile.Instance();
        break;
        case 6:
        inst = monsterTile.Instance();
        break;
        default:
        inst = singleTile.Instance();
        break;
      }
      // Node inst = singleTile.Instance();
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
        // flipTween.InterpolateCallback(tile, (0.1f * i), "FlipMe");
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
