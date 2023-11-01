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
/// Class that renders a Text to the screen
/// </summary>
public class TextLabel : Container2D, IRenderable2D
{
    /// <summary>
    /// Title of the text
    /// </summary>
    public string title;
    /// <summary>
    /// Content of the text
    /// </summary>
    public List<string> content = new List<string>();
    /// <summary>
    /// Flag that indicates if the text should be centred
    /// </summary>
    public bool centerStrings = false;
    /// <summary>
    /// Textlabel constructor that takes a name
    /// </summary>
    /// <param name="name">Name of the object</param>
    public TextLabel(string name)
    {
        this.name = name;
        this.title = "";
        this.position = new Vector2(0, 0);
        this.size = new Vector2(0, 0);
        this.collisionBox = Generate();
    }
    /// <summary>
    /// Sets the content of the Textlabel
    /// </summary>
    /// <param name="title"></param>
    /// <param name="content"></param>
    public void SetContent(string title, List<string> content)
    {
        this.title = title;
        this.content = content;
    }
    /// <summary>
    /// Renders the Textlabel
    /// </summary>
    /// <param name="rdManager">Rendering public interface</param>
    public void Render(RenderManager2D rdManager)
    {
        int textOffsetY = fontSize + 10;
        Vector2 pos = rdManager.WorldToScreen(this.position);
        int textLenght = MeasureText(this.title, this.fontSize);
        if (centerStrings) pos.X = pos.X + (this.size.X - textLenght) / 2;
        DrawRectangleRec(this.collisionBox, color);
        DrawText(title, (int)pos.X, (int)pos.Y, fontSize, Color.WHITE);
        for (int i = 0; i < content.Count(); i++)
        {
            DrawText(content[i], (int)pos.X, (int)pos.Y + textOffsetY * (i + 1), fontSize, Color.WHITE);
        }
    }
}
/// <summary>
/// Class for curved line drawing
/// </summary>
public class LineRenderer : GameObject2D, IRenderable2D
{
    /// <summary>
    /// Every point of the line
    /// </summary>
    public Vector2[] ptnsOfLine = new Vector2[0];
    /// <summary>
    /// Sets the point of the line 
    /// </summary>
    /// <param name="firstPoint">First point of the line</param>
    /// <param name="points">An array of all the points of the line</param>
    /// <param name="color">Color of the line</param>
    public void SetPoints(Vector2 firstPoint, Vector2[] points, Color color)
    {
        this.position = firstPoint;
        this.ptnsOfLine = points;
        this.color = color;
    }
    /// <summary>
    /// Renders the line
    /// </summary>
    /// <param name="rdManager">Rendering public interface</param>
    public void Render(RenderManager2D rdManager)
    {
        Vector2 oldPos = this.position;
        foreach (Vector2 ptn in this.ptnsOfLine)
        {
            DrawLineV(rdManager.WorldToScreen(oldPos / rdManager.Scene.zoom), rdManager.WorldToScreen(ptn / rdManager.Scene.zoom), this.color);
            oldPos = ptn;
        }
    }
}
