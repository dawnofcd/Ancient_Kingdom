using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveManager
{
    void LoadData(GameData _data);
    void SaveData(ref GameData _data); //ref: cho phép lấy dữ liệu và thay đổi dữ liệu khi được gọi dữ liệu đó
    
}
