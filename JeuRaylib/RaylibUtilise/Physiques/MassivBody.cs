/*******************************************************************************************
Projet Raylib pour l'atelier de première saison.
Auteur: Vinayak Ambigapathy
Date: Septembre 2023
********************************************************************************************/
using System.Numerics;
using VectorUtilises;
using Raylib.RaylibUtiles;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Newton;
/// <summary>
/// Class inplementation of a object having gravitation 
/// </summary>
public class MassiveBody : GameObject2D, IRenderable2D, IActivable2D
{
    /// <summary>
    /// Max iteration of gravitationelle interaction before breaking
    /// </summary>
    const int WEIGHT = 100;
    /// <summary>
    /// Universelle gravitationelle constante of all bodys
    /// </summary>
    const float CONSTGRAVITATION = 0.001f;
    /// <summary>
    /// Speed proprety of the body
    /// </summary>
    public Vector2 Speed;
    /// <summary>
    /// Raius proprety of the body
    /// </summary>
    public int Radius;
    /// <summary>
    /// Surface gravity proprety of the body
    /// </summary>
    public float SurfaceG;
    /// <summary>
    /// Masse proprety of the body
    /// </summary>
    public float Masse;
    /// <summary>
    ///  Flag indicating if the parametres of the body should be rendered
    /// </summary>
    public bool ShowParams = false;
    /// <summary>
    /// Flag indicating if the speed vector should be rendered
    /// </summary>
    public bool ShowVector = false;
    /// <summary>
    /// Texture of the planet
    /// </summary>
    public Texture2D texture;
    /// <summary>
    /// Color of the texture
    /// </summary>
    public Color textureColor;
    /// <summary>
    /// Flag indicating if the speed vector is being changed
    /// </summary>
    private bool vectorChange = false;
    /// <summary>
    /// 
    /// </summary>
    private float rotation = 0;
    /// <summary>
    /// Random number generator
    /// </summary>
    private Random rnd = new Random();
    /// <summary>
    /// Gives the rotation speed of the texture
    /// </summary>
    private int rotationSpeed = 0;
    /// <summary>
    /// Instantiation of a body
    /// </summary>
    /// <param name="name">object name</param>
    public MassiveBody(string name)
    {
        this.name = name;
        this.position = new Vector2(0,0);
        this.Speed = new Vector2(0,0);
        this.SurfaceG = 0;
        this.SetRadius(0);
        this.rotationSpeed = rnd.Next(-5, 5);
    }
    /// <summary>
    /// Sets the texture of the object;
    /// </summary>
    /// <param name="texture">texture of the object</param>
    public void SetTextureBody(Texture2D texture)
    {
        this.texture = texture;
    }
    /// <summary>
    /// Sets the radius of the body and changes the masse correspondly.
    /// </summary>
    /// <param name="radius">New radius</param>
    public void SetRadius(int radius)
    {
        this.Radius = radius;
        this.Masse = (this.SurfaceG * this.Radius * this.Radius) / CONSTGRAVITATION;// masse = surface_gravity * radius^2 / gravitationelle_constant
    }
    /// <summary>
    /// Sets the surface gravity of the body and changes the masse correspondly.
    /// </summary>
    /// <param name="surfaceG">New surface gravity</param>
    public void SetSurfaceGravity(float surfaceG)
    {
        this.SurfaceG = surfaceG;
        this.Masse = (this.SurfaceG * this.Radius * this.Radius) / CONSTGRAVITATION;
    }
    /// <summary>
    /// Loops srout all bodys in the list in parameter, 
    /// and changes this speed depending on acceleration du to the attraction with other bodys.    
    /// </summary>
    /// <param name="lstBody">List of bodys to interact whit</param>
    /// <param name="timeStep">Delta time</param>
    public void Gravity(List<MassiveBody> lstBody, float timeStep)
    {
        if (timeStep != 0) 
        {
            int i = 0;
            foreach (MassiveBody body in lstBody)
            {
                if (body != this)
                {
                    Vector2 distance = body.position - this.position;
                    float norme = Vector2Tools.Magnifie(distance);
                    float Fab = CONSTGRAVITATION * ((body.Masse) / norme);//Avec une seul masse 
                    Vector2 Force = Vector2Tools.Normelize(distance) * Fab;
                    Vector2 acceleration = Force / this.Masse;
                    this.Speed += acceleration * timeStep;
                }
                if (i > WEIGHT) break;
                i++;
            }
        }

    }
    /// <summary>
    /// Changes it position depending on dynamiques low. 
    /// </summary>
    /// <param name="timeStep">Delta time</param>
    public void ChangePosSpeed(float timeStep)
    {
        this.position += this.Speed * timeStep;
    }
    /// <summary>
    /// Creates a random name for a body.
    /// </summary>
    /// <returns>Random Name</returns>
    public static string CreatName()
    {
        string[] principal = { "Prime", "Star", "Luna", "Planetrium" };
        char[] chars = { '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'Z', 'Y', 'X', 'C' };
        Random rnd = new Random();
        string rndName = principal[rnd.Next(principal.Length)];
        for (int i = 0; i < rnd.Next(5); i++)
        {
            rndName += chars[rnd.Next(chars.Length)];
        }
        return rndName;
    }
    /// <summary>
    /// Create a rendering process to show the body.
    /// </summary>
    /// <param name="rdManager">Rendering public interface</param>
    public void Render(RenderManager2D rdManager)
    {
        Vector2 pos = rdManager.WorldToScreen(this.position / rdManager.Scene.zoom);
        if (ShowParams)
        {
            float textOffset = 40f;
            int fontSize = 35;
            int textPosX = (int)((pos.X + this.Radius) / rdManager.Scene.zoom + textOffset);
            string[] paramsInfo =
            {
                "Paramètre info",
                String.Format("Name : {0}", this.name),
                String.Format("Masse : {0}", this.Masse),
                String.Format("Radius : {0}", this.Radius),
                String.Format("Surface G : {0}", this.SurfaceG),
                String.Format("Speed : {0}", Vector2Tools.Magnifie(this.Speed)),
            };

            for(int i = 0; i < paramsInfo.Length; i++)
            {
                DrawText(paramsInfo[i], (int)textPosX, (int)(pos.Y + textOffset * (i + 1)), fontSize, this.color);
            }
        }
        if (ShowVector)
        {
            DrawLineV(pos, pos + this.Speed * 20, this.color);
        }
        if(this.Masse > 10000000000)
        {
            Color alphaColor = ColorAlpha(textureColor, 1.0f);
            this.rotation += this.rotationSpeed + Vector2Tools.Magnifie(this.Speed);
            Rectangle source = new Rectangle(0, 0, (float)this.texture.width, (float)this.texture.height);
            Rectangle dest = new Rectangle(pos.X, pos.Y, (this.Radius / rdManager.Scene.zoom) * 8, (this.Radius / rdManager.Scene.zoom) * 8);
            Vector2 origin = new Vector2((this.Radius / rdManager.Scene.zoom) * 4, (this.Radius / rdManager.Scene.zoom) * 4);
            DrawTexturePro(texture, source, dest, origin, this.rotation, alphaColor);
        }
        DrawCircleV(pos, this.Radius / rdManager.Scene.zoom, this.color);
    }
    /// <summary>
    /// Checkes collison of the mouse with the body.
    /// </summary>
    /// <param name="ptn">Mouse position</param>
    /// <param name="scene">Gives access to the scene propeties</param>
    /// <returns>returns true if there is a collision</returns>
    public bool CheckCollison(Vector2 ptn, Scene2D scene)
    {
        ptn *= scene.zoom;
        return CheckCollisionPointCircle(ptn, this.position, this.Radius);
    }
    /// <summary>
    /// This is the event called when a collision is detected.
    /// </summary>
    /// <param name="inController">InputHandling public interface</param>
    public void OnEvent(InputHandler inController)
    {
        if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT) && IsKeyDown(KeyboardKey.KEY_V))
        {
            this.vectorChange = true;
            this.ShowVector = true;
            inController.asPrioritieObj = true;
            inController.PrioritieObj = this;
        } 
        else if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_RIGHT))
        {
            foreach (GameObject2D gameObject in inController.scene.lstGameObjects)
            {
                if (gameObject.GetType() == typeof(MassiveBody) && gameObject != this)
                { 
                    MassiveBody body = (MassiveBody)gameObject;
                    body.ShowParams = false;
                }
            }
            this.ShowParams = !this.ShowParams;
            if (this.ShowParams) inController.LastActivation = this;
        }
        if (IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
        {
            this.vectorChange = false;
            this.ShowVector = false;
            inController.asPrioritieObj = false;
            inController.PrioritieObj = null;
        }
        if (this.vectorChange) 
        { 
            this.Speed = (inController.vMouse - (this.position / inController.scene.zoom)) * -0.1f;
        }
    }
    /// <summary>
    /// Gives information on body
    /// </summary>
    /// <returns>body as string</returns>
    public override string ToString()
    {
        return $"Class MassiveBody| name :{this.name} position :{this.position} speed :{this.Speed}";
    }
}

