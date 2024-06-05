using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;

/// <summary>
/// 인게임 데이터를 받아와서 해당 데이터를 기반으로 AI의 패턴에 변화를 줌.
/// </summary>
namespace LGProject
{
    public enum DATA_TYPE
    {
        NormalAttack,
        DashAttack,

        NormalAttackHit,
        DashAttackHit,
        JumpAttackHit,

        Jump,
        Chasing,
        Movement,

        Guard,
    }
    public struct DATA
    {
        public int Round;
        public int NormalAttackCount;
        public int DashAttackCount;

        public int NormalAttackHitCount;
        public int DashAttackHitCount;

        public int JumpCount;
        public int ChasingCount;
        public int NormalMoveCount;

        public int GuardCount;

        public void Init(string[] str)
        {
            #region Omit
            try
            {
                if (str[0] != "")
                    Round                   = int.Parse(str[0]);
                if (str[1] != "")
                    NormalAttackCount       = int.Parse(str[1]);
                if (str[2] != "")
                    DashAttackCount         = int.Parse(str[2]);
                if (str[3] != "")
                    NormalAttackHitCount    = int.Parse(str[3]);
                if (str[4] != "")
                    DashAttackHitCount      = int.Parse(str[4]);
                if (str[5] != "")
                    JumpCount               = int.Parse(str[5]);

                if (str[6] != "")
                    ChasingCount            = int.Parse(str[6]);
                if (str[7] != "")
                    NormalMoveCount         = int.Parse(str[7]);
                if (str[8] != "")
                    GuardCount              = int.Parse(str[8]);
            }
            catch
            {
                Debug.Log($"contents : null");
            }
            #endregion
        }
    }

    public class FileManager : MonoBehaviour
    {
        public static FileManager Instance { get; private set; }
        public Dictionary<int, DATA> AllData = new Dictionary<int, DATA>();

        public DATA Data;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            Data = new DATA();
        }

        #region getPath

        public static string GetPath(string fileName)
        {
            string path = GetPath();
            return Path.Combine(GetPath(), fileName);
        }
        public static string GetPath()
        {
            string path = null;
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    path = Application.persistentDataPath;
                    path = path.Substring(0, path.LastIndexOf('/'));
                    return Path.Combine(Application.persistentDataPath, "Resources/");
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    path = Application.persistentDataPath;
                    path = path.Substring(0, path.LastIndexOf('/'));
                    return Path.Combine(path, "Assets", "Resources/");
                case RuntimePlatform.WindowsEditor:
                    path = Application.dataPath;
                    path = path.Substring(0, path.LastIndexOf('/'));
                    return Path.Combine(path, "Assets", "Resources/");
                default:
                    path = Application.dataPath;
                    path = path.Substring(0, path.LastIndexOf('/'));
                    return Path.Combine(path, "Resources/");
            }
        }
        #endregion

        static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
        static char[] TRIM_CHARS = { '\"' };

        // 데이터를 반환함.
        //public static DATA 
        public void LoadData()
        {
            #region Omit
            TextAsset _text = (TextAsset)Resources.Load("ExcelFileCSV");
            string testFile = _text.text;
            bool endOfFile = false;
            var data_values = testFile.Split('\n');
            int count1 = 0;
            try
            {
                while (!endOfFile)
                {
                    DATA structData = new DATA();
                    if (count1 == 0)
                    {
                        count1++;
                        continue;
                    }
                    var data_value = data_values[count1].Split(',');
                    if (data_value == null)
                    {
                        endOfFile = true;
                        break;
                    }
                    if (data_value[0] == "")
                    {
                        endOfFile = true;
                        break;
                    }
                    structData.Init(data_value);
                    AllData.Add(int.Parse(data_value[0]), structData);
                    count1++;
                }
            }
            catch
            {
                Debug.LogWarning("CSV파일이 존재하지 않거나 받아온 데이터가 null 값입니다.");
            }
            #endregion
        }

        public string fileName = "ExcelFileCSV.csv";

        List<string[]> data = new List<string[]>();

        public void SaveData()
        {

            string[] tempData = new string[9];
            tempData[0] = Data.Round.ToString();
            tempData[1] = Data.NormalAttackCount.ToString();
            tempData[2] = Data.DashAttackCount.ToString();
            tempData[3] = Data.NormalAttackHitCount.ToString();
            tempData[4] = Data.DashAttackHitCount.ToString();
            tempData[5] = Data.JumpCount.ToString();
            tempData[6] = Data.ChasingCount.ToString();
            tempData[7] = Data.NormalMoveCount.ToString();
            tempData[8] = Data.GuardCount.ToString();

            data.Add(tempData);

            string[][] output = new string[data.Count][];

            for (int i = 0; i < output.Length; i++)
            {
                output[i] = data[i];
            }

            int length = output.GetLength(0);
            string delimiter = ",";

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                sb.AppendLine(string.Join(delimiter, output[i]));
            }

            string filepath = FileManager.GetPath();

            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }

            StreamWriter outStream = System.IO.File.CreateText(filepath + fileName);
            outStream.Write(sb);
            outStream.Close();
        }

        public void SetInGameData(DATA_TYPE dataType)
        {
            switch (dataType)
            {
                case DATA_TYPE.NormalAttack:
                    Data.NormalAttackCount++;
                    //Debug.Log("NAttack");
                    break;
                case DATA_TYPE.DashAttack:
                    Data.DashAttackCount++;
                    //Debug.Log("DAttack");
                    break;
                case DATA_TYPE.NormalAttackHit:
                    Data.NormalAttackHitCount++;
                    //Debug.Log("NAttackHit");
                    break;
                case DATA_TYPE.DashAttackHit:
                    Data.DashAttackHitCount++;
                    //Debug.Log("DAttackHit");
                    break;
                case DATA_TYPE.Jump:
                    Data.JumpCount++;
                    //Debug.Log("Jump");
                    break;
                case DATA_TYPE.Chasing:
                    Data.ChasingCount++;
                    //Debug.Log("Chasing");
                    break;
                case DATA_TYPE.Movement:
                    Data.NormalMoveCount++;
                    //Debug.Log("Movement");
                    break;
                case DATA_TYPE.Guard:
                    Data.GuardCount++;
                    //Debug.Log("Guard");
                    break;
                default:
                    break;
            }
        }
    }
}