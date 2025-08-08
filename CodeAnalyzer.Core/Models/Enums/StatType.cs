namespace CodeAnalyzer.Core.Models.Enums;

public enum StatType
{
    /// <summary>
    /// ATFD
    /// </summary>
    AccessToFieldData,
    
    /// <summary>
    /// CBO
    /// </summary>
    CouplingBetweenObjects,
    
    /// <summary>
    /// FanIn
    /// </summary>
    AfferentCoupling,
    
    /// <summary>
    /// TCC
    /// </summary>
    TightClassCohesion,
    
    /// <summary>
    /// WMPC
    /// </summary>
    WeightedMethodsPerClass
}