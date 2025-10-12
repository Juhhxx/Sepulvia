using System.Runtime.CompilerServices;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int speed;
    [SerializeField] private LayerMask grassLayer;
    [SerializeField] private int minStepsToEncounter;
    [SerializeField] private int maxStepsToEncounter;
    [ReadOnly]
    public int stepsInGrass;
    private PlayerControls playerControls;
    private Rigidbody rb;
    private Vector3 movement;
    private bool movinInGrass;
    private float steptimer;
    private int stepsToEncounter;
    private PartyManager partyManager;

    private const float TIME_PER_STEP = 0.5f;
    private const string BATTLE_SCENE = "BattleTestScene"; //Switch to whatever we consider the scene for battle
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerControls = new PlayerControls();
        CalculateStepsToNextEncounter();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        partyManager = GameObject.FindAnyObjectByType<PartyManager>();
        //if we have a position saved then we are going to move the player there
        if(partyManager.GetPosition() != Vector3.zero)
        {
            transform.position = partyManager.GetPosition();
        }
    }
    // Update is called once per frame
    void Update()
    {
        float x = playerControls.Player.Move.ReadValue<Vector2>().x;
        float z = playerControls.Player.Move.ReadValue<Vector2>().y;

        movement = new Vector3(x, 0, z).normalized;
        //Debug.Log(x + "," + z);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + movement * speed * Time.fixedDeltaTime);

        Collider[] colliders = Physics.OverlapSphere(transform.position, 2, grassLayer);
        movinInGrass = colliders.Length != 0 && movement != Vector3.zero;

        if (movinInGrass == true)
        {
            steptimer += Time.fixedDeltaTime;
            if (steptimer > TIME_PER_STEP)
            {
                stepsInGrass++;
                steptimer = 0;

                if (stepsInGrass >= stepsToEncounter) //Check to see if we "reached an encounter"
                {
                    partyManager.SetPosition(transform.position); //save our overworld position before we chaneg into the battleScene
                    Debug.Log("Change Scene Now");
                    SceneManager.LoadScene(BATTLE_SCENE);
                }
                
            }
        }
    }

    private void CalculateStepsToNextEncounter()
    {
        stepsToEncounter = Random.Range(minStepsToEncounter, maxStepsToEncounter);
    }
}
