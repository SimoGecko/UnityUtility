// (c) Simone Guggiari 2024

using System.Collections.Generic;
using System.Linq;

////////// PURPOSE: Various algorithms to work on graphs //////////

// traversal, flood-fill (DFS, BFS)
// SP path finding, shortest path (Dijkstra, A*, Bellman-Ford, Floyd-Warshall, Johnson)
// MST, minimum spanning tree (Kruskal, Prim)
// SCC strongly connected components (Tarjan, Kosaraju)
// TS topological sorting (topologicalsorting, ...)
// Flow (Edmonds-Karp, Dinic)
// Assignment (Hungarian)

namespace sxg
{
    public static class GraphUtility
    {
        // This works on a Graph, not only a tree
        public static void Dfs<Node>(Node startNode, System.Action<Node, Node> nodeProcessor, System.Func<Node, IEnumerable<Node>> neighborsFunc)
        {
            Stack<Node> stack = new();
            HashSet<Node> visited = new();
            Dictionary<Node, Node> parent = new();

            stack.Push(startNode);
            parent[startNode] = default; // TODO: parent should be null
            //visited.Add(startNode);
            while (stack.Count > 0)
            {
                Node node = stack.Pop();
                if (visited.Contains(node))
                    continue;
                visited.Add(node);
                nodeProcessor(node, parent[node]);

                IEnumerable<Node> neighbors = neighborsFunc(node);
                foreach (Node neigh in neighbors)
                {
                    //if (!visited.Contains(neigh)) // needed?
                    stack.Push(neigh);
                    parent[neigh] = node;
                }
            }
        }

        public static void Bfs<Node>(Node startNode, System.Action<Node, Node> nodeProcessor, System.Func<Node, IEnumerable<Node>> neighborsFunc)
        {
            Queue<Node> queue = new();
            HashSet<Node> visited = new();
            Dictionary<Node, Node> parent = new();

            queue.Enqueue(startNode);
            parent[startNode] = default; // TODO: parent should be null
            visited.Add(startNode);
            while (queue.Count > 0)
            {
                Node node = queue.Dequeue();
                nodeProcessor(node, parent[node]); // process vertex
                IEnumerable<Node> neighbors = neighborsFunc(node);
                foreach (Node neigh in neighbors)
                {
                    if (visited.Contains(neigh))
                        continue;
                    queue.Enqueue(neigh);
                    parent[neigh] = node;
                    visited.Add(neigh);
                }
            }
        }

        public static IEnumerable<(Node, Node)> Mst<Node>(IEnumerable<Node> nodes, System.Func<Node, Node, bool> hasEdge, System.Func<Node, Node, float> edgeCost)
        {
            List<(Node, Node)> edges = new();
            int N = nodes.Count();
            Dictionary<Node, int> UF = new(); // TODO: optimze Union-Find DS
            int i = 0;
            foreach (Node node in nodes)
                UF[node] = i++;
            List<(Node, Node, float)> costs = new();
            foreach (Node nodeA in nodes)
            {
                foreach (Node nodeB in nodes)
                {
                    if (hasEdge(nodeA, nodeB))
                    {
                        float cost = edgeCost(nodeA, nodeB);
                        costs.Add((nodeA, nodeB, cost));
                    }
                }
            }

            costs.Sort(cost => cost.Item3); // increasing
            foreach (var cost in costs)
            {
                if (UF[cost.Item1] != UF[cost.Item2]) // find-set
                {
                    edges.Add((cost.Item1, cost.Item2));
                    if (edges.Count == N - 1)
                        break;
                    int from = UF[cost.Item1];
                    int to = UF[cost.Item2];
                    foreach (Node node in nodes)
                    {
                        // union
                        if (UF[node] == from)
                            UF[node] = to;
                    }
                }
            }
            return edges;
        }

    }
}
