using UnityEngine;

namespace KK
{
    [DefaultExecutionOrder(-1)]
    public class OnLoadCaller : MonoBehaviour
    {
        void Awake()
        {
            foreach (var go in gameObject.scene.GetRootGameObjects())
            {
                if (go.TryGetComponent(out OnLoadParent listenerParent))
                {
                    listenerParent.OnLoad(this);
                }
            }
        }
    }
}
