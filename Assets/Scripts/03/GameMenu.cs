using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    public Image gameMenuImage;
    public AudioSource BGM_audio;
    public Toggle BGM_toggle;
    public PlayerMovement playerMovement;
    public Enemy enemyobject;

    void Start()
    {
        gameMenuImage.gameObject.SetActive(false);
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (GameManager.instance.isPaused)
            {
                //恢复游戏
                ResumeGame();
            }
            else {
                //暂停游戏
                PauseGame();
            }
        }
        BGMManager();
    }


    #region //BGM,游戏暂停开关
    public void BGMToggleButton()
    {
        if (BGM_toggle.isOn)
        {
            PlayerPrefs.SetInt("BGM", 1);
        }
        else
        {
            PlayerPrefs.SetInt("BGM", 0);
        }
    }
    private void BGMManager()
    {
        if (PlayerPrefs.GetInt("BGM") == 1)
        {
            BGM_toggle.isOn = true;
            BGM_audio.enabled = true;
        }
        else
        {
            BGM_toggle.isOn = false;
            BGM_audio.enabled = false;
        }
    }

    public void ResumeGame()
    {
        Debug.Log("Resume!!");
        gameMenuImage.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
        GameManager.instance.isPaused = false;
    }
    public void PauseGame()
    {
        Debug.Log("Pause!!");
        gameMenuImage.gameObject.SetActive(true);
        Time.timeScale = 0;
        GameManager.instance.isPaused = true;
    }
    #endregion

    public void SaveOnClick() {
        //PlayerPrefSave();
        //SerializeSave();
        //JsonSave();
        XMLSave();
    }

    public void LoadOnClick() {
        //PlayerPrefLoad();
        //SerializeLoad();
        //JsonLoad();
        XMLLoad();
    }



    #region //数据保存(playerPref)
    private void PlayerPrefSave() {
        PlayerPrefs.SetInt("coins", GameManager.instance.coin);
        PlayerPrefs.SetInt("diamond", GameManager.instance.diamond);
        PlayerPrefs.SetFloat("PosX", playerMovement.transform.position.x);
        PlayerPrefs.SetFloat("PosY", playerMovement.transform.position.y);
        Debug.Log("Save Success!!");
    }

    private void PlayerPrefLoad()
    {
        GameManager.instance.coin = PlayerPrefs.GetInt("coins");
        GameManager.instance.diamond = PlayerPrefs.GetInt("diamond");
        playerMovement.transform.position = new Vector2(PlayerPrefs.GetFloat("PosX"), PlayerPrefs.GetFloat("PosY"));
        Debug.Log("Load Success!!");
    }
    #endregion

    #region //数据保存(二进制序列化BinaryFormatter)
    private void SerializeSave()
    {
        SerializedFile serializedFile = CreateCurrentState();
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Create(Application.dataPath + "/Data.txt");
        binaryFormatter.Serialize(fileStream, serializedFile);
        fileStream.Close();
    }

    private void SerializeLoad()
    {
        if (File.Exists(Application.dataPath + "/Data.txt"))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = File.Open(Application.dataPath + "/Data.txt", FileMode.Open);
            SerializedFile serializedFile = binaryFormatter.Deserialize(fileStream) as SerializedFile;

            ReturnState(serializedFile);
        }
        else
        {
            Debug.Log("The file didn't Exist!!!");
        }
    }
    #endregion

    #region //数据保存(Json)
    private void JsonSave() {
        SerializedFile serializedFile = CreateCurrentState();
        string Json = JsonUtility.ToJson(serializedFile);
        StreamWriter sw = new StreamWriter(Application.dataPath + "/JsonData.json");
        sw.Write(Json);
        sw.Close();
    }

    private void JsonLoad() {
        if (File.Exists(Application.dataPath + "/JsonData.json")) {
            StreamReader sr = new StreamReader(Application.dataPath + "/JsonData.json");
            string Json = sr.ReadToEnd();
            sr.Close();
            SerializedFile serializedFile = JsonUtility.FromJson<SerializedFile>(Json);
            ReturnState(serializedFile);
        }
        else {
            Debug.Log("File didn't Exsit!!");
        }
    }
    #endregion

    private void XMLSave() {
        SerializedFile serializedFile = CreateCurrentState();
        XmlDocument xmlDocument = new XmlDocument();
        //xml格式将会有层次关系，我们通过root和AppendChild来进行对象的组合

        XmlElement root = xmlDocument.CreateElement("SerializedFile");
        root.SetAttribute("FileName", "DataSaving");

        XmlElement coinNumElement = xmlDocument.CreateElement("CoinNum");
        coinNumElement.InnerText = serializedFile.coinNum.ToString();
        root.AppendChild(coinNumElement);

        XmlElement diamondNumElement = xmlDocument.CreateElement("DiamondNum");
        diamondNumElement.InnerText = serializedFile.diamondNum.ToString();
        root.AppendChild(diamondNumElement);

        XmlElement PosXElement = xmlDocument.CreateElement("PosX");
        PosXElement.InnerText = serializedFile.PosX.ToString();
        root.AppendChild(PosXElement);

        XmlElement PosYElement = xmlDocument.CreateElement("PosY");
        PosYElement.InnerText = serializedFile.PosY.ToString();
        root.AppendChild(PosYElement);

        XmlElement Enemyelement, enemyposx, enemyposy, isdead;
        for (int i = 0; i < serializedFile.enemyPosX.Count; i++) {
            Enemyelement = xmlDocument.CreateElement("Enemy");
            enemyposx = xmlDocument.CreateElement("positionX");
            enemyposy = xmlDocument.CreateElement("positionY");
            isdead = xmlDocument.CreateElement("IsDead");

            enemyposx.InnerText = serializedFile.enemyPosX[i].ToString();
            enemyposy.InnerText = serializedFile.enemyPosY[i].ToString();
            isdead.InnerText = serializedFile.isDead[i].ToString();

            Enemyelement.AppendChild(enemyposx);
            Enemyelement.AppendChild(enemyposy);
            Enemyelement.AppendChild(isdead);

            root.AppendChild(Enemyelement);
        }

        xmlDocument.AppendChild(root);

        xmlDocument.Save(Application.dataPath + "/Data.xml");
    }

    private void XMLLoad() {
        if (File.Exists(Application.dataPath + "/Data.xml"))
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(Application.dataPath + "/Data.xml");

            XmlNodeList coinNum = xmlDocument.GetElementsByTagName("CoinNum");
            int coinNumCount = int.Parse(coinNum[0].InnerText);

            XmlNodeList diamondNum = xmlDocument.GetElementsByTagName("DiamondNum");
            int diamondNumCount = int.Parse(diamondNum[0].InnerText);

            XmlNodeList PosX = xmlDocument.GetElementsByTagName("PosX");
            float positionX = float.Parse(PosX[0].InnerText);

            XmlNodeList PosY = xmlDocument.GetElementsByTagName("PosY");
            float positionY = float.Parse(PosY[0].InnerText);


            List<float> enemyPosX = new List<float>();
            List<float> enemyPosY =  new List<float>();
            List<bool> isDead = new List<bool>();
            XmlNodeList Enemy = xmlDocument.GetElementsByTagName("Enemy");
            foreach (var item in Enemy)
            {
                XmlNodeList enemyposx = xmlDocument.GetElementsByTagName("positionX");
                XmlNodeList enemyposy = xmlDocument.GetElementsByTagName("positionY");
                XmlNodeList isdead = xmlDocument.GetElementsByTagName("IsDead");
                enemyPosX.Add(float.Parse(enemyposx[0].InnerText));
                enemyPosY.Add(float.Parse(enemyposy[0].InnerText));
                isDead.Add(bool.Parse(isdead[0].InnerText));
            }

            SerializedFile serializedFile = new SerializedFile(coinNumCount,diamondNumCount,positionX,positionY,enemyPosX,enemyPosY,isDead);
            ReturnState(serializedFile);
        }
        else {
            Debug.Log("File didn't Exist!!");
        }
    }



    private SerializedFile CreateCurrentState() {
        List<float> enemyPosX = new List<float>();
        List<float> enemyPosY = new List<float>();
        List<bool> isDead = new List<bool>();
        foreach (var item in GameManager.instance.enemies)
        {
            enemyPosX.Add(item.pos_X);
            enemyPosY.Add(item.pos_Y);
            isDead.Add(item.isDead);
        }
        SerializedFile serializedFile = new SerializedFile(GameManager.instance.coin,
                                                                                      GameManager.instance.diamond,
                                                                                       playerMovement.transform.position.x,
                                                                                       playerMovement.transform.position.y,
                                                                                       enemyPosX, enemyPosY, isDead);
        return serializedFile;
    }
    private void ReturnState(SerializedFile serializedFile) {
        //还原游戏对象的值
        GameManager.instance.coin = serializedFile.coinNum;
        GameManager.instance.diamond = serializedFile.diamondNum;
        playerMovement.transform.position = new Vector2(serializedFile.PosX, serializedFile.PosY);

        //还原敌人的值
        for (int i = 0; i < serializedFile.isDead.Count; i++)
        {
            if (GameManager.instance.enemies[i] == null)
            {
                if (!serializedFile.isDead[i])
                {
                    Enemy enemy = Instantiate(enemyobject, new Vector2(serializedFile.enemyPosX[i], serializedFile.enemyPosY[i]), Quaternion.identity);
                    GameManager.instance.enemies[i] = enemy;
                }
            }
            else
            {
                GameManager.instance.enemies[i].transform.position = new Vector2(serializedFile.enemyPosX[i], serializedFile.enemyPosY[i]);
            }
        }
    }


}
