using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DragonState
{
    //enum para nombrar todos los estados
    public enum STATE
    {
        CAMINAR,
        DESPEGAR,
        VOLAR,
        ATERRIZAR,
        PEGAR,
        BUSCAR_SONIDO,
        DISPARAR_FUEGO
    };

    //ciclo interno de cada estado
    public enum EVENT
    {
        ENTER,
        UPDATE,
        EXIT
    };

    public STATE name;
    public EVENT stage;
    protected GameObject npc;
    protected Animator anim;
    protected Transform player;
    protected DragonState nextState;
    protected NavMeshAgent agent;

    protected GameObject particleFire; //para el sistema de particulas del fuego

    //LOS DATOS SE CAMBIAN EN El DRAGON CONTROllER
    public float visDist = 20.0f; //distancia de vision
    public float visAngle = 270.0f; //vision de angulo
    public float shootDist = 5f; //rango de ataque
    public float distOutNavMesh = 1.0f;

    public DragonState(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, GameObject _particleFire)
    {
        this.npc = _npc;
        this.agent = _agent;
        this.anim = _anim;
        this.player = _player;
        this.particleFire = _particleFire;
        stage = EVENT.ENTER;
    }

    public virtual void Enter() { stage = EVENT.UPDATE; }
    public virtual void Update() { stage = EVENT.UPDATE; }
    public virtual void Exit() { stage = EVENT.EXIT; }

    //Este metodo es el corazon del FSM(maquina de estados finitos),
    //llama a Enter, Update o Exit segun corresponda
    public DragonState Process()
    {
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }

        return this;
    }

    //si el jugador esta visible
    public bool CanSeePlayer()
    {
        Vector3 direction = player.position - npc.transform.position;
        float angle = Vector3.Angle(direction, npc.transform.forward);

        if (direction.magnitude < visDist && angle < visAngle)
        {
            return true;
        }

        return false;
    }

    //si el jugador esta cerca para atacar
    public bool IsPlayerBehind()
    {
        Vector3 direction = npc.transform.position - player.position;
        float angle = Vector3.Angle(direction, npc.transform.forward);
        if (direction.magnitude < 2.0f && angle < 30.0f) return true;
        return false;
    }

    //si el jugador esta para atacar
    public bool CanAttackPlayer()
    {
        Vector3 direction = player.position - npc.transform.position;
        if (direction.magnitude < shootDist)
        {
            return true;
        }

        return false;
    }

    //Jugador esta dentro del NavMesh
    public bool IsPlayerReachable()
    {
        NavMeshHit hit;
        // Verifica si el jugador está sobre el NavMesh
        return NavMesh.SamplePosition(player.position, out hit, distOutNavMesh, NavMesh.AllAreas);
    }
}

///==============================================================================

/// <summary>
/// ESTADO DE PATRULLAJE ENTRE PUNTOS
/// </summary>
public class DragonCaminar : DragonState
{
    private int currentIndex = -1;
    private bool hasSetInitialDestination = false;

    public DragonCaminar(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, GameObject _particleFire)
        : base(_npc, _agent, _anim, _player, _particleFire)
    {
        name = STATE.CAMINAR;
        agent.speed = 5.0f;
        agent.isStopped = false;
    }

    public override void Enter()
    {
        // Verificar singleton
        if (CheckpointsSingleton.Instance == null || CheckpointsSingleton.Instance.checkpointsDragon == null)
        {
            Debug.LogError("CheckpointsSingleton no está configurado correctamente");
            return;
        }

        float lastDistance = Mathf.Infinity;

        //Encuentra el waypoint más cercano
        for (int i = 0; i < CheckpointsSingleton.Instance.checkpointsDragon.Length; ++i)
        {
            GameObject thisWP = CheckpointsSingleton.Instance.checkpointsDragon[i];
            if (thisWP != null)
            {
                float distance = Vector3.Distance(npc.transform.position, thisWP.transform.position);
                if (distance < lastDistance)
                {
                    currentIndex = i;
                    lastDistance = distance;
                }
            }
        }

        // Establecer destino inicial
        if (currentIndex >= 0 && currentIndex < CheckpointsSingleton.Instance.checkpointsDragon.Length)
        {
            agent.SetDestination(CheckpointsSingleton.Instance.checkpointsDragon[currentIndex].transform.position);
            hasSetInitialDestination = true;
        }

        anim.SetTrigger("esCaminar");
        base.Enter();
    }

    public override void Update()
    {
        // Verificar que tenemos checkpoints válidos
        if (CheckpointsSingleton.Instance == null || CheckpointsSingleton.Instance.checkpointsDragon == null)
        {
            return;
        }

        //Si llega al destino actual, pasa al siguiente checkpoint.
        if (agent.remainingDistance < 1.0f && !agent.pathPending && hasSetInitialDestination)
        {
            if (currentIndex >= CheckpointsSingleton.Instance.checkpointsDragon.Length - 1)
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex++;
            }

            if (currentIndex < CheckpointsSingleton.Instance.checkpointsDragon.Length)
            {
                agent.SetDestination(CheckpointsSingleton.Instance.checkpointsDragon[currentIndex].transform.position);
            }
        }

