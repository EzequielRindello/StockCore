using Microsoft.AspNetCore.Mvc.ViewFeatures;

public static class TempDataExtensions
{
    public static void Merge(this ITempDataDictionary tempData, (string key, string message) result)
    {
        tempData[result.key] = result.message;
    }
}
