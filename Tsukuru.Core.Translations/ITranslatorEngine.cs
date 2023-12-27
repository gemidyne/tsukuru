namespace Tsukuru.Core.Translations;

public interface ITranslatorEngine
{
    EProjectGenerateResult ImportFromSourceMod(string translationFileName);
    void ExportToSourceMod(string translationProjectFileName);
}