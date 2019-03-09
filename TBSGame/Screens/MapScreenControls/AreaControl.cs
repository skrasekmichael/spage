using MapDriver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSGame.Screens.MapScreenControls
{
    public class AreaControl : MapControl
    {
        public bool IsMouseOver { get; private set; } = false;
        public UnitControl UnitControl { get; set; } = null;
        public MapObjectControl Object { get; set; } = null;
        public int Index { get; set; } = -1;

        public event EventHandler AreaClicked;
        private void OnAreaClicked(EventArgs e)
        {
            AreaClicked?.Invoke(this, e);
        }

        public event EventHandler HoverArea;
        private void OnHoverArea(EventArgs e)
        {
            HoverArea?.Invoke(this, e);
        }

        public event EventHandler LeaveArea;
        private void OnLeaveArea(EventArgs e)
        {
            LeaveArea?.Invoke(this, e);
        }

        private VertexPositionTexture[] vertex = new VertexPositionTexture[4];
        private VertexPositionColor[] line = new VertexPositionColor[5];
        private Terrain terrain;
        private Vector2 p1, p2, p3, p4, center;
        private string texture;
        private bool draw_unit = false, is_mouse_down = false;

        private string get(string key)
        {
            switch (key)
            {
                case "":
                    return terrain.Texture;
                case "hover":
                    return $"{terrain.Texture}hover";
                case "mob":
                    return $"{terrain.Texture}mob";
                case "inrange":
                    return $"{terrain.Texture}inrange";
                case "hovermob":
                    return $"{terrain.Texture}hovermob";
                case "hoverinrange":
                    return $"{terrain.Texture}hoverinrange";
                case "hide":
                    return $"{terrain.Texture}hide";
                case "hidemob":
                    return $"{terrain.Texture}hidemob";
                case "hideinrange":
                    return $"{terrain.Texture}hideinrange";
                case "hoverhide":
                    return $"{terrain.Texture}hoverhide";
                case "hoverhidemob":
                    return $"{terrain.Texture}hoverhidemob";
                case "hoverhideinrange":
                    return $"{terrain.Texture}hoverhideinrange";
            }
            return terrain.Texture;
        }

        public AreaControl(Terrain terrain, int x, int y)
        {
            this.X = x;
            this.Y = y;
            this.terrain = terrain;
        }

        protected override void load()
        {
            if (driver[terrain.Texture] == null)
            {
                driver.LoadTexture(terrain.Texture);
                Texture2D base_texture = driver[terrain.Texture];
                Texture2D hover = sprite.Shadow(base_texture, Color.Crimson, 0.2f);
                Texture2D mob = sprite.Shadow(base_texture, Color.CornflowerBlue, 0.4f);
                Texture2D hovermob = sprite.Shadow(mob, Color.Crimson, 0.2f);
                Texture2D hide = sprite.Shadow(base_texture, Color.Black, 0.4f);
                Texture2D hidemob = sprite.Shadow(mob, Color.Black, 0.4f);
                Texture2D hoverhide = sprite.Shadow(hover, Color.Black, 0.4f);
                Texture2D hoverhidemob = sprite.Shadow(hovermob, Color.Black, 0.4f);
                Texture2D inrange = sprite.Shadow(base_texture, Color.Orange, 0.5f);
                Texture2D hoverinrange = sprite.Shadow(inrange, Color.Crimson, 0.2f);
                Texture2D hideinrange = sprite.Shadow(inrange, Color.Black, 0.4f);
                Texture2D hoverhideinrange = sprite.Shadow(hoverinrange, Color.Black, 0.4f);

                driver.LoadTexture(get("hover"), hover);
                driver.LoadTexture(get("mob"), mob);
                driver.LoadTexture(get("inrange"), inrange);
                driver.LoadTexture(get("hoverinrange"), hoverinrange);
                driver.LoadTexture(get("hovermob"), hovermob);
                driver.LoadTexture(get("hide"), hide);
                driver.LoadTexture(get("hidemob"), hidemob);
                driver.LoadTexture(get("hideinrange"), hideinrange);
                driver.LoadTexture(get("hoverhide"), hoverhide);
                driver.LoadTexture(get("hoverhidemob"), hoverhidemob);
                driver.LoadTexture(get("hoverhideinrange"), hoverhideinrange);
            }
        }

        public void UpdateData(Map map, Engine engine, GameTime time)
        {
            Visibility visibility = engine.GetVisibility(X, Y);
            bool drawing = visibility == Visibility.Visible || visibility == Visibility.Sighted;

            Unit unit = map.GetUnit(X, Y);
            if (unit != null)
            {
                if (UnitControl == null)
                {
                    UnitControl = new UnitControl(unit, X, Y);
                    UnitControl.Load(graphics, font);
                    UnitControl.BaseIndex = this.Index;
                }

                UnitControl.Update(map, engine, time);
                draw_unit = drawing;
            }
            else
            {
                UnitControl = null;
                draw_unit = false;
            }


            MapObject obj = map.GetMapObject(X, Y);
            if (obj != null)
            {
                if (Object == null)
                {
                    Object = new MapObjectControl(obj, X, Y);
                    Object.Load(graphics, font);
                }

                Object.Update(map, engine);
            }
            else
                Object = null;
        }

        public void Update(Map map, Engine engine, GameTime time, KeyboardState state, MouseState mouse, bool hover)
        {
            p1 = to_vector(engine.GetPoint(X, Y));
            p2 = to_vector(engine.GetPoint(X + 1, Y));
            p3 = to_vector(engine.GetPoint(X, Y + 1));
            p4 = to_vector(engine.GetPoint(X + 1, Y + 1));

            Vector2 pos = new Vector2(mouse.Position.X - Width / 2, Height / 2 - mouse.Position.Y);
            bool old_over = IsMouseOver;
            IsMouseOver = is_in_area(new Vector2[] { p1, p2, p3, p4 }, pos);

            vertex = new VertexPositionTexture[]
            {
                new VertexPositionTexture(new Vector3(p3, 0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(p4, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(p1, 0), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(p2, 0), new Vector2(1, 0))
            };

            Color grid = new Color(130, 130, 130);
            line = new VertexPositionColor[]
            {
                new VertexPositionColor(new Vector3(p1, 0), grid),
                new VertexPositionColor(new Vector3(p2, 0), grid),
                new VertexPositionColor(new Vector3(p4, 0), grid),
                new VertexPositionColor(new Vector3(p3, 0), grid),
                new VertexPositionColor(new Vector3(p1, 0), grid)
            };

            UpdateData(map, engine, time);

            texture = "";
            if (IsMouseOver && hover)
            {
                OnHoverArea(new EventArgs());
                texture += "hover";
                if (mouse.LeftButton != ButtonState.Pressed && is_mouse_down)
                {
                    OnAreaClicked(new EventArgs());
                    is_mouse_down = false;
                }
                else
                    is_mouse_down = mouse.LeftButton == ButtonState.Pressed;
            }
            else
            {
                is_mouse_down = false;
                if (old_over)
                    OnLeaveArea(new EventArgs());
            }

            if (engine.GetVisibility(X, Y) == Visibility.Hidden || engine.GetVisibility(X, Y) == Visibility.Sighted)
                texture += "hide";

            if (mouse.RightButton == ButtonState.Pressed)
            {
                if (engine.Mobility != null && engine.Mobility.ContainsKey(new System.Drawing.Point(X, Y)) && UnitControl == null)
                    texture += "mob";
            }
            else if (state.IsKeyDown(Keys.A) == true)
            {
                if (engine.AttackRange != null && engine.AttackRange.Contains(new System.Drawing.Point(X, Y)))
                    texture += "inrange";
            }
        }

        public void Draw()
        {
            sprite.FillArea(vertex, driver[get(texture)]);
            sprite.DrawLine(line);

            if (draw_unit)
                UnitControl.Draw();
            else
                if (UnitControl?.IsAnimating == true)
                    UnitControl?.DrawBallisticTrajectory();
            Object?.Draw();
        }
    }
}
