using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace HuntableHerds.Extensions {
    public static class MissionExtensions {
        public static Vec3 GetTrueRandomPositionAroundPoint(this Mission mission, Vec3 position, float minDistance, float maxDistance, bool nearFirst = false) {
            Vec3 randomPos = mission.GetRandomPositionAroundPoint(position, minDistance, maxDistance, nearFirst);
            while (randomPos == position)
                randomPos = mission.GetRandomPositionAroundPoint(position, minDistance, maxDistance, nearFirst);
            return randomPos;
        }
    }
}
