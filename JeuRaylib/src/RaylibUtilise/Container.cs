using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using VectorUtilises;
using System.Threading.Tasks;
using static Raylib_cs.Raylib;
using Newton;

namespace Raylib.RaylibUtile
{
    public delegate void Callback();
    public delegate void Validation<T>(T input);
    public delegate bool Verifier<T>(T input);
    public abstract class Container : GameObject
    {
        public int fontSize = 40;
        public Vector2 size;
        protected Rectangle border;
        protected Rectangle Generate()
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
        public override bool CheckCollison(Vector2 ptn, Scene scene)
        {
            bool check = CheckCollisionPointRec(ptn, this.border);
            return check;
        }
    }
    public class TextBox : Container
    {
        public string error = "";
        private Verifier<string>? cbVerifier;
        private Validation<string>? cbOnValidation;
        public bool isWriting = false;
        public List<char> inputsBuffer = new List<char>();
        public TextBox(string name)
        {
            this.name = name;
            this.position = new Vector2(0, 0);
            this.size = new Vector2(0, 0);
            this.border = Generate();
        }
        public void SetBehavior(Validation<string> cbOnValidation) { this.cbOnValidation = cbOnValidation; }
        public void SetBehavior(Verifier<string> cbVerifier, Validation<string> cbOnValidation)
        {
            this.cbVerifier = cbVerifier;
            this.cbOnValidation = cbOnValidation;
        }
        public string GetBufferInput()
        {
            return String.Concat(this.inputsBuffer);
        }
        public bool GetVerification()
        {
            if (cbVerifier != null)
            {
                return this.cbVerifier(this.GetBufferInput());
            }
            return true;
        }
        public void OnValidation()
        {
            if (cbOnValidation != null)
            {
                this.cbOnValidation(this.GetBufferInput());
            }
        }
        public void FlushBuffer()
        {
            this.inputsBuffer.Clear();
        }
        public char WriteToBuffer()
        {
            int keyPressed = GetCharPressed();
            Dictionary<KeyboardKey, char> tbKeyBoard = new Dictionary<KeyboardKey, char> {
                { KeyboardKey.KEY_ONE , '1'}, { KeyboardKey.KEY_TWO , '2' }, { KeyboardKey.KEY_THREE , '3'}, { KeyboardKey.KEY_FOUR , '4'}, { KeyboardKey.KEY_FIVE, '5' }, { KeyboardKey.KEY_SIX , '6'},
                { KeyboardKey.KEY_SEVEN, '7' }, { KeyboardKey.KEY_EIGHT, '8' }, { KeyboardKey.KEY_NINE, '9' }, { KeyboardKey.KEY_ZERO, '0' }, { KeyboardKey.KEY_A, 'a' }, { KeyboardKey.KEY_B, 'b' },
                { KeyboardKey.KEY_C, 'c' }, { KeyboardKey.KEY_D, 'd' }, { KeyboardKey.KEY_E, 'e' }, { KeyboardKey.KEY_F, 'f' }, { KeyboardKey.KEY_G, 'g' }, { KeyboardKey.KEY_H, 'h' }, { KeyboardKey.KEY_I, 'i' },
                { KeyboardKey.KEY_J, 'j' }, { KeyboardKey.KEY_K, 'k' }, { KeyboardKey.KEY_L, 'l' }, { KeyboardKey.KEY_M, 'm' }, { KeyboardKey.KEY_N, 'n' }, { KeyboardKey.KEY_O, 'o' }, { KeyboardKey.KEY_P, 'p' },
                { KeyboardKey.KEY_Q , 'q'}, { KeyboardKey.KEY_R, 'r' }, { KeyboardKey.KEY_S, 's' }, { KeyboardKey.KEY_T, 't' }, { KeyboardKey.KEY_U, 'u' }, { KeyboardKey.KEY_V, 'v' }, { KeyboardKey.KEY_W, 'w' },
                { KeyboardKey.KEY_X, 'x' }, { KeyboardKey.KEY_Y, 'y' }, { KeyboardKey.KEY_Z, 'z' }, { KeyboardKey.KEY_ENTER, '\0' }
            };
            foreach (var key in tbKeyBoard)
            {
                if (IsKeyPressed(key.Key))
                {
                    this.inputsBuffer.Add(key.Value);
                    return key.Value;
                }
                    
            }
            return '$';
        }
        public override void Render(RenderManager rdManager)
        {
            int textOffsetY = -20;
            Vector2 screenPos = rdManager.WorldToScreen(this.position);
            DrawRectangle((int)screenPos.X, (int)screenPos.Y, (int)this.size.X, (int)this.size.Y, this.color);
            string input = this.GetBufferInput();
            if (this.isWriting && !(input == null))
            {
                int textLenght = MeasureText(input, this.fontSize);
                DrawText(input, (int)(screenPos.X + (this.size.X - textLenght) / 2), (int)(textOffsetY + screenPos.Y + this.size.Y / 2), this.fontSize, Color.WHITE);
            }
            else if (!String.IsNullOrEmpty(this.error))
            {
                int textLenght = MeasureText(this.error, this.fontSize);
                DrawText(this.error, (int)(screenPos.X + (this.size.X - textLenght) / 2), (int)(textOffsetY + screenPos.Y + this.size.Y / 2), this.fontSize, Color.WHITE);
            }
            else
            {
                int textLenght = MeasureText(this.name, this.fontSize);
                DrawText(this.name, (int)(screenPos.X + (this.size.X - textLenght) / 2), (int)(textOffsetY + screenPos.Y + this.size.Y / 2), this.fontSize, Color.WHITE);
            }
        }
        public override void OnEvent(InputHandler inController)
        {
            if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) || this.isWriting) 
            {
                DisableCursor();
                this.fontSize = 40;
                this.isWriting = true;
                this.error = "";
                inController.isProtectedObj = true;
                inController.ProtectedObj = this;
                char input = this.WriteToBuffer();
                if (input == '\0')
                {
                    if (this.GetVerification())
                    {
                        this.OnValidation();
                    }
                    else
                    {
                        this.error = "Typing Error";
                        this.fontSize = 20;
                    }
                    inController.isProtectedObj = false;
                    inController.ProtectedObj = null;
                    this.isWriting = false;
                    this.FlushBuffer();
                    EnableCursor();
                }
            }
        }
    }

    public class Button : Container
    {
        private Callback? cbOnClick;
        public Button(string name)
        {
            this.name = name;
            this.position = new Vector2(0, 0);
            this.size = new Vector2(0, 0);
            this.border = Generate();
        }
        public void SetBehavior(Callback cbOnClick)
        {
            this.cbOnClick = cbOnClick;
        }
        public override void Render(RenderManager rdManager)
        {
            int textOffsetY = -20;
            Vector2 screenPos = rdManager.WorldToScreen(this.position);
            DrawRectangle((int)screenPos.X, (int)screenPos.Y, (int)this.size.X, (int)this.size.Y, this.color);
            int textLenght = MeasureText(this.name, this.fontSize);
            DrawText(this.name, (int)(screenPos.X + (this.size.X - textLenght) / 2), (int)(textOffsetY + screenPos.Y + this.size.Y / 2), this.fontSize, Color.WHITE);
        }
        public override void OnEvent(InputHandler inController)
        {
            if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && cbOnClick != null)
            {
                this.cbOnClick();
            }
        }
    }

    public class TextLabel : Container
    {
        public string title;
        public bool centerStrings = false;
        public List<string> content = new List<string>();
        public TextLabel(string name)
        {
            this.name = name;
            this.title = "";
            this.position = new Vector2(0, 0);
            this.size = new Vector2(0, 0);
            this.border = Generate();
        }
        public void SetContent(string title, List<string> content)
        {
            this.title = title;
            this.content = content;
        }
        public override void Render(RenderManager rdManager)
        {
            int textOffsetY = fontSize + 10;
            Vector2 pos = rdManager.WorldToScreen(this.position);
            int textLenght = MeasureText(this.title, this.fontSize);
            if (centerStrings) pos.X = pos.X + (this.size.X - textLenght) / 2;
            DrawText(title, (int)pos.X, (int)pos.Y, fontSize, Color.WHITE);
            for (int i = 0; i < content.Count(); i++)
            {
                DrawText(content[i], (int)pos.X, (int)pos.Y + textOffsetY * (i + 1), fontSize, Color.WHITE);
            }
        }
        public override void OnEvent(InputHandler inController) { }
    }

    public class LineRenderer : GameObject
    {
        public List<Vector2> ptnsOfLine = new List<Vector2>();   
        public void SetPoints(Vector2 firstPoint, List<Vector2> points, Color color)
        {
            this.position = firstPoint;
            this.ptnsOfLine = points;
            this.color = color;
        }
        public override void Render(RenderManager rdManager)
        {
            Vector2 oldPos = new Vector2(this.position.X, this.position.Y); 
            foreach (Vector2 ptn in this.ptnsOfLine)
            {
                DrawLineV(rdManager.WorldToScreen(oldPos / rdManager.scene.zoom), rdManager.WorldToScreen(ptn / rdManager.scene.zoom), this.color);
                oldPos = ptn;
            }
        }
        public override bool CheckCollison(Vector2 ptn, Scene scene) { return false; }
        public override void OnEvent(InputHandler inController) { }
    }
}
