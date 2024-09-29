﻿using BlasterMaster.Main.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BlasterMaster.Main.UI.Buttons;

public class Button : UIElement
{
    public Color NormalColor = Color.White;
    public Color SelectedColor = Color.Yellow;

    public Button(Vector2 position, string text, SpriteFont font, Action? onClick) : 
        base(position, text, font)
    {
        OnClick += onClick;
        
        DrawColor = NormalColor;
        
        OnStartHovering = () => { PlayTickSound(); Select(); };
        OnEndHovering = Deselect;
    }

    private void PlayTickSound()
    {
        SoundEngine.PlaySound(SoundID.Tick);
    }

    private void Select()
    {
        DrawColor = SelectedColor;
    }

    private void Deselect()
    {
        DrawColor = NormalColor;
    }
}