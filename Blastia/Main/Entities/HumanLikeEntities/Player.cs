﻿using Blastia.Main.Entities.Common;
using Blastia.Main.GameState;
using Blastia.Main.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Blastia.Main.Entities.HumanLikeEntities;

[Entity(Id = EntityID.Player)]
public class Player : HumanLikeEntity
{
	/// <summary>
	/// If True, player will play walking animation and disable all other logic
	/// </summary>
	public bool IsPreview { get; set; }

	private const float ArmMaxAngle = 20;
	private const float LegMaxAngle = 25;
	private const float WalkingAnimationDuration = 0.4f;

	public Camera Camera { get; set; }

	public Player(Vector2 position, float initialScaleFactor = 1f) : base(new Vector2(0, -24), Vector2.Zero,
		new Vector2(-13, -21), new Vector2(13, -21), new Vector2(-6, 21), new Vector2(10, 21))
	{
		SetId(EntityID.Player);
		
		Position = position;
		Camera = new Camera(position)
		{
			DrawWidth = (int) BlastiaGame.ScreenWidth,
			DrawHeight = (int) BlastiaGame.ScreenHeight
		};
		Scale = initialScaleFactor;
		MovementSpeed = 0.25f;
	}

	public override void Update()
	{
		if (IsPreview) PreviewUpdate();
		else RegularUpdate();
	}

	/// <summary>
	/// Update when IsPreview = false
	/// </summary>
	private void RegularUpdate()
	{
		HandleMovement();
		
		Camera.Update();
	}

	private void HandleMovement()
	{
		MovementVector = Vector2.Zero;
		
		Vector2 directionVector = Vector2.Zero;
		Keys[] keys = BlastiaGame.KeyboardState.GetPressedKeys();
		foreach (Keys key in keys)
		{
			MovementMap.TryGetValue(key, out Vector2 newDirectionVector);
			directionVector += newDirectionVector; 
		}
		
		if (directionVector != Vector2.Zero)
			directionVector = Vector2Extensions.Normalize(directionVector);
		
		MovementVector = directionVector * MovementSpeed;
		UpdatePosition();
	}

	/// <summary>
	/// Update called when IsPreview = true
	/// </summary>
	private void PreviewUpdate()
	{
		WalkingAnimation(ArmMaxAngle, LegMaxAngle, WalkingAnimationDuration);
	}
}