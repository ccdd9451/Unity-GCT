using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class XMLParser : MonoBehaviour
{

    [MenuItem("SteerSuite XML Parsing/Geometry Build")]
    static void Build()
    {

        string XmlFilePath = "Assets/Resources/revit.xml";

        string text = File.ReadAllText(XmlFilePath);
        text = text.Replace("><", "> <");
        File.WriteAllText(XmlFilePath, text);

        GameObject cube = Resources.Load("Cube") as GameObject;
        GameObject cylinder = Resources.Load("Cylinder") as GameObject;
        GameObject plane = Resources.Load("Plane") as GameObject;
        XmlReader reader = XmlReader.Create(XmlFilePath);

        GameObject building = new GameObject("Building");
        float groundXMin = 0.0f, groundXMax = 0.0f,
              groundYMin = 0.0f, groundYMax = 0.0f,
              groundZMin = 0.0f, groundZMax = 0.0f;

        reader.ReadToFollowing("worldBounds");
        while (true)
        {
            reader.Read();
            if (reader.IsStartElement())
            {
                switch (reader.Name)
                {
                    case "xmin": groundXMin = reader.ReadElementContentAsFloat(); break;
                    case "xmax": groundXMax = reader.ReadElementContentAsFloat(); break;
                    case "ymin": groundYMin = reader.ReadElementContentAsFloat(); break;
                    case "ymax": groundYMax = reader.ReadElementContentAsFloat(); break;
                    case "zmin": groundZMin = reader.ReadElementContentAsFloat(); break;
                    case "zmax": groundZMax = reader.ReadElementContentAsFloat(); break;
                }
            }
            if (reader.NodeType == XmlNodeType.EndElement)
            {
                break;
            }
        }
        GameObject ground = PrefabUtility.InstantiatePrefab(plane) as GameObject;
        ground.name = "Ground";
        ground.transform.position = new Vector3(groundXMax + groundXMin, groundYMax + groundYMin, groundZMax + groundZMin) / 2.0f;
        ground.transform.localScale = new Vector3(groundXMax - groundXMin, groundYMax - groundYMin, groundZMax - groundZMin) / 20.0f;
        ground.transform.parent = building.transform;
        GameObjectUtility.SetStaticEditorFlags(ground, StaticEditorFlags.NavigationStatic);

        while (reader.Read())
        {
            switch (reader.Name)
            {
                case "orientedBoxObstacle":
                    {
                        float thetaX = 0.0f, thetaY = 0.0f, thetaZ = 0.0f;
                        float scaleX = 1.0f, scaleY = 1.0f, scaleZ = 1.0f;
                        float posX_orientBox = 0.0f, posY_orientBox = 0.0f, posZ_orientBox = 0.0f;
                        while (reader.Read() && reader.Name != "orientedBoxObstacle")
                        {
                            switch (reader.Name)
                            {
                                case "thetaX": thetaX = reader.ReadElementContentAsFloat(); break;
                                case "thetaY": thetaY = reader.ReadElementContentAsFloat(); break;
                                case "thetaZ": thetaZ = reader.ReadElementContentAsFloat(); break;
                                case "size":
                                    while (reader.Read() && reader.Name != "size")
                                    {
                                        switch (reader.Name)
                                        {
                                            case "x": scaleX = reader.ReadElementContentAsFloat(); break;
                                            case "y": scaleY = reader.ReadElementContentAsFloat(); break;
                                            case "z": scaleZ = reader.ReadElementContentAsFloat(); break;
                                        }
                                    }
                                    break;
                                case "position":
                                    while (reader.Read() && reader.Name != "position")
                                    {
                                        switch (reader.Name)
                                        {
                                            case "x": posX_orientBox = reader.ReadElementContentAsFloat(); break;
                                            case "y": posY_orientBox = reader.ReadElementContentAsFloat(); break;
                                            case "z": posZ_orientBox = reader.ReadElementContentAsFloat(); break;
                                        }
                                    }
                                    break;
                            }
                        }
                        GameObject orientedBoxObstacle = PrefabUtility.InstantiatePrefab(cube) as GameObject;
                        orientedBoxObstacle.name = "Oriented Box Obstacle";
                        orientedBoxObstacle.transform.position = new Vector3(posX_orientBox, posY_orientBox + scaleY / 2.0f, posZ_orientBox);
                        orientedBoxObstacle.transform.rotation = Quaternion.Euler(thetaX, thetaY, thetaZ);
                        orientedBoxObstacle.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
                        orientedBoxObstacle.transform.parent = building.transform;
                        GameObjectUtility.SetStaticEditorFlags(orientedBoxObstacle, StaticEditorFlags.NavigationStatic);
                        break;
                    }

                case "obstacle":
                    {
                        float xmin = 0.0f, xmax = 0.0f, ymin = 0.0f, ymax = 0.0f, zmin = 0.0f, zmax = 0.0f;
                        while (reader.Read() && reader.Name != "obstacle")
                        {
                            switch (reader.Name)
                            {
                                case "xmin": xmin = reader.ReadElementContentAsFloat(); break;
                                case "xmax": xmax = reader.ReadElementContentAsFloat(); break;
                                case "ymin": ymin = reader.ReadElementContentAsFloat(); break;
                                case "ymax": ymax = reader.ReadElementContentAsFloat(); break;
                                case "zmin": zmin = reader.ReadElementContentAsFloat(); break;
                                case "zmax": zmax = reader.ReadElementContentAsFloat(); break;
                            }
                        }
                        GameObject obstacle = PrefabUtility.InstantiatePrefab(cube) as GameObject;
                        obstacle.name = "Obstacle";
                        obstacle.transform.position = new Vector3(xmax + xmin, ymax + ymin, zmax + zmin) / 2.0f;
                        obstacle.transform.localScale = new Vector3(xmax - xmin, ymax - ymin, zmax - zmin);
                        obstacle.transform.parent = building.transform;
                        GameObjectUtility.SetStaticEditorFlags(obstacle, StaticEditorFlags.NavigationStatic);
                        break;
                    }

                case "circleObstacle":
                    {
                        float radius = 0.0f, height = 0.0f;
                        float posX_circle = 0.0f, posY_circle = 0.0f, posZ_circle = 0.0f;
                        while (reader.Read() && reader.Name != "circleObstacle")
                        {
                            switch (reader.Name)
                            {
                                case "radius": radius = reader.ReadElementContentAsFloat(); break;
                                case "height": height = reader.ReadElementContentAsFloat(); break;
                                case "position":
                                    while (reader.Read() && reader.Name != "position")
                                    {
                                        switch (reader.Name)
                                        {
                                            case "x": posX_circle = reader.ReadElementContentAsFloat(); break;
                                            case "y": posY_circle = reader.ReadElementContentAsFloat(); break;
                                            case "z": posZ_circle = reader.ReadElementContentAsFloat(); break;
                                        }
                                    }
                                    break;
                            }
                        }
                        GameObject circleObstacle = PrefabUtility.InstantiatePrefab(cylinder) as GameObject;
                        circleObstacle.name = "Circle Obstacle";
                        circleObstacle.transform.position = new Vector3(posX_circle, posY_circle + height / 2.0f, posZ_circle);
                        circleObstacle.transform.localScale = new Vector3(radius, height / 2.0f, radius);
                        circleObstacle.transform.parent = building.transform;
                        GameObjectUtility.SetStaticEditorFlags(circleObstacle, StaticEditorFlags.NavigationStatic);

                        break;
                    }

                
            }
        }
        //NavMeshBuilder.BuildNavMesh();
    }

    [MenuItem("SteerSuite XML Parsing/Config Agents")]
    public static void Parse()
    {
        string XmlFilePath = "Assets/Resources/revit.xml";

        string text = File.ReadAllText(XmlFilePath);
        text = text.Replace("><", "> <");
        File.WriteAllText(XmlFilePath, text);
        XmlReader reader = XmlReader.Create(XmlFilePath);

        GameObject agentArea = new GameObject("agentArea");
        AgentAreaDef agentAreaDef = agentArea.AddComponent<AgentAreaDef>();

        reader.ReadToFollowing("worldBounds");
        while (reader.Read())
        {
            switch (reader.Name)
            {
                case "agentRegion":
                    {
                        int bornWeight = 1, targetWeight = 1;
                        float xmin = 0.0f, xmax = 0.0f, ymin = 0.0f, ymax = 0.0f, zmin = 0.0f, zmax = 0.0f;
                        float x = 0.0f, y = 0.0f, z = 0.0f;
                        while (reader.Read() && reader.Name != "agentRegion")
                        {
                            switch (reader.Name)
                            {
                                case "w_born": bornWeight = reader.ReadElementContentAsInt(); break;
                                case "w_target": targetWeight = reader.ReadElementContentAsInt(); break;
                                case "xmin": xmin = reader.ReadElementContentAsFloat(); break;
                                case "xmax": xmax = reader.ReadElementContentAsFloat(); break;
                                case "ymin": ymin = reader.ReadElementContentAsFloat(); break;
                                case "ymax": ymax = reader.ReadElementContentAsFloat(); break;
                                case "zmin": zmin = reader.ReadElementContentAsFloat(); break;
                                case "zmax": zmax = reader.ReadElementContentAsFloat(); break;
                                case "targetLocation":
                                    while (reader.Read() && reader.Name != "targetLocation")
                                    {
                                        switch (reader.Name)
                                        {
                                            case "x": x = reader.ReadElementContentAsFloat(); break;
                                            case "y": y = reader.ReadElementContentAsFloat(); break;
                                            case "z": z = reader.ReadElementContentAsFloat(); break;
                                        }
                                    }
                                    break;
                            }

                        }
                        agentAreaDef.agentArea.Add(new float[] {xmin, xmax, zmin, zmax});
                        agentAreaDef.bornWeights.Add(bornWeight);
                        agentAreaDef.targetWeights.Add(targetWeight);
                        agentAreaDef.agentAmount = 300;
                        break;
                    }
            }
        }
    }

}
