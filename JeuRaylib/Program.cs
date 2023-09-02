/*******************************************************************************************
Projet Raylib pour l'atelier de première saison.
Auteur: Vinayak Ambigapathy
********************************************************************************************/

using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Numerics;
using Raylib_cs;
using GUIInterfaceRaylib;
using VectorUtilises;

namespace Newton
{
    public class GameMenu
    {
        public static int Main()
        {
            SpatialManager2DRender spm = new SpatialManager2DRender(1500, 1000);
            spm.Start();
            return 0;
        }
    }
}