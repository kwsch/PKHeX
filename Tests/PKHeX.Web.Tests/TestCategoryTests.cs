using System.Reflection;
using Xunit;

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
        foreach (var type in typeof(TestCategoryTests).Assembly.GetTypes())
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                // TheoryAttribute derives from FactAttribute.
                if (method.GetCustomAttribute<FactAttribute>() is null)
                {
                    continue;
                }

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
        }

        Assert.True(tests > 0, "No test methods were found.");
        Assert.True(problems.Count == 0, $"Each test needs exactly one of {string.Join("/", Known)}:{Environment.NewLine}{string.Join(Environment.NewLine, problems)}");
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
