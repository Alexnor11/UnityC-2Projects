using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    [Header("Set Dynamically")]
    public Rigidbody rigit;

    private void Awake()
    {
        rigit = GetComponent<Rigidbody>();

        // ������� ��������� ��������� �������
        pos = Random.insideUnitSphere * Spawner.S.spawnRadius;

        // ������� ��������� ��������� ��������
        Vector3 vel = Random.onUnitSphere * Spawner.S.velosity;
        rigit.velocity = vel;

        LookAhead();

        Color randColor = Color.black;
        while(randColor.r + randColor.g + randColor.b < 1.0)
        {
            randColor = new Color(Random.value, Random.value, Random.value);
        }
        Renderer[] rends = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rends) 
        {
            r.material.color = randColor;
        }
        TrailRenderer tRend = GetComponent<TrailRenderer>();
        tRend.material.SetColor("_TintColor", randColor);
    }

    void LookAhead()
    {
        transform.LookAt(pos + rigit.velocity);
    }
    
    public Vector3 pos
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    private void FixedUpdate()
    {
        Vector3 vel = rigit.velocity;
        Spawner spn = Spawner.S;

        // ���������� � ������������ �������� � ������� ������� Attractor
        Vector3 delta = Attractor.POS - pos;
        // ���������, ���� ���������, � ������� Attractor ��� �� ����
        bool attracted = (delta.magnitude > spn.attractPushDist);
        Vector3 velAttract = delta.normalized * spn.velosity;

        // ��������� ��� ��������
        float fdt = Time.fixedDeltaTime;

        if(attracted)
        {
            vel = Vector3.Lerp(vel, velAttract, spn.attractPull*fdt);
        }
        else
        {
            vel = Vector3.Lerp(vel, -velAttract, spn.attractPull*fdt);
        }
        // ���������� vel � ������������ � velocity � �������-�������� Spawner
        vel = vel.normalized * spn.velosity;
        // � ���������� ��������� �������� ���������� Rigidbody
        rigit.velocity = vel;
        LookAhead();  

    }
}
