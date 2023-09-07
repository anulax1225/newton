using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;


namespace Raylib.RaylibUtile
{
    public class RenderManager
    {
        public bool isRendering = false;
        public Scene scene = new Scene();
        private Camera2D cam = new Camera2D();
        public void Init(Scene scene)
        {
            // Initialization of the camera
            this.scene = scene;
            this.scene.referencial = this.cam.target;
            this.cam.offset = new Vector2(this.scene.sceneSize.X / 2, this.scene.sceneSize.Y / 2);
            this.cam.zoom = 0.5f;
            this.isRendering = true;
            InitWindow((int)this.scene.sceneSize.X, (int)this.scene.sceneSize.Y, "Simu de Newton");
            // Set our game to run at 60 frames-per-second
            SetTargetFPS(60);
        }
        public void RenderFrame()
	    {
            if (!WindowShouldClose())
            {
                BeginDrawing();
                BeginMode2D(this.cam);
                ClearBackground(scene.backGroundColor);
                foreach (GameObject gameObj in this.scene.lstGameObjects)
                {
                    if (!gameObj.isHidden)
                    {
                        gameObj.Render(this);
                    }
                }
                EndMode2D();
                EndDrawing();
            } 
            else
            {
                // De-Initialization
                this.isRendering = false;
            }
	    }
        public void Close()
        {
            this.isRendering = false;
            CloseWindow();
        }
        public Vector2 WorldToScreen(Vector2 position)
        {
            Vector2 relativPos = position + this.scene.referencial;
            return relativPos;
        }
        public Vector2 ScreenToWorld(Vector2 position)
        {
            Vector2 relativPos = position - this.scene.referencial;
            return relativPos;
        }
    }
}
