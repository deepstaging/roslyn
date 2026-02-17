// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Extensions;

public class StringExtensionsTests
{
    #region ToBackingFieldName Tests

    [Test]
    [Arguments("Name", "_name")]
    [Arguments("MyProperty", "_myProperty")]
    [Arguments("ID", "_id")]
    [Arguments("XMLParser", "_xMLParser")]
    [Arguments("_existing", "_existing")]
    [Arguments("value", "_value")]
    public async Task ToBackingFieldName_converts_correctly(string input, string expected)
    {
        var result = input.ToBackingFieldName();
        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task ToBackingFieldName_handles_empty_string()
    {
        var result = "".ToBackingFieldName();
        await Assert.That(result).IsEqualTo("");
    }

    #endregion

    #region ToParameterName Tests

    [Test]
    [Arguments("Name", "name")]
    [Arguments("MyProperty", "myProperty")]
    [Arguments("_backingField", "backingField")]
    [Arguments("ID", "id")]
    [Arguments("value", "value")]
    public async Task ToParameterName_converts_correctly(string input, string expected)
    {
        var result = input.ToParameterName();
        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task ToParameterName_handles_empty_string()
    {
        var result = "".ToParameterName();
        await Assert.That(result).IsEqualTo("");
    }

    #endregion

    #region ToPropertyName Tests

    [Test]
    [Arguments("name", "Name")]
    [Arguments("myProperty", "MyProperty")]
    [Arguments("_backingField", "BackingField")]
    [Arguments("id", "Id")]
    [Arguments("Name", "Name")]
    public async Task ToPropertyName_converts_correctly(string input, string expected)
    {
        var result = input.ToPropertyName();
        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task ToPropertyName_handles_empty_string()
    {
        var result = "".ToPropertyName();
        await Assert.That(result).IsEqualTo("");
    }

    #endregion

    #region ToConstantName Tests

    [Test]
    [Arguments("maxValue", "MAX_VALUE")]
    [Arguments("MyConstant", "MY_CONSTANT")]
    [Arguments("HTTP_STATUS", "HTTP_STATUS")]
    [Arguments("xmlParser", "XML_PARSER")]
    [Arguments("ID", "ID")]
    public async Task ToConstantName_converts_correctly(string input, string expected)
    {
        var result = input.ToConstantName();
        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task ToConstantName_handles_empty_string()
    {
        var result = "".ToConstantName();
        await Assert.That(result).IsEqualTo("");
    }

    #endregion

    #region ToInterfaceName Tests

    [Test]
    [Arguments("Comparable", "IComparable")]
    [Arguments("equatable", "IEquatable")]
    [Arguments("IComparable", "IComparable")]
    [Arguments("IEquatable", "IEquatable")]
    [Arguments("disposable", "IDisposable")]
    public async Task ToInterfaceName_converts_correctly(string input, string expected)
    {
        var result = input.ToInterfaceName();
        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task ToInterfaceName_handles_empty_string()
    {
        var result = "".ToInterfaceName();
        await Assert.That(result).IsEqualTo("");
    }

    #endregion
}