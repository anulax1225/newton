/*******************************************************************************************
Projet Raylib pour l'atelier de première saison.
Auteur: Vinayak Ambigapathy
Date: Septembre 2023
********************************************************************************************/
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;


namespace Newton;

/// <summary>
/// Class managing all gameobject rendering
/// </summary>
public class RenderManager2D
{
    /// <summary>
    /// Flag indicating if the window is rendereds
    /// </summary>
    public bool IsRendering = false;
    /// <summary>
    /// Scene field
    /// </summary>
    public Scene2D Scene { get { return scene; } }
    /// <summary>
    /// Scene attribut 
    /// </summary>
    private Scene2D scene = new Scene2D();
    /// <summary>
    /// Camera of the world
    /// </summary>
    private Camera2D cam = new Camera2D();
    /// <summary>
    /// Sets the blending type of the textures
    /// </summary>
    private BlendMode blending = BlendMode.BLEND_ALPHA;
    /// <summary>
    /// Initialization of the RenderManager
    /// </summary>
    /// <param name="scene">Scene of the game</param>
    public void Init(Scene2D scene)
    {
        // Initialization of the window and the camera
        this.scene = scene;
        this.Scene.referencial = this.cam.target;
        this.cam.offset = new Vector2(this.Scene.sceneSize.X / 2, this.Scene.sceneSize.Y / 2);
        this.cam.zoom = 0.5f;
        this.IsRendering = true;
        InitWindow((int)this.Scene.sceneSize.X, (int)this.Scene.sceneSize.Y, "Simu de Newton");
        // Set our game to run at 60 frames-per-second
        SetTargetFPS(60);
    }
    /// <summary>
    /// Renders one frame of the scene
    /// </summary>
    public void RenderFrame()
	    {
            if (!WindowShouldClose())
            {
                BeginDrawing();
                BeginBlendMode(blending);
                BeginMode2D(this.cam);
                ClearBackground(this.Scene.backGroundColor);
                foreach (GameObject2D gameObj in this.Scene.lstGameObjects)
                {
                    if (!gameObj.isHidden)
                    {
                        try
                        {
                            IRenderable2D renderObj = (IRenderable2D)gameObj;
                            renderObj.Render(this);
                        }
                        catch { }
                    }
                }
                EndMode2D();
                EndBlendMode();
                EndDrawing();
            } 
            else
            {
                this.IsRendering = false;
            }
	    }
    /// <summary>
    /// Closes the Window
    /// </summary>
    public void Close()
    {
        CloseWindow();
    }
    /// <summary>
    /// Gives the position in Raylib axis systeme 
    /// </summary>
    /// <param name="position">World position</param>
    /// <returns>Real position</returns>
    public Vector2 WorldToScreen(Vector2 position)
    {
        Vector2 relativPos = position + this.Scene.referencial;
        return relativPos;
    }
    /// <summary>
    /// Gives the position relitiv to the camera center
    /// </summary>
    /// <param name="position">Real position</param>
    /// <returns>World position</returns>
    public Vector2 ScreenToWorld(Vector2 position)
    {
        Vector2 relativPos = position - this.Scene.referencial;
        return relativPos;
    }
    /// <summary>
    /// Mouves a list of GameObject relativ to a target GameObject, 
    /// making the target gameObject the center of the scene
    /// </summary>
    /// <param name="targetObj">Referencial Object</param>
    /// <param name="lstGameObj">List of GameObjects to mouve relative</param>
    public void MouvRelativ(GameObject2D targetObj, List<GameObject2D> lstGameObj)
    {
        Vector2 deltaDestination = this.WorldToScreen(targetObj.position);
        deltaDestination *= -1;
        Scene2D.MouvGameObjects(deltaDestination, lstGameObj);
    }
}
