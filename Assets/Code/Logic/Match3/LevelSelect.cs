using UnityEngine;

namespace Code.Logic.Match3
{
    public class LevelSelect : MonoBehaviour
    {
        public ButtonPlayerPrefs[] buttons;

        private void OnValidate()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i].gameObject != null && (buttons[i].stars == null || buttons[i].stars.Length == 0))
                {
                    GameObject[] stars = new GameObject[3];
                    for (int starIndex = 1; starIndex <= 3; starIndex++)
                    {
                        Transform starTransform = buttons[i].gameObject.transform.Find($"star{starIndex}");
                        if (starTransform != null)
                        {
                            stars[starIndex - 1] = starTransform.gameObject;
                        }
                    }
                    buttons[i].stars = stars;
                }
            }
        }

        private void Start()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                int score = PlayerPrefs.GetInt(buttons[i].playerPrefKey, 0);

                if (buttons[i].stars != null)
                {
                    for (int starIndex = 0; starIndex < buttons[i].stars.Length; starIndex++)
                    {
                        if (buttons[i].stars[starIndex] != null)
                        {
                            buttons[i].stars[starIndex].SetActive(starIndex < score);
                        }
                    }
                }
            }
        }

        public void OnButtonPress(string levelName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
        }
        
        [System.Serializable]
        public struct ButtonPlayerPrefs
        {
            public GameObject gameObject;
            public string playerPrefKey;
            public GameObject[] stars;
        };
    }
}