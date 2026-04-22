using System.Drawing;

namespace Brumak_Shared.Drawings.Skin
{
    public interface ISkinDrawer
    {
        void DrawCentered(Graphics g, Size clientSize);
        void DrawOnTile(Graphics g, Point tilePosition, int tileWidth, int tileHeight);
        void DrawOnTileInterpolated(Graphics gfx, Point iso, int tileWidth, int tileHeight, int offsetYAnim);
        ISkinDrawer SetColors(Color skin, Color primary, Color secondary);
    }
}
