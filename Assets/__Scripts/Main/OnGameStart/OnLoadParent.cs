using UnityEngine;
using KK.Utility;

namespace KK
{
    public class OnLoadParent : MonoBehaviour
    {
        [RequireInterface(typeof(IOnLoadListener))] public Object[] listeners;


        public void OnLoad(MonoBehaviour caller)
        {
            foreach (var listener in listeners) 
            {
                (listener as IOnLoadListener).OnLoad(caller);
            }
        }
    }
}
