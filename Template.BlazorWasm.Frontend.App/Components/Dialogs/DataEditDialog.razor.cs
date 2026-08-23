namespace Template.BlazorWasm.Frontend.App.Components.Dialogs;

using Microsoft.FluentUI.AspNetCore.Components;

using Template.BlazorWasm.Frontend.App.Models;

public partial class DataEditDialog : IDialogContentComponent<DataEditForm>
{
    [Parameter]
    public DataEditForm Content { get; set; } = default!;

    [CascadingParameter]
    public FluentDialog Dialog { get; set; } = default!;

    private async Task OnSaveClickAsync()
    {
        if (String.IsNullOrWhiteSpace(Content.Name))
        {
            return;
        }

        await Dialog.CloseAsync(Content);
    }

    private async Task OnCancelClickAsync()
    {
        await Dialog.CancelAsync();
    }
}
