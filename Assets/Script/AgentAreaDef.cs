using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AgentAreaDef : MonoBehaviour {

    public IList<float[]> agentArea = new List<float[]>();
    public IList<int> bornWeights = new List<int>();
    public IList<int> targetWeights = new List<int>();
    public int agentAmount = 200;

    IList<float> pBorn, pTarget;
    enum AC {Xmin, Xmax, Zmin, Zmax}; //Area Coordination Index

    public GameObject[] agentPrefabs;

    void Start() {

        int agentCount = 9;
        agentPrefabs = new GameObject[agentCount];
        for (int i = 0; i < agentCount; i++) {
            agentPrefabs[i] = Resources.Load("Agents/Agent" + (i + 1).ToString()) as GameObject;
        }

        float s = 0;

        pArea = weight.Select<int, float>(w => s += w).ToList();
        pArea = pArea.Select(w => w / s).ToList();

        s = 0;
        pBorn = bornWeights.Select<int, float>(w => s += w).ToList();
        pBorn = pBorn.Select(w => w / s).ToList();

        s = 0;
        pTarget = targetWeights.Select<int, float>(w => s += w).ToList();
        pTarget = pTarget.Select(w => w / s).ToList();

        foreach (float w in pBorn)
        {
            Debug.Log(w.ToString());
        }

        for (int i = 0; i < agentAmount; i++)
        {
            Vector3 agentLoc = GenerateRandLoc();
            Vector3 destLoc = GenerateRandLoc(false);

            PlaceAgentOn(agentLoc, destLoc);
        }
    }

    Vector3 GenerateRandLoc(bool isBorn = true)
    {
        int trailCount = 0;
        do
        {
            float rnd = UnityEngine.Random.value;
            float xrnd = UnityEngine.Random.value;
            float zrnd = UnityEngine.Random.value;

            int indexedBlk;
                
            if(isBorn)
            {
                indexedBlk = pBorn
                    .Select(w => w > rnd)
                    .ToList().IndexOf(true);
            }
            else
            {
                indexedBlk = pTarget
                    .Select(w => w > rnd)
                    .ToList().IndexOf(true);
            }

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
        GameObject agent = Instantiate(agentPrefabs[UnityEngine.Random.Range(0, agentPrefabs.Length)]) as GameObject;
        agent.name = "Agent";
        agent.transform.position = location;
        agent.transform.parent = transform;

        NavMeshAgent ag = agent.AddComponent<NavMeshAgent>();
        ag.SetDestination(destination);
    }
}
