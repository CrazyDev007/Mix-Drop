using NUnit.Framework;

[TestFixture]
public class LevelDataTests
{
    private LevelData levelData;

    [SetUp]
    public void SetUp()
    {
        levelData = new LevelData
        {
            Name = "Test Level",
            Objects = new List<PlaceableObject>(),
            Configuration = new LevelConfiguration()
        };
    }

    [Test]
    public void LevelData_Name_ShouldBeSetCorrectly()
    {
        Assert.AreEqual("Test Level", levelData.Name);
    }

    [Test]
    public void LevelData_Objects_ShouldBeInitializedEmpty()
    {
        Assert.IsNotNull(levelData.Objects);
        Assert.IsEmpty(levelData.Objects);
    }

    [Test]
    public void LevelData_Configuration_ShouldNotBeNull()
    {
        Assert.IsNotNull(levelData.Configuration);
    }

    [Test]
    public void LevelData_AddObject_ShouldIncreaseObjectCount()
    {
        var obj = new PlaceableObject();
        levelData.Objects.Add(obj);
        Assert.AreEqual(1, levelData.Objects.Count);
    }
}