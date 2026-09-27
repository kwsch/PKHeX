using System.Reflection;
using Xunit;
using Xunit.Sdk;

namespace PKHeX.Web.Tests;

/// <summary>
/// Guards the tier split: runs select tests by <see cref="TestCategory"/>, so a test without exactly one known tier would silently never run.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class TestCategoryTests
{
    private static readonly string[] Known = [TestCategory.Unit, TestCategory.E2E, TestCategory.RealSave];

    [Fact]
    public void EveryTestHasExactlyOneKnownCategory()
    {
        var problems = new List<string>();
        var tests = 0;
        foreach (var (type, method) in TestMethods())
        {
            // xUnit merges class-level and method-level traits.
            tests++;
            var categories = GetCategories(type.GetCustomAttributesData())
                .Concat(GetCategories(method.GetCustomAttributesData()))
                .ToArray();
            if (categories.Length != 1 || !Known.Contains(categories[0]))
            {
                problems.Add($"{type.Name}.{method.Name}: [{string.Join(", ", categories)}]");
            }
        }

        Assert.True(tests > 0, "No test methods were found.");
        Assert.True(problems.Count == 0, $"Each test needs exactly one of {string.Join("/", Known)}:{Environment.NewLine}{string.Join(Environment.NewLine, problems)}");
    }

    /// <summary>
    /// Opt-in tiers are skipped only through <see cref="TierFactAttribute"/>/<see cref="TierTheoryAttribute"/>, so their tier must match the class's category.
    /// Unit tests must use exactly <see cref="FactAttribute"/>/<see cref="TheoryAttribute"/> so that they always run; any other test attribute is rejected,
    /// because a custom subclass could skip without either guard noticing.
    /// </summary>
    [Fact]
    public void OptInTiersUseTheMatchingTierAttribute()
    {
        var problems = new List<string>();
        foreach (var (type, method) in TestMethods())
        {
            var category = GetCategories(type.GetCustomAttributesData()).Concat(GetCategories(method.GetCustomAttributesData())).FirstOrDefault();
            var attribute = method.GetCustomAttribute<FactAttribute>()!;
            string? tier = attribute switch
            {
                TierFactAttribute f => f.Tier,
                TierTheoryAttribute t => t.Tier,
                _ when attribute.GetType() == typeof(FactAttribute) || attribute.GetType() == typeof(TheoryAttribute) => TestCategory.Unit,
                _ => null,
            };
            if (tier is null)
            {
                problems.Add($"{type.Name}.{method.Name}: unsupported test attribute {attribute.GetType().Name}");
            }
            else if (tier != category)
            {
                problems.Add($"{type.Name}.{method.Name}: category {category}, attribute tier {tier}");
            }
        }

        Assert.True(problems.Count == 0, $"Use [Fact]/[Theory] for Unit and [TierFact]/[TierTheory] with the class's category otherwise:{Environment.NewLine}{string.Join(Environment.NewLine, problems)}");
    }

    /// <summary>
    /// An opted-in tier fails when its inputs are missing (see <see cref="TestEnvironment.Required"/>); any other skip would hide that.
    /// The only skip allowed is the tier attributes' own <see cref="TestEnvironment.SkipUnlessOptedIn"/> reason; data rows may not skip at all.
    /// Skips are read from the attribute instances, so skips set in a constructor are found as well as <c>Skip = ...</c> arguments.
    /// </summary>
    [Fact]
    public void NoTestIsSkippedExceptByTheOptIn()
    {
        var skipped = new List<string>();
        foreach (var (type, method) in TestMethods())
        {
            var attribute = method.GetCustomAttribute<FactAttribute>()!;
            var allowed = attribute switch
            {
                TierFactAttribute f => TestEnvironment.SkipUnlessOptedIn(f.Tier),
                TierTheoryAttribute t => TestEnvironment.SkipUnlessOptedIn(t.Tier),
                _ => null,
            };
            if (attribute.Skip != allowed || method.GetCustomAttributes<DataAttribute>().Any(d => !string.IsNullOrEmpty(d.Skip)))
            {
                skipped.Add($"{type.Name}.{method.Name}");
            }
        }

        Assert.True(skipped.Count == 0, $"Tests must fail rather than skip:{Environment.NewLine}{string.Join(Environment.NewLine, skipped)}");
    }

    [Theory]
    [InlineData(TestCategory.Unit, null, true)]
    [InlineData(TestCategory.Unit, "E2E", true)]
    [InlineData(TestCategory.E2E, null, false)]
    [InlineData(TestCategory.E2E, "", false)]
    [InlineData(TestCategory.E2E, "E2E", true)]
    [InlineData(TestCategory.E2E, " e2e ; RealSave ", true)]
    [InlineData(TestCategory.RealSave, "E2E,RealSave", true)]
    [InlineData(TestCategory.RealSave, "E2E RealSave", true)]
    [InlineData(TestCategory.E2E, "E2EX,Real", false)]
    [InlineData(TestCategory.RealSave, "E2E", false)]
    public void TiersAreOptInAndUnitAlwaysRuns(string tier, string? optIn, bool expected) => Assert.Equal(expected, TestEnvironment.IsOptedIn(tier, optIn));

    /// <summary>Every <see cref="FactAttribute"/> (including <see cref="TheoryAttribute"/>) method in this assembly.</summary>
    private static IEnumerable<(Type Type, MethodInfo Method)> TestMethods()
    {
        foreach (var type in typeof(TestCategoryTests).Assembly.GetTypes())
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                if (method.GetCustomAttribute<FactAttribute>() is not null)
                {
                    yield return (type, method);
                }
            }
        }
    }

    /// <summary>
    /// Reads <see cref="TestCategory.Name"/> trait values; xUnit v2's <see cref="TraitAttribute"/> keeps them only as constructor arguments.
    /// </summary>
    private static IEnumerable<string> GetCategories(IEnumerable<CustomAttributeData> attributes)
    {
        foreach (var attribute in attributes)
        {
            if (attribute.AttributeType != typeof(TraitAttribute) || attribute.ConstructorArguments.Count != 2)
            {
                continue;
            }

            if (attribute.ConstructorArguments[0].Value is TestCategory.Name && attribute.ConstructorArguments[1].Value is string value)
            {
                yield return value;
            }
        }
    }
}
