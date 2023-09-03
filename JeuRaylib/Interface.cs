using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace GUIInterfaceRaylib
{
    public delegate void Callback();
    public delegate void Validation(string input);
    public delegate bool Verifier(string input);
    public abstract class Container
    {
        public string name = "";
        public string error = "";
        public Vector2 position;
        public Vector2 size;
        protected Rectangle border;
        public Color color;

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

        public bool CheckCollison(Vector2 ptn)
        {
            return CheckCollisionPointRec(ptn, this.border);
        }
    }
    public class TextBox : Container
    {
        private Verifier verifier;
        private Validation cbOnValidation;
        private bool canWrite = false;
        public bool isWriting = false;
        public List<char> inputsBuffer = new List<char>();

        public TextBox(string name, Vector2 position, Vector2 size, Color color, bool canWrite, Verifier verifie, Validation cbOnValidation)
        {
            this.name = name;
            this.position = position;
            this.size = size;
            this.color = color;
            this.canWrite = canWrite;
            this.verifier = verifie;
            this.cbOnValidation = cbOnValidation;
            this.border = Generate();
        }

        public void EnabledWriting()
        {
            this.canWrite = true;
        }

        public bool CanWrite()
        {
            return this.canWrite;
        }

        public void SetStrVerifier(Verifier verifier)
        {
            this.verifier = verifier;
        }

        public string GetBufferInput()
        {
            return String.Concat(this.inputsBuffer);
        }

        public bool GetVerification()
        {
            return this.verifier(GetBufferInput());
        }

        public void OnValidation()
        {
            this.cbOnValidation(this.GetBufferInput());
        }

        public void FlushBuffer()
        {
            this.inputsBuffer.Clear();
        }

        public static char Write() 
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
            foreach (var key  in tbKeyBoard)
            {
                if (IsKeyPressed(key.Key)) return key.Value;
            }
            return '$';
        }
    }

    public class Button : Container
    {
        private Callback cbOnClick;
        public Button(string name, Vector2 position, Vector2 size, Color color, Callback cb)
        {
            this.name = name;
            this.position = position;
            this.size = size;
            this.color = color;
            this.cbOnClick = cb;
            this.border = Generate();
        }

        public void SetCallBack(Callback cbOnClick)
        {
            this.cbOnClick = cbOnClick;
        }
        public void OnClick()
        {
            if (cbOnClick != null)
            {
                this.cbOnClick();
            }
        }
    } 
}
