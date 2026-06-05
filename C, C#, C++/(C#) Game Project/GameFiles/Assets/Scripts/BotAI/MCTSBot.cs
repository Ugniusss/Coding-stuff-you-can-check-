using System;
using System.Collections.Generic;
using System.Diagnostics;

// UCT Monte Carlo Tree Search for Medium difficulty bot.
// Thread-safe — only operates on cloned GameStateSim (pure C#, no Unity APIs).
public static class MCTSBot
{
    private const double C = 1.41421356; // sqrt(2)

    private class Node
    {
        public GameStateSim   State;
        public BotAction?     Action;   // action that led to this state from parent
        public Node           Parent;
        public List<Node>     Children = new List<Node>();
        public List<BotAction> Untried; // candidate actions not yet expanded
        public int            Visits;
        public double         TotalScore;

        public bool IsFullyExpanded => Untried.Count == 0;
        public bool IsLeaf          => Children.Count == 0;

        public double UCT(int parentVisits)
        {
            if (Visits == 0) return double.MaxValue;
            return TotalScore / Visits + C * Math.Sqrt(Math.Log(parentVisits) / Visits);
        }
    }

    // Returns the best BotAction for the current player within budgetMs milliseconds.
    public static BotAction GetBotAction(GameStateSim state, GameSimulator sim, int budgetMs = 800)
    {
        int botIndex = state.currentPlayerIndex;

        var candidates = sim.GetCandidateActions(state);
        if (candidates.Count == 1) return candidates[0];

        var root = new Node
        {
            State   = state,
            Untried = new List<BotAction>(candidates),
        };

        var sw = Stopwatch.StartNew();

        while (sw.ElapsedMilliseconds < budgetMs)
        {
            Node node = Select(root);
            node = Expand(node, sim);
            double score = Rollout(node, sim, botIndex);
            Backprop(node, score);
        }

        // Return action of the most-visited child
        Node best = null;
        int  bestV = -1;
        foreach (var child in root.Children)
            if (child.Visits > bestV) { bestV = child.Visits; best = child; }

        return best?.Action ?? candidates[0];
    }

    // ── MCTS phases ───────────────────────────────────────────────────────────

    static Node Select(Node node)
    {
        while (node.IsFullyExpanded && !node.IsLeaf)
        {
            Node bestChild  = null;
            double bestScore = double.MinValue;
            foreach (var child in node.Children)
            {
                double s = child.UCT(node.Visits);
                if (s > bestScore) { bestScore = s; bestChild = child; }
            }
            if (bestChild == null) break;
            node = bestChild;
        }
        return node;
    }

    static Node Expand(Node node, GameSimulator sim)
    {
        if (node.Untried.Count == 0) return node;

        // Pop one untried action
        int      idx    = node.Untried.Count - 1;
        BotAction action = node.Untried[idx];
        node.Untried.RemoveAt(idx);

        // Apply bot's action
        GameStateSim child = node.State.Clone();
        sim.ApplyAction(child, action);

        // Simulate opponent's random turn so child is always at "bot's turn"
        if (child.roundsLeft > 0)
        {
            var (m, d) = sim.GetRandomMoves(child);
            sim.ApplyAction(child, new BotAction { ability = null, moves = m, doActions = d });
        }

        var childNode = new Node
        {
            State   = child,
            Action  = action,
            Parent  = node,
            Untried = sim.GetCandidateActions(child),
        };
        node.Children.Add(childNode);
        return childNode;
    }

    static double Rollout(Node node, GameSimulator sim, int botIndex)
    {
        int winner = sim.Rollout(node.State, botIndex);
        if (winner == botIndex) return 1.0;
        if (winner == -1)       return 0.5; // tie
        return 0.0;
    }

    static void Backprop(Node node, double score)
    {
        for (Node n = node; n != null; n = n.Parent)
        {
            n.Visits++;
            n.TotalScore += score;
        }
    }
}
