/*******************************************************************************************
Projet Raylib pour l'atelier de première saison.
Auteur: Vinayak Ambigapathy
Date: Septembre 2023
********************************************************************************************/
using System.Numerics;
using Raylib_cs;
using Raylib.RaylibUtiles;

namespace Newton;
/// <summary>
/// Game Menu class, staticty typed
/// </summary>
public static class GameMenu
{
    /// <summary>
    /// 
    /// </summary>
    static Scene2D scene = new Scene2D();
    /// <summary>
    /// 
    /// </summary>
    static RenderManager2D render = new RenderManager2D();
    /// <summary>
    /// 
    /// </summary>
    static InputHandler controller = new InputHandler();
    /// <summary>
    /// Main loop of the menu
    /// </summary>
    public static void Main()
    {
        Init();
        while (render.IsRendering)
        {
            controller.InputEvent();
            render.RenderFrame();
        }
    }
    /// <summary>
    /// Init of the menu components
    /// </summary>
    public static void Init()
    {
        scene.sceneSize = new Vector2(1500, 1000);
        scene.backGroundColor = Color.BLACK;

        Button btnStart = new Button("Start");
        btnStart.fontSize = 70;
        btnStart.color = Color.PINK;
        btnStart.Mouv(new Vector2(-150,-0));
        btnStart.Resize(new Vector2(300,150));
        btnStart.SetBehavior(() =>
        {
            GameManager game = new GameManager(1500, 1000, render, scene);
            game.Start();
        });

        Button btnQuit = new Button("Quit");
        btnQuit.fontSize = 70;
        btnQuit.color = Color.PINK;
        btnQuit.Mouv(new Vector2(-150, 200));
        btnQuit.Resize(new Vector2(300, 150));
        btnQuit.SetBehavior(() =>
        {
            render.Close();
        });

        TextLabel lbTitle = new TextLabel("Title");
        lbTitle.centerStrings = true;
        lbTitle.color = Color.GRAY;
        lbTitle.fontSize = 200;
        lbTitle.Mouv(new Vector2(-600, -700));
        lbTitle.Resize(new Vector2(1200, 200));
        lbTitle.SetContent("Gravitarion", new List<string>());

        scene.AddGameObject(btnStart);
        scene.AddGameObject(btnQuit);
        scene.AddGameObject(lbTitle);

        render.Init(scene);
        controller.Init(scene);
    }
}
