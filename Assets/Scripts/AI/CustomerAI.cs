using UnityEngine;
using UnityEngine.AI;

public class CustomerAI : MonoBehaviour
{
    public float roamRadius = 3f;
    public float waitTime = 3f;
    public float viewDistance = 2f;
    public LayerMask itemLayer;

    private NavMeshAgent agent;
    private float timer;
    private bool isWaiting;
    private PlayerWallet playerWallet;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerWallet = FindObjectOfType<PlayerWallet>(); // sahnedeki ilk PlayerWallet bileþeni
        GoToRandomPoint();
    }

    private void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.2f && !isWaiting)
        {
            isWaiting = true;
            timer = 0f;

            // Item taramasý burada yapýlýr
            TryBuyNearbyItem();
        }

        if (isWaiting)
        {
            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                GoToRandomPoint();
                isWaiting = false;
            }
        }
    }

    private void GoToRandomPoint()
    {
        Vector3 randomDir = Random.insideUnitSphere * roamRadius;
        randomDir += transform.position;
        randomDir.y = transform.position.y;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomDir, out navHit, roamRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(navHit.position);
        }
    }

    private void TryBuyNearbyItem()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, viewDistance, itemLayer);

        foreach (var hit in hits)
        {
            SellableItem item = hit.GetComponent<SellableItem>();
            if (item != null)
            {
                item.Sell(playerWallet);
                Debug.Log("Müþteri item satýn aldý: " + item.itemData.itemName);
                break; // sadece 1 item alsýn
            }
        }
    }
}
