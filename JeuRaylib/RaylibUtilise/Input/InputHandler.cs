/*******************************************************************************************
Projet Raylib pour l'atelier de première saison.
Auteur: Vinayak Ambigapathy
Date: Septembre 2023
********************************************************************************************/
using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;
namespace Raylib.RaylibUtiles;

/// <summary>
/// Manages inputs from user and the call back associated
/// </summary>
public class InputHandler
{
    /// <summary>
    /// Scene of the game
    /// </summary>
    public Scene2D scene = new Scene2D();
    /// <summary>
    /// Mouse position in the screen
    /// </summary>
    public Vector2 vMouse = new Vector2(0, 0);
    /// <summary>
    /// Sum of the mouvement with the W S A D keys
    /// </summary>
    public Vector2 targetDirection = new Vector2(0, 0);
    /// <summary>
    /// Referances a game object having request prioritie queue
    /// </summary>
    public IActivable2D? PrioritieObj;
    /// <summary>
    /// Flag indicating if control must be givent to the ProtectedObj Event
    /// </summary>
    public bool asPrioritieObj = false;
    /// <summary>
    /// References the last obj requesting it
    /// </summary>
    public GameObject2D? LastActivation;
    /// <summary>
    /// Scrooling behavior(function excuted when the scrolling happens)
    /// </summary>
    private Callback? cbScrollUP, cbScrollDOWN;
    /// <summary>
    /// List of InputEvents activated at every call
    /// </summary>
    private List<InputEvent> lstInputBinders = new List<InputEvent>();
    /// <summary>
    /// If a GameObject as collided with the mouse adds it to the event queue
    /// </summary>
    private List<IActivable2D> eventQueue = new List<IActivable2D>();
    /// <summary>
    /// Inits the InputHandler with GameObjects but without InputEvents 
    /// </summary>
    /// <param name="scene">Scene of the game</param>
    public void Init(Scene2D scene)
    {
        this.asPrioritieObj = false;
        this.targetDirection = new Vector2(0, 0);
        this.cbScrollUP = null;
        this.cbScrollDOWN = null;
        this.scene = scene;
    }
    /// <summary>
    /// Inits the InputHandler with InputEvents and GameObjects
    /// </summary>
    /// <param name="lstInputBinders">List of InputEvent</param>
    /// <param name="scene">Scene of the game</param>
    public void Init(List<InputEvent> lstInputBinders, Scene2D scene)
    {
        this.asPrioritieObj = false;
        this.targetDirection = new Vector2(0, 0);
        this.cbScrollUP = null;
        this.cbScrollDOWN = null;
        this.lstInputBinders = lstInputBinders;
        this.scene = scene;
    }
    /// <summary>
    /// Sets the scroll UP behavior
    /// </summary>
    /// <param name="callbackUP">Callback function</param>
    public void SetScrollUP(Callback callbackUP)
    {
        this.cbScrollUP = callbackUP;
    }
    /// <summary>
    /// Sets the scroll DOWN behavior
    /// </summary>
    /// <param name="callbackDOWN">Callback function</param>
    public void SetScrollDOWN(Callback callbackDOWN)
    {
        this.cbScrollDOWN = callbackDOWN;
    }
    /// <summary>
    /// Adds an InputEvent to the list 
    /// </summary>
    /// <param name="inputBinder">InputEvent to add</param>
    public void AddInputEvent(InputEvent inputBinder) { lstInputBinders.Add(inputBinder); }
    /// <summary>
    /// Removes an InputEvent from the list 
    /// </summary>
    /// <param name="inputBinder">InputEvent to remove</param>
    public void RemoveInputEvent(InputEvent inputBinder) { lstInputBinders.Remove(inputBinder); }
    /// <summary>
    /// Main event loop
    /// </summary>
    public void InputEvent()
    {
        this.UpdateMousePosition();
        if (!this.asPrioritieObj)
        {
            this.ScrollWheelCheck();
            this.MovementKey();
            if (this.scene.lstGameObjects.Count > 0)
            {
                foreach (GameObject2D gameObject in this.scene.lstGameObjects)
                {
                    try
                    {
                         IActivable2D eventObj = (IActivable2D)gameObject;
                        if (eventObj.CheckCollison(this.vMouse, this.scene) && gameObject.isEnabled)
                        {
                            this.eventQueue.Add(eventObj);
                        }
                    } catch { }
                }
                foreach (IActivable2D eventObj in this.eventQueue) { eventObj.OnEvent(this); }
                eventQueue.Clear();
            }
            if (this.lstInputBinders.Count > 0)
            {
                foreach (InputEvent inputEvent in lstInputBinders)
                {
                    bool gotInput = true;
                    if (inputEvent.KeyBinds != null) CheckBinder(inputEvent.KeyBinds, ref gotInput);
                    if (inputEvent.MouseBinds != null) CheckBinder(inputEvent.MouseBinds, ref gotInput);
                    if (gotInput) inputEvent.callback();
                }
            }
        }
        else if (this.PrioritieObj != null)
        {
            this.PrioritieObj.OnEvent(this);
        }
        else
        {
            this.asPrioritieObj = false;
        }
    }
    /// <summary>
    /// Checks KeyBind dic for correct key strocks
    /// </summary>
    /// <param name="keyBinds">KeyBind dic to check</param>
    /// <param name="gotInput">Ref flag indicating result of check</param>
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
    /// <summary>
    /// Checks MouseBind dic for correct key strocks
    /// </summary>
    /// <param name="mouseBinds">MouseBind dic to check</param>
    /// <param name="gotInput">Ref flag indicating result of check</param>
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
    /// <summary>
    /// Check if the mouvements key have been pressed an ajuste the targetDirection accordingly
    /// </summary>
    private void MovementKey()
    {
        // W S D A are the main mouv keys in games
        if (IsKeyDown(KeyboardKey.KEY_W))
        {
            this.targetDirection.Y += 1 * this.scene.zoom;// Move UP
        }
        else if (IsKeyDown(KeyboardKey.KEY_S))
        {
            this.targetDirection.Y -= 1 * this.scene.zoom;// Move DOWN
        }
        else if (IsKeyDown(KeyboardKey.KEY_A))
        {
            this.targetDirection.X += 1 * this.scene.zoom;// Move LEFT
        }
        else if (IsKeyDown(KeyboardKey.KEY_D))
        {
            this.targetDirection.X -= 1 * this.scene.zoom;// Move RIGHT
        }
        else
        {
            this.targetDirection.X = 0;
            this.targetDirection.Y = 0;
        }
    }
    /// <summary>
    /// Checks for mouse wheel scroll an activates a callback 
    /// if it's up or down an there is a callback
    /// </summary>
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
    /// <summary>
    /// Updates mouse position with offset to fit it in the scene
    /// </summary>
    private void UpdateMousePosition()
    {
        this.vMouse = GetMousePosition();
        this.vMouse *= 2;
        this.vMouse.X -= this.scene.sceneSize.X;
        this.vMouse.Y -= this.scene.sceneSize.Y;
    }
}

