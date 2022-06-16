using MapDriver;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TBSGame.Saver;

namespace TBSGame.AI
{
	public delegate void AttackEventHandler(object sender);

    public class GameAI
    {
        public event AttackEventHandler OnAttack;
        public int LastBattle { get; set; } = 0;

        private readonly GameSave game;
        private readonly Random random;
        private string target;

        public GameAI(GameSave game)
        {
            this.game = game;
            random = new Random(DateTime.Now.Millisecond);
        }

        public void Update()
        {
            int thresh = 1 + LastBattle * 5;
            int n = random.Next(100);

            if (n <= thresh)
                OnAttack?.Invoke(this);

            LastBattle++;
        }

        public List<Unit> GenUnits()
        {
            List<Unit> units = new List<Unit>();
            units.Add(new Scout(2));

            int max = game.Units.Count + 3;
            for (int i = 0; i < max; i++)
            {
                Unit unit = new Archer(2);
                units.Add(unit);
            }

            return units;
        }

        public LevelMap GetTarget(List<LevelMap> territories)
        {
            int index = random.Next(territories.Count);
            target = territories[index].Name;
            return territories[index];
        }

        public void SetUnits(Map target, List<Unit> units)
        {
            Dictionary<Point, Unit> list = new Dictionary<Point, Unit>();

            void swap(ref Point p)
            {
                if (list.ContainsKey(p))
                {
                    Queue<Point> points = new Queue<Point>();
                    points.Enqueue(new Point(p.X, p.Y));
                    while (true)
                    {
                        Point pp = points.Dequeue();
                        for (int i = 0; i < 4; i++)
                        {
                            Point np = new Point(pp.X, pp.Y);
                            if (i == 0)
                                np.X++;
                            else if (i == 1)
                                np.Y++;
                            else if (i == 2)
                                np.X--;
                            else
                                np.Y--;

                            if (target.TryCoordinates(np.X, np.Y) && target.GetMapObject(np.X, np.Y) == null && !list.ContainsKey(np))
                            {
                                p = np;
                                return;
                            }

                            points.Enqueue(np);
                        }
                    }
                }
            }

            int move = 0;
            Point pos = get_pos(target);
            for (int i = 0; i < units.Count; i++)
            {
                if (move < 3)
                {
                    move = 0;
                    pos = get_pos(target);
                }

                swap(ref pos);
                list.Add(pos, units[i]);

                move++;
            }

            game.Info[this.target].Units = list;
        }

        private Point get_pos(Map map)
        {
            Dictionary<Point, double> elevs = new Dictionary<Point, double>();
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    MapObject obj = map.GetMapObject(x, y);
                    if (obj == null)
                        elevs.Add(new Point(x, y), map.GetTerrain(x, y).Elevation);
                }
            }

            int index = random.Next(elevs.Count);
            return elevs.OrderBy(kvp => kvp.Value).ToArray()[index].Key;
        }
    }
}
