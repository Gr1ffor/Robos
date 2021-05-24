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

    // metodo de patrulha
    [Task]
    public void PickDestination(int x, int z)
    {
        // pegar as posições x e z
        Vector3 dest = new Vector3(x, 0, z);
        // setta o destino
        agent.SetDestination(dest);
        // fala que foi um sucesso
        Task.current.Succeed();
    }

    [Task]
    // bool para olhar o player
    bool SeePlayer()
    {
        // é a distancia dele menos a do player
        Vector3 distance = player.transform.position - this.transform.position;
        RaycastHit hit;
        // bool para observar a parede
        bool seeWall = false;
        // desenha o raycast
        Debug.DrawRay(this.transform.position, distance, Color.red);
        // se o raycast colidir
        if (Physics.Raycast(this.transform.position, distance, out hit))
        {
            // se o raycast colidir com a tag especifica
            if (hit.collider.gameObject.tag == "wall")
            {
                // ver a parede fica true
                seeWall = true;
            }
        }
        // se inspeciona
        if (Task.isInspected)
        {
            // distancia da wall
            Task.current.debugInfo = string.Format("wall={0}", seeWall);
        }
        // se a distancia for menor que o range visivel e o seewall for falso
        if (distance.magnitude < visibleRange && !seeWall)
        {
            // return como true
            return true;
        }
        // senão voltar como falso
        else
        {
            // return como false
            return false;
        }
    }
    [Task]
    public void TargetPlayer()
    {
        // posição do player
        target = player.transform.position;
        // task foi concluida
        Task.current.Succeed();
    }

    [Task]
    public void LookAtTarget()
    {
        // detecta a direção sendo menor do que a posição do player
        Vector3 direction = target - this.transform.position;
        // deixa a rotação mais suave
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);
        // se estiver inspecionando
        if (Task.isInspected)
        {
            // mostra o angle
            Task.current.debugInfo = string.Format("angle={0}", Vector3.Angle(this.transform.forward, direction));
        }
        // se o this for menor que 5
        if (Vector3.Angle(this.transform.forward, direction) < 5.0f)
        {
            // fala que foi concluida
            Task.current.Succeed();
        }
    }
    [Task]

    public bool Fire()
    {
        // instancia o projetil da bala
        GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        // da a força do impulso para a bala
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000);
        // retorna como verdadeiro
        return true;
    }

    [Task]
    bool Turn(float angle)
    {
        //pega a posição e faz ele virar para o alvo
        var p = this.transform.position + Quaternion.AngleAxis(angle, Vector3.up) * this.transform.forward;
        target = p;
        return true;
    }

}

