using UnityEngine;
using System.IO;
using System;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    private bool encryptData = false; 
    private string codeWord = "DMM";
    
    public FileDataHandler(string _dataDirPath, string _dataFileName, bool _encryptData)
    {
        dataDirPath = _dataDirPath;
        dataFileName = _dataFileName;
        encryptData = _encryptData;
    }

    public void Save(GameData _data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        //try: Sẽ cố gắng làm điều gì đó, nếu không được thì sẽ cố gắng làm điều đó
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)); //Directory: Tạo 1 thư mục

            string dataToStore = JsonUtility.ToJson(_data, true);
            
            if(encryptData)
                dataToStore = EncryptDecrypt(dataToStore);

            //FileStream: nguồn truy cập tệp tin
            using(FileStream stream = new FileStream(fullPath, FileMode.Create)) //Luồng tệp cho phép làm việc với các tệp, Tạo 1 tệp và sau đó sẽ sử trình ghi luồng, cho phép ghi luồng thông tin
            {
                using(StreamWriter writer = new StreamWriter(stream)) //ghi dữ liệu lưu trữ
                {
                    writer.Write(dataToStore);
                }
            }
        }

        //catch: bắt lỗi nếu có
        catch(Exception e)
        {
            Debug.LogError("Error on trying to save data to file: " + fullPath + "\n" +e);
        }

    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadData = null;

        //Nếu tệp tồn tại
        if(File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = ""; //tạo 1 chuỗi dữ liệu để tải, mặc định là trống

                using(FileStream stream = new FileStream(fullPath, FileMode.Open)) //mở tệp và điền thông tin
                {
                    using(StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd(); //tải dữ liệu, hiện dữ liệu này đang trống nên sẽ lấp đầy nó bằng 1 trình đọc
                    }
                }

                if(encryptData)
                    dataToLoad = EncryptDecrypt(dataToLoad);

                //sau khi có dữ liệu cần tải thì sẽ tải dữ liệu. Chuyển đổi nó từ tệp về lại dữ liệu trò chơi(Json).
                loadData = JsonUtility.FromJson<GameData>(dataToLoad);

            }

            catch(Exception e)
            {
                Debug.LogError("Error on try to load data from file: " + fullPath + "\n" + e);
            }
        }

        return loadData;  // trả về dữ liệu tải
    }


    public void Delete()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        if(File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    private string EncryptDecrypt(string _data)
    {
        string modifiedData = "";

        for (int i = 0; i < _data.Length; i++)
        {
            modifiedData += (char)(_data[i] ^ codeWord[i % codeWord.Length]);
        }

        return modifiedData;
    }

}
