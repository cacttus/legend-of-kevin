using Microsoft.Xna.Framework.Graphics;


namespace Core
{
    public abstract class WorldBase
    {
        public Screen Screen { get; private set; }
        public vec2 Gravity { get; set; } = new vec2(0, -1);
        public WorldBase(Screen screen)
        {
            Screen = screen;
        }
        public abstract void Update(float dt);
        public abstract void Draw(SpriteBatch sb);
    }
}
