namespace CodeAnalyzer.Core.Interfaces;

public interface IJoinable<TOther> where TOther : IJoinable<TOther>
{
    TOther Join(TOther other);
}