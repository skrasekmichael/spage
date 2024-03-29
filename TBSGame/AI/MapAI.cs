﻿using MapDriver;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TBSGame.Screens.MapScreenControls;
using Pos = System.Drawing.Point;

namespace TBSGame.AI
{
	public delegate void EndTurnEnventHandler(object sender);

    public class MapAI
    {
        public event EndTurnEnventHandler OnEndTurn;
        private void EndTurn()
        {
            index = -1;
            phase = ANALYZE;
            sighted_enemies = new HashSet<Unit>();
            OnEndTurn?.Invoke(this);
        }

        public int Player { get; set; }

        private MoveControl move = new MoveControl();
        private Dictionary<Pos, Unit> units;
        private Dictionary<Point, Unit> enemies;
        private HashSet<Unit> sighted_enemies = new HashSet<Unit>();
        private List<Pos> los;
        private Point target;
        private Map map;
        private Engine engine;
        private TimeSpan last_time = TimeSpan.Zero;
        private int index = -1, selected = -1, phase = ANALYZE;

        private const int ANALYZE = 0;
        private const int MOVE = 1;
        private const int ATTACK = 2;
        private const int NOTHING = 3;

        public MapAI(Map map, int player)
        {
            Player = player;
            this.map = map;

            move.OnTargetInSight += new TargetInSightEventHandle((sender, source, unit) => phase = ANALYZE);
            move.OnMovingDone += new MovingDoneEventHandle((sender, source) => phase = ANALYZE);
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
                //řazení podle priority strategie AI
                units = units.OrderByDescending(kvp => (int)kvp.Value.AIStrategy).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                if (index >= units.Count)
                    EndTurn();
                else
                {
                    var p = units.Keys.ToArray()[index];
                    if (selected == -1)
                    {
                        selected = MoveControl.GetIndex(map, p.X, p.Y);
                        if (engine.GetVisibility(p.X, p.Y) == Visibility.Visible)
                            engine.SetViewToPostion(p.X, p.Y);
                    }

                    //analýza dalšího kroku
                    if (phase == ANALYZE)
                    {
                        analyze(units[p], new Point(p.X, p.Y), areas);
                    }

                    //pohyb jednotky
                    if (phase == MOVE)
                    {
                        if (engine.GetVisibility(p.X, p.Y) == Visibility.Visible)
                            engine.SetViewToPostion(p.X, p.Y);

                        move.Update(time, map, engine, areas, ref selected, Player);
                    }

                    //útok na jednotku
                    if (phase == ATTACK)
                    {
                        UnitControl unit = areas[selected].UnitControl;
                        if (!unit.IsAttacking)
                        {
                            AreaControl area = areas[MoveControl.GetIndex(map, target.X, target.Y)];
                            area.UpdateData(map, engine, time);
                            UnitControl enemy = area.UnitControl;
                            if (enemy == null) //jednotka umřela
                                phase = ANALYZE;
                            else
                            {
                                int attack = unit.Attack(enemy);
                                if (attack == -1) //došla stamina
                                    phase = NOTHING;
                                else if (attack == -2) //jednotka nemá dostřel - chyba v reverzním algoritmu pro AttackRange
                                    phase = NOTHING;
                            }

                            last_time = time.TotalGameTime;
                        }
                    }

                    //jednotka už nebude nic dělat
                    if (phase == NOTHING)
                    {
                        phase = ANALYZE;
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
                    scout(unit, point);
                    break;
            }
        }

        private void scout(Unit unit, Point point)
        {
            if (sighted_enemies.Count == 0) //nevidí nepřítele
                search();
            else
            {
                List<Pos> los = engine.GetLineOfSight(point.X, point.Y);
                List<Point> enemy_in_sight = new List<Point>();
                foreach (Pos p in los)
                {
                    Unit enemy = map.GetUnit(p.X, p.Y);
                    if (enemy != null && enemy.Player == 1)
                        enemy_in_sight.Add(new Point(p.X, p.Y));
                }

                if (enemy_in_sight.Count == 0) //nepřítel není na dohled
                {/*
                    Dictionary<Pos, double> mob = engine.GetMobility(point.X, point.Y, unit, -1);
                    foreach (KeyValuePair<Pos, double> kvp in mob)
                    {
                        if (engine.GetVisibility(kvp.Key.X, kvp.Key.Y, Player) == Visibility.Visible)
                        {
                            Unit enemy = map.GetUnit(kvp.Key.X, kvp.Key.Y);
                            if (enemy != null && enemy.Player == 1)
                            {
                                HashSet<Pos> in_sight = new HashSet<Pos>();
                                foreach (KeyValuePair<Point, Unit> kvp in enemies)
                                {
                                    foreach (Pos p in engine.GetAttackRange(kvp.Key.X, kvp.Key.Y, unit, unit.Range))
                                    {
                                        in_sight.Add(p);
                                        if (p.X == point.X && p.Y == point.Y)
                                        {
                                            targets.Add(kvp.Key);
                                            phase = ATTACK;
                                        }
                                    }
                                }

                                phase = MOVE;

                                Point next = next_hop(new Point(kvp.Key.X, kvp.Key.Y), unit, in_sight, null, mob);
                                if (next == new Point(-1, -1) || next == point)
                                    phase = NOTHING;
                                else
                                {
                                    engine.Mobility = real_mob;
                                    move.Move(engine, real_mob, areas[selected].UnitControl, next);
                                }

                                break;
                            }
                        }
                    }*/
                }
                else //nepřítel na dohled
                    goback(); 
            }
        }

        private void search()
        {

        }

        private void goback()
        {

        }

        private void rush(Unit unit, Point point, List<AreaControl> areas)
        {
            Dictionary<Pos, double> real_mob = engine.GetMobility(point.X, point.Y, unit); //reálná pohbylivost

            List<Point> targets = new List<Point>();

            //pozice, na kterých dostřelí na jednotku
            HashSet<Pos> in_range = new HashSet<Pos>();
            foreach (KeyValuePair<Point, Unit> kvp in enemies)
            {
                foreach (Pos p in engine.GetAttackRange(kvp.Key.X, kvp.Key.Y, unit, unit.Range))
                {
                    in_range.Add(p);
                    if (p.X == point.X && p.Y == point.Y)
                    {
                        targets.Add(kvp.Key);
                        phase = ATTACK;
                    }
                }
            }

            if (phase == ANALYZE)
            {
                phase = MOVE;
                Point next = next_hop(point, unit, in_range, real_mob);
                if (next == new Point(-1, -1) || next == point)
                    phase = NOTHING;
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

        private void set_mobility(ref Dictionary<Pos, double> real_mob, ref Dictionary<Pos, double> mob, Point point, Unit unit)
        {
            if (mob == null)
                mob = engine.GetMobility(point.X, point.Y, unit, -1); //1000x větší pohyblivost

            if (real_mob == null)
                real_mob = engine.GetMobility(point.X, point.Y);
        }

        private Point next_hop(Point point, Unit unit, HashSet<Pos> places,
            Dictionary<Pos, double> real_mob = null,
            Dictionary<Pos, double> mob = null)
        {
            set_mobility(ref real_mob, ref mob, point, unit);

            //zbylá stamina po přesunutí na povolené pozice
            List<KeyValuePair<Pos, double>> _mob_to_enemy = mob.Where(kvp => places.Contains(kvp.Key)).ToList();

            //setřízení pozic, podle vzdálenosti
            _mob_to_enemy.Sort((kvp1, kvp2) => kvp1.Value.CompareTo(kvp2.Value) * -1);
            Dictionary<Pos, double> mob_to_enemy = _mob_to_enemy.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            //nelze se dostat na pozici, ze které můžu zaútočit na jednotku
            if (mob_to_enemy.Count > 0)
            {
                Pos key = mob_to_enemy.Keys.ToArray()[0];
                List<Pos> points = engine.GetMove(mob, point.X, point.Y, key.X, key.Y); //trasa pohybu

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
