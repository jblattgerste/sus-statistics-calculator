# System Usability Scale Statistics Calculator

The [SUS Statistics Calculator](https://statistics.sus.tools) is part of the [SUS Analysis Toolkit](https://sus.tools) and provides the utility to calculate inferential statistics for System Usability Scale studies with dependent and independent samples.

It is written in C#/Razor using the .NET 8 Blazor Webassembly framework. Therefore, it can be hosted on GitHub pages as a static site and runs "client-side-only", entirely in the users browser using the WASM .NET runtime. This has the advantage that it does not send any data "loaded" into the tool to any server.

The statistical calculations are fully implemented in C# and mostly based on the mono version of Accord.NET, with some additional tests and metrics being custom implementations.

![SUSStatisticsCalculatorMockup](https://github.com/user-attachments/assets/ae0a4ce1-fd4d-4001-a70e-ec6606ceecfc)


## Statistical Tests of the SUS Statistics Calculator

All descriptive statistics for the calculation of the SUS study scores can be found in the [SUSStudy.cs](Statistics/Services/SUSStudy.cs) class.

For the decision assistance on the question if parametric or non-parametric tests should be used, Levenes Test and Shapiro-Wilk tests are used:
| Implemented | Test (Implementation source)             | Description                                    |
|---|--------------------------------------------|------------------------------------------|
| ✅ | Levene's Test [(Accord.NET)](http://accord-framework.net/docs/html/T_Accord_Statistics_Testing_LeveneTest.htm) | Assessment of the equality of variances for two or more groups. |
| ✅ | Shapiro-Wilk Test [(Accord.NET)](http://accord-framework.net/docs/html/T_Accord_Statistics_Testing_ShapiroWilkTest.htm) | Checking the normality of a distribution for a given sample. |

The statistical test used depends on the number of samples the user wants to compare, if they are independent or dependent samples, and if the user wants to use a parametric or non-parametric test. This results in 8 possible tests:
| Implemented | Test (Implementation source)          | Samples | Dependence | Parametric       |
|-----------------|-----------------------------------------|-----------|------------|------------------|
| ✅ | Independent Samples t-test [(Accord.NET)](http://accord-framework.net/docs/html/T_Accord_Statistics_Testing_TwoSampleTTest.htm) | 2         | Independent | Parametric        |
| ✅ | Mann-Whitney U Test [(Accord.NET)](http://accord-framework.net/docs/html/T_Accord_Statistics_Testing_MannWhitneyWilcoxonTest.htm) | 2         | Independent | Non-Parametric    |
| ✅ | Paired t-test [(Accord.NET)](http://accord-framework.net/docs/html/T_Accord_Statistics_Testing_TTest.htm) | 2         | Dependent   | Parametric        |
| ✅ | Wilcoxon Signed-Rank Test [(Accord.NET)](http://accord-framework.net/docs/html/T_Accord_Statistics_Testing_WilcoxonSignedRankTest.htm) | 2         | Dependent   | Non-Parametric    |
| ✅ | One-way ANOVA [(Accord.NET)](http://accord-framework.net/docs/html/T_Accord_Statistics_Testing_OneWayAnova.htm) | n         | Independent | Parametric        |
| ❌ | Kruskal-Wallis H Test [(Metanumerics)](https://github.com/dcwuser/metanumerics/blob/master/Numerics/Statistics/Sample.cs) or [(F#.Stats)](https://fslab.org/FSharp.Stats/Testing.html#H-Test)* | n         | Independent | Non-Parametric    |
| ❌ | Repeated Measures ANOVA (?) | n         | Dependent   | Parametric    |
| ❌ | Friedman Test [(F#.Stats)](https://fslab.org/FSharp.Stats/Testing.html#Friedman-Test)* | n         | Dependent   | Non-Parametric    |

Notes: Tests marked with * are currently work in progress. For one, Accord.NET does not provide them to be implemented through their statistics library, and furthermore they are slightly more complicated to get right because of assumptions, corrections and post-hoc tests. So I will try to focus on the most common tests for now to make sure they are working as expected. Furthermore, while i do find e.g. a Kruskal-Wallis H Test in C# and both the Kruskal-Wallis and Friedman test through F# libraries, at least for the Reapeated Measures ANOVA I cant find any libraries at all so it might be necessary to fully implement it from scratch.

## Limitations / Work in progress

- I am currently not fully happy with the way raw SUS questionnaire data is provided to the tool. The current way is fully compatible with how data is provided to the [sus-analysis-toolkit](https://github.com/jblattgerste/sus-analysis-toolkit) component and you can provide data implicitly in the same order for dependent samples so there is (Variable A for P1, Variable A for P2, Variable A, ..., Variable B for P1, Variable B for P2, Variable B for P3, ...) or (Variable A for P1, Variable B for P1, Variable A for P2, Variable B for P2, Variable A for P3, Variable B for P3, ...) but this might be error prone if the user is not careful for dependent samples hypothesis testing.
- We could probably easily get [routing parameters](https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/routing?view=aspnetcore-8.0#route-parameters) to work in Blazor so that the tool can be called diretly with all data as the parameter. This way we could better connect it to the analysis component.
- Currently, the [build pipeline](.github/workflows/deploy.yml) copies the index.html into the wwwroot as the 404.html. This way 404 errors are avoided, rerouted to Blazor and the "correct" routing is applied if the page exists. The problem for this current implementation specifically is, that this leads to an error on some pages as the data used on the pages is deleted when refreshing/reloading a page.
- There is probably some edge cases which i did not fully cover. E.g. one I know of: if two samples have the exact same pre/post measures and we do a Shapiro-Wilk test on the residuals for repeated measures, this crashes the application because the impossibility to do that is not properly handled by the ShapiroWilkTestCalculator service. If you find something like this that could actually happen with non-mock data, open an issue and I will take a look.

## Acknowledgement
The tool itself and its source code is freely accesible for commercial and non-commercial use under the MIT license and does not require acknowledgement. If you use the SUS Statistics Calculator for scientific purposes, an acknowledgement in form of a citation to the SUS Analysis Toolkit would be appreciated:

```tex
@inproceedings{10.1145/3529190.3529216,
author = {Blattgerste, Jonas and Behrends, Jan and Pfeiffer, Thies},
title = {A Web-Based Analysis Toolkit for the System Usability Scale},
year = {2022},
isbn = {9781450396318},
publisher = {Association for Computing Machinery},
address = {New York, NY, USA},
url = {https://doi.org/10.1145/3529190.3529216},
doi = {10.1145/3529190.3529216},
pages = {237–246},
numpages = {10},
location = {Corfu, Greece},
series = {PETRA '22}
}
```
