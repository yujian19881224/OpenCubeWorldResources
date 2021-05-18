using UnityEngine;
using System.Collections;


namespace X
{
    public class GameCache : Singleton<GameCache>
    {
        protected override void InitSingleton()
        {

        }

        public void AddCache( GameObject obj )
        {
            obj.transform.parent = transform;
            obj.SetActive( false );
        }




    }

}
