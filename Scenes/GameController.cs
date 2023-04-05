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

  //game logic
  public enum GAME_STATE {
    MAIN_MENU,
    PLAYER_TURN,
    PROCESSING,
    GAME_OVER
  }
  private GAME_STATE gameState;
  private int player_health = 3;
  private int difficulty = 1;

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

    //game state stuff
    init();

    if(instance == null) {
      instance = this;
    } else {
      //duplicate, remove this.
      GD.Print("ERROR - Duplicate GameController Instance.");
      // this.queuefree()
    }
  }

  private void init() {
    //on first load
    gameState = GAME_STATE.MAIN_MENU;
    player_health = 3;
    difficulty = 1;
  }

  private bool restart() {
    //called when the player wants to play again
    //flip all tiles face up
    Godot.Collections.Array tiles = GetTree().GetNodesInGroup("tiles");
    foreach (Node t in tiles) {
      SingleTile tile = t as SingleTile;
      if(!tile.GetIsFaceUp()) {
        tile.FlipMe();
      }
    }
    player_health = 3;
    clickCounter = 0;
    //repopulate tiles?

    return false;
  }

  private bool gameOver() {
    //called when the map / level ends

    return false;
  }

  private bool pauseGame() {
    //player told the game to bring up the menu

    return false;
  }

  public override void _Process(float delta) {
    //handle screen shake, if active.
    if (shakeDuration > 0) {
      shakeDuration -= delta;
      mainCam_actual.HOffset = noise.GetNoise1d(OS.GetTicksMsec() * 0.1f) * shakeDuration * maxShakeH;
      mainCam_actual.VOffset = noise.GetNoise1d(OS.GetTicksMsec() * 0.1f + 100.0f) * shakeDuration * maxShakeV;
    }
  }

  public void _on_MainMenu_StartButtonPressed() {
    GD.Print("Got 'startbuttonpressed' from mainmenu - tell camera to animate.");
    // mainCam_actual.StartGame();
    anim.Play("StartGameSwoosh");
    gameState = GAME_STATE.PLAYER_TURN;
  }

  public void _on_HUD_TestButtonPressed() {
    GD.Print("Got 'testbuttonpressed' from hud - test the juice.");
    MakeScreenShake();
  }

  public void MakeScreenShake() {
    shakeDuration = shakeMaxDuration;
  }

  public void _on_HUD_ResetButtonPressed() {
    restart();
  }

  public void _on_HUD_DifficultySliderChanged(int val) {
    difficulty = val;
    GD.Print(difficulty);
  }

  // private void slideInCamera() {
  //   SceneTreeTween tw = GetTree().CreateTween();
  //   tw.TweenProperty(mainCam_actual, "h_offset", 0.0f, shakeDuration).From(maxShakeH).SetTrans(Tween.TransitionType.Bounce).SetEase(Tween.EaseType.Out);
  // }

  private bool PopulateTiles() {
    //for each position in tilePositions
    // spawn a new tile
    int remainingMonsters = difficulty;

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
          if (remainingMonsters > 0) {
            inst = monsterTile.Instance();
            remainingMonsters--;
          } else {
            inst = singleTile.Instance();
          }
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

    //if fewer monsters than difficulty, regenerate board.

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


  public void TileFlagAttempt(Node whichTile) {
    GD.Print("Game Controller received TileFlagAttempt:");
    GD.Print(whichTile.Name);
    //tile was right-clicked
    if(gameState == GAME_STATE.PLAYER_TURN) {

    //if tile is monster tile, flag monster tile
      //if flags = number of monster tiles (difficulty?), win!
    // if tile is not monster tile, lose hp
      //if hp = 0, lose. Pop up option to reset.
    }
  }

  public void TileWasClicked(Node whichTile) {
    // GD.Print("Game Controller received TileWasClicked:");
    // GD.Print(whichTile.Name);
    // Vector3 pos = whichTile.GetParent().Transform;
    
    if(gameState == GAME_STATE.PLAYER_TURN) {
      
      if (whichTile is MonsterFootprintTile) {
        GD.Print("clicked monster footprint, GAME OVER!");
        // do screenshake and things
        MakeScreenShake();
        gameOver();
      } else {
        //non-monster tile.
        SingleTile tile = whichTile as SingleTile;
        Position3D pos = tile.GetParent() as Position3D;
        GD.Print(pos.Translation);
        GD.Print("distance: "+GetDistanceToNearestMonster(pos.Translation));
        // float col = tile.GetColumn();
        // FlipTilesInColumn(col);
        if(tile.GetIsFaceUp()) {
          //face up logic
          tile.FlipMe();
        } else {
          //face down logic.
        }

        clickCounter++;
        string update = "Clicks: "+clickCounter;///+"\nColumn: "+col;
        hud.UpdateStatsText(update);
      }
    }

  }

  private int GetNumberOfNeighboringMonsters(Vector3 targetpos) {
    //given a position, how many monster tiles are adjacent to this one
    int adjacent = 0;

    Godot.Collections.Array tiles = GetTree().GetNodesInGroup("tiles");
    foreach (Node t in tiles) {
      // SingleTile tile = t as SingleTile;
      Position3D pos = t.GetParent() as Position3D;
      // GD.Print(tile.Translation);
      if(t is MonsterFootprintTile) {
        float dist = pos.Translation.DistanceTo(targetpos);
        if(dist < 2.2f) {
          adjacent++;
        }
      }
    }

    return adjacent;
  }

  private float GetDistanceToNearestMonster(Vector3 targetpos) {
    //given a position, return the nearest monster tile distance in game units.
    float minDistance = 30.0f;
    
    Godot.Collections.Array tiles = GetTree().GetNodesInGroup("tiles");
    foreach (Node t in tiles) {
      // SingleTile tile = t as SingleTile;
      Position3D pos = t.GetParent() as Position3D;
      // GD.Print(tile.Translation);
      if(t is MonsterFootprintTile) {
        float dist = pos.Translation.DistanceTo(targetpos);
        if(dist < minDistance) {
          minDistance = dist;
        }
      }
    }
    return minDistance;
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
  // after each input is resolved, do gameState things (process tiles, apply damage, collect items, whatever)
  // *then* resolve the next item in the queue. Only one queued move allowed, and any new clicks while waiting *replace* the queued move.

  //singleton pattern.
  private static GameController instance;
  public static GameController GetInstance() {
    return instance;
  }

}
