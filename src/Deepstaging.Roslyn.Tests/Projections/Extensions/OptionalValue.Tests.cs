// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Projections.Extensions;

public class OptionalValueTests : RoslynTestBase
{
    [Test]
    public async Task Can_create_with_value()
    {
        var optional = OptionalValue<int>.WithValue(42);

        await Assert.That(optional.HasValue).IsTrue();
        await Assert.That(optional.IsEmpty).IsFalse();
    }

    [Test]
    public async Task Can_create_empty()
    {
        var optional = OptionalValue<int>.Empty();

        await Assert.That(optional.HasValue).IsFalse();
        await Assert.That(optional.IsEmpty).IsTrue();
    }

    [Test]
    public async Task OrNull_returns_value_or_null()
    {
        var withValue = OptionalValue<int?>.WithValue(42);
        var empty = OptionalValue<int?>.Empty();

        await Assert.That(withValue.OrNull()).IsEqualTo(42);
        await Assert.That(empty.OrNull()).IsNull();
    }

    [Test]
    public async Task OrDefault_returns_value_or_default()
    {
        var withValue = OptionalValue<int>.WithValue(42);
        var empty = OptionalValue<int>.Empty();

        await Assert.That(withValue.OrDefault(0)).IsEqualTo(42);
        await Assert.That(empty.OrDefault(99)).IsEqualTo(99);
    }

    [Test]
    public async Task Map_transforms_value()
    {
        var optional = OptionalValue<int>.WithValue(42);
        var mapped = optional.Map(x => x.ToString());

        await Assert.That(mapped.HasValue).IsTrue();
        await Assert.That(mapped.OrDefault("")).IsEqualTo("42");
    }

    [Test]
    public async Task ToEnum_converts_int_to_enum()
    {
        var optional = OptionalValue<int>.WithValue(1);
        
        var asEnum = optional.ToEnum<DayOfWeek>();

        await Assert.That(asEnum.HasValue).IsTrue();
        await Assert.That(asEnum.Value).IsEqualTo(DayOfWeek.Monday);
    }

    [Test]
    public async Task Optional_values_are_equatable()
    {
        var opt1 = OptionalValue<int>.WithValue(42);
        var opt2 = OptionalValue<int>.WithValue(42);
        var opt3 = OptionalValue<int>.WithValue(99);
        var empty = OptionalValue<int>.Empty();

        await Assert.That(opt1.Equals(opt2)).IsTrue();
        await Assert.That(opt1.Equals(opt3)).IsFalse();
        await Assert.That(opt1.Equals(empty)).IsFalse();
    }

    [Test]
    public async Task Match_executes_correct_branch()
    {
        var withValue = OptionalValue<int>.WithValue(42);
        var empty = OptionalValue<int>.Empty();

        var withValueResult = 0;
        var emptyResult = 0;

        withValue.Match(v => withValueResult = v, () => withValueResult = -1);
        empty.Match(v => emptyResult = v, () => emptyResult = -1);

        await Assert.That(withValueResult).IsEqualTo(42);
        await Assert.That(emptyResult).IsEqualTo(-1);
    }
}
