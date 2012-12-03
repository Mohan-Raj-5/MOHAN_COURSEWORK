using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame
{
    class GameConstants
    {
        public const float PlayfieldSizeX = 40000f;
        public const float PlayfieldSizeZ = 40000f;
        public const float PlayfieldSizeY = 20000f;

        public const int NumEnemies = 10;
        public const float EnemieskMinSpeed = 3.0f;
        public const float EnemiesMaxSpeed = 10.0f;
        public const float EnemiesSpeedAdjustment = 2.5f;
        public const float EnemiesScalar = 0.01f;
    }
}
