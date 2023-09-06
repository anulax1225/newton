using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newton;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Raylib.RaylibUtile
{
    public abstract class GameObject
    {
        public string name = "";
        public bool isHidden = false;
        public bool isEnabled = true;
        public Vector2 position;
        public Color color;
        public abstract void Render(RenderManager rdManager);
        public abstract bool CheckCollison(Vector2 ptn, Scene scene);
        public abstract void OnEvent(InputHandler inController);
    }
    public class Scene
    {
        public Vector2 referencial = new Vector2(0,0);
        public Color backGroundColor = Color.WHITE;
        public Vector2 sceneSize = new Vector2(0,0);
        public float zoom = 1f;
        public List<GameObject> lstGameObjects = new List<GameObject>();
        public void AddGameObject(GameObject gameObj) { lstGameObjects.Add(gameObj); }
        public void RemoveGameObject(GameObject gameObj) { lstGameObjects.Remove(gameObj); }
    }
}
