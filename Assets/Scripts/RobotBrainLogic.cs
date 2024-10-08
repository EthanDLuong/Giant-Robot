using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RobotBrainLogic : MonoBehaviour
{
    // Define robot parts and resources
    public enum RobotPart { Head, Arm, Leg, Core }
    public enum Resource { Metal, Circuit, Battery, Wire }

    // Number of rounds before the repair deadline
    public int roundsBeforeDeadline = 5;
    public int difficultyLevel = 1; // Initial difficulty level

    // Store the robot parts and their repair status
    private Dictionary<RobotPart, List<Resource>> repairsNeeded = new Dictionary<RobotPart, List<Resource>>();
    private Dictionary<RobotPart, bool> repairStatus = new Dictionary<RobotPart, bool>();

    private RobotBrainVisuals visuals;

    void Start()
    {
        visuals = GetComponent<RobotBrainVisuals>();
        GenerateRepairs();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndRound();
        }
    }

    // Generates the repair requirements based on the difficulty level
    void GenerateRepairs()
    {
        repairsNeeded.Clear();
        repairStatus.Clear();

        // Calculate how many parts need repairs based on difficulty level
        int partsToRepair = Mathf.Min(difficultyLevel, System.Enum.GetValues(typeof(RobotPart)).Length);

        // Randomly determine which parts need repair and with what resources
        List<RobotPart> availableParts = new List<RobotPart>((RobotPart[])System.Enum.GetValues(typeof(RobotPart)));
        for (int i = 0; i < partsToRepair; i++)
        {
            RobotPart selectedPart = availableParts[Random.Range(0, availableParts.Count)];
            availableParts.Remove(selectedPart);

            // Generate a random resource for the repair
            Resource requiredResource = (Resource)Random.Range(0, System.Enum.GetValues(typeof(Resource)).Length);
            repairsNeeded.Add(selectedPart, new List<Resource> { requiredResource });
            repairStatus.Add(selectedPart, false); // Set the initial repair status to false

            // Display the repair message
            visuals.DisplayMessage($"{selectedPart} needs repairing, bring {requiredResource}.");
        }
                    // After the repair message, display the rounds remaining
            StartCoroutine(DisplayMessageWithDelay($"{roundsBeforeDeadline} rounds left before DEADLINE.", 3f));
    }

    // Coroutine to handle displaying a message with a delay
    private IEnumerator DisplayMessageWithDelay(string message, float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        visuals.DisplayMessage(message);
    }

    // Call this to simulate the end of a round and update the round count only when space bar is pressed
    public void EndRound()
    {
        if (roundsBeforeDeadline > 0)
        {
            roundsBeforeDeadline--;

            if (roundsBeforeDeadline > 0)
            {
                visuals.DisplayMessage($"{roundsBeforeDeadline} rounds left before DEADLINE.");
            }
            else
            {
                CheckRepairStatus(); // Check if repairs are completed when rounds reach 0
            }
        }
    }

    // Check if the repairs are successful or if the game is over
    private void CheckRepairStatus()
    {
        bool allRepaired = true;

        foreach (var part in repairsNeeded.Keys)
        {
            if (!repairStatus[part])
            {
                allRepaired = false;
                break;
            }
        }

        if (allRepaired)
        {
            visuals.DisplayMessage("Good Job! Repairs successful.");
            difficultyLevel++; // Increase difficulty for next round
            roundsBeforeDeadline = 5; // Reset rounds for the new deadline
            GenerateRepairs(); // Generate new repair requirements
        }
        else
        {
            visuals.DisplayMessage("Game OVER! Repairs failed.");
        }
    }
}