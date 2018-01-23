using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AgentAreaDef : MonoBehaviour {


    public IList<float[]> agentArea = new List<float[]>();
    public IList<int> weight = new List<int>();
    public int agentAmount = 200;

    IList<float> pArea;
    enum AC {Xmin, Xmax, Zmin, Zmax}; //Area Coordination Index

    GameObject sphere;


    void Start() {
        sphere = Resources.Load("Agent") as GameObject;
        float s = 0;

        pArea = weight.Select<int, float>(w => s += w).ToList();
        pArea = pArea.Select(w => w / s).ToList();

        for(int i = 0; i < agentAmount; i++)
        {
            Vector3 agentLoc = GenerateRandLoc();
            Vector3 destLoc = GenerateRandLoc();

            PlaceAgentOn(agentLoc, destLoc);
        }
    }

    Vector3 GenerateRandLoc()
    {
        int trailCount = 0;
        do
        {
            float rnd = UnityEngine.Random.value;
            float xrnd = UnityEngine.Random.value;
            float zrnd = UnityEngine.Random.value;

            int indexedBlk = pArea
                .Select(w => w > rnd)
                .ToList().IndexOf(true);

            float[] selArea = agentArea[indexedBlk];

            float xpos = xrnd * (selArea[(int)AC.Xmax] - selArea[(int)AC.Xmin]) + selArea[(int)AC.Xmin];
            float zpos = zrnd * (selArea[(int)AC.Zmax] - selArea[(int)AC.Zmin]) + selArea[(int)AC.Zmin];

            if (!Physics.CheckSphere(new Vector3(xpos, 0.5f, zpos), 0.5f))
            {
                return new Vector3(xpos, 0f, zpos);
            }

            trailCount++;

        } while (trailCount < 50);

        throw new TimeoutException();
    }


    void PlaceAgentOn(Vector3 location, Vector3 destination)
    {
        GameObject agent = Instantiate(sphere) as GameObject;
        agent.name = "Agent";
        agent.transform.position = location;
        agent.transform.parent = transform;

        NavMeshAgent ag = agent.AddComponent<NavMeshAgent>();
        ag.SetDestination(destination);
    }
}
