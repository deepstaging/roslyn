// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class ConfigurationTypesTests
{
    [Test]
    public async Task IConfiguration_produces_correct_type()
    {
        await Assert.That(ConfigurationTypes.IConfiguration.Value)
            .IsEqualTo("global::Microsoft.Extensions.Configuration.IConfiguration");
    }

    [Test]
    public async Task IConfigurationSection_produces_correct_type()
    {
        await Assert.That(ConfigurationTypes.IConfigurationSection.Value)
            .IsEqualTo("global::Microsoft.Extensions.Configuration.IConfigurationSection");
    }

    [Test]
    public async Task IConfigurationRoot_produces_correct_type()
    {
        await Assert.That(ConfigurationTypes.IConfigurationRoot.Value)
            .IsEqualTo("global::Microsoft.Extensions.Configuration.IConfigurationRoot");
    }

    [Test]
    public async Task IConfigurationBuilder_produces_correct_type()
    {
        await Assert.That(ConfigurationTypes.IConfigurationBuilder.Value)
            .IsEqualTo("global::Microsoft.Extensions.Configuration.IConfigurationBuilder");
    }
}
