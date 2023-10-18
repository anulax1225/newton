/*******************************************************************************************
Projet Raylib pour l'atelier de première saison.
Auteur: Vinayak Ambigapathy
Date: Septembre 2023
********************************************************************************************/
using System.Numerics;
using Raylib_cs;

namespace Raylib.RaylibUtiles;
/// <summary>
/// Class representing the base of any objects in the scene
/// </summary>
public class GameObject2D 
{
    /// <summary>
    /// Name of the object
    /// </summary>
    public string name = "";
    /// <summary>
    /// Position in the 2D plan
    /// </summary>
    public Vector2 position;
    /// <summary>
    /// Indecates if it can be interacted with by the user.
    /// If it can it should inplement the interface IActivable2D
    /// </summary>
    public bool isEnabled = true;
    /// <summary>
    /// Indecates if it will be rendered.
    /// If it can it should inplement the interface IRenderable2D 
    /// </summary>
    public bool isHidden = false;
    /// <summary>
    /// Color of the object
    /// </summary>
    public Color color;
}
/// <summary>
/// Defines an interface to render a GameObject
/// </summary>
public interface IRenderable2D
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rdManager"></param>
    public abstract void Render(RenderManager2D rdManager);
}
/// <summary>
/// Defines an interface to interact with a GameObject
/// </summary>
public interface IActivable2D
{
    /// <summary>
    /// Inplements checks for mouse collison
    /// </summary>
    /// <param name="mousePtn">mouse point</param>
    /// <param name="scene">The scene of the object</param>
    /// <returns>result of collison detection</returns>
    public abstract bool CheckCollison(Vector2 mousePtn, Scene2D scene);
    /// <summary>
    /// Inplements the event called when CheckCollison is true
    /// </summary>
    /// <param name="inController">Gives access to some trigger flag etc. to control inputs.</param>
    public abstract void OnEvent(InputHandler inController);
}
/// <summary>
/// Class representing the virtual enviroment in witch all GameObjects are stored 
/// All classes affecting all the GameObjects will use this
/// </summary>
public class Scene2D 
{
    /// <summary>
    /// Represents the offset of the scene from the render axis
    /// </summary>
    public Vector2 referencial = new Vector2(0,0);
    /// <summary>
    /// Back ground color of the scene
    /// </summary>
    public Color backGroundColor = Color.WHITE;
    /// <summary>
    /// Scene size (for screen representation, GameObjects can extend out of the scene)
    /// </summary>
    public Vector2 sceneSize = new Vector2(0,0);
    /// <summary>
    /// Zoom of the scene
    /// </summary>
    public float zoom = 1f;
    /// <summary>
    /// List of all the GameObjects in the scene
    /// </summary>
    public List<GameObject2D> lstGameObjects = new List<GameObject2D>();
    /// <summary>
    /// Adds the new GameObject to the scene
    /// </summary>
    /// <param name="gameObj">GameObject to add</param>
    public void AddGameObject(GameObject2D gameObj) { lstGameObjects.Add(gameObj); }
    /// <summary>
    /// Removes the GameObject from the scene
    /// </summary>
    /// <param name="gameObj">GameObject to remove</param>
    public void RemoveGameObject(GameObject2D gameObj) { lstGameObjects.Remove(gameObj); }
    /// <summary>
    /// Gives access to the GameObject in the scene by there type
    /// </summary>
    /// <typeparam name="T">Type of GameObject</typeparam>
    /// <returns>List of GameObject of type requested</returns>
    public List<GameObject2D> GetListGameObjectFromType<T>()
    {
        List<GameObject2D> lsTypeObj = new List<GameObject2D>();
        foreach (GameObject2D gameObj in lstGameObjects) 
        { 
            if (gameObj.GetType() == typeof(T))
            {
                lsTypeObj.Add(gameObj);
            }
        }
        return lsTypeObj;
    }
    /// <summary>
    /// Translate a list of GameObjects by a distance delta  
    /// </summary>
    /// <param name="deltaDestination">distance to mouv</param>
    /// <param name="lstGameObj">List of GameObjects to translate</param>
    public static void MouvGameObjects(Vector2 deltaDestination, List<GameObject2D> lstGameObj)
    {
        foreach (GameObject2D gameObj in lstGameObj)
        {
            if(gameObj != null) gameObj.position += deltaDestination;
        }
    }
}
