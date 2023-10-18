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
/// 
/// </summary>
public class TextLabel : Container2D, IRenderable2D
{
    /// <summary>
    /// 
    /// </summary>
    public string title;
    /// <summary>
    /// 
    /// </summary>
    public List<string> content = new List<string>();
    /// <summary>
    /// 
    /// </summary>
    public bool centerStrings = false;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    public TextLabel(string name)
    {
        this.name = name;
        this.title = "";
        this.position = new Vector2(0, 0);
        this.size = new Vector2(0, 0);
        this.border = Generate();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="content"></param>
    public void SetContent(string title, List<string> content)
    {
        this.title = title;
        this.content = content;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rdManager"></param>
    public void Render(RenderManager2D rdManager)
    {
        int textOffsetY = fontSize + 10;
        Vector2 pos = rdManager.WorldToScreen(this.position);
        int textLenght = MeasureText(this.title, this.fontSize);
        if (centerStrings) pos.X = pos.X + (this.size.X - textLenght) / 2;
        DrawRectangleRec(this.border, color);
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
    /// 
    /// </summary>
    public Vector2[] ptnsOfLine = new Vector2[0];
    /// <summary>
    /// 
    /// </summary>
    /// <param name="firstPoint"></param>
    /// <param name="points"></param>
    /// <param name="color"></param>
    public void SetPoints(Vector2 firstPoint, Vector2[] points, Color color)
    {
        this.position = firstPoint;
        this.ptnsOfLine = points;
        this.color = color;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rdManager"></param>
    public void Render(RenderManager2D rdManager)
    {
        Vector2 oldPos = new Vector2(this.position.X, this.position.Y);
        foreach (Vector2 ptn in this.ptnsOfLine)
        {
            DrawLineV(rdManager.WorldToScreen(oldPos / rdManager.Scene.zoom), rdManager.WorldToScreen(ptn / rdManager.Scene.zoom), this.color);
            oldPos = ptn;
        }
    }
}
