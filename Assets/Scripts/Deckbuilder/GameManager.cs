using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action<int> OnRoundStarted;
    public event Action<Player> OnTurnStarted;
    public event Action OnStateChanged;
    public event Action<Team> OnMatchEnded;

    public IReadOnlyList<Team> Teams => teams;
    public Player CurrentPlayer { get; private set; }

    private List<Team> teams = new List<Team>();
    private bool waitingForTurnEnd;

    public void StartMatch(List<Team> matchTeams)
    {
        teams = matchTeams;
        StartCoroutine(RunMatch());
    }

    private IEnumerator RunMatch()
    {
        int roundNumber = 0;

        while (!IsMatchOver())
        {
            roundNumber++;
            OnRoundStarted?.Invoke(roundNumber);

            foreach (Player player in DetermineTurnOrder())
            {
                if (!player.IsAlive || IsMatchOver())
                {
                    continue;
                }

                CurrentPlayer = player;
                player.StartTurn();
                waitingForTurnEnd = true;
                OnTurnStarted?.Invoke(player);

                yield return new WaitUntil(() => !waitingForTurnEnd || IsMatchOver());

                player.EndTurn();
                CurrentPlayer = null;
                OnStateChanged?.Invoke();
            }
        }

        OnMatchEnded?.Invoke(GetWinner());
    }

    private List<Player> DetermineTurnOrder()
    {
        List<Player> allAlive = teams.SelectMany(t => t.GetAlivePlayers()).ToList();

        for (int i = allAlive.Count - 1; i > 0; i--)
        {
            int swapIndex = UnityEngine.Random.Range(0, i + 1);
            (allAlive[i], allAlive[swapIndex]) = (allAlive[swapIndex], allAlive[i]);
        }

        return allAlive;
    }

    public List<Player> GetCandidateTargets(CardData card, Player actingPlayer)
    {
        Team ownTeam = teams.First(t => t.Players.Contains(actingPlayer));
        List<Player> allies = ownTeam.GetAlivePlayers();
        List<Player> enemies = teams.Where(t => t != ownTeam).SelectMany(t => t.GetAlivePlayers()).ToList();

        switch (card.Target)
        {
            case TargetType.Self:
                return new List<Player> { actingPlayer };
            case TargetType.SingleAlly:
            case TargetType.AllAllies:
                return allies;
            case TargetType.SingleEnemy:
            case TargetType.AllEnemies:
            default:
                return enemies;
        }
    }

    public bool RequiresManualTarget(CardData card)
    {
        return card.Target == TargetType.SingleEnemy || card.Target == TargetType.SingleAlly;
    }

    public bool SubmitCardPlay(CardData card, Player manualTarget)
    {
        if (CurrentPlayer == null || !CurrentPlayer.CanPlay(card))
        {
            return false;
        }

        List<Player> targets = RequiresManualTarget(card)
            ? new List<Player> { manualTarget }
            : GetCandidateTargets(card, CurrentPlayer);

        if (targets.Count == 0 || targets.Contains(null))
        {
            return false;
        }

        CurrentPlayer.PlayCard(card, targets);
        OnStateChanged?.Invoke();
        return true;
    }

    public void EndCurrentTurn()
    {
        waitingForTurnEnd = false;
    }

    private bool IsMatchOver()
    {
        return teams.Count(t => !t.IsDefeated) <= 1;
    }

    private Team GetWinner()
    {
        return teams.FirstOrDefault(t => !t.IsDefeated);
    }
}
