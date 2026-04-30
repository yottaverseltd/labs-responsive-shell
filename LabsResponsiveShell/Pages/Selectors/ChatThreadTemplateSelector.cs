using LabsResponsiveShell.Data;

namespace LabsResponsiveShell.Pages.Selectors;

public sealed class ChatThreadTemplateSelector : DataTemplateSelector
{
    public DataTemplate? DateTemplate { get; set; }

    public DataTemplate? TextMineTemplate { get; set; }

    public DataTemplate? TextTheirTemplate { get; set; }

    public DataTemplate? LinkTemplate { get; set; }

    public DataTemplate? ImageTemplate { get; set; }

    public DataTemplate? VoiceTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item)
    {
        if (item is not MockThreadItem m)
        {
            return new DataTemplate(); // unreachable UI guard
        }

        return m.Kind switch
        {
            MockThreadKind.DateSeparator => DateTemplate ?? new DataTemplate(),
            MockThreadKind.Text => m.IsMine
                ? TextMineTemplate ?? new DataTemplate()
                : TextTheirTemplate ?? new DataTemplate(),
            MockThreadKind.LinkPreview => LinkTemplate ?? new DataTemplate(),
            MockThreadKind.ImagePlaceholder => ImageTemplate ?? new DataTemplate(),
            MockThreadKind.VoiceStub => VoiceTemplate ?? new DataTemplate(),
            _ => TextTheirTemplate ?? new DataTemplate(),
        };
    }
}
