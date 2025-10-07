using System.Collections.Generic;
using KQLAnalyzer;

public static class EnvironmentUtils
{
    /// <summary>
    /// If both m365 and sentinel environments exist, create a merged environment m365_with_sentinel.
    /// Adds the merged environment to the dictionary if applicable.
    /// </summary>
    /// <param name="kqlEnvironments">The environments dictionary to update.</param>
    public static void AddM365WithSentinelIfPresent(IDictionary<string, EnvironmentDefinition> kqlEnvironments)
    {
        if (kqlEnvironments.ContainsKey("m365") && kqlEnvironments.ContainsKey("sentinel"))
        {
            var m365 = kqlEnvironments["m365"];
            var sentinel = kqlEnvironments["sentinel"];

            var merged = new EnvironmentDefinition
            {
                TableDetails = new Dictionary<string, TableDetails>(),
                MagicFunctions = new List<string>(m365.MagicFunctions)
            };

            // Deep copy tables from m365
            foreach (var tableEntry in m365.TableDetails)
            {
                var copiedTableDetails = new TableDetails();
                foreach (var columnEntry in tableEntry.Value)
                {
                    copiedTableDetails[columnEntry.Key] = columnEntry.Value;
                }

                merged.TableDetails[tableEntry.Key] = copiedTableDetails;
            }

            // Add tables from sentinel if not already present (no overwrite)
            foreach (var tableEntry in sentinel.TableDetails)
            {
                if (!merged.TableDetails.ContainsKey(tableEntry.Key))
                {
                    var copiedTableDetails = new TableDetails();
                    foreach (var columnEntry in tableEntry.Value)
                    {
                        copiedTableDetails[columnEntry.Key] = columnEntry.Value;
                    }

                    merged.TableDetails[tableEntry.Key] = copiedTableDetails;
                }
            }

            kqlEnvironments["m365_with_sentinel"] = merged;
        }
    }
}