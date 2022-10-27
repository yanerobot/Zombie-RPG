using UnityEngine;

namespace KK
{
    public class TimeCondition
    {
        float startTime;
        float timeLimit;
        bool isStarted;

        public TimeCondition(float time)
        {
            timeLimit = time;
        }

        public bool HasTimePassed()
        {
            if (!isStarted)
            {
                startTime = Time.time;
                isStarted = true;
            }

            if (Time.time - startTime >= timeLimit)
            {
                isStarted = false;
                return true;
            }

            return false;
        }

        public void ResetTimer()
        {
            isStarted = false;
        }
    }
}
