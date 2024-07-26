using System.Drawing;
using System.Numerics;
using System.Xml.Linq;
using static Pathfinder.Program;

namespace Pathfinder
{
    public class Node
    {
        public Point Location { get; private set; }
        public bool IsWalkable { get; set; }
        public float G { get; set; }
        public float H { get; set; }
        public float F { get { return this.G + this.H; } }
        public NodeState State { get; set; }
        public Node ParentNode { get; set; }

        public Node(Point pos, bool walkable)
        {
            ParentNode = null;
            Location = pos;
            IsWalkable = walkable;
        }
    }


    public enum NodeState { Untested, Open, Closed }

    public class Program
    {

        static public List<List<Node>> Grid = new List<List<Node>>();

        static int GridRows
        {
            get
            {
                return Grid[0].Count;
            }
        }
        static int GridCols
        {
            get
            {
                return Grid.Count;
            }
        }


        public static Stack<Node> FindPath(List<List<Node>> grid, Point Start, Point End)
        {
            Grid = grid;
            Node start = new Node(Start, true);
            Node end = new Node(End, true);

            Stack<Node> Path = new Stack<Node>();
            PriorityQueue<Node, float> OpenList = new PriorityQueue<Node, float>();
            List<Node> ClosedList = new List<Node>();
            List<Node> adjacencies;
            Node current = start;

            OpenList.Enqueue(start, start.F);

            while (OpenList.Count != 0 && !ClosedList.Exists(x => x.Location == end.Location))
            {
                current = OpenList.Dequeue();
                ClosedList.Add(current);
                adjacencies = GetAdjacentNodes(current);

                foreach (Node n in adjacencies)
                {
                    if (!ClosedList.Contains(n) && n.IsWalkable)
                    {
                        bool isFound = false;
                        foreach (var oLNode in OpenList.UnorderedItems)
                        {
                            if (oLNode.Element == n)
                            {
                                isFound = true;
                            }
                        }
                        if (!isFound)
                        {
                            n.ParentNode = current;
                            n.H = Math.Abs(n.Location.X - end.Location.X) + Math.Abs(n.Location.Y - end.Location.Y);
                            n.G = n.ParentNode.G + 1;
                            OpenList.Enqueue(n, n.F);
                        }
                    }
                }

            }
            if (!ClosedList.Exists(x => x.Location == end.Location))
            {
                return null;
            }

            // if all good, return path
            Node temp = ClosedList[ClosedList.IndexOf(current)];
            if (temp == null) return null;
            do
            {
                Path.Push(temp);
                temp = temp.ParentNode;
            } while (temp != start && temp != null);
            
            return Path;
        }

        static private List<Node> GetAdjacentNodes(Node n)
        {
            List<Node> temp = new List<Node>();

            int row = (int)n.Location.Y;
            int col = (int)n.Location.X;

            if (row + 1 < GridRows)
            {
                temp.Add(Grid[row + 1][col]);
            }
            if (row - 1 >= 0)
            {
                temp.Add(Grid[row - 1][col]);
            }
            if (col - 1 >= 0)
            {
                temp.Add(Grid[row][col - 1]);
            }
            if (col + 1 < GridCols)
            {
                temp.Add(Grid[row][col + 1]);
            }

            return temp;
        }

        static void Main(string[] args)
        {
            List<List<Node>> Grid = new List<List<Node>>();
            Console.WriteLine("Input number of columns");
            int colNum = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Input number of rows");
            int rowNum = Convert.ToInt32(Console.ReadLine());

            for (int i = 0; i < rowNum; i++)
            {
                List<Node> colNode = new List<Node>();
                for (int j = 0; j < colNum; j++)
                {
                    Console.WriteLine("Input value for co-ord {0},{1} - 0 for passable or 1 for obstacle", j.ToString(), i.ToString());
                    int passableInt = Convert.ToInt32(Console.ReadLine());
                    if (passableInt == 0)
                    {
                        colNode.Add(new Node(new Point(j, i), true));
                    }
                    else
                    {
                        colNode.Add(new Node(new Point(j, i), false));
                    }
                }
                Grid.Add(colNode);
            }

            string colLine = "";
            int colNumber = 0;

            foreach (var rowNode in Grid)
            {
                foreach (var colNode in rowNode)
                {
                    if (colNode.IsWalkable)
                    {
                        colLine += ",";
                    }
                    else
                    { 
                        colLine += "!";
                    }
                }
                Console.WriteLine(colLine);
                colLine = "";
            }


            int xStart = 0;
            int yStart = 0;
            int xEnd = 0;
            int yEnd = 0;

            bool invalidStart = true;
            bool invalidEnd = true;

            while (invalidStart)
            {

                Console.WriteLine("Enter x co-ord for start");
                xStart = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Enter y co-ord for start");
                yStart = Convert.ToInt32(Console.ReadLine());

                if (xStart < 0 || yStart < 0 || xStart >= colNum || yStart >= rowNum)
                {
                    Console.WriteLine("That's out of bounds, enter new co-ords");
                }
                else if (Grid[yStart][xStart].IsWalkable == false)
                {
                    Console.WriteLine("Tile is unwalkable, enter new co-ords")
                }
                else
                {
                    invalidStart = false;
                }
            }

            while (invalidEnd)
            {
                Console.WriteLine("Enter x co-ord for end");
                xEnd = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Enter y co-ord for end");
                yEnd = Convert.ToInt32(Console.ReadLine());

                if (xEnd < 0 || yEnd < 0 || xEnd >= colNum || yEnd >= rowNum)
                {
                    Console.WriteLine("That's out of bounds, enter new co-ords");
                }
                else if (Grid[yEnd][xEnd].IsWalkable == false)
                {
                    Console.WriteLine("Tile is unwalkable, enter new co-ords")
                }
            }

            var Path = FindPath(Grid, new Point(xStart, yStart), new Point(xEnd, yEnd));

            if (Path == null)
            {
                Console.WriteLine("No path found");

            }
            foreach (var pathNode in Path)
            {
                Console.Write(pathNode.Location.ToString());
            }

        }
    }

}
