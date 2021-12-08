using UnityEngine;

/// <summary>
/// Class for sample use
/// </summary>
[System.Serializable]
public class TestData
{
    public int data = 10;
    public string moreData = "xDddd";
    public TestData2[] datas = new[] { new TestData2(), new TestData2(), new TestData2() };
}
