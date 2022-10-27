using UnityEngine;

namespace KK.Utility
{
    public class KKDebug : MonoBehaviour
    {
        [SerializeField] bool enableLogging;
        public void Log(object message)
        {
            if (enableLogging)
                Debug.Log(message);
        }
    }
}
