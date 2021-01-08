using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ShortTimeMsgHandler
    {
        private GameObject shortTimeTextGameObj;
        private Text shortTimeMsgText;
        private Stopwatch stopwatch;
        private const long SHORT_TIME_MSG_DURATION_MS = 2500;  // 2.5s

        public ShortTimeMsgHandler()
        {
            this.shortTimeTextGameObj = GameObject.Find("ShortTimeMsgText");
            this.shortTimeMsgText = this.shortTimeTextGameObj.GetComponentInChildren<Text>();
            this.shortTimeTextGameObj.SetActive(false);
            this.stopwatch = new Stopwatch();
        }

        public void Show(string msg)
        {
            shortTimeTextGameObj.SetActive(true);
            shortTimeMsgText.text = msg;
            stopwatch.Stop();
            stopwatch.Reset();
            stopwatch.Start();
        }

        public void Hide()
        {
            stopwatch.Stop();
            stopwatch.Reset();
            shortTimeTextGameObj.SetActive(false);
        }

        public void Update()
        {
            stopwatch.Stop();
            if (stopwatch.ElapsedMilliseconds >= SHORT_TIME_MSG_DURATION_MS)
            {
                Hide();
            }
            else
            {
                stopwatch.Start();
            }
        }
    }
}
