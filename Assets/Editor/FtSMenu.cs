using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Editor
{
    public class FtSMenu : MonoBehaviour
    {
        //[MenuItem("PE/Remove prefs")]
        [MenuItem("FtS/Remove prefs")]
        static void CleartPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }
        /*
        [MenuItem("PE/Clear cache")]
        static void ClearCache()
        {
            Caching.ClearCache();
        }
        
        [MenuItem("PE/Write PROGRESS")]
        static void WriteOldSave()
        {
            PlayerPrefs.DeleteKey("PROGRESSV2");
            PlayerPrefs.SetString("PROGRESS","Easy:33,Normal:116,Hard:227,Super:301,Special:406,God:501");
        }
        
        [MenuItem("PE/PROGRESSV2/Random")]
        private static void WriteOldSaveV2()
        {
            var progress = GenerateProgress();
            SaveProgress(progress);
        }
        
        [MenuItem("PE/PROGRESSV2/Zero")]
        private static void WriteOldSaveV2Zero()
        {
            var progress = GenerateProgress(true);
            SaveProgress(progress);
        }
        
        private static void SaveProgress(Dictionary<Difficult,List<int>> progress)
        {
            var savestr = string.Empty;
            foreach (var keyValuePair in progress)
            {
                var stageStr = string.Empty;
                foreach (var stage in keyValuePair.Value)
                {
                    stageStr += stage + ":";
                }
                savestr += keyValuePair.Key.ToString() + ";" + stageStr.Remove(stageStr.Length - 1) + ",";
            }
            savestr = savestr.Remove(savestr.Length - 1);
            PlayerPrefs.SetString("PROGRESSV2",savestr);
            PlayerPrefs.Save();
        }

        private static Dictionary<Difficult,List<int>> GenerateProgress(bool zero = false)
        {
            var progress = new Dictionary<Difficult,List<int>>();
            foreach (Difficult diff in Enum.GetValues(typeof(Difficult)))
            {
                var stages = new List<int>();
                for (int i = 0; i < 40; i++)
                {
                    stages.Add(zero ? 0 : Random.Range(1, 20));
                }
                progress.Add(diff, stages);
            }

            return progress;
        }
        */
    }
}