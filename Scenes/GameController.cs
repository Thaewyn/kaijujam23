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
  private Timer tileFlipTimer;

  private float shakeDuration = 0.0f;
  [Export] public float shakeMaxDuration = 1.0f;
  [Export] public float maxShakeH = 5.0f;
  [Export] public float maxShakeV = 5.0f;
  private OpenSimplexNoise noise = new OpenSimplexNoise();

  private AudioStreamPlayer KaijuFootstep;
  private AudioStreamPlayer TileClick;
  private AudioStreamPlayer Ready;
  private AudioStreamPlayer Begin;
  private AudioStreamPlayer Winner;
  private AudioStreamPlayer GameOverVoice;
  private AudioStreamPlayer Loser;
  private AudioStreamPlayer Error;
  private AudioStreamPlayer Confirmation;
  private AudioStreamPlayer PrepareVoice;

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
  private int kaijuCount;
  private int kaijuFlags = 0;

  public override void _Ready() {
    tilePositions = (Position3D) GetNodeOrNull(tileContainer);
    mainCam_actual = (Camera) GetNodeOrNull(mainCamera);
    anim = GetNode<AnimationPlayer>("AnimationPlayer");

    mainMenu = GetNode<CanvasLayer>("MainMenu");
    hud = GetNode<HUD>("HUD");
    tileFlipTimer = GetNode<Timer>("TileHideTimer");

    KaijuFootstep = GetNode<AudioStreamPlayer>("KaijuFootstep");
    TileClick = GetNode<AudioStreamPlayer>("TileClick");
    Ready = GetNode<AudioStreamPlayer>("Ready");
    Begin = GetNode<AudioStreamPlayer>("Begin");
    Winner = GetNode<AudioStreamPlayer>("Winner");
    GameOverVoice = GetNode<AudioStreamPlayer>("GameOverVoice");
    Loser = GetNode<AudioStreamPlayer>("Loser");
    Error = GetNode<AudioStreamPlayer>("Error");
    Confirmation = GetNode<AudioStreamPlayer>("Confirmation");
    PrepareVoice = GetNode<AudioStreamPlayer>("PrepareVoice");

    noise.Seed = (int)GD.Randi();
    noise.Octaves = 4;
    noise.Period = 20.0f;
    noise.Persistence = 0.8f;

    if(tilePositions != null) {
      PopulateTiles();
    }

    //game state stuff
    Init();

    if(instance == null) {
      instance = this;
    } else {
      //duplicate, remove this.
      GD.Print("ERROR - Duplicate GameController Instance.");
      // this.queuefree()
    }
  }

  private void Init() {
    //on first load
    gameState = GAME_STATE.MAIN_MENU;
    player_health = 3;
    difficulty = 1;
    kaijuFlags = 0;
    PrintInstructions();
    Ready.Play();
  }

  private void PrintInstructions(){
    hud.UpdateStatsText("Welcome to Kaiju Sweeper!");
    hud.AppendStatsText("Click a tile to scan - get the distance to the nearest Kaiju!");
    // hud.AppendStatsText("Scans will report their distance from the nearest Kaiju.");
    hud.AppendStatsText("Clicking a Kaiju tile loses the game!");
    hud.AppendStatsText("Right-click a tile with a Kaiju to set a flag!");
    hud.AppendStatsText("If you flag incorrectly, you lose health!");
    hud.AppendStatsText("Once all Kaiju are flagged, you win!");
  }

  private bool Restart() {
    //called when the player wants to play again

    RemoveAllTiles();
    //repopulate tiles?
    PopulateTiles();

    Begin.Play();
    player_health = 3;
    clickCounter = 0;
    kaijuFlags = 0;
    gameState = GAME_STATE.PLAYER_TURN;
    return false;
  }

  private void AllTilesFaceUp() {
    //flip all tiles face up
    Godot.Collections.Array tiles = GetTree().GetNodesInGroup("tiles");
    foreach (Node t in tiles) {
      SingleTile tile = t as SingleTile;
      if(!tile.GetIsFaceUp()) {
        tile.FlipMe();
      }
    }
  }

  private void AllTilesFaceDown() {
    Godot.Collections.Array tiles = GetTree().GetNodesInGroup("tiles");
    foreach (Node t in tiles) {
      SingleTile tile = t as SingleTile;
      if(tile.GetIsFaceUp()) {
        tile.FlipMe();
      }
    }
  }

  private void GameOver() {
    //called when the map / level ends
    GameOverVoice.Play();
    hud.UpdateStatsText("Game Over!\nClick the button to restart!");
    hud.ShowResetButton();
  }

  private void PauseGame() {
    //player told the game to bring up the menu

  }

  private bool RemoveAllTiles() {
    // GD.Print("RemoveAllTiles called.");
    Godot.Collections.Array tiles = GetTree().GetNodesInGroup("tiles");
    foreach (Node t in tiles) {
      t.QueueFree();
      // SingleTile tile = t as SingleTile;
      // if(!tile.GetIsFaceUp()) {
      //   tile.FlipMe();
      // }
    }
    return true;
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
    // GD.Print("Got 'startbuttonpressed' from mainmenu - tell camera to animate.");
    // mainCam_actual.StartGame();
    anim.Play("StartGameSwoosh");
    Begin.Play();
    gameState = GAME_STATE.PLAYER_TURN;
  }

  public void _on_TileHideTimer_timeout() {
    //timer done. Flip all tiles
    AllTilesFaceDown();
    tileFlipTimer.Stop();
  }

  // public void _on_HUD_TestButtonPressed() {
  //   GD.Print("Got 'testbuttonpressed' from hud - test the juice.");
  //   MakeScreenShake();
  // }

  public void MakeScreenShake() {
    shakeDuration = shakeMaxDuration;
  }

  public void _on_HUD_ResetButtonPressed() {
    Restart();
    hud.HideAllButtons();
  }

  public void _on_HUD_PlayagainButtonPressed() {
    Restart();
    hud.HideAllButtons();
  }

  public void _on_HUD_LevelupButtonPressed() {
    difficulty++;
    Restart();
    hud.HideAllButtons();
  }

  public void _on_HUD_DifficultySliderChanged(int val) {
    difficulty = val;
    // GD.Print(difficulty);
  }

  // private void slideInCamera() {
  //   SceneTreeTween tw = GetTree().CreateTween();
  //   tw.TweenProperty(mainCam_actual, "h_offset", 0.0f, shakeDuration).From(maxShakeH).SetTrans(Tween.TransitionType.Bounce).SetEase(Tween.EaseType.Out);
  // }

  private bool PopulateTiles() {
    //for each position in tilePositions
    // spawn a new tile
    int remainingMonsters = difficulty;
    kaijuCount = 0;

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
            kaijuCount++;
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

    //if fewer monsters than difficulty, regenerate board?
    // GD.Print("Final Kaiju count for this gen: "+kaijuCount);

    tileFlipTimer.Start();

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
    // GD.Print("Game Controller received TileFlagAttempt:");
    // GD.Print(whichTile.Name);
    //tile was right-clicked
    if(gameState == GAME_STATE.PLAYER_TURN) {

      if(whichTile is MonsterFootprintTile) {
        //correct! Flag tile.
        Confirmation.Play();
        hud.UpdateStatsText("Correct! It's a Kaiju! Flag this tile.");
        MonsterFootprintTile m = whichTile as MonsterFootprintTile;
        m.FlagTile();
        kaijuFlags++;
        // GD.Print("flags = "+kaijuFlags+", count = "+kaijuCount);
        if(kaijuFlags >= kaijuCount) {
          Winner.Play();
          hud.UpdateStatsText("YOU WIN!");
          hud.AppendStatsText("Click a button to continue! Increase difficulty if you want more possible Kaiju tiles");
          hud.ShowPlayagainButton();
          hud.ShowLevelupButton();
          gameState = GAME_STATE.GAME_OVER;
        }
      } else {
        Error.Play();
        //incorrect. Lose HP
        hud.UpdateStatsText("Incorrect Flag!");
        player_health--;
        // GD.Print("player health: "+player_health);
        if(player_health <= 0) {
          Loser.Play();
          hud.UpdateStatsText("Game Over!");
          hud.AppendStatsText("Missed too many times. Click 'Restart' to try again!");
          hud.ShowResetButton();
          gameState = GAME_STATE.GAME_OVER;
        } else {
          hud.AppendStatsText("You have "+player_health+" misses remaining.");
        }
      }
    }
  }

  public void TileWasClicked(Node whichTile) {
    // GD.Print("Game Controller received TileWasClicked:");
    // GD.Print(whichTile.Name);
    // Vector3 pos = whichTile.GetParent().Transform;
    TileClick.Play();

    if(gameState == GAME_STATE.PLAYER_TURN) {

      if (whichTile is MonsterFootprintTile) {
        // GD.Print("clicked monster footprint, GAME OVER!");
        // do screenshake and things
        SingleTile tile = whichTile as SingleTile;
        tile.FlipMe();
        MakeScreenShake();
        KaijuFootstep.Play();
        GameOver();
      } else {
        //non-monster tile.
        SingleTile tile = whichTile as SingleTile;
        Position3D pos = tile.GetParent() as Position3D;
        // GD.Print(pos.Translation);
        // GD.Print("distance: "+GetDistanceToNearestMonster(pos.Translation));
        float dist = GetDistanceToNearestMonster(pos.Translation);
        // float col = tile.GetColumn();
        // FlipTilesInColumn(col);
        if(!tile.GetIsFaceUp()) {
          //face up logic
          tile.FlipMe();
          clickCounter++;

          hud.UpdateStatsText("Clicks: "+clickCounter);
          hud.AppendStatsText("Nearest Kaiju: "+dist);
        } else {
          //face down logic.
        }

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
        MonsterFootprintTile m = t as MonsterFootprintTile;
        if(!m.IsFlagged()) { //scan only shows distance to un-tagged monsters!
          float dist = pos.Translation.DistanceTo(targetpos);
          if(dist < minDistance) {
            minDistance = dist;
          }
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
        // GD.Print("attempting to call interpolateCallback on tile "+tile.Name+" with i="+i);
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
