using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PirateAdventures.Input;
using PirateAdventures.Level;
using System;
using System.Collections.Generic;
using System.IO;


namespace PirateAdventures;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D _heroTexture, _tileset;
    private Hero hero;
    private List<Block> _blocks;
    private char[,] gameboard;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.GraphicsProfile = GraphicsProfile.HiDef;  // zorgt ervoor dat we hoger resolutie sprites kunnen gebruiken
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // Add your initialization logic here

        base.Initialize();  // bevat de LoadContent() method, dus na deze lijn zijn de textures geladen

        _blocks = new List<Block>();

        InitializeGameObjects();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // use this.Content to load your game content here
        _heroTexture = Content.Load<Texture2D>("hero");

        _tileset = Content.Load<Texture2D>("tileset64");

    }

    private void InitializeGameObjects()
    {
        hero = new Hero(_heroTexture, new KeyboardInputReader());

        gameboard = readLevelFromFile("./../../../level1.txt");
        for (int i = 0; i < gameboard.GetLength(0); i++)
        {
            for (int j = 0; j < gameboard.GetLength(1); j++)
            {
                _blocks.Add(BlockFactory.CreateBlock(gameboard[i, j], j, i, _tileset, 64));
            }
        }
    }

    private char[,] readLevelFromFile(string filePath)
    {
        int rows = 0, cols = 0;

        using (var reader = File.OpenText(filePath))
        {
            cols = reader.ReadLine().Length;
            rows++;
            while (reader.ReadLine() != null) rows++;
        }

        char[,] stringMatrix = new char[rows, cols];

        try
        {
            using var sr = new StreamReader(filePath);
            for (int i = 0; i < rows; i++)
            {
                var tiles = sr.ReadLine()?.ToCharArray();
                for (int j = 0; j < cols; j++)
                {
                    if (tiles != null) stringMatrix[i, j] = tiles[j];
                }
            }
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
            throw;
        }

        return stringMatrix;
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Add your update logic here
        hero.Update(gameTime);



        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(50, 52, 67));

        // Add your drawing code here
        _spriteBatch.Begin();

        foreach (var block in _blocks)
        {
            block!.Draw(_spriteBatch);
        }

        hero.Draw(_spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
