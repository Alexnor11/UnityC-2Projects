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

        // Выбрать случайную начальную позицию
        pos = Random.insideUnitSphere * Spawner.S.spawnRadius;

        // Выбрать случайную начальную скорость
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

        // ПРЕДОТВРАЩЕНИЕ СТОЛКНОВЕНИЙ – избегать близких соседей
        Vector3 velAvoid = Vector3.zero;
        Vector3 tooCloswPos = neighborhood.avgClosePos;
        // Если получен вектор Vector3.zero, ничего предпринимать не надо
        if (tooCloswPos != Vector3.zero) 
        {
            velAvoid = pos - tooCloswPos;
            velAvoid.Normalize();
            velAvoid *= spn.velosity;
        }

        // СОГЛАСОВАНИЕ СКОРОСТИ – попробовать согласовать скорость с соседями
        Vector3 velAlign = neighborhood.avgVel;
        // Согласование требуется, только если velAlign не равно Vector3.zero
        if (velAlign != Vector3.zero) 
        {
            // Нас интересует только направление, поэтому нормализуем скорость
            velAlign.Normalize();
            // и затем преобразуем в выбранную скорость
            velAlign *= spn.velosity;
        }

        // ПРИТЯЖЕНИЕ – организовать движение в сторону объекта Attractor
        Vector3 delta = Attractor.POS - pos;
        // Проверить, куда двигаться, в сторону Attractor или от него
        bool attracted = (delta.magnitude > spn.attractPushDist);
        Vector3 velAttract = delta.normalized * spn.velosity;

        // КОНЦЕНТРАЦИЯ СОСЕДЕЙ – движение в сторону центра группы соседей
        Vector3 velCtnter = neighborhood.avgPos;
        if (velCtnter != Vector3.zero)
        {
            velCtnter -= transform.position;
            velCtnter.Normalize();
            velCtnter *= spn.velosity;
        }


        // Применить все скорости
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
        // Установить vel в соответствии с velocity в объекте-одиночке Spawner
        vel = vel.normalized * spn.velosity;
        // В заключение присвоить скорость компоненту Rigidbody
        rigit.velocity = vel;
        LookAhead();  

    }
}
