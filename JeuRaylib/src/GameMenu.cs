/*******************************************************************************************
Projet Raylib pour l'atelier de première saison.
Auteur: Vinayak Ambigapathy
********************************************************************************************/

using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Numerics;
using Raylib_cs;
using Newton;
using VectorUtilises;
using Raylib.RaylibUtile;

namespace Newton
{
    public class GameMenu
    {
        static Scene scene = new Scene();
        static RenderManager render = new RenderManager();
        static InputHandler controller = new InputHandler();
        public static int Main()
        {
            Init();
            while (render.isRendering)
            {
                controller.InputEvent();
                render.RenderFrame();
            }
            return 0;
        }
        public static void Init()
        {
            scene.sceneSize = new Vector2(700, 500);
            scene.backGroundColor = Color.BLACK;

            Button btnStart = new Button("Start");
            btnStart.fontSize = 70;
            btnStart.color = Color.PINK;
            btnStart.Mouv(new Vector2(-150,-100));
            btnStart.Resize(new Vector2(300,200));
            btnStart.SetBehavior(() =>
            {
                render.Close();
                SpatialManager2D spm = new SpatialManager2D(new RenderManager(), new InputHandler());
                spm.Start();
            });

            Button btnQuit = new Button("Quit");
            btnQuit.fontSize = 70;
            btnQuit.color = Color.PINK;
            btnQuit.Mouv(new Vector2(-150, 200));
            btnQuit.Resize(new Vector2(300, 200));
            btnQuit.SetBehavior(() =>
            {
                render.Close();
            });

            TextLabel lbTitle = new TextLabel("Title");
            lbTitle.centerStrings = true;
            lbTitle.color = Color.PINK;
            lbTitle.fontSize = 100;
            lbTitle.Mouv(new Vector2(-250, -400));
            lbTitle.Resize(new Vector2(500, 300));
            lbTitle.SetContent("Gravitarion", new List<string>());

            scene.AddGameObject(btnStart);
            scene.AddGameObject(btnQuit);
            scene.AddGameObject(lbTitle);

            render.Init(scene);
            controller.Init(scene);
        }
    }
}