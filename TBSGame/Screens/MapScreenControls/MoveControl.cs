using MapDriver;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSGame.Screens.MapScreenControls
{
    public delegate void TargetInSightEventHandle(object sender, Unit source, Unit unit);
    public delegate void MovingDoneEventHandle(object sender, Unit source);

    public class MoveControl
    {
        public event TargetInSightEventHandle OnTargetInSight;
        private void TargetInSight(Unit source, Unit unit)
        {
            OnTargetInSight?.Invoke(this, source, unit);
        }

        public event MovingDoneEventHandle OnMovingDone;
        private void MovingDone(Unit source)
        {
            OnMovingDone?.Invoke(this, source);
        }

        public bool IsMoving => !(index == -1);

        private List<Point> visibled = null;
        private List<MoveUnit> moves = new List<MoveUnit>();
        private int index = -1;
        private TimeSpan start_move = TimeSpan.Zero;

        public static int GetIndex(Map map, int px, int py)
        {
            int index = 0;
            for (int x = map.Width - 1; x >= 0; x--)
            {
                for (int y = map.Height - 1; y >= 0; y--)
                {
                    if (x == px && y == py)
                        return index;
                    index++;
                }
            }
            return -1;
        }

        public void Move(Engine engine, Dictionary<System.Drawing.Point, double> mob, UnitControl unit, Point end)
        {
            index = 0;
            moves = move(engine, mob, unit, end);
        }

        private List<MoveUnit> move(Engine engine, Dictionary<System.Drawing.Point, double> mob, UnitControl unit, Point end)
        {
            List<System.Drawing.Point> moves = engine.GetMove(mob, unit.X, unit.Y, end.X, end.Y);
            return moves.Select(point => new MoveUnit(unit, point.X, point.Y, mob[point])).ToList();
        }

        public void Update(GameTime time, Map map, Engine engine, List<AreaControl> areas, ref int selected_index, int player = 1)
        {
            if (index != -1)
            {
                int oldx = areas[selected_index].X;
                int oldy = areas[selected_index].Y;
                
                if (index > 0)
                { 
                    oldx = moves[index - 1].X;
                    oldy = moves[index - 1].Y;
                }

                var new_visible = map.Units.Where(kvp => kvp.Value.Player != player && engine.GetVisibility(kvp.Key.X, kvp.Key.Y, player - 1) == Visibility.Visible || engine.GetVisibility(kvp.Key.X, kvp.Key.Y, player - 1) == Visibility.Sighted).Select(kvp => new Point(kvp.Key.X, kvp.Key.Y)).ToList();
                if (visibled == null)
                    visibled = new_visible;
                else
                {
                    for (int i = 0; i < new_visible.Count; i++)
                    {
                        if (visibled.IndexOf(new_visible[i]) == -1)
                        {
                            moves.RemoveRange(index, moves.Count - index);
                            selected_index = GetIndex(map, oldx, oldy);
                            TargetInSight(areas[selected_index].UnitControl.Unit, map.GetUnit(new_visible[i].X, new_visible[i].Y));
                            break;
                        }
                    }
                    visibled = new_visible;
                }

                if (index < moves.Count)
                {
                    if (start_move == TimeSpan.Zero)
                        start_move = time.TotalGameTime - TimeSpan.FromMilliseconds(50);

                    if (time.TotalGameTime - start_move >= TimeSpan.FromMilliseconds(50))
                    {
                        MoveUnit mc = moves[index];
                        mc.Update(map, oldx, oldy);
                        selected_index = GetIndex(map, mc.X, mc.Y);
                        index++;

                        start_move = time.TotalGameTime;
                    }
                }
                else
                {
                    MoveUnit mc = moves.Last();
                    selected_index = GetIndex(map, mc.X, mc.Y);
                    mc.UnitControl.Unit.Stamina = (ushort)mc.Stamina;

                    if (player == 1)
                    {
                        engine.Mobility = engine.GetMobility(mc.X, mc.Y);
                        engine.AttackRange = engine.GetAttackRange(mc.X, mc.Y);
                    }

                    visibled = null;
                    index = -1;
                    moves = new List<MoveUnit>();

                    MovingDone(mc.UnitControl.Unit);
                    return;
                }
            }
        }

    }
}
