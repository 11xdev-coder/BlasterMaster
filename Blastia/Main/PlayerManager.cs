﻿using Blastia.Main.Blocks.Common;
using Blastia.Main.Utilities;
using Blastia.Main.Utilities.ListHandlers;

namespace Blastia.Main;

public enum SaveFolder { Player, World }
public enum Extension { Player, World }

public class PlayerManager : Singleton<PlayerManager>
{
	private string? _playersSaveFolder;
	private string? _worldsSaveFolder;
	
	public PlayerState? SelectedPlayer { get; private set; }
	public WorldState? SelectedWorld { get; private set; }

	public void Initialize(string playersSaveFolder, string worldsSaveFolder)
	{
		_playersSaveFolder = playersSaveFolder;
		_worldsSaveFolder = worldsSaveFolder;
	}

	// TODO: saving in documents ?
	// TODO: event notifications
	// TODO: flatbuffers (remake serialization)
	// TODO: IDisposable
	private void New(SaveFolder folderType, string name, Extension extensionType, object? data = null)
	{
		string? folder = GetFolder(folderType);
		string extension = GetExtension(extensionType);
		
		if (!string.IsNullOrEmpty(folder))
		{
			string fileName = GetPath(folder, name, extension);

			if (Directory.Exists(folder) && !File.Exists(fileName))
			{
				// save data if provided
				if (data != null) 
				{
					Saving.Save(fileName, data);
				}
				else 
				{
					File.Create(fileName).Close();
				}				
			}
		}
		else throw new Exception("Provided folder path is null.");
	}

	private bool Exists(SaveFolder folderType, string name, Extension extensionType)
	{
		string? folder = GetFolder(folderType);
		string extension = GetExtension(extensionType);
		
		if (!string.IsNullOrEmpty(folder))
		{
			// search at folder/name.extension
			string fileName = GetPath(folder, name, extension);
			return File.Exists(fileName);
		}
		
		throw new Exception("Provided folder path is null.");
	}

	private List<T> LoadAll<T>(SaveFolder folderType, Extension extensionType)
		where T : new()
	{
		string? folder = GetFolder(folderType);
		string extension = GetExtension(extensionType);
		
		if (!string.IsNullOrEmpty(folder))
		{
			List<T> items = new List<T>();

			// go through each file
			foreach (string file in Directory.GetFiles(folder))
			{
				// if correct extension
				if (file.EndsWith(extension))
				{
					// load new instance
					var loadedState = Saving.Load<T>(file);
					items.Add(loadedState);				
				}
			}

			return items;
		}
		
		// return nothing if path is not initialized
		return new List<T>();
	}

	private string GetPath(string folder, string name, string extension)
	{
		// Players/Name.bmplr or Worlds/Name.bmwld
		return Path.Combine(folder, name + extension);
	}
	
	private string? GetFolder(SaveFolder folderType) 
	{
		string? folder = folderType switch 
		{
			SaveFolder.Player => _playersSaveFolder,
			SaveFolder.World => _worldsSaveFolder,
			_ => ""
		};
		
		return folder;
	}
	
	private string GetExtension(Extension extensionType) 
	{
		string extension = extensionType switch
		{
			Extension.Player => ".bmplr",
			Extension.World => ".bmwld",
			_ => ""
		};
		
		return extension;
	}
	
	// PLAYER
	public void NewPlayer(string playerName) 
	{
		PlayerState playerData = new() 
		{
			Name = playerName
		};
		New(SaveFolder.Player, playerName, Extension.Player, playerData);
	}
	public bool PlayerExists(string playerName) => Exists(SaveFolder.Player, playerName, Extension.Player);
	public List<PlayerState> LoadAllPlayers() => LoadAll<PlayerState>(SaveFolder.Player, Extension.Player);

	public void SelectPlayer(PlayerState playerState)
	{
		SelectedPlayer = playerState;
	}
	
	// WORLD
	public void NewWorld(string worldName, WorldDifficulty difficulty = WorldDifficulty.Easy, 
			int worldWidth = 0, int worldHeight = 0) 
	{
		WorldState worldData = new WorldState 
		{ 
			Name = worldName, 
			Difficulty = difficulty,
			WorldWidth = worldWidth,
			WorldHeight = worldHeight,
			Tiles = new ushort[worldWidth * worldHeight]
		};
		GenerateWorldTiles(worldData);
		
		New(SaveFolder.World, worldName, Extension.World, worldData);
	}
	
	public bool WorldExists(string worldName) => Exists(SaveFolder.World, worldName, Extension.World);
	public List<WorldState> LoadAllWorlds() => LoadAll<WorldState>(SaveFolder.World, Extension.World);
		
	public void SelectWorld(WorldState worldState)
	{
		SelectedWorld = worldState;
		BlastiaGame.RequestWorldInitialization();
	}
	
	private void GenerateWorldTiles(WorldState worldState) 
	{
		int width = worldState.WorldWidth;
		int height = worldState.WorldHeight;
		
		for (int x = 0; x < width; x++) 
		{
			for (int y = 0; y < height; y++) 
			{
				worldState.SetTile(x, y, BlockID.Stone);
			}
		}
	}
}

[Serializable]
public class PlayerState
{
	public string Name { get; set; } = "";
	public override string ToString() => Name;
}

[Serializable]
public class WorldState
{
	public string Name { get; set; } = "";
	public override string ToString() => Name;
	public WorldDifficulty Difficulty { get; set; } = WorldDifficulty.Easy;
	
	// 1D to support serialization
	public ushort[] Tiles { get; set; } = Array.Empty<ushort>();
	public int WorldWidth { get; set; }
	public int WorldHeight { get; set; }
	
	public ushort GetTile(int x, int y) 
	{
		return Tiles[y * WorldWidth + x];
	}
	
	 public void SetTile(int x, int y, ushort value)
	{
		Tiles[y * WorldWidth + x] = value;
	}
}