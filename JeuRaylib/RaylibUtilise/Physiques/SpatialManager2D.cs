/*******************************************************************************************
Projet Raylib pour l'atelier de première saison.
Auteur: Vinayak Ambigapathy
Date: Septembre 2023
********************************************************************************************/
using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Diagnostics;
using System.Numerics;


namespace Newton;
/// <summary>
/// Manages all bodys in the scene and the anticipation of there mouves with a line
/// </summary>
public class SpatialManager2D
{
    /// <summary>
    /// Delta time  
    /// </summary>
    public float timeStep = 1f;
    /// <summary>
    /// Flag indicating if every thing is simulated relativ to a body
    /// </summary>
    public bool isCamFixed = false;
    /// <summary>
    /// Flag indicating if anticipated targectory should be computed
    /// </summary>
    public bool doSimulation = false; 
    /// <summary>
    /// List of lineRenderers
    /// </summary>
    public List<LineRenderer> lstOfAnticipationLines = new List<LineRenderer>();
    /// <summary>
    /// List of all the bodyes in the scene
    /// </summary>
    public List<MassiveBody> lstBodys = new List<MassiveBody>();
    /// <summary>
    /// Map of computed position in the futur
    /// </summary>
    private Vector2[][] lstSimuPosition = new Vector2[0][];
    /// <summary>
    /// Scene of the game
    /// </summary>
    private Scene2D scene = new Scene2D();
    /// <summary>
    /// Random number generator
    /// </summary>
    private Random rnd = new Random();
    /// <summary>
    /// Initialisis the SpatialManager with the game scene
    /// </summary>
    /// <param name="scene">Game scene</param>
    public void Init(Scene2D scene) 
    { 
        this.scene = scene;
    }
    /// <summary>
    /// Generates a New Body with random parameters
    /// </summary>
    public void GenerateBody(Texture2D texture)
    {
        LineRenderer newLine = new LineRenderer();
        MassiveBody body = new MassiveBody(MassiveBody.CreatName());
        body.SetTextureBody(texture);
        body.position = new Vector2(rnd.Next(1000) - rnd.Next(1000), rnd.Next(1000) - rnd.Next(1000));
        body.Radius = rnd.Next(200) + 10;
        body.SetSurfaceGravity(rnd.Next(100) + 10);
        body.Speed = new Vector2(rnd.Next(4) - rnd.Next(4), rnd.Next(4) - rnd.Next(4));
        body.color = RndColor();
        body.textureColor = NuencedColor(body.color, 50);
        this.lstBodys.Add(body);
        this.scene.AddGameObject(body);
        this.lstOfAnticipationLines.Add(newLine);
        this.scene.AddGameObject(newLine);
    }
    /// <summary>
    /// First makes the bodyes interact with all other bodyes to modifie speed,
    /// then makes the bodyes change there position depending on speed,
    /// then recenters every thing reletiv the refbody.
    /// The first two operations are done parrallely on the computer. 
    /// </summary>
    /// <param name="lstBodys">List of all the bodys in the scene</param>
    /// <param name="timeStep">Delta time for the calculation</param>
    /// <param name="relBody">Referencial body</param>
    /// <param name="renderManager">To get the methode enabling transposition of all element from the point of reference</param>
    public void DoInteractionParallel(List<MassiveBody> lstBodys, float timeStep, MassiveBody? relBody, RenderManager2D renderManager)
    {
        int goodIndex = -1;
        GameObject2D[] list = new GameObject2D[lstBodys.Count];
        Parallel.For(0, lstBodys.Count, (index) =>
        {
            lstBodys[index]?.Gravity(lstBodys, timeStep);
        });
        Parallel.For(0, lstBodys.Count, (index) =>
        {
            lstBodys[index]?.ChangePosSpeed(timeStep);
            if (relBody != null && lstBodys[index].name == relBody.name) goodIndex = index;
            list[index] = lstBodys[index];
        }); 
        if(goodIndex >= 0 && goodIndex < list.Length && list[goodIndex] != null) renderManager.MouvRelativ(list[goodIndex], list.ToList());
    }
    /// <summary>
    /// Simulates the positions of the bodys in the futur to render it as a line
    /// </summary>
    /// <param name="relBody">Possible referencial body</param>
    /// <param name="renderManager">To give access to Relativ mouvement of object</param>
    public void SimulateCPU(MassiveBody? relBody, RenderManager2D renderManager)
    {
        if (lstBodys.Count < 30)
        {
            int nbTurns = 20000;
            List<MassiveBody> cpLstBodys = CopyList(this.lstBodys);
            if (cpLstBodys.Count != this.lstSimuPosition.Length)
            {
                lstSimuPosition = new Vector2[cpLstBodys.Count][];
                for (int k = 0; k < cpLstBodys.Count; k++)
                {
                    lstSimuPosition[k] = new Vector2[nbTurns];
                }
            }
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            float timeStep = this.timeStep != 0 ? this.timeStep : 50f;
            for (int i = 0; i < nbTurns; i++)
            {
                this.DoInteractionParallel(cpLstBodys, timeStep, relBody, renderManager);
                Parallel.For(0, cpLstBodys.Count, (index) =>
                {
                    lstSimuPosition[index][i] = new Vector2(cpLstBodys[index].position.X, cpLstBodys[index].position.Y);
                });
            }
            stopwatch.Stop();
            Console.WriteLine($"Time to simulate {stopwatch.ElapsedMilliseconds}");
            Parallel.For(0, lstSimuPosition.Length, (index) =>
            {
                this.lstOfAnticipationLines[index].SetPoints(lstSimuPosition[index][0], lstSimuPosition[index], cpLstBodys[index].color);
            });
        }
    }
    /// <summary>
    /// Gives a random color from the set
    /// </summary>
    /// <returns>Random color</returns>
    private Color RndColor()
    {
        return new Color(rnd.Next(10, 240), rnd.Next(10, 240), rnd.Next(10, 240), 255);
    }
    private static Color NuencedColor(Color color, int nuencing)
    {
        int r, g, b;
        r = color.r;
        g = color.g;
        b = color.b;
        for(int i = 0; i < nuencing; i++)
        {
            if (r > 0)
            {
                r -= 1;
            }
            if (g > 0)
            {
                g -= 1;
            }
            if (b > 0)
            {
                b -= 1;
            }
        }
        return new Color(r, g, b, 255);
    }
    /// <summary>
    /// Makes a copy of a List of MassiveBody and all the bodys in it
    /// </summary>
    /// <param name="ls">List to copy</param>
    /// <returns>Copied list</returns>
    public static List<MassiveBody> CopyList(List<MassiveBody> ls)
    {
        List<MassiveBody> newLs = new List<MassiveBody>();
        foreach (MassiveBody body in ls)
        {
            MassiveBody newBody = new MassiveBody(body.name);
            newBody.position = new Vector2(body.position.X, body.position.Y);
            newBody.Radius = body.Radius;
            newBody.SetSurfaceGravity(body.SurfaceG);
            newBody.Speed = new Vector2(body.Speed.X, body.Speed.Y);
            newBody.color = body.color;
            newLs.Add(newBody);
        }
        return newLs;
    }
}

