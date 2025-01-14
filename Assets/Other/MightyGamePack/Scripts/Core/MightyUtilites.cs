﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MightyGamePack
{
    public class MightyUtilites
    {
        ///<summary>
        ///Clears Y component of the vector3 (Eg. (2,5,3) -> (2,0,3))
        ///</summary>
        public static Vector3 ClearY(Vector3 vector) 
        {
            return new Vector3(vector.x, 0, vector.z);
        }

        ///<summary>
        ///Converts vector2 to vector3 moving Y to Z(Eg. (8,1) -> (8,0,1))
        ///</summary>
        public static Vector3 Vec2ToVec3(Vector2 vector) 
        {
            return new Vector3(vector.x, 0, vector.y);
        }

        ///<summary>
        ///Converts vector3 to vector2 moving Z to Y(Eg. (8,3,1) -> (8,1))
        ///</summary>
        public static Vector2 Vec3ToVec2(Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        ///<summary>
        ///Creates unit vector perpendicular to the line defined by two points
        ///</summary>
        public static Vector3 PerpendicularToLine(Vector3 lineStartPoint, Vector3 lineEndPoint)
        {
            Vector3 lineDir = lineStartPoint - lineEndPoint;
            Vector3 perpendicularDir = Vector3.Cross(lineDir, Vector3.up).normalized;
            return perpendicularDir;
        }

        
    }
}

/* Other usefull scripts

//Reversed for
for (int i = enemies.Count - 1; i >= 0; i--)
{
    GameObject objToDestroy = enemies[i];
    enemies.RemoveAt(i);
    Destroy(objToDestroy);       
}

//Classic timer
flota myTimer;
float myTime;
void Update() 
{
    if(myTimer < myTime)
    {
        myTimer += 1 * Time.deltaTime;
    }
    else
    {
        //Do stuff
        myTimer = 0;
    }
}







*/
