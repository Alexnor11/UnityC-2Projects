using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    [Header("Set Dynamically")]
    public Rigidbody rigit;

    private Neighborhood neighborhood;

    private void Awake()
    {
        neighborhood = GetComponent<Neighborhood>();
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

        // �������������� ������������ � �������� ������� �������
        Vector3 velAvoid = Vector3.zero;
        Vector3 tooCloswPos = neighborhood.avgClosePos;
        // ���� ������� ������ Vector3.zero, ������ ������������� �� ����
        if (tooCloswPos != Vector3.zero) 
        {
            velAvoid = pos - tooCloswPos;
            velAvoid.Normalize();
            velAvoid *= spn.velosity;
        }

        // ������������ �������� � ����������� ����������� �������� � ��������
        Vector3 velAlign = neighborhood.avgVel;
        // ������������ ���������, ������ ���� velAlign �� ����� Vector3.zero
        if (velAlign != Vector3.zero) 
        {
            // ��� ���������� ������ �����������, ������� ����������� ��������
            velAlign.Normalize();
            // � ����� ����������� � ��������� ��������
            velAlign *= spn.velosity;
        }

        // ���������� � ������������ �������� � ������� ������� Attractor
        Vector3 delta = Attractor.POS - pos;
        // ���������, ���� ���������, � ������� Attractor ��� �� ����
        bool attracted = (delta.magnitude > spn.attractPushDist);
        Vector3 velAttract = delta.normalized * spn.velosity;

        // ������������ ������� � �������� � ������� ������ ������ �������
        Vector3 velCtnter = neighborhood.avgPos;
        if (velCtnter != Vector3.zero)
        {
            velCtnter -= transform.position;
            velCtnter.Normalize();
            velCtnter *= spn.velosity;
        }


        // ��������� ��� ��������
        float fdt = Time.fixedDeltaTime;

        if(velAvoid != Vector3.zero)
        {
            vel = Vector3.Lerp(vel, velAvoid, spn.collAvoid * fdt);
        }
        else
        {
           if (velAlign != Vector3.zero) 
            {
                vel = Vector3.Lerp(vel, velAlign, spn.velMatching * fdt);
            }
           if (velCtnter!= Vector3.zero)
            {
                vel = Vector3.Lerp(vel, velAlign, spn.flockCentering * fdt);
            }
           if (velAttract != Vector3.zero)
            {
                if (attracted)
                {
                    vel = Vector3.Lerp(vel, velAttract, spn.attractPull *  fdt);
                }else
                {
                    vel = Vector3.Lerp(vel, -velAttract, spn.attractPush * fdt);
                }
            }
        }
        // ���������� vel � ������������ � velocity � �������-�������� Spawner
        vel = vel.normalized * spn.velosity;
        // � ���������� ��������� �������� ���������� Rigidbody
        rigit.velocity = vel;
        LookAhead();  

    }
}
