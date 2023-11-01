/*******************************************************************************************
Projet Raylib pour l'atelier de première saison.
Auteur: Vinayak Ambigapathy
Date: Septembre 2023
********************************************************************************************/
using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Newton;

/// <summary>
/// UI object that handle data retrival from the user
/// </summary>
public class TextBox : Container2D, IActivable2D, IRenderable2D
{
    /// <summary>
    /// Error message if there is one
    /// </summary>
    public string error = "";
    /// <summary>
    /// Input verifier callback
    /// </summary>
    private Verifier<string>? cbVerifier;
    /// <summary>
    /// Input computer callback
    /// </summary>
    private Validation<string>? cbOnValidation;
    /// <summary>
    /// Flag indicating if the user is actualie writing to the textbox
    /// </summary>
    public bool isWriting = false;
    /// <summary>
    /// List of the char input
    /// </summary>
    public List<char> inputsBuffer = new List<char>();

    /// <summary>
    /// Textbox constructor with the name 
    /// </summary>
    /// <param name="name">Name of the textbox</param>
    public TextBox(string name)
    {
        this.name = name;
        this.position = new Vector2(0, 0);
        this.size = new Vector2(0, 0);
        this.collisionBox = Generate();
    }
    /// <summary>
    /// Sets the textbox behavior without the verification process
    /// </summary>
    /// <param name="cbOnValidation">Callback on validation</param>
    public void SetBehavior(Validation<string> cbOnValidation) { this.cbOnValidation = cbOnValidation; }
    /// <summary>
    ///  Sets the textbox behavior with the verification process
    /// </summary>
    /// <param name="cbVerifier">Callback on verification</param>
    /// <param name="cbOnValidation">Callback on validation</param>
    public void SetBehavior(Verifier<string> cbVerifier, Validation<string> cbOnValidation)
    {
        this.cbVerifier = cbVerifier;
        this.cbOnValidation = cbOnValidation;
    }
    /// <summary>
    /// Gives the input buffer as a string
    /// </summary>
    /// <returns>Input</returns>
    public string GetBufferInput()
    {
        return String.Concat(this.inputsBuffer);
    }
    /// <summary>
    /// Verifies the input
    /// </summary>
    /// <returns>Verication</returns>
    private bool GetVerification()
    {
        if (cbVerifier != null)
        {
            return this.cbVerifier(this.GetBufferInput());
        }
        return true;
    }
    /// <summary>
    /// Compute the input
    /// </summary>
    private void OnValidation()
    {
        if (cbOnValidation != null)
        {
            this.cbOnValidation(this.GetBufferInput());
        }
    }
    /// <summary>
    /// Flush the input buffer
    /// </summary>
    private void FlushBuffer()
    {
        this.inputsBuffer.Clear();
    }
    /// <summary>
    /// Check for keyboard inputs and writes to the input buffer
    /// </summary>
    /// <returns>The char pressed</returns>
    private char WriteToBuffer()
    {
        int keyPressed = GetCharPressed();
        Dictionary<KeyboardKey, char> tbKeyBoard = new Dictionary<KeyboardKey, char> {
            { KeyboardKey.KEY_ONE , '1'}, { KeyboardKey.KEY_TWO , '2' }, { KeyboardKey.KEY_THREE , '3'}, { KeyboardKey.KEY_FOUR , '4'}, { KeyboardKey.KEY_FIVE, '5' }, { KeyboardKey.KEY_SIX , '6'},
            { KeyboardKey.KEY_SEVEN, '7' }, { KeyboardKey.KEY_EIGHT, '8' }, { KeyboardKey.KEY_NINE, '9' }, { KeyboardKey.KEY_ZERO, '0' }, { KeyboardKey.KEY_A, 'a' }, { KeyboardKey.KEY_B, 'b' },
            { KeyboardKey.KEY_C, 'c' }, { KeyboardKey.KEY_D, 'd' }, { KeyboardKey.KEY_E, 'e' }, { KeyboardKey.KEY_F, 'f' }, { KeyboardKey.KEY_G, 'g' }, { KeyboardKey.KEY_H, 'h' }, { KeyboardKey.KEY_I, 'i' },
            { KeyboardKey.KEY_J, 'j' }, { KeyboardKey.KEY_K, 'k' }, { KeyboardKey.KEY_L, 'l' }, { KeyboardKey.KEY_M, 'm' }, { KeyboardKey.KEY_N, 'n' }, { KeyboardKey.KEY_O, 'o' }, { KeyboardKey.KEY_P, 'p' },
            { KeyboardKey.KEY_Q , 'q'}, { KeyboardKey.KEY_R, 'r' }, { KeyboardKey.KEY_S, 's' }, { KeyboardKey.KEY_T, 't' }, { KeyboardKey.KEY_U, 'u' }, { KeyboardKey.KEY_V, 'v' }, { KeyboardKey.KEY_W, 'w' },
            { KeyboardKey.KEY_X, 'x' }, { KeyboardKey.KEY_Y, 'y' }, { KeyboardKey.KEY_Z, 'z' }, { KeyboardKey.KEY_ENTER, '\0' }, { KeyboardKey.KEY_BACKSPACE, '#' }
        };
        foreach (var key in tbKeyBoard)
        {
            if (IsKeyPressed(key.Key))
            {
                if(key.Value == '#')
                {
                    this.inputsBuffer.RemoveAt(this.inputsBuffer.Count - 1);
                } else
                {
                    this.inputsBuffer.Add(key.Value);
                }
                return key.Value;
            }

        }
        return '$';
    }
    /// <summary>
    /// Renders the textbox
    /// </summary>
    /// <param name="rdManager">Rendering public interface</param>
    public void Render(RenderManager2D rdManager)
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
    /// <summary>
    /// Check if the mouse collided with the border
    /// </summary>
    /// <param name="ptn">mouse position</param>
    /// <param name="scene">Game scene</param>
    /// <returns></returns>
    public bool CheckCollison(Vector2 ptn, Scene2D scene)
    {
        bool check = CheckCollisionPointRec(ptn, this.collisionBox);
        return check;
    }
    /// <summary>
    /// Handles input from the user
    /// </summary>
    /// <param name="inController">InputHandling public interface</param>
    public void OnEvent(InputHandler inController)
    {
        if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) || this.isWriting)
        {
            DisableCursor();
            this.fontSize = 40;
            this.isWriting = true;
            this.error = "";
            inController.asPrioritieObj = true;
            inController.PrioritieObj = this;
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
                inController.asPrioritieObj = false;
                inController.PrioritieObj = null;
                this.isWriting = false;
                this.FlushBuffer();
                EnableCursor();
            }
        }
    }
}
/// <summary>
/// UI object that handles click input from the user
/// </summary>
public class Button : Container2D, IActivable2D, IRenderable2D
{
    /// <summary>
    /// 
    /// </summary>
    private Callback? cbOnClick;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    public Button(string name)
    {
        this.name = name;
        this.position = new Vector2(0, 0);
        this.size = new Vector2(0, 0);
        this.collisionBox = Generate();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cbOnClick"></param>
    public void SetBehavior(Callback cbOnClick)
    {
        this.cbOnClick = cbOnClick;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rdManager"></param>
    public void Render(RenderManager2D rdManager)
    {
        int textOffsetY = -20;
        Vector2 screenPos = rdManager.WorldToScreen(this.position);
        DrawRectangle((int)screenPos.X, (int)screenPos.Y, (int)this.size.X, (int)this.size.Y, this.color);
        int textLenght = MeasureText(this.name, this.fontSize);
        DrawText(this.name, (int)(screenPos.X + (this.size.X - textLenght) / 2), (int)(textOffsetY + screenPos.Y + this.size.Y / 2), this.fontSize, Color.WHITE);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ptn"></param>
    /// <param name="scene"></param>
    /// <returns></returns>
    public bool CheckCollison(Vector2 ptn, Scene2D scene)
    {
        bool check = CheckCollisionPointRec(ptn, this.collisionBox);
        return check;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="inController"></param>
    public void OnEvent(InputHandler inController)
    {
        if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && cbOnClick != null)
        {
            this.cbOnClick();
        }
    }
}