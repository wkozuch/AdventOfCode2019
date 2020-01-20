using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Graphs
{
    public class Vertex<T>
    {

        List<Vertex<T>> neighbors;
        private Dictionary<Vertex<T>, int> neighborsWeights;
        T value;
        private readonly int _weight;
        bool isVisited;

        public List<Vertex<T>> Neighbors { get { return neighbors; } set { neighbors = value; } }
        public Dictionary<Vertex<T>, int> NeighborsWeigths { get { return neighborsWeights; } set { neighborsWeights = value; } }
        public T Value { get { return value; } set { this.value = value; } }
        public bool IsVisited { get { return isVisited; } set { isVisited = value; } }
        public int NeighborsCount { get { return neighbors.Count; } }

        public Vertex(T value)
        {
            this.value = value;
            isVisited = false;
            neighbors = new List<Vertex<T>>();
            neighborsWeights = new Dictionary<Vertex<T>, int>();
        }

        public Vertex(T value, List<Vertex<T>> neighbors)
        {
            this.value = value;
            isVisited = false;
            this.neighbors = neighbors;
        }

        public void Visit()
        {
            isVisited = true;
        }

        public virtual void AddEdge(Vertex<T> vertex)
        {
            if (neighbors.Contains(vertex)) return;
            neighbors.Add(vertex);
        }

        public void AddEdge(Vertex<T> vertex, int weight)
        {
            AddEdge(vertex);
            neighborsWeights.Add(vertex, weight);
        }

        public void AddEdges(List<Vertex<T>> newNeighbors) 
        {
            neighbors.AddRange(newNeighbors);
        }

        public void RemoveEdge(Vertex<T> vertex)
        {
            neighbors.Remove(vertex);
        }
        public override string ToString()
        {

            StringBuilder allNeighbors = new StringBuilder("");
            allNeighbors.Append(value + ": ");

            foreach (Vertex<T> neighbor in neighbors)
            {
                allNeighbors.Append(neighbor.value + "  ");
            }

            return allNeighbors.ToString();

        }

    }
   
    public class Vertex
    {
        private readonly string _name;

        List<Vertex> neighbors;
        private Dictionary<Vertex, int> neighborsWeights;
        private readonly int _weight;
        bool isVisited;

        public List<Vertex> Neighbors { get { return neighbors; } set { neighbors = value; } }
        public Dictionary<Vertex, int> NeighborsWeigths { get { return neighborsWeights; } set { neighborsWeights = value; } }
        public bool IsVisited { get { return isVisited; } set { isVisited = value; } }
        public int NeighborsCount { get { return neighbors.Count; } }
        public string Value => _name;
        public Vertex(string name)
        {
            _name = name;
            isVisited = false;
            neighbors = new List<Vertex>();
            neighborsWeights = new Dictionary<Vertex, int>();
        }

        public Vertex(string name, List<Vertex> neighbors)
        {
            _name = name;
            isVisited = false;
            this.neighbors = neighbors;
        }

        public void Visit()
        {
            isVisited = true;
        }

        public virtual void AddEdge(Vertex vertex)
        {
            neighbors.Add(vertex);
        }

        public void AddEdge(Vertex vertex, int weight)
        {
            AddEdge(vertex);
            neighborsWeights.Add(vertex, weight);
        }

        public void AddEdges(List<Vertex> newNeighbors)
        {
            neighbors.AddRange(newNeighbors);
        }

        public void RemoveEdge(Vertex vertex)
        {
            neighbors.Remove(vertex);
        }
        public override string ToString()
        {

            StringBuilder allNeighbors = new StringBuilder("");
            allNeighbors.Append(Value + ": ");

            foreach (Vertex neighbor in neighbors)
            {
                allNeighbors.Append(neighbor.Value + "  ");
            }

            return allNeighbors.ToString();

        }

    }
}