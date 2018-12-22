using MapDriver;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSGame.Screens.MapScreenControls;

namespace TBSGame.AI
{
    public delegate void EndTurnEnventHandler(object sender);

    public class MapAI
    {
        public event EndTurnEnventHandler OnEndTurn;
        private void EndTurn()
        {
            index = -1;
            sighted_enemies = new HashSet<Unit>();
            OnEndTurn?.Invoke(this);
        }

        public int Player { get; set; }

        private MoveControl move = new MoveControl();
        private Dictionary<System.Drawing.Point, Unit> units;
        private Dictionary<Point, Unit> enemies;
        private HashSet<Unit> sighted_enemies = new HashSet<Unit>();
        private List<System.Drawing.Point> los;
        private Point target;
        private Map map;
        private Engine engine;
        private TimeSpan last_time = TimeSpan.Zero;
        private int index = -1, selected = -1, phase = 0;

        public MapAI(Map map, int player)
        {
            Player = player;
            this.map = map;

            move.OnTargetInSight += new TargetInSightEventHandle((sender, source, unit) => phase = 0);
            move.OnMovingDone += new MovingDoneEventHandle((sender, source) => phase = 0);
        }
        
        public void Turn(Map map)
        {
            index = 0;
        }

        public void Update(Map map, Engine engine, List<AreaControl> areas, GameTime time)
        {
            this.map = map;
            this.engine = engine;

            units = map.Units.Where(kvp => kvp.Value.Player == Player).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            los = engine.GetLineOfSight(this.Player);
            sighted_enemies.UnionWith(get_enemies());

            if (index != -1)
            {
                this.enemies = map.Units.Where(kvp => sighted_enemies.Contains(kvp.Value)).ToDictionary(kvp => new Point(kvp.Key.X, kvp.Key.Y), kvp => kvp.Value);

                if (index >= units.Count)
                    EndTurn();
                else
                {
                    var p = units.Keys.ToArray()[index];
                    if (selected == -1)
                        selected = MoveControl.GetIndex(map, p.X, p.Y);

                    //analýza dalšího kroku
                    if (phase == 0)
                    {
                        analyze(units[p], new Point(p.X, p.Y), areas);
                    }

                    //pohyb jednotky
                    if (phase == 1)
                    {
                        move.Update(time, map, engine, areas, ref selected, Player);
                    }

                    //útok na jednotku
                    if (phase == 2)
                    {
                        if (last_time == TimeSpan.Zero)
                            last_time = time.TotalGameTime - TimeSpan.FromMilliseconds(600);

                        if (time.TotalGameTime - last_time >= TimeSpan.FromMilliseconds(600))
                        {
                            UnitControl enemy = areas[MoveControl.GetIndex(map, target.X, target.Y)].UnitControl;
                            if (enemy == null) //jednotk umřela
                                phase = 0;
                            else if (areas[selected].UnitControl.Attack(enemy) == -1) //došla stamina
                                phase = 3;

                            last_time = time.TotalGameTime;
                        }
                    }

                    //jednotka už nebude nic dělat
                    if (phase == 3)
                    {
                        phase = 0;
                        selected = -1;
                        index++;
                    }
                }
            }
        }

        private void analyze(Unit unit, Point point, List<AreaControl> areas)
        {
            switch (unit.AIStrategy)
            {
                case AIStrategy.Attack: //heuristické hodnocení pozice, dobré pro útok
                    rush(unit, point, areas);
                    break;
                case AIStrategy.Defend: //heuristické hodnocení pozice, dobré pro obranu
                    break;
                case AIStrategy.Rush: //útok
                    break;
                case AIStrategy.Static: //jednotka se nehýbe
                    break;
                case AIStrategy.Scout: //hledání nepřítele
                    break;
            }
        }

        private void rush(Unit unit, Point point, List<AreaControl> areas)
        {
            Dictionary<System.Drawing.Point, double> real_mob = engine.GetMobility(point.X, point.Y, unit); //reálná pohbylivost

            List<Point> targets = new List<Point>();

            //pozice, na kterých dostřelí na jednotku
            HashSet<System.Drawing.Point> in_range = new HashSet<System.Drawing.Point>();
            foreach (KeyValuePair<Point, Unit> kvp in enemies)
            {
                foreach (System.Drawing.Point p in engine.GetAttackRange(point.X, point.Y, unit, unit.Range))
                {
                    in_range.Add(p);
                    if (p.X == kvp.Key.X && p.Y == kvp.Key.Y)
                    {
                        targets.Add(kvp.Key);
                        phase = 2;
                    }
                }
            }

            if (phase == 0)
            {
                phase = 1;
                Point next = next_hop(real_mob, in_range, point, unit);
                if (next == new Point(-1, -1) || next == point)
                    phase = 3;
                else
                {
                    engine.Mobility = real_mob;
                    move.Move(engine, real_mob, areas[selected].UnitControl, next);
                }
            }
            else
            {
                //TODO: vyhledání nejvhodnějšího cíle
                target = targets[0];
            }
        }

        private Point next_hop(
            Dictionary<System.Drawing.Point, double> real_mob, 
            HashSet<System.Drawing.Point> in_range, 
            Point point, 
            Unit unit
            )
        {
            Dictionary<System.Drawing.Point, double> mob = engine.GetMobility(point.X, point.Y, unit, -1); //1000x větší pohyblivost

            //zbylá stamina po přesunutí na pozice, ze které muže jednotka zaútočit
            List<KeyValuePair<System.Drawing.Point, double>> _mob_to_enemy = mob.Where(kvp => in_range.Contains(kvp.Key)).ToList();

            //setřízení pozic, podle vzdálenosti
            _mob_to_enemy.Sort((kvp1, kvp2) => kvp1.Value.CompareTo(kvp2.Value) * -1); 
            Dictionary<System.Drawing.Point, double> mob_to_enemy = _mob_to_enemy.ToDictionary(kvp => kvp.Key, kvp => kvp.Value); 

            //nelze se dostat na pozici, ze které můžu zaútočit na jednotku
            if (mob_to_enemy.Count > 0)
            {
                System.Drawing.Point key = mob_to_enemy.Keys.ToArray()[0];
                List<System.Drawing.Point> points = engine.GetMove(mob, point.X, point.Y, key.X, key.Y); //trasa pohybu

                //zjištění nebližší možné pozice přesunu
                for (int i = points.Count - 1; i >= 0; i--)
                {
                    if (real_mob.ContainsKey(points[i]))
                    {
                        return new Point(points[i].X, points[i].Y); //nebližší možná pozice přesunu
                    }
                }
            }
            return new Point(-1, -1);
        }
   
        private HashSet<Unit> get_enemies()
        {
            HashSet<Unit> enemies = new HashSet<Unit>();

            for (int i = 0; i < los.Count; i++)
            {
                int x = los[i].X;
                int y = los[i].Y;

                Unit u = map.GetUnit(x, y);
                if (u != null && u.Player == 1)
                    enemies.Add(u);
            }

            return enemies;
        }
    }
}
