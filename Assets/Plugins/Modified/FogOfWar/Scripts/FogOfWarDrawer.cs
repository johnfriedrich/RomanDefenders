using UnityEngine;

namespace FoW
{
    [System.Serializable]
    public abstract class FogOfWarDrawer
    {
        protected FogOfWarMap _map;

        public virtual void Initialise(FogOfWarMap map)
        {
            _map = map;
            OnInitialise();
        }

        protected virtual void OnInitialise() { }
        public abstract void Clear(byte value);
        public abstract void Fade(int to, int amount);
        public abstract void GetValues(byte[] outvalues);
        public abstract void SetValues(byte[] values);

        protected abstract void DrawCircle(FogOfWarShapeCircle shape);
        protected abstract void DrawBox(FogOfWarShapeBox shape);
        protected abstract void DrawTexture(FogOfWarShapeTexture shape);
        public abstract void Unfog(Rect rect);

        public void Draw(FogOfWarShape shape)
        {
            if (shape is FogOfWarShapeCircle)
                DrawCircle(shape as FogOfWarShapeCircle);
            else if (shape is FogOfWarShapeBox)
                DrawBox(shape as FogOfWarShapeBox);
            else if (shape is FogOfWarShapeTexture)
                DrawTexture(shape as FogOfWarShapeTexture);
        }
    }
}
