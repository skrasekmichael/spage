using MapDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TBSGame.Screens.MapScreenControls.MapWindows
{
    public class MiniMapWindow : MapWindow
    {
        private Texture2D p1, enemy;
        private Map map;
        private Engine engine;
        private Rectangle bounds;
        private int part;
        private VertexPositionColor[] view, fill, border;
        private bool pressed = false;

        public MiniMapWindow(Map map)
        {
            this.map = map;
        }

        public void Update(Map map, Engine engine, MouseState mouse)
        {
            this.map = map;
            this.engine = engine;

            if (Visible)
            {
                var points = engine.GetMiniMap(part / 2);

                if (fill == null)
                {
                    Vector2[] bounds = new Vector2[] {
                        to_vector(points[0, 0]),
                        to_vector(points[map.Width, 0]),  
                        to_vector(points[0, map.Height]),
                        to_vector(points[map.Width, map.Height]),
                    };

                    Color black = Color.Black;
                    fill = get_vertex(bounds, black);

                    bounds[0].Y += -2;
                    bounds[1].X -= -2;
                    bounds[2].X += -2;
                    bounds[3].Y -= -2;

                    Color frame = Color.Silver;
                    border = get_vertex(bounds, frame);
                }

                double wcoef = 2 * engine.Width / part;
                double hcoef = 2 * engine.Height / part;
                int nw = (int)(Width / wcoef);
                int nh = (int)(Height / hcoef);

                Point center = new Point((int)(engine.View.X / -wcoef), (int)(engine.View.Y / -hcoef));

                Color color = Color.White;
                view = new VertexPositionColor[]
                {
                    new VertexPositionColor(new Vector3(center.X - nw / 2, center.Y + nh / 2, 0), color),
                    new VertexPositionColor(new Vector3(center.X + nw / 2, center.Y + nh / 2, 0), color),
                    new VertexPositionColor(new Vector3(center.X + nw / 2, center.Y - nh / 2, 0), color),
                    new VertexPositionColor(new Vector3(center.X - nw / 2, center.Y - nh / 2, 0), color),
                    new VertexPositionColor(new Vector3(center.X - nw / 2, center.Y + nh / 2, 0), color)
                };

                if (mouse.RightButton == ButtonState.Pressed)
                {
                    Visible = false;
                    pressed = false;
                    return;
                }

                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    if (is_in_area(
                           new Vector2[]
                           {
                               parse(new Vector2(points[0, 0].X, points[0, 0].Y)),
                               parse(new Vector2(points[map.Width, 0].X, points[map.Width, 0].Y)),
                               parse(new Vector2(points[0, map.Height].X, points[0, map.Height].Y)),
                               parse(new Vector2(points[map.Width, map.Height].X, points[map.Width, map.Height].Y))
                           }, mouse.Position.ToVector2()))
                    {
                        Vector2 delta = new Vector2(Width / 2, Height / 2) - mouse.Position.ToVector2();
                        engine.View = new System.Drawing.PointF((float)(delta.X * wcoef), (float)(delta.Y * -hcoef));
                    }
                    else
                    {
                        if (!pressed)
                        {
                            Visible = false;
                            return;
                        }
                    }
                    pressed = true;
                }
                else
                    pressed = false;
            }
        }

        protected override void draw()
        {
            sprite.FillArea(border);
            sprite.FillArea(fill);

            var points = engine.MiniMapPoints;

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    Visibility visibility = engine.GetVisibility(x, y);
                    if (visibility != MapDriver.Visibility.Gone)
                    {
                        string shad = (visibility == MapDriver.Visibility.Hidden || visibility == MapDriver.Visibility.Sighted) ? "shadow" : "";
                        Vector2[] pos = engine.GetMiniMapPoints(points, x, y).Select(p => to_vector(p)).ToArray();
                        VertexPositionTexture[] vertex1 = get_vertex(pos);

                        pos[0].Y += 1;
                        pos[1].X -= 1;
                        pos[2].X += 1;
                        pos[3].Y -= 1;

                        VertexPositionTexture[] vertex2 = get_vertex(pos);

                        Terrain t = map.GetTerrain(x, y);
                        sprite.FillArea(vertex1, driver[$"map{t.Texture}" + shad]);

                        MapObject o = map.GetMapObject(x, y);
                        if (o != null)
                            sprite.FillArea(vertex2, driver[$"map{o.Texture}" + shad]);

                        if (visibility == MapDriver.Visibility.Visible || visibility == MapDriver.Visibility.Sighted)
                        {
                            Unit unit = map.GetUnit(x, y);
                            if (unit != null)
                                sprite.FillArea(vertex2, unit.Player == 1 ? p1 : enemy);
                        }
                    }
                }
            }

            sprite.DrawLine(view);
        }

        protected override void load()
        {
            p1 = sprite.GetColorFill(Color.CornflowerBlue);
            enemy = sprite.GetColorFill(Color.Red);

            double maxwidth = Width * 0.9f;
            double maxheight = (Height - 75) * 0.9f;

            int max = Math.Max(map.Width, map.Height);
            double maxwpart = maxwidth / max;
            double maxhpart = maxheight / max;

            part = (int)Math.Floor(Math.Min(maxwpart, maxhpart));
            int width = part * map.Width, height = part * map.Height;

            bounds = new Rectangle((Width - width) / 2, (Height - 75 - height) / 2, width, height);

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    Terrain t = map.GetTerrain(x, y);
                    if (driver[$"map{t.Texture}"] == null)
                    {
                        System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(t.Color);
                        Texture2D bt = sprite.GetColorFill(color);
                        driver.LoadTexture($"map{t.Texture}", bt);
                        driver.LoadTexture($"map{t.Texture}shadow", sprite.Shadow(bt, Color.Black, 0.4f));
                    }
                    MapObject o = map.GetMapObject(x, y);
                    if (o != null && driver[$"map{o.Texture}"] == null)
                    {
                        System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(o.Color);
                        Texture2D bt = sprite.GetColorFill(color);
                        driver.LoadTexture($"map{o.Texture}", bt);
                        driver.LoadTexture($"map{o.Texture}shadow", sprite.Shadow(bt, Color.Black, 0.4f));
                    }
                }
            }
        }
    }
}