/// <summary>
/// Represents a state for a key or mouse input
/// </summary>
public enum InputState
{
    /// <summary>
    /// Activates when input is pressed once
    /// </summary>
    PRESS,
    /// <summary>
    /// Actives while input is pressed
    /// </summary>
    DOWN,
    /// <summary>
    /// Activates when input is relased
    /// </summary>
    REALESED
} 

/// <summary>
/// Class wrapping input binders that are the activators with a callback that is the event  
/// </summary>
public class InputEvent
{
    /// <summary>
    /// Keyboard input binders
    /// </summary>
    public Dictionary<KeyboardKey, InputState>? KeyBinds { get; private set; }
    /// <summary>
    /// Mouse input binders
    /// </summary>
    public Dictionary<MouseButton, InputState>? MouseBinds { get; private set; }
    /// <summary>
    /// Event after input
    /// </summary>
    public Callback callback;
    /// <summary>
    /// Init of a InputBinder
    /// </summary>
    /// <param name="callback">callback after input</param>
    public InputEvent(Callback callback) 
    { 
        this.callback = callback;
    }
    /// <summary>
    /// Generates the dic of Key binds
    /// </summary>
    /// <param name="key">Key Input</param>
    /// <param name="state">State of Input</param>
    private void GenerateKeyBind(KeyboardKey key, InputState state)
    {
        this.KeyBinds = new Dictionary<KeyboardKey, InputState>{ { key, state } };
    }
    /// <summary>
    /// Generates the dic of Mouse binds
    /// </summary>
    /// <param name="mouseButton">Mouse Input</param>
    /// <param name="state">State of Input</param>
    private void GenerateMouseBind(MouseButton mouseButton, InputState state)
    {
        this.MouseBinds = new Dictionary<MouseButton, InputState>{ { mouseButton, state } };
    }
    /// <summary>
    /// Add a Key bind to the dic
    /// </summary>
    /// <param name="key">Key Input</param>
    /// <param name="state">State of Input</param>
    public void AddKeyBinder(KeyboardKey key, InputState state)
    {
        if (this.KeyBinds != null) this.KeyBinds.Add(key, state);
        else this.GenerateKeyBind(key,state);
    }
    /// <summary>
    /// Add a Mouse bind to the dic
    /// </summary>
    /// <param name="mouseButton">Mouse Input</param>
    /// <param name="state">State of Input</param>
    public void AddMouseBinder(MouseButton mouseButton, InputState state)
    {
        if (this.MouseBinds != null) this.MouseBinds.Add(mouseButton, state);
        else this.GenerateMouseBind(mouseButton, state);
    }

}