        if (CanSeePlayer())
        {
            //Si ve al jugador
            nextState = new DragonVolar(npc, agent, anim, player, particleFire);
            stage = EVENT.EXIT;
        }
        else if (IsPlayerBehind())
        {
            //Si el jugador esta por detras
            nextState = new DragonVolar(npc, agent, anim, player, particleFire);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("esCaminar");
        base.Exit();
    }
}

///=================================================================

public class DragonBuscarSonido : DragonState
{
    private Vector3 botella;
    private float investigationTime = 3.0f;
    private float currentInvestigationTime = 0.0f;
    private bool hasReachedDestination = false;

    public DragonBuscarSonido(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, Vector3 _bottle, GameObject _particleFire)
        : base(_npc, _agent, _anim, _player, _particleFire)
    {
        name = STATE.BUSCAR_SONIDO;
        botella = _bottle;
        agent.speed = 6.0f;
        agent.isStopped = false;
    }

    public override void Enter()
    {
        anim.SetTrigger("esCaminar");
        hasReachedDestination = false;
        currentInvestigationTime = 0.0f;

        // Buscar el punto más cercano en el NavMesh cerca de la botella
        NavMeshHit hit;
        if (NavMesh.SamplePosition(botella, out hit, 5.0f, NavMesh.AllAreas))
        {
            //Ir al punto más cercano posible
            agent.SetDestination(hit.position);
        }
        else
        {
            //Si no encuentra punto válido, ir a la posición original
            agent.SetDestination(botella);
        }

        Debug.Log("Dragon buscando sonido en: " + botella);
        base.Enter();
    }

    public override void Update()
    {
        // Prioridad: si ve al jugador, abandona la búsqueda
        if (CanSeePlayer())
        {
            nextState = new DragonVolar(npc, agent, anim, player, particleFire);
            stage = EVENT.EXIT;
            return;
        }

        // Verificar si llegó al destino
        if (!hasReachedDestination && agent.remainingDistance < 2.0f && !agent.pathPending)
        {
            hasReachedDestination = true;
            agent.isStopped = true;
            Debug.Log("Dragon llegó al lugar del sonido, investigando...");
        }

        // Si llegó, investigar por un tiempo
        if (hasReachedDestination)
        {
            currentInvestigationTime += Time.deltaTime;

            if (currentInvestigationTime >= investigationTime)
            {
                // Terminó la investigación
                nextState = new DragonCaminar(npc, agent, anim, player, particleFire);
                stage = EVENT.EXIT;
            }
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("esCaminar");
        agent.isStopped = false;
        base.Exit();
    }
}

///=================================================================
/// <summary>
/// ESTADO DE PERSEGUIR JUGADOR - VERSIÓN CORREGIDA
/// </summary>
public class DragonVolar : DragonState
{
    private bool isAttackingOutOfNavMesh = false; // Flag para controlar el ataque fuera del NavMesh

    public DragonVolar(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, GameObject _particleFire)
        : base(_npc, _agent, _anim, _player, _particleFire)
    {
        name = STATE.VOLAR;
        agent.speed = 20.0f;
        agent.isStopped = false;
        isAttackingOutOfNavMesh = false;
    }

    public override void Enter()
    {
        anim.SetTrigger("esDespegar");
        isAttackingOutOfNavMesh = false;
        // Usar corrutina para secuencia de animaciones
        npc.GetComponent<MonoBehaviour>().StartCoroutine(PlayFlySequence());
        base.Enter();
    }

    private IEnumerator PlayFlySequence()
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetTrigger("esVolar");
        anim.ResetTrigger("esDespegar");
    }

    public override void Update()
    {
        // Verificar si el jugador está en el NavMesh
        if (IsPlayerReachable())
        {
            // Jugador está en NavMesh, ir directamente
            agent.SetDestination(player.position);
            isAttackingOutOfNavMesh = false;

            if (CanAttackPlayer())
            {
                //Si esta lo suficientemente cerca cambia a ataque directo
                nextState = new DragonPegar(npc, agent, anim, player, particleFire);
                stage = EVENT.EXIT;
                return; // Salir inmediatamente
            }
        }
        else
        {
            // Jugador no está sobre NavMesh: encontrar punto más cercano
            NavMeshHit hit;
            // Usar una distancia mayor para encontrar puntos válidos
            if (NavMesh.SamplePosition(player.position, out hit, distOutNavMesh + 30f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);

                if (agent.remainingDistance < 2.0f && !agent.pathPending)
                {
                    isAttackingOutOfNavMesh = true;
                    nextState = new DragonDispararFuego(npc, agent, anim, player, particleFire);
                    stage = EVENT.EXIT;
                    return;
                }
            }
        }

        // Solo volver a caminar si realmente pierde de vista al jugador Y no está en proceso de ataque
        if (!CanSeePlayer() && !isAttackingOutOfNavMesh)
        {
            nextState = new DragonCaminar(npc, agent, anim, player, particleFire);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        anim.SetTrigger("esAterrizar");
        npc.GetComponent<MonoBehaviour>().StartCoroutine(ResetFlyAnimations());
        base.Exit();
    }

    private IEnumerator ResetFlyAnimations()
    {
        yield return new WaitForSeconds(0.5f);
        anim.ResetTrigger("esVolar");
        anim.ResetTrigger("esAterrizar");
    }
}
//====================================================================

/// <summary>
/// ESTADO DE DISPARAR FUEGO AL JUGADOR
/// </summary>
public class DragonDispararFuego : DragonState
{
    private ParticleFireDragonController fuego;
    private float rotationSpeed = 2.0f;
    private float fireTime = 0.0f;
    private float maxFireTime = 5.0f;
    private bool fireStarted = false;

