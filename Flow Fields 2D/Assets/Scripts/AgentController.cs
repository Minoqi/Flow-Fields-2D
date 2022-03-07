using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    // Variables
    public GridController gridController;
    public GameObject agentPrefab;
    public int numAgentsSpawn;
    public float moveSpeed;

    [SerializeField] private List<GameObject> agentsInGame;

    // Start is called before the first frame update
    void Awake()
    {
        agentsInGame = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnUnits();
        }
    }

    private void FixedUpdate()
    {
        if (gridController.currentFlowField == null)
        {
            return;
        }
        else
        {
            foreach(GameObject agent in agentsInGame)
            {
                GridCell currentCell = gridController.currentFlowField.GetCellFromWorldPosition(agent.transform.position);
                Vector3 moveDirection = new Vector3(currentCell.bestDirection.x, currentCell.bestDirection.y, 0);
                Rigidbody2D agentRB = agent.GetComponent<Rigidbody2D>();
                agentRB.velocity = moveDirection * moveSpeed;
            }
        }
    }

    public void SpawnUnits()
    {
        Vector2Int gridSize = gridController.gridSize;
        float nodeRadius = gridController.cellRadius;
        Vector2 maxSpawnPos = new Vector2(gridSize.x * nodeRadius * 2 + nodeRadius, gridSize.y * nodeRadius * 2 + nodeRadius);
        int colMask = LayerMask.GetMask("Impassible", "Agents");
        Vector3 newPosition;

        for (int i = 0; i < numAgentsSpawn; i++)
        {
            GameObject newUnit = Instantiate(agentPrefab);
            newUnit.transform.parent = transform;
            agentsInGame.Add(newUnit);

            do
            {
                newPosition = new Vector3(Random.Range(0, maxSpawnPos.x), 0, Random.Range(0, maxSpawnPos.y));
                newUnit.transform.position = newPosition;
            } while (Physics.OverlapSphere(newPosition, 1f, colMask).Length > 0);
        }
    }
}
