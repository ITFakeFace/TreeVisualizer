using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TreeVisualizer.Utils.Coordinator
{
    public class CoordinateCalculator
    {
        // Required Properties
        private Coordinate MapSize { get; set; }
        public List<List<Coordinate>> GridCoordinateMap { get; set; }
        private Coordinate GridSize { get; set; }

        // Generated Properties
        private int Row { get; set; }
        private int Column { get; set; }
        private double PaddingX { get; set; }
        private double PaddingY { get; set; }

        public CoordinateCalculator(Coordinate mapSize)
        {
            this.MapSize = mapSize;
            GridSize = new Coordinate
            {
                X = (double)Application.Current.TryFindResource("NodeSize"),
                Y = (double)Application.Current.TryFindResource("NodeSize"),
            };
            InitializeProperties();
        }
        public CoordinateCalculator(Coordinate mapSize, Coordinate gridSize)
        {
            this.MapSize = mapSize;
            this.GridSize = gridSize;
            InitializeProperties();
        }

        public CoordinateCalculator(Coordinate mapSize, int gridSizeX, int gridSizeY)
        {
            this.MapSize = mapSize;
            GridSize = new Coordinate(gridSizeX, gridSizeY);
            InitializeProperties();
        }

        public CoordinateCalculator(Coordinate mapSize, int gridSize)
        {
            this.MapSize = mapSize;
            this.GridSize = new Coordinate(gridSize, gridSize);
            PaddingX = CalculatePaddingX();
            PaddingY = CalculatePaddingY();

        }

        public void InitializeProperties()
        {
            PaddingX = CalculatePaddingX();
            PaddingY = CalculatePaddingY();
            GridCoordinateMap = GenerateCoordinateMap();
        }

        public Coordinate GetNodeCoordinate(int X, int Y)
        {
            //Console.WriteLine($"Padding: X:{PaddingX}, Y:{PaddingY}");
            //Console.WriteLine($"GridSize: X:{GridSize.X}, Y:{GridSize.Y}");
            //return new Coordinate(PaddingX + X * GridSize.X, PaddingY + Y * GridSize.Y);
            return new Coordinate(X * GridSize.X, Y * GridSize.Y);
        }

        public Coordinate GetNodeCoordinate(GridCoordinate node)
        {
            //return new Coordinate(PaddingX + node.X * GridSize.X, PaddingY + node.Y * GridSize.Y);
            return new Coordinate(node.X * GridSize.X, node.Y * GridSize.Y);
        }

        public double CalculatePaddingX()
        {
            //Console.WriteLine($"Padding Cal: MapSize:{MapSize}, GridSize:{GridSize}");
            return (MapSize.X != double.NaN ? MapSize.X : 0) % GridSize.X / 2;
        }

        public double CalculatePaddingY()
        {
            return (MapSize.Y != double.NaN ? MapSize.Y : 0) % GridSize.Y / 2;
        }

        public List<List<Coordinate>> GenerateCoordinateMap()
        {
            List<List<Coordinate>> map = new List<List<Coordinate>>();
            Column = (int)((MapSize.X - MapSize.X % GridSize.X) / GridSize.X);
            Row = (int)((MapSize.Y - MapSize.Y % GridSize.Y) / GridSize.Y);
            for (int i = 0; i < Row; i++)
            {
                map.Add(new List<Coordinate>());
                for (int j = 0; j < Column; j++)
                {
                    map[i].Add(GetNodeCoordinate(j, i));
                }
            }
            Console.WriteLine("Map Created");
            this.GridCoordinateMap = map;
            return map;
        }
    }
}
