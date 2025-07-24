namespace Template.BlazorWasm;

using Template.BlazorWasm.Backend.Components.Storage;

public class DummyTest
{
    [Fact]
    public void Test()
    {
        var storage = new FileStorage(new FileStorageOptions { Root = "." });
        Assert.NotNull(storage);
    }
}
