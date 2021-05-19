using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Panda;

public class AI : MonoBehaviour
{
    // variaveis
    public Transform player;
    public Transform bulletSpawn;
    public Slider healthBar;   
    public GameObject bulletPrefab;

    NavMeshAgent agent;
    public Vector3 destination; // The movement destination.
    public Vector3 target;      // The position to aim to.
    float health = 100.0f;
    float rotSpeed = 5.0f;

    float visibleRange = 80.0f;
    float shotRange = 40.0f;

    void Start()
    {
        // chamando o componente
        agent = this.GetComponent<NavMeshAgent>();
        // desaceleração do tiro
        agent.stoppingDistance = shotRange - 5; //for a little buffer
        InvokeRepeating("UpdateHealth",5,0.5f);
    }

    void Update()
    {
        // barra de Vida
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);
        // valor da vida
        healthBar.value = (int)health;
        healthBar.transform.position = healthBarPos + new Vector3(0,60,0);
    }

    void UpdateHealth()
    {
        // se a vida for menor que 100, você vai curar
       if(health < 100)
        health ++;
    }

    void OnCollisionEnter(Collision col)
    {
        // se collidir com a tag bullet perde vida
        if(col.gameObject.tag == "bullet")
        {
            health -= 10;
        }
    }

    [Task]
    public void PickRandomDestination()
    {
        // escolhe um local aleatorio
        Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
        // setta o destino
        agent.SetDestination(dest);
        Task.current.Succeed();
    }

    [Task]
    public void MoveToDestination()
    {

        if (Task.isInspected)
        {
            // timer
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);
        }
        // se chegar na posição selecionada a task é um sucesso
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            Task.current.Succeed();
        }
    }

}

