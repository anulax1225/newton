using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace GUIInterfaceRaylib
{
    public class Button
    {
        public string name;
        public Vector2 position;
        public Vector2 size;
        private Rectangle border;
        public Color color;

        public Button(string name, Vector2 position, Vector2 size, Color color)
        {
            this.name = name;
            this.position = position;
            this.size = size;
            this.color = color;
            this.border = Generate();
        }
        private Rectangle Generate()
        {
            return new Rectangle((int)this.position.X, (int)this.position.Y, (int)this.size.X, (int)this.size.Y);
        }

        public void Resize(Vector2 newSize)
        {
            this.size = newSize;
            this.border = Generate();
        }

        public void Mouv(Vector2 newPosition)
        {
            this.position = newPosition;
            this.border = Generate();
        }

        public Rectangle GetBorder()
        {
            return this.border;
        }
    }
}
