using System;
using Microsoft.Xna.Framework;

namespace DeltaTune.Settings
{
    public class PositionPresetHelper
    {
        private static readonly Vector2[] presetPositions = new Vector2[5];

        static PositionPresetHelper()
        {
            presetPositions[(int)PositionPreset.TopLeft] = new Vector2(0.025f, 0.055f);
            presetPositions[(int)PositionPreset.TopRight] = new Vector2(0.74f, 0.055f);
            presetPositions[(int)PositionPreset.BottomLeft] = new Vector2(0.025f, 0.975f);
            presetPositions[(int)PositionPreset.BottomRight] = new Vector2(0.74f, 0.975f);
            presetPositions[(int)PositionPreset.Original] = new Vector2(0.74f, 0.66f);
        }

        public static Vector2 GetFractionalPosition(PositionPreset preset)
        {
            return presetPositions[(int)preset];
        }

        public static PositionPreset GetPositionPreset(Vector2 preset)
        {
            if(preset == presetPositions[(int)PositionPreset.TopLeft]) return PositionPreset.TopLeft;
            if(preset == presetPositions[(int)PositionPreset.TopRight]) return PositionPreset.TopRight;
            if(preset == presetPositions[(int)PositionPreset.BottomLeft]) return PositionPreset.BottomLeft;
            if(preset == presetPositions[(int)PositionPreset.BottomRight]) return PositionPreset.BottomRight;
            if(preset == presetPositions[(int)PositionPreset.Original]) return PositionPreset.Original;
            
            throw new ArgumentOutOfRangeException(nameof(preset));
        }
    }
}