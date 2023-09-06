using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Raylib_cs.Raylib;
using System.ComponentModel;
using System.CodeDom.Compiler;
using System.Reflection;

namespace Raylib.RaylibUtile
{
    public class InputHandler
    {
        public Scene scene = new Scene();  
        public Vector2 vMouse = new Vector2(0, 0);
        public Vector2 targetDirection = new Vector2(0, 0);

        public bool isProtectedObj = false;
        public GameObject? ProtectedObj;
        public GameObject? LastActivation;

        private Callback? cbScrollUP, cbScrollDOWN;
        private List<InputEvent> lstInputBinders = new List<InputEvent>();
        private List<GameObject> eventQueue = new List<GameObject>();
        public void Init(Scene scene)
        {
            this.isProtectedObj = false;
            this.targetDirection = new Vector2(0, 0);
            this.cbScrollUP = null;
            this.cbScrollDOWN = null;
            this.scene = scene;
        }
        public void Init(List<InputEvent> lstInputBinders, Scene scene)
        {
            this.isProtectedObj = false;
            this.targetDirection = new Vector2(0, 0);
            this.cbScrollUP = null;
            this.cbScrollDOWN = null;
            this.lstInputBinders = lstInputBinders;
            this.scene = scene;
        }
        public void InputEvent()
        {
            this.UpdateMousePosition();
            if (!this.isProtectedObj)
            {
                this.ScrollWheelCheck();
                this.MovementKey();
                if (this.scene.lstGameObjects.Count > 0)
                {
                    foreach (GameObject gameObject in this.scene.lstGameObjects)
                    {
                        if (gameObject.CheckCollison(this.vMouse, this.scene) && gameObject.isEnabled)
                        {
                            this.eventQueue.Add(gameObject);
                        }
                    }
                    foreach (GameObject gameObject in this.eventQueue) { gameObject.OnEvent(this); }
                    eventQueue.Clear();
                }
                if (this.lstInputBinders.Count > 0)
                {
                    foreach (InputEvent inputEvent in lstInputBinders)
                    {
                        bool gotInput = true;
                        if (inputEvent.KeyBind != null) CheckBinder(inputEvent.KeyBind, ref gotInput);
                        if (inputEvent.MouseBind != null) CheckBinder(inputEvent.MouseBind, ref gotInput);
                        if (gotInput) inputEvent.callback();
                    }
                }
            }
            else if (this.ProtectedObj != null)
            {
                this.ProtectedObj.OnEvent(this);
            }
            else
            {
                this.isProtectedObj = false;
            }
        }
        public void SetScrollUP(Callback callbackUP)
        {
            this.cbScrollUP = callbackUP;
        }
        public void SetScrollDOWN(Callback callbackDOWN)
        {
            this.cbScrollDOWN = callbackDOWN;
        }
        public void AddInputEvent(InputEvent inputBinder) { lstInputBinders.Add(inputBinder); }
        public void RemoveInputEvent(InputEvent inputBinder) { lstInputBinders.Remove(inputBinder); }
        private void CheckBinder(Dictionary<KeyboardKey,InputState> keyBinds, ref bool gotInput)
        {
            foreach (var keyBind in keyBinds)
            {
                if (!gotInput) break;
                switch (keyBind.Value)
                {
                    case InputState.DOWN:
                        if (!IsKeyDown(keyBind.Key)) gotInput = false;
                        break;
                    case InputState.PRESS:
                        if (!IsKeyPressed(keyBind.Key)) gotInput = false;
                        break;
                    case InputState.REALESED:
                        if (!IsKeyReleased(keyBind.Key)) gotInput = false;
                        break;
                }
            }
        }
        private void CheckBinder(Dictionary<MouseButton, InputState> mouseBinds, ref bool gotInput)
        {
            foreach (var mouseBind in mouseBinds)
            {
                if (!gotInput) break;
                switch (mouseBind.Value)
                {
                    case InputState.DOWN:
                        if (!IsMouseButtonDown(mouseBind.Key)) gotInput = false;
                        break;
                    case InputState.PRESS:
                        if (!IsMouseButtonPressed(mouseBind.Key)) gotInput = false;
                        break;
                    case InputState.REALESED:
                        if (!IsMouseButtonReleased(mouseBind.Key)) gotInput = false;
                        break;
                }
            }
        }
        private void MovementKey()
        {
            if (IsKeyDown(KeyboardKey.KEY_W))
            {
                this.targetDirection.Y += 4;
            }
            else if (IsKeyDown(KeyboardKey.KEY_S))
            {
                this.targetDirection.Y -= 4;
            }
            else if (IsKeyDown(KeyboardKey.KEY_A))
            {
                this.targetDirection.X += 4;
            }
            else if (IsKeyDown(KeyboardKey.KEY_D))
            {
                this.targetDirection.X -= 4;
            }
            else
            {
                this.targetDirection.X = 0;
                this.targetDirection.Y = 0;
            }
        }
        private void ScrollWheelCheck()
        {
            float mouseWheelScroll = GetMouseWheelMove();
            if (mouseWheelScroll < 0 && this.cbScrollUP != null)
            {
                this.cbScrollUP();
            }
            if (mouseWheelScroll > 0 && this.cbScrollDOWN != null)
            {
                this.cbScrollDOWN();
            }
        }
        private void UpdateMousePosition()
        {
            this.vMouse = GetMousePosition();
            this.vMouse *= 2;
            this.vMouse.X -= this.scene.sceneSize.X;
            this.vMouse.Y -= this.scene.sceneSize.Y;
        }
    }
    public enum InputState
    {
        PRESS,
        DOWN,
        REALESED
    } 
    public class InputEvent
    {
        public Dictionary<KeyboardKey, InputState>? KeyBind { get; private set; }
        public Dictionary<MouseButton, InputState>? MouseBind { get; private set; }
        public Callback callback;
        public InputEvent(Callback callback) 
        { 
            this.callback = callback;
        }
        private void GenerateKeyBind(KeyboardKey key, InputState state)
        {
            this.KeyBind = new Dictionary<KeyboardKey, InputState>();
            this.KeyBind.Add(key, state);
        }
        private void GenerateMouseBind(MouseButton mouseButton, InputState state)
        {
            this.MouseBind = new Dictionary<MouseButton, InputState>();
            this.MouseBind.Add(mouseButton, state);
        }
        public void AddKeyBinder(KeyboardKey key, InputState state)
        {
            if (this.KeyBind != null) this.KeyBind.Add(key, state);
            else this.GenerateKeyBind(key,state);
        }
        public void AddMouseBinder(MouseButton mouseButton, InputState state)
        {
            if (this.MouseBind != null) this.MouseBind.Add(mouseButton, state);
            else this.GenerateMouseBind(mouseButton, state);
        }

    }
}