    public DragonDispararFuego(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, GameObject _particleFire)
        : base(_npc, _agent, _anim, _player, _particleFire)
    {
        name = STATE.DISPARAR_FUEGO;
    }

    public override void Enter()
    {
        // Detener movimiento para disparar
        agent.isStopped = true;
        fireTime = 0.0f;

        anim.SetTrigger("esCaminar"); // Usar animación de ataque
        base.Enter();

        fuego = particleFire.GetComponent<ParticleFireDragonController>();

        if (fuego != null)
        {
            fuego.activarParticulasFuego();
            fireStarted = true;
            Debug.Log("Sistema de partículas iniciado");
        }
        
        else
        {
            Debug.LogError("No se encontró sistema de partículas en " + npc.name);
        }     
    }

    public override void Update()
    {
        // Actualizar tiempo de fuego
        fireTime += Time.deltaTime;

        // Rotar hacia el jugador
        Vector3 direction = player.position - npc.transform.position;
        direction.y = 0.0f;

        if (direction != Vector3.zero)
        {
            npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation,
                Quaternion.LookRotation(direction), Time.deltaTime * rotationSpeed);
        }
        
        // Verificar condiciones para salir del estado
        if (fireTime >= maxFireTime)
        {
            // Tiempo de fuego terminado
            nextState = new DragonCaminar(npc, agent, anim, player, particleFire);
            stage = EVENT.EXIT;
        }
        else if (IsPlayerReachable() && CanAttackPlayer())
        {
            // Si el jugador vuelve al NavMesh y está cerca, cambiar a ataque directo
            nextState = new DragonPegar(npc, agent, anim, player, particleFire);
            stage = EVENT.EXIT;
        }
        else if (!CanSeePlayer())
        {
            // Si pierde de vista al jugador
            nextState = new DragonCaminar(npc, agent, anim, player, particleFire);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        // Detener el sistema de partículas
        if (fuego != null && fireStarted)
        {
            fuego.desactivarParticulasFuego();
            Debug.Log("Sistema de partículas detenido");
        }

        anim.ResetTrigger("esCaminar");
        agent.isStopped = false;
        base.Exit();
    }

}

/// <summary>
/// ESTADO DE ATAQUE DIRECTO AL JUGADOR
/// </summary>
public class DragonPegar : DragonState
{
    private float rotationSpeed = 2.0f;
    private float attackCooldown = 0.0f;
    private float attackInterval = 2.0f;

    public DragonPegar(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, GameObject _particleFire)
        : base(_npc, _agent, _anim, _player, _particleFire)
    {
        name = STATE.PEGAR;
    }

    public override void Enter()
    {
        anim.SetTrigger("esPegar");
        agent.isStopped = true;
        attackCooldown = 0.0f;
        base.Enter();
    }

    public override void Update()
    {
        Vector3 direction = player.position - npc.transform.position;
        direction.y = 0.0f;

        //Mira hacia el jugador 
        if (direction != Vector3.zero)
        {
            npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation,
                Quaternion.LookRotation(direction), Time.deltaTime * rotationSpeed);
        }

        // Cooldown para ataques
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0.0f)
        {
            // Realizar ataque
            PerformAttack();
            attackCooldown = attackInterval;
        }

        if (!CanAttackPlayer())
        {
            //Si el jugador se aleja
            if (CanSeePlayer())
            {
                nextState = new DragonVolar(npc, agent, anim, player, particleFire);
            }
            else
            {
                nextState = new DragonCaminar(npc, agent, anim, player, particleFire);
            }
            stage = EVENT.EXIT;
        }
    }

    private void PerformAttack()
    {
        // Lógica del ataque directo
        Debug.Log("Dragon ataca directamente!");
        // Aquí puedes agregar efectos de sonido, daño, etc.
    }

    public override void Exit()
    {
        anim.ResetTrigger("esPegar");
        agent.isStopped = false;
        base.Exit();
    }
}