using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManager : MonoBehaviour
{

    const float sprintConstant = 0.02f;
    const float damping = 0.04f;
    const float spread = 0.05f;
    const float z = -1f;

    public GameObject splash;
    public Material mat;
    public GameObject waterMesh;

    public float waterWidth = 20;
    public float waterStartPosition = 0;
    public float waterTopPosition = 0;
    public float waterBottomPosition = -10;

    float baseHeight;
    float leftPosition;
    float waterBase;

    float[] xPositions;
    float[] yPositions;
    float[] velocities;
    float[] accelerations;
    LineRenderer Body;

    GameObject[] meshObjects;
    Mesh[] meshes;
    GameObject[] colliders;

	// Use this for initialization
	void Start ()
    {
        SpawnWater(waterStartPosition, waterWidth, waterTopPosition + transform.position.y, waterBottomPosition);
	}
	
	// Update is called once per frame
	void Update ()
    {
    }

    public void SpawnWater(float left, float width, float top, float bottom)
    {
        int edgecount = Mathf.RoundToInt(width) * 5;
        int nodecount = edgecount + 1;

        Body = gameObject.AddComponent<LineRenderer>();
        Body.material = mat;
        Body.material.renderQueue = 1000;
        Body.numPositions = nodecount;
        Body.startWidth = 0.1f;
        Body.endWidth = 0.1f;


        xPositions = new float[nodecount];
        yPositions = new float[nodecount];
        velocities = new float[nodecount];
        accelerations = new float[nodecount];

        meshObjects = new GameObject[edgecount];
        meshes = new Mesh[edgecount];
        colliders = new GameObject[edgecount];

        baseHeight = top;
        waterBase = bottom;
        leftPosition = left;

        for(int i = 0; i < nodecount; i++)
        {
            yPositions[i] = baseHeight;
            xPositions[i] = leftPosition + width * i / edgecount;
            accelerations[i] = 0;
            velocities[i] = 0;
            Body.SetPosition(i, new Vector3(xPositions[i], yPositions[i], z));
        }

        for(int i = 0; i < edgecount; i++)
        {
            meshes[i] = new Mesh();

            Vector3[] Verticies = new Vector3[4];
            Verticies[0] = new Vector3(xPositions[i], yPositions[i], z);
            Verticies[1] = new Vector3(xPositions[i + 1], yPositions[i + 1], z);
            Verticies[2] = new Vector3(xPositions[i], waterBase, z);
            Verticies[3] = new Vector3(xPositions[i + 1], waterBase, z);

            Vector2[] UVs = new Vector2[4];
            UVs[0] = new Vector2(0, 1);
            UVs[1] = new Vector2(1, 1);
            UVs[2] = new Vector2(0, 0);
            UVs[3] = new Vector2(1, 0);

            int[] tris = new int[6] { 0, 1, 3, 3, 2, 0 };
            meshes[i].vertices = Verticies;
            meshes[i].uv = UVs;
            meshes[i].triangles = tris;

            meshObjects[i] = Instantiate(waterMesh, Vector3.zero, Quaternion.identity);
            meshObjects[i].GetComponent<MeshFilter>().mesh = meshes[i];
            meshObjects[i].transform.parent = transform;

            colliders[i] = new GameObject();
            colliders[i].name = "Trigger";
            colliders[i].AddComponent<BoxCollider2D>();
            colliders[i].transform.parent = transform;
            colliders[i].transform.position = new Vector3(leftPosition + width * (i + 0.5f) / edgecount, baseHeight - 0.5f, 0);
            colliders[i].transform.localScale = new Vector3(width / edgecount, 1, 1);
            colliders[i].GetComponent<BoxCollider2D>().isTrigger = true;
            colliders[i].AddComponent<WaterDetector>();

        }

    }

    void UpdateMeshes()
    {
        for(int i = 0; i < meshes.Length; i++)
        {
            Vector3[] Verticies = new Vector3[4];
            Verticies[0] = new Vector3(xPositions[i], yPositions[i], z);
            Verticies[1] = new Vector3(xPositions[i + 1], yPositions[i + 1], z);
            Verticies[2] = new Vector3(xPositions[i], waterBase, z);
            Verticies[3] = new Vector3(xPositions[i + 1], waterBase, z);

            meshes[i].vertices = Verticies;
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < xPositions.Length; i++)
        {
            float force = sprintConstant * (yPositions[i] - baseHeight) + velocities[i] * damping;
            accelerations[i] = -force;
            yPositions[i] += velocities[i];
            velocities[i] += accelerations[i];
            Body.SetPosition(i, new Vector3(xPositions[i], yPositions[i] + transform.position.y, z));
        }

        float[] leftDeltas = new float[xPositions.Length];
        float[] rightDeltas = new float[xPositions.Length];

        for (int j = 0; j < 8; j++)
        {
            for (int i = 0; i < xPositions.Length; i++)
            {
                if (i > 0)
                {
                    leftDeltas[i] = spread * (yPositions[i] - yPositions[i - 1]);
                    velocities[i - 1] += leftDeltas[i];
                }
                if (i < xPositions.Length - 1)
                {
                    rightDeltas[i] = spread * (yPositions[i] - yPositions[i + 1]);
                    velocities[i + 1] += rightDeltas[i];
                }
            }
        }

        for(int i = 0; i < xPositions.Length; i++)
        {
            if(i > 0)
            {
                yPositions[i - 1] += leftDeltas[i];
            }
            if(i < xPositions.Length - 1)
            {
                yPositions[i + 1] += rightDeltas[i];
            }
        }

        UpdateMeshes();
        RandomWaterNoise();
    }

    public void Splash(float xpos, float velocity)
    {
        if(xpos >= xPositions[0] && xpos <= xPositions[xPositions.Length - 1])
        {
            xpos -= xPositions[0];
            int index = Mathf.RoundToInt((xPositions.Length-1) * (xpos / (xPositions[xPositions.Length-1] - xPositions[0])));

            velocities[index] = velocity;

            //float lifetime = 0.93f + Mathf.Abs(velocity) * 0.07f;
            //splash.GetComponent<ParticleSystem>().startSpeed = 8 + 2 * Mathf.Pow(Mathf.Abs(velocity), 0.5f);
            //splash.GetComponent<ParticleSystem>().startSpeed = 9 + 2 * Mathf.Pow(Mathf.Abs(velocity), 0.5f);
            //splash.GetComponent<ParticleSystem>().startLifetime = lifetime;

            //Vector3 position = new Vector3(xPositions[index], yPositions[index] - 0.35f, 5);
            //Quaternion rotation = Quaternion.LookRotation(new Vector3(xPositions[Mathf.FloorToInt(xPositions.Length / 2)], baseHeight + 8, 5));

            //GameObject splish = Instantiate(splash, position, rotation) as GameObject;
            //Destroy(splish, lifetime + 0.3f);

        }
    }

    public void RandomWaterNoise()
    {
        int pos = Mathf.RoundToInt(UnityEngine.Random.Range(0, xPositions.Length - 1));
        float velocity = UnityEngine.Random.Range(0, 0.1f);
        Splash(pos, velocity);
    }

    //private void OnTriggerEnter(Collider2D hit)
    //{
       
    //}
}
