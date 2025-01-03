using Blastia.Main.Blocks.Common;
using Blastia.Main.Entities.HumanLikeEntities;
using Blastia.Main.UI;
using Blastia.Main.UI.Menus.InGame;
using Microsoft.Xna.Framework;

namespace Blastia.Main.GameState;

public class World
{
	private WorldState _state;
	private Player? _myPlayer;
	
	public bool RulerMode;
	private Vector2 _rulerStart;
	private Vector2 _rulerEnd;

	private readonly RulerHighlight _rulerStartHighlight;
	private readonly RulerHighlight _rulerEndHighlight;

	public World(WorldState state)
	{
		_state = state;

		// TODO: clamp line
		_rulerStartHighlight = new RulerHighlight();
		_rulerEndHighlight = new RulerHighlight();

		Awake();
	}

	/// <summary>
	/// Sets World's player and initializes ruler start and end highlight
	/// </summary>
	/// <param name="myPlayer"></param>
	public void SetPlayer(Player myPlayer)
	{
		_myPlayer = myPlayer;
		if (_myPlayer?.Camera == null || BlastiaGame.RulerMenu == null) return;
		
		BlastiaGame.RulerMenu.AddHighlight(_rulerStartHighlight, Vector2.Zero, _myPlayer.Camera);
		BlastiaGame.RulerMenu.AddHighlight(_rulerEndHighlight, Vector2.Zero, _myPlayer.Camera);
	}

	public static float GetBlocksAmount(int width, int height)
	{
		return width * height;
	}
	
	// volume of a block: 1 m3
	// mass of 1 m3 of stone: 1602 kg
	// mass of 1 m3 of dirt: 1300 kg
	// mass of all blocks  (36% stone 24% dirt 40% air): 
	public static double GetMass(int width, int height)
	{
		var amount = GetBlocksAmount(width, height);

		var stoneMass = amount * 0.36 * 1602;
		var dirtMass = amount * 0.24 * 1300;
		var mass = stoneMass + dirtMass;

		return mass;
	}

	public void SetRulerStart(Vector2 start) => _rulerStart = start;
	public void SetRulerEnd(Vector2 end) => _rulerEnd = end;
	
	public Vector2 GetRulerStartRoundedToBlocks() => new((float)Math.Floor(_rulerStart.X / Block.Size) * Block.Size, 
		(float)Math.Floor(_rulerStart.Y / Block.Size) * Block.Size);
	public Vector2 GetRulerEndRoundedToBlocks() => new((float)Math.Floor(_rulerEnd.X / Block.Size) * Block.Size, 
		(float)Math.Floor(_rulerEnd.Y / Block.Size) * Block.Size);

	private void Awake()
	{
		
	}
	
	public void DrawRulerLine()
	{
		if (BlastiaGame.RulerMenu == null || _myPlayer?.Camera == null) return;

		var startX = GetRulerStartRoundedToBlocks().X;
		var startY = GetRulerStartRoundedToBlocks().Y;
		
		var xDiff = GetRulerEndRoundedToBlocks().X - GetRulerStartRoundedToBlocks().X;
		var yDiff = GetRulerEndRoundedToBlocks().Y - GetRulerStartRoundedToBlocks().Y;
		
		var blocksX = Math.Abs(xDiff) / 8;
		var xToAdd = 0;
		for (var block = 1; block <= blocksX; block++)
		{
			if (xDiff < 0) xToAdd = block * -8; // go left
			else if (xDiff > 0) xToAdd = block * 8;

			var pos = new Vector2(startX + xToAdd, startY);
			var rulerHighlight = new RulerHighlight();
			rulerHighlight.SetPosition(pos, _myPlayer.Camera);
			
			BlastiaGame.RulerMenu.AddHighlight(rulerHighlight, pos, _myPlayer.Camera);
		}
	}
	
	public void Update()
	{
		if (_myPlayer?.Camera == null || BlastiaGame.RulerMenu == null) return;
		
		BlastiaGame.RulerMenu.UpdateHighlightPosition(_rulerStartHighlight, _rulerStart, _myPlayer.Camera);
		BlastiaGame.RulerMenu.UpdateHighlightPosition(_rulerEndHighlight, _rulerEnd, _myPlayer.Camera);
		BlastiaGame.RulerMenu.Update();
	}
}