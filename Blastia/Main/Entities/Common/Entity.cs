﻿using Blastia.Main.Blocks.Common;
using Blastia.Main.GameState;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Object = Blastia.Main.GameState.Object;

namespace Blastia.Main.Entities.Common;

public abstract class Entity : Object
{
    // map for movement keys and their vectors
    protected readonly Dictionary<Keys, Vector2> MovementMap = new()
    {
        {Keys.A, new Vector2(-1, 0)},
        {Keys.D, new Vector2(1, 0)},
        {Keys.W, new Vector2(0, -1)},
        {Keys.S, new Vector2(0, 1)}
    };
    
    // velocity
    protected Vector2 MovementVector;
    protected float MovementSpeed;

    // GRAVITY
    protected virtual bool ApplyGravity { get; set; }
    protected const float Gravity = 9.8f; // G constant
    protected virtual float Mass { get; set; } = 1f; // kg
    
    // HITBOX
    /// <summary>
    /// Entity height in blocks
    /// </summary>
    protected virtual float Height { get; set; }

    protected virtual ushort ID { get; set; }

    protected Entity(Vector2 position, float initialScaleFactor)
    {
        Position = position;
        Scale = initialScaleFactor;
    }

    /// <summary>
    /// Call in constructor to set the ID
    /// </summary>
    /// <param name="id">EntityID</param>
    protected void SetId(ushort id)
    {
        ID = id;
    }

    protected ushort GetId() => ID;

    /// <summary>
    /// Updates position and adds all natural forces
    /// </summary>
    public override void Update()
    {
        ApplyGravityForce();
        UpdatePosition();
    }

    /// <summary>
    /// Adds MovementVector to Position. Call this when Entity should move and MovementVector has been set
    /// </summary>
    private void UpdatePosition()
    {
        Position += MovementVector;
    }
    
    /// <summary>
    /// Newton's law of universal gravitation
    /// </summary>
    private void ApplyGravityForce()
    {
        if (ApplyGravity)
        {
            var currentWorld = PlayerManager.Instance.SelectedWorld;
            var x = Position.X / Block.Size;
            var y = Position.Y / Block.Size + Height; // correct from top-left corner to bottom
            
            // less than 0 -> air
            if (currentWorld != null && currentWorld.GetTile((int) x, (int) y) < 1)
            {
                // Newton's law of universal gravitation
                
            }
        }
    }
}