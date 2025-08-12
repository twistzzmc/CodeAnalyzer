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
    /// Ca
    /// </summary>
    AfferentCoupling,
    
    /// <summary>
    /// TCC
    /// </summary>
    TightClassCohesion,
    
    /// <summary>
    /// WMC
    /// </summary>
    WeightedMethodsPerClass
}