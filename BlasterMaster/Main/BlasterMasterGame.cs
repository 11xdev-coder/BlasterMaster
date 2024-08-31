﻿using System.Diagnostics;
using BlasterMaster.Main.Sounds;
using BlasterMaster.Main.UI;
using BlasterMaster.Main.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BlasterMaster.Main;

public class BlasterMasterGame : Game
{
    /// <summary>
    /// Graphics manages the graphics device (connection between game and graphics hardware).
    /// Manages things such as resolution, VSync, fullscreen or windowed mode, etc.
    /// Used for low-level graphics operations.
    /// </summary>
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public static float ScreenWidth;
    public static float ScreenHeight;

    public MouseState PreviousMouseState;
    public MouseState CurrentMouseState;
    public static bool HasClickedLeft;

    private Texture2D _cursorTexture;
    public static Vector2 CursorPosition;
    
    private Texture2D _texture;
    
    public MainMenu MainMenu;
    public SpriteFont MainMenuFont;

    private static event Action? ExitRequestEvent;
    
    public BlasterMasterGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        
        // root folder of all assets
        Content.RootDirectory = "Main/Content";
        SoundEngine.Initialize(Content);
        
        ExitRequestEvent += OnExitRequested;
    }

    protected override void Initialize()
    {
        base.Initialize();
        
        _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
        _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
        _graphics.ApplyChanges();
        
        UpdateResolution();
    }
    
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        SoundEngine.LoadSounds();
        
        _cursorTexture = LoadingUtilities.LoadTexture(GraphicsDevice, 
            Content.RootDirectory + Paths.CursorTexturePath);
        
        _texture = LoadingUtilities.LoadTexture(GraphicsDevice, 
            Content.RootDirectory + "/Textures/Toilet.png");

        MainMenuFont = Content.Load<SpriteFont>("Font/Andy_24_Regular");
        MainMenu = new MainMenu(MainMenuFont);
    }

    protected override void UnloadContent()
    {
        base.UnloadContent();
        SoundEngine.UnloadSounds();
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        CurrentMouseState = Mouse.GetState();
        
        HasClickedLeft = CurrentMouseState.LeftButton == ButtonState.Released 
                         && PreviousMouseState.LeftButton == ButtonState.Pressed;
        
        
        CursorPosition = new Vector2(CurrentMouseState.X, CurrentMouseState.Y);
        
        MainMenu.Update();
        
        if(Keyboard.GetState().IsKeyDown(Keys.Escape)) SoundEngine.PlaySound(SoundID.Tick);
        
        // set previous in the end
        PreviousMouseState = CurrentMouseState;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        
        _spriteBatch.Begin();
        _spriteBatch.Draw(_texture, new Vector2(0, 0), Color.White);
        
        MainMenu.Draw(_spriteBatch);
        
        // draw cursor texture last on top of everything
        _spriteBatch.Draw(_cursorTexture, CursorPosition, Color.White);
        _spriteBatch.End();
    }

    private void UpdateResolution()
    {
        ScreenWidth = GraphicsDevice.Viewport.Width;
        ScreenHeight = GraphicsDevice.Viewport.Height;
    }

    private void OnExitRequested()
    {
        Exit();
    }

    public static void RequestExit()
    {
        ExitRequestEvent?.Invoke();
    }
}