using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace APA.Core
{
    public class APAGameLoaderComponent : MonoBehaviour
    {
        private void Start()
        {
            var manager = new APAManager();
            manager.LoadManagers(() =>
            {
                int mainMenuIndex = 1;
                SceneManager.LoadScene(mainMenuIndex);
            });
        }
    }
}
